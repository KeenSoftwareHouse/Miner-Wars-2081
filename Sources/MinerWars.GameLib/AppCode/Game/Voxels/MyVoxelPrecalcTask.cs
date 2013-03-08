using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Threading;
using MinerWars.AppCode.App;

//  This class is used for precalculation of voxels into triangles and vertex buffers
//  It is not static and not thread too. But it may be called from thread.
//  This class doesn't know if it is called from multiple threads or just from main thread.

namespace MinerWars.AppCode.Game.Voxels
{
    class MyVoxelPrecalcTask
    {
        //  Here I store vertex indices for final triangles
        class MyEdgeVertex
        {
            public short VertexIndex;             //  If this vertex is in the list, this is its m_notCompressedIndex 
            public int CalcCounter;             //  For knowing if edge vertex was calculated in this PrecalcImmediatelly() or one of previous
        }

        //  Here I store data for edges on marching cube
        class MyEdge
        {
            public Vector3 Position;
            public Vector3 Normal;
            public float Ambient;
            public MyMwcVoxelMaterialsEnum Material;
        }

        //class MyShadowCell
        //{
        //    public float ShadowValue;
        //    public MyMwcVector3Int CoordOfDataCellCenter;
        //}

        //  Temporary voxel values, serve as cache between precalc and voxel map - so we don't have to always access voxel maps but can look here
        class MyTemporaryVoxel
        {
            public Vector3 Position;

            public byte Content;
            public MyMwcVoxelMaterialsEnum Material;
            public Vector3 Normal;
            public float Ambient;

            public int Content_CalcCounter;
            public int Material_CalcCounter;
            public int Normal_CalcCounter;
            public int Ambient_CalcCounter;

            public MyTemporaryVoxel()
            {
                //  Reset counters
                Content_CalcCounter = 0;
                Material_CalcCounter = 0;
                Normal_CalcCounter = 0;
                Ambient_CalcCounter = 0;
            }
        }

        //  Array of vectors. Used for temporary storing interpolated vertexes on cube edges.
        const int POLYCUBE_EDGES = 12;
        MyEdge[] m_edges = new MyEdge[POLYCUBE_EDGES];

        //  Array of edges in the cell
        const int CELL_EDGES_SIZE = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + 1;
        MyEdgeVertex[][][][] m_edgeVertex;
        int m_edgeVertexCalcCounter;

        //  Here we store calculated vertexes and vertex info
        readonly MyVoxelVertex[] m_resultVertices = new MyVoxelVertex[MyVoxelConstants.MAX_TRIANGLES_COUNT_IN_VOXEL_DATA_CELL * 3];
        short m_resultVerticesCounter;

        // Index buffer
        readonly MyVoxelTriangle[] m_resultTriangles = new MyVoxelTriangle[MyVoxelConstants.MAX_TRIANGLES_COUNT_IN_VOXEL_DATA_CELL];
        int m_resultTrianglesCounter;

        //  This variables are set every time PrecalcImmediatelly() is called
        int m_polygCubesX;
        int m_polygCubesY;
        int m_polygCubesZ;
        MyMwcVector3Int m_voxelStart;
        MyVoxelMap m_voxelMap;

        //  Here we store voxel content values from cell we are precalculating. So we don't need to call VoxelMap.GetVoxelContent during precalculation.
        //  Items in array are nullable because we want to know, if we already retrieved/calculated that voxel/normal during current precalculation.
        const int COPY_TABLE_SIZE = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + 3;
        int m_temporaryVoxelsCounter = 0;
        MyTemporaryVoxel[][][] m_temporaryVoxels;

        //  Each voxel triangleVertexes has unique ID in whole game (but only on client side)
        //int m_triangleIdCounter = 0;

        MyLodTypeEnum m_precalcType;
        MyMwcVector3Int m_sizeMinusOne;


        public MyVoxelPrecalcTask()
        {
            //  Cube Edges
            for (int i = 0; i < m_edges.Length; i++)
            {
                m_edges[i] = new MyEdge();
            }

            //  Temporary voxel values, serve as cache between precalc and voxel map - so we don't have to always access voxel maps but can look here
            m_temporaryVoxels = new MyTemporaryVoxel[COPY_TABLE_SIZE][][];
            for (int x = 0; x < COPY_TABLE_SIZE; x++)
            {
                m_temporaryVoxels[x] = new MyTemporaryVoxel[COPY_TABLE_SIZE][];
                for (int y = 0; y < COPY_TABLE_SIZE; y++)
                {
                    m_temporaryVoxels[x][y] = new MyTemporaryVoxel[COPY_TABLE_SIZE];
                    for (int z = 0; z < COPY_TABLE_SIZE; z++)
                    {
                        m_temporaryVoxels[x][y][z] = new MyTemporaryVoxel();
                    }
                }
            }

            //  Array of edges in the cell
            m_edgeVertexCalcCounter = 0;
            m_edgeVertex = new MyEdgeVertex[CELL_EDGES_SIZE][][][];
            for (int x = 0; x < CELL_EDGES_SIZE; x++)
            {
                m_edgeVertex[x] = new MyEdgeVertex[CELL_EDGES_SIZE][][];
                for (int y = 0; y < CELL_EDGES_SIZE; y++)
                {
                    m_edgeVertex[x][y] = new MyEdgeVertex[CELL_EDGES_SIZE][];
                    for (int z = 0; z < CELL_EDGES_SIZE; z++)
                    {
                        m_edgeVertex[x][y][z] = new MyEdgeVertex[MyVoxelPrecalcConstants.CELL_EDGE_COUNT];
                        for (int w = 0; w < MyVoxelPrecalcConstants.CELL_EDGE_COUNT; w++)
                        {
                            m_edgeVertex[x][y][z][w] = new MyEdgeVertex();
                            m_edgeVertex[x][y][z][w].CalcCounter = 0;
                        }
                    }
                }
            }
        }

        //  Copy voxels from voxel map into our temporary array. If TRUE returned, we don't have to calculate rest because this cell plus its surroinding is empy or full, thus there aren't any triangles
        bool CopyVoxelContents()
        {
            //  Increase counter for temp voxels
            m_temporaryVoxelsCounter++;

            //  This is return value
            bool everythingFull = true;
            bool everythingEmpty = true;

            if (m_precalcType == MyLodTypeEnum.LOD0)
            {
                //  This is cell we are trying to precalculate            
                MyMwcVector3Int precalculatedCellCoord = m_voxelMap.GetDataCellCoordinate(ref m_voxelStart);

                //  Iterate over "our" cell and all its neighbouring cells. That is total eight cells, including "our" cell
                const int NEIGHBOURING_CELL_IN_DIRECTION = 1;
                MyMwcVector3Int tempNeighCell;
                for (tempNeighCell.X = 0; tempNeighCell.X <= NEIGHBOURING_CELL_IN_DIRECTION; tempNeighCell.X++)
                {
                    for (tempNeighCell.Y = 0; tempNeighCell.Y <= NEIGHBOURING_CELL_IN_DIRECTION; tempNeighCell.Y++)
                    {
                        for (tempNeighCell.Z = 0; tempNeighCell.Z <= NEIGHBOURING_CELL_IN_DIRECTION; tempNeighCell.Z++)
                        {
                            //  Ignore data cells that are outisde the voxel map
                            MyMwcVector3Int neighCell = new MyMwcVector3Int(precalculatedCellCoord.X + tempNeighCell.X, precalculatedCellCoord.Y + tempNeighCell.Y, precalculatedCellCoord.Z + tempNeighCell.Z);
                            if ((neighCell.X >= m_voxelMap.DataCellsCount.X) || (neighCell.Y >= m_voxelMap.DataCellsCount.Y) || (neighCell.Z >= m_voxelMap.DataCellsCount.Z)) continue;

                            //  How many voxels to copy from this data cell (all voxels or just neighbouring voxels)
                            MyMwcVector3Int cellSizeToCopy;
                            cellSizeToCopy.X = (tempNeighCell.X == 0) ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : 1;
                            cellSizeToCopy.Y = (tempNeighCell.Y == 0) ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : 1;
                            cellSizeToCopy.Z = (tempNeighCell.Z == 0) ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : 1;

                            //  This particular cell (precalculated or one of the neighbouring)
                            MyVoxelContentCell voxelCell = m_voxelMap.GetCell(ref neighCell);

                            MyMwcVector3Int voxelStart = new MyMwcVector3Int(tempNeighCell.X * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS, tempNeighCell.Y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS, tempNeighCell.Z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);

                            MyMwcVector3Int voxelCoord;
                            for (voxelCoord.X = 0; voxelCoord.X < cellSizeToCopy.X; voxelCoord.X++)
                            {
                                for (voxelCoord.Y = 0; voxelCoord.Y < cellSizeToCopy.Y; voxelCoord.Y++)
                                {
                                    for (voxelCoord.Z = 0; voxelCoord.Z < cellSizeToCopy.Z; voxelCoord.Z++)
                                    {
                                        MyTemporaryVoxel tempVoxel = m_temporaryVoxels[voxelCoord.X + voxelStart.X + 1][voxelCoord.Y + voxelStart.Y + 1][voxelCoord.Z + voxelStart.Z + 1];
                                        tempVoxel.Content = (voxelCell == null) ? MyVoxelConstants.VOXEL_CONTENT_FULL : voxelCell.GetVoxelContent(ref voxelCoord);

                                        if (tempVoxel.Content > MyVoxelConstants.VOXEL_CONTENT_EMPTY) everythingEmpty = false;
                                        if (tempVoxel.Content < MyVoxelConstants.VOXEL_CONTENT_FULL) everythingFull = false;

                                        tempVoxel.Content_CalcCounter = m_temporaryVoxelsCounter;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (m_precalcType == MyLodTypeEnum.LOD1)
            {
                MyMwcVector3Int dataCellCoord;
                for (dataCellCoord.X = 0; dataCellCoord.X < m_polygCubesX; dataCellCoord.X++)
                {
                    for (dataCellCoord.Y = 0; dataCellCoord.Y < m_polygCubesY; dataCellCoord.Y++)
                    {
                        for (dataCellCoord.Z = 0; dataCellCoord.Z < m_polygCubesZ; dataCellCoord.Z++)
                        {
                            MyTemporaryVoxel tempVoxel = m_temporaryVoxels[dataCellCoord.X + 1][dataCellCoord.Y + 1][dataCellCoord.Z + 1];

                            MyMwcVector3Int tempDataCellCoord = new MyMwcVector3Int(m_voxelStart.X + dataCellCoord.X, m_voxelStart.Y + dataCellCoord.Y, m_voxelStart.Z + dataCellCoord.Z);
                            tempVoxel.Content = m_voxelMap.GetDataCellAverageContent(ref tempDataCellCoord);

                            if (tempVoxel.Content > MyVoxelConstants.VOXEL_CONTENT_EMPTY) everythingEmpty = false;
                            if (tempVoxel.Content < MyVoxelConstants.VOXEL_CONTENT_FULL) everythingFull = false;

                            tempVoxel.Content_CalcCounter = m_temporaryVoxelsCounter;
                        }
                    }
                }

            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            return (everythingEmpty == true) || (everythingFull == true);
        }

        //  Size of cube depends on if we are at the end of the map
        void CalcPolygCubeSize()
        {
            if (m_precalcType == MyLodTypeEnum.LOD0)
            {
                m_polygCubesX = (m_voxelStart.X + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS) >= m_voxelMap.Size.X ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + 1;
                m_polygCubesY = (m_voxelStart.Y + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS) >= m_voxelMap.Size.Y ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + 1;
                m_polygCubesZ = (m_voxelStart.Z + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS) >= m_voxelMap.Size.Z ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + 1;
            }
            else if (m_precalcType == MyLodTypeEnum.LOD1)
            {
                m_polygCubesX = (m_voxelStart.X + MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE) >= m_voxelMap.DataCellsCount.X ? MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE : MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE + 1;
                m_polygCubesY = (m_voxelStart.Y + MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE) >= m_voxelMap.DataCellsCount.Y ? MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE : MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE + 1;
                m_polygCubesZ = (m_voxelStart.Z + MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE) >= m_voxelMap.DataCellsCount.Z ? MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE : MyVoxelConstants.VOXEL_DATA_CELLS_IN_RENDER_CELL_SIZE + 1;
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        //  Get material from lookup table or calc it
        void GetVoxelMaterial(MyTemporaryVoxel temporaryVoxel, ref MyMwcVector3Int voxelCoord)
        {
            if (temporaryVoxel.Material_CalcCounter != m_temporaryVoxelsCounter)
            {
                if (m_precalcType == MyLodTypeEnum.LOD0)
                {
                    temporaryVoxel.Material = m_voxelMap.GetVoxelMaterial(ref voxelCoord);
                }
                else if (m_precalcType == MyLodTypeEnum.LOD1)
                {
                    temporaryVoxel.Material = m_voxelMap.GetDataCellAverageMaterial(ref voxelCoord);
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }
                temporaryVoxel.Material_CalcCounter = m_temporaryVoxelsCounter;
            }
        }

        //  IMPORTANT: This method is called only from GetVoxelNormal(). Reason is that border voxels aren't copied during CopyVoxelContents, so we can check it now.
        byte GetVoxelContent(int x, int y, int z)
        {
            MyTemporaryVoxel temporaryVoxel = m_temporaryVoxels[x][y][z];

            if (temporaryVoxel.Content_CalcCounter != m_temporaryVoxelsCounter)
            {
                //  If this requested voxel wasn't copied during CopyVoxelContents, we need to get him now
                MyMwcVector3Int tempVoxelCoord = new MyMwcVector3Int(m_voxelStart.X + x - 1, m_voxelStart.Y + y - 1, m_voxelStart.Z + z - 1);

                if (m_precalcType == MyLodTypeEnum.LOD0)
                {
                    temporaryVoxel.Content = m_voxelMap.GetVoxelContent(ref tempVoxelCoord);
                }
                else if (m_precalcType == MyLodTypeEnum.LOD1)
                {
                    temporaryVoxel.Content = m_voxelMap.GetDataCellAverageContent(ref tempVoxelCoord);
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }

                temporaryVoxel.Content_CalcCounter = m_temporaryVoxelsCounter;
            }

            return temporaryVoxel.Content;
        }


        bool IsOutside(int coord, int maxMinusOne)
        {
            if (coord <= 0) return true;
            if (coord >= maxMinusOne) return true;

            return false;
        }


        //  IMPORTANT: This method is called only from GetVoxelNormal(). Reason is that border voxels aren't copied during CopyVoxelContents, so we can check it now.
        // This is test  method which handles also borders of voxels
        byte GetVoxelContent2(int x, int y, int z)
        {
            if (IsOutside(x, m_sizeMinusOne.X))
                return MyVoxelConstants.VOXEL_CONTENT_EMPTY;

            if (IsOutside(y, m_sizeMinusOne.Y))
                return MyVoxelConstants.VOXEL_CONTENT_EMPTY;

            if (IsOutside(z, m_sizeMinusOne.Z))
                return MyVoxelConstants.VOXEL_CONTENT_EMPTY;

            MyTemporaryVoxel temporaryVoxel = m_temporaryVoxels[x][y][z];

            if (temporaryVoxel.Content_CalcCounter != m_temporaryVoxelsCounter)
            {
                //  If this requested voxel wasn't copied during CopyVoxelContents, we need to get him now
                MyMwcVector3Int tempVoxelCoord = new MyMwcVector3Int(m_voxelStart.X + x - 1, m_voxelStart.Y + y - 1, m_voxelStart.Z + z - 1);

                if (m_precalcType == MyLodTypeEnum.LOD0)
                {
                    temporaryVoxel.Content = m_voxelMap.GetVoxelContent(ref tempVoxelCoord);
                }
                else if (m_precalcType == MyLodTypeEnum.LOD1)
                {
                    temporaryVoxel.Content = m_voxelMap.GetDataCellAverageContent(ref tempVoxelCoord);
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }

                temporaryVoxel.Content_CalcCounter = m_temporaryVoxelsCounter;
            }

            return temporaryVoxel.Content;
        }

        //  Get normal from lookup table or calc it
        void GetVoxelNormal(MyTemporaryVoxel temporaryVoxel, ref MyMwcVector3Int coord, ref MyMwcVector3Int voxelCoord, MyTemporaryVoxel centerVoxel)
        {
            if (temporaryVoxel.Normal_CalcCounter != m_temporaryVoxelsCounter)
            {
                if ((voxelCoord.X == 0) || (voxelCoord.X == (m_sizeMinusOne.X)) ||
                    (voxelCoord.Y == 0) || (voxelCoord.Y == (m_sizeMinusOne.Y)) ||
                    (voxelCoord.Z == 0) || (voxelCoord.Z == (m_sizeMinusOne.Z)))
                {
                    //  If asked for normal vector for voxel that is at the voxel map border, we can't compute it by gradient, so return this hack.
                    temporaryVoxel.Normal = centerVoxel.Normal;
                }
                else
                {
                    Vector3 normal = new Vector3(
                            (GetVoxelContent(coord.X - 1, coord.Y, coord.Z) - GetVoxelContent(coord.X + 1, coord.Y, coord.Z)) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT,
                            (GetVoxelContent(coord.X, coord.Y - 1, coord.Z) - GetVoxelContent(coord.X, coord.Y + 1, coord.Z)) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT,
                            (GetVoxelContent(coord.X, coord.Y, coord.Z - 1) - GetVoxelContent(coord.X, coord.Y, coord.Z + 1)) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT);

                    if (normal.LengthSquared() <= 0.0f)
                    {
                        //  If voxels surounding voxel for which we want to get normal vector are of the same value, their subtracting leads to zero vector and that can't be used. So following line is hack.
                        temporaryVoxel.Normal = centerVoxel.Normal;
                    }
                    else
                    {
                        MyMwcUtils.Normalize(ref normal, out temporaryVoxel.Normal);
                    }
                }
                temporaryVoxel.Normal_CalcCounter = m_temporaryVoxelsCounter;
            }
        }

        //  Get sun color (or light) from lookup table or calc it 
        //  IMPORTANT: At this point normals must be calculated because GetVoxelAmbientAndSun() will be retrieving them from temp table and not checking if there is actual value
        void GetVoxelAmbient(MyTemporaryVoxel temporaryVoxel, ref MyMwcVector3Int coord, ref MyMwcVector3Int tempVoxelCoord)
        {
            if (temporaryVoxel.Ambient_CalcCounter != m_temporaryVoxelsCounter)
            {
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //  Ambient light calculation is same for LOD and no-LOD
                //  This formula was choosen by experiments and observation, no real thought is behind it.
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                float ambient = 0;

                /* 3 Point lighting - disabled now, instead used ambient occlusion
                float dot0;
                Vector3.Dot(ref temporaryVoxel.Normal, ref MyVoxelConstants.AMBIENT_LIGHT_DIRECTION_0, out dot0);

                float dot1;
                Vector3.Dot(ref temporaryVoxel.Normal, ref MyVoxelConstants.AMBIENT_LIGHT_DIRECTION_1, out dot1);

                float dot2;
                Vector3.Dot(ref temporaryVoxel.Normal, ref MyVoxelConstants.AMBIENT_LIGHT_DIRECTION_2, out dot2);


                ambient += 1.0f * MathHelper.Clamp(dot0, 0, 1);
                ambient += 0.8f * MathHelper.Clamp(dot1, 0, 1);
                ambient += 0.6f * MathHelper.Clamp(dot2, 0, 1);
                //ambient *= 5;
                ambient = MathHelper.Clamp(ambient, 0, 1);
                ambient = 1 - ambient;
                ambient = 0.8f;
                //ambient *= MyConstants.AMBIENT_COLOR;
                //ambient *= 0.1f + ambient * 0.15f;
                */

                // Voxel ambient occlusion
                const int VOXELS_CHECK_COUNT = 1;
                for (int ambientX = -VOXELS_CHECK_COUNT; ambientX <= VOXELS_CHECK_COUNT; ambientX++)
                {
                    for (int ambientY = -VOXELS_CHECK_COUNT; ambientY <= VOXELS_CHECK_COUNT; ambientY++)
                    {
                        for (int ambientZ = -VOXELS_CHECK_COUNT; ambientZ <= VOXELS_CHECK_COUNT; ambientZ++)
                        {
                            MyMwcVector3Int tmpVoxelCoord = new MyMwcVector3Int(m_voxelStart.X + coord.X + ambientX - 1, m_voxelStart.Y + coord.Y + ambientY - 1, m_voxelStart.Z + coord.Z + ambientZ - 1);

                            if ((tmpVoxelCoord.X < 0) || (tmpVoxelCoord.X > (m_sizeMinusOne.X)) ||
                                (tmpVoxelCoord.Y < 0) || (tmpVoxelCoord.Y > (m_sizeMinusOne.Y)) ||
                                (tmpVoxelCoord.Z < 0) || (tmpVoxelCoord.Z > (m_sizeMinusOne.Z)))
                            {
                                //  Ambient occlusion for requested voxel can't be calculated because surounding voxels are outside of the map
                            }
                            else
                            {
                                if (VOXELS_CHECK_COUNT == 1)
                                {
                                    ambient += (float)GetVoxelContent(coord.X + ambientX, coord.Y + ambientY, coord.Z + ambientZ);
                                }
                                else
                                {
                                    //  IMPORTANT: We trace 3x3x3 voxels around our voxel. So when dividng to get <0..1> interval, divide by this number.
                                    ambient += m_voxelMap.GetVoxelContent(ref tmpVoxelCoord);
                                }
                            }
                        }
                    }
                }

                //  IMPORTANT: We trace 3x3x3 voxels around our voxel. So when dividng to get <0..1> interval, divide by this number.
                ambient /= MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT * (VOXELS_CHECK_COUNT * 2 + 1) * (VOXELS_CHECK_COUNT * 2 + 1) * (VOXELS_CHECK_COUNT * 2 + 1);

                //  Flip the number, so from now dark voxels are 0.0 and light are 1.0
                ambient = 1.0f - ambient;

                //  This values are chosen by trial-and-error
                const float MIN = 0.4f;// 0.1f;
                const float MAX = 0.9f;// 0.6f;

                ambient = MathHelper.Clamp(ambient, MIN, MAX);

                ambient = (ambient - MIN) / (MAX - MIN);
                ambient -= 0.5f;

                temporaryVoxel.Ambient = ambient;
                temporaryVoxel.Ambient_CalcCounter = m_temporaryVoxelsCounter;
            }
        }

        //  Linearly interpolates position, normal and material on poly-cube edge. Interpolated point is where an isosurface cuts an edge between two vertices, each with their own scalar value.
        void GetVertexInterpolation(MyTemporaryVoxel inputVoxelA, MyTemporaryVoxel inputVoxelB, int edgeIndex)
        {
            MyEdge edge = m_edges[edgeIndex];

            if (Math.Abs(MyVoxelConstants.VOXEL_ISO_LEVEL - inputVoxelA.Content) < 0.00001f)
            {
                edge.Position = inputVoxelA.Position;
                edge.Normal = inputVoxelA.Normal;
                edge.Material = inputVoxelA.Material;
                edge.Ambient = inputVoxelA.Ambient;
                return;
            }

            if (Math.Abs(MyVoxelConstants.VOXEL_ISO_LEVEL - inputVoxelB.Content) < 0.00001f)
            {
                edge.Position = inputVoxelB.Position;
                edge.Normal = inputVoxelB.Normal;
                edge.Material = inputVoxelB.Material;
                edge.Ambient = inputVoxelB.Ambient;
                return;
            }

            float mu = (float)(MyVoxelConstants.VOXEL_ISO_LEVEL - inputVoxelA.Content) / (float)(inputVoxelB.Content - inputVoxelA.Content);
            System.Diagnostics.Debug.Assert(mu > 0.0f && mu < 1.0f);

            edge.Position.X = inputVoxelA.Position.X + mu * (inputVoxelB.Position.X - inputVoxelA.Position.X);
            edge.Position.Y = inputVoxelA.Position.Y + mu * (inputVoxelB.Position.Y - inputVoxelA.Position.Y);
            edge.Position.Z = inputVoxelA.Position.Z + mu * (inputVoxelB.Position.Z - inputVoxelA.Position.Z);

            edge.Normal.X = inputVoxelA.Normal.X + mu * (inputVoxelB.Normal.X - inputVoxelA.Normal.X);
            edge.Normal.Y = inputVoxelA.Normal.Y + mu * (inputVoxelB.Normal.Y - inputVoxelA.Normal.Y);
            edge.Normal.Z = inputVoxelA.Normal.Z + mu * (inputVoxelB.Normal.Z - inputVoxelA.Normal.Z);
            if (MyMwcUtils.IsZero(edge.Normal))
                edge.Normal = inputVoxelA.Normal;
            else
                edge.Normal = MyMwcUtils.Normalize(edge.Normal);

            float mu2 = ((float)inputVoxelB.Content) / (((float)inputVoxelA.Content) + ((float)inputVoxelB.Content));
            //edge.Material = (mu <= MyVoxelConstants.VOXEL_ISO_LEVEL) ? inputVoxelA.Material : inputVoxelB.Material;
            edge.Material = (mu2 <= 0.5f) ? inputVoxelA.Material : inputVoxelB.Material;

            edge.Ambient = inputVoxelA.Ambient + mu2 * (inputVoxelB.Ambient - inputVoxelA.Ambient);
            //edge.Ambient = inputVoxelA.Ambient + mu * (inputVoxelB.Ambient - inputVoxelA.Ambient);

            return;
        }

        ////  Copy shadow values of surrounding cells
        //void CopyShadowValues()
        //{
        //    if (m_precalcType == MyLodTypeEnum.LOD0)
        //    {
        //        //  This is cell we are trying to precalculate            
        //        MyMwcVector3Int precalculatedCellCoord = m_voxelMap.GetDataCellCoordinate(ref m_voxelStart);

        //        for (int x = 0; x < SHADOW_CELLS; x++)
        //        {
        //            for (int y = 0; y < SHADOW_CELLS; y++)
        //            {
        //                for (int z = 0; z < SHADOW_CELLS; z++)
        //                {
        //                    MyMwcVector3Int tempCellCoord = new MyMwcVector3Int(precalculatedCellCoord.X + x - 1, precalculatedCellCoord.Y + y - 1, precalculatedCellCoord.Z + z - 1);

        //                    //  This is coordinate of cell's middle and it may contain values that are outside voxel map. It is OK, because we use it for bilinear/average calculations.
        //                    m_shadows[x][y][z].CoordOfDataCellCenter = m_voxelMap.GetVoxelCoordinatesOfCenterOfDataCell(ref tempCellCoord);

        //                    if (m_voxelMap.IsDataCellInVoxelMap(ref tempCellCoord) == false)
        //                    {
        //                        //  Data cell isn't in this voxel map, so it can't cast shadow map
        //                        m_shadows[x][y][z].ShadowValue = 1.0f;
        //                    }
        //                    else
        //                    {
        //                        m_shadows[x][y][z].ShadowValue = (float)m_voxelMap.GetDataCellShadow(ref tempCellCoord) / 255.0f;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //  Precalculate voxel cell into cache (makes triangles and vertex buffer from voxels)
        public void Precalc(MyVoxelPrecalcTaskItem task)
        {
            m_precalcType = task.Type;

            m_resultVerticesCounter = 0;
            m_resultTrianglesCounter = 0;
            m_edgeVertexCalcCounter++;
            m_voxelMap = task.VoxelMap;
            m_voxelStart = task.VoxelStart;

            try
            {
                //m_voxelMap.LockAcquireWriterLock(Timeout.Infinite);

                CalcPolygCubeSize();

                //CopyShadowValues();

                //  Copy voxels into temp array
                bool totallyEmptyOrTotallyFull = CopyVoxelContents();

                if (totallyEmptyOrTotallyFull == false)
                {
                    //  Size of voxel or cell (in meters) and size of voxel map / voxel cells
                    float size;
                    if (m_precalcType == MyLodTypeEnum.LOD0)
                    {
                        size = MyVoxelConstants.VOXEL_SIZE_IN_METRES;
                        m_sizeMinusOne = m_voxelMap.SizeMinusOne;
                    }
                    else if (m_precalcType == MyLodTypeEnum.LOD1)
                    {
                        size = MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES;
                        m_sizeMinusOne = m_voxelMap.DataCellsCountMinusOne;
                    }
                    else
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }

                    //  Origin position for voxels (in meters)
                    Vector3 originPosition;
                    if (m_precalcType == MyLodTypeEnum.LOD0)
                    {
                        originPosition = m_voxelMap.GetVoxelCenterPositionRelative(ref m_voxelStart);
                    }
                    else if (m_precalcType == MyLodTypeEnum.LOD1)
                    {
                        originPosition = m_voxelMap.GetDataCellCenterPositionRelative(ref m_voxelStart);
                    }
                    else
                    {
                        throw new MyMwcExceptionApplicationShouldNotGetHere();
                    }

                    MyMwcVector3Int coord0;
                    for (coord0.X = 1; coord0.X <= (m_polygCubesX - 1); coord0.X++)
                    {
                        for (coord0.Y = 1; coord0.Y <= (m_polygCubesY - 1); coord0.Y++)
                        {
                            for (coord0.Z = 1; coord0.Z <= (m_polygCubesZ - 1); coord0.Z++)
                            {
                                //  We can get this voxel content right from cache (not using GetVoxelContent method), because after CopyVoxelContents these array must be filled. But only content, not material, normal, etc.
                                MyTemporaryVoxel tempVoxel0 = m_temporaryVoxels[coord0.X][coord0.Y][coord0.Z];
                                MyTemporaryVoxel tempVoxel1 = m_temporaryVoxels[coord0.X + 1][coord0.Y][coord0.Z];
                                MyTemporaryVoxel tempVoxel2 = m_temporaryVoxels[coord0.X + 1][coord0.Y][coord0.Z + 1];
                                MyTemporaryVoxel tempVoxel3 = m_temporaryVoxels[coord0.X][coord0.Y][coord0.Z + 1];
                                MyTemporaryVoxel tempVoxel4 = m_temporaryVoxels[coord0.X][coord0.Y + 1][coord0.Z];
                                MyTemporaryVoxel tempVoxel5 = m_temporaryVoxels[coord0.X + 1][coord0.Y + 1][coord0.Z];
                                MyTemporaryVoxel tempVoxel6 = m_temporaryVoxels[coord0.X + 1][coord0.Y + 1][coord0.Z + 1];
                                MyTemporaryVoxel tempVoxel7 = m_temporaryVoxels[coord0.X][coord0.Y + 1][coord0.Z + 1];

                                System.Diagnostics.Debug.Assert(tempVoxel0.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel1.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel2.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel3.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel4.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel5.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel6.Content_CalcCounter == m_temporaryVoxelsCounter);
                                System.Diagnostics.Debug.Assert(tempVoxel7.Content_CalcCounter == m_temporaryVoxelsCounter);

                                //  We can get this voxel content right from cache (not using GetVoxelContent method), because after CopyVoxelContents these array must be filled. But only content, not material, normal, etc.
                                int cubeIndex = 0;
                                if (tempVoxel0.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 1;
                                if (tempVoxel1.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 2;
                                if (tempVoxel2.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 4;
                                if (tempVoxel3.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 8;
                                if (tempVoxel4.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 16;
                                if (tempVoxel5.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 32;
                                if (tempVoxel6.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 64;
                                if (tempVoxel7.Content < MyVoxelConstants.VOXEL_ISO_LEVEL) cubeIndex |= 128;

                                //  Cube is entirely in/out of the surface
                                if (MyVoxelPrecalcConstants.EdgeTable[cubeIndex] == 0)
                                {
                                    continue;
                                }

                                MyMwcVector3Int coord1 = new MyMwcVector3Int(coord0.X + 1, coord0.Y, coord0.Z);
                                MyMwcVector3Int coord2 = new MyMwcVector3Int(coord0.X + 1, coord0.Y, coord0.Z + 1);
                                MyMwcVector3Int coord3 = new MyMwcVector3Int(coord0.X, coord0.Y, coord0.Z + 1);
                                MyMwcVector3Int coord4 = new MyMwcVector3Int(coord0.X, coord0.Y + 1, coord0.Z);
                                MyMwcVector3Int coord5 = new MyMwcVector3Int(coord0.X + 1, coord0.Y + 1, coord0.Z);
                                MyMwcVector3Int coord6 = new MyMwcVector3Int(coord0.X + 1, coord0.Y + 1, coord0.Z + 1);
                                MyMwcVector3Int coord7 = new MyMwcVector3Int(coord0.X, coord0.Y + 1, coord0.Z + 1);

                                MyMwcVector3Int tempVoxelCoord0 = new MyMwcVector3Int(m_voxelStart.X + coord0.X - 1, m_voxelStart.Y + coord0.Y - 1, m_voxelStart.Z + coord0.Z - 1);
                                MyMwcVector3Int tempVoxelCoord1 = new MyMwcVector3Int(m_voxelStart.X + coord1.X - 1, m_voxelStart.Y + coord1.Y - 1, m_voxelStart.Z + coord1.Z - 1);
                                MyMwcVector3Int tempVoxelCoord2 = new MyMwcVector3Int(m_voxelStart.X + coord2.X - 1, m_voxelStart.Y + coord2.Y - 1, m_voxelStart.Z + coord2.Z - 1);
                                MyMwcVector3Int tempVoxelCoord3 = new MyMwcVector3Int(m_voxelStart.X + coord3.X - 1, m_voxelStart.Y + coord3.Y - 1, m_voxelStart.Z + coord3.Z - 1);
                                MyMwcVector3Int tempVoxelCoord4 = new MyMwcVector3Int(m_voxelStart.X + coord4.X - 1, m_voxelStart.Y + coord4.Y - 1, m_voxelStart.Z + coord4.Z - 1);
                                MyMwcVector3Int tempVoxelCoord5 = new MyMwcVector3Int(m_voxelStart.X + coord5.X - 1, m_voxelStart.Y + coord5.Y - 1, m_voxelStart.Z + coord5.Z - 1);
                                MyMwcVector3Int tempVoxelCoord6 = new MyMwcVector3Int(m_voxelStart.X + coord6.X - 1, m_voxelStart.Y + coord6.Y - 1, m_voxelStart.Z + coord6.Z - 1);
                                MyMwcVector3Int tempVoxelCoord7 = new MyMwcVector3Int(m_voxelStart.X + coord7.X - 1, m_voxelStart.Y + coord7.Y - 1, m_voxelStart.Z + coord7.Z - 1);

                                tempVoxel0.Position.X = originPosition.X + (coord0.X - 1) * size;
                                tempVoxel0.Position.Y = originPosition.Y + (coord0.Y - 1) * size;
                                tempVoxel0.Position.Z = originPosition.Z + (coord0.Z - 1) * size;

                                tempVoxel1.Position.X = tempVoxel0.Position.X + size;
                                tempVoxel1.Position.Y = tempVoxel0.Position.Y;
                                tempVoxel1.Position.Z = tempVoxel0.Position.Z;

                                tempVoxel2.Position.X = tempVoxel0.Position.X + size;
                                tempVoxel2.Position.Y = tempVoxel0.Position.Y;
                                tempVoxel2.Position.Z = tempVoxel0.Position.Z + size;

                                tempVoxel3.Position.X = tempVoxel0.Position.X;
                                tempVoxel3.Position.Y = tempVoxel0.Position.Y;
                                tempVoxel3.Position.Z = tempVoxel0.Position.Z + size;

                                tempVoxel4.Position.X = tempVoxel0.Position.X;
                                tempVoxel4.Position.Y = tempVoxel0.Position.Y + size;
                                tempVoxel4.Position.Z = tempVoxel0.Position.Z;

                                tempVoxel5.Position.X = tempVoxel0.Position.X + size;
                                tempVoxel5.Position.Y = tempVoxel0.Position.Y + size;
                                tempVoxel5.Position.Z = tempVoxel0.Position.Z;

                                tempVoxel6.Position.X = tempVoxel0.Position.X + size;
                                tempVoxel6.Position.Y = tempVoxel0.Position.Y + size;
                                tempVoxel6.Position.Z = tempVoxel0.Position.Z + size;

                                tempVoxel7.Position.X = tempVoxel0.Position.X;
                                tempVoxel7.Position.Y = tempVoxel0.Position.Y + size;
                                tempVoxel7.Position.Z = tempVoxel0.Position.Z + size;

                                //  Normals at grid corners (calculated from gradient)
                                GetVoxelNormal(tempVoxel0, ref coord0, ref tempVoxelCoord0, tempVoxel0);
                                GetVoxelNormal(tempVoxel1, ref coord1, ref tempVoxelCoord1, tempVoxel0);
                                GetVoxelNormal(tempVoxel2, ref coord2, ref tempVoxelCoord2, tempVoxel0);
                                GetVoxelNormal(tempVoxel3, ref coord3, ref tempVoxelCoord3, tempVoxel0);
                                GetVoxelNormal(tempVoxel4, ref coord4, ref tempVoxelCoord4, tempVoxel0);
                                GetVoxelNormal(tempVoxel5, ref coord5, ref tempVoxelCoord5, tempVoxel0);
                                GetVoxelNormal(tempVoxel6, ref coord6, ref tempVoxelCoord6, tempVoxel0);
                                GetVoxelNormal(tempVoxel7, ref coord7, ref tempVoxelCoord7, tempVoxel0);

                                //  Ambient occlusion colors at grid corners
                                //  IMPORTANT: At this point normals must be calculated because GetVoxelAmbientAndSun() will be retrieving them from temp table and not checking if there is actual value
                                GetVoxelAmbient(tempVoxel0, ref coord0, ref tempVoxelCoord0);
                                GetVoxelAmbient(tempVoxel1, ref coord1, ref tempVoxelCoord1);
                                GetVoxelAmbient(tempVoxel2, ref coord2, ref tempVoxelCoord2);
                                GetVoxelAmbient(tempVoxel3, ref coord3, ref tempVoxelCoord3);
                                GetVoxelAmbient(tempVoxel4, ref coord4, ref tempVoxelCoord4);
                                GetVoxelAmbient(tempVoxel5, ref coord5, ref tempVoxelCoord5);
                                GetVoxelAmbient(tempVoxel6, ref coord6, ref tempVoxelCoord6);
                                GetVoxelAmbient(tempVoxel7, ref coord7, ref tempVoxelCoord7);

                                //  Materials at grid corners
                                GetVoxelMaterial(tempVoxel0, ref tempVoxelCoord0);
                                GetVoxelMaterial(tempVoxel1, ref tempVoxelCoord1);
                                GetVoxelMaterial(tempVoxel2, ref tempVoxelCoord2);
                                GetVoxelMaterial(tempVoxel3, ref tempVoxelCoord3);
                                GetVoxelMaterial(tempVoxel4, ref tempVoxelCoord4);
                                GetVoxelMaterial(tempVoxel5, ref tempVoxelCoord5);
                                GetVoxelMaterial(tempVoxel6, ref tempVoxelCoord6);
                                GetVoxelMaterial(tempVoxel7, ref tempVoxelCoord7);

                                //  Find the vertices where the surface intersects the cube
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 1) == 1)
                                {
                                    GetVertexInterpolation(tempVoxel0, tempVoxel1, 0);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 2) == 2)
                                {
                                    GetVertexInterpolation(tempVoxel1, tempVoxel2, 1);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 4) == 4)
                                {
                                    GetVertexInterpolation(tempVoxel2, tempVoxel3, 2);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 8) == 8)
                                {
                                    GetVertexInterpolation(tempVoxel3, tempVoxel0, 3);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 16) == 16)
                                {
                                    GetVertexInterpolation(tempVoxel4, tempVoxel5, 4);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 32) == 32)
                                {
                                    GetVertexInterpolation(tempVoxel5, tempVoxel6, 5);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 64) == 64)
                                {
                                    GetVertexInterpolation(tempVoxel6, tempVoxel7, 6);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 128) == 128)
                                {
                                    GetVertexInterpolation(tempVoxel7, tempVoxel4, 7);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 256) == 256)
                                {
                                    GetVertexInterpolation(tempVoxel0, tempVoxel4, 8);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 512) == 512)
                                {
                                    GetVertexInterpolation(tempVoxel1, tempVoxel5, 9);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 1024) == 1024)
                                {
                                    GetVertexInterpolation(tempVoxel2, tempVoxel6, 10);
                                }
                                if ((MyVoxelPrecalcConstants.EdgeTable[cubeIndex] & 2048) == 2048)
                                {
                                    GetVertexInterpolation(tempVoxel3, tempVoxel7, 11);
                                }

                                //  Create the triangles
                                MyMwcVector3Int edge = new MyMwcVector3Int(coord0.X - 1, coord0.Y - 1, coord0.Z - 1);
                                for (int i = 0; MyVoxelPrecalcConstants.TriangleTable[cubeIndex, i] != -1; i += 3)
                                {
                                    //  Edge indexes inside the cube
                                    int edgeIndex0 = MyVoxelPrecalcConstants.TriangleTable[cubeIndex, i + 0];
                                    int edgeIndex1 = MyVoxelPrecalcConstants.TriangleTable[cubeIndex, i + 1];
                                    int edgeIndex2 = MyVoxelPrecalcConstants.TriangleTable[cubeIndex, i + 2];

                                    MyEdge edge0 = m_edges[edgeIndex0];
                                    MyEdge edge1 = m_edges[edgeIndex1];
                                    MyEdge edge2 = m_edges[edgeIndex2];


                                    //  Edge indexes inside the cell
                                    MyMwcVector4Int edgeConversion0 = MyVoxelPrecalcConstants.EdgeConversion[edgeIndex0];
                                    MyMwcVector4Int edgeConversion1 = MyVoxelPrecalcConstants.EdgeConversion[edgeIndex1];
                                    MyMwcVector4Int edgeConversion2 = MyVoxelPrecalcConstants.EdgeConversion[edgeIndex2];

                                    MyEdgeVertex edgeVertex0 = m_edgeVertex[edge.X + edgeConversion0.X][edge.Y + edgeConversion0.Y][edge.Z + edgeConversion0.Z][edgeConversion0.W];
                                    MyEdgeVertex edgeVertex1 = m_edgeVertex[edge.X + edgeConversion1.X][edge.Y + edgeConversion1.Y][edge.Z + edgeConversion1.Z][edgeConversion1.W];
                                    MyEdgeVertex edgeVertex2 = m_edgeVertex[edge.X + edgeConversion2.X][edge.Y + edgeConversion2.Y][edge.Z + edgeConversion2.Z][edgeConversion2.W];


                                    MyVoxelVertex compressedVertex0 = new MyVoxelVertex();
                                    compressedVertex0.Position = edge0.Position;
                                    MyVoxelVertex compressedVertex1 = new MyVoxelVertex();
                                    compressedVertex1.Position = edge1.Position;
                                    MyVoxelVertex compressedVertex2 = new MyVoxelVertex();
                                    compressedVertex2.Position = edge2.Position;

                                    //  We want to skip all wrong triangles, those that have two vertex at almost the same location, etc.
                                    //  We do it simply, by calculating triangle normal and then checking if this normal has length large enough
                                    if (IsWrongTriangle(ref compressedVertex0, ref compressedVertex1, ref compressedVertex2) == true)
                                    {
                                        continue;
                                    }

                                    //  Vertex at edge 0     
                                    if (edgeVertex0.CalcCounter != m_edgeVertexCalcCounter)
                                    {
                                        //  If vertex at edge0 wasn't calculated for this cell during this precalc, we need to add it

                                        //  Short overflow check
                                        System.Diagnostics.Debug.Assert(m_resultVerticesCounter <= short.MaxValue);

                                        edgeVertex0.CalcCounter = m_edgeVertexCalcCounter;
                                        edgeVertex0.VertexIndex = m_resultVerticesCounter;

                                        m_resultVertices[m_resultVerticesCounter].Position = edge0.Position;
                                        m_resultVertices[m_resultVerticesCounter].Normal = edge0.Normal;
                                        m_resultVertices[m_resultVerticesCounter].Ambient = edge0.Ambient;
                                        m_resultVertices[m_resultVerticesCounter].Material = edge0.Material;
                                        //m_resultVertices[m_resultVerticesCounter].OnRenderCellEdge = false;

                                        if (IsCoordOnRenderCellEdge(tempVoxelCoord0))
                                        {
                                            //m_resultVertices[m_resultVerticesCounter].OnRenderCellEdge = true;
                                        }

                                        m_resultVerticesCounter++;
                                    }

                                    //  Vertex at edge 1
                                    if (edgeVertex1.CalcCounter != m_edgeVertexCalcCounter)
                                    {
                                        //  If vertex at edge1 wasn't calculated for this cell during this precalc, we need to add it

                                        //  Short overflow check
                                        System.Diagnostics.Debug.Assert(m_resultVerticesCounter <= short.MaxValue);

                                        edgeVertex1.CalcCounter = m_edgeVertexCalcCounter;
                                        edgeVertex1.VertexIndex = m_resultVerticesCounter;

                                        m_resultVertices[m_resultVerticesCounter].Position = edge1.Position;
                                        m_resultVertices[m_resultVerticesCounter].Normal = edge1.Normal;
                                        m_resultVertices[m_resultVerticesCounter].Ambient = edge1.Ambient;
                                        m_resultVertices[m_resultVerticesCounter].Material = edge1.Material;
                                        //m_resultVertices[m_resultVerticesCounter].OnRenderCellEdge = false;

                                        if (IsCoordOnRenderCellEdge(tempVoxelCoord0))
                                        {
                                            //m_resultVertices[m_resultVerticesCounter].OnRenderCellEdge = true;
                                        }

                                        m_resultVerticesCounter++;
                                    }

                                    //  Vertex at edge 2
                                    if (edgeVertex2.CalcCounter != m_edgeVertexCalcCounter)
                                    {
                                        //  If vertex at edge2 wasn't calculated for this cell during this precalc, we need to add it

                                        //  Short overflow check
                                        System.Diagnostics.Debug.Assert(m_resultVerticesCounter <= short.MaxValue);

                                        edgeVertex2.CalcCounter = m_edgeVertexCalcCounter;
                                        edgeVertex2.VertexIndex = m_resultVerticesCounter;

                                        m_resultVertices[m_resultVerticesCounter].Position = edge2.Position;
                                        m_resultVertices[m_resultVerticesCounter].Normal = edge2.Normal;
                                        m_resultVertices[m_resultVerticesCounter].Ambient = edge2.Ambient;
                                        m_resultVertices[m_resultVerticesCounter].Material = edge2.Material;
                                        //m_resultVertices[m_resultVerticesCounter].OnRenderCellEdge = false;

                                        if (IsCoordOnRenderCellEdge(tempVoxelCoord0))
                                        {
                                          //  m_resultVertices[m_resultVerticesCounter].OnRenderCellEdge = true;
                                        }

                                        m_resultVerticesCounter++;
                                    }

                                    //  Triangle
                                    m_resultTriangles[m_resultTrianglesCounter].VertexIndex0 = edgeVertex0.VertexIndex;
                                    m_resultTriangles[m_resultTrianglesCounter].VertexIndex1 = edgeVertex1.VertexIndex;
                                    m_resultTriangles[m_resultTrianglesCounter].VertexIndex2 = edgeVertex2.VertexIndex;
                                    Debug.Assert(edgeVertex0.VertexIndex < m_resultVerticesCounter);
                                    Debug.Assert(edgeVertex1.VertexIndex < m_resultVerticesCounter);
                                    Debug.Assert(edgeVertex2.VertexIndex < m_resultVerticesCounter);

                                    //  Each voxel triangleVertexes has unique ID in whole game (but only on client side)
                                    //m_resultTriangles[m_resultTrianglesCounter].TriangleId = m_triangleIdCounter++;

                                    m_resultTrianglesCounter++;
                                }
                            }
                        }
                    }
                }

                // Cache the vertices and triangles and precalculate the octree
                task.Cache.PrepareCache(m_resultVertices, m_resultVerticesCounter, m_resultTriangles, m_resultTrianglesCounter);
            }
            finally
            {
                //m_voxelMap.Lock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Returns true if the coordinate is on boundary of voxel render cells.
        /// </summary>
        bool IsCoordOnRenderCellEdge(MyMwcVector3Int coord)
        {
            return
                ((coord.X % (MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS)) == 0) ||
                ((coord.X % (MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS)) == MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS - 1) ||
                ((coord.Y % (MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS)) == 0) ||
                ((coord.Y % (MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS)) == MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS - 1) ||
                ((coord.Z % (MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS)) == 0) ||
                ((coord.Z % (MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS)) == MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS - 1);
        }

        //  We want to skip all wrong triangles, those that have two vertex at almost the same location, etc.
        //  We do it simply, by calculating triangle normal and then checking if this normal has length large enough
        bool IsWrongTriangle(ref MyVoxelVertex edge0, ref MyVoxelVertex edge1, ref MyVoxelVertex edge2)
        {
            //  Distance between two vertices is the fastest test
            Vector3 triangleEdgeVector1 = edge2.Position - edge0.Position;
            if (triangleEdgeVector1.LengthSquared() <= MyMwcMathConstants.EPSILON_SQUARED) return true;

            //  Distance between two vertexes is the fastest test
            Vector3 triangleEdgeVector2 = edge1.Position - edge0.Position;
            if (triangleEdgeVector2.LengthSquared() <= MyMwcMathConstants.EPSILON_SQUARED) return true;

            //  Distance between two vertexes is the fastest test
            Vector3 triangleEdgeVector3 = edge1.Position - edge2.Position;
            if (triangleEdgeVector3.LengthSquared() <= MyMwcMathConstants.EPSILON_SQUARED) return true;

            //  We don't need to do this advanced test, because it has zero improvement on voxel triangles (and testing takes time too)
            /*Vector3 norm;
            Vector3.Cross(ref triangleEdgeVector1, ref triangleEdgeVector2, out norm);
            if (norm.Length() < MyMwcMathConstants.EPSILON) return true;*/

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//  This class is used for importing 3D models (FBX files) into voxels.
//  It makes a lot of garbage, so GC.Collect() should be called after done.
//  It's one-time usable class - call it with constructor, let it change your voxel map and then abandon.
//
//  This algorythm travers triangles from the bottom and switches between full/empty column if intersection found.
//  It ignores normal vector at intersection point, because that version had problems with wrong models. Current version doesn't.

namespace MinerWars.AppCode.Game.Voxels
{
    using App;

    enum MyvoxelImportAction
    {
        AddVoxels,
        RemoveVoxels,
        ChangeMaterial,
        SoftenVoxels,
        WrinkleVoxels
    }

    enum MyVoxelImportOptions
    {
        None,
        KeepAspectRatio,
        //Will only keep scale if it can fit into the voxel
        KeepScale
    }
    class MyVoxelImport
    {
        Vector3 m_minCoord;
        Vector3 m_maxCoord;
        List<MyImportTriangle> m_triangles;

        //  Space between two grid points in metres
        float m_gridPointsSize;
        float m_gridPointsSizeHalf;

        List<MyImportTriangle>[,] m_trianglesLookup;
        int m_trianglesLookupSizeX;
        int m_trianglesLookupSizeZ;
        float m_trianglesLookupElementSizeX;
        float m_trianglesLookupElementSizeZ;

        //  Number of grid points stored in one voxel, in one direction. So total count of grid points in voxel is power of 3.
        const int GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION = 8;
        const int GRID_POINTS_IN_ONE_VOXEL_TOTAL = GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION * GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION * GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION;

        //  This is max number of grid points in one directions. It's purpose is that we can't make grid for whole voxel map (if it's very large). 
        //  So we need to split processing into more iterations, every time working with only part of the map.
        const int MAX_GRID_SIZE_IN_ONE_DIRECTION = 256;

        //  Count of voxels in one grid, in one direction
        const int VOXELS_IN_GRID_IN_ONE_DIRECTION = MAX_GRID_SIZE_IN_ONE_DIRECTION / GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION;

        //  This is just for setting initial capacity of triangleVertexes lists
        const int INITIAL_COUNT_OF_TRIANGLES_IN_GRID_POSITION = 5;

        //  This is just for setting initial capacity of intersections lists
        const int INITIAL_COUNT_OF_INTERSECTIONS = 20;


        class MyImportTriangle
        {
            public Vector3 Vertex0;
            public Vector3 Vertex1;
            public Vector3 Vertex2;
            public Vector3 Normal;

            public MyImportTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
            {
                Vertex0 = vertex0;
                Vertex1 = vertex1;
                Vertex2 = vertex2;
                Normal = MyMwcUtils.Normalize(Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0));
            }
        }

        class MyImportIntersection
        {
            public Vector3 Intersection;
            public Vector3 Normal;
            public float Distance;

            public MyImportIntersection(Vector3 intersection, Vector3 normal, float distance)
            {
                Intersection = intersection;
                Normal = normal;
                Distance = distance;
            }
        }

        public static void Fill(MyVoxelMap voxelMap)
        {
            float safeSize = 5; // Size in voxels to leave area empty on voxel map border
            var size = voxelMap.Size;
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    for (int z = 0; z < size.Z; z++)
                    {
                        MyMwcVector3Int coords = new MyMwcVector3Int(x, y, z);

                        if (x <= safeSize || y <= safeSize || z <= safeSize || x >= size.X - safeSize - 1 || y >= size.Y - safeSize - 1 || z >= size.Z - safeSize - 1)
                        {
                            voxelMap.SetVoxelContent(0, ref coords);
                        }
                        else
                        {
                            voxelMap.SetVoxelContent(255, ref coords);
                        }
                    }
                }
            }
            voxelMap.InvalidateCache(new MyMwcVector3Int(0, 0, 0), size);
        }

        public static void FillEmpty(MyVoxelMap voxelMap)
        {
            float safeSize = 5; // Size in voxels to leave area empty on voxel map border
            var size = voxelMap.Size;
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    for (int z = 0; z < size.Z; z++)
                    {
                        MyMwcVector3Int coords = new MyMwcVector3Int(x, y, z);

                        if (x <= safeSize || y <= safeSize || z <= safeSize || x >= size.X - safeSize - 1 || y >= size.Y - safeSize - 1 || z >= size.Z - safeSize - 1)
                        {
                            voxelMap.SetVoxelContent(0, ref coords);
                        }
                        else
                        {
                            voxelMap.SetVoxelContent(0, ref coords);
                        }
                    }
                }
            }
            voxelMap.InvalidateCache(new MyMwcVector3Int(0, 0, 0), size);
        }

        //  Use this static method do one-time import of a voxel map
        public static void Run(MyVoxelMap voxelMap, string modelName)
        {
            MyVoxelImport voxelMapImport = new MyVoxelImport(voxelMap, modelName, MyVoxelImportOptions.None);
        }

        //  Use this static method do one-time import of a voxel map
        public static void Run(MyVoxelMap voxelMap, string modelName, MyVoxelImportOptions importOptions)
        {
            MyVoxelImport voxelMapImport = new MyVoxelImport(voxelMap, modelName, importOptions);
        }

        //  Use this static method do one-time import of a voxel map
        public static void Run(MyVoxelMap voxelMap, MyModel model)
        {
            MyVoxelImport voxelMapImport = new MyVoxelImport(voxelMap, model, MyVoxelImportOptions.None);
        }

        //  Use this static method do one-time import of a voxel map
        public static void Run(MyVoxelMap voxelMap, MyModel model, MyVoxelImportOptions importOptions)
        {
            MyVoxelImport voxelMapImport = new MyVoxelImport(voxelMap, model, importOptions);
        }

        //  Use this static method do one-time import of a voxel map
        public static void Run(MyVoxelMap voxelMap, MyModelObj model)
        {
            MyVoxelImport voxelMapImport = new MyVoxelImport(voxelMap, model, MyVoxelImportOptions.None);
        }

        //  Use this static method do one-time import of a voxel map
        public static void Run(MyVoxelMap voxelMap, MyModelObj model, MyVoxelImportOptions importOptions)
        {
            MyVoxelImport voxelMapImport = new MyVoxelImport(voxelMap, model, importOptions);
        }

        public static void Run(MyVoxelMap voxelMap, MyModel model, MyvoxelImportAction importAction, Matrix modelWorld, float modelScale, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            MyVoxelImport voxelImport = new MyVoxelImport(voxelMap, model, importAction, modelWorld, modelScale, voxelMaterial, ref changed);
        }

        public static void Run(MyVoxelMap voxelMap, Vector3[] vertexes, MyTriangleVertexIndices[] triangles, MyvoxelImportAction importAction, Matrix modelWorld, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            MyVoxelImport voxelImport = new MyVoxelImport(voxelMap, vertexes, triangles, importAction, modelWorld, voxelMaterial, ref changed);
        }

        MyVoxelImport()
        {
            //  This class can be instantiated only by parameter constructor and called from static method Run()
            throw new NotImplementedException();
        }

        //  Model will be scaled/translated to fit voxel map size (X, Y and Z)
        MyVoxelImport(MyVoxelMap voxelMap, string modelName, MyVoxelImportOptions importOptions)
        {
            //  Load model, get triangles
            LoadModel(modelName);

            //  Rescale the model so it fits voxelMap (three directions!!!)
            RescaleModel(voxelMap, importOptions);

            //  Fill lookup array with triangles located at specified voxel positions. Array is 2D.
            FillTrianglesLookup(voxelMap);

            //  Create XZ map where every voxel center gets list of triangles that lie on its Y line
            //      Do this by iterating over all triangles and making references to them from XZ map
            Import(voxelMap);
        }

        //  Model will be scaled/translated to fit voxel map size (X, Y and Z)
        MyVoxelImport(MyVoxelMap voxelMap, MyModel model, MyVoxelImportOptions importOptions)
        {
            //  Load model, get triangles
            LoadModel(model);

            //  Rescale the model so it fits voxelMap (three directions!!!)
            RescaleModel(voxelMap, importOptions);

            //  Fill lookup array with triangles located at specified voxel positions. Array is 2D.
            FillTrianglesLookup(voxelMap);

            //  Create XZ map where every voxel center gets list of triangles that lie on its Y line
            //      Do this by iterating over all triangles and making references to them from XZ map
            Import(voxelMap);
        }

        //  Model will be scaled/translated to fit voxel map size (X, Y and Z)
        MyVoxelImport(MyVoxelMap voxelMap, MyModelObj model, MyVoxelImportOptions importOptions)
        {
            //  Load model, get triangles
            LoadModel(model);

            //  Rescale the model so it fits voxelMap (three directions!!!)
            RescaleModel(voxelMap, importOptions);

            //  Fill lookup array with triangles located at specified voxel positions. Array is 2D.
            FillTrianglesLookup(voxelMap);

            //  Create XZ map where every voxel center gets list of triangles that lie on its Y line
            //      Do this by iterating over all triangles and making references to them from XZ map
            Import(voxelMap);
        }

        MyVoxelImport(MyVoxelMap voxelMap, Vector3[] vertexes, MyTriangleVertexIndices[] triangles, MyvoxelImportAction importAction, Matrix modelWorld, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("voxel import");

            // Load model, get triangles transformed to model's world matrix
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("load model");
            LoadModel(vertexes, triangles, modelWorld);

            //  Fill lookup array with triangles located at specified voxel positions. Array is 2D.
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("fill lookup");
            FillTrianglesLookup(voxelMap);

            // Performs action
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("perform action");
            PerformAction(voxelMap, importAction, voxelMaterial, ref changed);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        MyVoxelImport(MyVoxelMap voxelMap, MyModel model, MyvoxelImportAction importAction, Matrix modelWorld, float modelScale, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            // Load model, get triangles transformed to model's world matrix
            LoadModel(model, modelWorld, modelScale);

            RescaleModel(voxelMap, MyVoxelImportOptions.KeepScale);

            //  Fill lookup array with triangles located at specified voxel positions. Array is 2D.
            FillTrianglesLookup(voxelMap);

            // Performs action
           //PerformAction(voxelMap, importAction, voxelMaterial, ref changed);
            Import(voxelMap);
        }

        void LoadModel(Vector3[] vertexes, MyTriangleVertexIndices[] triangles, Matrix world)
        {
            m_minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            m_triangles = new List<MyImportTriangle>(triangles.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                Vector3 vertex0 = Vector3.Transform(vertexes[triangles[i].I0], world);
                Vector3 vertex1 = Vector3.Transform(vertexes[triangles[i].I1], world);
                Vector3 vertex2 = Vector3.Transform(vertexes[triangles[i].I2], world);

                MyImportTriangle triangle = new MyImportTriangle(vertex0, vertex1, vertex2);
                //  Ignore triangles that lie in XZ plane
                if (triangle.Normal.Y != 0)
                {
                    m_triangles.Add(triangle);
                    CheckMinMaxCoords(vertex0);
                    CheckMinMaxCoords(vertex1);
                    CheckMinMaxCoords(vertex2);
                }
            }
        }

        void LoadModel(MyModel model, Matrix modelWorld, float modelScale)
        {
            Matrix world = modelWorld * Matrix.CreateScale(modelScale);

            m_minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            MyTriangleVertexIndices[] triangles = model.Triangles;
            m_triangles = new List<MyImportTriangle>(triangles.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                Vector3 vertex0 = Vector3.Transform(model.GetVertex(triangles[i].I0), world);
                Vector3 vertex1 = Vector3.Transform(model.GetVertex(triangles[i].I1), world);
                Vector3 vertex2 = Vector3.Transform(model.GetVertex(triangles[i].I2), world);

                MyImportTriangle triangle = new MyImportTriangle(vertex0, vertex1, vertex2);
                //  Ignore triangles that lie in XZ plane
                if (triangle.Normal.Y != 0)
                {
                    m_triangles.Add(triangle);
                    CheckMinMaxCoords(vertex0);
                    CheckMinMaxCoords(vertex1);
                    CheckMinMaxCoords(vertex2);
                }
            }
        }

        void LoadModel(MyModel model)
        {
            model.LoadData();

            m_minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            MyTriangleVertexIndices[] triangles = model.Triangles;
            m_triangles = new List<MyImportTriangle>(triangles.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                Vector3 vertex0 = model.GetVertex(triangles[i].I0);
                Vector3 vertex1 = model.GetVertex(triangles[i].I1);
                Vector3 vertex2 = model.GetVertex(triangles[i].I2);

                MyImportTriangle triangle = new MyImportTriangle(vertex0, vertex1, vertex2);
                //  Ignore triangles that lie in XZ plane
                if (triangle.Normal.Y != 0)
                {
                    m_triangles.Add(triangle);
                    CheckMinMaxCoords(vertex0);
                    CheckMinMaxCoords(vertex1);
                    CheckMinMaxCoords(vertex2);
                }
            }
        }

        void LoadModel(MyModelObj model)
        {
            m_minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Vector3[] vertices = model.Vertexes.ToArray();
            MyTriangleVertexIndices[] triangles = model.Triangles.ToArray();
            m_triangles = new List<MyImportTriangle>(triangles.Length);

            for (int i = 0; i < triangles.Length; i++)
            {
                Vector3 vertex0 = vertices[triangles[i].I0];
                Vector3 vertex1 = vertices[triangles[i].I1];
                Vector3 vertex2 = vertices[triangles[i].I2];

                MyImportTriangle triangle = new MyImportTriangle(vertex0, vertex1, vertex2);
                //  Ignore triangles that lie in XZ plane
                if (triangle.Normal.Y != 0)
                {
                    m_triangles.Add(triangle);
                    CheckMinMaxCoords(vertex0);
                    CheckMinMaxCoords(vertex1);
                    CheckMinMaxCoords(vertex2);
                }
            }
        }

        void LoadModel(string modelName)
        {
            //LoadModel(MyMinerGame.Static.Content.Load<MyModel>(modelName));
        }

        void CheckMinMaxCoords(Vector3 vertex)
        {
            if (vertex.X < m_minCoord.X) m_minCoord.X = vertex.X;
            if (vertex.Y < m_minCoord.Y) m_minCoord.Y = vertex.Y;
            if (vertex.Z < m_minCoord.Z) m_minCoord.Z = vertex.Z;
            if (vertex.X > m_maxCoord.X) m_maxCoord.X = vertex.X;
            if (vertex.Y > m_maxCoord.Y) m_maxCoord.Y = vertex.Y;
            if (vertex.Z > m_maxCoord.Z) m_maxCoord.Z = vertex.Z;
        }

        //  Rescales the model to size specified by 'sizeInMetres'
        void RescaleModel(MyVoxelMap voxelMap, MyVoxelImportOptions importOptions)
        {
            Vector3 originalSize = m_maxCoord - m_minCoord;
            Vector3 originalCenter = (m_maxCoord + m_minCoord) / 2.0f;

            m_minCoord = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            m_maxCoord = new Vector3(float.MinValue, float.MinValue, float.MinValue);


            //  We don't want to touch borders of a voxel map so we need to subtract size of data cell from each side.
            //  I have choosen data cell (not one or two voxels) because LOD - average content of some data cells that were too close to voxel map border 
            //  were almost full data cell so when converted as LOD, hole appear there (originaly I was just multiplying by 0.9, but that is wasting in case of large voxel maps).
            //Vector3 rescaleFactor = voxelMap.SizeInMetres / originalSize * 0.9f;
            Vector3 rescaleFactor;
            rescaleFactor.X = (voxelMap.SizeInMetres.X - 2 * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES) / originalSize.X;
            rescaleFactor.Y = (voxelMap.SizeInMetres.Y - 2 * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES) / originalSize.Y;
            rescaleFactor.Z = (voxelMap.SizeInMetres.Z - 2 * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES) / originalSize.Z;

            switch (importOptions)
            {
                case MyVoxelImportOptions.KeepAspectRatio:
                    rescaleFactor.X = rescaleFactor.Y = rescaleFactor.Z = Math.Min(Math.Min(rescaleFactor.X, rescaleFactor.Y), rescaleFactor.Z);
                    break;
                case MyVoxelImportOptions.KeepScale:
                    //Cannot keep scale if the voxel map isn't big enough to contain it, will keep aspect ratio instead
                    if (rescaleFactor.X >= 1 && rescaleFactor.Y >= 1 && rescaleFactor.Z >= 1)
                    {
                        rescaleFactor = Vector3.One;
                    }
                    else
                    {
                        rescaleFactor.X = rescaleFactor.Y = rescaleFactor.Z = Math.Min(Math.Min(rescaleFactor.X, rescaleFactor.Y), rescaleFactor.Z);
                    }
                    break;
            }

            for (int i = 0; i < m_triangles.Count; i++)
            {
                //  Translate vertexes to the origin
                m_triangles[i].Vertex0 -= originalCenter;
                m_triangles[i].Vertex1 -= originalCenter;
                m_triangles[i].Vertex2 -= originalCenter;

                //  Rescale vertexes
                m_triangles[i].Vertex0 *= rescaleFactor;
                m_triangles[i].Vertex1 *= rescaleFactor;
                m_triangles[i].Vertex2 *= rescaleFactor;

                //  Translate vertexes to voxel map position
                m_triangles[i].Vertex0 += voxelMap.PositionLeftBottomCorner + voxelMap.SizeInMetresHalf;
                m_triangles[i].Vertex1 += voxelMap.PositionLeftBottomCorner + voxelMap.SizeInMetresHalf;
                m_triangles[i].Vertex2 += voxelMap.PositionLeftBottomCorner + voxelMap.SizeInMetresHalf;

                CheckMinMaxCoords(m_triangles[i].Vertex0);
                CheckMinMaxCoords(m_triangles[i].Vertex1);
                CheckMinMaxCoords(m_triangles[i].Vertex2);
            }
        }

        //  Fill lookup array with triangles located at specified voxel positions. Array is 2D.
        void FillTrianglesLookup(MyVoxelMap voxelMap)
        {
            m_trianglesLookupSizeX = voxelMap.Size.X;
            m_trianglesLookupSizeZ = voxelMap.Size.Z;
            m_trianglesLookupElementSizeX = voxelMap.SizeInMetres.X / voxelMap.Size.X;
            m_trianglesLookupElementSizeZ = voxelMap.SizeInMetres.Z / voxelMap.Size.Z;
            m_trianglesLookup = new List<MyImportTriangle>[m_trianglesLookupSizeX, m_trianglesLookupSizeZ];

            //  Initialize array
            for (int x = 0; x < m_trianglesLookupSizeX; x++)
            {
                for (int z = 0; z < m_trianglesLookupSizeZ; z++)
                {
                    m_trianglesLookup[x, z] = new List<MyImportTriangle>(INITIAL_COUNT_OF_TRIANGLES_IN_GRID_POSITION);
                }
            }

            //  Iterate over all triangles and put them into correct array positions based on their coordinates
            for (int i = 0; i < m_triangles.Count; i++)
            {
                //  Get bounding rectangle for this triangleVertexes (in fact we need only 2D coordinates, but it's clearer to use 3D and ignore Y value)
                Vector3 minCoord = new Vector3(float.MaxValue, 0, float.MaxValue);
                Vector3 maxCoord = new Vector3(float.MinValue, 0, float.MinValue);
                CheckMinMaxCoords2d(ref minCoord, ref maxCoord, m_triangles[i].Vertex0);
                CheckMinMaxCoords2d(ref minCoord, ref maxCoord, m_triangles[i].Vertex1);
                CheckMinMaxCoords2d(ref minCoord, ref maxCoord, m_triangles[i].Vertex2);

                MyMwcVector2Int minCoordInt = GetTriangleLookupCoord(voxelMap, minCoord);
                MyMwcVector2Int maxCoordInt = GetTriangleLookupCoord(voxelMap, maxCoord);

                if (minCoordInt.X < 0) minCoordInt.X = 0;
                if (minCoordInt.Y < 0) minCoordInt.Y = 0;
                if (minCoordInt.X > m_trianglesLookupSizeX - 1) minCoordInt.X = m_trianglesLookupSizeX - 1;
                if (minCoordInt.Y > m_trianglesLookupSizeZ - 1) minCoordInt.Y = m_trianglesLookupSizeZ - 1;
                if (maxCoordInt.X < 0) maxCoordInt.X = 0;
                if (maxCoordInt.Y < 0) maxCoordInt.Y = 0;
                if (maxCoordInt.X > m_trianglesLookupSizeX - 1) maxCoordInt.X = m_trianglesLookupSizeX - 1;
                if (maxCoordInt.Y > m_trianglesLookupSizeZ - 1) maxCoordInt.Y = m_trianglesLookupSizeZ - 1;

                //  Iterate over all elements that may contain this triangleVertexes.
                //  Notice that we swap Y and Z. It's because MyMwcVector2Int has X and Y, but we are working on XZ plane. So it's still Z value, even if its name is Y.
                for (int x = minCoordInt.X; x <= maxCoordInt.X; x++)
                {
                    for (int z = minCoordInt.Y; z <= maxCoordInt.Y; z++)
                    {
                        m_trianglesLookup[x, z].Add(m_triangles[i]);
                    }
                }
            }
        }

        //  We splited voxel map to more grids because if voxel map is large, it can't fit in one grid.
        //  So here we iterate over this grid. Everytime clear the grid, fill with triangleVertexes intersections and then set voxels.
        void Import(MyVoxelMap voxelMap)
        {
            //  Voxel map size must be multiple of grid size. Or, whole grid must fit exactly into X times into voxel map, without crossing border!
            MyCommonDebugUtils.AssertRelease((voxelMap.Size.X % VOXELS_IN_GRID_IN_ONE_DIRECTION) == 0);

            //  Voxel map size must be multiple of grid size. Or, whole grid must fit exactly into X times into voxel map, without crossing border!
            MyCommonDebugUtils.AssertRelease((voxelMap.Size.Z % VOXELS_IN_GRID_IN_ONE_DIRECTION) == 0);

            int gridsCountX = voxelMap.Size.X / VOXELS_IN_GRID_IN_ONE_DIRECTION;
            int gridsCountZ = voxelMap.Size.Z / VOXELS_IN_GRID_IN_ONE_DIRECTION;

            //  Space between two grid points in metres
            m_gridPointsSize = MyVoxelConstants.VOXEL_SIZE_IN_METRES / (float)GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION;
            m_gridPointsSizeHalf = m_gridPointsSize / 2.0f;


            for (int gridX = 0; gridX < gridsCountX; gridX++)
            {
                for (int gridZ = 0; gridZ < gridsCountZ; gridZ++)
                {
                    ImportGrid(voxelMap, gridX, gridZ);
                }
            }
        }

        // Returns temporary in/out values for voxels
        int[, ,] GetVoxelContentSum(MyVoxelMap voxelMap, int gridX, int gridZ)
        {
            int gridStartPointX = gridX * MAX_GRID_SIZE_IN_ONE_DIRECTION;
            int gridStartPointZ = gridZ * MAX_GRID_SIZE_IN_ONE_DIRECTION;

            //  Here we store intersections with line and triangles and specified grid point
            List<MyImportIntersection> intersections = new List<MyImportIntersection>(INITIAL_COUNT_OF_INTERSECTIONS);

            //  Here we store temporary in/out values for voxels. After that, we convert it to common voxel content values (0..255)
            int[, ,] voxelContentSum = null; 

            for (int gridPointX = 0; gridPointX < MAX_GRID_SIZE_IN_ONE_DIRECTION; gridPointX++)
            {
                for (int gridPointZ = 0; gridPointZ < MAX_GRID_SIZE_IN_ONE_DIRECTION; gridPointZ++)
                {
                    Vector3 gridCoord = GetGridCoord(voxelMap, gridStartPointX + gridPointX, 0, gridStartPointZ + gridPointZ);
                    if (gridCoord.X < m_minCoord.X || gridCoord.Z < m_minCoord.Z || gridCoord.X > m_maxCoord.X || gridCoord.Z > m_maxCoord.Z)
                    {
                        continue;
                    }

                    if (voxelContentSum == null)
                        voxelContentSum = new int[VOXELS_IN_GRID_IN_ONE_DIRECTION, voxelMap.Size.Y, VOXELS_IN_GRID_IN_ONE_DIRECTION];

                    MyMwcVector2Int triangleLookupCoord = GetTriangleLookupCoord(voxelMap, gridCoord);

                    //  We need to clear list of intersections
                    intersections.Clear();

                    //  Get triangles that lie on this grid point/line
                    List<MyImportTriangle> triangles = m_trianglesLookup[triangleLookupCoord.X, triangleLookupCoord.Y];
                    for (int i = 0; i < triangles.Count; i++)
                    {
                        //  Ray is always in Y-axis direction
                        //MyLine line = new MyLine(gridCoord, gridCoord + Vector3.Up * 100000, false);
                        MyLine line = new MyLine(new Vector3(gridCoord.X, m_minCoord.Y, gridCoord.Z) + Vector3.Down * 10, new Vector3(gridCoord.X, m_maxCoord.Y, gridCoord.Z) + Vector3.Up * 10, false);

                        MyTriangle_Vertexes triangle;
                        triangle.Vertex0 = triangles[i].Vertex0;
                        triangle.Vertex1 = triangles[i].Vertex1;
                        triangle.Vertex2 = triangles[i].Vertex2;

                        float? distance = MyUtils.GetLineTriangleIntersection(ref line, ref triangle);
                        if (distance.HasValue == true)
                        {
                            intersections.Add(new MyImportIntersection(line.From + line.Direction * distance.Value, triangles[i].Normal, distance.Value));
                        }
                    }

                    //  SortForSAP intersections by their distance from the origin (from the grid point)
                    intersections.Sort(delegate(MyImportIntersection p1, MyImportIntersection p2) { return p1.Distance.CompareTo(p2.Distance); });

                    int lastY = 0;
                    bool contentSwitch = false;     //  This tells us if we will add empty or full voxels. False = until now it's empty. True = it full.                    
                    for (int i = 0; i < intersections.Count; i++)
                    {
                        Vector3 tempGridCoord = GetGridCoord(voxelMap, gridStartPointX + gridPointX, lastY, gridStartPointZ + gridPointZ);
                        int length = (int)((intersections[i].Intersection.Y - tempGridCoord.Y) / m_gridPointsSize);
                        // this is here, because we must find intersection with triangle out of borders of voxel map, because we can use voxel hand at the borders
                        if (length < 0)
                        {
                            contentSwitch = !contentSwitch;
                            continue;
                        }

                        for (int y = lastY; y < (lastY + length); y++)
                        {
                            MyMwcVector3Int voxelCoord = voxelMap.GetVoxelCoordinateFromMeters(GetGridCoord(voxelMap, gridPointX, y, gridPointZ));
                            if (voxelCoord.Y < voxelContentSum.GetLength(1) &&
                               voxelCoord.Y >= 0)
                            {
                                voxelContentSum[voxelCoord.X, voxelCoord.Y, voxelCoord.Z] += (contentSwitch == false) ? 0 : 1;
                            }
                        }

                        contentSwitch = !contentSwitch;
                        lastY = lastY + length;
                    }
                }
            }

            return voxelContentSum;
        }

        //  Import one individual grid
        void ImportGrid(MyVoxelMap voxelMap, int gridX, int gridZ)
        {
            int[, ,] voxelContentSum = GetVoxelContentSum(voxelMap, gridX, gridZ);

            //  Transform grid values to voxels
            int voxelStartX = gridX * VOXELS_IN_GRID_IN_ONE_DIRECTION;
            int voxelStartZ = gridZ * VOXELS_IN_GRID_IN_ONE_DIRECTION;
            for (int x = 0; x < VOXELS_IN_GRID_IN_ONE_DIRECTION; x++)
            {
                for (int y = 0; y < voxelMap.Size.Y; y++)
                {
                    for (int z = 0; z < VOXELS_IN_GRID_IN_ONE_DIRECTION; z++)
                    {
                        MyMwcVector3Int voxelCoord = new MyMwcVector3Int(voxelStartX + x, y, voxelStartZ + z);

                        float content = 0;

                        if (voxelContentSum != null)
                            content = (float)voxelContentSum[x, y, z] / (float)GRID_POINTS_IN_ONE_VOXEL_TOTAL;

                        //  Content value must be in interval <0..1>
                        MyCommonDebugUtils.AssertRelease((content >= 0.0f) && (content <= 1.0f));

                        voxelMap.SetVoxelContent((byte)(content * MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT), ref voxelCoord);
                    }
                }
            }
        }

        void PerformAction(MyVoxelMap voxelMap, MyvoxelImportAction action, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PerformAction");

            //  Voxel map size must be multiple of grid size. Or, whole grid must fit exactly into X times into voxel map, without crossing border!
            MyCommonDebugUtils.AssertRelease((voxelMap.Size.X % VOXELS_IN_GRID_IN_ONE_DIRECTION) == 0);

            //  Voxel map size must be multiple of grid size. Or, whole grid must fit exactly into X times into voxel map, without crossing border!
            MyCommonDebugUtils.AssertRelease((voxelMap.Size.Z % VOXELS_IN_GRID_IN_ONE_DIRECTION) == 0);

            int gridsCountX = voxelMap.Size.X / VOXELS_IN_GRID_IN_ONE_DIRECTION;
            int gridsCountZ = voxelMap.Size.Z / VOXELS_IN_GRID_IN_ONE_DIRECTION;

            //  Space between two grid points in metres
            m_gridPointsSize = MyVoxelConstants.VOXEL_SIZE_IN_METRES / (float)GRID_POINTS_IN_ONE_VOXEL_IN_ONE_DIRECTION;
            m_gridPointsSizeHalf = m_gridPointsSize / 2.0f;

            //  Get min corner of the box
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(m_minCoord);

            //  Get max corner of the box
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(m_maxCoord);

            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            //  We are tracking which voxels were changed, so we can invalidate only needed cells in the cache
            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            bool contentChanged = false;

            for (int gridX = 0; gridX < gridsCountX; gridX++)
            {
                for (int gridZ = 0; gridZ < gridsCountZ; gridZ++)
                {
                    if (PerformGridAction(voxelMap, action, gridX, gridZ, ref minChanged, ref maxChanged, voxelMaterial, ref changed))
                    {
                        contentChanged = true;
                    }
                }
            }

            if (contentChanged)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                voxelMap.FixVoxelCoord(ref minChanged);
                voxelMap.FixVoxelCoord(ref maxChanged);

                voxelMap.InvalidateCache(minChanged, maxChanged);
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        bool PerformGridAction(MyVoxelMap voxelMap, MyvoxelImportAction action, int gridX, int gridZ, ref MyMwcVector3Int minChanged, ref MyMwcVector3Int maxChanged, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PerformGridAction");

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("get content sum");
            int[, ,] voxelContentSum = GetVoxelContentSum(voxelMap, gridX, gridZ);
            if (voxelContentSum == null)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                return false;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("loop");
            bool anyContentChanged = false;

            //  Transform grid values to voxels
            int voxelStartX = gridX * VOXELS_IN_GRID_IN_ONE_DIRECTION;
            int voxelStartZ = gridZ * VOXELS_IN_GRID_IN_ONE_DIRECTION;


            switch (action)
            {
                case MyvoxelImportAction.AddVoxels:
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyvoxelImportAction.AddVoxels");
                    for (int x = 0; x < VOXELS_IN_GRID_IN_ONE_DIRECTION; x++) for (int y = 0; y < voxelMap.Size.Y; y++) for (int z = 0; z < VOXELS_IN_GRID_IN_ONE_DIRECTION; z++)
                    {
                        MyMwcVector3Int voxelCoord = new MyMwcVector3Int(voxelStartX + x, y, voxelStartZ + z);

                        byte newContent =  (byte)(voxelContentSum[x, y, z] * MyVoxelConstants.VOXEL_CONTENT_FULL / GRID_POINTS_IN_ONE_VOXEL_TOTAL);
                        if (newContent == 0) continue;

                        bool c = PerformAddVoxels(voxelMap, voxelCoord, newContent, ref changed);
                        c |= PerformChangeMaterialVoxels(voxelMap, voxelCoord, voxelMaterial, ref changed);
                        if (c)
                        {
                            anyContentChanged = true;
                            if (voxelCoord.X < minChanged.X) minChanged.X = voxelCoord.X;
                            if (voxelCoord.Y < minChanged.Y) minChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z < minChanged.Z) minChanged.Z = voxelCoord.Z;
                            if (voxelCoord.X > maxChanged.X) maxChanged.X = voxelCoord.X;
                            if (voxelCoord.Y > maxChanged.Y) maxChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z > maxChanged.Z) maxChanged.Z = voxelCoord.Z;
                        }
                    }
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    break;

                case MyvoxelImportAction.RemoveVoxels:
                    for (int x = 0; x < VOXELS_IN_GRID_IN_ONE_DIRECTION; x++) for (int y = 0; y < voxelMap.Size.Y; y++) for (int z = 0; z < VOXELS_IN_GRID_IN_ONE_DIRECTION; z++)
                    {
                        MyMwcVector3Int voxelCoord = new MyMwcVector3Int(voxelStartX + x, y, voxelStartZ + z);

                        byte newContent = (byte)(voxelContentSum[x, y, z] * MyVoxelConstants.VOXEL_CONTENT_FULL / GRID_POINTS_IN_ONE_VOXEL_TOTAL);
                        if (newContent == 0) continue;

                        if (PerformRemoveVoxels(voxelMap, voxelCoord, newContent, ref changed))
                        {
                            anyContentChanged = true;
                            if (voxelCoord.X < minChanged.X) minChanged.X = voxelCoord.X;
                            if (voxelCoord.Y < minChanged.Y) minChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z < minChanged.Z) minChanged.Z = voxelCoord.Z;
                            if (voxelCoord.X > maxChanged.X) maxChanged.X = voxelCoord.X;
                            if (voxelCoord.Y > maxChanged.Y) maxChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z > maxChanged.Z) maxChanged.Z = voxelCoord.Z;
                        }
                    }
                    break;

                case MyvoxelImportAction.ChangeMaterial:
                    for (int x = 0; x < VOXELS_IN_GRID_IN_ONE_DIRECTION; x++) for (int y = 0; y < voxelMap.Size.Y; y++) for (int z = 0; z < VOXELS_IN_GRID_IN_ONE_DIRECTION; z++)
                    {
                        MyMwcVector3Int voxelCoord = new MyMwcVector3Int(voxelStartX + x, y, voxelStartZ + z);

                        byte newContent = (byte)(voxelContentSum[x, y, z] * MyVoxelConstants.VOXEL_CONTENT_FULL / GRID_POINTS_IN_ONE_VOXEL_TOTAL);
                        if (newContent == 0) continue;

                        if (PerformChangeMaterialVoxels(voxelMap, voxelCoord, voxelMaterial, ref changed))
                        {
                            anyContentChanged = true;
                            if (voxelCoord.X < minChanged.X) minChanged.X = voxelCoord.X;
                            if (voxelCoord.Y < minChanged.Y) minChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z < minChanged.Z) minChanged.Z = voxelCoord.Z;
                            if (voxelCoord.X > maxChanged.X) maxChanged.X = voxelCoord.X;
                            if (voxelCoord.Y > maxChanged.Y) maxChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z > maxChanged.Z) maxChanged.Z = voxelCoord.Z;
                        }
                    }
                    break;

                case MyvoxelImportAction.SoftenVoxels:
                    for (int x = 0; x < VOXELS_IN_GRID_IN_ONE_DIRECTION; x++) for (int y = 0; y < voxelMap.Size.Y; y++) for (int z = 0; z < VOXELS_IN_GRID_IN_ONE_DIRECTION; z++)
                    {
                        MyMwcVector3Int voxelCoord = new MyMwcVector3Int(voxelStartX + x, y, voxelStartZ + z);

                        byte newContent = (byte)(voxelContentSum[x, y, z] * MyVoxelConstants.VOXEL_CONTENT_FULL / GRID_POINTS_IN_ONE_VOXEL_TOTAL);
                        if (newContent == 0) continue;

                        if (PerformSoftenVoxels(voxelMap, voxelCoord, ref changed))
                        {
                            anyContentChanged = true;
                            if (voxelCoord.X < minChanged.X) minChanged.X = voxelCoord.X;
                            if (voxelCoord.Y < minChanged.Y) minChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z < minChanged.Z) minChanged.Z = voxelCoord.Z;
                            if (voxelCoord.X > maxChanged.X) maxChanged.X = voxelCoord.X;
                            if (voxelCoord.Y > maxChanged.Y) maxChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z > maxChanged.Z) maxChanged.Z = voxelCoord.Z;
                        }
                    }
                    break;

                case MyvoxelImportAction.WrinkleVoxels:
                    for (int x = 0; x < VOXELS_IN_GRID_IN_ONE_DIRECTION; x++) for (int y = 0; y < voxelMap.Size.Y; y++) for (int z = 0; z < VOXELS_IN_GRID_IN_ONE_DIRECTION; z++)
                    {
                        MyMwcVector3Int voxelCoord = new MyMwcVector3Int(voxelStartX + x, y, voxelStartZ + z);

                        byte newContent = (byte)(voxelContentSum[x, y, z] * MyVoxelConstants.VOXEL_CONTENT_FULL / GRID_POINTS_IN_ONE_VOXEL_TOTAL);

                        if (newContent == 0) continue;

                        if (PerformWrinkleVoxels(voxelMap, voxelCoord, ref changed))
                        {
                            anyContentChanged = true;
                            if (voxelCoord.X < minChanged.X) minChanged.X = voxelCoord.X;
                            if (voxelCoord.Y < minChanged.Y) minChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z < minChanged.Z) minChanged.Z = voxelCoord.Z;
                            if (voxelCoord.X > maxChanged.X) maxChanged.X = voxelCoord.X;
                            if (voxelCoord.Y > maxChanged.Y) maxChanged.Y = voxelCoord.Y;
                            if (voxelCoord.Z > maxChanged.Z) maxChanged.Z = voxelCoord.Z;
                        }
                    }
                    break;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return anyContentChanged;
        }

        bool PerformAddVoxels(MyVoxelMap voxelMap, MyMwcVector3Int voxelCoord, byte content, ref bool changed)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PerformAddVoxels");

            bool contentChanged = false;
            byte originalContent = voxelMap.GetVoxelContent(ref voxelCoord);
            if (content > originalContent)
            {
                changed = true;
                voxelMap.SetVoxelContent(content, ref voxelCoord);
                contentChanged = true;
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return contentChanged;
        }

        bool PerformRemoveVoxels(MyVoxelMap voxelMap, MyMwcVector3Int voxelCoord, byte content, ref bool changed)
        {
            bool contentChanged = false;
            byte originalContent = voxelMap.GetVoxelContent(ref voxelCoord);
            if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY && content > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
            {
                int newVal = originalContent - content;
                if (newVal < MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                {
                    newVal = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                }
                changed = true;
                voxelMap.SetVoxelContent((byte)newVal, ref voxelCoord);
                contentChanged = true;
            }
            return contentChanged;
        }

        bool PerformChangeMaterialVoxels(MyVoxelMap voxelMap, MyMwcVector3Int voxelCoord, MyMwcVoxelMaterialsEnum? voxelMaterial, ref bool changed)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PerformChangeMaterialVoxels");
            if (voxelMaterial != null)
            {
                byte originalContent = voxelMap.GetVoxelContent(ref voxelCoord);
                if (originalContent == MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                {
                    // if there are no voxel content then do nothing
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    return false;
                }

                MyMwcVoxelMaterialsEnum originalMaterial;
                byte originalIndestructibleContent;
                voxelMap.GetMaterialAndIndestructibleContent(ref voxelCoord, out originalMaterial,
                                                             out originalIndestructibleContent);
                if (originalMaterial == voxelMaterial.Value)
                {
                    // if original material is same as new material then do nothing
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    return false;
                }

                byte indestructibleContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;

                voxelMap.SetVoxelMaterialAndIndestructibleContent(voxelMaterial.Value, indestructibleContent, ref voxelCoord);
                changed = true;
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                return true;
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return false;
        }

        bool PerformSoftenVoxels(MyVoxelMap voxelMap, MyMwcVector3Int voxelCoord, ref bool changed)
        {
            bool contentChanged = false;
            byte originalContent = voxelMap.GetVoxelContent(ref voxelCoord);
            if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
            {
                changed = true;
                voxelMap.SoftenVoxelContent(voxelCoord, MyVoxelConstants.DEFAULT_SOFTEN_WEIGHT);
                contentChanged = true;
            }
            return contentChanged;
        }

        bool PerformWrinkleVoxels(MyVoxelMap voxelMap, MyMwcVector3Int voxelCoord, ref bool changed)
        {
            bool contentChanged = false;
            byte originalContent = voxelMap.GetVoxelContent(ref voxelCoord);

            voxelMap.WrinkleVoxelContent(voxelCoord, MyVoxelConstants.DEFAULT_WRINKLE_WEIGHT_ADD, MyVoxelConstants.DEFAULT_WRINKLE_WEIGHT_REMOVE);
            byte newContent = voxelMap.GetVoxelContent(ref voxelCoord);
            if (originalContent != newContent)
            {
                contentChanged = true;
                changed = true;
            }

            return contentChanged;
        }

        Vector3 GetGridCoord(MyVoxelMap voxelMap, int gridPointX, int gridPointY, int gridPointZ)
        {
            return voxelMap.PositionLeftBottomCorner + new Vector3(
                gridPointX * m_gridPointsSize + m_gridPointsSizeHalf,
                gridPointY * m_gridPointsSize + m_gridPointsSizeHalf,
                gridPointZ * m_gridPointsSize + m_gridPointsSizeHalf);
        }

        void CheckMinMaxCoords2d(ref Vector3 minCoord, ref Vector3 maxCoord, Vector3 vertex)
        {
            if (vertex.X < minCoord.X) minCoord.X = vertex.X;
            if (vertex.Z < minCoord.Z) minCoord.Z = vertex.Z;
            if (vertex.X > maxCoord.X) maxCoord.X = vertex.X;
            if (vertex.Z > maxCoord.Z) maxCoord.Z = vertex.Z;
        }

        //  Converts vertex from 'world coordinates' to 'triangleVertexes lookup array coordinates'. 
        MyMwcVector2Int GetTriangleLookupCoord(MyVoxelMap voxelMap, Vector3 vertex)
        {
            return new MyMwcVector2Int(
                (int)((vertex.X - voxelMap.PositionLeftBottomCorner.X) / m_trianglesLookupElementSizeX),
                (int)((vertex.Z - voxelMap.PositionLeftBottomCorner.Z) / m_trianglesLookupElementSizeZ));
        }
    }
}

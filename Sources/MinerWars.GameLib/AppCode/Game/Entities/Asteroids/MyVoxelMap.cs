//#define DETECT_POTENCIAL_COLLISIONS_CALLS

using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Physics;

namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using App;
    using CommonLIB.AppCode.Networking;
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Voxels;
    using CommonLIB.AppCode.Utils;
    using MinerWarsMath;
    using MinerWarsMath.Graphics;
    using Models;
    using TransparentGeometry;
    using SysUtils;
    using SysUtils.Utils;
    using Utils;
    using VoxelHandShapes;
    using Voxels;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.Effects;
    using MinerWars.AppCode.Game.Managers;
    using MinerWars.AppCode.Game.Audio;
    
    using MinerWars.CommonLIB.AppCode.Generics;
    using BulletXNA.LinearMath;
    using System.Linq;
    //using System.Threading;
    using MinerWars.AppCode.Game.Managers.Session;
    using ParallelTasks;
    using MinerWars.AppCode.Game.TransparentGeometry.Particles;
    using MinerWars.AppCode.Physics.Collisions;
    using KeenSoftwareHouse.Library.Parallelization.Threading;
    using MinerWars.CommonLIB.AppCode.Import;

    delegate void OnVoxelHandShapeCountChange(int changeCount);

    class MyVoxelMap : MyEntity
    {
        //  Voxel map ID must be unique, now just on client side, but later maybe on server too
        public int VoxelMapId;

#if DETECT_POTENCIAL_COLLISIONS_CALLS
        List<BoundingBox> m_potencianCollsBBs = new List<BoundingBox>();
#endif

        //  Position of left/bottom corner of this voxel map, in world space (not relative to sector)
        public Vector3 PositionLeftBottomCorner;

        //  Size of voxel map (in voxels)
        public MyMwcVector3Int Size;
        public MyMwcVector3Int SizeMinusOne;

        //  Size of voxel map (in metres)
        public Vector3 SizeInMetres;
        public Vector3 SizeInMetresHalf;

        //  Count of voxel data cells in all directions
        public MyMwcVector3Int DataCellsCount;
        public MyMwcVector3Int DataCellsCountMinusOne;

        //  Count of voxel render cells in all directions
        public MyMwcVector3Int RenderCellsCount;
        public MyMwcVector3Int RenderCellsCountMinusOne;

        public static readonly List<int> AsteroidSizes = new List<int>()
        {
            (int)(64  * MyVoxelConstants.VOXEL_SIZE_IN_METRES),
            (int)(128 * MyVoxelConstants.VOXEL_SIZE_IN_METRES),
            (int)(256 * MyVoxelConstants.VOXEL_SIZE_IN_METRES),
            (int)(512 * MyVoxelConstants.VOXEL_SIZE_IN_METRES),
        };

        //  Array of voxel cells in this voxel map
        MyVoxelContentCell[][][] m_voxelContentCells;

        //  Here we store material for each voxel; And average material for data cell too (that is used for LOD)
        MyVoxelMaterialCell[][][] m_voxelMaterialCells;

        private List<MyVoxelHandShape> m_voxelHandShapes;
        private List<BoundingSphere> m_explosions;

        MyMeshMaterial m_fakeVoxelMaterial = new MyMeshMaterial("VoxelMaterial", null, null, null, null);

        public FastResourceLock ContentLock = new FastResourceLock();
        public FastResourceLock OreDepositsLock = new FastResourceLock();

        [ThreadStatic]
        private static List<int> m_octreeOverlapElementList;
        private static List<List<int>> m_octreeOverlapElementListCollection = new List<List<int>>();

        private static List<int> OctreeOverlapElementList
        {
            get
            {
                if (m_octreeOverlapElementList == null)
                {
                    m_octreeOverlapElementList = new List<int>(1024);
                    lock (m_octreeOverlapElementListCollection)
                    {
                        m_octreeOverlapElementListCollection.Add(m_octreeOverlapElementList);
                    }
                }
                return m_octreeOverlapElementList;
            }
        }

        
        // Used for fast data cell - line intersection
        [ThreadStatic]
        private static List<MyMwcVector3Int> m_sweepResult;
        private static List<List<MyMwcVector3Int>> m_sweepResultCollection = new List<List<MyMwcVector3Int>>();

        private static List<MyMwcVector3Int> SweepResult
        {
            get
            {
                if (m_sweepResult == null)
                {
                    m_sweepResult = new List<MyMwcVector3Int>(128);
                    lock (m_sweepResultCollection)
                    {
                        m_sweepResultCollection.Add(m_sweepResult);
                    }
                }
                return m_sweepResult;
            }
        }


        private bool m_isClosed = false; //This is probably temporary - now it is needed by undo/redo - voxel map needs to be completely closed/reinitialized for that

        //  Ore deposits fields        
        private Dictionary<Int64, MyVoxelMapOreDepositCell> m_oreDepositCells;
        int m_oreDepositCellsCountX = 0;
        int m_oreDepositCellsCountY = 0;
        int m_oreDepositCellsCountZ = 0;
        public List<MyVoxelMapOreDepositCell> OreDepositCellsContainsOre = new List<MyVoxelMapOreDepositCell>();

        public event OnVoxelHandShapeCountChange OnVoxelHandShapeCountChange;

        // Creates full voxel map, does not initialize base !!! Use only for importing voxel maps from models !!!
        public virtual void Init(Vector3 position, MyMwcVector3Int size, MyMwcVoxelMaterialsEnum material)
        {
            InitVoxelMap(position, size, material);
        }

        static byte[] buffer = new byte[MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_TOTAL];

        public virtual void Init(string hudLabelText, Vector3 position, MyMwcObjectBuilder_VoxelMap objectBuilder)
        {
            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("base init");

            base.Init(hudLabelTextSb, null, null, null, null, objectBuilder);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Decompress file");

            LoadFile(position, objectBuilder);
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private void LoadFile(Vector3 position, MyMwcObjectBuilder_VoxelMap objectBuilder)
        {
            MyCompressionFileLoad decompressFile = null;

            if (objectBuilder.VoxelData != null)
            {
                decompressFile = new MyCompressionFileLoad(objectBuilder.VoxelData);    
            }
            else
            {
                string voxelFilePath = MyVoxelFiles.Get(objectBuilder.VoxelFile).GetVoxFilePath();
                m_voxelFile = objectBuilder.VoxelFile;

                decompressFile = new MyCompressionFileLoad(voxelFilePath);
            }

            
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Decompress file header");

            //  Version of a VOX file
            int fileVersion = decompressFile.GetInt32();

            //  Not supported VOX file version
            MyCommonDebugUtils.AssertRelease(fileVersion == MyVoxelConstants.VOXEL_FILE_ACTUAL_VERSION);

            //  Size of this voxel map (in voxels)
            int sizeX = decompressFile.GetInt32();
            int sizeY = decompressFile.GetInt32();
            int sizeZ = decompressFile.GetInt32();

            //  Size of data cell in voxels. Has to be the same as current size specified by our constants.
            int cellSizeX = decompressFile.GetInt32();
            int cellSizeY = decompressFile.GetInt32();
            int cellSizeZ = decompressFile.GetInt32();
            MyCommonDebugUtils.AssertDebug(
                cellSizeX == MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS &&
                cellSizeY == MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS &&
                cellSizeZ == MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);

            int cellsCountX = sizeX / cellSizeX;
            int cellsCountY = sizeY / cellSizeY;
            int cellsCountZ = sizeZ / cellSizeZ;

            //  Init ore deposit cells
            if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
            {
                m_oreDepositCellsCountX = cellsCountX / MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS;
                m_oreDepositCellsCountY = cellsCountY / MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS;
                m_oreDepositCellsCountZ = cellsCountZ / MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS;

                m_oreDepositCells = new Dictionary<long, MyVoxelMapOreDepositCell>();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("InitVoxelMap");

            //  Init this voxel map (arrays are allocated, sizes calculated). It must be called before we start reading and seting voxels.
            InitVoxelMap(position, new MyMwcVector3Int(sizeX, sizeY, sizeZ), objectBuilder.VoxelMaterial);
            
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Cells foreach");

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < cellsCountX; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < cellsCountY; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < cellsCountZ; cellCoord.Z++)
                    {
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("cell");

                        MyVoxelCellType cellType = (MyVoxelCellType)decompressFile.GetByte();

                        //  Cell's are FULL by default, therefore we don't need to change them
                        if (cellType != MyVoxelCellType.FULL)
                        {
                            MyVoxelContentCell newCell = AddCell(ref cellCoord);

                            //  If cell is empty we don't need to set all its voxels to empty. Just allocate cell and set its type to empty.
                            if (cellType == MyVoxelCellType.EMPTY)
                            {
                                newCell.SetToEmpty();
                            }
                            else if (cellType == MyVoxelCellType.MIXED)
                            {
                                decompressFile.GetBytes(cellSizeX * cellSizeY * cellSizeZ, buffer);
                                if (!MyFakes.MWCURIOSITY)
                                {
                                    newCell.SetAllVoxelContents(buffer);
                                }
                            }
                        }
                        //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Materials + indestructible");

            if (!decompressFile.EndOfFile())
            {
                // Read materials and indestructible
                for (cellCoord.X = 0; cellCoord.X < cellsCountX; cellCoord.X++)
                {
                    for (cellCoord.Y = 0; cellCoord.Y < cellsCountY; cellCoord.Y++)
                    {
                        for (cellCoord.Z = 0; cellCoord.Z < cellsCountZ; cellCoord.Z++)
                        {
                            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("mat");
                            var matCell = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z];

                            bool isSingleMaterial = decompressFile.GetByte() == 1;
                            MyMwcVoxelMaterialsEnum material = MyMwcVoxelMaterialsEnum.Indestructible_01;
                            byte indestructibleContent = 0;

                            MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoordByDataCellCoord(ref cellCoord);

                            if (isSingleMaterial)
                            {
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("single material");
                                material = (MyMwcVoxelMaterialsEnum)decompressFile.GetByte();
                                indestructibleContent = MyVoxelMaterials.IsIndestructible(material) ? (byte)255 : (byte)0;
                                this.SetVoxelMaterialAndIndestructibleContentForWholeCell(material, indestructibleContent, ref cellCoord);
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            }
                            else
                            {
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("mixed material");                                

                                MyMwcVector3Int voxelCoordInCell;
                                for (voxelCoordInCell.X = 0; voxelCoordInCell.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.X++)
                                {
                                    for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Y++)
                                    {
                                        for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Z++)
                                        {
                                            material = (MyMwcVoxelMaterialsEnum)decompressFile.GetByte();
                                            indestructibleContent = decompressFile.GetByte();
                                            matCell.SetMaterialAndIndestructibleContent(material, indestructibleContent, ref voxelCoordInCell);
                                        }
                                    }
                                }
                                matCell.CalcAverageCellMaterial();
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            }
                            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        }
                    }
                }
            }

            // compute ore deposits
            if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("Compute ore deposits");

                for (cellCoord.X = 0; cellCoord.X < cellsCountX; cellCoord.X++)
                {
                    for (cellCoord.Y = 0; cellCoord.Y < cellsCountY; cellCoord.Y++)
                    {
                        for (cellCoord.Z = 0; cellCoord.Z < cellsCountZ; cellCoord.Z++)
                        {
                            MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoordByDataCellCoord(ref cellCoord);
                            var matCell = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z];
                            var contentCell = m_voxelContentCells[cellCoord.X][cellCoord.Y][cellCoord.Z];

                            if (matCell.IsSingleMaterialForWholeCell())
                            {
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("single material");
                                var material = matCell.GetAverageCellMaterial();
                                if (MyVoxelMapOreMaterials.IsRareOre(material))
                                {
                                    int content = (contentCell == null) ? MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL : contentCell.GetVoxelContentSum();
                                    if (GetOreDepositCell(ref oreDepositCellCoord) == null)
                                    {
                                        AddOreDepositCell(ref oreDepositCellCoord).SetOreContent(material, content);
                                    }
                                }
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            }
                            else
                            {
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("more materials");
                                // buffer one rare material
                                MyMwcVoxelMaterialsEnum bufferedMaterial = MyMwcVoxelMaterialsEnum.Ice_01;  // IMPORTANT: the default value must be a rare material
                                int bufferedContent = 0;

                                MyMwcVector3Int voxelCoordInCell;
                                for (voxelCoordInCell.X = 0; voxelCoordInCell.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.X++)
                                {
                                    for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Y++)
                                    {
                                        for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Z++)
                                        {
                                            var material = matCell.GetMaterial(ref voxelCoordInCell);
                                            if (material == bufferedMaterial)  // do we buffer this material?
                                            {
                                                int content = (contentCell == null) ? MyVoxelConstants.VOXEL_CONTENT_FULL : contentCell.GetVoxelContent(ref voxelCoordInCell);
                                                bufferedContent += content;  // yes: just update the content
                                            }
                                            else
                                            {
                                                if (MyVoxelMapOreMaterials.IsRareOre(material))  // skip non-rare materials
                                                {
                                                    int content = (contentCell == null) ? MyVoxelConstants.VOXEL_CONTENT_FULL : contentCell.GetVoxelContent(ref voxelCoordInCell);
                                                    if (content > 0)  // skip empty cells
                                                    {
                                                        // new rare material: if there's any old buffered content, add it to the deposit
                                                        if (bufferedContent > 0 && GetOreDepositCell(ref oreDepositCellCoord) == null)
                                                        {
                                                            AddOreDepositCell(ref oreDepositCellCoord).SetOreContent(bufferedMaterial, bufferedContent);
                                                        }

                                                        bufferedMaterial = material;
                                                        bufferedContent = content;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (bufferedContent > 0 && GetOreDepositCell(ref oreDepositCellCoord) == null)  // if there's any buffered content left, add it to the deposit
                                {
                                    AddOreDepositCell(ref oreDepositCellCoord).SetOreContent(bufferedMaterial, bufferedContent);
                                }
                                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            }
                        }
                    }
                }
            }
        }

        public void OptimizeSize()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("OptimizeSize");

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                    {
                        var matCell = this.GetVoxelMaterialCells()[cellCoord.X][cellCoord.Y][cellCoord.Z];
                        matCell.OptimizeSize();
                    }
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  This method initializes voxel map (size, position, etc) but doesn't load voxels
        //  It only presets all materials to values specified in 'defaultMaterial' - so it will become material everywhere.
        void InitVoxelMap(Vector3 position, MyMwcVector3Int size, MyMwcVoxelMaterialsEnum material)
        {
            MyMwcLog.WriteLine("MyVoxelMap.InitVoxelMap() - Start");
            MyMwcLog.IncreaseIndent();

            VoxelMaterial = material;

            VoxelMapId = MyVoxelMaps.GetUniqueVoxelMapId();

            MyMwcLog.WriteLine("ID: " + VoxelMapId, LoggingOptions.VOXEL_MAPS);
            MyMwcLog.WriteLine("Size: " + MyUtils.GetFormatedVector3Int(size), LoggingOptions.VOXEL_MAPS);

            //  If you need more voxel maps, enlarge this constant.
            MyCommonDebugUtils.AssertRelease(VoxelMapId <= MyVoxelConstants.MAX_VOXEL_MAP_ID);

            Size = size;
            SizeMinusOne = new MyMwcVector3Int(Size.X - 1, Size.Y - 1, Size.Z - 1);

            SizeInMetres = GetVoxelSizeInMetres(ref size);
            SizeInMetresHalf = SizeInMetres / 2.0f;

            LocalAABB = new BoundingBox(-SizeInMetresHalf, SizeInMetresHalf);

            PositionLeftBottomCorner = position;
            SetWorldMatrix(Matrix.CreateTranslation(PositionLeftBottomCorner + SizeInMetresHalf));

            Flags |= EntityFlags.EditableInEditor;

            //  If you need larged voxel maps, enlarge this constant.
            MyCommonDebugUtils.AssertRelease(Size.X <= MyVoxelConstants.MAX_VOXEL_MAP_SIZE_IN_VOXELS);
            MyCommonDebugUtils.AssertRelease(Size.Y <= MyVoxelConstants.MAX_VOXEL_MAP_SIZE_IN_VOXELS);
            MyCommonDebugUtils.AssertRelease(Size.Z <= MyVoxelConstants.MAX_VOXEL_MAP_SIZE_IN_VOXELS);

            //  Voxel map size must be multiple of a voxel data cell size.
            MyCommonDebugUtils.AssertRelease((Size.X & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK) == 0);
            MyCommonDebugUtils.AssertRelease((Size.Y & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK) == 0);
            MyCommonDebugUtils.AssertRelease((Size.Z & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK) == 0);
            DataCellsCount.X = Size.X >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;
            DataCellsCount.Y = Size.Y >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;
            DataCellsCount.Z = Size.Z >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;
            DataCellsCountMinusOne = new MyMwcVector3Int(DataCellsCount.X - 1, DataCellsCount.Y - 1, DataCellsCount.Z - 1);

            //  Voxel map size must be multiple of a voxel data cell size.
            MyCommonDebugUtils.AssertRelease((Size.X % MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS) == 0);
            MyCommonDebugUtils.AssertRelease((Size.Y % MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS) == 0);
            MyCommonDebugUtils.AssertRelease((Size.Z % MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS) == 0);
            RenderCellsCount.X = Size.X / MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS;
            RenderCellsCount.Y = Size.Y / MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS;
            RenderCellsCount.Z = Size.Z / MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS;
            RenderCellsCountMinusOne = new MyMwcVector3Int(RenderCellsCount.X - 1, RenderCellsCount.Y - 1, RenderCellsCount.Z - 1);

            m_fakeVoxelMaterial.DrawTechnique = MyMeshDrawTechnique.VOXEL_MAP;
            //  Array of voxel cells in this voxel map
            m_voxelContentCells = new MyVoxelContentCell[DataCellsCount.X][][];
            for (int x = 0; x < m_voxelContentCells.Length; x++)
            {
                m_voxelContentCells[x] = new MyVoxelContentCell[DataCellsCount.Y][];
                for (int y = 0; y < m_voxelContentCells[x].Length; y++)
                {
                    m_voxelContentCells[x][y] = new MyVoxelContentCell[DataCellsCount.Z];
                }
            }

            CastShadows = true;

            InitRenderObjects();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Materials
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            byte defaultIndestructibleContents = GetIndestructibleContentByMaterial(material);

            m_voxelMaterialCells = new MyVoxelMaterialCell[DataCellsCount.X][][];
            for (int x = 0; x < DataCellsCount.X; x++)
            {
                m_voxelMaterialCells[x] = new MyVoxelMaterialCell[DataCellsCount.Y][];
                for (int y = 0; y < DataCellsCount.Y; y++)
                {
                    m_voxelMaterialCells[x][y] = new MyVoxelMaterialCell[DataCellsCount.Z];
                    for (int z = 0; z < DataCellsCount.Z; z++)
                    {
                        m_voxelMaterialCells[x][y][z] = new MyVoxelMaterialCell(material, defaultIndestructibleContents);
                    }
                }
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Voxel map as phys object
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////            

            //initialize list of voxelHandShapes used for editor purpose
            m_voxelHandShapes = new List<MyVoxelHandShape>(100);

            //initialize list of explosions, to save cutted spheres fast, before we convert them to voxel hands
            m_explosions = new List<BoundingSphere>(1000);

            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRBVoxelElementDesc voxelDesc = physobj.GetRBVoxelElementDesc();
            MyMaterialType materialType = MyMaterialType.ROCK;

            voxelDesc.SetToDefault();
            voxelDesc.m_Size = SizeInMetres;
            voxelDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(materialType).PhysicsMaterial;

            MyRBVoxelElement voxEl = (MyRBVoxelElement)physobj.CreateRBElement(voxelDesc);

            this.Physics = new MyPhysicsBody(this, 100000, RigidBodyFlag.RBF_RBO_STATIC) { MaterialType = materialType };
            this.Physics.Enabled = true;
            this.Physics.AddElement(voxEl, true);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelMap.InitVoxelMap() - End");
        }

        // Not used
        //public void Reinitialize()
        //{
        //    if (m_isClosed)
        //    {
        //        Init(this.Name, this.PositionLeftBottomCorner, (MyMwcObjectBuilder_VoxelMap)this.GetObjectBuilderInternal());
        //        m_isClosed = false;
        //    }
        //}

        //  Merges a specified voxel map (from a file) into our actual voxel map at a specified position. 
        //  This merging is slower than loading voxel map through constructor (because we are setting voxels through SetVoxelContent) - so use it only 
        //  for merging-in small areas.
        //  Parameter 'voxelPosition' - where will be placed new merged voxel map withing actual voxel map. It's in voxel coords.
        //  Voxel map we are trying to merge into existing voxel map can be bigger or outside of area of existing voxel map. This method will just ignore those parts.
        //  Coordinate of 'voxelPosition' DOESN'T NEED to be aligned to data cell (multiplies of 8).
        public void MergeVoxelContents(MyMwcVoxelFilesEnum voxelFile, MyMwcVector3Short voxelPosition, MyMwcVoxelMapMergeTypeEnum mergeType)
        {
            MyCompressionFileLoad decompressFile = new MyCompressionFileLoad(MyVoxelFiles.Get(voxelFile).GetVoxFilePath());

            //  Version of a VOX file
            int fileVersion = decompressFile.GetInt32();

            //  Not supported VOX file version
            MyCommonDebugUtils.AssertRelease(fileVersion == MyVoxelConstants.VOXEL_FILE_ACTUAL_VERSION);

            //  Size of this voxel map (in voxels)
            int sizeX = decompressFile.GetInt32();
            int sizeY = decompressFile.GetInt32();
            int sizeZ = decompressFile.GetInt32();

            //  Size of data cell in voxels, doesn't have to be same as current size specified by our constants.
            int cellSizeX = decompressFile.GetInt32();
            int cellSizeY = decompressFile.GetInt32();
            int cellSizeZ = decompressFile.GetInt32();

            int cellsCountX = sizeX / cellSizeX;
            int cellsCountY = sizeY / cellSizeY;
            int cellsCountZ = sizeZ / cellSizeZ;

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < cellsCountX; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < cellsCountY; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < cellsCountZ; cellCoord.Z++)
                    {
                        MyVoxelCellType cellType = (MyVoxelCellType)decompressFile.GetByte();

                        MyMwcVector3Int cellCoordInVoxels = GetVoxelCoordinatesOfDataCell(ref cellCoord);

                        //  Go through every voxel in a cell and change it's value. If cell is empty, set all voxels to empty. Otherwise set by value from file.
                        MyMwcVector3Int voxelCoordInCell;
                        for (voxelCoordInCell.X = 0; voxelCoordInCell.X < cellSizeX; voxelCoordInCell.X++)
                        {
                            for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < cellSizeY; voxelCoordInCell.Y++)
                            {
                                for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < cellSizeZ; voxelCoordInCell.Z++)
                                {
                                    byte newContent;
                                    if (cellType == MyVoxelCellType.EMPTY)
                                    {
                                        newContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                    }
                                    else if (cellType == MyVoxelCellType.FULL)
                                    {
                                        newContent = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                    }
                                    else
                                    {
                                        newContent = decompressFile.GetByte();
                                    }

                                    MyMwcVector3Int voxelCoord;
                                    voxelCoord.X = voxelPosition.X + cellCoordInVoxels.X + voxelCoordInCell.X;
                                    voxelCoord.Y = voxelPosition.Y + cellCoordInVoxels.Y + voxelCoordInCell.Y;
                                    voxelCoord.Z = voxelPosition.Z + cellCoordInVoxels.Z + voxelCoordInCell.Z;

                                    if (IsVoxelInVoxelMap(ref voxelCoord) == true)
                                    {
                                        byte originalContent = GetVoxelContent(ref voxelCoord);

                                        if (mergeType == MyMwcVoxelMapMergeTypeEnum.ADD)
                                        {
                                            //  Set new content only if its value is higher than actual - so we only can add matter
                                            if (newContent > originalContent)
                                            {
                                                SetVoxelContent(newContent, ref voxelCoord);
                                            }
                                        }
                                        else if (mergeType == MyMwcVoxelMapMergeTypeEnum.INVERSE_AND_SUBTRACT)
                                        {
                                            //  Subtract new content from original, so if original voxel is full and new is full too, result will be empty voxel
                                            //  If new is empty, nothing will happen. If new is full but original is empty, nothing will happen either.
                                            SetVoxelContent((byte)MyMwcUtils.GetClampInt((int)originalContent - (int)newContent, MyVoxelConstants.VOXEL_CONTENT_EMPTY, MyVoxelConstants.VOXEL_CONTENT_FULL), ref voxelCoord);
                                        }
                                        else
                                        {
                                            throw new MyMwcExceptionApplicationShouldNotGetHere();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //  Sets new voxel material and invalidates cache
        public void SetVoxelMaterialInvalidateCache(MyMwcVoxelMaterialsEnum materialToSet, BoundingSphere sphere, ref bool changed)
        {
            //  Get min corner of the sphere
            MyMwcVector3Int minCorner = GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - 2 * MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - 2 * MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - 2 * MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the sphere
            MyMwcVector3Int maxCorner = GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + 2 * MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + 2 * MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + 2 * MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            FixVoxelCoord(ref minCorner);
            FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            bool isMaterialToSetIndestructible = MyVoxelMaterials.IsIndestructible(materialToSet);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        if (IsVoxelInVoxelMap(ref tempVoxelCoord))
                        {
                            Vector3 voxelPosition = GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                            float dist = (voxelPosition - sphere.Center).Length();
                            float diff = dist - sphere.Radius;

                            if (diff <= MyVoxelConstants.VOXEL_SIZE_IN_METRES * 1.5f)  // could be VOXEL_SIZE_IN_METRES_HALF, but we want to set material in empty cells correctly
                            {
                                byte indestructibleContentToSet = MyVoxelConstants.VOXEL_CONTENT_FULL;
                                if (diff >= MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)  // outside
                                {
                                    indestructibleContentToSet = MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                                }
                                else if (diff >= -MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF)  // boundary
                                {
                                    indestructibleContentToSet = (byte)(MyVoxelConstants.VOXEL_ISO_LEVEL - diff / MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF * MyVoxelConstants.VOXEL_ISO_LEVEL);
                                }

                                MyMwcVoxelMaterialsEnum originalMaterial;
                                byte originalIndestructibleContent;

                                GetMaterialAndIndestructibleContent(ref tempVoxelCoord, out originalMaterial, out originalIndestructibleContent);

                                byte newIndestructibleContent;
                                if (isMaterialToSetIndestructible)
                                    newIndestructibleContent = Math.Max(indestructibleContentToSet, originalIndestructibleContent);  // add
                                else
                                    newIndestructibleContent = Math.Min((byte)(MyVoxelConstants.VOXEL_CONTENT_FULL - indestructibleContentToSet), originalIndestructibleContent);  // subtract

                                // Change the material: 
                                // - always on boundaries between material and nothing
                                // - smoothly on inner boundaries
                                MyMwcVoxelMaterialsEnum newMaterial = materialToSet;
                                if (diff > 0)
                                {
                                    byte content = GetVoxelContent(ref tempVoxelCoord);
                                    if (content == MyVoxelConstants.VOXEL_CONTENT_FULL)
                                        newMaterial = originalMaterial;
                                    if (diff >= MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF && content != MyVoxelConstants.VOXEL_CONTENT_EMPTY)  // set material behind boundary only for empty voxels
                                        newMaterial = originalMaterial;
                                }

                                if (originalMaterial == newMaterial && originalIndestructibleContent == newIndestructibleContent)
                                {
                                    continue;
                                }

                                SetVoxelMaterialAndIndestructibleContent(newMaterial, newIndestructibleContent, ref tempVoxelCoord);
                                changed = true;

                                if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                                if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                                if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                                if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                                if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                                if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                            }
                        }
                    }
                }
            }

            if (changed == true)
            {
                //  Extend borders for cleaning by one voxel
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                FixVoxelCoord(ref minChanged);
                FixVoxelCoord(ref maxChanged);

                // Optimize
                MyMwcVector3Int minCellCoord = GetDataCellCoordinate(ref minChanged);
                MyMwcVector3Int maxCellCoord = GetDataCellCoordinate(ref maxChanged);
                MyMwcVector3Int cellCoord;
                for (cellCoord.X = minCellCoord.X; cellCoord.X <= maxCellCoord.X; cellCoord.X++)
                    for (cellCoord.Y = minCellCoord.Y; cellCoord.Y <= maxCellCoord.Y; cellCoord.Y++)
                        for (cellCoord.Z = minCellCoord.Z; cellCoord.Z <= maxCellCoord.Z; cellCoord.Z++)
                            GetVoxelMaterialCells()[cellCoord.X][cellCoord.Y][cellCoord.Z].OptimizeSize();

                InvalidateCache(minChanged, maxChanged);
            }
        }

        //  Sets new voxel material and invalidates cache
        public void SetVoxelMaterialInvalidateCache(MyMwcVoxelMaterialsEnum materialToSet, BoundingBox box, ref bool changed)
        {
            //  Get min corner of the box
            MyMwcVector3Int minCorner = GetVoxelCoordinateFromMeters(new Vector3(
                box.Min.X - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                box.Min.Y - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                box.Min.Z - MyVoxelConstants.VOXEL_SIZE_IN_METRES));


            //  Get max corner of the box
            MyMwcVector3Int maxCorner = GetVoxelCoordinateFromMeters(new Vector3(
                box.Max.X + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                box.Max.Y + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                box.Max.Z + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            FixVoxelCoord(ref minCorner);
            FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            bool materialChanged = false;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        if (IsVoxelInVoxelMap(ref tempVoxelCoord))
                        {
                            //  Actual voxel content
                            byte voxelContent = GetVoxelContent(ref tempVoxelCoord);
                            byte indestructibleContent = MyVoxelConstants.VOXEL_CONTENT_EMPTY;

                            SetVoxelMaterialAndIndestructibleContent(materialToSet, indestructibleContent, ref tempVoxelCoord);
                            materialChanged = true;

                            if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                            if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                            if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                            if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                        }
                    }
                }
            }

            if (materialChanged == true)
            {
                //  Extend borders for cleaning, so it's one pixel on both sides
                minChanged.X -= 1;
                minChanged.Y -= 1;
                minChanged.Z -= 1;
                maxChanged.X += 1;
                maxChanged.Y += 1;
                maxChanged.Z += 1;
                FixVoxelCoord(ref minChanged);
                FixVoxelCoord(ref maxChanged);

                // Optimize
                MyMwcVector3Int minCellCoord = GetDataCellCoordinate(ref minChanged);
                MyMwcVector3Int maxCellCoord = GetDataCellCoordinate(ref maxChanged);
                MyMwcVector3Int cellCoord;
                for (cellCoord.X = minCellCoord.X; cellCoord.X <= maxCellCoord.X; cellCoord.X++)
                    for (cellCoord.Y = minCellCoord.Y; cellCoord.Y <= maxCellCoord.Y; cellCoord.Y++)
                        for (cellCoord.Z = minCellCoord.Z; cellCoord.Z <= maxCellCoord.Z; cellCoord.Z++)
                            GetVoxelMaterialCells()[cellCoord.X][cellCoord.Y][cellCoord.Z].OptimizeSize();

                InvalidateCache(minChanged, maxChanged);
            }
        }

        //  Merges specified materials (from file) into our actual voxel map - overwriting materials only.
        //  We are using a regular voxel map to define areas where we want to set a specified material. Empty voxels are ignored and 
        //  only mixed/full voxels are used to tell us that that voxel will contain new material - 'materialToSet'.
        //  If we are seting indestructible material, voxel content values from merged voxel map will be used to define indestructible content.
        //  Parameter 'voxelPosition' - place where we will place merged voxel map withing actual voxel map. It's in voxel coords.
        //  IMPORTANT: THIS METHOD WILL WORK ONLY IF WE PLACE THE MAP THAT WE TRY TO MERGE FROM IN VOXEL COORDINATES THAT ARE MULTIPLY OF DATA CELL SIZE
        //  This method is used to load small material areas, overwriting actual material only if value from file is 1. Zeros are ignored (it's empty space).
        //  This method is quite fast, even on large maps - 512x512x512, so we can do more overwrites.
        //  Parameter 'materialToSet' tells us what material to set at places which are full in file. Empty are ignored - so stay as they were before this method was called.
        //  IMPORTANT: THIS MERGE MATERIAL CAN BE CALLED ONLY AFTER ALL VOXEL CONTENTS ARE LOADED. THAT'S BECAUSE WE NEED TO KNOW THEM FOR MIN CONTENT / INDESTRUCTIBLE CONTENT.
        //  Voxel map we are trying to merge into existing voxel map can be bigger or outside of area of existing voxel map. This method will just ignore those parts.
        public void MergeVoxelMaterials(MyMwcVoxelFilesEnum voxelFile, MyMwcVector3Short voxelPosition, MyMwcVoxelMaterialsEnum materialToSet)
        {
            MyMwcLog.WriteLine("MyVoxelMap.MergeVoxelMaterials() - Start");
            MyMwcLog.IncreaseIndent();

            MyCompressionFileLoad decompressFile = new MyCompressionFileLoad(MyVoxelFiles.Get(voxelFile).GetVoxFilePath());

            //  Version of a VOX file
            int fileVersion = decompressFile.GetInt32();

            //  Not supported VOX file version
            MyCommonDebugUtils.AssertRelease(fileVersion == MyVoxelConstants.VOXEL_FILE_ACTUAL_VERSION);

            //  Size of this voxel map (in voxels)
            int sizeX = decompressFile.GetInt32();
            int sizeY = decompressFile.GetInt32();
            int sizeZ = decompressFile.GetInt32();

            //  Size of data cell in voxels, doesn't have to be same as current size specified by our constants.
            int cellSizeX = decompressFile.GetInt32();
            int cellSizeY = decompressFile.GetInt32();
            int cellSizeZ = decompressFile.GetInt32();

            int cellsCountX = sizeX / cellSizeX;
            int cellsCountY = sizeY / cellSizeY;
            int cellsCountZ = sizeZ / cellSizeZ;

            //  This method will work only if we place the map that we try to merge from in voxel coordinates that are multiply of data cell size
            MyCommonDebugUtils.AssertRelease((voxelPosition.X & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK) == 0);
            MyCommonDebugUtils.AssertRelease((voxelPosition.Y & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK) == 0);
            MyCommonDebugUtils.AssertRelease((voxelPosition.Z & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK) == 0);
            MyMwcVector3Int cellFullForVoxelPosition = GetDataCellCoordinate(ref voxelPosition);

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < cellsCountX; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < cellsCountY; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < cellsCountZ; cellCoord.Z++)
                    {
                        MyVoxelCellType cellType = (MyVoxelCellType)decompressFile.GetByte();

                        //  We can do "continue" here, becase we need to read this file properly, even if we will ignore that data
                        bool isDataCellInVoxelMap = IsDataCellInVoxelMap(
                            new MyMwcVector3Int(
                                cellFullForVoxelPosition.X + cellCoord.X,
                                cellFullForVoxelPosition.Y + cellCoord.Y,
                                cellFullForVoxelPosition.Z + cellCoord.Z));

                        if (cellType == MyVoxelCellType.EMPTY)
                        {
                            //  If merged cell is empty, there is nothing to overwrite, so we can skip this cell
                            continue;
                        }
                        else if (cellType == MyVoxelCellType.FULL)
                        {
                            //  If merged cell is full, than we reset whole material cell to 'materialToSet'
                            if (isDataCellInVoxelMap)
                            {
                                m_voxelMaterialCells[cellFullForVoxelPosition.X + cellCoord.X][cellFullForVoxelPosition.Y +
                                    cellCoord.Y][cellFullForVoxelPosition.Z + cellCoord.Z].Reset(
                                    materialToSet, GetIndestructibleContentByMaterial(materialToSet));
                            }
                        }
                        else
                        {
                            MyMwcVector3Int cellCoordInVoxels = GetVoxelCoordinatesOfDataCell(ref cellCoord);

                            MyMwcVector3Int voxelCoordRelative;
                            voxelCoordRelative.X = voxelPosition.X + cellCoordInVoxels.X;
                            voxelCoordRelative.Y = voxelPosition.Y + cellCoordInVoxels.Y;
                            voxelCoordRelative.Z = voxelPosition.Z + cellCoordInVoxels.Z;

                            MyMwcVector3Int voxelCoordInCell;
                            for (voxelCoordInCell.X = 0; voxelCoordInCell.X < cellSizeX; voxelCoordInCell.X++)
                            {
                                for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < cellSizeY; voxelCoordInCell.Y++)
                                {
                                    for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < cellSizeZ; voxelCoordInCell.Z++)
                                    {
                                        byte voxelFromFile = decompressFile.GetByte();

                                        if (isDataCellInVoxelMap)
                                        {
                                            //  Ignore empty voxels, but use mixed/full for seting the material
                                            if (voxelFromFile > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                                            {
                                                MyMwcVector3Int voxelCoord = new MyMwcVector3Int(
                                                    voxelCoordRelative.X + voxelCoordInCell.X,
                                                    voxelCoordRelative.Y + voxelCoordInCell.Y,
                                                    voxelCoordRelative.Z + voxelCoordInCell.Z);

                                                //  Actual voxel content
                                                byte voxelContent = GetVoxelContent(ref voxelCoord);

                                                //  If this is indestructible material, here we will get 'min content' for this voxel
                                                byte indestructibleContent = GetIndestructibleContentsByMaterialAndContent(materialToSet, voxelFromFile);

                                                //  Indestructible content can be less than real voxel at this place. First I made this mistake.
                                                //  If forgoten, then during explosions we will in fact create matter from 'indestructible content' array.
                                                if (indestructibleContent > voxelContent)
                                                {
                                                    indestructibleContent = voxelContent;
                                                }

                                                SetVoxelMaterialAndIndestructibleContent(materialToSet, indestructibleContent, ref voxelCoord);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelMap.MergeVoxelMaterials() - End");
        }

        public void SetVoxelMaterialAndIndestructibleContentForWholeCell(MyMwcVoxelMaterialsEnum material, byte indestructibleContent, ref MyMwcVector3Int cellCoord)
        {
            m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].Reset(material, indestructibleContent);
        }

        public void SetVoxelMaterialAndIndestructibleContent(MyMwcVoxelMaterialsEnum material, byte indestructibleContent, ref MyMwcVector3Int voxelCoord)
        {
            MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
            MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
            MyMwcVoxelMaterialsEnum oldMaterial = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetMaterial(ref voxelCoordInCell);
            m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].SetMaterialAndIndestructibleContent(material, indestructibleContent, ref voxelCoordInCell);
            if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
            {
                ChangeOreDepositMaterial(oldMaterial, material, ref voxelCoord);
            }
        }

        //  Voxel at specified coordinate 'x, y, z' sets to value 'content'
        //  Coordinates are relative to voxel map
        public void SetVoxelContent(byte content, ref MyMwcVector3Int voxelCoord, bool needLock = true)
        {
            try
            {
                if (needLock)
                    ContentLock.AcquireExclusive();

                //  We don't change voxel if it's a border voxel and it would be an empty voxel (not full). Because that would make voxel map with wrong/missing edges.
                if ((content > 0) && (IsVoxelAtBorder(ref voxelCoord))) return;

                MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
                MyVoxelContentCell voxelCell = GetCell(ref cellCoord);

                if (voxelCell == null)
                {
                    //  Voxel wasn't found in cell dictionary, therefore cell must be FULL

                    if (content == MyVoxelConstants.VOXEL_CONTENT_FULL)
                    {
                        //  Cell is full and we are seting voxel to full, so nothing needs to be changed
                        return;
                    }
                    else
                    {
                        //  We are switching cell from type FULL to EMPTY or MIXED, therefore we need to allocate new cell
                        MyVoxelContentCell newCell = AddCell(ref cellCoord);
                        MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
                        newCell.SetVoxelContent(content, ref voxelCoordInCell);

                        //  We change ore deposit content from full to new content
                        if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                        {
                            ChangeOreDepositCellContent(MyVoxelConstants.VOXEL_CONTENT_FULL, MyVoxelContentCellContent.QuantizedValue(content), ref voxelCoord);
                        }
                    }
                }
                else if (voxelCell.CellType == MyVoxelCellType.FULL)
                {
                    if (content == MyVoxelConstants.VOXEL_CONTENT_FULL)
                    {
                        //  Cell is full and we are seting voxel to full, so nothing needs to be changed
                        return;
                    }
                    else
                    {
                        MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
                        voxelCell.SetVoxelContent(content, ref voxelCoordInCell);
                        CheckIfCellChangedToFull(voxelCell, ref cellCoord);

                        if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                        {
                            //  We change ore deposit content from full to new content
                            ChangeOreDepositCellContent(MyVoxelConstants.VOXEL_CONTENT_FULL, MyVoxelContentCellContent.QuantizedValue(content), ref voxelCoord);
                        }
                    }
                }
                else if (voxelCell.CellType == MyVoxelCellType.EMPTY)
                {
                    if (content == MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                    {
                        //  Cell is empty and we are seting voxel to empty, so nothing needs to be changed
                        return;
                    }
                    else
                    {
                        MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
                        voxelCell.SetVoxelContent(content, ref voxelCoordInCell);
                        CheckIfCellChangedToFull(voxelCell, ref cellCoord);

                        //  We change ore deposit content from empty to new content
                        if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                        {
                            ChangeOreDepositCellContent(MyVoxelConstants.VOXEL_CONTENT_EMPTY, MyVoxelContentCellContent.QuantizedValue(content), ref voxelCoord);
                        }
                    }
                }
                else if (voxelCell.CellType == MyVoxelCellType.MIXED)
                {
                    MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
                    byte oldContent = voxelCell.GetVoxelContent(ref voxelCoordInCell);
                    voxelCell.SetVoxelContent(content, ref voxelCoordInCell);
                    CheckIfCellChangedToFull(voxelCell, ref cellCoord);

                    byte newContent = voxelCell.GetVoxelContent(ref voxelCoordInCell);
                    //  We change ore deposit content from old to new
                    if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                    {
                        ChangeOreDepositCellContent(oldContent, newContent, ref voxelCoord);
                    }
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }
            }
            finally
            {
                if (needLock)
                    ContentLock.ReleaseExclusive();
            }
        }

        //	Softenuje skalarnu hodnotu voxelu. Spriemeruje skalarnu hodnotu voxelu podla zadanej vahy a skalarnych hodnot susednych voxelov.
        public bool SoftenVoxelContent(
            MyMwcVector3Int voxelCoord,		//	Voxel ktory chceme softenovat
            float softenWeight		        //	Vaha na morfovanie medzi povodnou skalarnou hodnotou a novou spriemerovanou (0.0 = dominantna je stara hodnota, 0.5 = stred, 1.0 = dominantna je nova/priemerna hodnota)
            )
        {
            int sumScalar = 0;		//	Suma skalarov
            int count = 0;			//	Pocet spocitanych voxelov

            //	Spocitame skalary vsetkych susednych voxelov nasho voxelu (aj diagonalne)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        MyMwcVector3Int tempVoxelCoord = new MyMwcVector3Int(voxelCoord.X + x, voxelCoord.Y + y, voxelCoord.Z + z);
                        sumScalar += GetVoxelContent(ref tempVoxelCoord);
                        count++;
                    }
                }
            }

            //	Povodna skalarna hodnota
            int oldScalar = GetVoxelContent(ref voxelCoord);

            //	Nova priemerna hodnota je suma skalarov deleno pocet susednych voxelov
            int avgScalar = sumScalar / count;

            //	Nova hodnota je vypocitana na zaklade starej a novej priemernej hodnoty a vahy
            int newVal = (int)MathHelper.Lerp((float)oldScalar, (float)avgScalar, softenWeight);

            bool softened = false;
            //	Zapiseme priemernu skalarnu hodnotu voxelu, len ak je mensia nez ta ktora tam bola doteraz
            if (newVal < oldScalar)
            {
                //  Content must be in interval <0..255>
                MyCommonDebugUtils.AssertDebug((newVal >= MyVoxelConstants.VOXEL_CONTENT_EMPTY) && (newVal <= MyVoxelConstants.VOXEL_CONTENT_FULL));
                SetVoxelContent((byte)newVal, ref voxelCoord);
                softened = true;
            }

            return softened;
        }

        //	Softenuje skalarne hodnoty voxelov v kvadry ktoreho zaciatok je vo voxely "kro_From" a koniec vo voxely "kro_To".
        public void SoftenVoxelContentInBox(
            MyMwcVector3Int from,			//	Voxel v ktorom zacne soften
            MyMwcVector3Int to,			    //	Voxel kvadra v ktorom konci soften
            float softenWeight	//	Vaha na morfovanie medzi povodnou skalarnou hodnotou a novou spriemerovanou.
            )
        {
            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = from.X; tempVoxelCoord.X <= to.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = from.Y; tempVoxelCoord.Y <= to.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = from.Z; tempVoxelCoord.Z <= to.Z; tempVoxelCoord.Z++)
                    {
                        SoftenVoxelContent(tempVoxelCoord, softenWeight);
                    }
                }
            }
        }

        /// <summary>
        /// Softens voxel material and invalidates cache
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="softenWeight"></param>
        /// <param name="originalContents"></param>
        public void SoftenVoxelContentInSphereInvalidateCache(BoundingSphere sphere, float softenWeight, ref bool changed)
        {
            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            FixVoxelCoord(ref minCorner);
            FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        if (IsVoxelInVoxelMap(ref tempVoxelCoord))
                        {
                            Vector3 voxelPosition = GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                            float dist = (voxelPosition - sphere.Center).Length();
                            float diff = dist - sphere.Radius;

                            if (diff <= 0f)
                            {
                                byte originalContent = GetVoxelContent(ref tempVoxelCoord);
                                if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                                {
                                    bool result = SoftenVoxelContent(tempVoxelCoord, softenWeight);
                                    if (changed == false) changed = result;
                                    if (result == true)
                                    {
                                        if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                                        if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                                        if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                                        if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                                        if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                                        if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (changed == true)
            {
                //  Extend borders for cleaning, so it's two voxels on both sides (two voxels because softening takes more space than pure set-voxel-content)
                minChanged.X -= 2;
                minChanged.Y -= 2;
                minChanged.Z -= 2;
                maxChanged.X += 2;
                maxChanged.Y += 2;
                maxChanged.Z += 2;
                FixVoxelCoord(ref minChanged);
                FixVoxelCoord(ref maxChanged);

                InvalidateCache(minChanged, maxChanged);
            }
        }

        public void SoftenVoxelContentInBoxInvalidateCache(BoundingBox box, float softenWeight, ref bool changed)
        {
            //  Get min corner of the box
            MyMwcVector3Int minCorner = GetVoxelCoordinateFromMeters(box.Min);

            //  Get max corner of the box
            MyMwcVector3Int maxCorner = GetVoxelCoordinateFromMeters(box.Max);

            FixVoxelCoord(ref minCorner);
            FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            MyMwcVector3Int tempVoxelCoord;
            bool softened = false;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        if (IsVoxelInVoxelMap(ref tempVoxelCoord))
                        {
                            bool result = SoftenVoxelContent(tempVoxelCoord, softenWeight);
                            if (softened == false) softened = result;
                            if (result == true)
                            {
                                if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                                if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                                if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                                if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                                if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                                if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                            }
                        }
                    }
                }
            }

            if (softened == true)
            {
                //  Extend borders for cleaning, so it's two voxels on both sides (two voxels because softening takes more space than pure set-voxel-content)
                minChanged.X -= 2;
                minChanged.Y -= 2;
                minChanged.Z -= 2;
                maxChanged.X += 2;
                maxChanged.Y += 2;
                maxChanged.Z += 2;
                FixVoxelCoord(ref minChanged);
                FixVoxelCoord(ref maxChanged);

                InvalidateCache(minChanged, maxChanged);
            }
        }


        public bool WrinkleVoxelContent(
            MyMwcVector3Int voxelCoord,
            float wrinkleWeightAdd,
            float wrinkleWeightRemove
        )
        {
            int max = Int32.MinValue, min = Int32.MaxValue;
            int randomizationAdd = (int)(wrinkleWeightAdd * 255);
            int randomizationRemove = (int)(wrinkleWeightRemove * 255);

            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    for (int z = -1; z <= 1; z++)
                    {
                        MyMwcVector3Int tempVoxelCoord = new MyMwcVector3Int(voxelCoord.X + x, voxelCoord.Y + y, voxelCoord.Z + z);
                        var content = GetVoxelContent(ref tempVoxelCoord);
                        max = Math.Max(max, content);
                        min = Math.Min(min, content);
                    }

            if (min == max) return false;

            int old = GetVoxelContent(ref voxelCoord);

            byte newVal = (byte)MyMwcUtils.GetClampInt(old + MyMwcUtils.GetRandomInt(randomizationAdd + randomizationRemove) - randomizationRemove, min, max);
            newVal = MyVoxelContentCellContent.QuantizedValue(newVal);

            if (newVal != old)
            {
                SetVoxelContent((byte)newVal, ref voxelCoord);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Wrinkles voxel material and invalidates cache
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="wrinkleWeight"></param>
        /// <param name="originalContents"></param>
        public void WrinkleVoxelContentInSphereInvalidateCache(BoundingSphere sphere, float wrinkleWeightAdd, float wrinkleWeightRemove, ref bool changed)
        {
            //  Get min corner of the explosion
            MyMwcVector3Int minCorner = GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z - sphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            //  Get max corner of the explosion
            MyMwcVector3Int maxCorner = GetVoxelCoordinateFromMeters(new Vector3(
                sphere.Center.X + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Y + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                sphere.Center.Z + sphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));

            FixVoxelCoord(ref minCorner);
            FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int minChanged = maxCorner;
            MyMwcVector3Int maxChanged = minCorner;

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        if (IsVoxelInVoxelMap(ref tempVoxelCoord))
                        {
                            Vector3 voxelPosition = GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);
                            float dist = (voxelPosition - sphere.Center).Length();
                            float diff = dist - sphere.Radius;

                            if (diff <= 0f)
                            {
                                byte originalContent = GetVoxelContent(ref tempVoxelCoord);
                                if (originalContent > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                                {
                                    bool result = WrinkleVoxelContent(tempVoxelCoord, wrinkleWeightAdd, wrinkleWeightRemove);
                                    if (changed == false) changed = result;
                                    if (result == true)
                                    {
                                        if (tempVoxelCoord.X < minChanged.X) minChanged.X = tempVoxelCoord.X;
                                        if (tempVoxelCoord.Y < minChanged.Y) minChanged.Y = tempVoxelCoord.Y;
                                        if (tempVoxelCoord.Z < minChanged.Z) minChanged.Z = tempVoxelCoord.Z;
                                        if (tempVoxelCoord.X > maxChanged.X) maxChanged.X = tempVoxelCoord.X;
                                        if (tempVoxelCoord.Y > maxChanged.Y) maxChanged.Y = tempVoxelCoord.Y;
                                        if (tempVoxelCoord.Z > maxChanged.Z) maxChanged.Z = tempVoxelCoord.Z;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (changed == true)
            {
                //  Extend borders for cleaning, so it's two voxels on both sides (two voxels because wrinkling takes more space than pure set-voxel-content)
                minChanged.X -= 2;
                minChanged.Y -= 2;
                minChanged.Z -= 2;
                maxChanged.X += 2;
                maxChanged.Y += 2;
                maxChanged.Z += 2;
                FixVoxelCoord(ref minChanged);
                FixVoxelCoord(ref maxChanged);

                InvalidateCache(minChanged, maxChanged);
            }
        }

        //  Coordinates are relative to voxel map. Returned value is just byte converted to float on interval <0..1>
        public float GetVoxelContentAsFloat(ref MyMwcVector3Int voxelCoord)
        {
            return (float)GetVoxelContent(ref voxelCoord) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT;
        }

        //  Coordinates are relative to voxel map
        public byte GetVoxelContent(ref MyMwcVector3Int voxelCoord)
        {
            using (ContentLock.AcquireSharedUsing())
            {
                MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
                MyVoxelContentCell voxelCell = GetCell(ref cellCoord);

                if (voxelCell == null)
                {
                    //  Voxel wasn't found in cell dictionary, therefore cell must be full
                    return MyVoxelConstants.VOXEL_CONTENT_FULL;
                }
                else
                {
                    MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
                    byte ret = voxelCell.GetVoxelContent(ref voxelCoordInCell);
                    return ret;
                }
            }
        }

        //  Return average content for whole data cell. This is used only for LOD (level of detail) - or lower/higher level.
        //  Coordinates are 'data cell coordinates'
        public byte GetDataCellAverageContent(ref MyMwcVector3Int cellCoord)
        {
            MyVoxelContentCell cell = GetCell(ref cellCoord);

            if (cell == null)
            {
                //  Cell wasn't found in cell dictionary, therefore cell must be full
                return MyVoxelConstants.VOXEL_CONTENT_FULL;
            }
            else
            {
                if (cell.CellType == MyVoxelCellType.EMPTY)
                {
                    return MyVoxelConstants.VOXEL_CONTENT_EMPTY;
                }
                else
                {
                    return cell.GetAverageContent();
                }
            }
        }

        ////  Return shadow value of whole data cell.
        ////  Coordinates are 'data cell coordinates'
        //public byte GetDataCellShadow(ref MyMwcVector3Int dataCellCoord)
        //{
        //    return m_dataCellShadows[dataCellCoord.X][dataCellCoord.Y][dataCellCoord.Z];
        //}

        ////  Return shadow value of whole data cell.
        ////  Coordinates are 'data cell coordinates'
        //public byte GetVoxelShadow(ref MyMwcVector3Int voxelCoord)
        //{
        //    return m_voxelShadows[voxelCoord.X / MyShadowConstants.SHADOW_ENTITY_SIZE_IN_VOXELS][voxelCoord.Y / MyShadowConstants.SHADOW_ENTITY_SIZE_IN_VOXELS][voxelCoord.Z / MyShadowConstants.SHADOW_ENTITY_SIZE_IN_VOXELS];
        //}

        //  Return average material for whole data cell. This is used only for LOD (level of detail) - or lower/higher level.
        //  Coordinates are 'data cell coordinates'
        public MyMwcVoxelMaterialsEnum GetDataCellAverageMaterial(ref MyMwcVector3Int cellCoord)
        {
            return m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetAverageCellMaterial();
        }

        //  Return voxel's coordinates relative to cell (in voxel space)
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public MyMwcVector3Int GetVoxelCoordinatesInDataCell(ref MyMwcVector3Int voxelCoord)
        {
            return new MyMwcVector3Int(voxelCoord.X & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK, voxelCoord.Y & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK, voxelCoord.Z & MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_MASK);
        }

        //  Return cell coordinates, but defined in voxels.
        //  You enter 'precalculatedCellCoord' in cell coords and get voxel coordinate of corner of a cell.
        public MyMwcVector3Int GetVoxelCoordinatesOfDataCell(ref MyMwcVector3Int cellCoord)
        {
            return new MyMwcVector3Int(cellCoord.X << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Y << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Z << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
        }

        //  Return coordinates of data cell center, but defined in voxels.
        //  You enter 'precalculatedCellCoord' in cell coords and get voxel coordinate of corner of a cell.
        public MyMwcVector3Int GetVoxelCoordinatesOfCenterOfDataCell(ref MyMwcVector3Int cellCoord)
        {
            return new MyMwcVector3Int(
                (cellCoord.X << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS) + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS / 2,
                (cellCoord.Y << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS) + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS / 2,
                (cellCoord.Z << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS) + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS / 2);
        }

        //  Return data cell to which belongs specified voxel (data cell)
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public MyMwcVector3Int GetDataCellCoordinate(ref MyMwcVector3Int voxelCoord)
        {
            return new MyMwcVector3Int(voxelCoord.X >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, voxelCoord.Y >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, voxelCoord.Z >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
        }

        //  Return data cell to which belongs specified voxel (data cell)
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public MyMwcVector3Int GetDataCellCoordinate(ref MyMwcVector3Short voxelCoord)
        {
            return new MyMwcVector3Int(voxelCoord.X >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, voxelCoord.Y >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, voxelCoord.Z >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
        }

        //  Return data cell to which belongs point specified by 'position'. It convertes from metres to cell coordinate.
        //  Data cell begins at voxel center, not its corner. This method takes it into account.
        public MyMwcVector3Int GetDataCellCoordinateFromMeters(ref Vector3 position)
        {
            MyMwcVector3Int voxelCoord = GetVoxelCenterCoordinateFromMeters(ref position);
            return new MyMwcVector3Int(voxelCoord.X >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, voxelCoord.Y >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, voxelCoord.Z >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
        }

        //  Return cell to which belongs specified voxel (render cell)
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public MyMwcVector3Int GetVoxelRenderCellCoordinateFromMeters(ref Vector3 point)
        {
            MyMwcVector3Int voxelCoord = GetVoxelCenterCoordinateFromMeters(ref point);
            return GetVoxelRenderCellCoordinate(ref voxelCoord);
        }

        //  Return cell to which belongs specified voxel (render cell)
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public MyMwcVector3Int GetVoxelRenderCellCoordinate(ref MyMwcVector3Int voxelCoord)
        {
            return new MyMwcVector3Int(voxelCoord.X / MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, voxelCoord.Y / MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, voxelCoord.Z / MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS);
        }

        //  Coordinates are relative to voxel map
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public void GetMaterialAndIndestructibleContent(ref MyMwcVector3Int voxelCoord, out MyMwcVoxelMaterialsEnum voxelMaterial, out byte voxelMaterialIndestructibleContent)
        {
            MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
            MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
            voxelMaterialIndestructibleContent = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetIndestructibleContent(ref voxelCoordInCell);
            voxelMaterial = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetMaterial(ref voxelCoordInCell);
            voxelMaterial = MyVoxelMaterials.GetAllowedVoxelMaterial(voxelMaterial);
        }

        //  Coordinates are relative to voxel map
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public byte GetVoxelMaterialIndestructibleContent(ref MyMwcVector3Int voxelCoord)
        {
            MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
            MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
            return m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetIndestructibleContent(ref voxelCoordInCell);
        }

        //  Coordinates are relative to voxel map
        //  IMPORTANT: Input variable 'tempVoxelCoord' is 'ref' only for optimization. Never change its value in the method!!!
        public MyMwcVoxelMaterialsEnum GetVoxelMaterial(ref MyMwcVector3Int voxelCoord)
        {
            MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
            MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
            MyMwcVoxelMaterialsEnum material = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetMaterial(ref voxelCoordInCell);

            return MyVoxelMaterials.GetAllowedVoxelMaterial(material);
        }

        //  Calculates normal for specified voxel. Used by poligonization grid.
        //  This method has only one drawback: if voxels around voxel for which we want to get normal are empty, resulting normal will be NaN
        //  If this happens, I replace it by vector with right direction. It's a fake. Hope nobody notices.
        public Vector3 GetVoxelNormal(ref MyMwcVector3Int voxelCoord)
        {
            if ((voxelCoord.X == 0) || (voxelCoord.X == (Size.X - 1)) ||
                (voxelCoord.Y == 0) || (voxelCoord.Y == (Size.Y - 1)) ||
                (voxelCoord.Z == 0) || (voxelCoord.Z == (Size.Z - 1)))
            {
                //  If asked for normal vector for voxel that is at the voxel map border, we can't compute it by gradient, so return this hack.
                return Vector3.Up;
            }

            MyMwcVector3Int tempVoxelCoord0 = new MyMwcVector3Int(voxelCoord.X - 1, voxelCoord.Y, voxelCoord.Z);
            MyMwcVector3Int tempVoxelCoord1 = new MyMwcVector3Int(voxelCoord.X + 1, voxelCoord.Y, voxelCoord.Z);
            MyMwcVector3Int tempVoxelCoord2 = new MyMwcVector3Int(voxelCoord.X, voxelCoord.Y - 1, voxelCoord.Z);
            MyMwcVector3Int tempVoxelCoord3 = new MyMwcVector3Int(voxelCoord.X, voxelCoord.Y + 1, voxelCoord.Z);
            MyMwcVector3Int tempVoxelCoord4 = new MyMwcVector3Int(voxelCoord.X, voxelCoord.Y, voxelCoord.Z - 1);
            MyMwcVector3Int tempVoxelCoord5 = new MyMwcVector3Int(voxelCoord.X, voxelCoord.Y, voxelCoord.Z + 1);

            Vector3 normal = new Vector3(
                (GetVoxelContent(ref tempVoxelCoord0) - GetVoxelContent(ref tempVoxelCoord1)) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT,
                (GetVoxelContent(ref tempVoxelCoord2) - GetVoxelContent(ref tempVoxelCoord3)) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT,
                (GetVoxelContent(ref tempVoxelCoord4) - GetVoxelContent(ref tempVoxelCoord5)) / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT);

            if (normal.LengthSquared() <= 0.0f)
            {
                //  If voxels surounding voxel for which we want to get normal vector are of the same value, their subtracting leads to zero vector and that can't be used. So following line is hack.
                return Vector3.Up;
            }
            else
            {
                return MyMwcUtils.Normalize(normal);
            }
        }

        //  Return voxel center coordinate relative to voxel map (in metres)
        //  It's coordinate of center of a voxel (not corner)
        public Vector3 GetVoxelCenterPositionRelative(ref MyMwcVector3Int voxelCoord)
        {
            return new Vector3(
                voxelCoord.X * MyVoxelConstants.VOXEL_SIZE_IN_METRES + MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF,
                voxelCoord.Y * MyVoxelConstants.VOXEL_SIZE_IN_METRES + MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF,
                voxelCoord.Z * MyVoxelConstants.VOXEL_SIZE_IN_METRES + MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF);
        }

        //  Return voxel's coordinate (from meters) to voxel map (int)
        //  Method return position of voxel corner, not the center.
        public MyMwcVector3Int GetVoxelCoordinateFromMeters(Vector3 pos)
        {
            return new MyMwcVector3Int(
                (int)((pos.X - PositionLeftBottomCorner.X) / MyVoxelConstants.VOXEL_SIZE_IN_METRES),
                (int)((pos.Y - PositionLeftBottomCorner.Y) / MyVoxelConstants.VOXEL_SIZE_IN_METRES),
                (int)((pos.Z - PositionLeftBottomCorner.Z) / MyVoxelConstants.VOXEL_SIZE_IN_METRES));
        }

        //  Return voxel's coordinate (from meters) to voxel map (int)
        //  Method return position of the center, not voxel corner
        public MyMwcVector3Int GetVoxelCenterCoordinateFromMeters(ref Vector3 pos)
        {
            return new MyMwcVector3Int(
                (int)((pos.X - PositionLeftBottomCorner.X - MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF) / MyVoxelConstants.VOXEL_SIZE_IN_METRES),
                (int)((pos.Y - PositionLeftBottomCorner.Y - MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF) / MyVoxelConstants.VOXEL_SIZE_IN_METRES),
                (int)((pos.Z - PositionLeftBottomCorner.Z - MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF) / MyVoxelConstants.VOXEL_SIZE_IN_METRES));
        }

        //  Return voxel's coordinate in world space (in metres)
        //  It's coordinate of closest corner (not center of a voxel)
        public Vector3 GetVoxelPositionAbsolute(ref MyMwcVector3Int voxelCoord)
        {
            return PositionLeftBottomCorner + GetVoxelSizeInMetres(ref voxelCoord);
        }

        //  Return voxel's coordinate in world space (in metres)
        //  It's coordinate of center of a voxel (not corner)
        public Vector3 GetVoxelCenterPositionAbsolute(ref MyMwcVector3Int voxelCoord)
        {
            return PositionLeftBottomCorner + GetVoxelCenterPositionRelative(ref voxelCoord);
        }

        //  Return render cell absolute coordinate in world space (in metres).
        //  It's coordinate of closest corner (not center of a cell)
        //  It can be used to calculate bounding box of a render cell.
        //  IMPORTANT: It's hard to say if border should be offset by half-voxel or not... depends on requirements.
        public Vector3 GetRenderCellPositionAbsolute(ref MyMwcVector3Int cellCoord)
        {
            MyMwcVector3Int voxelCoord = new MyMwcVector3Int(cellCoord.X * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, cellCoord.Y * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, cellCoord.Z * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS);
            //return GetVoxelCenterPositionAbsolute(ref voxelCoord);
            return GetVoxelPositionAbsolute(ref voxelCoord);
        }

        //  Return render cell center absolute coordinate in world space (in metres).
        //  It can be used to calculate bounding box of a render cell.
        //  IMPORTANT: It's hard to say if center should be offset by half-voxel or not... depends on requirements.
        public Vector3 GetRenderCellCenterPositionAbsolute(ref MyMwcVector3Int cellCoord)
        {
            MyMwcVector3Int voxelCoord = new MyMwcVector3Int(cellCoord.X * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, cellCoord.Y * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS, cellCoord.Z * MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_VOXELS);
            return GetVoxelPositionAbsolute(ref voxelCoord) + new Vector3(MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_IN_METRES_HALF);
        }

        //  Return data cell center absolute coordinate in world space (in metres).
        //  It can be used to calculate bounding box of a data cell.
        //  IMPORTANT: It's hard to say if center should be offset by half-voxel or not... depends on requirements.
        public Vector3 GetDataCellCenterPositionAbsolute(ref MyMwcVector3Int cellCoord)
        {
            MyMwcVector3Int voxelCoord = new MyMwcVector3Int(cellCoord.X << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Y << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Z << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
            return GetVoxelPositionAbsolute(ref voxelCoord) + new Vector3(MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES_HALF);
        }

        //  Return data cell center absolute coordinate in world space (in metres).
        //  It can be used to calculate bounding box of a data cell.
        //  IMPORTANT: It's hard to say if center should be offset by half-voxel or not... depends on requirements.
        public Vector3 GetDataCellCenterPositionRelative(ref MyMwcVector3Int cellCoord)
        {
            MyMwcVector3Int voxelCoord = new MyMwcVector3Int(cellCoord.X << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Y << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Z << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
            return GetVoxelSizeInMetres(ref voxelCoord) + new Vector3(MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES_HALF);
        }

        //  Return data cell absolute coordinate in world space (in metres).
        //  It is based on center of voxels (not corner positions).
        //  It can be used to calculate bounding box of a data cell.
        //  IMPORTANT: Border is calculated using voxel centers!!! I tested it and it looks like right solution (at least for cell bounding box)
        public Vector3 GetDataCellPositionAbsolute(ref MyMwcVector3Int cellCoord)
        {
            MyMwcVector3Int voxelCoord = new MyMwcVector3Int(cellCoord.X << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Y << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS, cellCoord.Z << MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS);
            //return GetVoxelPositionAbsolute(ref voxelCoord);// GetVoxelPositionAbsolute(ref tempVoxelCoord);
            return GetVoxelCenterPositionAbsolute(ref voxelCoord);// GetVoxelPositionAbsolute(ref tempVoxelCoord);
        }

        //  Calculates bounding box of a specified data cell. Coordinates are in world/absolute space.
        public void GetDataCellBoundingBox(ref MyMwcVector3Int cellCoord, out BoundingBox outBoundingBox)
        {
            Vector3 dataCellMin = GetDataCellPositionAbsolute(ref cellCoord);
            outBoundingBox = new BoundingBox(dataCellMin, dataCellMin + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_VECTOR_IN_METRES);
        }

        //  Calculates bounding sphere of a specified data cell. Coordinates are in world/absolute space.
        public void GetDataCellBoundingSphere(ref MyMwcVector3Int cellCoord, out BoundingSphere outBoundingSphere)
        {
            Vector3 dataCellMin = GetDataCellPositionAbsolute(ref cellCoord);
            outBoundingSphere = new BoundingSphere(dataCellMin + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES, MyVoxelConstants.VOXEL_DATA_CELL_RADIUS);
        }

        //  Calculates bounding box of a specified render cell. Coordinates are in world/absolute space.
        public void GetRenderCellBoundingBox(ref MyMwcVector3Int cellCoord, out BoundingBox outBoundingBox)
        {
            Vector3 renderCellMin = GetRenderCellPositionAbsolute(ref cellCoord);
            outBoundingBox = new BoundingBox(renderCellMin, renderCellMin + MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_VECTOR_IN_METRES);
        }

        //  Calculates bounding sphere of a specified render cell. Coordinates are in world/absolute space.
        public void GetRenderCellBoundingSphere(ref MyMwcVector3Int cellCoord, out BoundingSphere outBoundingSphere)
        {
            Vector3 renderCellMin = GetRenderCellPositionAbsolute(ref cellCoord);
            outBoundingSphere = new BoundingSphere(renderCellMin + MyVoxelConstants.VOXEL_RENDER_CELL_SIZE_HALF_VECTOR_IN_METRES, MyVoxelConstants.VOXEL_RENDER_CELL_RADIUS);
        }

        //  Same as 'int-version' of ScanLine3D, but this one accepts line coordinates in metres in absolute space.
        //  and returns coordinates in metres in absolute space.
        public bool ScanLine3D(Vector3 lineStart, Vector3 lineEnd, out Vector3? intersection)
        {
            MyMwcVector3Int? temp;
            if (ScanLine3D(GetVoxelCoordinateFromMeters(lineStart), GetVoxelCoordinateFromMeters(lineEnd), out temp) == true)
            {
                MyMwcVector3Int tempForThisCall = temp.Value;
                intersection = GetVoxelCenterPositionAbsolute(ref tempForThisCall);
                return true;
            }

            //  We didn't find any non-empty voxel along the line
            intersection = null;
            return false;
        }

        //  Scans line in 3D space of voxels using bresenhem line scanning algorithm.
        //  Voxels outside of a voxel map are treated as empty
        //  Returns true if intersection with non-empty voxel found and also returns coordinate of first non-empty voxel - in "intersection"
        //  Uses Bresenham algorithm.
        //
        //  IDEA: This method can be optimized so it will start scaning the line not where lineStart is, but where the line enters
        //  the voxel map. Same for lineEnd. Now we do voxel map boundary check for every scanned voxel (it's slower and
        //  we iterate over a lot of not relevant voxels).
        //
        //	Source:
        //	    C code from the article "Voxel Traversal along a 3D Line" by Daniel Cohen, danny@bengus.bgu.ac.il
        //	    in "Graphics Gems IV", Academic Press, 1994
        public bool ScanLine3D(MyMwcVector3Int lineStart, MyMwcVector3Int lineEnd, out MyMwcVector3Int? intersection)
        {
            int i_Dx = lineEnd.X - lineStart.X;
            int i_Dy = lineEnd.Y - lineStart.Y;
            int i_Dz = lineEnd.Z - lineStart.Z;

            int sx = MyUtils.GetBresenhamSgn(i_Dx);
            int sy = MyUtils.GetBresenhamSgn(i_Dy);
            int sz = MyUtils.GetBresenhamSgn(i_Dz);
            int ax = Math.Abs(i_Dx);
            int ay = Math.Abs(i_Dy);
            int az = Math.Abs(i_Dz);
            int bx = 2 * ax;
            int by = 2 * ay;
            int bz = 2 * az;
            int exy = ay - ax;
            int exz = az - ax;
            int ezy = ay - az;

            for (int n = (ax + ay + az); n > 0; n--)
            {
                //  Voxels outside of a voxel map are treated as empty
                if ((lineStart.X >= 0) && (lineStart.Y >= 0) && (lineStart.Z >= 0) && (lineStart.X < Size.X) && (lineStart.Y < Size.Y) && (lineStart.Z < Size.Z))
                {
                    if (GetVoxelContent(ref lineStart) > MyVoxelConstants.VOXEL_CONTENT_EMPTY)
                    {
                        intersection = lineStart;
                        return true;
                    }
                }

                if (exy < 0)
                {
                    if (exz < 0)
                    {
                        lineStart.X += sx;
                        exy += by;
                        exz += bz;
                    }
                    else
                    {
                        lineStart.Z += sz;
                        exz -= bx;
                        ezy += by;
                    }
                }
                else
                {
                    if (ezy < 0)
                    {
                        lineStart.Z += sz;
                        exz -= bx;
                        ezy += by;
                    }
                    else
                    {
                        lineStart.Y += sy;
                        exy -= bx;
                        ezy -= bz;
                    }
                }
            }

            //  We didn't find any non-empty voxel along the line
            intersection = null;
            return false;
        }

        //  Prepares render cell cache. Basicaly, it will precalculate all cells in this voxel map.
        //  Cells that don't contain triangles will be precalced too, but of course not stored in the cache.
        //  This method prepares render and data cells too, so you don't have to call PrepareDataCellCache()
        //  IMPORTANT: Do not use this method because it fills vertex/index buffers and when called from background thread 
        //  while game is minimized through alt+f4, those VB/IB won't be filled
        public void PrepareRenderCellCache()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMapCache::PrepareRenderCellCache");

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < RenderCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < RenderCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < RenderCellsCount.Z; cellCoord.Z++)
                    {
                        MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD0);
                        MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD1);
                    }
                }
            }

            UpdateAABBHr();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        //  Return true if this voxel is on voxel map border
        public bool IsVoxelAtBorder(ref MyMwcVector3Int voxelCoord)
        {
            float offset = MyFakes.VOXEL_MAP_SMALLER_BOUNDARIES ? MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS : 0f;

            if (voxelCoord.X <= 0 + offset) return true;
            if (voxelCoord.Y <= 0 + offset) return true;
            if (voxelCoord.Z <= 0 + offset) return true;
            if (voxelCoord.X >= SizeMinusOne.X - offset) return true;
            if (voxelCoord.Y >= SizeMinusOne.Y - offset) return true;
            if (voxelCoord.Z >= SizeMinusOne.Z - offset) return true;
            return false;
        }

        //  If voxel coord0 (in voxel units, not meters) is outside of the voxelmap, we fix its coordinate so it lie in the voxelmap.
        public void FixVoxelCoord(ref MyMwcVector3Int voxelCoord)
        {
            if (voxelCoord.X < 0) voxelCoord.X = 0;
            if (voxelCoord.Y < 0) voxelCoord.Y = 0;
            if (voxelCoord.Z < 0) voxelCoord.Z = 0;
            if (voxelCoord.X > SizeMinusOne.X) voxelCoord.X = SizeMinusOne.X;
            if (voxelCoord.Y > SizeMinusOne.Y) voxelCoord.Y = SizeMinusOne.Y;
            if (voxelCoord.Z > SizeMinusOne.Z) voxelCoord.Z = SizeMinusOne.Z;
        }

        //  If data cell coord0 (in data cell units, not voxels or metres) is outside of the voxelmap, we fix its coordinate so
        //  it lie in the voxelmap.
        public void FixDataCellCoord(ref MyMwcVector3Int cellCoord)
        {
            if (cellCoord.X < 0) cellCoord.X = 0;
            if (cellCoord.Y < 0) cellCoord.Y = 0;
            if (cellCoord.Z < 0) cellCoord.Z = 0;
            if (cellCoord.X > DataCellsCountMinusOne.X) cellCoord.X = DataCellsCountMinusOne.X;
            if (cellCoord.Y > DataCellsCountMinusOne.Y) cellCoord.Y = DataCellsCountMinusOne.Y;
            if (cellCoord.Z > DataCellsCountMinusOne.Z) cellCoord.Z = DataCellsCountMinusOne.Z;
        }

        //  If render cell coord0 (in render cell units, not voxels or metres) is outside of the voxelmap, we fix its coordinate so
        //  it lie in the voxelmap.
        public void FixRenderCellCoord(ref MyMwcVector3Int cellCoord)
        {
            if (cellCoord.X < 0) cellCoord.X = 0;
            if (cellCoord.Y < 0) cellCoord.Y = 0;
            if (cellCoord.Z < 0) cellCoord.Z = 0;
            if (cellCoord.X > RenderCellsCountMinusOne.X) cellCoord.X = RenderCellsCountMinusOne.X;
            if (cellCoord.Y > RenderCellsCountMinusOne.Y) cellCoord.Y = RenderCellsCountMinusOne.Y;
            if (cellCoord.Z > RenderCellsCountMinusOne.Z) cellCoord.Z = RenderCellsCountMinusOne.Z;
        }

        //  Method fills preallocated array with voxel triangles that potentialy intersects bounding box. Later, JLX will do intersection testing on these triangles.
        //  Input:
        //      boundingBox - bounding box that can intersect with voxel maps
        //  Output:
        //      potentialTriangles - potential voxel triangles
        //      numTriangles - count of potential voxel triangles
        public void GetPotentialTrianglesForColDet(ref int numTriangles, ref BoundingBox boundingBox)
        {
#if DETECT_POTENCIAL_COLLISIONS_CALLS
            m_potencianCollsBBs.Add(boundingBox);
#endif

            //  If no overlap between the voxel map bounding pitch and the box
            if (IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref boundingBox) == false)
                return;

            //  Get min and max cell coordinate where boundingBox can fit
            MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref boundingBox.Min);
            MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref boundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixDataCellCoord(ref cellCoordMin);
            FixDataCellCoord(ref cellCoordMax);

            BoundingBox modelSpaceBox = new BoundingBox(boundingBox.Min - PositionLeftBottomCorner, boundingBox.Max - PositionLeftBottomCorner);

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        MyVoxelCacheCellData cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord, true);
                        if (cachedDataCell == null || cachedDataCell.VoxelTrianglesCount == 0 || cachedDataCell.Octree == null) continue;

                        //  If no overlap between bounding pitch of data cell and the box
                        BoundingBox dataCellBoundingBox;
                        GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);
                        //if (dataCellBoundingBox.Intersects(boundingBox) == false) continue;
                        if (MyUtils.IsBoxIntersectingBox(ref dataCellBoundingBox, ref boundingBox) == false)
                            continue;

                        //  Get cell from cache. If not there, precalc it and store in the cache.
                        //  If null is returned, we know that cell doesn't contain any triangleVertexes so we don't need to do intersections.

                        OctreeOverlapElementList.Clear();
                        
                        cachedDataCell.Octree.BoxQuery(ref modelSpaceBox, OctreeOverlapElementList);
                        foreach (var e in OctreeOverlapElementList)
                        {
                            MyVoxelTriangle voxelTriangle = cachedDataCell.VoxelTriangles[e];

                            MyTriangle_Vertexes triangle;
                            triangle.Vertex0 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex0].Position + PositionLeftBottomCorner;
                            triangle.Vertex1 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex1].Position + PositionLeftBottomCorner;
                            triangle.Vertex2 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex2].Position + PositionLeftBottomCorner;

                            // Uncomment to test performance when doing triangle exact AABB tests (it passes less triangle to physics then, but it takes more CPU)
                            BoundingBox voxelTriangleBoundingBox = BoundingBoxHelper.InitialBox;
                            BoundingBoxHelper.AddTriangle(ref voxelTriangleBoundingBox, ref triangle.Vertex0, ref triangle.Vertex1, ref triangle.Vertex2);

                            //  If no overlap between bounding pitch of voxel triangleVertexes and the box
                            if (voxelTriangleBoundingBox.Intersects(boundingBox) == false)
                                continue;

                            //  IMPORTANT: We swap vertex indices, because JLX wants it so.
                            MyVoxelMaps.PotentialColDetTriangles[numTriangles].Update(
                                ref triangle.Vertex0,
                                ref triangle.Vertex2,
                                ref triangle.Vertex1);

                            if (MyUtils.IsValidNormal(MyVoxelMaps.PotentialColDetTriangles[numTriangles].Normal))
                            {
                                numTriangles++;
                            }
                        }
                    }
                }
            }
        }


        //collisions
        //sphere vs voxel volumetric test
        public bool DoOverlapSphereTest(float sphereRadius, Vector3 spherePos)
        {

            MyVoxelMap voxelMap = this;

            if (voxelMap == null)
                return false;
            Vector3 body0Pos = spherePos; // sphere pos
            BoundingSphere newSphere;
            newSphere.Center = body0Pos;
            newSphere.Radius = sphereRadius;

            //  We will iterate only voxels contained in the bounding box of new sphere, so here we get min/max corned in voxel units
            MyMwcVector3Int minCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                newSphere.Center.X - newSphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                newSphere.Center.Y - newSphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                newSphere.Center.Z - newSphere.Radius - MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            MyMwcVector3Int maxCorner = voxelMap.GetVoxelCoordinateFromMeters(new Vector3(
                newSphere.Center.X + newSphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                newSphere.Center.Y + newSphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES,
                newSphere.Center.Z + newSphere.Radius + MyVoxelConstants.VOXEL_SIZE_IN_METRES));
            voxelMap.FixVoxelCoord(ref minCorner);
            voxelMap.FixVoxelCoord(ref maxCorner);

            MyMwcVector3Int tempVoxelCoord;
            for (tempVoxelCoord.X = minCorner.X; tempVoxelCoord.X <= maxCorner.X; tempVoxelCoord.X++)
            {
                for (tempVoxelCoord.Y = minCorner.Y; tempVoxelCoord.Y <= maxCorner.Y; tempVoxelCoord.Y++)
                {
                    for (tempVoxelCoord.Z = minCorner.Z; tempVoxelCoord.Z <= maxCorner.Z; tempVoxelCoord.Z++)
                    {
                        byte voxelContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);

                        //  Ignore voxels bellow the ISO value (empty, partialy empty...)
                        if (voxelContent < MyVoxelConstants.VOXEL_ISO_LEVEL) continue;

                        Vector3 voxelPosition = voxelMap.GetVoxelCenterPositionAbsolute(ref tempVoxelCoord);

                        //float voxelSize = MyVoxelMaps.GetVoxelContentAsFloat(voxelContent) * MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF;
                        float voxelSize = MyVoxelMaps.GetVoxelContentAsFloat(voxelContent) * MyVoxelConstants.VOXEL_RADIUS;

                        //  If distance to voxel border is less than sphere radius, we have a collision
                        //  So now we calculate normal vector and penetration depth but on OLD sphere
                        float newDistanceToVoxel = Vector3.Distance(voxelPosition, newSphere.Center) - voxelSize;
                        if (newDistanceToVoxel < (newSphere.Radius))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        //  Return list of triangles intersecting specified sphere. 
        //  Triangles are returned in 'retTriangles', and this list must be preallocated!
        //  Count of returned triangles is in 'retTrianglesCount'
        //public void GetTrianglesIntersectingSphere(ref BoundingSphere sphere, int ignoreTriangleWithIndex, List<MyTriangle_Vertex_Normals_Sun> retTriangles, int maxNeighbourTriangles)
        public void GetTrianglesIntersectingSphere(ref BoundingSphere sphere, List<MyTriangle_Vertex_Normals> retTriangles,
            int maxNeighbourTriangles, bool returnAbsoluteVoxelVertexCoordinates)
        {
            //  If sphere doesn't intersect bounding box of a voxel map, we end this method
            if (IsSphereIntersectingBoundingBoxOfThisVoxelMap(ref sphere) == false) return;


            //  Get min and max cell coordinate where boundingBox can fit
            BoundingBox sphereBoundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(sphere, ref sphereBoundingBox);
            MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref sphereBoundingBox.Min);
            MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref sphereBoundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixDataCellCoord(ref cellCoordMin);
            FixDataCellCoord(ref cellCoordMax);

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        //  If no overlap between bounding pitch of data cell and the sphere
                        BoundingBox dataCellBoundingBox;
                        GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);
                        if (MyUtils.IsBoxIntersectingSphere(ref dataCellBoundingBox, ref sphere) == false) continue;

                        //  Get cell from cache. If not there, precalc it and store in the cache.
                        //  If null is returned, we know that cell doesn't contain any triangleVertexes so we don't need to do intersections.
                        MyVoxelCacheCellData cachedDataCell = null;
                        using (MyVoxelCacheData.Locker.AcquireSharedUsing())
                        {
                            cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord, true);

                            if (cachedDataCell == null) continue;

                            for (int i = 0; i < cachedDataCell.VoxelTrianglesCount; i++)
                            {
                                //  If we reached end of the buffer of neighbour triangles, we stop adding new ones. This is better behavior than throwing exception because of array overflow.
                                if (retTriangles.Count == maxNeighbourTriangles)
                                {
                                    return;
                                }

                                MyVoxelTriangle voxelTriangle = cachedDataCell.VoxelTriangles[i];

                                MyVoxelVertex voxelVertex0 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex0];
                                MyVoxelVertex voxelVertex1 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex1];
                                MyVoxelVertex voxelVertex2 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex2];

                                MyTriangle_Vertexes triangle;
                                triangle.Vertex0 = voxelVertex0.Position + PositionLeftBottomCorner;
                                triangle.Vertex1 = voxelVertex1.Position + PositionLeftBottomCorner;
                                triangle.Vertex2 = voxelVertex2.Position + PositionLeftBottomCorner;

                                BoundingBox voxelTriangleBoundingBox = BoundingBoxHelper.InitialBox;
                                BoundingBoxHelper.AddTriangle(ref voxelTriangleBoundingBox, ref triangle.Vertex0, ref triangle.Vertex1, ref triangle.Vertex2);

                                //  First test intersection of triangle's bounding box with line's bounding box. And only if they overlap or intersect, do further intersection tests.
                                if (MyUtils.IsBoxIntersectingSphere(ref voxelTriangleBoundingBox, ref sphere) == true)
                                {
                                    //if (ignoreTriangleWithIndex != cachedDataCell.VoxelTriangles[value].TriangleId)
                                    {
                                        MyPlane trianglePlane = new MyPlane(ref triangle);

                                        if (MyUtils.GetSphereTriangleIntersection(ref sphere, ref trianglePlane, ref triangle) != null)
                                        {
                                            MyTriangle_Vertex_Normals retTriangle;
                                            retTriangle.Normals.Normal0 = voxelVertex0.Normal;
                                            retTriangle.Normals.Normal1 = voxelVertex1.Normal;
                                            retTriangle.Normals.Normal2 = voxelVertex2.Normal;

                                            // Hack: fix thix
                                            retTriangle.Binormals.Normal0 = MyMwcUtils.Normalize(Vector3.Cross(retTriangle.Normals.Normal0, new Vector3(0.2341f, 0.2341f, 0.9865f)));
                                            retTriangle.Binormals.Normal1 = MyMwcUtils.Normalize(Vector3.Cross(retTriangle.Normals.Normal1, new Vector3(0.2341f, 0.2341f, 0.9865f)));
                                            retTriangle.Binormals.Normal2 = MyMwcUtils.Normalize(Vector3.Cross(retTriangle.Normals.Normal2, new Vector3(0.2341f, 0.2341f, 0.9865f)));

                                            retTriangle.Tangents.Normal0 = MyMwcUtils.Normalize(Vector3.Cross(retTriangle.Binormals.Normal0, retTriangle.Normals.Normal0));
                                            retTriangle.Tangents.Normal1 = MyMwcUtils.Normalize(Vector3.Cross(retTriangle.Binormals.Normal1, retTriangle.Normals.Normal1));
                                            retTriangle.Tangents.Normal2 = MyMwcUtils.Normalize(Vector3.Cross(retTriangle.Binormals.Normal2, retTriangle.Normals.Normal2));

                                            if (returnAbsoluteVoxelVertexCoordinates)
                                            {
                                                retTriangle.Vertexes = triangle;
                                            }
                                            else
                                            {
                                                retTriangle.Vertexes.Vertex0 = voxelVertex0.Position;
                                                retTriangle.Vertexes.Vertex1 = voxelVertex1.Position;
                                                retTriangle.Vertexes.Vertex2 = voxelVertex2.Position;
                                            }

                                            retTriangles.Add(retTriangle);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //  Return true if voxel map intersects specified sphere.
        //  This method doesn't return exact point of intersection or any additional data.
        //  We don't look for closest intersection - so we stop on first intersection found.
        public override bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {

            //Lock.AcquireReaderLock(Timeout.Infinite);
            //  If sphere doesn't intersect bounding box of a voxel map, we end this method
            if (IsSphereIntersectingBoundingBoxOfThisVoxelMap(ref sphere) == false) return false;

            //  Get min and max cell coordinate where boundingBox can fit
            BoundingBox sphereBoundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(sphere, ref sphereBoundingBox);
            MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref sphereBoundingBox.Min);
            MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref sphereBoundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixDataCellCoord(ref cellCoordMin);
            FixDataCellCoord(ref cellCoordMax);

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        //  If no overlap between bounding box of data cell and the sphere
                        BoundingBox dataCellBoundingBox;
                        GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);
                        if (MyUtils.IsBoxIntersectingSphere(ref dataCellBoundingBox, ref sphere) == false) continue;

                        //  Get cell from cache. If not there, precalc it and store in the cache.
                        //  If null is returned, we know that cell doesn't contain any triangleVertexes so we don't need to do intersections.
                        MyVoxelCacheCellData cachedDataCell = null;
                        using (MyVoxelCacheData.Locker.AcquireSharedUsing())
                        {
                            cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord, true);

                            if (cachedDataCell == null) continue;

                            for (int i = 0; i < cachedDataCell.VoxelTrianglesCount; i++)
                            {
                                MyVoxelTriangle voxelTriangle = cachedDataCell.VoxelTriangles[i];

                                MyVoxelVertex voxelVertex0 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex0];
                                MyVoxelVertex voxelVertex1 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex1];
                                MyVoxelVertex voxelVertex2 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex2];

                                MyTriangle_Vertexes triangle;
                                triangle.Vertex0 = voxelVertex0.Position + PositionLeftBottomCorner;
                                triangle.Vertex1 = voxelVertex1.Position + PositionLeftBottomCorner;
                                triangle.Vertex2 = voxelVertex2.Position + PositionLeftBottomCorner;

                                BoundingBox voxelTriangleBoundingBox = BoundingBoxHelper.InitialBox;
                                BoundingBoxHelper.AddTriangle(ref voxelTriangleBoundingBox, ref triangle.Vertex0, ref triangle.Vertex1, ref triangle.Vertex2);

                                //  First test intersection of triangle's bounding box with line's bounding box. And only if they overlap or intersect, do further intersection tests.
                                if (MyUtils.IsBoxIntersectingSphere(ref voxelTriangleBoundingBox, ref sphere) == true)
                                {
                                    //if (ignoreTriangleWithIndex != cachedDataCell.VoxelTriangles[value].TriangleId)
                                    {
                                        MyPlane trianglePlane = new MyPlane(ref triangle);

                                        if (MyUtils.GetSphereTriangleIntersection(ref sphere, ref trianglePlane, ref triangle) != null)
                                        {
                                            //  If intersection found - we are finished. We don't need to look for more.
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        //  Return true if voxel map intersects specified box.
        //  This method doesn't return exact point of intersection or any additional data.
        //  We don't look for closest intersection - so we stop on first intersection found.
        public bool GetIntersectionWithBox(ref BoundingBox box)
        {

            //   Lock.AcquireReaderLock(Timeout.Infinite);
            //  If box doesn't intersect bounding box of a voxel map, we end this method
            if (IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref box) == false) return false;

            //  Get min and max cell coordinate where boundingBox can fit            
            MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref box.Min);
            MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref box.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixDataCellCoord(ref cellCoordMin);
            FixDataCellCoord(ref cellCoordMax);

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        //  If no overlap between bounding box of data cell and the sphere
                        BoundingBox dataCellBoundingBox;
                        GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);
                        if (MyUtils.IsBoxIntersectingBox(ref dataCellBoundingBox, ref box))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Called when [world position changed].
        /// </summary>
        /// <param name="source">The source object that caused this event.</param>
        public override void OnWorldPositionChanged(object source)
        {
            if (MyFakes.MW25DCorrectVoxelPosition)
            {
                if (m_voxelFile == MyMwcVoxelFilesEnum.Flat_256x64x256)
                {
                    this.SetPosition(new Vector3(this.GetPosition().X, 50, this.GetPosition().Z));
                }
            }
            base.OnWorldPositionChanged(source);
        }

        protected override void UpdateWorldVolume()
        {
            this.PositionLeftBottomCorner = (this.WorldMatrix.Translation - this.SizeInMetresHalf);
            m_worldAABB = MyUtils.GetNewBoundingBox(PositionLeftBottomCorner, SizeInMetres);
            m_worldVolume = BoundingSphere.CreateFromBoundingBox(m_worldAABB);

            this.RecalculateOreDepositCellBoundingBoxes();

            InvalidateRenderObjects();
        }

        //  Method finds intersection with line and any voxel triangleVertexes in this voxel map. Closes intersection is returned.
        public override bool GetIntersectionWithLine(ref MyLine line, out MyIntersectionResultLineTriangleEx? t, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            ++MyPerformanceCounter.PerCameraDraw.RayCastModelsProcessed;

            t = null;
            //  Line and voxel map bounding box intersection test
            //  We don't need to transform line into voxel map space, because they both are always in world space
            if (IsLineIntersectingBoundingBoxOfThisVoxelMap(ref line) == false)
                return false;

            MyIntersectionResultLineTriangle? result = null;
            if (MyVoxelMaps.GetSortedDataCellList() == null)
                return false;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection");

            MyVoxelMaps.GetSortedDataCellList().Clear();

            MyMwcVector3Int cellCoord;

            MyLine modelSpaceLine = new MyLine(line.From - PositionLeftBottomCorner, line.To - PositionLeftBottomCorner, true);
            MyLine gridLine = modelSpaceLine;

            // convert from center-based to corner-based coordinates
            gridLine.From -= MyVoxelConstants.VOXEL_SIZE_VECTOR_HALF;
            gridLine.To -= MyVoxelConstants.VOXEL_SIZE_VECTOR_HALF;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection AABB sweep");
            SweepResult.Clear();
            MyGridIntersection.Calculate(
                SweepResult,
                (int)MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES,
                modelSpaceLine.From,
                modelSpaceLine.To,
                new MyMwcVector3Int(0, 0, 0),
                DataCellsCountMinusOne
            );
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection test AABBs");
            float? minDistanceUntilNow = null;
            BoundingBox cellBoundingBox;

            if (MyFakes.DRAW_TESTED_CELLS_IN_VOXEL_LINE_INTERSECTION)
            {
                // debug: draw all intersected cells
                var mat = Matrix.Identity;
                var color = new Vector4(1, 1, 0, 0.5f);
                foreach (var coord in SweepResult)
                {
                    cellCoord = coord;
                    GetDataCellBoundingBox(ref cellCoord, out cellBoundingBox);
                    MySimpleObjectDraw.DrawWireFramedBox(ref mat, ref cellBoundingBox, ref color, 0.01f, 1);
                }
            }

            //List<float> allIntersections = new List<float>();
            //foreach (var coord in SweepResult)
            for (int index = 0; index < SweepResult.Count; index++)
            {
                var coord = SweepResult[index];
                cellCoord = coord;
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection init tests");

                GetDataCellBoundingBox(ref cellCoord, out cellBoundingBox);

                float? distanceToBoundingBox = MyUtils.GetLineBoundingBoxIntersection(ref line, ref cellBoundingBox);

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                // Sweep results are sorted; when we get far enough, make an early exit
                const float earlyOutDistance = 1.948557f * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES;  // = sqrt(3) * 9/8 * cell_side
                if (minDistanceUntilNow != null && distanceToBoundingBox != null && minDistanceUntilNow + earlyOutDistance < distanceToBoundingBox.Value)
                {
                    break;
                }
                //  Continue to check only if distance to data cell bounding box is closer than closest intersection point found so far
                //                if ((distanceToBoundingBox.HasValue == false) || ((minDistanceUntilNow != null) && (minDistanceUntilNow < distanceToBoundingBox.Value)))
                //                    continue;

                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection get cell");

                //  Get cell from cache. If not there, precalc it and store in the cache.
                //  If null is returned, we know that cell doesn't contain any triangleVertexes so we don't need to do intersections.
                MyVoxelCacheCellData cachedDataCell = null;
                using (MyVoxelCacheData.Locker.AcquireSharedUsing())
                {

                    cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord, true);


                    //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    if (cachedDataCell == null || cachedDataCell.VoxelTrianglesCount == 0) continue;

                    GetCellLineIntersectionOctree(ref result, ref modelSpaceLine, ref minDistanceUntilNow, cachedDataCell, flags);
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            if (result != null)
            {
                t = new MyIntersectionResultLineTriangleEx(result.Value, this, ref line);
                return true;
            }
            else
            {
                t = null;
                return false;
            }
        }

        private void GetCellLineIntersectionOctree(ref MyIntersectionResultLineTriangle? result, ref MyLine modelSpaceLine, ref float? minDistanceUntilNow, MyVoxelCacheCellData cachedDataCell, IntersectionFlags flags)
        {
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.ClearList Octree");
            OctreeOverlapElementList.Clear();
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            if (cachedDataCell.Octree != null)
            {
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection Octree prun");
                var ray = new Ray(modelSpaceLine.From, modelSpaceLine.Direction);
                cachedDataCell.Octree.GetIntersectionWithLine(ref ray, OctreeOverlapElementList);
                //cachedDataCell.Octree.GetAllTriangles(OctreeOverlapElementList);
                //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelMap.LineIntersection test tris Octree");

            for (int j = 0; j < OctreeOverlapElementList.Count; j++)
            {
                var i = OctreeOverlapElementList[j];

                if (cachedDataCell.VoxelTriangles == null) //probably not calculated yet
                    continue;

                // this should never happen
                if (i >= cachedDataCell.VoxelTriangles.Length)
                {
                    MyCommonDebugUtils.AssertDebug(i < cachedDataCell.VoxelTriangles.Length);
                    continue;
                }

                MyVoxelTriangle voxelTriangle = cachedDataCell.VoxelTriangles[i];

                MyVoxelVertex voxelVertex0 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex0];
                MyVoxelVertex voxelVertex1 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex1];
                MyVoxelVertex voxelVertex2 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex2];

                MyTriangle_Vertexes triangleVertices;
                triangleVertices.Vertex0 = voxelVertex0.Position;
                triangleVertices.Vertex1 = voxelVertex1.Position;
                triangleVertices.Vertex2 = voxelVertex2.Position;


                Vector3 calculatedTriangleNormal = MyUtils.GetNormalVectorFromTriangle(ref triangleVertices);

                //We dont want backside intersections
                if (((int)(flags & IntersectionFlags.FLIPPED_TRIANGLES) == 0) &&
                    Vector3.Dot(modelSpaceLine.Direction, calculatedTriangleNormal) > 0)
                        continue;

                // draw triangles that were tested
                if (MyFakes.DRAW_TESTED_TRIANGLES_IN_VOXEL_LINE_INTERSECTION)
                {
                    Vector3 avg02 = (voxelVertex0.Position + voxelVertex1.Position + voxelVertex2.Position) * 0.33333333333f * 0.2f;
                    MyDebugDrawCachedLines.AddLine(PositionLeftBottomCorner + voxelVertex0.Position * 0.8f + avg02, PositionLeftBottomCorner + voxelVertex1.Position * 0.8f + avg02, Color.White, Color.White);
                    MyDebugDrawCachedLines.AddLine(PositionLeftBottomCorner + voxelVertex1.Position * 0.8f + avg02, PositionLeftBottomCorner + voxelVertex2.Position * 0.8f + avg02, Color.White, Color.White);
                    MyDebugDrawCachedLines.AddLine(PositionLeftBottomCorner + voxelVertex2.Position * 0.8f + avg02, PositionLeftBottomCorner + voxelVertex0.Position * 0.8f + avg02, Color.White, Color.White);
                }

                // AABB intersection test removed, AABB is tested inside BVH
                float? distance = MyUtils.GetLineTriangleIntersection(ref modelSpaceLine, ref triangleVertices);

                //if (distance != null) allIntersections.Add(distance.Value);

                //  If intersection occured and if distance to intersection is closer to origin than any previous intersection
                if ((distance != null) && ((result == null) || (distance.Value < result.Value.Distance)))
                {
                    minDistanceUntilNow = distance.Value;
                    result = new MyIntersectionResultLineTriangle(ref triangleVertices, ref calculatedTriangleNormal, distance.Value);
                }
            }

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            MyIntersectionResultLineTriangleEx? result;
            GetIntersectionWithLine(ref line, out result);
            v = null;
            if (result != null)
            {
                v = result.Value.IntersectionPointInWorldSpace;
                return true;
            }
            return false;
        }

        //  Checks if specified sphere intersects this bounding box of this voxel map.
        public bool IsSphereIntersectingBoundingBoxOfThisVoxelMap(ref BoundingSphere boundingSphere)
        {
            bool outRet;
            WorldAABB.Intersects(ref boundingSphere, out outRet);
            return outRet;
        }

        //  Checks if specified sphere intersects this bounding box of this voxel map.
        public bool IsSphereIntersectingBoundingSphereOfThisVoxelMap(ref BoundingSphere boundingSphere)
        {
            // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("IsSphereIntersectingBoundingSphereOfThisVoxelMap");
            bool outRet;
            WorldVolume.Intersects(ref boundingSphere, out outRet);

            // MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            return outRet;

        }

        //  Checks if specified box intersects bounding box of this this voxel map.
        public bool IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref BoundingBox boundingBox)
        {
            bool outRet;
            WorldAABB.Intersects(ref boundingBox, out outRet);
            return outRet;
        }

        //  Checks if specified line intersects bounding box of this voxel map.
        public bool IsLineIntersectingBoundingBoxOfThisVoxelMap(ref MyLine line)
        {
            return MyUtils.IsLineIntersectingBoundingBox(ref line, ref m_worldAABB);
        }

        //  This is just wrapper for refed IsDataCellInVoxelMap()
        public bool IsDataCellInVoxelMap(MyMwcVector3Int cellCoord)
        {
            return IsDataCellInVoxelMap(ref cellCoord);
        }

        //  Checks if precalculatedCellCoord is in the voxel map (true), if it isn't outside (false).
        //  Input parameter is cell coordinate (not voxel, not in meters)
        public bool IsDataCellInVoxelMap(ref MyMwcVector3Int cellCoord)
        {
            if (cellCoord.X < 0) return false;
            if (cellCoord.Y < 0) return false;
            if (cellCoord.Z < 0) return false;
            if (cellCoord.X >= DataCellsCount.X) return false;
            if (cellCoord.Y >= DataCellsCount.Y) return false;
            if (cellCoord.Z >= DataCellsCount.Z) return false;
            return true;
        }

        //  Checks if voxel is in the voxel map (true), if it isn't outside (false).
        //  Input parameter is voxel coordinate in voxel coords
        public bool IsVoxelInVoxelMap(ref MyMwcVector3Int voxelCoord)
        {
            if (voxelCoord.X < 0) return false;
            if (voxelCoord.Y < 0) return false;
            if (voxelCoord.Z < 0) return false;
            if (voxelCoord.X >= Size.X) return false;
            if (voxelCoord.Y >= Size.Y) return false;
            if (voxelCoord.Z >= Size.Z) return false;
            return true;
        }

        //  Calculates reverb value for our sound engine. For interval, see MySounds.ReverbControl.
        //  This value is calculated by finding how many mixed or full voxel cells are around the camera (it is similar to occlusion lighting). If no, reverb is 0. If many, reverb is 100.
        //  Return null if camera isn't in the voxel map bounding box (so we can't calculate reverb)
        //  Otherwise calculate reverb value and return float.
        public float? GetReverb(Vector3 cameraPosition)
        {
            //  If camera isn't in voxel map, we can't calculate reverb, so end this method and try another voxel map
            if (m_worldAABB.Contains(cameraPosition) == ContainmentType.Disjoint) return null;

            MyMwcVector3Int cameraCellCoord = GetDataCellCoordinateFromMeters(ref cameraPosition);

            float reverb = 0;

            const int CELLS_IN_ONE_DIRECTION_COUNT = 2;
            const int CELLS_IN_ONE_DIRECTION_COUNT_TOTAL = CELLS_IN_ONE_DIRECTION_COUNT * 2 + 1;
            const float CELLS_TOTAL = (float)(CELLS_IN_ONE_DIRECTION_COUNT_TOTAL * CELLS_IN_ONE_DIRECTION_COUNT_TOTAL * CELLS_IN_ONE_DIRECTION_COUNT_TOTAL);

            MyMwcVector3Int cellCoordMin = new MyMwcVector3Int(cameraCellCoord.X - CELLS_IN_ONE_DIRECTION_COUNT, cameraCellCoord.Y - CELLS_IN_ONE_DIRECTION_COUNT, cameraCellCoord.Z - CELLS_IN_ONE_DIRECTION_COUNT);
            MyMwcVector3Int cellCoordMax = new MyMwcVector3Int(cameraCellCoord.X + CELLS_IN_ONE_DIRECTION_COUNT, cameraCellCoord.Y + CELLS_IN_ONE_DIRECTION_COUNT, cameraCellCoord.Z + CELLS_IN_ONE_DIRECTION_COUNT);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixDataCellCoord(ref cellCoordMin);
            FixDataCellCoord(ref cellCoordMax);

            //  Check
            MyMwcVector3Int cellCoord;
            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        MyVoxelContentCell cell = GetCell(ref cellCoord);
                        if ((cell == null) || (cell.CellType != MyVoxelCellType.EMPTY))
                        {
                            reverb++;
                        }
                    }
                }
            }

            //  At this point, 'reverb' contains count of data cells that aren't empty. We need to convert
            //  it to interval 0..100, so we divide it by total count of data cells we checked, except
            //  the cell camera is in. Number 2 is for negative and positive side, number 3 is for XYZ.
            return MathHelper.Clamp(reverb / CELLS_TOTAL * MyAudioConstants.REVERB_MAX, 0, MyAudioConstants.REVERB_MAX);
        }

        //  Save all of the vertices in this voxel map to the OBJ file format
        public void SaveVoxelVertices(StreamWriter sw)
        {
            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                    {
                        MyVoxelCacheCellData cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord);
                        if (cachedDataCell != null)
                        {
                            for (int k = 0; k < cachedDataCell.VoxelVerticesCount; k++)
                            {
                                MyVoxelVertex vert = cachedDataCell.VoxelVertices[k];
                                sw.WriteLine("v " +
                                    MyValueFormatter.GetFormatedFloat(vert.Position.X, 6, "") + ' ' +
                                    MyValueFormatter.GetFormatedFloat(vert.Position.Y, 6, "") + ' ' +
                                    MyValueFormatter.GetFormatedFloat(vert.Position.Z, 6, ""));
                            }
                        }

                    }
                }
            }

        }

        //  Save all the normals of this voxel map to the OBJ file format
        public void SaveVoxelNormals(StreamWriter sw)
        {
            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                    {
                        MyVoxelCacheCellData cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord);
                        if (cachedDataCell != null)
                        {
                            for (int k = 0; k < cachedDataCell.VoxelVerticesCount; k++)
                            {
                                MyVoxelVertex vert = cachedDataCell.VoxelVertices[k];
                                sw.WriteLine("vn " +
                                    MyValueFormatter.GetFormatedFloat(vert.Normal.X, 6, "") + ' ' +
                                    MyValueFormatter.GetFormatedFloat(vert.Normal.Y, 6, "") + ' ' +
                                    MyValueFormatter.GetFormatedFloat(vert.Normal.Z, 6, ""));
                            }
                        }

                    }
                }
            }
        }

        //  Save each triangle in the voxel map to the OBJ file format
        public void SaveVoxelFaces(StreamWriter sw, ref int vertexOffset)
        {
            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                    {
                        MyVoxelCacheCellData cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord);
                        if (cachedDataCell != null)
                        {
                            for (int k = 0; k < cachedDataCell.VoxelTrianglesCount; k++)
                            {
                                MyVoxelTriangle triangle = cachedDataCell.VoxelTriangles[k];
                                int index0 = ((int)triangle.VertexIndex0) + vertexOffset + 1;
                                int index1 = ((int)triangle.VertexIndex1) + vertexOffset + 1;
                                int index2 = ((int)triangle.VertexIndex2) + vertexOffset + 1;
                                sw.Write("f " +
                                    index0.ToString() + "//" + index0.ToString() + ' ' +
                                    index1.ToString() + "//" + index1.ToString() + ' ' +
                                    index2.ToString() + "//" + index2.ToString() + '\n');
                            }
                            //  Update offset for each data cell
                            vertexOffset += cachedDataCell.VoxelVerticesCount;
                        }
                    }
                }
            }
        }


        private MyCompressionFileSave SaveVoxelContents(bool saveMaterialContent = false)
        {
            MyCompressionFileSave compressFile = new MyCompressionFileSave();

            //  Version of a VOX file
            compressFile.Add(MyVoxelConstants.VOXEL_FILE_ACTUAL_VERSION);

            //  Size of this voxel map (in voxels)
            compressFile.Add(Size.X);
            compressFile.Add(Size.Y);
            compressFile.Add(Size.Z);

            //  Size of data cell in voxels, doesn't have to be same as current size specified by our constants.
            compressFile.Add(MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);
            compressFile.Add(MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);
            compressFile.Add(MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS);

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                    {
                        MyVoxelContentCell voxelCell = GetCell(ref cellCoord);

                        if (voxelCell == null)
                        {
                            //  Voxel wasn't found in cell dictionary, so cell must be full
                            compressFile.Add((byte)MyVoxelCellType.FULL);
                        }
                        else
                        {
                            //MyCommonDebugUtils.AssertDebug((voxelCell.CellType == MyVoxelCellType.EMPTY) || (voxelCell.CellType == MyVoxelCellType.MIXED));

                            //  Cell type
                            compressFile.Add((byte)voxelCell.CellType);

                            //  Cell coordinates - otherwise we won't know which cell is this when loading a voxel map
                            //compressFile.Add(cellCoord.X);
                            //compressFile.Add(cellCoord.Y);
                            //compressFile.Add(cellCoord.Z);

                            //  If we are here, cell is empty or mixed. If empty, we don't need to save each individual voxel.
                            //  But if it is mixed, we will do it here.
                            if (voxelCell.CellType == MyVoxelCellType.MIXED)
                            {
                                MyMwcVector3Int voxelCoordInCell;
                                for (voxelCoordInCell.X = 0; voxelCoordInCell.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.X++)
                                {
                                    for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Y++)
                                    {
                                        for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Z++)
                                        {
                                            compressFile.Add(voxelCell.GetVoxelContent(ref voxelCoordInCell));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (saveMaterialContent)
            {
                // Save material cells
                for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
                {
                    for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                        {
                            var matCell = this.GetVoxelMaterialCells()[cellCoord.X][cellCoord.Y][cellCoord.Z];

                            MyMwcVector3Int voxelCoordInCell = new MyMwcVector3Int(0, 0, 0);

                            bool isWholeMaterial = matCell.IsSingleMaterialForWholeCell();
                            compressFile.Add((byte)(isWholeMaterial ? 1 : 0));
                            if (isWholeMaterial)
                            {
                                MyMwcVoxelMaterialsEnum singleMaterial = matCell.GetMaterial(ref voxelCoordInCell);
                                compressFile.Add((byte)singleMaterial);
                            }
                            else
                            {
                                for (voxelCoordInCell.X = 0; voxelCoordInCell.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.X++)
                                {
                                    for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Y++)
                                    {
                                        for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Z++)
                                        {
                                            compressFile.Add((byte)matCell.GetMaterial(ref voxelCoordInCell));
                                            compressFile.Add((byte)matCell.GetIndestructibleContent(ref voxelCoordInCell));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return compressFile;
        }

        //  Save voxel map into a file. 
        //  Final byte array is compressed and saved on disk.
        //  This method used the idea of data cells, but size of data cell doesn't have to be same as current size specified by our constants.
        //  It is stored in the file only for knowing how many voxels has each data cell.
        public void SaveVoxelContents(string fileName, bool saveMaterialContent = false)
        {
            MyCompressionFileSave compressFile = SaveVoxelContents(saveMaterialContent);

            compressFile.Save(fileName);
        }

        public void SaveVoxelContents(out byte[] savedVoxelContents, bool saveMaterialContent = false)
        {
            MyCompressionFileSave compressFile = SaveVoxelContents(saveMaterialContent);

            compressFile.Save(out savedVoxelContents);
        }


        List<Tuple<BoundingBox, Vector4>> m_voxelsDebugDraw = new List<Tuple<BoundingBox, Vector4>>();

        public override bool DebugDraw()
        {
            if (MyFakes.DRAW_TESTED_TRIANGLES_IN_VOXEL_LINE_INTERSECTION)
            {
                MyDebugDrawCachedLines.DrawLines();
                MyDebugDrawCachedLines.Clear();
            }

            BoundingBox boundingBox = WorldAABB;

            //Vector4 clrgr = new Vector4(0,1,0,1);
            //MyDebugDraw.DrawAABBLine(ref boundingBox, ref clrgr, 1);

            return true;



            //  Get min and max cell coordinate where camera bounding box can fit
            MyMwcVector3Int cellCoordMin = GetVoxelRenderCellCoordinateFromMeters(ref boundingBox.Min);
            MyMwcVector3Int cellCoordMax = GetVoxelRenderCellCoordinateFromMeters(ref boundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixRenderCellCoord(ref cellCoordMin);
            FixRenderCellCoord(ref cellCoordMax);

            MyVoxelMaps.GetSortedRenderCells().Clear();

            //  Get non-empty render cells visible to the frustum and sort them by distance to camera
            MyMwcVector3Int cellCoord;

            //MyRender.GetRenderProfiler().EndProfilingBlock();

            //MyRender.GetRenderProfiler().StartProfilingBlock("for for for");

            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        //  If no overlap between bounding box of a render cell and camera bounding frustum, we skip this cell
                        //  We need to do this because checking only bounding boxes isn't sufficient as frustum's bounding box is too large
                        BoundingBox renderCellBoundingBox;
                        GetRenderCellBoundingBox(ref cellCoord, out renderCellBoundingBox);

                        /*
                  MyVoxelCacheCellRender myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD0);

                  foreach (MyVoxelCacheCellRenderBatch batch in myVoxelCacheCellRender.Batches)
                  {
                      short[] indices = new short[batch.IndexBufferSize / 2];
                      batch.IndexBuffer.GetData(indices);

                      MinerWars.AppCode.Game.Utils.MyVertexFormats.MyVertexFormatVoxelSingleMaterial[] vertices = new Utils.MyVertexFormats.MyVertexFormatVoxelSingleMaterial[batch.VertexBufferCount];

                      batch.VertexBuffer.GetData(vertices);

                      for (int i = 0; i < indices.Length; i += 3)
                      {
                          MinerWars.AppCode.Game.Utils.MyVertexFormats.MyVertexFormatVoxelSingleMaterial v1 = vertices[indices[i + 0]];
                          MinerWars.AppCode.Game.Utils.MyVertexFormats.MyVertexFormatVoxelSingleMaterial v2 = vertices[indices[i + 1]];
                          MinerWars.AppCode.Game.Utils.MyVertexFormats.MyVertexFormatVoxelSingleMaterial v3 = vertices[indices[i + 2]];

                          if (renderCellBoundingBox.Contains(v1.Position) == ContainmentType.Disjoint)
                          {
                          }

                          if (renderCellBoundingBox.Contains(v2.Position) == ContainmentType.Disjoint)
                          {
                          }

                          if (renderCellBoundingBox.Contains(v3.Position) == ContainmentType.Disjoint)
                          {
                          }
                      }
                  }   */

                        Vector4 cl = Vector4.One;
                        MyDebugDraw.DrawAABBLine(ref renderCellBoundingBox, ref cl, 1);
                    }
                }
            }

            //return true;
#if DETECT_POTENCIAL_COLLISIONS_CALLS
            foreach (BoundingBox bbox in m_potencianCollsBBs)
            {
                Vector4 color = new Vector4(1, 0, 0, 1);
                BoundingBox b = bbox;
                MyDebugDraw.DrawAABB(ref b, ref color, 1);
            }

            m_potencianCollsBBs.Clear();
#endif
            m_voxelsDebugDraw.Clear();

            Vector4 c = Vector4.One;
            Vector4 c2 = new Vector4(0, 1, 0, 1);
            Vector4 c3 = new Vector4(1, 0, 0, 1);
            /*
            BoundingBox box = new BoundingBox();
            box = box.CreateInvalid();
            Vector3 rn = new Vector3();
            rn.X = MyMwcUtils.GetRandomFloat(0,1) * m_worldAABB.Size().X;
            rn.Y = MyMwcUtils.GetRandomFloat(0,1) * m_worldAABB.Size().Y;
            rn.Z = MyMwcUtils.GetRandomFloat(0, 1) * m_worldAABB.Size().Z;
            rn += m_worldAABB.Min;
            box = box.Include(ref rn);
            rn = new Vector3();
            rn.X = MyMwcUtils.GetRandomFloat(0, 1) * m_worldAABB.Size().X;
            rn.Y = MyMwcUtils.GetRandomFloat(0, 1) * m_worldAABB.Size().Y;
            rn.Z = MyMwcUtils.GetRandomFloat(0, 1) * m_worldAABB.Size().Z;
            rn += m_worldAABB.Min;
            box = box.Include(ref rn);

            m_selectionHighlightColor = box.Min;
            m_collisionHighlightColor = box.Max;
                */

            //BoundingBox box = new BoundingBox(m_selectionHighlightColor, m_collisionHighlightColor);
            //m_selectionHighlightColor = Vector3.Zero;
            /*
MyEntity e;
MyEntities.TryGetEntityById(new MyEntityIdentifier(23), out e);
if (e == null)
    return true;
            */
            BoundingBox box = WorldAABB;


            //MyDebugDraw.DrawAABBLowRes(ref box, ref v, 1);

            //  Get min and max cell coordinate where boundingBox can fit
            cellCoordMin = GetDataCellCoordinateFromMeters(ref box.Min);
            cellCoordMax = GetDataCellCoordinateFromMeters(ref box.Max);

            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        //  If no overlap between bounding box of a render cell and camera bounding frustum, we skip this cell
                        //  We need to do this because checking only bounding boxes isn't sufficient as frustum's bounding box is too large
                        BoundingBox dataCellBoundingBox;
                        GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);

                        if (GetDataCellAverageContent(ref cellCoord) > 0) 
                            MyDebugDraw.DrawAABBLowRes(ref dataCellBoundingBox, ref c3, 1);
                        /*
if (dataCellBoundingBox.Intersects(box) == false)
    continue;

//  Get min and max cell coordinate where boundingBox can fit
MyMwcVector3Int cellCoordMin2 = GetDataCellCoordinateFromMeters(ref dataCellBoundingBox.Min);
MyMwcVector3Int cellCoordMax2 = GetDataCellCoordinateFromMeters(ref dataCellBoundingBox.Max);

BoundingBox dataCellBoundingBox2;
GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox2);
MyDebugDraw.DrawAABBLowRes(ref dataCellBoundingBox2, ref v2, 1);

MyVoxelCacheCellData cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord);
if (cachedDataCell == null) continue;




if (cachedDataCell.VoxelTrianglesCount > 0)
{
    foreach (MyVoxelTriangle tri in cachedDataCell.VoxelTriangles)
    {
        MyDebugDraw.DrawTriangle(
            cachedDataCell.VoxelVertices[tri.VertexIndex0].Position + PositionLeftBottomCorner,
            cachedDataCell.VoxelVertices[tri.VertexIndex1].Position + PositionLeftBottomCorner,
            cachedDataCell.VoxelVertices[tri.VertexIndex2].Position + PositionLeftBottomCorner,
            Color.White,
            Color.White,
            Color.White);
    }
}                         */
                    }
                }
            }



            /*
            //This code now draws colored spheres depending on voxel cell material
          // if (MyMwcFinalBuildConstants.DrawVoxelContentAsBillboards)
            {
                //  Get min and max cell coordinate where boundingBox can fit
                MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref MyCamera.BoundingBox.Min);
                MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref MyCamera.BoundingBox.Max);

                //  Fix min and max cell coordinates so they don't overlap the voxelmap
                FixDataCellCoord(ref cellCoordMin);
                FixDataCellCoord(ref cellCoordMax);

                MyMwcVector3Int cellCoord;
                for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
                {
                    for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                        {
                            //  If no overlap between bounding box of a render cell and camera bounding frustum, we skip this cell
                            //  We need to do this because checking only bounding boxes isn't sufficient as frustum's bounding box is too large
                            BoundingBox dataCellBoundingBox;
                            GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);

                            if (!MyCamera.IsInFrustum(ref dataCellBoundingBox))
                            {
                                continue;
                            }

                            // draw only nonempty cells
                            if (GetDataCellAverageContent(ref cellCoord) <= 0)
                            {
                                continue;
                            }

                            MyMwcVector3Int cellCoordInVoxels = GetVoxelCoordinatesOfDataCell(ref cellCoord);

                            var matCell = this.GetVoxelMaterialCells()[cellCoord.X][cellCoord.Y][cellCoord.Z];


                            if (!matCell.IsMaterialForWholeCell())
                            {

                                MyMwcVector3Int voxelCoordInCell;

                                for (voxelCoordInCell.X = 0; voxelCoordInCell.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.X++)
                                {
                                    for (voxelCoordInCell.Y = 0; voxelCoordInCell.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Y++)
                                    {
                                        for (voxelCoordInCell.Z = 0; voxelCoordInCell.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordInCell.Z++)
                                        {
                                            // compressFile.Add((byte)matCell.GetMaterial(ref voxelCoordInCell));
                                            // compressFile.Add((byte)matCell.GetIndestructibleContent(ref voxelCoordInCell));
                                            Vector4 color = new Vector4(0.5f, 0, 0, 0.5f);

                                            Vector3 delta = (dataCellBoundingBox.Max - dataCellBoundingBox.Min) / MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;

                                            BoundingBox vvv = new BoundingBox(dataCellBoundingBox.Min + new Vector3(delta.X * voxelCoordInCell.X, delta.Y * voxelCoordInCell.Y, delta.Z * voxelCoordInCell.Z),
                                                dataCellBoundingBox.Min + new Vector3(delta.X * (voxelCoordInCell.X + 1), delta.Y * (voxelCoordInCell.Y + 1), delta.Z * (voxelCoordInCell.Z + 1)));


                                            MyMwcVoxelMaterialsEnum mat = matCell.GetMaterial(ref voxelCoordInCell);
                                            if (mat != MyMwcVoxelMaterialsEnum.Lava_01)
                                            {
                                                color = new Vector4(0, 0, 1, 1);
                                                
                                            }
                                            else
                                                color = new Vector4(1, 0, 0, 1);

                                            MyMwcVector3Int cnt;
                                            cnt.X = cellCoordInVoxels.X + voxelCoordInCell.X;
                                            cnt.Y = cellCoordInVoxels.Y + voxelCoordInCell.Y;
                                            cnt.Z = cellCoordInVoxels.Z + voxelCoordInCell.Z;

                                            if (GetVoxelContent(ref cnt) > 0)
                                            {
                                                m_voxelsDebugDraw.Add(new Tuple<BoundingBox, Vector4>(vvv, color));
                                            }

                                            // MyDebugDraw.DrawAABBSolidLowRes(vvv, color, 1);


                                        }
                                    }
                                }


                            }
                            else
                            {
                            }

                            //MyDebugDraw.DrawAABBLowRes(ref renderCellBoundingBox, ref color, 1);
                            //MyDebugDraw.DrawAABBSolidLowRes(dataCellBoundingBox, color, 1);
                        }
                    }
                }
            }

    //        if (EntityId.Value.NumericValue == 42059)
            {

                foreach (Tuple<BoundingBox, Vector4> bb in m_voxelsDebugDraw)
                {
                    BoundingBox d = bb.Item1;
                    Vector4 c = bb.Item2;
                    MyDebugDraw.DrawSphereWireframe(d.GetCenter(), 1, new Vector3(c.X, c.Y, c.Z), 1);
                    //MyDebugDraw.DrawAABBSolidLowRes(bb.Item1, bb.Item2, 1);
                }
            }
            */

            if (!base.DebugDraw())
                return false;

            return true;
        }

        static void AddRenderCellToSortingList(MyVoxelCacheCellRender cachedRenderCell)
        {
            //  If cell is empty, skip it (don't add to sorting list)
            if (cachedRenderCell.Batches.Count <= 0) return;

            //  Cell is in the frustum and isn't empty, so add to sorting list
            MyVoxelMaps.GetSortedRenderCells().Add(new MyRenderCellForSorting(cachedRenderCell));
        }

        //  This distance isn't from center of render cell to camera, but from bounding sphere of a render cell. So if camera
        //  is inside, distance is zero.
        public float GetSmallestDistanceFromCameraToRenderCell(ref MyMwcVector3Int cellCoord)
        {
            BoundingSphere renderCellBoundingSphere;
            Vector3 campos = MyCamera.Position;
            GetRenderCellBoundingSphere(ref cellCoord, out renderCellBoundingSphere);
            return MyUtils.GetSmallestDistanceToSphereAlwaysPositive(ref campos, ref renderCellBoundingSphere);
        }

        //  This distance isn't from center of render cell to camera, but from opposite side of bounding sphere of a render cell. 
        public float GetLargestDistanceFromCameraToRenderCell(ref MyMwcVector3Int cellCoord)
        {
            BoundingSphere renderCellBoundingSphere;
            GetRenderCellBoundingSphere(ref cellCoord, out renderCellBoundingSphere);
            Vector3 campos = MyCamera.Position;
            return MyUtils.GetLargestDistanceToSphere(ref campos, ref renderCellBoundingSphere);
        }

        //  This distance isn't from center of data cell to camera, but from bounding sphere of a render cell. So if camera
        //  is inside, distance is zero.
        public float GetSmallestDistanceFromCameraToDataCell(ref MyMwcVector3Int cellCoord)
        {
            BoundingSphere dataCellBoundingSphere;
            GetDataCellBoundingSphere(ref cellCoord, out dataCellBoundingSphere);
            Vector3 campos = MyCamera.Position;
            return MyUtils.GetSmallestDistanceToSphereAlwaysPositive(ref campos, ref dataCellBoundingSphere);
        }

        /*
        public void GetRenderElements(List<MyRender.MyRenderElement> renderElements, BoundingBox boundingBox, MyLodTypeEnum lod)
        {
            if (MyRender.SkipVoxels)
                return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMap.GetRenderElements");

            using (MyVoxelCacheData.Locker.AcquireSharedUsing())
            {
                //MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMap::GetRenderElements");

                //MyRender.GetRenderProfiler().StartProfilingBlock("GetVoxelRenderCellCoordinateFromMeters");

                //  Get min and max cell coordinate where camera bounding box can fit
                MyMwcVector3Int cellCoordMin = GetVoxelRenderCellCoordinateFromMeters(ref boundingBox.Min);
                MyMwcVector3Int cellCoordMax = GetVoxelRenderCellCoordinateFromMeters(ref boundingBox.Max);

                //  Fix min and max cell coordinates so they don't overlap the voxelmap
                FixRenderCellCoord(ref cellCoordMin);
                FixRenderCellCoord(ref cellCoordMax);

                MyVoxelMaps.GetSortedRenderCells().Clear();

                //  Get non-empty render cells visible to the frustum and sort them by distance to camera
                MyMwcVector3Int cellCoord;
                MyVoxelCacheCellRender myVoxelCacheCellRender = null;

                //MyRender.GetRenderProfiler().EndProfilingBlock();

                //MyRender.GetRenderProfiler().StartProfilingBlock("for for for");

                for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
                {
                    for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                        {
                            //  If no overlap between bounding box of a render cell and camera bounding frustum, we skip this cell
                            //  We need to do this because checking only bounding boxes isn't sufficient as frustum's bounding box is too large
                            BoundingBox renderCellBoundingBox;
                            GetRenderCellBoundingBox(ref cellCoord, out renderCellBoundingBox);

                            if (!MyCamera.IsInFrustum(ref renderCellBoundingBox))
                            {
                                continue;
                            }

                            if (lod == MyLodTypeEnum.LOD0)
                            {
                                float distanceForLod0 = GetSmallestDistanceFromCameraToRenderCell(ref cellCoord);

                                if (distanceForLod0 <= MyRender.CurrentRenderSetup.LodTransitionFar.Value)
                                {
                                    myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD0);
                                    //myVoxelCacheCellRender.CellBoundingBox = renderCellBoundingBox;

                                    if (myVoxelCacheCellRender != null)
                                    {
                                        //AddRenderCellToSortingList(myVoxelCacheCellRender);
                                        GetRenderElementsFromRenderCell(renderElements, myVoxelCacheCellRender);
                                    }
                                }
                            }
                            else if (lod == MyLodTypeEnum.LOD1)
                            {
                                float distanceForLod1 = GetLargestDistanceFromCameraToRenderCell(ref cellCoord);
                                float distanceForBackground = GetSmallestDistanceFromCameraToRenderCell(ref cellCoord);
                                if (distanceForLod1 >= MyRender.CurrentRenderSetup.LodTransitionNear.Value
                                    &&
                                    distanceForBackground < MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd.Value)
                                {
                                    myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD1);
                                    //myVoxelCacheCellRender.CellBoundingBox = renderCellBoundingBox;
                                    //AddRenderCellToSortingList(myVoxelCacheCellRender);

                                    if (myVoxelCacheCellRender != null)
                                    {
                                        GetRenderElementsFromRenderCell(renderElements, myVoxelCacheCellRender);
                                    }
                                }
                            }

                            if (myVoxelCacheCellRender == null)
                                continue;
                        }
                    }
                }

                //MyRender.GetRenderProfiler().EndProfilingBlock();
                //MyRender.GetRenderProfiler().EndProfilingBlock();
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }  */


        public void GetRenderElements(List<MyRender.MyRenderElement> renderElements, MyMwcVector3Int cellCoord, MyLodTypeEnum lod)
        {
            if (MyRender.SkipVoxels)
                return;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMap.GetRenderElements2");

            
                MyVoxelCacheCellRender myVoxelCacheCellRender = null;

                if (lod == MyLodTypeEnum.LOD0)
                {
                    float distanceForLod0 = GetSmallestDistanceFromCameraToRenderCell(ref cellCoord);

                    if (distanceForLod0 <= MyRender.CurrentRenderSetup.LodTransitionFar.Value)
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Lock");
                        using (MyVoxelCacheData.Locker.AcquireExclusiveUsing())
                        {
                            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                            myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD0);

                            //myVoxelCacheCellRender.CellBoundingBox = renderCellBoundingBox;

                            if (myVoxelCacheCellRender != null)
                            {
                                //AddRenderCellToSortingList(myVoxelCacheCellRender);
                                MyRender.GetRenderProfiler().StartProfilingBlock("GetRenderElementsFromRenderCellLOD0");
                                GetRenderElementsFromRenderCell(renderElements, myVoxelCacheCellRender);
                                MyRender.GetRenderProfiler().EndProfilingBlock();
                            }
                        }
                    }
                }
                else if (lod == MyLodTypeEnum.LOD1)
                {
                    float distanceForLod1 = GetLargestDistanceFromCameraToRenderCell(ref cellCoord);
                    float distanceForBackground = GetSmallestDistanceFromCameraToRenderCell(ref cellCoord);
                    if (distanceForLod1 >= MyRender.CurrentRenderSetup.LodTransitionNear.Value
                        &&
                        distanceForBackground < MyRender.CurrentRenderSetup.LodTransitionBackgroundEnd.Value)
                    {
                         MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Lock");
                         using (MyVoxelCacheData.Locker.AcquireExclusiveUsing())
                         {
                             MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                             myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, MyLodTypeEnum.LOD1);

                             //myVoxelCacheCellRender.CellBoundingBox = renderCellBoundingBox;
                             //AddRenderCellToSortingList(myVoxelCacheCellRender);

                             if (myVoxelCacheCellRender != null)
                             {
                                 MyRender.GetRenderProfiler().StartProfilingBlock("GetRenderElementsFromRenderCellLOD0");
                                 GetRenderElementsFromRenderCell(renderElements, myVoxelCacheCellRender);
                                 MyRender.GetRenderProfiler().EndProfilingBlock();
                             }
                         }
                    }
                }
                //MyRender.GetRenderProfiler().EndProfilingBlock();
                //MyRender.GetRenderProfiler().EndProfilingBlock();
            

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        void GetRenderElementsFromRenderCell(List<MyRender.MyRenderElement> renderElements, MyVoxelCacheCellRender renderCell)
        {      
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetElementsFromRenderCell");

            //  Performance counters
            if (renderCell.CellHashType == MyLodTypeEnum.LOD1)
                MyPerformanceCounter.PerCameraDraw.RenderCellsInFrustum_LOD1++;
            else
                MyPerformanceCounter.PerCameraDraw.RenderCellsInFrustum_LOD0++;

            foreach (MyVoxelCacheCellRenderBatch batch in renderCell.Batches)
            {
                if (batch.IndexBuffer == null)
                    continue;

                MyRender.MyRenderElement renderElement;
                MyRender.AllocateRenderElement(out renderElement);
                //MyRender.AddRenderElement(renderElements, out renderElement);

                //renderElement.DebugName = this.Name;
                renderElement.Entity = this;
                renderElement.IndexBuffer = batch.IndexBuffer;
                renderElement.VertexDeclaration = MinerWars.AppCode.Game.Utils.VertexFormats.MyVertexFormatVoxelSingleMaterial.VertexDeclaration;
                renderElement.VertexStride = MinerWars.AppCode.Game.Utils.VertexFormats.MyVertexFormatVoxelSingleMaterial.Stride;
                renderElement.VertexCount = batch.VertexBufferCount;
                renderElement.VertexBuffer = batch.VertexBuffer;
                renderElement.IndexStart = 0;
                renderElement.TriCount = batch.IndexBufferCount / 3;
                renderElement.WorldMatrix = Matrix.CreateTranslation(PositionLeftBottomCorner);
                renderElement.DrawTechnique = MyMeshDrawTechnique.VOXEL_MAP;
                renderElement.Material = m_fakeVoxelMaterial;
                renderElement.VoxelBatch = batch;

                renderElements.Add(renderElement);

                MyPerformanceCounter.PerCameraDraw.VoxelTrianglesInFrustum += renderElement.TriCount;

            }
              
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }
        /*
        public void GetRenderElementsForShadowmap(List<MyRender.MyRenderElement> renderElements, ref BoundingBox boundingBox, BoundingFrustum frustum, MyLodTypeEnum lod, bool generateIfNotExists)
        {
            if (MyRender.SkipVoxels)
                return;
            using (MyVoxelCacheData.Locker.AcquireExclusiveUsing())
            {

                //   Lock.AcquireReaderLock(Timeout.Infinite);

                //  Get min and max cell coordinate where camera bounding box can fit
                MyMwcVector3Int cellCoordMin = GetVoxelRenderCellCoordinateFromMeters(ref boundingBox.Min);
                MyMwcVector3Int cellCoordMax = GetVoxelRenderCellCoordinateFromMeters(ref boundingBox.Max);

                //  Fix min and max cell coordinates so they don't overlap the voxelmap
                FixRenderCellCoord(ref cellCoordMin);
                FixRenderCellCoord(ref cellCoordMax);

                SharpDX.Direct3D9.Device device = MyMinerGame.Static.GraphicsDevice;

                if (MyVoxelMaps.GetSortedRenderCells() != null)
                    MyVoxelMaps.GetSortedRenderCells().Clear();

                //  Get non-empty render cells visible to the frustum and sort them by distance to camera
                MyMwcVector3Int cellCoord;
                MyVoxelCacheCellRender myVoxelCacheCellRender;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetElements from MyVoxelMap");

                for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
                {
                    for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                        {
                            //Skip render cells outside frustum
                            if (frustum.Contains(boundingBox) == ContainmentType.Disjoint)
                                return;

                            var loadingMode = generateIfNotExists ? MyVoxelCellQueryMode.LoadImmediately : MyVoxelCellQueryMode.DoNotLoad;

                            myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, lod, loadingMode);
                            if (myVoxelCacheCellRender == null)
                                continue;
                            //myVoxelCacheCellRender.CellBoundingBox = renderCellBoundingBox;

                            foreach (MyVoxelCacheCellRenderBatch batch in myVoxelCacheCellRender.Batches)
                            {
                                if (batch.IndexBufferCount == 0)
                                    continue;

                                MyRender.MyRenderElement renderElement;
                                //MyRender.AddRenderElement(renderElements, out renderElement);
                                MyRender.AllocateRenderElement(out renderElement);

                                //renderElement.DebugName = this.Name;
                                renderElement.Entity = this;
                                renderElement.IndexBuffer = batch.IndexBuffer;
                                renderElement.IndexStart = 0;
                                if (renderElement.IndexBuffer != null)
                                {
                                    renderElement.TriCount = batch.IndexBufferCount / 3;
                                }
                                renderElement.VertexDeclaration = MinerWars.AppCode.Game.Utils.VertexFormats.MyVertexFormatVoxelSingleMaterial.VertexDeclaration;
                                renderElement.VertexStride = MinerWars.AppCode.Game.Utils.VertexFormats.MyVertexFormatVoxelSingleMaterial.Stride;
                                renderElement.VertexCount = batch.VertexBufferCount;
                                renderElement.VertexBuffer = batch.VertexBuffer;
                                renderElement.WorldMatrix = Matrix.CreateTranslation(PositionLeftBottomCorner);
                                // renderElement.Center = GetDataCellPositionAbsolute(ref cellCoord) + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES;
                                renderElement.VoxelBatch = batch;

                                renderElements.Add(renderElement);

                                //Vector3 one = Vector3.One;
                                //renderElement.Material = m_fakeVoxelMaterial;
                                //renderElement.BoundingBox = myVoxelCacheCellRender.CellBoundingBox;
                            }
                        }
                    }
                }

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            } 
        }        */

        public void GetRenderElementsForShadowmap(List<MyRender.MyRenderElement> renderElements, MyMwcVector3Int cellCoord, MyLodTypeEnum lod, bool generateIfNotExists)
        {      
            if (MyRender.SkipVoxels)
                return;

            //   Lock.AcquireReaderLock(Timeout.Infinite);

            SharpDX.Direct3D9.Device device = MyMinerGame.Static.GraphicsDevice;

            //  Get non-empty render cells visible to the frustum and sort them by distance to camera
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GetElements from MyVoxelMap");
            var loadingMode = generateIfNotExists ? MyVoxelCellQueryMode.LoadImmediately : MyVoxelCellQueryMode.DoNotLoad;

            MyVoxelCacheCellRender myVoxelCacheCellRender = null;
            using (MyVoxelCacheData.Locker.AcquireExclusiveUsing())
            {
                myVoxelCacheCellRender = MyVoxelCacheRender.GetCell(this, ref cellCoord, lod, loadingMode);

                if (myVoxelCacheCellRender == null)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();     
                    return;
                }

                foreach (MyVoxelCacheCellRenderBatch batch in myVoxelCacheCellRender.Batches)
                {
                    if (batch.IndexBufferCount == 0)
                        continue;

                    MyRender.MyRenderElement renderElement;
                    //MyRender.AddRenderElement(renderElements, out renderElement);
                    MyRender.AllocateRenderElement(out renderElement);

                    //renderElement.DebugName = this.Name;
                    renderElement.Entity = this;
                    renderElement.IndexBuffer = batch.IndexBuffer;
                    renderElement.IndexStart = 0;
                    if (renderElement.IndexBuffer != null)
                    {
                        renderElement.TriCount = batch.IndexBufferCount / 3;
                    }
                    renderElement.VertexDeclaration = MinerWars.AppCode.Game.Utils.VertexFormats.MyVertexFormatVoxelSingleMaterial.VertexDeclaration;
                    renderElement.VertexStride = MinerWars.AppCode.Game.Utils.VertexFormats.MyVertexFormatVoxelSingleMaterial.Stride;
                    renderElement.VertexCount = batch.VertexBufferCount;
                    renderElement.VertexBuffer = batch.VertexBuffer;
                    renderElement.WorldMatrix = Matrix.CreateTranslation(PositionLeftBottomCorner);
                    // renderElement.Center = GetDataCellPositionAbsolute(ref cellCoord) + MyVoxelConstants.VOXEL_DATA_CELL_SIZE_HALF_VECTOR_IN_METRES;
                    renderElement.VoxelBatch = batch;

                    renderElements.Add(renderElement);

                    //Vector3 one = Vector3.One;
                    //renderElement.Material = m_fakeVoxelMaterial;
                    //renderElement.BoundingBox = myVoxelCacheCellRender.CellBoundingBox;
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();     
        }

        public override bool Draw(MyRenderObject renderObject = null)
        {
            if (MyMwcFinalBuildConstants.DrawVoxelContentAsBillboards)
                DrawVoxelsAsBillboards();

            return base.Draw(renderObject);
        }

        //  This method is for debugging only, so doesn't have to be super-optimal.
        //  We don't use data/render cache, so we even don't check if cell is empty. It's because we want to be sure we display all voxels.
        //  IMPORTANT: This method adds billboars to list in MyParticles. So if there is too many voxels in front of camera, some particles
        //  IMPORTANT: may not be displayed (because billboard buffer is full and we sort particles by distance).
        public void DrawVoxelsAsBillboards()
        {
            //  Get min and max cell coordinate where camera bounding box can fit
            MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref MyCamera.BoundingBox.Min);
            MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref MyCamera.BoundingBox.Max);

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixDataCellCoord(ref cellCoordMin);
            FixDataCellCoord(ref cellCoordMax);

            //  Get data cells visible to the frustum and draw its voxels as billboards
            MyMwcVector3Int cellCoord;
            for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
            {
                for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                    {
                        float distanceToDataCell = GetSmallestDistanceFromCameraToDataCell(ref cellCoord);

                        //  Don't draw cells far away, because it's very slow
                        if (distanceToDataCell < (30.0f))
                        {
                            MyMwcVector3Int voxelCoordTemp;
                            for (voxelCoordTemp.X = 0; voxelCoordTemp.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordTemp.X++)
                            {
                                for (voxelCoordTemp.Y = 0; voxelCoordTemp.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordTemp.Y++)
                                {
                                    for (voxelCoordTemp.Z = 0; voxelCoordTemp.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; voxelCoordTemp.Z++)
                                    {
                                        MyMwcVector3Int cellVoxelCoord = GetVoxelCoordinatesOfDataCell(ref cellCoord);
                                        MyMwcVector3Int voxelCoord;
                                        voxelCoord.X = cellVoxelCoord.X + voxelCoordTemp.X;
                                        voxelCoord.Y = cellVoxelCoord.Y + voxelCoordTemp.Y;
                                        voxelCoord.Z = cellVoxelCoord.Z + voxelCoordTemp.Z;

                                        float radius = GetVoxelContentAsFloat(ref voxelCoord);
                                        if (radius > 0)
                                        {
                                            radius *= MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF;// *0.99f;
                                            MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Test, new Vector4(1, 1, 1, 1),
                                                GetVoxelCenterPositionAbsolute(ref voxelCoord), radius, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //  Draw bounding box around whole voxel map (12 lines)
        public override void DebugDrawOBB()
        {
            base.DebugDrawOBB();

            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, 0, 0), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, 0, 0), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, 0, 0), PositionLeftBottomCorner + new Vector3(0, SizeInMetres.Y, 0), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, 0, 0), PositionLeftBottomCorner + new Vector3(0, 0, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, SizeInMetres.Y, 0), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, SizeInMetres.Y, 0), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(SizeInMetres.X, SizeInMetres.Y, 0), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, SizeInMetres.Y, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(SizeInMetres.X, 0, 0), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, 0, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, SizeInMetres.Y, 0), PositionLeftBottomCorner + new Vector3(0, SizeInMetres.Y, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, 0, SizeInMetres.Z), PositionLeftBottomCorner + new Vector3(0, SizeInMetres.Y, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(SizeInMetres.X, 0, SizeInMetres.Z), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, SizeInMetres.Y, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(SizeInMetres.X, 0, 0), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, SizeInMetres.Y, 0), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, SizeInMetres.Y, SizeInMetres.Z), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, SizeInMetres.Y, SizeInMetres.Z), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(PositionLeftBottomCorner + new Vector3(0, 0, SizeInMetres.Z), PositionLeftBottomCorner + new Vector3(SizeInMetres.X, 0, SizeInMetres.Z), Color.Red, Color.Red);
        }

        //  Draw bounding boxes of data cells
        public void DrawDataCellBoundingBoxres()
        {
            return;

            if (MyMwcFinalBuildConstants.DrawHelperPrimitives == false) return;

            Color color = new Color(new Vector4(0.35f, 0, 0, 1));

            //  Draw lines perpendicular to YZ plane
            for (int y = 0; y <= DataCellsCount.Y; y++)
            {
                for (int z = 0; z <= DataCellsCount.Z; z++)
                {
                    Vector3 start = PositionLeftBottomCorner + new Vector3(0, y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES);
                    Vector3 end = PositionLeftBottomCorner + new Vector3(SizeInMetres.X, y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES);
                    MyDebugDraw.DrawLine3D(start, end, color, color);
                }
            }

            //  Draw lines perpendicular to XY plane
            for (int x = 0; x <= DataCellsCount.X; x++)
            {
                for (int y = 0; y <= DataCellsCount.Y; y++)
                {
                    Vector3 start = PositionLeftBottomCorner + new Vector3(x * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, 0);
                    Vector3 end = PositionLeftBottomCorner + new Vector3(x * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, SizeInMetres.Z);
                    MyDebugDraw.DrawLine3D(start, end, color, color);
                }
            }

            //  Draw lines perpendicular to XZ plane
            for (int x = 0; x <= DataCellsCount.X; x++)
            {
                for (int z = 0; z <= DataCellsCount.Z; z++)
                {
                    Vector3 start = PositionLeftBottomCorner + new Vector3(x * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, 0, z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES);
                    Vector3 end = PositionLeftBottomCorner + new Vector3(x * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES, SizeInMetres.Y, z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_METRES);
                    MyDebugDraw.DrawLine3D(start, end, color, color);
                }
            }
        }

        public void DrawBounding()
        {
            Vector4 color = new Vector4(0f, 0.95f, 0f, 0.1f);
            float x = SizeInMetres.X / 2f;
            float y = SizeInMetres.Y / 2f;
            float z = SizeInMetres.Z / 2f;
            AddBoundingBillboard(Vector3.Left, Vector3.Forward, Vector3.Up, new Vector3(-x, 0, 0), y, z, color);
            AddBoundingBillboard(Vector3.Left, Vector3.Forward, Vector3.Up, new Vector3(x, 0, 0), y, z, color);
            AddBoundingBillboard(Vector3.Left, Vector3.Forward, Vector3.Right, new Vector3(0, y, 0), x, z, color);
            AddBoundingBillboard(Vector3.Left, Vector3.Forward, Vector3.Right, new Vector3(0, -y, 0), x, z, color);
            AddBoundingBillboard(Vector3.Left, Vector3.Up, Vector3.Right, new Vector3(0, 0, z), x, y, color);
            AddBoundingBillboard(Vector3.Left, Vector3.Up, Vector3.Right, new Vector3(0, 0, -z), x, y, color);
        }

        private void AddBoundingBillboard(Vector3 localForward, Vector3 localUp, Vector3 localRight, Vector3 localTranslation, float width, float height, Vector4 color)
        {
            Matrix localRot = Matrix.Identity;
            localRot.Forward = localForward;
            localRot.Up = localUp;
            localRot.Right = localRight;
            localRot.Translation = localTranslation;
            Matrix matrix = localRot * this.GetOrientation();
            matrix.Translation += this.GetPosition();

            MyTransparentGeometry.AddBillboardOriented(MyTransparentMaterialEnum.ContainerBorderSelected, color, matrix.Translation, matrix.Left, matrix.Up, width, height);
        }

        //  Get cell. If not found (cell is full), null is returned.
        //  IMPORTANT: This method doesn't check if input cell coord0 is inside of the voxel map.
        //  IMPORTANT: This method has overloaded version that is sometimes needed too.
        public MyVoxelContentCell GetCell(MyMwcVector3Int cellCoord)
        {
            if (!CheckVoxelCoord(cellCoord)) return null;
            return m_voxelContentCells[cellCoord.X][cellCoord.Y][cellCoord.Z];
        }

        //  Get cell. If not found (cell is full), null is returned.
        //  IMPORTANT: This method doesn't check if input cell coord0 is inside of the voxel map.
        //  IMPORTANT: This method has overloaded version that is sometimes needed too.
        public MyVoxelContentCell GetCell(ref MyMwcVector3Int cellCoord)
        {
            if (!CheckVoxelCoord(ref cellCoord)) return null;
            return m_voxelContentCells[cellCoord.X][cellCoord.Y][cellCoord.Z];
        }

        private bool CheckVoxelCoord(MyMwcVector3Int cellCoord)
        {
            if (cellCoord.X >= 0 && cellCoord.Y >= 0 && cellCoord.Z >= 0)
            {
                if (cellCoord.X < m_voxelContentCells.Length &&
                    cellCoord.Y < m_voxelContentCells[cellCoord.X].Length &&
                    cellCoord.Z < m_voxelContentCells[cellCoord.X][cellCoord.Y].Length)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckVoxelCoord(ref MyMwcVector3Int cellCoord)
        {
            if (cellCoord.X >= 0 && cellCoord.Y >= 0 && cellCoord.Z >= 0)
            {
                if (cellCoord.X < m_voxelContentCells.Length &&
                    cellCoord.Y < m_voxelContentCells[cellCoord.X].Length &&
                    cellCoord.Z < m_voxelContentCells[cellCoord.X][cellCoord.Y].Length)
                {
                    return true;
                }
            }
            return false;
        }

        //  Allocates cell from a buffer, store reference to dictionary and return reference to the cell
        //  Use it when changing cell type from full to empty or mixed.
        public MyVoxelContentCell AddCell(ref MyMwcVector3Int cellCoord)
        {
            //  Adding or creating cell can be made only once
            Debug.Assert(m_voxelContentCells[cellCoord.X][cellCoord.Y][cellCoord.Z] == null);

            MyVoxelContentCell ret = new MyVoxelContentCell();
            m_voxelContentCells[cellCoord.X][cellCoord.Y][cellCoord.Z] = ret;

            return ret;
        }

        public MyVoxelContentCell[][][] GetVoxelContentCells()
        {
            return m_voxelContentCells;
        }

        public MyVoxelMaterialCell[][][] GetVoxelMaterialCells()
        {
            return m_voxelMaterialCells;
        }

        //  Checks if cell didn't change to FULL and if is, we set it to null
        public void CheckIfCellChangedToFull(MyVoxelContentCell voxelCell, ref MyMwcVector3Int cellCoord)
        {
            if (voxelCell.CellType == MyVoxelCellType.FULL)
            {
                m_voxelContentCells[cellCoord.X][cellCoord.Y][cellCoord.Z] = null;
            }
        }

        //  Return true if data cell specified by 'cellPosition' and its neighborhood cells/voxels are completely full or completely empty.
        //  If some are empty and other are full, that's taken as not completely full or empty. All cells must have same type and that can't be mixed.
        //  It's used to know if cell may have triangles. So if completely full/empty, it can't have triangles.        
        //  We care only about cells on right/bottom side of specified cell (because during precalc it get its voxels).
        public bool IsDataCellCompletelyFullOrCompletelyEmpty(ref MyMwcVector3Int cellCoord)
        {
            MyMwcVector3Int cellCoordMax = new MyMwcVector3Int(cellCoord.X + 1, cellCoord.Y + 1, cellCoord.Z + 1);

            //  Fix max cell coordinates so they don't fall from voxelmap
            FixDataCellCoord(ref cellCoordMax);

            bool foundFull = false;
            bool foundEmpty = false;

            MyMwcVector3Int tempCellCoord;
            for (tempCellCoord.X = cellCoord.X; tempCellCoord.X <= cellCoordMax.X; tempCellCoord.X++)
            {
                for (tempCellCoord.Y = cellCoord.Y; tempCellCoord.Y <= cellCoordMax.Y; tempCellCoord.Y++)
                {
                    for (tempCellCoord.Z = cellCoord.Z; tempCellCoord.Z <= cellCoordMax.Z; tempCellCoord.Z++)
                    {
                        //  Cell may be FULL because it's still null, or if allocated, may have cell type equal to FULL
                        MyVoxelContentCell voxelCell = GetCell(ref tempCellCoord);

                        if (voxelCell == null)
                        {
                            foundFull = true;
                        }
                        else if (voxelCell.CellType == MyVoxelCellType.MIXED)
                        {
                            return false;
                        }
                        else if (voxelCell.CellType == MyVoxelCellType.FULL)
                        {
                            foundFull = true;
                        }
                        else if (voxelCell.CellType == MyVoxelCellType.EMPTY)
                        {
                            foundEmpty = true;
                        }

                        if ((foundFull == true) && (foundEmpty == true))
                        {
                            if (MyFakes.MWCURIOSITY)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }

            //  If we get here, all cells are full
            return true;
        }

        //  Calculates average material per data cell so it can be used for drawing LOD
        public void CalcAverageDataCellMaterials()
        {
            MyPerformanceTimer.CalcAverageDataCellMaterials.Start();

            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < DataCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < DataCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < DataCellsCount.Z; cellCoord.Z++)
                    {
                        m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].CalcAverageCellMaterial();
                    }
                }
            }

            MyPerformanceTimer.CalcAverageDataCellMaterials.End();
        }

        //  Iterate over all data cells and checks if "average data cell content" value is calculated properly (because it might have been incorectly calculated during voxel map loading)
        //  If doesn't match, exception is thrown. It is good to call this method for every loaded voxel map. At least during debuging.
        public void CheckAverageDataCellContent()
        {
            MyMwcVector3Int dataCellCoord;
            for (dataCellCoord.X = 0; dataCellCoord.X < DataCellsCount.X; dataCellCoord.X++)
            {
                for (dataCellCoord.Y = 0; dataCellCoord.Y < DataCellsCount.Y; dataCellCoord.Y++)
                {
                    for (dataCellCoord.Z = 0; dataCellCoord.Z < DataCellsCount.Z; dataCellCoord.Z++)
                    {
                        byte average = GetDataCellAverageContent(ref dataCellCoord);

                        int calculated = 0;

                        MyMwcVector3Int tempVoxelCoord;
                        for (tempVoxelCoord.X = 0; tempVoxelCoord.X < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; tempVoxelCoord.X++)
                        {
                            for (tempVoxelCoord.Y = 0; tempVoxelCoord.Y < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; tempVoxelCoord.Y++)
                            {
                                for (tempVoxelCoord.Z = 0; tempVoxelCoord.Z < MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS; tempVoxelCoord.Z++)
                                {
                                    MyMwcVector3Int voxelCoord = new MyMwcVector3Int(
                                        dataCellCoord.X * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + tempVoxelCoord.X,
                                        dataCellCoord.Y * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + tempVoxelCoord.Y,
                                        dataCellCoord.Z * MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS + tempVoxelCoord.Z);

                                    calculated += GetVoxelContent(ref voxelCoord);
                                }
                            }
                        }

                        byte voxelContent = (byte)(calculated / MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_TOTAL);
                        if (voxelContent != average)
                        {
                            throw new Exception("Average data cell content in cell [" + dataCellCoord.X + ", " + dataCellCoord.Y + ", " + dataCellCoord.Z + "] doesn't match!!!");
                        }
                    }
                }
            }
        }

        //  This is just for writing debugging information
        public void WriteDebugInfo()
        {
            if (!MyMwcLog.LogFlag(LoggingOptions.VOXEL_MAPS))
            {
                return;
            }

            MyMwcLog.WriteLine("MyVoxelMap.WriteDebugInfo - Start", LoggingOptions.VOXEL_MAPS);
            MyMwcLog.IncreaseIndent(LoggingOptions.VOXEL_MAPS);

            MyMwcLog.WriteLine("ID: " + VoxelMapId, LoggingOptions.VOXEL_MAPS);
            MyMwcLog.WriteLine("Size: " + MyUtils.GetFormatedVector3Int(Size), LoggingOptions.VOXEL_MAPS);

            int fullCellsCount = 0;
            int emptyCellsCount = 0;
            int mixedCellsCount = 0;
            MyMwcVector3Int dataCellCoord;
            for (dataCellCoord.X = 0; dataCellCoord.X < DataCellsCount.X; dataCellCoord.X++)
            {
                for (dataCellCoord.Y = 0; dataCellCoord.Y < DataCellsCount.Y; dataCellCoord.Y++)
                {
                    for (dataCellCoord.Z = 0; dataCellCoord.Z < DataCellsCount.Z; dataCellCoord.Z++)
                    {
                        MyVoxelContentCell voxelCell = GetCell(ref dataCellCoord);

                        if (voxelCell == null)
                        {
                            fullCellsCount++;
                        }
                        else if (voxelCell.CellType == MyVoxelCellType.EMPTY)
                        {
                            emptyCellsCount++;
                        }
                        else if (voxelCell.CellType == MyVoxelCellType.MIXED)
                        {
                            mixedCellsCount++;
                        }
                        else
                        {
                            throw new MyMwcExceptionApplicationShouldNotGetHere();
                        }
                    }
                }
            }
            MyMwcLog.WriteLine("FullCellsCount: " + MyValueFormatter.GetFormatedInt(fullCellsCount), LoggingOptions.VOXEL_MAPS);
            MyMwcLog.WriteLine("EmptyCellsCount: " + MyValueFormatter.GetFormatedInt(emptyCellsCount), LoggingOptions.VOXEL_MAPS);
            MyMwcLog.WriteLine("MixedCellsCount: " + MyValueFormatter.GetFormatedInt(mixedCellsCount), LoggingOptions.VOXEL_MAPS);

            MyMwcLog.DecreaseIndent(LoggingOptions.VOXEL_MAPS);
            MyMwcLog.WriteLine("MyVoxelMap.WriteDebugInfo - End", LoggingOptions.VOXEL_MAPS);
        }

        //  If material is indestructible, then indestructible content value must be FULL, because that means that is the smallest 
        //  possible voxel content for that voxel
        //  This method is used when initializing voxel map so if material is indestructible, we set whole cell to FULL
        byte GetIndestructibleContentByMaterial(MyMwcVoxelMaterialsEnum material)
        {
            if (MyVoxelMaterials.Get(material).IsIndestructible)
            {
                return MyVoxelConstants.VOXEL_CONTENT_FULL;
            }
            else
            {
                return MyVoxelConstants.VOXEL_CONTENT_EMPTY;
            }
        }

        //  This method is used when merging individual voxels from voxel map file, so if material is indestructible, we need 
        //  to look on voxel value at that place
        byte GetIndestructibleContentsByMaterialAndContent(MyMwcVoxelMaterialsEnum material, byte content)
        {
            if (MyVoxelMaterials.Get(material).IsIndestructible)
            {
                return content;
            }
            else
            {
                return MyVoxelConstants.VOXEL_CONTENT_EMPTY;
            }
        }

        public static bool IsNewAllowed(MyMwcVoxelFilesEnum voxelFileEnum)
        {
            int totalDataCells = MyVoxelMaps.GetTotalDataCellsCount();
            return IsNewAllowed(voxelFileEnum, ref totalDataCells);
        }

        public static bool IsNewAllowed(MyMwcVoxelFilesEnum voxelFileEnum, ref int totalDataCells)
        {
            bool allowed = false;

            MyVoxelFile voxelFile = MyVoxelFiles.Get(voxelFileEnum);
            MyCompressionFileLoad decompressFile = new MyCompressionFileLoad(voxelFile.GetVoxFilePath());

            int fileVersion = decompressFile.GetInt32();

            //  Size of this voxel map (in voxels)
            int sizeX = decompressFile.GetInt32();
            int sizeY = decompressFile.GetInt32();
            int sizeZ = decompressFile.GetInt32();

            int dataCellCountX = sizeX >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;
            int dataCellCountY = sizeY >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;
            int dataCellCountZ = sizeZ >> MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS_BITS;

            int newVoxelCellCount = dataCellCountX * dataCellCountY * dataCellCountZ;

            totalDataCells = newVoxelCellCount + totalDataCells;
            if (totalDataCells < MyVoxelConstants.MAX_VOXEL_MAPS_DATA_CELL_COUNT)
            {
                allowed = true;
            }

            return allowed;
        }

        public static Vector3 GetVoxelSizeInMetres(ref MyMwcVector3Int sizeInVoxels)
        {
            return new Vector3(sizeInVoxels.X * MyVoxelConstants.VOXEL_SIZE_IN_METRES, sizeInVoxels.Y * MyVoxelConstants.VOXEL_SIZE_IN_METRES, sizeInVoxels.Z * MyVoxelConstants.VOXEL_SIZE_IN_METRES);
        }

        public static Vector3 GetVoxelSizeInMetres(ref MyMwcVector3Short sizeInVoxels)
        {
            return new Vector3(sizeInVoxels.X * MyVoxelConstants.VOXEL_SIZE_IN_METRES, sizeInVoxels.Y * MyVoxelConstants.VOXEL_SIZE_IN_METRES, sizeInVoxels.Z * MyVoxelConstants.VOXEL_SIZE_IN_METRES);
        }

        public static void GetAsteroidsBySizeInMeters(int size, List<MyMwcVoxelFilesEnum> asteroids, bool treatNonuniformAsBigger)
        {
            switch (size)
            {
                case (int)(512 * MyVoxelConstants.VOXEL_SIZE_IN_METRES):
                    asteroids.Add(MyMwcVoxelFilesEnum.PerfectSphereSplitted_512x512x512);
                    asteroids.Add(MyMwcVoxelFilesEnum.PerfectSphereWithFewTunnels_512x512x512);
                    asteroids.Add(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels_512x512x512);
                    asteroids.Add(MyMwcVoxelFilesEnum.PerfectSphereWithMassiveTunnels2_512x512x512);
                    asteroids.Add(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel_512x512x512);
                    asteroids.Add(MyMwcVoxelFilesEnum.PerfectSphereWithRaceTunnel2_512x512x512);
                    break;

                case (int)(256 * MyVoxelConstants.VOXEL_SIZE_IN_METRES):
                    asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_256x256x256);
                    if (treatNonuniformAsBigger)
                    {
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusStorySector_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_2_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.VerticalIsland_128x256x128);
                        asteroids.Add(MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128);
                    }
                    break;

                case (int)(128 * MyVoxelConstants.VOXEL_SIZE_IN_METRES):
                    asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCraters_128x128x128);
                    asteroids.Add(MyMwcVoxelFilesEnum.ScratchedBoulder_128x128x128);
                    asteroids.Add(MyMwcVoxelFilesEnum.SphereWithLargeCutOut_128x128x128);
                    asteroids.Add(MyMwcVoxelFilesEnum.VerticalIsland_128x128x128);
                    if (treatNonuniformAsBigger)
                    {
                        asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64);
                        asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64);
                    }
                    else
                    {
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusStorySector_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_2_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusWithManyTunnels_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.TorusWithSmallTunnel_256x128x256);
                        asteroids.Add(MyMwcVoxelFilesEnum.VerticalIsland_128x256x128);
                        asteroids.Add(MyMwcVoxelFilesEnum.VerticalIslandStorySector_128x256x128);
                    }
                    break;

                case (int)(64 * MyVoxelConstants.VOXEL_SIZE_IN_METRES):
                    if (!treatNonuniformAsBigger)
                    {
                        asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphereWithCorridor_128x64x64);
                        asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphereWithHoles_64x128x64);
                    }
                    asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphere1_64x64x64);
                    asteroids.Add(MyMwcVoxelFilesEnum.DeformedSphere2_64x64x64);
                    break;
            }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_VoxelMap voxelMapBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_VoxelMap;
            if (voxelMapBuilder == null)
            {
                voxelMapBuilder = new MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels.MyMwcObjectBuilder_VoxelMap(this.GetPosition(), MyMwcVoxelFilesEnum.Cube_512x512x512, MyMwcVoxelMaterialsEnum.Stone_01);
            }
            voxelMapBuilder.PositionAndOrientation.Position = PositionLeftBottomCorner;
            voxelMapBuilder.VoxelMaterial = VoxelMaterial;
            if (GetVoxelHandShapes().Count > 0)
            {
                if (!MyFakes.MWBUILDER)
                {
                    voxelMapBuilder.VoxelHandShapes = new List<MyMwcObjectBuilder_VoxelHand_Shape>();
                    foreach (MyVoxelHandShape voxelHandShape in GetVoxelHandShapes())
                    {

                        voxelMapBuilder.VoxelHandShapes.Add((MyMwcObjectBuilder_VoxelHand_Shape)voxelHandShape.GetObjectBuilder(getExactCopy));
                    }
                }
            }

            if (MyFakes.MWBUILDER)
            {
                SaveVoxelContents(out voxelMapBuilder.VoxelData, true);
            }

            return voxelMapBuilder;
        }

        //  This method must be called when this object dies or is removed
        //  E.g. it removes lights, sounds, etc
        public override void Close()
        {
            base.Close();

            //  Delete this voxel map from data cell cache
            MyMwcVector3Int dataCellCoord;
            for (dataCellCoord.X = 0; dataCellCoord.X < DataCellsCount.X; dataCellCoord.X++)
            {
                for (dataCellCoord.Y = 0; dataCellCoord.Y < DataCellsCount.Y; dataCellCoord.Y++)
                {
                    for (dataCellCoord.Z = 0; dataCellCoord.Z < DataCellsCount.Z; dataCellCoord.Z++)
                    {
                        MyVoxelCacheData.RemoveCell(VoxelMapId, ref dataCellCoord);
                        MyVoxelContentCell voxelCell = GetCell(ref dataCellCoord);
                        if (voxelCell != null) voxelCell.Deallocate();
                    }
                }
            }

            //  Delete this voxel map from render cell cache
            MyMwcVector3Int renderCellCoord;
            for (renderCellCoord.X = 0; renderCellCoord.X < RenderCellsCount.X; renderCellCoord.X++)
            {
                for (renderCellCoord.Y = 0; renderCellCoord.Y < RenderCellsCount.Y; renderCellCoord.Y++)
                {
                    for (renderCellCoord.Z = 0; renderCellCoord.Z < RenderCellsCount.Z; renderCellCoord.Z++)
                    {
                        MyVoxelCacheRender.RemoveCell(VoxelMapId, ref renderCellCoord, MyLodTypeEnum.LOD1);
                        MyVoxelCacheRender.RemoveCell(VoxelMapId, ref renderCellCoord, MyLodTypeEnum.LOD0);
                    }
                }
            }

            if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD && m_oreDepositCells != null)
            {
                /*
                MyMwcVector3Int oreDepositCellCoord;
                for (oreDepositCellCoord.X = 0; oreDepositCellCoord.X < m_oreDepositCells.GetLength(0); oreDepositCellCoord.X++)
                {
                    for (oreDepositCellCoord.Y = 0; oreDepositCellCoord.Y < m_oreDepositCells.GetLength(1); oreDepositCellCoord.Y++)
                    {
                        for (oreDepositCellCoord.Z = 0; oreDepositCellCoord.Z < m_oreDepositCells.GetLength(2); oreDepositCellCoord.Z++)
                        {
                            if (m_oreDepositCells[oreDepositCellCoord.X, oreDepositCellCoord.Y, oreDepositCellCoord.Z] != null)
                            {
                                RemoveOreDepositCell(ref oreDepositCellCoord, false);
                            }
                        }
                    }
                } */

                foreach (var ore in m_oreDepositCells)
                {
                    RemoveOreDepositCell(ore.Value, false);
                }
                m_oreDepositCells.Clear();
                
                using (OreDepositsLock.AcquireExclusiveUsing())
                {
                    OreDepositCellsContainsOre.Clear();
                }
            }

            lock (m_octreeOverlapElementListCollection)
            {
                foreach (var item in m_octreeOverlapElementListCollection)
                {
                    item.Clear();
                }
            }
            lock (m_sweepResultCollection)
            {
                foreach (var item in m_sweepResultCollection)
                {
                    item.Clear();
                }
            }


            MyVoxelMaps.RemoveVoxelMap(this);

            m_isClosed = true;
        }

        /// <summary>
        /// Applies voxel hand shape to this voxel map
        /// </summary>
        /// <param name="voxelHandShape"></param>
        public void AddVoxelHandShape(MyVoxelHandShape voxelHandShape, bool playSounds)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("AddVoxelHandShape");

            if (MyVoxelMaps.GetVoxelShapesCount() + 1 <= MyVoxelConstants.MAX_VOXEL_HAND_SHAPES_COUNT)
            {
                bool changed = false;
                MyMwcVoxelHandModeTypeEnum voxelHandModeType = voxelHandShape.ModeType;
                // sphere
                if (voxelHandShape is MyVoxelHandSphere)
                {
                    MyVoxelHandShape voxelHandSphere = (MyVoxelHandShape)voxelHandShape;
                    BoundingSphere bSphere = voxelHandSphere.WorldVolume;

                    if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SUBTRACT)
                    {
                        MyVoxelGenerator.CutOutSphereInvalidateCache(this, bSphere, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandRemove);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.ADD)
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("CreateSphereInvalidateCache");
                        MyVoxelGenerator.CreateSphereInvalidateCache(this, bSphere, ref changed, voxelHandShape.Material);
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartNextBlock("MyAudio.AddCue2D");
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandAdd);
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SET_MATERIAL)
                    {
                        SetVoxelMaterialInvalidateCache(voxelHandShape.Material.Value, bSphere, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandMaterial);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SOFTEN)
                    {
                        SoftenVoxelContentInSphereInvalidateCache(bSphere, MyVoxelConstants.DEFAULT_SOFTEN_WEIGHT, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSoften);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.WRINKLE)
                    {
                        WrinkleVoxelContentInSphereInvalidateCache(bSphere, MyVoxelConstants.DEFAULT_WRINKLE_WEIGHT_ADD, MyVoxelConstants.DEFAULT_WRINKLE_WEIGHT_REMOVE, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSoften);
                    }
                }
                // box
                else if (voxelHandShape is MyVoxelHandBox)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("VoxelHand MyVoxelHandBox");

                    MyVoxelHandBox voxelHandBox = (MyVoxelHandBox)voxelHandShape;
                    //BoundingBox bBox = voxelHandBox.GetLocalBoundingBox();
                    //Matrix world = Matrix.CreateWorld(voxelHandBox.GetPosition(), this.WorldMatrix.Forward, this.WorldMatrix.Up);
                    Matrix world = voxelHandBox.WorldMatrix;
                    //bBox = bBox.Transform(world);

                    //MyModel model = MyModels.GetModelForDraw(MyModelsEnum.ExplosionDebrisVoxel);
                    //MyModel model = MyMinerGameDX.Static.Content.Load<MyModel>("Box");
                    List<Vector3> vertexes = new List<Vector3>();
                    List<MyTriangleVertexIndices> triangles = new List<MyTriangleVertexIndices>();
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GenerateBoxVerticesAndTriangles");
                    GenerateBoxVerticesAndTriangles(ref vertexes, ref triangles, voxelHandBox.Size, voxelHandBox.Size);
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SUBTRACT)
                    {
                        //MyVoxelGenerator.CutOutBoxInvalidateCache(this, bBox, ref originalContents);
                        //MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.RemoveVoxels, world, voxelHandShape.Material, ref changed);

                        MyOrientedBoundingBox box = MyOrientedBoundingBox.Create(voxelHandBox.GetLocalBoundingBox(), voxelHandBox.WorldMatrix);
                        MyVoxelGenerator.CutOutOrientedBox(this, box, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandRemove);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.ADD)
                    {
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyMwcVoxelHandModeTypeEnum.ADD");
                        
                        //MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.AddVoxels, world, voxelHandShape.Material, ref changed);
                        
                        MyOrientedBoundingBox box = MyOrientedBoundingBox.Create(voxelHandBox.GetLocalBoundingBox(), voxelHandBox.WorldMatrix);
                        MyVoxelGenerator.CreateOrientedBox(this, box, voxelHandShape.Material);
                        changed = true;
                          
                        MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                        //MyVoxelGenerator.CreateBoxInvalidateCache(this, bBox, ref originalContents);
                        //if (voxelHandShape.Material != null)
                        //{
                        //    SetVoxelMaterialInvalidateCache(voxelHandShape.Material.Value, bBox, ref originalContents);
                        //}
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandAdd);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SET_MATERIAL)
                    {
                        //SetVoxelMaterialInvalidateCache(voxelHandShape.Material.Value, bBox, ref originalContents);
                        MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.ChangeMaterial, world, voxelHandShape.Material, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandMaterial);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SOFTEN)
                    {
                        //SoftenVoxelContentInBoxInvalidateCache(bBox, MyVoxelConstants.DEFAULT_SOFTEN_WEIGHT, ref originalContents);
                        MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.SoftenVoxels, world, voxelHandShape.Material, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSoften);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.WRINKLE)
                    {
                        //SoftenVoxelContentInBoxInvalidateCache(bBox, MyVoxelConstants.DEFAULT_WRINKLE_WEIGHT, ref originalContents);
                        MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.WrinkleVoxels, world, voxelHandShape.Material, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSoften);
                    }

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
                // cuboid
                else if (voxelHandShape is MyVoxelHandCuboid)
                {
                    MyVoxelHandCuboid voxelHandCuboid = (MyVoxelHandCuboid)voxelHandShape;
                    Matrix world = voxelHandCuboid.WorldMatrix;
                  
                    if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SUBTRACT)
                    {
                        MyVoxelGenerator.CutOutCuboid(this, voxelHandCuboid.Cuboid.CreateTransformed(ref world), voxelHandShape.Material);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandRemove);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.ADD)
                    {
                        MyVoxelGenerator.CreateCuboid(this, voxelHandCuboid.Cuboid.CreateTransformed(ref world), voxelHandShape.Material);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandAdd);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SET_MATERIAL)
                    {
                       // MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.ChangeMaterial, world, voxelHandShape.Material, ref changed);
                        //SetVoxelMaterialInvalidateCache(voxelHandShape.Material.Value, bBox, ref originalContents);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandMaterial);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SOFTEN)
                    {
                        //MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.SoftenVoxels, world, voxelHandShape.Material, ref changed);
                        //SoftenVoxelContentInBoxInvalidateCache(bBox, MyVoxelConstants.DEFAULT_SOFTEN_WEIGHT, ref originalContents);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSoften);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.WRINKLE)
                    {
                        //MyVoxelImport.Run(this, vertexes.ToArray(), triangles.ToArray(), MyvoxelImportAction.WrinkleVoxels, world, voxelHandShape.Material, ref changed);
                        //SoftenVoxelContentInBoxInvalidateCache(bBox, MyVoxelConstants.DEFAULT_WRINKLE_WEIGHT, ref originalContents);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSoften);
                    }
                }
                else if (voxelHandShape is MyVoxelHandCylinder)
                {
                    MyVoxelHandCylinder voxelHandCylinder = (MyVoxelHandCylinder)voxelHandShape;

                    MyOrientedBoundingBox box = MyOrientedBoundingBox.Create(voxelHandCylinder.GetLocalBoundingBox(), voxelHandCylinder.WorldMatrix);

                    if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.SUBTRACT)
                    {
                        MyVoxelGenerator.CutOutCylinder(this, voxelHandCylinder.Radius1, voxelHandCylinder.Radius2, box, voxelHandShape.Material, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandRemove);
                    }
                    else if (voxelHandModeType == MyMwcVoxelHandModeTypeEnum.ADD)
                    {
                        MyVoxelGenerator.CreateCylinder(this, voxelHandCylinder.Radius1, voxelHandCylinder.Radius2, box, voxelHandShape.Material, ref changed);
                        if (playSounds)
                            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandAdd);
                    }
                }
                else
                    System.Diagnostics.Debug.Assert(false);

                // adds only if there are any changes
                if (changed == true)
                {
                    this.AddChild(voxelHandShape, true);

                    m_voxelHandShapes.Add(voxelHandShape);
                    if (OnVoxelHandShapeCountChange != null)
                    {
                        OnVoxelHandShapeCountChange(1);
                    }

                    CalcAverageDataCellMaterials();
                }
            }

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        private static void GenerateBoxVerticesAndTriangles(ref List<Vector3> vertexes, ref List<MyTriangleVertexIndices> triangles, float size, float length)
        {
            if (vertexes == null)
            {
                vertexes = new List<Vector3>();
            }
            if (triangles == null)
            {
                triangles = new List<MyTriangleVertexIndices>();
            }

            float x = length / 2f;
            float y = size / 2f;
            float z = size / 2f;

            // left side vertexes 
            vertexes.Add(new Vector3(-x, y, z));
            vertexes.Add(new Vector3(-x, y, -z));
            vertexes.Add(new Vector3(-x, -y, -z));
            vertexes.Add(new Vector3(-x, -y, z));

            // right side vertexes            
            vertexes.Add(new Vector3(x, y, z));
            vertexes.Add(new Vector3(x, y, -z));
            vertexes.Add(new Vector3(x, -y, -z));
            vertexes.Add(new Vector3(x, -y, z));

            // left triangles
            triangles.Add(new MyTriangleVertexIndices(0, 1, 2));
            triangles.Add(new MyTriangleVertexIndices(0, 2, 3));

            // right triangles
            triangles.Add(new MyTriangleVertexIndices(4, 5, 6));
            triangles.Add(new MyTriangleVertexIndices(4, 6, 7));

            // front triangles
            triangles.Add(new MyTriangleVertexIndices(0, 4, 7));
            triangles.Add(new MyTriangleVertexIndices(0, 7, 3));

            // back triangles
            triangles.Add(new MyTriangleVertexIndices(1, 5, 6));
            triangles.Add(new MyTriangleVertexIndices(1, 6, 2));

            // top triangles
            triangles.Add(new MyTriangleVertexIndices(3, 7, 6));
            triangles.Add(new MyTriangleVertexIndices(3, 6, 2));

            // bottom triangles
            triangles.Add(new MyTriangleVertexIndices(0, 4, 5));
            triangles.Add(new MyTriangleVertexIndices(0, 5, 1));
        }

        /// <summary>
        /// Invalidates voxel cache
        /// </summary>
        /// <param name="minChanged"></param>
        /// <param name="maxChanged"></param>
        public void InvalidateCache(MyMwcVector3Int minChanged, MyMwcVector3Int maxChanged)
        {
            MyVoxelCacheData.RemoveCellForVoxels(this, minChanged, maxChanged);
            MyVoxelCacheRender.RemoveCellForVoxels(this, minChanged, maxChanged);

            //Cannot be here because of peaks
            //InvalidateRenderObjects(true);
        }

        public List<MyVoxelHandShape> GetVoxelHandShapes()
        {
            return m_voxelHandShapes;
        }

        // moved to MyVoxelMaps, because max shapes count is now for all voxel maps
        // This method returns count of remaining voxel hand shapes, that can be applied to to this voxel map
        //public int GetRemainingVoxelHandShapes()
        //{
        //    return MyVoxelConstants.MAX_VOXEL_HAND_SHAPES_COUNT - m_voxelHandShapes.Count;
        //}

        public int GetCountOfVoxelHandShapes()
        {
            return m_voxelHandShapes.Count;
        }

        private MyMwcVoxelFilesEnum m_voxelFile;
        public MyMwcVoxelFilesEnum GetVoxelFile()
        {
            return ((MyMwcObjectBuilder_VoxelMap)GetObjectBuilderInternal(false)).VoxelFile;
        }

        public bool IsCorrectVoxelCoords(ref MyMwcVector3Int voxelCoord)
        {
            return CheckVoxelCoord(ref voxelCoord);
        }

        private MyVoxelMapOreDepositCell AddOreDepositCell(ref MyMwcVector3Int oreDepositCellCoord)
        {
            //  Adding or creating cell can be made only once
            Int64 key = MyVoxelMaps.GetCellHashCode(VoxelMapId, ref oreDepositCellCoord, MyLodTypeEnum.LOD0);
            Debug.Assert(!m_oreDepositCells.ContainsKey(key));

            MyVoxelMapOreDepositCell ret = new MyVoxelMapOreDepositCell(this, oreDepositCellCoord);
            ret.OnVoxelMapOreDepositCellContainsOreChanged += OnMyVoxelMapOreDepositCellContainsOreChanged;
            m_oreDepositCells[key] = ret;

            RecalculateOreDepositCellBoundingBox(ref ret, ref key);

            return ret;
        }


        private void RemoveOreDepositCell(MyVoxelMapOreDepositCell oreDepositCell, bool lockEntityCloseLock = true)
        {
            oreDepositCell.OnVoxelMapOreDepositCellContainsOreChanged -= OnMyVoxelMapOreDepositCellContainsOreChanged;
            oreDepositCell.Close(lockEntityCloseLock);
        }
                                                   
        private void RemoveOreDepositCell(ref MyMwcVector3Int oreDepositCellCoord, bool lockEntityCloseLock = true)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(VoxelMapId, ref oreDepositCellCoord, MyLodTypeEnum.LOD0);
            MyVoxelMapOreDepositCell oreDepositCell = m_oreDepositCells[key];

            RemoveOreDepositCell(oreDepositCell, lockEntityCloseLock);

            m_oreDepositCells.Remove(key);
        }                                            

        private void ChangeOreDepositCellContent(byte oldContent, byte newContent, ref MyMwcVector3Int voxelCoord)
        {
            // if contents are same, then do nothing
            if (oldContent == newContent)
            {
                return;
            }

            MyMwcVector3Int cellCoord = GetDataCellCoordinate(ref voxelCoord);
            MyMwcVector3Int voxelCoordInCell = GetVoxelCoordinatesInDataCell(ref voxelCoord);
            MyMwcVoxelMaterialsEnum material = m_voxelMaterialCells[cellCoord.X][cellCoord.Y][cellCoord.Z].GetMaterial(ref voxelCoordInCell);

            // we change content only if material is ore
            if (MyVoxelMapOreMaterials.IsRareOre(material))
            {
                MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoord(ref voxelCoord);
                MyVoxelMapOreDepositCell oreDepositCell = GetOreDepositCell(ref oreDepositCellCoord);
                if (oreDepositCell == null)
                {
                    oreDepositCell = AddOreDepositCell(ref oreDepositCellCoord);
                }
                int content = newContent - oldContent;
                oreDepositCell.SetOreContent(material, content);

                if (oreDepositCell.GetOreWithContent().Count == 0)
                {
                    RemoveOreDepositCell(oreDepositCell);
                    Int64 key = MyVoxelMaps.GetCellHashCode(VoxelMapId, ref oreDepositCellCoord, MyLodTypeEnum.LOD0);
                    m_oreDepositCells.Remove(key);
                }
            }
        }

        private void ChangeOreDepositMaterial(MyMwcVoxelMaterialsEnum oldMaterial, MyMwcVoxelMaterialsEnum newMaterial, ref MyMwcVector3Int voxelCoord)
        {
            int content = GetVoxelContent(ref voxelCoord);

            // if there is no content in voxel, then do nothing
            if (content == MyVoxelConstants.VOXEL_CONTENT_EMPTY)
            {
                return;
            }

            MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoord(ref voxelCoord);
            MyVoxelMapOreDepositCell oreDepositCell = GetOreDepositCell(ref oreDepositCellCoord);

            // if new material is ore, then we must add it to ore deposit cell
            if (MyVoxelMapOreMaterials.IsRareOre(newMaterial))
            {
                if (oreDepositCell == null) 
                    oreDepositCell = AddOreDepositCell(ref oreDepositCellCoord);

                oreDepositCell.SetOreContent(newMaterial, content);
            }

            // if old material is ore, then we must remove it from ore deposit cell
            if (MyVoxelMapOreMaterials.IsRareOre(oldMaterial))
            {
                //MyCommonDebugUtils.AssertDebug(oreDepositCell != null, "Ore deposit wasn't initialized!");  // may not happen
                //// could happen!!!
                //if (oreDepositCell == null) oreDepositCell = AddOreDepositCell(ref oreDepositCellCoord);

                if (oreDepositCell != null)
                {
                    oreDepositCell.SetOreContent(oldMaterial, -content);

                    if (oreDepositCell.GetOreWithContent().Count <= 0) 
                        RemoveOreDepositCell(ref oreDepositCellCoord);
                }
            }
        }

        // Fast, no locking: should be called only during loading
        private void ChangeOreDepositMaterialFast(MyMwcVoxelMaterialsEnum oldMaterial, MyMwcVoxelMaterialsEnum newMaterial, ref MyMwcVector3Int cellCoord, ref MyMwcVector3Int coordInCell, ref MyMwcVector3Int oreDepositCellCoord)
        {
            int content;

            MyVoxelContentCell voxelCell = GetCell(ref cellCoord);
            if (voxelCell == null)
                content = MyVoxelConstants.VOXEL_CONTENT_FULL;
            else
                content = voxelCell.GetVoxelContent(ref coordInCell);

            // if there is no content in voxel, then do nothing
            if (content == MyVoxelConstants.VOXEL_CONTENT_EMPTY)
            {
                return;
            }

            MyVoxelMapOreDepositCell oreDepositCell = GetOreDepositCell(ref oreDepositCellCoord);

            // if new material is ore, then we must add it to ore deposit cell
            if (MyVoxelMapOreMaterials.IsRareOre(newMaterial))
            {
                if (oreDepositCell == null) oreDepositCell = AddOreDepositCell(ref oreDepositCellCoord);
                oreDepositCell.SetOreContent(newMaterial, content);
            }

            // if old material is ore, then we must remove it from ore deposit cell
            if (MyVoxelMapOreMaterials.IsRareOre(oldMaterial))
            {
                MyCommonDebugUtils.AssertDebug(oreDepositCell != null, "Ore deposit wasn't initialized!");  // may not happen
                //if (oreDepositCell == null) oreDepositCell = AddOreDepositCell(ref oreDepositCellCoord);  

                oreDepositCell.SetOreContent(oldMaterial, -content);
                if (oreDepositCell.GetOreWithContent().Count == 0) RemoveOreDepositCell(ref oreDepositCellCoord);
            }
        }

        private MyMwcVector3Int GetOreDepositCellCoord(ref MyMwcVector3Int voxelCoord)
        {
            MyMwcVector3Int dataCellCoord = GetDataCellCoordinate(ref voxelCoord);
            return GetOreDepositCellCoordByDataCellCoord(ref dataCellCoord);
        }

        private MyMwcVector3Int GetOreDepositCellCoordByDataCellCoord(ref MyMwcVector3Int dataCellCoord)
        {
            return new MyMwcVector3Int(
                    dataCellCoord.X / MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS,
                    dataCellCoord.Y / MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS,
                    dataCellCoord.Z / MyVoxelConstants.VOXEL_MAP_ORE_DEPOSIT_CELL_IN_DATA_CELLS
                );
        }

        private MyVoxelMapOreDepositCell GetOreDepositCell(ref MyMwcVector3Int oreDepositCellCoord)
        {
            Int64 key = MyVoxelMaps.GetCellHashCode(VoxelMapId, ref oreDepositCellCoord, MyLodTypeEnum.LOD0);

            MyVoxelMapOreDepositCell cell;
            m_oreDepositCells.TryGetValue(key, out cell);
            return cell;
        }

        private MyVoxelMapOreDepositCell GetOreDepositCellByVoxelCoord(ref MyMwcVector3Int voxelCoord)
        {
            MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoord(ref voxelCoord);
            return GetOreDepositCell(ref oreDepositCellCoord);
        }

        private MyVoxelMapOreDepositCell GetOreDepositCellByDataCellCoord(ref MyMwcVector3Int dataCellCoord)
        {
            MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoordByDataCellCoord(ref dataCellCoord);
            return GetOreDepositCell(ref oreDepositCellCoord);
        }

        private void RecalculateOreDepositCellBoundingBoxes()
        {
            if (m_oreDepositCells == null)
                return;

            /*
            MyMwcVector3Int oreDepositCellCoord;
            for (oreDepositCellCoord.X = 0; oreDepositCellCoord.X < m_oreDepositCells.GetLength(0); oreDepositCellCoord.X++)
            {
                for (oreDepositCellCoord.Y = 0; oreDepositCellCoord.Y < m_oreDepositCells.GetLength(1); oreDepositCellCoord.Y++)
                {
                    for (oreDepositCellCoord.Z = 0; oreDepositCellCoord.Z < m_oreDepositCells.GetLength(2); oreDepositCellCoord.Z++)
                    {
                        MyVoxelMapOreDepositCell oreDepositCell = GetOreDepositCell(ref oreDepositCellCoord);
                        if (oreDepositCell != null)
                        {
                            RecalculateOreDepositCellBoundingBox(ref oreDepositCell, ref oreDepositCellCoord);
                        }
                    }
                }
            } */

            foreach (var oreCell in m_oreDepositCells)
            {
                MyVoxelMapOreDepositCell oreDepositCell = oreCell.Value;
                Int64 oreDepositCellCoord = oreCell.Key;
                RecalculateOreDepositCellBoundingBox(ref oreDepositCell, ref oreDepositCellCoord);
            }
        }

        private void RecalculateOreDepositCellBoundingBox(ref MyVoxelMapOreDepositCell oreDepositCell, ref Int64 oreDepositCellCoord)
        {
            Vector3 minPosition;
            Vector3 maxPosition;
            GetMinAndMaxPositionOfOreDepositCell(ref oreDepositCellCoord, out minPosition, out maxPosition);
            //oreDepositCell.BoundingBox = new BoundingBox(minPosition, maxPosition);
            oreDepositCell.WorldAABB = new BoundingBox(minPosition, maxPosition);
        }

        private void GetMinAndMaxPositionOfOreDepositCell(ref Int64 oreDepositCellCoord, out Vector3 minPosition, out Vector3 maxPosition)
        {
            Vector3 oreDepositCellSize = new Vector3(SizeInMetres.X / m_oreDepositCellsCountX,
                                                     SizeInMetres.Y / m_oreDepositCellsCountY,
                                                     SizeInMetres.Z / m_oreDepositCellsCountZ);

            MyVoxelMapOreDepositCell oreCell = m_oreDepositCells[oreDepositCellCoord];

            minPosition = PositionLeftBottomCorner + new Vector3(oreCell.Coord.X * oreDepositCellSize.X, oreCell.Coord.Y * oreDepositCellSize.Y, oreCell.Coord.Z * oreDepositCellSize.Z);
            maxPosition = minPosition + oreDepositCellSize;
        }

        private void OnMyVoxelMapOreDepositCellContainsOreChanged(MyVoxelMapOreDepositCell oreDepositCell, bool isEmpty)
        {
            using (OreDepositsLock.AcquireExclusiveUsing())
            {
                if (isEmpty)
                {
                    OreDepositCellsContainsOre.Remove(oreDepositCell);
                }
                else
                {
                    OreDepositCellsContainsOre.Add(oreDepositCell);
                }
            }
        }

        public override string GetFriendlyName()
        {
            return "MyVoxelMap";
        }


        public override MyMwcVoxelMaterialsEnum VoxelMaterial
        {
            set
            {
                if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                {
                    foreach (var oreDeposit in OreDepositCellsContainsOre)
                    {
                        MyMwcVector3Int coord = oreDeposit.Coord;
                        RemoveOreDepositCell(ref coord);
                    }
                    using (OreDepositsLock.AcquireExclusiveUsing())
                    {
                        OreDepositCellsContainsOre.Clear();
                    }
                }

                byte defaultIndestructibleContents = GetIndestructibleContentByMaterial(value);

                for (int x = 0; x < DataCellsCount.X; x++)
                {
                    for (int y = 0; y < DataCellsCount.Y; y++)
                    {
                        for (int z = 0; z < DataCellsCount.Z; z++)
                        {
                            m_voxelMaterialCells[x][y][z].Reset(value, defaultIndestructibleContents);
                            m_voxelMaterialCells[x][y][z].CalcAverageCellMaterial();

                            if (MyFakes.ENABLE_DISPLAYING_ORE_ON_HUD)
                            {
                                // we need update ore deposit values
                                MyMwcVector3Int dataCellCoord = new MyMwcVector3Int(x, y, z);
                                MyMwcVector3Int oreDepositCellCoord = GetOreDepositCellCoordByDataCellCoord(ref dataCellCoord);
                                MyVoxelMapOreDepositCell oreDepositCell = GetOreDepositCell(ref oreDepositCellCoord);
                                if (MyVoxelMapOreMaterials.IsRareOre(value))
                                {
                                    if (oreDepositCell == null)
                                    {
                                        oreDepositCell = AddOreDepositCell(ref oreDepositCellCoord);
                                    }
                                    int content;
                                    MyVoxelContentCell contentCell = GetCell(new MyMwcVector3Int(x, y, z));
                                    if (contentCell == null)
                                    {
                                        content = MyVoxelConstants.VOXEL_CELL_CONTENT_SUM_TOTAL;
                                    }
                                    else
                                    {
                                        content = contentCell.GetVoxelContentSum();
                                    }
                                    oreDepositCell.SetOreContent(value, content);
                                }
                            }
                        }
                    }
                }

                base.VoxelMaterial = value;
                MyMwcVector3Int minCorner = new MyMwcVector3Int(0, 0, 0);
                MyMwcVector3Int maxCorner = Size;
                InvalidateCache(minCorner, maxCorner);

                if (m_voxelHandShapes != null)
                {
                    foreach (MyVoxelHandShape handShape in m_voxelHandShapes.ToArray())
                    {
                        if (handShape.ModeType == MyMwcVoxelHandModeTypeEnum.SET_MATERIAL)
                        {
                            m_voxelHandShapes.Remove(handShape);
                        }

                        if (handShape.ModeType == MyMwcVoxelHandModeTypeEnum.ADD)
                        {
                            handShape.Material = value;
                        }
                    }
                }
            }
        }

        public void AddExplosion(BoundingSphere explosion)
        {
            m_explosions.Add(explosion);
        }

        public override void BeforeSave()
        {
            base.BeforeSave();
            // we convert explosions to voxel hands
            foreach (BoundingSphere explosion in m_explosions)
            {
                MyVoxelHandSphere voxelHandSphere = new MyVoxelHandSphere();
                MyMwcObjectBuilder_VoxelHand_Sphere builder = new MyMwcObjectBuilder_VoxelHand_Sphere(new MyMwcPositionAndOrientation(explosion.Center, Vector3.Forward, Vector3.Up), explosion.Radius, MyMwcVoxelHandModeTypeEnum.SUBTRACT);
                voxelHandSphere.Init(builder, this);
                m_voxelHandShapes.Add(voxelHandSphere);
            }

            m_explosions.Clear();
        }

        public void ClearVoxelHands()
        {
            m_voxelHandShapes.Clear();

            MyMwcVector3Int voxelCoordMin = new MyMwcVector3Int(0, 0, 0);
            MyMwcVector3Int voxelCoordMax = Size;

            //  Fix min and max cell coordinates so they don't overlap the voxelmap
            FixVoxelCoord(ref voxelCoordMin);
            FixVoxelCoord(ref voxelCoordMax);
            this.LoadFile(this.GetPosition(), this.GetObjectBuilder(true) as MyMwcObjectBuilder_VoxelMap);
            this.Activate(false, false);
            this.Activate(true, false);
        }

        [ThreadStatic]
        Vector3[] m_helperFrustumCorners;

        Vector3[] HelperFrustumCorners
        {
            get
            {
                if (m_helperFrustumCorners == null)
                {
                    m_helperFrustumCorners = new Vector3[8];
                }
                return m_helperFrustumCorners;
            }
        }

        public override bool GetIntersectionWithBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            ContainmentType con = boundingFrustum.Contains(WorldAABB);
            if (con == ContainmentType.Contains)
            {
                return true;
            }

            if (con == ContainmentType.Intersects)
            {
                BoundingSphere sphere = boundingFrustum.ToBoundingSphere(HelperFrustumCorners);

                //  Get min and max cell coordinate where boundingBox can fit
                BoundingBox sphereBoundingBox = BoundingBoxHelper.InitialBox;
                BoundingBoxHelper.AddSphere(sphere, ref sphereBoundingBox);
                MyMwcVector3Int cellCoordMin = GetDataCellCoordinateFromMeters(ref sphereBoundingBox.Min);
                MyMwcVector3Int cellCoordMax = GetDataCellCoordinateFromMeters(ref sphereBoundingBox.Max);

                //  Fix min and max cell coordinates so they don't overlap the voxelmap
                FixDataCellCoord(ref cellCoordMin);
                FixDataCellCoord(ref cellCoordMax);

                MyMwcVector3Int cellCoord;
                for (cellCoord.X = cellCoordMin.X; cellCoord.X <= cellCoordMax.X; cellCoord.X++)
                {
                    for (cellCoord.Y = cellCoordMin.Y; cellCoord.Y <= cellCoordMax.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = cellCoordMin.Z; cellCoord.Z <= cellCoordMax.Z; cellCoord.Z++)
                        {
                            //  If no overlap between bounding pitch of data cell and the sphere
                            BoundingBox dataCellBoundingBox;
                            GetDataCellBoundingBox(ref cellCoord, out dataCellBoundingBox);
                            if (MyUtils.IsBoxIntersectingSphere(ref dataCellBoundingBox, ref sphere) == false) continue;

                            //  Get cell from cache. If not there, precalc it and store in the cache.
                            //  If null is returned, we know that cell doesn't contain any triangleVertexes so we don't need to do intersections.
                            MyVoxelCacheCellData cachedDataCell = null;
                            using (MyVoxelCacheData.Locker.AcquireSharedUsing())
                            {
                                cachedDataCell = MyVoxelCacheData.GetCell(this, ref cellCoord, true);

                                if (cachedDataCell == null) continue;

                                for (int i = 0; i < cachedDataCell.VoxelTrianglesCount; i++)
                                {
                                    MyVoxelTriangle voxelTriangle = cachedDataCell.VoxelTriangles[i];

                                    MyVoxelVertex voxelVertex0 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex0];
                                    MyVoxelVertex voxelVertex1 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex1];
                                    MyVoxelVertex voxelVertex2 = cachedDataCell.VoxelVertices[voxelTriangle.VertexIndex2];

                                    if (boundingFrustum.Contains(voxelVertex0.Position + PositionLeftBottomCorner) == ContainmentType.Contains)
                                        return true;
                                    if (boundingFrustum.Contains(voxelVertex1.Position + PositionLeftBottomCorner) == ContainmentType.Contains)
                                        return true;
                                    if (boundingFrustum.Contains(voxelVertex2.Position + PositionLeftBottomCorner) == ContainmentType.Contains)
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }


        protected override void InitRenderObjects()
        {
            m_renderObjects = new MyRenderObject[RenderCellsCount.X * RenderCellsCount.Y * RenderCellsCount.Z];

            int i = 0;
            MyMwcVector3Int cellCoord;
            for (cellCoord.X = 0; cellCoord.X < RenderCellsCount.X; cellCoord.X++)
            {
                for (cellCoord.Y = 0; cellCoord.Y < RenderCellsCount.Y; cellCoord.Y++)
                {
                    for (cellCoord.Z = 0; cellCoord.Z < RenderCellsCount.Z; cellCoord.Z++)
                    {
                        m_renderObjects[i++] = new MyRenderObject(this, cellCoord);
                    }
                }
            }
        }

        protected override void AddRenderObjects()
        {
            foreach (MyRenderObject renderObject in m_renderObjects)
            {
                MyRender.AddRenderObject(renderObject);
            }
        }

        protected override void RemoveRenderObjects()
        {
            foreach (MyRenderObject renderObject in m_renderObjects)
            {
                MyRender.RemoveRenderObject(renderObject);
            }
        }

        public void Explode()
        {
            var explosionEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_Bomb);
            explosionEffect.WorldMatrix = this.WorldMatrix;
            explosionEffect.UserScale = .05f * this.WorldVolume.Radius;
            this.MarkForClose();
        }
    }
}

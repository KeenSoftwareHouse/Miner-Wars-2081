using System;
using System.Collections.Generic;
using System.IO;
using MinerWarsMath;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.Networking;

namespace MinerWars.AppCode.Game.Voxels
{
    //  Used to sort voxel maps by distance between their center and camera
    class MyPhysObjectVoxelMapByDistanceComparer : IComparer<MyEntity>
    {
        public int Compare(MyEntity x, MyEntity y)
        {
            float xDist, yDist;

            Vector3 vxPos = x.GetPosition();
            Vector3 vyPos = y.GetPosition();

            Vector3 campos = MyCamera.Position;
            Vector3.DistanceSquared(ref vxPos, ref campos, out xDist);
            Vector3.DistanceSquared(ref vyPos, ref campos, out yDist);
            return xDist.CompareTo(yDist);
        }
    }

    //  Used to sort data cells by their distance to given line's starting point
    class MySortDataCellByDistanceComparer : IComparer<MyDataCellForSorting>
    {
        public int Compare(MyDataCellForSorting x, MyDataCellForSorting y)
        {
            return x.CellDistanceToLineFrom.CompareTo(y.CellDistanceToLineFrom);
        }
    }

    //  Used to sort render cell by distance between their center and camera
    class MyRenderCellByDistanceComparer : IComparer<MyRenderCellForSorting>
    {
        public int Compare(MyRenderCellForSorting x, MyRenderCellForSorting y)
        {
            float xDist, yDist;
            Vector3 campos = MyCamera.Position;
            Vector3.Distance(ref x.RenderCell.Center, ref campos, out xDist);
            Vector3.Distance(ref y.RenderCell.Center, ref campos, out yDist);
            return xDist.CompareTo(yDist);
        }
    }

    struct MyRenderCellForSorting
    {
        public MyVoxelCacheCellRender RenderCell;

        public MyRenderCellForSorting(MyVoxelCacheCellRender renderCell)
        {
            RenderCell = renderCell;
        }
    }

    struct MyDataCellForSorting
    {
        public MyMwcVector3Int DataCell;
        public float CellDistanceToLineFrom;

        public MyDataCellForSorting(MyMwcVector3Int dataCell, float cellDistanceToLineFrom)
        {
            DataCell = dataCell;
            CellDistanceToLineFrom = cellDistanceToLineFrom;
        }
    }

    class MyVoxelMaps
    {
        //  This array is used for holding potential triangles we need to test for intersections more closly
        // Size of this array is 2048 * 100B = 200KB
        public readonly static MyColDetVoxelTriangle[] PotentialColDetTriangles = new MyColDetVoxelTriangle[MyVoxelConstants.MAX_POTENTIAL_COLDET_TRIANGLES_COUNT];

        //  Used to calculate cell's hash code. Const with '_ASSERT' is here only for compile time checking if we can't overflow Int64 boundaries. But we can't.
        const Int64 MAX_VOXEL_CELLS_COUNT = MyVoxelConstants.MAX_VOXEL_MAP_SIZE_IN_VOXELS / MyVoxelConstants.VOXEL_DATA_CELL_SIZE_IN_VOXELS;
        const Int64 MAX_VOXEL_CELLS_COUNT_ASSERT = MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MyVoxelConstants.MAX_VOXEL_MAP_ID;

        //  List of voxel maps in this sector
        static List<MyVoxelMap> m_voxelMaps = new List<MyVoxelMap>();
        //  Sometimes we need to remove objects from "m_voxelMaps" while we iterate that list, so this is the helper
        static List<MyVoxelMap> m_voxelMapsSafeIterationHelper = new List<MyVoxelMap>();

        //  For sorting render cells by distance to camera
        static List<MyRenderCellForSorting> m_sortedRenderCells;
        public static readonly MyRenderCellByDistanceComparer SortedRenderCellsByDistanceComparer = new MyRenderCellByDistanceComparer();

        //  For sorting voxel data cell by distance to given line
        static List<MyDataCellForSorting> m_sortedDataCellList;
        public static readonly MySortDataCellByDistanceComparer SortedDataCellByDistanceToLineComparer = new MySortDataCellByDistanceComparer();

        //Spare some time in un/load
        public static bool AutoRecalculateVoxelMaps = true;


        //  For generating new and unique VoxelMapId
        static int m_voxelMapIdGenerator = 0;

        static int m_voxelShapesCount = 0;

        public static Action<MyVoxelMapOreDepositCell> OnRemoveOreCell;

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMaps.LoadData");

            MyMwcLog.WriteLine("MyVoxelMaps.LoadData() - START");
            MyMwcLog.IncreaseIndent();

            m_voxelMaps.Clear();
            m_voxelMapsSafeIterationHelper.Clear();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelMaps.LoadData() - END");

            for (int i = 0; i < MyVoxelConstants.MAX_POTENTIAL_COLDET_TRIANGLES_COUNT; i++)
            {
                PotentialColDetTriangles[i] = new MyColDetVoxelTriangle();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }


        //  Here we load only textures, effects, etc, no voxel-maps.
        public static void LoadContent()
        {

        }

        public static List<MyRenderCellForSorting> GetSortedRenderCells()
        {
            return m_sortedRenderCells;
        }

        public static void UnloadData()
        {
            RemoveAll();
            m_voxelMapIdGenerator = 0;
        }

        //  Prepare list of phys objects, so then we can do remove while iterating it
        static void PrepareVoxelMapsSafeIterationHelper()
        {
            m_voxelMapsSafeIterationHelper.Clear();
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                m_voxelMapsSafeIterationHelper.Add(voxelMap);
            }
        }

        //  Allows you to iterate through all voxel maps
        public static List<MyVoxelMap> GetVoxelMaps()
        {
            return m_voxelMaps;
        }

        public static void RecalcVoxelMaps()
        {
            MyMwcLog.WriteLine("MyVoxelMaps.RecalcVoxelMaps - START");

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Add all voxel maps into shadow maps; And calculates average data cell materials
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // THIS IS DONE DURING LOAD
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMaps::CalcAverageDataCellMaterials");

            //foreach (MyVoxelMap voxelmap in m_voxelMaps)
            //{
            //    voxelmap.CalcAverageDataCellMaterials();
            //}

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Get voxel map with highest count of render cells, and then use this number to preallocate sorting list
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("sortedRenderCells");

            int maxRenderCellsCount = 0;
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                int count = voxelMap.RenderCellsCount.X * voxelMap.RenderCellsCount.Y * voxelMap.RenderCellsCount.Z;
                if (count > maxRenderCellsCount) 
                    maxRenderCellsCount = count;                
            }
            m_sortedRenderCells = new List<MyRenderCellForSorting>(maxRenderCellsCount);

            m_sortedDataCellList = new List<MyDataCellForSorting>(maxRenderCellsCount);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Precalculate all data cells (we get triangles from voxels). In this step we don't get render cells, but that will be fast.
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelMaps::PrepareRenderCellCache");

            PrepareRenderCellCache();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.WriteLine("MyVoxelMaps.RecalcVoxelMaps - END");
        }

        public static int GetUniqueVoxelMapId()
        {
            return m_voxelMapIdGenerator++;
        }

        public static int GetTotalDataCellsCount()
        {
            int totalCellsCount = 0;
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                totalCellsCount += voxelMap.DataCellsCount.X * voxelMap.DataCellsCount.Y * voxelMap.DataCellsCount.Z;
            }
            return totalCellsCount;
        }

        public static void ClearVoxelMapMaterials(MyVoxelMap voxelMap)
        {
            //TODO
        }

        public static bool Exist(MyVoxelMap voxelMap)
        {
            return m_voxelMaps.Contains(voxelMap);
        }

        public static void RemoveVoxelMap(MyVoxelMap voxelMap)
        {
            if (m_voxelMaps.Remove(voxelMap))
            {
                m_voxelShapesCount -= voxelMap.GetVoxelHandShapes().Count;
                voxelMap.OnVoxelHandShapeCountChange -= OnVoxelHandShapeCountChange;
                m_voxelMaps.Remove(voxelMap);
                if (AutoRecalculateVoxelMaps)
                    RecalcVoxelMaps();
            }
        }

        public static void RemoveAll()
        {
            PrepareVoxelMapsSafeIterationHelper();

            foreach (MyVoxelMap voxelMap in m_voxelMapsSafeIterationHelper)
            {
                // this prevents from recalculating voxel maps after every single voxel map removed
                voxelMap.OnVoxelHandShapeCountChange -= OnVoxelHandShapeCountChange;
                m_voxelMaps.Remove(voxelMap);
                voxelMap.Close();
            }

            m_voxelShapesCount = 0;
            RecalcVoxelMaps();
        }

        private static void OnVoxelHandShapeCountChange(int count)
        {
            m_voxelShapesCount += count;
        }

        //  I am using method for calculating voxel cell hash code instead of struct for storing key.
        //  I am trying to generate hash code scaled in larger space. Minimal difference between too closest cells is MAX_VOXEL_CELLS_COUNT.
        //  Resons:
        //      - Int is faster than struct storing four ints
        //      - Struct was for some unknown reasons making garbage. Every call to TryGetValue()
        //        was allocating something. Probably because of boxing when calculating GetHashCode(0
        //        or doing Equals(), but I am not sure.
        public static Int64 GetCellHashCode(int voxelMapId, ref MyMwcVector3Int cellCoord, MyLodTypeEnum cellHashType)
        {
            return
                voxelMapId * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT +
                cellCoord.X * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT +
                cellCoord.Y * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT +
                cellCoord.Z * MAX_VOXEL_CELLS_COUNT * MAX_VOXEL_CELLS_COUNT +
                (int)cellHashType * MAX_VOXEL_CELLS_COUNT;
        }

        //  Scans all voxel maps along the line. Return true if any non-empty voxel found.
        //  Computes absulute space coordinate of collision point and returns reference to voxel map.
        public static bool ScanLine3D(Vector3 lineStart, Vector3 lineEnd, out Vector3? intersection, out MyVoxelMap outVoxelMap)
        {
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                if (voxelMap.ScanLine3D(lineStart, lineEnd, out intersection) == true)
                {
                    outVoxelMap = voxelMap;
                    return true;
                }
            }

            //  We didn't find any non-empty voxel along the line
            intersection = null;
            outVoxelMap = null;
            return false;
        }

        public static MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(ref MyLine line)
        {
            MyIntersectionResultLineTriangleEx? result = null;

            //  Check all voxel maps
            for (int value = 0; value < m_voxelMaps.Count; value++)
            {
                MyIntersectionResultLineTriangleEx? testResultEx;
                m_voxelMaps[value].GetIntersectionWithLine(ref line, out testResultEx);

                //  If intersection occured and distance to intersection is closer to origin than any previous intersection)
                result = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref result, ref testResultEx);
            }

            return result;
        }

        
          
        //  
        // 
        public static MyVoxelMap GetOverlappingWithSphere(ref BoundingSphere sphere)
        {
            for (int i = 0; i < m_voxelMaps.Count; i++)
            {
                MyVoxelMap voxelMap = m_voxelMaps[i];
                if (voxelMap.DoOverlapSphereTest(sphere.Radius, sphere.Center))
                {
                    return voxelMap;
                }
            }

            //  No intersection found
            return null;
        }

        //  Return reference to a voxel map that intersects with the specified sphere. If not intersection, null is returned.
        //  We don't look for closest intersection - so we stop on first intersection found.
        //  Params:
        //      sphere - sphere we want to test for intersection
        public static MyVoxelMap GetIntersectionWithSphere(ref BoundingSphere sphere, MyVoxelMap selected)
        {
            for (int i = 0; i < m_voxelMaps.Count; i++) if (selected == null || m_voxelMaps[i] == selected)
            {
                MyVoxelMap voxelMap = m_voxelMaps[i];
                if (voxelMap.IsSphereIntersectingBoundingBoxOfThisVoxelMap(ref sphere))
                {
                    return voxelMap;
                }
            }

            //  No intersection found
            return null;
        }

        //  Return reference to a voxel map that intersects with the specified box. If not intersection, null is returned.
        //  We don't look for closest intersection - so we stop on first intersection found.
        //  Params:
        //      localBoundingBox - local bounding box, we transform it to VoxelMap orientation
        //      boundingBoxWorldPosition - position of bounding box in world coordinates
        public static MyVoxelMap GetIntersectionWithBox(ref BoundingBox localBoundingBox, ref Vector3 boundingBoxWorldPosition, MyVoxelMap selected)
        {
            for (int i = 0; i < m_voxelMaps.Count; i++) if (selected == null || m_voxelMaps[i] == selected)
            {
                MyVoxelMap voxelMap = m_voxelMaps[i];
                Matrix world = Matrix.CreateWorld(boundingBoxWorldPosition, voxelMap.WorldMatrix.Forward, voxelMap.WorldMatrix.Up);
                BoundingBox worldBoundingBox = localBoundingBox.Transform(world);
                if (voxelMap.IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref worldBoundingBox))
                {
                    return voxelMap;
                }
            }

            //  No intersection found
            return null;
        }

        //  Method fills preallocated array with voxel triangles that potentialy intersects bounding box. Later, JLX will do intersection testing on these triangles.
        //  Input:
        //      boundingBox - bounding box that can intersect with voxel maps
        //  Output:
        //      potentialTriangles - potential voxel triangles
        //      numTriangles - count of potential voxel triangles
        public static void GetPotentialTrianglesForColDet(out int numTriangles, ref BoundingBox boundingBox)
        {
            numTriangles = 0;

            //  Check all voxel maps
            for (int i = 0; i < m_voxelMaps.Count; i++)
            {
                m_voxelMaps[i].GetPotentialTrianglesForColDet(ref numTriangles, ref boundingBox);
            }
        }        

        //  Return reference to voxel map that intersects the box. If not voxel map found, null is returned.
        public static MyVoxelMap GetVoxelMapWhoseBoundingBoxIntersectsBox(ref BoundingBox boundingBox)
        {
            for (int i = 0; i < m_voxelMaps.Count; i++)
            {
                MyVoxelMap voxelMap = m_voxelMaps[i];
                if (voxelMap.IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref boundingBox) == true) return voxelMap;
            }

            //  If we get here, no intersection was found
            return null;
        }

        //  Return reference to voxel map that intersects the box. If not voxel map found, null is returned.
        public static MyVoxelMap GetVoxelMapWhoseBoundingBoxIntersectsBox(ref BoundingBox boundingBox, MyVoxelMap ignoreVoxelMap)
        {
            for (int i = 0; i < m_voxelMaps.Count; i++)
            {
                MyVoxelMap voxelMap = m_voxelMaps[i];
                if (voxelMap != ignoreVoxelMap)
                {
                    if (voxelMap.IsBoxIntersectingBoundingBoxOfThisVoxelMap(ref boundingBox) == true) return voxelMap;
                }
            }

            //  If we get here, no intersection was found
            return null;
        }
        
        public static bool GetListOfVoxelMapsWhoseBoundingSphereIntersectsSphere(ref BoundingSphere boundingSphere, IList<MyVoxelMap> listOfVoxelMaps, MyEntity ignorePhysObject)
        {
            listOfVoxelMaps.Clear();
            /*
            //  Get collision elements near the line's bounding box (use sweep-and-prune, so we iterate only close objects)
            BoundingBox boundingBox = BoundingBoxHelper.InitialBox;
            BoundingBoxHelper.AddSphere(ref boundingSphere, ref boundingBox);

            var elements = MyEntities.GetElementsInBox(ref boundingBox);
            foreach (MinerWars.AppCode.Physics.MyRBElement element in elements)
            {
                MyEntity physicObject = ((MinerWars.AppCode.Game.Physics.MyPhysicsBody)element.GetRigidBody().m_UserData).Entity;

                MyVoxelMap map = physicObject as MyVoxelMap;
                if (map != null)
                    listOfVoxelMaps.Add(map);
            }
            elements.Clear();
            */
            //MyEntities.GetIntersectionWithSphere(ref boundingSphere, ignorePhysObject, null, false, false, ref list);

            
            for (int i = 0; i < m_voxelMaps.Count; i++)
            {
                MyVoxelMap voxelMapForCollisionTest = m_voxelMaps[i];
                if (voxelMapForCollisionTest != ignorePhysObject)
                {
                    if (voxelMapForCollisionTest.IsSphereIntersectingBoundingSphereOfThisVoxelMap(ref boundingSphere) == true)
                        listOfVoxelMaps.Add(voxelMapForCollisionTest);
                }
            } 
            if (listOfVoxelMaps.Count > 0)
                return true;
            //  If we get here, no intersection was found
            return false;
        }

        public static bool IsCollidingWithVoxelMap(MyMwcVoxelFilesEnum voxelFileEnum, Vector3 voxelPosition)
        {
            MyVoxelFile voxelFile = MyVoxelFiles.Get(voxelFileEnum);
            Vector3 sizeInMeters = MyVoxelMap.GetVoxelSizeInMetres(ref voxelFile.SizeInVoxels);

            BoundingBox newBoundingBox = MyUtils.GetNewBoundingBox(voxelPosition, sizeInMeters);
            MyVoxelMap intersectingVoxelMap = MyVoxelMaps.GetVoxelMapWhoseBoundingBoxIntersectsBox(ref newBoundingBox, null);

            if (intersectingVoxelMap != null)
            {
                return true;
            }
            return false;
        }

        //  Calculates reverb value for our sound engine. For interval, see MySounds.ReverbControl.
        //  This value is calculated by finding how many mixed or full voxel cells are around the camera (it is similar to occlusion lighting). If no, reverb is 0. If many, reverb is 100.
        public static float GetReverb(Vector3 cameraPosition)
        {
            for (int i = 0; i < m_voxelMaps.Count; i++)
            {
                float? reverb = m_voxelMaps[i].GetReverb(cameraPosition);
                if (reverb.HasValue == true)
                {
                    return reverb.Value;
                }
            }

            //  If we get here, camera isn't in any voxel map, so reverb must be zero
            return 0;
        }

        //  Create vertex buffers and index buffers and fill them with voxel render cells
        //  IMPORTANT: Don't call from background thread or from LoadContent. Only from Draw call
        public static void PrepareRenderCellCache()
        {
            MyMwcLog.WriteLine("MyVoxelMaps.PrepareRenderCellCache - START");
            MyMwcLog.IncreaseIndent();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("BoundingSphere");

            BoundingSphere sphere = new BoundingSphere(MyCamera.BoundingSphere.Center, MyCamera.BoundingSphere.Radius * 1.2f);
          
            MyPerformanceTimer.PrepareRenderCellCache.Start();

            for (int voxelMapIterator = 0; voxelMapIterator < m_voxelMaps.Count; voxelMapIterator++)
            {
                //  Because this LoadInDraw will stop normal update calls, we might not be able to send keep alive
                //  messages to server for some time. This will help it - it will make networking be up-to-date.
                //MyClientServer.Update();
                
                MyVoxelMap voxelMap = m_voxelMaps[voxelMapIterator];

                //  Only voxel maps near bounding sphere
                if (voxelMap.IsSphereIntersectingBoundingSphereOfThisVoxelMap(ref sphere))
                {
                    voxelMap.PrepareRenderCellCache();
                }
            }

            MyPerformanceTimer.PrepareRenderCellCache.End();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyVoxelMaps.PrepareRenderCellCache - END");
        }

        //  Save every voxel map into file. Destination folder is same as for log files.
        public static void SaveVoxelContents()
        {
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                string voxelName = "VoxelMap_" + voxelMap.VoxelMapId + "_" + MyConfig.Username + ".vox";
                string voxelFilePath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), voxelName);
                voxelMap.SaveVoxelContents(voxelFilePath, true);
                if (MyFakes.MWBUILDER)
                {   //Use name as storage for filepath
                    voxelMap.SetName(voxelName);
                }
            }
        }

        //  Outputs all voxel maps in the scene to a file of the OBJ file format
        //  All voxel maps will be output as a single mesh
        public static void SaveTriangles()
        {
            if (m_voxelMaps.Count <= 0) return;

            String fileName = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "VoxelMaps_" + MyValueFormatter.GetFormatedDateTimeForFilename(DateTime.Now) + ".obj");
            using (FileStream fs = File.Create(fileName))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (MyVoxelMap voxelMap in m_voxelMaps)
                    {
                        voxelMap.SaveVoxelVertices(sw);
                    }

                    foreach (MyVoxelMap voxelMap in m_voxelMaps)
                    {
                        voxelMap.SaveVoxelNormals(sw);
                    }

                    int vertexOffset = 0;
                    foreach (MyVoxelMap voxelMap in m_voxelMaps)
                    {
                        voxelMap.SaveVoxelFaces(sw, ref vertexOffset);
                    }
                }
            }
        }

        //  Converts voxel's content from byte to float. Returned value is just byte converted to float on interval <0..1>
        public static float GetVoxelContentAsFloat(byte content)
        {
            return (float)content / MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT;
        }

        public static MyMwcVector3Short GetVoxelCenterCoordinateFromMeters(Vector3 voxelPosition)
        {
            return new MyMwcVector3Short(
                (short)Math.Round((voxelPosition.X - MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF) / MyVoxelConstants.VOXEL_SIZE_IN_METRES),
                (short)Math.Round((voxelPosition.Y - MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF) / MyVoxelConstants.VOXEL_SIZE_IN_METRES),
                (short)Math.Round((voxelPosition.Z - MyVoxelConstants.VOXEL_SIZE_IN_METRES_HALF) / MyVoxelConstants.VOXEL_SIZE_IN_METRES));
        }


        public static void Add(MyVoxelMap voxelMap)
        {
            if (!Exist(voxelMap))
            {
                voxelMap.OnVoxelHandShapeCountChange += OnVoxelHandShapeCountChange;
                m_voxelShapesCount += voxelMap.GetVoxelHandShapes().Count;
                m_voxelMaps.Add(voxelMap);
                //if (MyFakes.DETECT_ORE_DEPOSITS_IN_VOXEL_MAPS)
                //{
                //    // seek all ore deposits of voxel map

                //    MyMwcLog.WriteLine("Seek ore deposits - START (" + voxelMap.SizeInMetresHalf.X + "," + voxelMap.SizeInMetres.Y + "," + voxelMap.SizeInMetres.Z + ")");
                //    DateTime startTime = DateTime.Now;                    
                //    voxelMap.SeekOreDeposits(new MyMwcVector3Int(0, 0, 0), voxelMap.SizeMinusOne);
                //    DateTime endTime = DateTime.Now;
                //    MyMwcLog.WriteLine("Seek ore deposits - END (" + endTime.Subtract(startTime).TotalMilliseconds + ")");
                //}
                //Need to be here because of materials, when added voxel in editor
                if (AutoRecalculateVoxelMaps)
                    RecalcVoxelMaps();
            }
        }

        public static void CutOutSphere(BoundingSphere sphere)
        {
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                MyVoxelGenerator.CutOutSphere(voxelMap, sphere);
            }
        }

        public static MyVoxelMap GetRandomVoxelMap()
        {
            int randomVoxelIndex = MyMwcUtils.GetRandomInt(0, m_voxelMaps.Count - 1);
            return m_voxelMaps[randomVoxelIndex];
        }

        public static MyVoxelMap GetLargestVoxelMap()
        {
            MyVoxelMap largestVoxelMap = null;
            Vector3 voxelSizeInMetres = Vector3.Zero;
            foreach (MyVoxelMap voxelMap in m_voxelMaps)
            {
                if (voxelMap.SizeInMetres.Length() > voxelSizeInMetres.Length())
                {
                    voxelSizeInMetres = voxelMap.SizeInMetres;
                    largestVoxelMap = voxelMap;
                }
            }
            return largestVoxelMap;
        }

        public static int GetVoxelMapsCount()
        {
            return m_voxelMaps.Count;
        }

        public static int GetVoxelShapesCount()
        {
            //int count = 0;
            //foreach (MyVoxelMap voxelMap in m_voxelMaps)
            //{
            //    count += voxelMap.GetVoxelHandShapes().Count;
            //}
            //return count;
            return m_voxelShapesCount;
        }

        public static int GetRemainingVoxelHandShapes()
        {
            return MyVoxelConstants.MAX_VOXEL_HAND_SHAPES_COUNT - GetVoxelShapesCount();
        }

        public static List<MyDataCellForSorting> GetSortedDataCellList()
        {
            return m_sortedDataCellList;
        }

        public static void RemoveVoxelMapOreDepositCell(MyVoxelMapOreDepositCell cell)
        {
            if (OnRemoveOreCell != null)
                OnRemoveOreCell(cell);
        }
    }
}

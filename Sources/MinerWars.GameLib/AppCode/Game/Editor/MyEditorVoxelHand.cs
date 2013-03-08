using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Audio;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Entities.WayPoints;
using System;
using System.Collections.Generic;

namespace MinerWars.AppCode.Game.Editor
{
    delegate void OnVoxelShapeSizeChanged(float newSize);
    delegate void OnVoxelShapeDistanceChanged(float newDistance);

    static class MyEditorVoxelHand
    {
        public static MyVoxelHandShape VoxelHandShape; // We keep reference to shape, that is currently attached to camera
        static float m_distance = MyVoxelConstants.DEFAULT_VOXEL_HAND_DISTANCE; // when objects attached to camera, this is used to change distance of objects from camera as required
        static MyVoxelMap m_applyToVoxelMap;  // reference to voxel map, to which voxel hand shapes are applied
        private static bool m_Enabled;
        private static Vector3 m_conePosition;
        public static bool IsProjected;
        public static bool IsProjectedToWaypoints;

        private static int m_timeFromLastShaping = 0;
        public static MyVoxelMap SelectedVoxelMap = null;

        public static MyDummyPoint DetachedVoxelHand = null;

        static MyEditorVoxelHand()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.VoxelHand, "Voxel hand", Draw, Render.MyRenderStage.PrepareForDraw);
        }

        public static void LoadData()
        {
            // default shape is sphere
            MyMwcObjectBuilder_VoxelHand_Sphere defaultBuilder = new MyMwcObjectBuilder_VoxelHand_Sphere(new MyMwcPositionAndOrientation(MySpectator.Orientation, Vector3.Forward, Vector3.Up), MyVoxelConstants.DEFAULT_VOXEL_HAND_SIZE, MyMwcVoxelHandModeTypeEnum.SUBTRACT);
            VoxelHandShape = new MyVoxelHandSphere();
            ((MyVoxelHandSphere)VoxelHandShape).Init(defaultBuilder, null);

            m_Enabled = false;
            m_timeFromLastShaping = 0;
            m_distance = MyVoxelConstants.DEFAULT_VOXEL_HAND_DISTANCE;
            IsProjected = false;
            IsProjectedToWaypoints = false;
        }

        public static void UnloadData()
        {
        }

        public static void LoadContent()
        {
        }

        public static void UnloadContent()
        {
        }

        //public static event OnVoxelShapeSizeChanged OnVoxelShapeSize;
        //public static event OnVoxelShapeDistanceChanged OnVoxelShapeDistance;
        
        public static void HandleInput(MyGuiInput input)
        {
            // exit voxel hand using this key
            if (input.IsEditorControlNewPressed(MyEditorControlEnums.VOXEL_HAND))
            {
                SwitchEnabled();
            }

            if (m_Enabled == false || !IsAnyEditorActive()) return;

            m_applyToVoxelMap = null;

            //here possible change
            if ((input.IsEditorControlNewPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY)
                || input.IsAnyShiftKeyPressed() && input.IsEditorControlPressed(MyEditorControlEnums.PRIMARY_ACTION_KEY)) &&
                (m_timeFromLastShaping >= MyVoxelConstants.VOXEL_HAND_SHAPING_INTERVAL || MyFakes.RAPID_VOXEL_HAND_SHAPING_ENABLED || MyFakes.MWBUILDER))
            {
                m_timeFromLastShaping = 0;

                if (DetachedVoxelHand != null && !input.IsKeyPress(Keys.Space))
                    return;

                MyVoxelMap voxelMap = null;

                if (VoxelHandShape is MyVoxelHandSphere)
                {
                    MyVoxelHandSphere sphere = (MyVoxelHandSphere)VoxelHandShape;
                    BoundingSphere vol = sphere.WorldVolume;
                    voxelMap = MyVoxelMaps.GetIntersectionWithSphere(ref vol, SelectedVoxelMap);
                }
                else if (VoxelHandShape is MyVoxelHandBox)
                {
                    MyVoxelHandBox box = (MyVoxelHandBox)VoxelHandShape;
                    BoundingBox localBoundingBox = box.GetLocalBoundingBox();
                    Vector3 boxWorldPosition = box.GetPosition();
                    voxelMap = MyVoxelMaps.GetIntersectionWithBox(ref localBoundingBox, ref boxWorldPosition, SelectedVoxelMap);
                }
                else if (VoxelHandShape is MyVoxelHandCuboid)
                {
                    MyVoxelHandCuboid cuboid = (MyVoxelHandCuboid)VoxelHandShape;
                    BoundingBox localBoundingBox = cuboid.GetLocalBoundingBox();
                    Vector3 boxWorldPosition = cuboid.GetPosition();
                    voxelMap = MyVoxelMaps.GetIntersectionWithBox(ref localBoundingBox, ref boxWorldPosition, SelectedVoxelMap);
                }
                else if (VoxelHandShape is MyVoxelHandCylinder)
                {
                    MyVoxelHandCylinder cylinder = (MyVoxelHandCylinder)VoxelHandShape;
                    BoundingBox localBoundingBox = cylinder.GetLocalBoundingBox();
                    Vector3 boxWorldPosition = cylinder.GetPosition();
                    voxelMap = MyVoxelMaps.GetIntersectionWithBox(ref localBoundingBox, ref boxWorldPosition, SelectedVoxelMap);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false);
                }

                if (voxelMap != null)
                {
                    m_applyToVoxelMap = voxelMap;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Change size of asteroid tool from camera
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                     /*
            if (input.IsAnyShiftKeyPressed())
            {
                if (input.PreviousMouseScrollWheelValue() > input.MouseScrollWheelValue())
                {                    
                    SetVoxelSize(MyEditorVoxelHand.VoxelHandShape.GetShapeSize() - MyVoxelConstants.VOXEL_HAND_SIZE_STEP);
                }
                else if (input.PreviousMouseScrollWheelValue() < input.MouseScrollWheelValue())
                {                    
                    SetVoxelSize(MyEditorVoxelHand.VoxelHandShape.GetShapeSize() + MyVoxelConstants.VOXEL_HAND_SIZE_STEP);
                }

            }          

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Change distance of asteroid tool from camera
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (input.IsAnyControlPress())
            {
                if (input.PreviousMouseScrollWheelValue() > input.MouseScrollWheelValue())
                {
                    SetShapeDistance(GetShapeDistance() - MyVoxelConstants.VOXEL_HAND_DISTANCE_STEP);

                }
                else if (input.PreviousMouseScrollWheelValue() < input.MouseScrollWheelValue())
                {
                    SetShapeDistance(GetShapeDistance() + MyVoxelConstants.VOXEL_HAND_DISTANCE_STEP);
                }
            } */
        }        

        public static Matrix UpdateShapePosition()
        {
            if (DetachedVoxelHand != null)
            {
                VoxelHandShape.MoveAndRotate(DetachedVoxelHand.WorldMatrix.Translation, DetachedVoxelHand.WorldMatrix);
                return DetachedVoxelHand.WorldMatrix;
            }

            Matrix world = Matrix.Identity;

            float minDist = 2 * VoxelHandShape.LocalVolume.Radius;
            float dist = 2 * minDist;
            Vector3 from = MyCamera.Position - MyCamera.UpVector * minDist * 0.5f;

            if (IsProjected)
            {
                if (!MyFakes.MWBUILDER)
                {
                    var line = new MyLine(from, from + MyCamera.ForwardVector * 10000, true);
                    var hit = MyEntities.GetIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref line, new System.Type[] { typeof(MyVoxelMap) });
                    if (hit != null)
                        dist = Vector3.Distance(MyCamera.Position, hit.Value.IntersectionPointInWorldSpace);
                    else
                        dist = 5000;

                    m_conePosition = from + MyCamera.ForwardVector * minDist * 0.7f;
                    Vector3 shapePosition = from + MyCamera.ForwardVector * (dist + VoxelHandShape.LocalVolume.Radius * m_distance * 2);
                    Vector3 shapeForward = Vector3.Normalize(MyCamera.UpVector * minDist * 0.5f + shapePosition - from);
                    Vector3 shapeUp = shapeForward - MyCamera.ForwardVector + MyCamera.UpVector;
                    world = Matrix.CreateWorld(shapePosition, shapeForward, shapeUp);
                    VoxelHandShape.MoveAndRotate(shapePosition, world);
                    return world;
                }
                else
                {
                    var line = new MyLine(from, from + MyCamera.ForwardVector * 10000, true);
                    var hit = MyEntities.GetIntersectionWithLine_IgnoreOtherThanSpecifiedClass(ref line, new System.Type[] { typeof(MyVoxelMap) });
                    Vector3 normal = Vector3.Up;
                    dist = 5000;
                    Vector3 shapePosition = from + MyCamera.ForwardVector * dist;
                    if (hit != null)
                    {
                        dist = Vector3.Distance(MyCamera.Position, hit.Value.IntersectionPointInWorldSpace);
                        normal = hit.Value.NormalInWorldSpace;
                        shapePosition = hit.Value.IntersectionPointInWorldSpace;
                    }
                    

                    m_conePosition = from + MyCamera.ForwardVector * minDist * 0.7f;
                    
                    Vector3 shapeUp = normal;
                    Vector3 shapeForward = Vector3.Cross(-MyCamera.LeftVector, shapeUp);
                    float dot = Vector3.Dot(shapeUp, shapeForward);
                    if ((dot > 0.9f) || (dot < -0.9f))
                    {
                        shapeForward = Vector3.Forward;
                    }

                    shapePosition.X = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(shapePosition.X / MyEditorGrid.GetGridStepInMeters());
                    shapePosition.Y = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(shapePosition.Y / MyEditorGrid.GetGridStepInMeters());
                    shapePosition.Z = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(shapePosition.Z / MyEditorGrid.GetGridStepInMeters());

                     
                    world = Matrix.CreateWorld(shapePosition, shapeForward, shapeUp);
                    VoxelHandShape.MoveAndRotate(shapePosition, world);
                    return world; 
                }
            }
            else if (IsProjectedToWaypoints)
            {
                //Lets find projection on closest collision with waypoint edge
                from = MyCamera.Position - MyCamera.UpVector * 10.5f;
                m_conePosition = from + MyCamera.ForwardVector * 10.7f;
                Vector3 shapePosition = from + MyCamera.ForwardVector * m_distance;

                List<Tuple<MyWayPoint, MyWayPoint>> edges = MyWayPointGraph.GetAllEdgesInSphere(shapePosition, VoxelHandShape.LocalVolume.Radius * 2);

                Tuple<MyWayPoint, MyWayPoint> closestEdge = null;
                float minDistance = float.MaxValue;
                Vector3 closestPoint = shapePosition;
                float distFromFirstWaypoint = 0;

                foreach (Tuple<MyWayPoint, MyWayPoint> edge in edges)
                {
                    Vector3 linePos1 = edge.Item1.Position;
                    Vector3 linePos2 = edge.Item2.Position;

                    Vector3 point = MyUtils.GetClosestPointOnLine(ref linePos1, ref linePos2, ref shapePosition, out distFromFirstWaypoint);

                    float distance = Vector3.Distance(shapePosition, point);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestEdge = edge;
                        closestPoint = point;
                    }
                }

                if (closestEdge != null)
                {
                    shapePosition = closestPoint;

                    Vector3 shapeForward = Vector3.Normalize(shapePosition - from);
                    Vector3 shapeUp = MyCamera.UpVector;
                    world = Matrix.CreateWorld(shapePosition, shapeForward, shapeUp);
                    VoxelHandShape.MoveAndRotate(shapePosition, world);

                    MyWayPoint firstWaypoint = null;
                    MyWayPoint secondWaypoint = null;
                    float edgeDistance = Vector3.Distance(closestEdge.Item1.Position, closestEdge.Item2.Position);

                    if (closestEdge.Item1.Position.Y < closestEdge.Item2.Position.Y)
                    {
                        firstWaypoint = closestEdge.Item1;
                        secondWaypoint = closestEdge.Item2;
                    }
                    else
                    {
                        firstWaypoint = closestEdge.Item2;
                        secondWaypoint = closestEdge.Item1;
                        distFromFirstWaypoint = edgeDistance - distFromFirstWaypoint;
                    }

                    float edgeRatio = edgeDistance > 0 ? distFromFirstWaypoint / edgeDistance : 0;

                    Quaternion quaternion1 = Quaternion.CreateFromRotationMatrix(closestEdge.Item1.WorldMatrix);
                    Quaternion quaternion2 = Quaternion.CreateFromRotationMatrix(closestEdge.Item2.WorldMatrix);

                    Quaternion resultQuaternion = Quaternion.Lerp(quaternion1, quaternion2, edgeRatio);

                    Matrix resultMatrix = Matrix.CreateFromQuaternion(resultQuaternion);
                    resultMatrix.Translation = shapePosition;

                    VoxelHandShape.MoveAndRotate(shapePosition, resultMatrix);

                    return resultMatrix;
                }
            }

            if (MyFakes.MWBUILDER)
            {
                from = MyCamera.Position;
                m_conePosition = from + MyCamera.ForwardVector * 10.7f;
                        /*
                //Vector3 planeNormal = new Vector3(MyCamera.ForwardVector.X, 0, MyCamera.ForwardVector.Z);
                Vector3 planeNormal = new Vector3(0, MyCamera.ForwardVector.Y, MyCamera.ForwardVector.Z);
                
                
                planeNormal.Normalize();
                Vector3 planePoint = planePoint = from + planeNormal * m_distance;
                planeNormal = -planeNormal;

                Plane plane = new Plane(planeNormal, -Vector3.Dot(planeNormal, planePoint));

                Ray r = new Ray(from, MyCamera.ForwardVector);
                float? intr = r.Intersects(plane); */
               // if (intr.HasValue)
                {
                    Vector3 shapePosition = from + MyCamera.ForwardVector * m_distance;
                    Vector3 shapeForward = Vector3.Forward;
                    Vector3 shapeUp = Vector3.Up;

                    shapePosition.X = MyEditorGrid.GetGridStepInMeters() *
                                  (float)Math.Round(shapePosition.X / MyEditorGrid.GetGridStepInMeters());
                    shapePosition.Y = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(shapePosition.Y / MyEditorGrid.GetGridStepInMeters());
                    shapePosition.Z = MyEditorGrid.GetGridStepInMeters() *
                                (float)Math.Round(shapePosition.Z / MyEditorGrid.GetGridStepInMeters());

                    world = Matrix.CreateWorld(shapePosition, shapeForward, shapeUp);
                    VoxelHandShape.MoveAndRotate(shapePosition, world);
                }
            }
            else
            {
                from = MyCamera.Position - MyCamera.UpVector * 10.5f;
                m_conePosition = from + MyCamera.ForwardVector * 10.7f;
                Vector3 shapePosition = from + MyCamera.ForwardVector * m_distance;
                Vector3 shapeForward = Vector3.Normalize(/*MyCamera.UpVector * minDist * 0.5f*/ shapePosition - from);
                Vector3 shapeUp = /*shapeForward - MyCamera.ForwardVector +*/ MyCamera.UpVector;
                world = Matrix.CreateWorld(shapePosition, shapeForward, shapeUp);
                VoxelHandShape.MoveAndRotate(shapePosition, world);
            }

            return world;
        }

        public static void Update()
        {                        
            // update carving tool
            if (m_Enabled && IsAnyEditorActive())
            {
                m_timeFromLastShaping += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;

                // If we pressed LMB and collision of voxel hand shape with voxel map occurs, apply voxel hand shape to voxelMap
                if (m_applyToVoxelMap != null)
                {
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("if (m_applyToVoxelMap != null)");
                                
                    // New voxel hand shape object needs to be created, because otherwise all voxel hand shapes will point to same reference
                    // Perform action
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("new MyEditorActionVoxelHand");
                    MyEditorActionVoxelHand voxelHandAction = new MyEditorActionVoxelHand(m_applyToVoxelMap, VoxelHandShape.CreateCopy());
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("voxelHandAction.RegisterAndDoAction()");
                    voxelHandAction.RegisterAndDoAction();
                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                    // Immediately remove voxel map reference, so that only one hand shape is created at one LMB click
                    m_applyToVoxelMap = null;

                    MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
                }
            }
        }
                  /*
        public static void SetVoxelSize(float size)
        {            
            if ((size >= MyVoxelConstants.MIN_VOXEL_HAND_SIZE) && (size <= MyVoxelConstants.MAX_VOXEL_HAND_SIZE))
            {
                MyEditorVoxelHand.VoxelHandShape.SetShapeSize(size);
                if(OnVoxelShapeSize != null)
                {
                    OnVoxelShapeSize(size);
                }
            }
        }           */

        public static float GetShapeDistance()
        {
            return m_distance;
        }

        public static void SetShapeDistance(float distance)
        {
            float oldDistance = m_distance;
            if (IsProjected)
            {
                if (!MyFakes.MWBUILDER)
                {
                    if (distance < MyVoxelConstants.MIN_PROJECTED_VOXEL_HAND_OFFSET) distance = MyVoxelConstants.MIN_PROJECTED_VOXEL_HAND_OFFSET;
                    if (distance > MyVoxelConstants.MAX_PROJECTED_VOXEL_HAND_OFFSET) distance = MyVoxelConstants.MAX_PROJECTED_VOXEL_HAND_OFFSET;
                }

                if (distance != oldDistance)
                {
                    m_distance = distance;
                    //if (OnVoxelShapeDistance != null) OnVoxelShapeDistance(distance);
                }                
            }
            else if (IsProjectedToWaypoints)
            {
            }
            else
            {
                if (distance < MyVoxelConstants.MIN_VOXEL_HAND_DISTANCE) distance = MyVoxelConstants.MIN_VOXEL_HAND_DISTANCE;
                if (distance > MyVoxelConstants.MAX_VOXEL_HAND_DISTANCE) distance = MyVoxelConstants.MAX_VOXEL_HAND_DISTANCE;
                if (distance != oldDistance)
                {
                    m_distance = distance;
                    //if (OnVoxelShapeDistance != null) OnVoxelShapeDistance(distance);
                }
            }
        }


        public static void SetVoxelProperties(MyVoxelHandShapeType voxelHandShapeType, float distance, MyMwcVoxelHandModeTypeEnum modeType, MyMwcVoxelMaterialsEnum? materialEnum, bool isProjected, bool isProjectedToWaypoints)
        {
            IsProjected = isProjected;
            IsProjectedToWaypoints = isProjectedToWaypoints;
            // same shape type
            if (voxelHandShapeType == MyEditorVoxelHand.VoxelHandShape.GetShapeType())
            {
                //if ((size >= MyVoxelConstants.MIN_VOXEL_HAND_SIZE) && (size <= MyVoxelConstants.MAX_VOXEL_HAND_SIZE))
                {
                    //MyEditorVoxelHand.VoxelHandShape.SetShapeSize(size);
                    //MyEditorVoxelHand.VoxelHandShape.SetShapeSize2(size2);
                    //MyEditorVoxelHand.VoxelHandShape.SetShapeSize3(size3);
                    MyEditorVoxelHand.VoxelHandShape.ModeType = modeType;
                    MyEditorVoxelHand.VoxelHandShape.Material = materialEnum;
                }
            }
            // another shape type
            else 
            {
              //  float newShapeSize = size;
               // if ((newShapeSize < MyVoxelConstants.MIN_VOXEL_HAND_SIZE) && (newShapeSize > MyVoxelConstants.MAX_VOXEL_HAND_SIZE))
               
                float    newShapeSize = MyVoxelConstants.DEFAULT_VOXEL_HAND_SIZE;
                
                MyMwcPositionAndOrientation positionAndOritentation = new MyMwcPositionAndOrientation(MySpectator.Position, Vector3.Forward, Vector3.Up);
                switch (voxelHandShapeType) 
                {
                    case MyVoxelHandShapeType.Sphere:
                        MyMwcObjectBuilder_VoxelHand_Sphere sphereObjectBuilder = new MyMwcObjectBuilder_VoxelHand_Sphere(positionAndOritentation, newShapeSize, modeType);
                        MyEditorVoxelHand.VoxelHandShape = new MyVoxelHandSphere();
                        ((MyVoxelHandSphere)MyEditorVoxelHand.VoxelHandShape).Init(sphereObjectBuilder, null);
                        break;
                    case MyVoxelHandShapeType.Box:
                        MyMwcObjectBuilder_VoxelHand_Box boxObjectBuilder = new MyMwcObjectBuilder_VoxelHand_Box(positionAndOritentation, newShapeSize, modeType);
                        MyEditorVoxelHand.VoxelHandShape = new MyVoxelHandBox();
                        ((MyVoxelHandBox)MyEditorVoxelHand.VoxelHandShape).Init(boxObjectBuilder, null);
                        break;
                    case MyVoxelHandShapeType.Cuboid:
                        MyMwcObjectBuilder_VoxelHand_Cuboid cuboidObjectBuilder = new MyMwcObjectBuilder_VoxelHand_Cuboid(positionAndOritentation, newShapeSize, newShapeSize, newShapeSize, newShapeSize, newShapeSize, modeType);
                        MyEditorVoxelHand.VoxelHandShape = new MyVoxelHandCuboid();
                        ((MyVoxelHandCuboid)MyEditorVoxelHand.VoxelHandShape).Init(cuboidObjectBuilder, null);
                        break;
                    case MyVoxelHandShapeType.Cylinder:
                        MyMwcObjectBuilder_VoxelHand_Cylinder cylinderObjectBuilder = new MyMwcObjectBuilder_VoxelHand_Cylinder(positionAndOritentation, newShapeSize, newShapeSize, newShapeSize, modeType);
                        MyEditorVoxelHand.VoxelHandShape = new MyVoxelHandCylinder();
                        ((MyVoxelHandCylinder)MyEditorVoxelHand.VoxelHandShape).Init(cylinderObjectBuilder, null);
                        break;

                    default:
                        System.Diagnostics.Debug.Assert(false);
                        break;
                }
                MyEditorVoxelHand.VoxelHandShape.Material = materialEnum;
            }

            if (IsProjected)
            {
                if (!MyFakes.MWBUILDER)
                {
                    if ((distance >= MyVoxelConstants.MIN_PROJECTED_VOXEL_HAND_OFFSET) && (distance <= MyVoxelConstants.MAX_PROJECTED_VOXEL_HAND_OFFSET))
                        MyEditorVoxelHand.SetShapeDistance(distance);
                }
            }
            else if (IsProjectedToWaypoints)
            {
            }
            else
            {
                if ((distance >= MyVoxelConstants.MIN_VOXEL_HAND_DISTANCE) && (distance <= MyVoxelConstants.MAX_VOXEL_HAND_DISTANCE))
                    MyEditorVoxelHand.SetShapeDistance(distance);
            }

            MyEditorVoxelHand.UpdateShapePosition();
        }

        private static bool IsAnyEditorActive()
        {
            return MyGuiScreenGamePlay.Static.IsEditorActive() ||
                   MyGuiScreenGamePlay.Static.IsEditorMmoActive() ||
                   MyGuiScreenGamePlay.Static.IsEditorSandboxActive() ||
                   MyGuiScreenGamePlay.Static.IsEditorStoryActive() ||
                   MyGuiScreenGamePlay.Static.IsIngameEditorActive();
        }

        public static void Draw()
        {                                                       
            if (m_Enabled && IsAnyEditorActive())
            {                
                VoxelHandShape.Draw();
                if (MyVoxelConstants.VOXEL_HAND_DRAW_CONE && !MyFakes.MWBUILDER)
                {
                    VoxelHandShape.DrawCone(m_conePosition);
                }

                if (MyEditor.DisplayVoxelBounding)
                {
                    foreach (MyEntity voxelMapEntity in MyVoxelMaps.GetVoxelMaps())
                    {
                        MyVoxelMap voxelMap = voxelMapEntity as MyVoxelMap;
                        voxelMap.DrawBounding();
                    }
                }

                if (MyFakes.MWBUILDER)
                {
                    Matrix wm = VoxelHandShape.WorldMatrix;
                    Vector4 clr = Vector4.One;
                    MySimpleObjectDraw.DrawTransparentSphere(ref wm, 10, ref clr, false, 8);

                    MyDebugDraw.TextBatch.AddText(wm.Translation, new System.Text.StringBuilder(MyUtils.GetFormatedVector3(wm.Translation, 0)), Color.White, 0.8f);
                }
            }
        }

        public static void SwitchEnabled()
        {
            m_Enabled = !m_Enabled;
            MyAudio.AddCue2D(MySoundCuesEnum.GuiEditorVoxelHandSwitch);

            //if (m_Enabled == true)
            //{
            //    SelectedVoxelMap = null;
            //    foreach (var entity in MyEditorGizmo.SelectedEntities) if (entity is MyVoxelMap)
            //    {
            //        SelectedVoxelMap = entity as MyVoxelMap;
            //    }
            //}
        }

        public static bool IsEnabled()
        {
            return m_Enabled;
        }

        public static void SetEnabled(bool enabled)
        {
            if (m_Enabled != enabled)
            {
                SwitchEnabled();
            }
        }
    }
}

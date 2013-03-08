using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Entities.Ships.SubObjects
{
    class MyCubeBuilder : MyEntity
    {
        public bool BuilderActive { get; set; }

        public List<MyEntity> m_intersections = new List<MyEntity>(3);

        public MyCubeBuilder()
        {
        }

        public override void Init(StringBuilder displayName, Models.MyModelsEnum? modelLod0Enum, Models.MyModelsEnum? modelLod1Enum, MyEntity parentObject, float? scale, CommonLIB.AppCode.ObjectBuilders.MyMwcObjectBuilder_Base objectBuilder, Models.MyModelsEnum? modelCollision = null, Models.MyModelsEnum? modelLod2Enum = null)
        {
            // Some fake builder
            var builder = new MyMwcObjectBuilder_SmallShip_Weapon(MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher);

            base.Init(displayName, modelLod0Enum, modelLod1Enum, parentObject, scale, builder, modelCollision, modelLod2Enum);
            SetModel(MyModelsEnum.MysteriousBox_matt_5m);
        }

        void SetModel(MyModelsEnum model)
        {
            m_modelLod0 = MyModels.GetModelForDraw(model);
            m_modelLod1 = null;
        }

        public MyIntersectionResultLineTriangleEx? Intersect()
        {
            float maxDist = 200;
            MyLine line = new MyLine(MyCamera.Position, MyCamera.Position + MyCamera.ForwardVector * maxDist);
            var result = MyEntities.GetIntersectionWithLine(ref line, MySession.PlayerShip, null, true, true, false, false, true);
            return result;
        }

        public void Add()
        {
            Vector3 position;
            if (GetAddPosition(out position))
            {
                Matrix matrix = Matrix.CreateTranslation(position);
                var cube = new MyMwcObjectBuilder_MysteriousCube(new CommonLIB.AppCode.Networking.MyMwcPositionAndOrientation(matrix));
                cube.MysteriousCubeType = MyMwcObjectBuilder_MysteriousCube_TypesEnum.Type2;
                MyEntities.CreateFromObjectBuilderAndAdd(null, cube, matrix);
            }
        }

        void MakeCubePosition(ref Vector3 position)
        {
            Vector3 size = m_modelLod0.BoundingBoxSize;
            Vector3 center = m_modelLod0.BoundingBox.Min + size / 2;
            position -= center;
            Vector3 div = position / size;
            div.X = (float)Math.Round(div.X);
            div.Y = (float)Math.Round(div.Y);
            div.Z = (float)Math.Round(div.Z);

            position = div * size;
        }

        bool GetAddPosition(out Vector3 position)
        {
            var result = Intersect();
            if (result != null)
            {
                position = result.Value.IntersectionPointInWorldSpace - MyCamera.ForwardVector * 0.05f;
            }
            else
            {
                position = MyCamera.Position + MyCamera.ForwardVector * 70;
            }

            MakeCubePosition(ref position);
            return result == null || position != result.Value.Entity.WorldMatrix.Translation;
        }

        bool IsSmallship(MyEntity entity)
        {
            return entity is MySmallShip;
        }

        public void Remove()
        {
            var result = Intersect();
            if (result.HasValue && result.Value.Entity is MyMysteriousCube)
            {
                result.Value.Entity.MarkForClose();
            }
        }

        public override bool Draw(MyRenderObject renderObject = null)
        {
            if (BuilderActive)
            {
                MyRender.AddRenderObjectToDraw(RenderObjects[0]);
                return true;
            }
            return false;
        }

        public override bool DebugDraw()
        {
            bool boxMode = false;

            Vector3 pos;
            if (GetAddPosition(out pos))
            {
                var result = Intersect();
                if(result.HasValue && result.Value.Entity is MyMysteriousCube)
                {
                    if (!boxMode)
                    {
                        float mult = 0.01f;
                        var bb = m_modelLod0.BoundingBox.Translate(pos);
                        bb.Max += m_modelLod0.BoundingBoxSize * mult;
                        bb.Min -= m_modelLod0.BoundingBoxSize * mult;
                        var bb2 = m_modelLod0.BoundingBox.Translate(result.Value.Entity.WorldMatrix.Translation);
                        bb2.Max += m_modelLod0.BoundingBoxSize * mult;
                        bb2.Min -= m_modelLod0.BoundingBoxSize * mult;

                        if (bb.Intersects(bb2))
                        {
                            bb2.Min = Vector3.Clamp(bb2.Min, bb.Min, bb.Max);
                            bb2.Max = Vector3.Clamp(bb2.Max, bb.Min, bb.Max);
                            Vector4 color = Color.Green.ToVector4();
                            color.W = 0.5f;
                            MyStateObjects.DepthStencil_TestFarObject_DepthReadOnly.Apply();
                            MyStateObjects.Additive_NoAlphaWrite_BlendState.Apply();
                            MyDebugDraw.DrawAABBSolidLowRes(bb2, color, 1.0f);
                        }
                    }
                    else
                    {
                        var bb = result.Value.Entity.WorldAABB;
                        bb.Inflate(0.1f);
                        Vector4 color = Color.Red.ToVector4();
                        MyStateObjects.DepthStencil_TestFarObject.Apply();
                        MyDebugDraw.DrawAABBLowRes(ref bb, ref color, 1.0f);
                    }
                }

                if (boxMode)
                {
                    BoundingBox bb = m_modelLod0.BoundingBox;
                    bb = bb.Translate(pos);
                    bb.Inflate(0.1f);
                    Vector4 color = Color.Green.ToVector4();
                    color.W = 0.5f;
                    MyStateObjects.DepthStencil_StencilReadOnly.Apply();
                    MyStateObjects.Additive_NoAlphaWrite_BlendState.Apply();
                    MyDebugDraw.DrawAABBSolidLowRes(bb, color, 1.0f);
                }
            }

            return base.DebugDraw();
        }

        public override void UpdateAfterSimulation()
        {
            LocalMatrix = Matrix.CreateScale(0.7f) * Matrix.CreateTranslation(new Vector3(0, -7, -10));
            base.UpdateAfterSimulation();
        }
    }
}

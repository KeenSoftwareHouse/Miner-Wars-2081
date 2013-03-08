//  How to find correct positions for lights or lams on mother ships or similar large and static objects?
//  You can't get those coords from 3dsmax because there is still a chance that coords that 3dsmax tells
//  you will be different than what you import (internal 3dsmax transformations...), and also model can
//  be rescaled and re-centered so that will definitely destroy your hand grabbed coords.
//  Instead go with camera at place where you wanna place light/lamp, press F12 for debug screen, get your
//  current camera position, subtract object's center out of it and you have relative coordinates.
/*
namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using App;
    using CommonLIB.AppCode.ObjectBuilders.Object3D;
    using Lights;
    using MinerWarsMath;
    using Models;
    using PhysicsManager.Physics;
    using TransparentGeometry;
    using Physics;
    using SysUtils.Utils;
    using Utils;
    using MinerWars.CommonLIB.AppCode.Utils;
    
    using MinerWars.AppCode.Game.HUD;

    /// <summary>
    /// Large ship.
    /// </summary>
    class MyLargeShip : MyShip
    {
        class MyLargeShipLamp
        {
            public readonly Vector3 Position;
            public readonly float RadiusMin;
            public readonly float RadiusMax;
            public readonly int TimerForBlic;
            public readonly MyLight Light;

            public MyLargeShipLamp(Vector3 position, float radiusMin, float radiusMax, int timerForBlic)
            {
                Position = position;
                RadiusMin = radiusMin;
                RadiusMax = radiusMax;
                TimerForBlic = timerForBlic;
                Light = MyLights.AddLight();
                Light.Start(MyLight.LightTypeEnum.PointLight, position, Vector4.One, 1, radiusMin);
                Light.Intensity = 1;
                Light.LightOn = true;
            }
        }

        private List<MyLargeShipLamp> m_lamps;
        const float BLIC_DURATON_IN_MILISECONDS = 30.0f;
        bool m_debugIndicateShip = false;

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_LargeShip objectBuilder)
        {
            base.Init(hudLabelText, objectBuilder);
            switch (objectBuilder.ShipType)
            {
                case MyMwcObjectBuilder_LargeShip_TypesEnum.KAI:
                    {
                        List<MyLargeShipLamp> localLamps = new List<MyLargeShipLamp>();

                        MyLight hangarLight = null;
                        
                        //TODO: temporary, we need to set this from editor
                        Name = "KAI Mothership";

                        Dictionary<string, MyModelDummy> dummies = MyModels.GetModelOnlyData(MyModelsEnum.Kai_LOD0).Dummies;
                        foreach (KeyValuePair<string, MyModelDummy> dummy in dummies)
                        {
                            float scale = 4 * dummy.Value.Matrix.Left.Length();
                            if (dummy.Key.StartsWith("LAMP"))
                                localLamps.Add(new MyLargeShipLamp(dummy.Value.Matrix.Translation, 5 * scale, 20 * scale, 980));
                            if (dummy.Key.StartsWith("HANGAR_LIGHT"))
                            {
                                hangarLight = MyLights.AddLight();
                                hangarLight.Start(MyLight.LightTypeEnum.PointLight, WorldMatrix.Translation + dummy.Value.Matrix.Translation, new Vector4(0, 1, 0, 1), 2f, 25 * scale);
                            }
                        }

                        InitShip(Name, MyModelsEnum.Kai_LOD0, MyModelsEnum.Kai_LOD1, MyMaterialType.SHIP, objectBuilder,
                                 hangarLight, localLamps);

                        MyHud.ChangeText(this, new StringBuilder(Name), null, 0, MyHudIndicatorFlagsEnum.SHOW_ALL);

                    }
                    break;
                case MyMwcObjectBuilder_LargeShip_TypesEnum.MOTHERSHIP_SAYA:
                    InitShip(hudLabelText, MyModelsEnum.MotherShipSaya, MyModelsEnum.MotherShipSaya_LOD1, MyMaterialType.SHIP, objectBuilder, null, null);
                    break;
                //temporary for testing - 0000177: Red large ship
                case MyMwcObjectBuilder_LargeShip_TypesEnum.CRUISER_SHIP:
                    InitShip(hudLabelText, MyModelsEnum.CruiserShip, null, MyMaterialType.SHIP, objectBuilder, null, null);
                    break;
                case MyMwcObjectBuilder_LargeShip_TypesEnum.ARDANT:
                    InitShip(hudLabelText, MyModelsEnum.Ardant, MyModelsEnum.Ardant_LOD1, MyMaterialType.SHIP, objectBuilder, null, null);
                    break;
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        //  Position - in world space
        //  HangarLight - position in world space
        //  LocalLamps - position in world space
        void InitShip(string hudLabelText, MyModelsEnum modelLod0Enum, MyModelsEnum? modelLod1Enum, MyMaterialType materialType, MyMwcObjectBuilder_LargeShip objectBuilder, MyLight hangarLight,
            List<MyLargeShipLamp> localLamps)
        {
            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);
            MyModel model = MyModels.GetModelOnlyData(modelLod0Enum);

            base.Init(hudLabelTextSb, modelLod0Enum, modelLod1Enum, null, null, objectBuilder);

            MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
            MyRBTriangleMeshElementDesc trianglemeshDesc = physobj.GetRBTriangleMeshElementDesc();

            trianglemeshDesc.SetToDefault();
            trianglemeshDesc.m_Model = model;
            trianglemeshDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(materialType).PhysicsMaterial; 

            MyRBTriangleMeshElement trEl = (MyRBTriangleMeshElement)physobj.CreateRBElement(trianglemeshDesc);

            Matrix matrix = objectBuilder.PositionAndOrientation.GetMatrix();
            SetWorldMatrix(matrix);

            this.Physics = new MyGameRigidBody(this, 100.0f, RigidBodyFlag.RBF_RBO_STATIC) { MaterialType = materialType };
            //this.Physics.Enabled = true;
            this.Physics.AddElement(trEl, true);
                       
            if (localLamps != null)
            {
                m_lamps = new List<MyLargeShipLamp>();
                for (int i = 0; i < localLamps.Count; i++)
                {
                    m_lamps.Add(new MyLargeShipLamp(matrix.Translation + localLamps[i].Position, localLamps[i].RadiusMin,
                        localLamps[i].RadiusMax, localLamps[i].TimerForBlic));
                }
            }
        }

        public override bool Draw()
        {
            bool retVal = base.Draw();
            if (retVal)
            {
                if (m_lamps != null)
                {
                    for (int i = 0; i < m_lamps.Count; i++)
                    {
                        MyLargeShipLamp lamp = m_lamps[i];
                        DrawGlare(lamp);
                    }
                }
            }

            return retVal;
        }

        public override bool DebugDraw()
        {
            if (!base.DebugDraw())
                return false;

            if (m_debugIndicateShip)
            {
                MyDebugDraw.DrawSphereWireframe(WorldMatrix.Translation, 20, Vector3.One, 1);
            }  



            return true;
        }


        static void DrawGlare(MyLargeShipLamp lamp)
        {
            Vector3 dir = MyMwcUtils.Normalize(MyCamera.Position - lamp.Position);

            float timeBlic = MyMinerGame.TotalGamePlayTimeInMilliseconds % lamp.TimerForBlic;
            if (timeBlic > BLIC_DURATON_IN_MILISECONDS) timeBlic = lamp.TimerForBlic - timeBlic;
            timeBlic = MathHelper.Clamp(1 - timeBlic / BLIC_DURATON_IN_MILISECONDS, 0, 1);

            float radius = MathHelper.Lerp(lamp.RadiusMin, lamp.RadiusMax, timeBlic);

            MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, Vector4.One,
                lamp.Position + dir * 5, radius, 0);

            lamp.Light.Range = radius * 4;
        }

        public override void Close()
        {
            base.Close();
        }

        //  Calculate thrusts strength, etc
        public override bool UpdateAfterIntegration()
        {
            if (base.UpdateAfterIntegration() == false) return false;

            return true;
        }
    }
}
*/
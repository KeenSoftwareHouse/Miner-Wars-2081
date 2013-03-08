using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using System;
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.Networking;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using CommonLIB.AppCode.Utils;
    using MinerWars.AppCode.Game.Decals;
    using Lights;
    using MinerWarsMath;
    
    using Models;
    using Utils;
    using Voxels;
    using Weapons;
    using World;
    using MinerWars.AppCode.Game.Inventory;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Networking.SectorService;
    using System.Diagnostics;
    using MinerWars.AppCode.Game.Sessions;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.HUD;
    using KeenSoftwareHouse.Library.Extensions;
    using MinerWars.AppCode.Game.GUI.Helpers;
    using MinerWars.AppCode.Game.GUI.Core;
    using MinerWars.AppCode.Game.Localization;
    using System.Collections.Generic;

    public enum MyHarvestingDeviceEnum
    {
        FindingVoxel,           //  When tube is ejecting and looking for voxels
        InVoxel,                //  When tube/head mounted into voxels and is harvesting
        ReturningBack,          //  When tube didn't find voxels so it's pulling back
        FastReturningBack,      //  When tube collided with something so we must pull it back quickly, so player won't notice intersection
        InsideShip              //  When tube is inside miner ship, default status
    }

    class MyHarvestingDevice : MySmallShipGunBase
    {
        MyHarvestingHead m_harvestingOreHead;
        MyHarvestingDeviceEnum m_currentState;

        public MyHarvestingDeviceEnum CurrentState
        {
            get { return m_currentState; }
            private set
            {
                m_currentState = value;
                Visible = m_currentState != MyHarvestingDeviceEnum.InsideShip;
            }
        }

        MySoundCue? m_grindingCue;
        MySoundCue? m_grindingReleaseCue;
        MySoundCue? m_tubeMovingCue;
        MySoundCue? m_implodeCue;

        int? m_lastTimeParticleAdded;
        Vector3 m_headPositionLocal;                //  Position of head relative to this launcher (in object space)
        Vector3 m_headPositionTransformed;          //  Position of head in world space
        MySmallShip m_parentMinerShip;
        float m_lastTimeCheckedForVoxelPresence;    //  If we are connected to voxel we check every time if we are still connected
        MyLight m_light;                            //  Dynamic light at the tube head
        MyVoxelMap m_inVoxelMap;          //  To which voxel map are we sticked in

        float m_harvestingSpeed;
        //  Sphere for head of harvester
        //BoundingSphere m_headSphere;

        //  We calculate how much should be in voxel while harvesting. It is stored in float because we remove each update from it small value
        float m_actualVoxelContent;

        //  How much content was in voxel when we started harvesthing
        byte m_originalVoxelContent;

        //  Material of voxel to which we are connected (approximate)
        MyMwcVoxelMaterialsEnum m_voxelMaterial;

        MyParticleEffect m_harvestingParticleEffect;

        //  How much updates it takes to harvest whole voxel (ignoreing its content value, even empty voxels will take this time to harvest)
        const float TIME_TO_HARVEST_WHOLE_VOXEL_IN_UPDATE_TIMES = 5 * MyConstants.PHYSICS_STEPS_PER_SECOND;
        //const float HARVESTER_ROCK_THROWN_STRENGTH = 7;

        public override void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            base.Init(hudLabelText, MyModelsEnum.HarvestingTube, MyMaterialType.METAL, parentObject,
                position, forwardVector, upVector, objectBuilder);

            m_harvestingOreHead = new MyHarvestingHead();
            m_harvestingOreHead.Init(this);

            m_parentMinerShip = (MySmallShip)Parent;

            StartInsideShip();
        }

        //  When tube is ejecting and looking for voxels
        void StartFindingVoxel()
        {
            //  Else we normally start ejecting
            CurrentState = MyHarvestingDeviceEnum.FindingVoxel;
            m_headPositionLocal = Vector3.Zero;
            StartTubeMovingCue();
            StopGrindingCue();

            if (m_light != null)
                MyLights.RemoveLight(m_light);
            m_light = null;

            //m_light = MyLights.AddLight();

            if (m_light != null)
            {
                var color = 5 * new Vector4(1, 0.3f, 0.3f, 1);
                m_light.Start(MyLight.LightTypeEnum.PointLight, color, 3, 10);
            }

            MyScriptWrapper.HarvesterUse();
            UpdateAfterSimulation();
        }

        //  When tube/head mounted into voxels and is harvesting
        void StartInVoxel(MyVoxelMap voxelMap)
        {
            //  We found voxel so we stop here
            m_inVoxelMap = voxelMap;
            CurrentState = MyHarvestingDeviceEnum.InVoxel;
            StopTubeMovingCue();
            StartGrindingCue();
            m_lastTimeParticleAdded = null;
            m_parentMinerShip.Physics.Clear();
            m_parentMinerShip.Physics.Immovable = true;

            MyMwcVector3Int tempVoxelCoord = voxelMap.GetVoxelCenterCoordinateFromMeters(ref m_headPositionTransformed);
            m_originalVoxelContent = voxelMap.GetVoxelContent(ref tempVoxelCoord);
            m_voxelMaterial = voxelMap.GetVoxelMaterial(ref tempVoxelCoord);


            m_harvestingParticleEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Harvester_Harvesting);
            m_harvestingParticleEffect.UserBirthMultiplier = 0.25f;
            m_harvestingParticleEffect.UserRadiusMultiplier = 1;
            m_harvestingParticleEffect.UserColorMultiplier = new Vector4(3, 3, 3, 3);
            Matrix dirMatrix = MyMath.MatrixFromDir(WorldMatrix.Forward);
            m_harvestingParticleEffect.WorldMatrix = Matrix.CreateWorld(m_headPositionTransformed, dirMatrix.Forward, dirMatrix.Up);

            //  Empty voxels are problematic and can lead to "extremely fast harvesting". So here we do this
            //  trick and its effect will be that even empty voxels will take few seconds to harvest.
            if (m_originalVoxelContent == 0) m_originalVoxelContent = 1;

            m_actualVoxelContent = (float)m_originalVoxelContent;
            m_harvestingSpeed = m_actualVoxelContent / TIME_TO_HARVEST_WHOLE_VOXEL_IN_UPDATE_TIMES;

            if (!MyVoxelMapOreMaterials.CanBeHarvested(m_voxelMaterial))
            {
                HUD.MyHud.ShowIndestructableAsteroidNotification();
                StartReturningBack();
            }
        }

        //  When tube didn't find voxels so it's pulling back
        void StartReturningBack()
        {
            //  Head was in voxels, so we return it back to the ship
            CurrentState = MyHarvestingDeviceEnum.ReturningBack;
            m_parentMinerShip.Physics.Immovable = false;
            StopGrindingCue();
            StartTubeMovingCue();
            CloseEffect();
        }

        //  When tube collided with something so we must pull it back quickly, so player won't notice intersection
        void StartFastReturningBack()
        {
            CurrentState = MyHarvestingDeviceEnum.FastReturningBack;
            m_parentMinerShip.Physics.Immovable = false;
            MyAudio.AddCue2dOr3d(Parent, MySoundCuesEnum.VehHarvesterTubeCollision2d,
                Parent.GetPosition(), Parent.WorldMatrix.Forward, Parent.WorldMatrix.Up, Parent.Physics.LinearVelocity);
            StopGrindingCue();
            StopTubeMovingCue();
            CloseEffect();
        }

        //  When tube is inside miner ship, default status
        void StartInsideShip()
        {
            CurrentState = MyHarvestingDeviceEnum.InsideShip;
            StopTubeMovingCue();

            if (m_light != null) MyLights.RemoveLight(m_light);
            m_light = null;
        }

        //  Called when we finished harvesting current voxel
        void StartReleaseVoxel()
        {
            if (m_parentMinerShip == null || m_parentMinerShip.IsDead() || m_parentMinerShip.Inventory == null)
                return;

            StartImplodeCue();
            BoundingSphere explosion = new BoundingSphere(m_headPositionTransformed, MyVoxelConstants.VOXEL_SIZE_IN_METRES * 1);
            //remove decals
            MyDecals.HideTrianglesAfterExplosion(m_inVoxelMap, ref explosion);
            //cut off 

            var minedMaterialsWithContents = MyVoxelGenerator.CutOutSphereFastWithMaterials(m_inVoxelMap, explosion);

            var dustEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Harvester_Finished);
            dustEffect.WorldMatrix = Matrix.CreateTranslation(m_headPositionTransformed);

            bool hudHarvestingCompletePlayed = false;
            bool harvestingFinishedAsyncSent = false;

            var minedOres = new Dictionary<int, float>();

            foreach (var minedMaterialWithContent in minedMaterialsWithContents)
            {
                MyOreRatioFromVoxelMaterial[] oreFromVoxel = MyVoxelMapOreMaterials.GetOreFromVoxelMaterial(minedMaterialWithContent.Key);
                if (oreFromVoxel != null && this.Parent == MySession.PlayerShip)
                {
                    // accumulate amounts of the same type
                    foreach (MyOreRatioFromVoxelMaterial oreRatio in oreFromVoxel)
                    {
                        float amount = minedMaterialWithContent.Value * oreRatio.Ratio * MyHarvestingTubeConstants.MINED_CONTENT_RATIO;
                        float oldAmount = 0;
                        minedOres.TryGetValue((int)oreRatio.OreType, out oldAmount);
                        minedOres[(int)oreRatio.OreType] = amount + oldAmount;
                    }

                    if (!harvestingFinishedAsyncSent)
                    {
                        try
                        {
                            // Disabled, still unused on server
                            //var client = MySectorServiceClient.GetCheckedInstance();
                            //client.HarvestingFinishedAsync(minedMaterialWithContent.Key, (byte)(minedMaterialWithContent.Value * MyVoxelConstants.VOXEL_CONTENT_FULL_FLOAT));
                        }
                        catch (Exception)
                        {
                            Debug.Fail("Cannot send harvesting to server");
                        }
                        harvestingFinishedAsyncSent = true;
                    }

                    if (!hudHarvestingCompletePlayed)
                    {
                        hudHarvestingCompletePlayed = true;
                        MyAudio.AddCue2D(MySoundCuesEnum.HudHarvestingComplete);
                    }
                }
            }


            bool inventoryFullWarningPlayed = false;

            // add ores to inventory
            foreach (var ore in minedOres)
            {
                float amountLeft = m_parentMinerShip.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.Ore, ore.Key, ore.Value, false);

                float amountAdded = ore.Value - amountLeft;
                if (amountAdded > 0f)
                {
                    MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.HarvestNotification, 3500, textFormatArguments: new object[]
                        {
                            amountAdded,
                            ((MyGuiOreHelper)MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, ore.Key)).Name
                        }
                    ));
                }

                if (amountLeft > 0f)
                {
                    MyHudNotification.AddNotification(new MyHudNotification.MyNotification(MyTextsWrapperEnum.HarvestNotificationInventoryFull, MyGuiManager.GetFontMinerWarsRed(), 3500,
                        textFormatArguments: new object[]
                        {
                            amountLeft,
                            ((MyGuiOreHelper)MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.Ore, ore.Key)).Name
                        }
                    ));
                    if (!inventoryFullWarningPlayed)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryFullWarning);
                        inventoryFullWarningPlayed = true;
                    }
                }
            }

            StartReturningBack();
        }

        void StartTubeMovingCue()
        {
            if ((m_tubeMovingCue == null) || (m_tubeMovingCue.Value.IsPlaying == false))
            {
                m_tubeMovingCue = MyAudio.AddCue3D(MySoundCuesEnum.VehHarvesterTubeMovingLoop2d, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
        }

        void StartGrindingCue()
        {
            if ((m_grindingCue == null) || (m_grindingCue.Value.IsPlaying == false))
            {
                m_grindingCue = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.VehHarvesterTubeColliding2d, m_headPositionTransformed,
                    WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
        }

        void StartImplodeCue()
        {
            if ((m_implodeCue == null) || (m_implodeCue.Value.IsPlaying == false))
            {
                m_implodeCue = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.VehHarvesterTubeImplode2d, m_headPositionTransformed,
                    WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
        }

        void StopGrindingCue()
        {
            if ((m_grindingCue != null) && (m_grindingCue.Value.IsPlaying == true))
            {
                m_grindingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_grindingReleaseCue = MyAudio.AddCue2dOr3d(this, MySoundCuesEnum.VehHarvesterTubeRelease2d, m_headPositionTransformed, WorldMatrix.Forward,
                    WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }
        }

        void StopTubeMovingCue()
        {
            if ((m_tubeMovingCue != null) && (m_tubeMovingCue.Value.IsPlaying == true))
            {
                m_tubeMovingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
        }

        //  Every child of this base class must implement Shot() method, which shots projectile or missile.
        //  Method returns true if something was shot. False if not (because interval between two shots didn't pass)
        public override bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo)
        {
            if (CurrentState == MyHarvestingDeviceEnum.InVoxel)
            {
                StartReturningBack();
            }
            else if (CurrentState == MyHarvestingDeviceEnum.InsideShip)
            {
                StartFindingVoxel();
            }
            else
            {
                StartFastReturningBack();
            }

            return true;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            bool retVal = base.Draw(renderObject);

            foreach (MyEntity child in Children)
            {
                child.Draw();
            }

            return retVal;
        }

        public override bool DebugDraw()
        {
            return false;

            bool retVal = base.DebugDraw();

            if (retVal)
            {
                //MyDebugDraw.DrawLine3D(WorldMatrix.Translation, m_headPositionTransformed, Color.Red, Color.Red);

                float length = Vector3.Distance(WorldMatrix.Translation, m_headPositionTransformed);
                float scale = (length / (2 * ModelLod0.BoundingSphere.Radius));

                //float scale = GetScaleMatrix().Forward.Length();
                MyDebugDraw.DrawLine3D(WorldMatrix.Translation, WorldMatrix.Translation + length * WorldMatrix.Forward, Color.Red, Color.Red);

                MyDebugDraw.DrawSphereWireframe(Matrix.CreateScale(m_harvestingOreHead.ModelLod0.BoundingSphere.Radius) *
                    Matrix.CreateTranslation(m_headPositionTransformed), new Vector3(0, 1, 0), 0.6f);
            }

            return true;
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            if ((!GetParentMinerShip().Config.Engine.On || GetParentMinerShip().Fuel <= 0) &&
                (CurrentState == MyHarvestingDeviceEnum.InVoxel || CurrentState == MyHarvestingDeviceEnum.FindingVoxel))
            {
                StartFastReturningBack();
            }

            if (CurrentState == MyHarvestingDeviceEnum.InsideShip)
            {
                //  Do nothing
                return;
            }

            if (CurrentState == MyHarvestingDeviceEnum.FindingVoxel)
            {
                //  Head in local space
                m_headPositionLocal += this.LocalMatrix.Forward * MyHarvestingTubeConstants.EJECTION_SPEED_IN_METERS_PER_SECOND;
            }
            else if (CurrentState == MyHarvestingDeviceEnum.ReturningBack)
            {
                //  Head in local space
                m_headPositionLocal -= this.LocalMatrix.Forward * MyHarvestingTubeConstants.EJECTION_SPEED_IN_METERS_PER_SECOND;
            }
            else if (CurrentState == MyHarvestingDeviceEnum.FastReturningBack)
            {
                //  Head in local space
                m_headPositionLocal -= this.LocalMatrix.Forward * MyHarvestingTubeConstants.FAST_PULL_BACK_IN_METERS_PER_SECOND;
            }

            //  Transform head position into world space
            Matrix worldMatrix = Parent.WorldMatrix;
            m_harvestingOreHead.LocalMatrix = Matrix.CreateWorld(m_headPositionLocal, m_harvestingOreHead.LocalMatrix.Forward, m_harvestingOreHead.LocalMatrix.Up);
            m_headPositionTransformed = m_harvestingOreHead.WorldMatrix.Translation;

            //  Distance must carry sign (plus/minus) because when we need to know if head is behind it's starting point when pulling back
            //  IMPORTANT: Forward is in -Z direction, so that's why I subtract position from head and not oppositely

            float distance = this.LocalMatrix.Translation.Z - m_headPositionLocal.Z;
            if (CurrentState == MyHarvestingDeviceEnum.ReturningBack ||
                CurrentState == MyHarvestingDeviceEnum.FastReturningBack)
            {
                if (distance <= MyHarvestingTubeConstants.DISTANCE_TO_PLUG_IN_THE_TUBE)
                {
                    StartInsideShip();
                }
            }
            else
            {
                if (distance >= MyHarvestingTubeConstants.MAX_DISTANCE_OF_HARVESTING_DEVICE)
                {
                    StartReturningBack();
                }
            }

            //  Sphere for head of harvester
            BoundingSphere headSphere = new BoundingSphere(m_headPositionTransformed, m_harvestingOreHead.ModelLod0.BoundingSphere.Radius);

            if (CurrentState == MyHarvestingDeviceEnum.FindingVoxel)
            {
                //  Check if head doesn't collide with anything
                MyEntity sphereResult = MyEntities.GetIntersectionWithSphere(ref headSphere, m_parentMinerShip, null);
                if (sphereResult != null)
                {
                    if (sphereResult is MyVoxelMap)
                    {
                        StartInVoxel((MyVoxelMap)sphereResult);
                    }
                    else
                    {
                        //  Intersection between sphere and anything else but voxels, so we start pulling back quickly
                        StartFastReturningBack();
                    }
                }
            }

            //  If we are connected to voxel check permanently if the voxel is still there
            if ((CurrentState == MyHarvestingDeviceEnum.InVoxel) && (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_lastTimeCheckedForVoxelPresence) > MyHarvestingTubeConstants.INTERVAL_TO_CHECK_FOR_VOXEL_CONNECTION_IN_MILISECONDS)
            {
                m_lastTimeCheckedForVoxelPresence = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                //  Check if head doesn't collide with anything
                MyEntity sphereResult = MyEntities.GetIntersectionWithSphere(ref headSphere, m_parentMinerShip, null);
                if (!(sphereResult is MyVoxelMap))
                {
                    StartFastReturningBack();
                }
            }

            //  If we are connected to voxel we can harvest it
            if (CurrentState == MyHarvestingDeviceEnum.InVoxel)
            {
                m_actualVoxelContent -= m_harvestingSpeed;

                if (m_actualVoxelContent <= 0)
                {
                    StartReleaseVoxel();
                }
            }

            if ((WorldMatrix.Translation - m_headPositionTransformed).Length() > MyMwcMathConstants.EPSILON)
            {
                //  Check if tube doesn't colide with something
                MyLine tubeLine = new MyLine(WorldMatrix.Translation, m_headPositionTransformed, true);
                MyIntersectionResultLineTriangleEx? tubeIntersection = MyEntities.GetIntersectionWithLine(ref tubeLine, this, m_parentMinerShip);

                //  We colide with something and we need fast return back
                if (tubeIntersection != null)
                {
                    StartFastReturningBack();
                }
            }

            if (CurrentState == MyHarvestingDeviceEnum.InVoxel)
            {
                m_parentMinerShip.IncreaseHeadShake(MyHarvestingTubeConstants.SHAKE_DURING_IN_VOXELS);
            }
            else if ((CurrentState == MyHarvestingDeviceEnum.FastReturningBack) ||
                    (CurrentState == MyHarvestingDeviceEnum.FindingVoxel) ||
                    (CurrentState == MyHarvestingDeviceEnum.ReturningBack))
            {
                m_parentMinerShip.IncreaseHeadShake(MyHarvestingTubeConstants.SHAKE_DURING_EJECTION);
            }

            if (m_light != null)
            {
                m_light.SetPosition(m_headPositionTransformed - WorldMatrix.Forward);
            }

            if ((m_tubeMovingCue != null) && (m_tubeMovingCue.Value.IsPlaying == true))
            {
                MyAudio.UpdateCuePosition(m_tubeMovingCue, WorldMatrix.Translation,
                    WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }

            if ((m_grindingCue != null) && (m_grindingCue.Value.IsPlaying == true))
            {
                MyAudio.UpdateCuePosition(m_grindingCue, m_headPositionTransformed,
                    WorldMatrix.Forward, WorldMatrix.Up, Parent.Physics.LinearVelocity);
            }

            m_harvestingOreHead.SetData(ref m_worldMatrixForRenderingFromCockpitView);
        }

        //  Only for drawing this object, because some objects need to use special world matrix
        public override Matrix GetWorldMatrixForDraw()
        {
            float length = Vector3.Distance(WorldMatrix.Translation, m_headPositionTransformed);
            float scale = (length / (2 * ModelLod0.BoundingSphere.Radius));
            Matrix scaleMatrix = Matrix.CreateScale(new Vector3(1, 1, scale));

            if (NearFlag)
            {
                //  Only our player's guns are rendered using this trick. It's because we can't use WorldMatrix in large positions as 
                //  floating point inprecision is too visible if viewed from inside the ship
                return scaleMatrix * m_worldMatrixForRenderingFromCockpitView;
            }
            else
            {
                return scaleMatrix * base.GetWorldMatrixForDraw();
            }
        }

        public override void Close()
        {
            //  Free the light
            if (m_light != null)
            {
                MyLights.RemoveLight(m_light);
                m_light = null;
            }

            //  Stop sound cues
            if ((m_grindingCue != null) && (m_grindingCue.Value.IsPlaying == true))
            {
                m_grindingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_grindingReleaseCue != null) && (m_grindingReleaseCue.Value.IsPlaying == true))
            {
                m_grindingReleaseCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            if ((m_tubeMovingCue != null) && (m_tubeMovingCue.Value.IsPlaying == true))
            {
                m_tubeMovingCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }

            CloseEffect();

            base.Close();
        }

        private void CloseEffect()
        {
            if (m_harvestingParticleEffect != null)
            {
                m_harvestingParticleEffect.Stop();
                m_harvestingParticleEffect = null;
            }
        }

        public bool IsHarvesterActive
        {
            get { return CurrentState != MyHarvestingDeviceEnum.InsideShip; }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_Base objectBuilder = base.GetObjectBuilderInternal(getExactCopy);
            if (objectBuilder == null)
            {
                objectBuilder = new MyMwcObjectBuilder_SmallShip_Weapon(WeaponType = MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device);
            }
            return objectBuilder;
        }
    }
}
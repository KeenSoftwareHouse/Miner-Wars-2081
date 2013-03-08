using MinerWars.AppCode.Game.VideoMode;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System;
    using System.Text;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders;
    using GUI;
    using MinerWarsMath;
    using Models;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using System.Reflection;
    using KeenSoftwareHouse.Library.Extensions;
    using System.Diagnostics;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.Utils;

    abstract class MySmallShipGunBase : MyGunBase
    {
        public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum WeaponType { get; set; }

        public bool IsHarvester
        {
            get { return WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device; }
        }

        public bool IsDrill
        {
            get
            {
                switch (WeaponType)
                {
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser:
                    case MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure:
                        return true;
                }

                return false;
            }
        }

        public bool IsUniversalLauncher
        {
            get
            {
                return WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Front ||
                       WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Universal_Launcher_Back;
            }
        }

        public bool IsNormalWeapon
        {
            get { return !IsDrill && !IsHarvester && !IsUniversalLauncher; }
        }

        protected Matrix m_worldMatrixForRenderingFromCockpitView = Matrix.Identity;    //  This is something like world matrix for this gun, except it is in gun's object space and is used only if gun's parent is our player.

        public float YOffset;       // z-offset from ship's dummy
        public float ZOffset;       // z-offset from ship's dummy

        public void Init(StringBuilder hudLabelText, MyModelsEnum? modelEnum, MyMaterialType materialType,
            MySmallShip parent, Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_Base objectBuilder)
        {
            base.Init(hudLabelText, modelEnum, materialType, parent, position, forwardVector, upVector, objectBuilder);

            Debug.Assert(objectBuilder is MyMwcObjectBuilder_SmallShip_Weapon);
            WeaponType = ((MyMwcObjectBuilder_SmallShip_Weapon)objectBuilder).WeaponType;
            //  Don't need to check frustum, because it's checked by this gun's parent
            m_frustumCheckBeforeDrawEnabled = false;
        }

        public virtual void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_SmallShip_Weapon objectBuilder)
        {
            WeaponType = objectBuilder.WeaponType;
            //  Don't need to check frustum, because it's checked by this gun's parent
            m_frustumCheckBeforeDrawEnabled = false;
        }

        /// <summary>
        /// Called after remoting.Deserialize
        /// Remove old init version after objectBuilder is removed
        /// </summary>
        public void Init(StringBuilder hudLabelText, MySmallShip parentObject,
            Vector3 position, Vector3 forwardVector, Vector3 upVector)
        {
            Debug.Assert((int)WeaponType != 0);
            //  Don't need to check frustum, because it's checked by this gun's parent
            m_frustumCheckBeforeDrawEnabled = false;

            Init(hudLabelText, parentObject, position, forwardVector, upVector, new MyMwcObjectBuilder_SmallShip_Weapon(WeaponType));
        }

        public override void UpdateAfterSimulation()
        {
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShipGunBase.UAI base");
            base.UpdateAfterSimulation();
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShipGunBase.UAI matrix");
            if (Parent == MySession.PlayerShip)
            {
                var localMatrix = GetLocalMatrixForCockpitView();

                Matrix.Multiply(ref localMatrix, ref ((MySmallShip)Parent).PlayerHeadForGunsWorldMatrix, out m_worldMatrixForRenderingFromCockpitView);
            }
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MySmallShipGunBase.UAI muzzle pos");
            m_positionMuzzleInWorldSpace = GetMuzzlePosition(m_positionMuzzleInWorldSpace);
            //MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        protected virtual Matrix GetLocalMatrixForCockpitView()
        {
            float yOffset = MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE * YOffset;
            float zOffset = MySmallShipConstants.ALL_SMALL_SHIP_MODEL_SCALE * ZOffset;

            // in triple monitors, we must hack weapon's zOffset
            if (MyVideoModeManager.IsTripleHead())
            {
                float maxDistanceFromCenterX = 10f;
                float muFOV = (MyCamera.FieldOfView - MyConstants.FIELD_OF_VIEW_CONFIG_MIN) /
                              (MyConstants.FIELD_OF_VIEW_CONFIG_MAX_TRIPLE_HEAD - MyConstants.FIELD_OF_VIEW_CONFIG_MIN);
                float distanceFromCenterX = Math.Abs(this.LocalMatrix.Translation.X);
                float muDistanceX = (distanceFromCenterX - 0f) / (maxDistanceFromCenterX - 0f);

                zOffset = zOffset - ((1f - muDistanceX) * (1f - muFOV) * 0.9f) + muDistanceX * muFOV * 2.5f;
            }

            Matrix localMatrix = Matrix.CreateTranslation(0f, yOffset, zOffset) * this.LocalMatrix;
            return localMatrix;
        }

        public MySmallShip GetParentMinerShip()
        {
            return (MySmallShip)Parent;
        }

        //  Only for drawing this object, because some objects need to use special world matrix
        public override Matrix GetWorldMatrixForDraw()
        {
            if (MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip && GetTopMostParent() == MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip)
            {
                //  Only our player's guns are rendered using this trick. It's because we can't use WorldMatrix in large positions as 
                //  floating point inprecision is too visible if viewed from inside the ship
                return m_worldMatrixForRenderingFromCockpitView;
            }
            else
            {
                return base.GetWorldMatrixForDraw();
            }
        }

        //  Only for drawing this object, because some objects need to use special world matrix
        public Vector3 GetMuzzlePosition(Vector3 worldPosition)
        {
            if ((MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip) &&
                (Parent == MySession.PlayerShip))
            {
                //  Only our player's guns are rendered using this trick. It's because we can't use WorldMatrix in large positions as 
                //  floating point inprecision is too visible if viewed from inside the ship
                Vector3 delta = (GetWorldMatrixForDraw().Translation - WorldMatrix.Translation) + Parent.WorldMatrix.Translation;
                return worldPosition + delta;
            }
            else
            {
                return worldPosition;
            }
        }

        protected void AddWeaponCue(MySoundCuesEnum cueEnum)
        {
            GetParentMinerShip().UnifiedWeaponCueSet(cueEnum, MyAudio.AddCue2dOr3d(GetParentMinerShip(), cueEnum,
                        m_positionMuzzleInWorldSpace, Parent.WorldMatrix.Forward, Parent.WorldMatrix.Up, Parent.Physics.LinearVelocity));
        }

        protected MySoundCue? GetWeaponCue(MySoundCuesEnum cueEnum)
        {
            return GetParentMinerShip().UnifiedWeaponCueGet(cueEnum);
        }


        public override bool IsThisGunFriendly()
        {
            return ((Parent is MySmallShip) &&
                ((MySession.Static != null) && //credits
                MyFactions.GetFactionsRelation((Parent as MySmallShip), MySession.Static.Player) == MyFactionRelationEnum.Friend));
        }

        protected void AddProjectile(MyAmmoProperties ammoProperties, MyEntity weapon)
        {
            Vector3 projectileForwardVector = GetDeviatedVector(ammoProperties);

            MyProjectiles.Add(ammoProperties, Parent, m_positionMuzzleInWorldSpace, Parent.Physics.LinearVelocity, projectileForwardVector, false, 1.0f, weapon);
        }

        public virtual void StopAllSounds()
        {
        }
    }
}

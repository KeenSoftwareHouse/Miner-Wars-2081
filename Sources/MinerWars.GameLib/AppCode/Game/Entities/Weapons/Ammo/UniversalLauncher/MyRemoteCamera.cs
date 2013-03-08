using System.Diagnostics;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    internal class MyRemoteCamera : MyAmmoBase, IUniversalLauncherShell, IMyUseableEntity
    {
        private const int DISTANCE_TO_TEST_VOXEL_INTERSECTION = 50;
        // if this has value, that means the camera is sticked on some surface
        private Vector3? m_directionAfterContact;
        private Matrix m_matrixAfterContact;
        private float m_elevation, m_azimuth;

        public void Init()
        {
            Init(MyModelsEnum.RemoteCamera, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);

            // initialize gameplay properties here:
            m_gameplayProperties = MyGameplayConstants.GetGameplayProperties(MyMwcObjectBuilderTypeEnum.SmallShip_Tool,
                                                                             (int) MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.REMOTE_CAMERA,
                                                                             MyMwcObjectBuilder_FactionEnum.None);
            Debug.Assert(m_gameplayProperties != null);
            if (m_gameplayProperties != null)
            {
                MaxHealth = m_gameplayProperties.MaxHealth;
                Health = m_gameplayProperties.MaxHealth;
            }

            //Physics.RigidBody.RaiseFlag(RigidBodyFlag.RBF_COLDET_THROUGH_VOXEL_TRIANGLES);

            m_canByAffectedByExplosionForce = true;

            UseProperties = new MyUseProperties(MyUseType.Solo, MyUseType.None, MyTextsWrapperEnum.NotificationYouCanTake);
            UseProperties.Init(MyUseType.Solo, MyUseType.None, 0, 1, false);
        }

        //  This method realy initiates/starts the missile
        //  IMPORTANT: Direction vector must be normalized!
        public void Start(
            Vector3 position,
            Vector3 initialVelocity,
            Vector3 direction,
            float impulseMultiplier,
            MyEntity owner)
        {
            if (Physics.Static)
            {
                Physics.Static = false;
            }

            base.Start(position, initialVelocity, direction, impulseMultiplier, owner, MyTextsWrapper.Get(MyTextsWrapperEnum.RemoteCameraHud));

            Physics.AngularDamping = 1;
            Health = MaxHealth;
            m_directionAfterContact = null;

            var ownerShip = owner as MySmallShip;
            if (ownerShip != null)
            {
                ownerShip.Config.BackCamera.SetOn();
                ownerShip.AddRemoteCamera(this);
                ownerShip.SelectLastRemoteCamera();
            }
        }

        public override void Close()
        {
            var ownerShip = OwnerEntity as MySmallShip;
            if (ownerShip != null)
            {
                ownerShip.RemoveRemoteCamera(this);
            }
            m_directionAfterContact = null;

            base.Close();

            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }

        protected override void OnContactStart(MyContactEventInfo contactInfo)
        {
            if (!m_directionAfterContact.HasValue)
            {
                base.OnContactStart(contactInfo);

                var entityA = ((MyPhysicsBody) contactInfo.m_RigidBody1.m_UserData).Entity;
                var entityB = ((MyPhysicsBody) contactInfo.m_RigidBody2.m_UserData).Entity;

                var otherEntity = entityA == this ? entityB : entityA;

                var voxelMap = otherEntity as MyVoxelMap;

                Vector3 position;
                Vector3 normal;

                if (voxelMap != null)
                {
                    var line = new MyLine(this.GetPosition(), this.GetPosition() + DISTANCE_TO_TEST_VOXEL_INTERSECTION * this.GetForward());
                    MyIntersectionResultLineTriangleEx? intersectionResult;
                    var intersected = voxelMap.GetIntersectionWithLine(ref line, out intersectionResult);
                    //Debug.Assert(intersected);
                    //Debug.Assert(intersectionResult != null);
                    if (intersectionResult != null)
                    {
                        normal = intersectionResult.Value.NormalInWorldSpace;
                        position = intersectionResult.Value.IntersectionPointInWorldSpace - this.WorldVolume.Radius * 0.5f * normal;
                    }
                    else
                    {
                        normal = -contactInfo.m_ContactNormal;
                        position = GetPosition() + this.WorldVolume.Radius * 0.5f * normal;
                    }
                }
                else
                {
                    normal = -contactInfo.m_ContactNormal;
                    position = GetPosition() + this.WorldVolume.Radius * 0.5f * normal;
                }

                Physics.LinearVelocity = Vector3.Zero;

                Vector3 upVector;
                MyUtils.GetPerpendicularVector(ref normal, out upVector);
                m_directionAfterContact = normal;
                this.MoveAndRotate(position, Matrix.CreateWorld(position, m_directionAfterContact.Value, upVector));

                Physics.Static = true;

                m_matrixAfterContact = WorldMatrix;
                m_elevation = 0;
                m_azimuth = 0;
            }
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);

            Explode();
        }

        public override void UpdateBeforeSimulation()
        {
            if (m_elapsedMiliseconds > 1000)
            {
                Physics.GroupMask = AppCode.Physics.MyGroupMask.Empty;
            }

            if (m_isExploded)
            {
                var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Damage_Sparks);
                effect.WorldMatrix = WorldMatrix;
                effect.UserScale = .20f;
                MarkForClose();
                return;
            }

            base.UpdateBeforeSimulation();
        }

        internal void Rotate(Vector3 rotationVector)
        {
            if (CanBeRotated())
            {
                m_elevation += 0.0005f * rotationVector.X;
                m_azimuth += 0.0005f * rotationVector.Y;

                // clamp azimuth and elevation spherically
                var angleSquared = (m_azimuth * m_azimuth) + (m_elevation * m_elevation);
                if (angleSquared > MyRemoteCameraConstants.MAX_ANGLE_SQUARED)
                {
                    var normalizationFactor = MyRemoteCameraConstants.MAX_ANGLE_SQUARED / angleSquared;
                    m_elevation *= normalizationFactor;
                    m_azimuth *= normalizationFactor;
                }

                var desiredDirection = m_directionAfterContact.Value;
                desiredDirection = Vector3.Transform(desiredDirection,
                                                     Quaternion.CreateFromAxisAngle(m_matrixAfterContact.Up, m_azimuth));
                desiredDirection = Vector3.Transform(desiredDirection,
                                                     Quaternion.CreateFromAxisAngle(m_matrixAfterContact.Right, m_elevation));

                var position = GetPosition();
                Vector3 upVector = m_matrixAfterContact.Up;

                this.MoveAndRotate(position, Matrix.CreateWorld(position, desiredDirection, upVector));
            }
        }

        public static StringBuilder GetAmmoSpecialText()
        {
            if (MySession.PlayerShip != null)
            {
                if (MySession.PlayerShip.IsSelectedRemoteCamera() && !MySession.PlayerShip.HasFiredRemoteBombs())
                {
                    return new StringBuilder().AppendFormat(MyTextsWrapper.Get(MyTextsWrapperEnum.RemoteCameraRotate).ToString(), MyGuiManager.GetInput().GetGameControlTextEnum(MyGameControlEnums.CONTROL_SECONDARY_CAMERA));
                }
            }

            return null;
        }

        public MyUseProperties UseProperties { get; set; }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return usedBy == OwnerEntity;
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return false;
        }

        public bool CanBeRotated()
        {
            return m_directionAfterContact.HasValue;
        }

        public void Use(MySmallShip useBy)
        {
            // return the camera to owner's inventory
            bool addedSuccess = useBy.Inventory.AddInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int?) MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera, 1, false) == 0f;
            if (!addedSuccess) 
            {
                MyAudio.AddCue2D(MySoundCuesEnum.HudInventoryFullWarning);
            }
            this.MarkForClose();
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
        }
    }
}

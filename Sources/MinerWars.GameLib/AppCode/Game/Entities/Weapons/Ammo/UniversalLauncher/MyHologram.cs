using System;
using System.Diagnostics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Weapons.Ammo.UniversalLauncher;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.Ships.AI;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Render;

namespace MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher
{
    /// <summary>
    /// A tool which, when fired and landed, begins to project holografic copy of smallship it was fired from. It inherits world matrix, but has no collision and it can blink sometimes. It disappears after some time. It makes no sounds, but it can simulate particles/billboards too.
    /// </summary>
    class MyHologram : MyAmmoBase, IUniversalLauncherShell
    {
        private enum HologramState
        {
            Deactivated,
            Activating,
            Activated,
            Deactivating,
        }

        /// <summary>
        /// The state of the hologram shell.
        /// </summary>
        private HologramState m_hologramState;

        /// <summary>
        /// The small ship representing the holographic image.
        /// </summary>
        private MySmallShip m_hologramShip;

        /// <summary>
        /// Current scaling vector of the hologram (used in Draw()).
        /// </summary>
        private Vector3 m_scale;

        /// <summary>
        /// The base world matrix of the hologram, is scaled in draw by m_scale.
        /// After the holographic image is created, this remains unchanged.
        /// </summary>
        private Matrix m_hologramBaseMatrix;

        /// <summary>
        /// The number of milliseconds elapsed from last flicker start.
        /// </summary>
        private float m_flickerElapsedMilliseconds;

        /// <summary>
        /// The target flicker scale for one flicker occurrence.
        /// </summary>
        private Vector3 m_flickerScale;

        private List<MySmallShipBot> m_awareBots;

        private MyMwcObjectBuilder_SmallShip m_hologramBuilder = new MyMwcObjectBuilder_SmallShip(
            MyMwcObjectBuilder_SmallShip_TypesEnum.DOON,
            null, new List<MyMwcObjectBuilder_SmallShip_Weapon>(),
            null, new List<MyMwcObjectBuilder_AssignmentOfAmmo>(),
            null, null, 1, 1, 1, 1, 1, true, false, 1, 0);

        public void Init()
        {
            Init(MyModelsEnum.Hologram, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram, MyUniversalLauncherConstants.USE_SPHERE_PHYSICS);

            m_awareBots = new List<MySmallShipBot>();
        }

        public void Start(Vector3 position, Vector3 initialVelocity, Vector3 direction, float impulseMultiplier, MyEntity owner)
        {
            base.Start(position, initialVelocity, direction, impulseMultiplier, owner);

            m_hologramState = HologramState.Deactivated;

            if (!TimeToActivate.HasValue)
            {
                TimeToActivate = MyHologramConstants.TIME_TO_ACTIVATE;
            }

            foreach (var bot in MyBotCoordinator.GetBots())
            {
                if (bot.CanSeeTarget(owner))
                {
                    m_awareBots.Add(bot);
                }
            }
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            switch (m_hologramState)
            {
                case HologramState.Deactivated:
                    if (m_elapsedMiliseconds > TimeToActivate)
                    {
                        Physics.GroupMask = MyGroupMask.Empty;

                        ActivateHologram();
                    }
                    break;

                case HologramState.Activating:
                    EnlargeHologram();
                    UpdateOrientation();
                    break;

                case HologramState.Activated:
                    if (m_elapsedMiliseconds > MyHologramConstants.TIME_TO_DEACTIVATE)
                    {
                        DeactivateHologram();
                    }

                    GenerateFlicker();
                    UpdateOrientation();

                    break;

                case HologramState.Deactivating:
                    ShrinkHologram();
                    UpdateOrientation();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Sets the orientation of the hologram to the owner ship's orientation.
        /// </summary>
        private void UpdateOrientation()
        {
            m_hologramBaseMatrix.Translation = this.GetPosition();

            MyEntity owner = OwnerEntity;
            if (owner != null)
            {
                m_hologramBaseMatrix.Up = owner.WorldMatrix.Up;
                m_hologramBaseMatrix.Forward = owner.WorldMatrix.Forward;
                m_hologramBaseMatrix.Right = owner.WorldMatrix.Right;
            }
        }

        /// <summary>
        /// Prepares the hologram ship for display.
        /// </summary>
        private void ActivateHologram()
        {
            //this.Physics.Enabled = false;
            //this.Physics.LinearVelocity = Vector3.Zero;

            MyEntity owner = OwnerEntity;

            if (owner != null)
            {
                var ownerShipBuilder = (owner.GetObjectBuilder(false) as MyMwcObjectBuilder_SmallShip);
                Debug.Assert(ownerShipBuilder != null, "ownerShipBuilder != null");

                m_hologramBuilder.ShipType = ownerShipBuilder.ShipType;
                m_hologramBuilder.ShipMaxHealth = ownerShipBuilder.ShipMaxHealth;
                m_hologramBuilder.ShipHealthRatio = ownerShipBuilder.ShipHealthRatio;
                m_hologramBuilder.ArmorHealth = ownerShipBuilder.ArmorHealth;
                m_hologramBuilder.Oxygen = ownerShipBuilder.Oxygen;
                m_hologramBuilder.Fuel = ownerShipBuilder.Fuel;
                m_hologramBuilder.ReflectorLight = ownerShipBuilder.ReflectorLight;
                m_hologramBuilder.ReflectorLongRange = ownerShipBuilder.ReflectorLongRange;
                m_hologramBuilder.ReflectorShadowDistance = ownerShipBuilder.ReflectorShadowDistance;
                m_hologramBuilder.PositionAndOrientation = ownerShipBuilder.PositionAndOrientation;
                m_hologramBuilder.Faction = ownerShipBuilder.Faction;

                string displayName = ownerShipBuilder.DisplayName;
                if (MyFactions.GetFactionsRelation(m_hologramBuilder.Faction, MySession.Static.Player.Faction) == MyFactionRelationEnum.Friend || MySession.PlayerShip == owner)
                {
                    displayName = "Hologram";
                }

                m_hologramShip = MyHologramShips.Add(displayName, m_hologramBuilder);
                m_hologramShip.AIPriority = 1;
                m_hologramShip.IsHologram = true;
                if (m_hologramShip != null)
                {
                    m_hologramBaseMatrix = this.WorldMatrix;

                    m_scale = new Vector3(0, 1, 1);
                    m_hologramState = HologramState.Activating;

                    foreach (var bot in m_awareBots)
                    {
                        bot.SpoilHologram(m_hologramShip);
                    }
                }            
            }
        }

        /// <summary>
        /// Starts shrinking the hologram.
        /// </summary>
        private void DeactivateHologram()
        {
            m_hologramState = HologramState.Deactivating;
        }

        private void EnlargeHologram()
        {
            m_scale.X += MyHologramConstants.APPEAR_SPEED;
            if (m_scale.X >= 1)
            {
                m_scale = Vector3.One;
                m_hologramState = HologramState.Activated;
            }
        }

        private void ShrinkHologram()
        {
            m_scale.X -= MyHologramConstants.APPEAR_SPEED;
            if (m_scale.X <= MyMwcMathConstants.EPSILON_SQUARED)
            {
                m_scale = Vector3.Zero;
                m_hologramState = HologramState.Deactivated;
                MarkForClose();
            }
        }

        /// <summary>
        /// Once per few frames, generates a flicker.
        /// Actually only manipulates the m_scale vector in order to simulate flicker appearance.
        /// </summary>
        private void GenerateFlicker()
        {
            // TODO simon how do I make this independent of FPS?
            bool startFlickerNow = MyMwcUtils.GetRandomBool(MyHologramConstants.FLICKER_FREQUENCY);
            if (startFlickerNow && m_flickerElapsedMilliseconds > MyHologramConstants.FLICKER_DURATION)
            {
                m_flickerElapsedMilliseconds = 0;
                m_flickerScale = MyMwcUtils.GetRandomVector3Normalized();
            }

            if (m_flickerElapsedMilliseconds < MyHologramConstants.FLICKER_DURATION)
            {
                float flickerProgress = m_flickerElapsedMilliseconds / MyHologramConstants.FLICKER_DURATION;
                Vector3 maxFlickerScale = MyHologramConstants.FLICKER_MAX_SIZE * Vector3.One;
                m_scale = Vector3.Lerp(maxFlickerScale, m_flickerScale, flickerProgress);
            }
            else
            {
                m_scale = Vector3.One;
            }

            m_flickerElapsedMilliseconds += MyConstants.PHYSICS_STEP_SIZE_IN_MILLISECONDS;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            bool draw = base.Draw(renderObject);

            if (draw && m_hologramState != HologramState.Deactivated)
            {
                Vector3 maxScale = MyHologramConstants.FLICKER_MAX_SIZE * Vector3.One;
                Vector3 minScale = new Vector3(.01f);
                Vector3 clampedScale = Vector3.Clamp(m_scale, minScale, maxScale);
                m_hologramShip.SetWorldMatrix(Matrix.CreateScale(clampedScale) * m_hologramBaseMatrix);

                m_hologramShip.Draw();
            }

            return draw;
        }

        public override void Close()
        {
            if (m_hologramShip != null)
            {
                MyHologramShips.Remove(m_hologramShip);
                m_hologramShip.MarkForClose();
                m_hologramShip = null;
            }
            base.Close();
            MyUniversalLauncherShells.Remove(this, m_ownerEntityID.PlayerId);
        }

        protected override void DoDamageInternal(float playerDamage, float damage, float empDamage, MyDamageType damageType, MyAmmoType ammoType, MyEntity damageSource, bool justDeactivate)
        {
            base.DoDamageInternal(playerDamage, damage, empDamage, damageType, ammoType, damageSource, justDeactivate);
            MarkForClose();
        }
    }
}

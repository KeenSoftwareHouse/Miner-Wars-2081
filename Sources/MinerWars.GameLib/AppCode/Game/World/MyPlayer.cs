using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;

namespace MinerWars.AppCode.Game.World
{
    using System.Collections.Generic;
    using MinerWarsMath;
    using System.Reflection;
    using KeenSoftwareHouse.Library.Extensions;
    using MinerWars.AppCode.Game.Managers;
    using MinerWars.AppCode.Game.Entities;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.AppCode.Game.Missions;
    using MinerWars.AppCode.Game.HUD;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.Entities.Prefabs;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using MinerWars.AppCode.Game.Inventory;
    using KeenSoftwareHouse.Library.Trace;
    using MinerWars.AppCode.Game.Utils;
    using MinerWars.AppCode.Game.Entities.Tools;
    using MinerWars.CommonLIB.AppCode.Utils;
    using SysUtils.Utils;
    using MinerWars.AppCode.Game.Sessions;
    using MinerWars.AppCode.Networking.SectorService;
    using MinerWars.AppCode.Networking;

    delegate void TravelLeavingHandler(MyMwcTravelTypeEnum travelType);
    delegate void TravelEnterHandler();

    class MyPlayer : IMyHasFaction
    {
        #region Properties

        MyEntity m_LastDamageSource;

        /// <summary>
        /// Gets or sets Health.
        /// </summary>
        /// <value>
        /// The Health.
        /// </value>
        float m_health;
        public float Health
        {
            get
            {
                return m_health;
            }
            private set
            {
                m_health = value;

                if (m_health <= 0 && !m_wasDead)
                {
                    m_wasDead = true;
                    OnAliveChanged();
                }

                if (m_health > 0 && m_wasDead)
                {
                    m_wasDead = false;
                    OnAliveChanged();
                }
                m_LastDamageSource = null;
            }
        }

        bool m_wasDead;

        /// <summary>
        /// Raised when health is decreased under or equal to zero.
        /// Or when it is raised to positive from negative or from zero.
        /// </summary>
        public event Action<MyPlayer> AliveChanged;

        public float MaxHealth { get; set; }

        void OnAliveChanged()
        {
            var handler = AliveChanged;
            if (handler != null)
            {
                handler(this);
            }

            if (IsDead())
            {
                m_deathBeep = MyAudio.AddCue2D(MySoundCuesEnum.SfxPlayerDeathBeep);

                if (MyMultiplayerGameplay.IsRunning && MySession.PlayerShip != null)
                {
                    MyMultiplayerGameplay.Static.PilotDie(MySession.PlayerShip, m_LastDamageSource);
                }
            }
            else
            {
                if(m_deathBeep.HasValue && m_deathBeep.Value.IsPlaying)
                {
                    m_deathBeep.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
            }
        }

        MySoundCue? m_deathBeep;

        /// <summary>
        /// Gets or sets the last ship used ship.
        /// </summary>
        /// <value>
        /// The last ship.
        /// </value>
        //MySmallShip m_ship;

        public MySmallShip Ship
        {
            get
            {
                //return m_ship;
                return MySession.PlayerShip;
            }
            set
            {
                MySession.PlayerShip = value;
                //m_ship = value;
            }
        }

        float m_money;

        public float Money
        {
            get
            {
                return m_money;
            }
            set
            {
                float oldValue = m_money;
                m_money = MathHelper.Clamp(value, 0, MyPlayerConstants.MONEY_MAX);
                if (!MySteam.IsActive && MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.IsSandBox() && (m_money - oldValue) != 0)
                {
                    var client = MySectorServiceClient.GetCheckedInstance();
                    client.UpdateGameMoneyCompleted += client_UpdateGameMoneyCompleted;
                    client.UpdateGameMoneyAsync((decimal)(m_money - oldValue));
                }
            }
        }

        void client_UpdateGameMoneyCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MySectorServiceClient.SafeClose();
        }

        MyPlayerStatistics m_statistics;

        public MyPlayerStatistics Statistics
        {
            get
            {
                return m_statistics;
            }
            set
            {
                m_statistics = value;
            }
        }

        public MyMedicine[] Medicines;

        /// <summary>
        /// Time without oxygen in seconds
        /// </summary>
        public float TimeWithoutOxygen { get; set; }


        public MyMwcObjectBuilder_FactionEnum Faction { get; set; }

        #endregion

        private List<MyInventoryItem> m_heplerInventoryItems = new List<MyInventoryItem>();
        public const float DEFAULT_PLAYER_MAX_HEALTH = 100f;

        public MyPlayer()
        {
            MaxHealth = Health = DEFAULT_PLAYER_MAX_HEALTH;
            Statistics = new MyPlayerStatistics();
            Faction = MyMwcObjectBuilder_FactionEnum.Rainiers;
            Medicines = MyMedicine.GetArrayOfAllMedicines();
        }

        public event TravelEnterHandler TravelEntered;
        public event TravelLeavingHandler TravelLeaving;

        public void TravelEnter()
        {
            MyMwcLog.WriteLine("MyPlayer::TravelEnter - START");
            MyMwcLog.IncreaseIndent();

            MyTrace.Send(TraceWindow.Saving, "Player.TravelEnter()");
            UnpackAfterEnter();

            var handler = TravelEntered;
            if (handler != null)
            {
                handler();
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyPlayer::TravelEnter - END");
        }

        public void TravelLeave(MyMwcTravelTypeEnum travelType)
        {
            MyTrace.Send(TraceWindow.Saving, "Player.TravelLeave()");
            var handler = TravelLeaving;
            if (handler != null)
            {
                handler(travelType);
            }

            PackBeforeLeave(travelType);
        }

        private void UnpackAfterEnter()
        {
            UnpackMothership();
            UnpackDrones();
            MySession.PlayerFriends.UnpackFriends();

            ///
            //MySession.Static.Inventory.ClearInventoryItems();
            ///
        }


        public static Matrix FindMothershipPosition()
        {
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MyDummyPoint dummyPoint = entity as MyDummyPoint;
                if (dummyPoint != null)
                {
                    if ((dummyPoint.DummyFlags & MyDummyPointFlags.MOTHERSHIP_START) > 0)
                    {
                        return dummyPoint.WorldMatrix;
                    }
                }
            }

            return Matrix.CreateWorld(MySession.PlayerShip.GetPosition() + new Vector3(100, 100, 0), Vector3.Forward, Vector3.Up);
        }

        private static void UnpackMothership()
        {
            MyTrace.Send(TraceWindow.Saving, "Player unpacking mothership");
            MyInventoryItem item = MySession.Static.Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.PrefabContainer, null);
            if (item != null)
            {
                //place largeship
                //MyMwcObjectBuilder_LargeShip largeShipObjectBuilder = (MyMwcObjectBuilder_LargeShip)inventoryItems[0].GetObjectBuilder(true);
                MyMwcObjectBuilder_PrefabContainer containerObjectBuilder =
                    (MyMwcObjectBuilder_PrefabContainer)item.GetInventoryItemObjectBuilder(true);

                // We need to remove id's because mothership container could have same entity id in different sector
                containerObjectBuilder.RemapEntityIds(new MyEntityIdRemapContext());

                MyPrefabContainer container = new MyPrefabContainer();
                container.Init(null, containerObjectBuilder, FindMothershipPosition());

                MyEntities.Add(container);

                //CreateFromObjectBuilderAndAdd(null, largeShipObjectBuilder, Matrix.CreateTranslation(MySession.PlayerShip.GetPosition() + new Vector3(100,100,0)));
                MySession.Static.Inventory.RemoveInventoryItem(item);

                container.Link();

                MyTrace.Send(TraceWindow.Saving, "Player mothership found and unpacked");
            }
        }

        private void UnpackDrones()
        {
            MyTrace.Send(TraceWindow.Saving, "Player unpacking drones");

            m_heplerInventoryItems.Clear();
            MySession.Static.Inventory.GetInventoryItems(ref m_heplerInventoryItems, MyMwcObjectBuilderTypeEnum.Drone, null);
            foreach (var inventoryItem in m_heplerInventoryItems)
            {
                var droneObjectBuilder = (MyMwcObjectBuilder_Drone)inventoryItem.GetInventoryItemObjectBuilder(true);

                // We need to removed id's because mothership container could have same entity id in different sector
                droneObjectBuilder.EntityId = null;
                droneObjectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(
                    MySession.PlayerShip.GetPosition() + new Vector3(-100, 100, 50),
                    MySession.PlayerShip.WorldMatrix.Forward,
                    MySession.PlayerShip.WorldMatrix.Up);

                var drone = MyEntities.CreateFromObjectBuilderAndAdd("Drone", droneObjectBuilder,
                                                         droneObjectBuilder.PositionAndOrientation.GetMatrix());

                drone.Link();

                /*
                MyPrefabContainer container = new MyPrefabContainer();
                Matrix worldMatrix = Matrix.CreateTranslation(MySession.PlayerShip.GetPosition() + new Vector3(100, 100, 0));

                container.Init(null, containerObjectBuilder, worldMatrix);

                MyEntities.Add(container);

                //CreateFromObjectBuilderAndAdd(null, largeShipObjectBuilder, Matrix.CreateTranslation(MySession.PlayerShip.GetPosition() + new Vector3(100,100,0)));
                 */
                 
                MySession.Static.Inventory.RemoveInventoryItem(inventoryItem);
            }

            MyTrace.Send(TraceWindow.Saving, "Drones unpacked");
        }

        private void PackBeforeLeave(MyMwcTravelTypeEnum travelType)
        {
            if (travelType == MyMwcTravelTypeEnum.SOLAR)
            {
                PackMothership();
            }

            PackDrones();
            MySession.PlayerFriends.PackFriends();
        }


        private static void UpdateStartDummy(MyDummyPointFlags flags, Matrix position)
        {
            MyDummyPoint playerStartDummy = null;
            foreach (var entity in MyEntities.GetEntities())
            {
                MyDummyPoint dummy = entity as MyDummyPoint;
                if (dummy != null && (dummy.DummyFlags & flags) > 0)
                {
                    playerStartDummy = dummy;
                    break;
                }
            }

            if (playerStartDummy == null)
            {
                MyMwcObjectBuilder_DummyPoint dummyPointObjectBuilder =
                    MyMwcObjectBuilder_Base.CreateNewObject(MyMwcObjectBuilderTypeEnum.DummyPoint, null) as
                    MyMwcObjectBuilder_DummyPoint;
                playerStartDummy =
                    MyEntities.CreateFromObjectBuilderAndAdd(null, dummyPointObjectBuilder, Matrix.Identity) as MyDummyPoint;
                playerStartDummy.DummyFlags |= flags;
            }

            playerStartDummy.SetWorldMatrix(position);
        }

        private static void PackMothership()
        {
            MyTrace.Send(TraceWindow.Saving, "Player packing mothership");
            MyPrefabHangar hangar = MySession.Static.Player.Ship.GetNearMotherShipContainer();

            if (hangar == null)
            { //We are traveling in solar map but madelyn is far away. Assume we are travelling with Madelyn by default
                MyEntity madelynHangar;
                MyEntities.TryGetEntityByName(MyMission.MyMissionLocation.MADELYN_HANGAR, out madelynHangar);
                hangar = madelynHangar as MyPrefabHangar;
            }

            if (hangar != null)
            {
                // Move player start location dummy, so that next time player starts on same position
                UpdateStartDummy(MyDummyPointFlags.PLAYER_START, MySession.PlayerShip.WorldMatrix);

                // Move mothership start location dummy, so that next time mothership starts on same position
                UpdateStartDummy(MyDummyPointFlags.MOTHERSHIP_START, hangar.Parent.WorldMatrix);

                var container = hangar.Parent;
                MyInventoryItem item = MyInventory.CreateInventoryItemFromObjectBuilder(container.GetObjectBuilder(true));
                MySession.Static.Inventory.AddInventoryItem(item);

                container.MarkForClose();

                MyTrace.Send(TraceWindow.Saving, "Player mothership found and packed");
            }
        }

        private void PackDrones()
        {
            MyTrace.Send(TraceWindow.Saving, "Player packing drones");
            var drones = MySession.PlayerShip.Drones;
            if (drones != null && drones.Count > 0)
            {
                for (int index = drones.Count - 1; index >= 0; index--)
                {
                    var drone = drones[index];
                    var inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(drone.GetObjectBuilder(true));
                    MySession.Static.Inventory.AddInventoryItem(inventoryItem);

                    drone.MarkForClose();
                }

                MyTrace.Send(TraceWindow.Saving, "Drones found and packed");
            }
        }



        public void Init(MyMwcObjectBuilder_Player playerObjectBuilder)
        {
            System.Diagnostics.Debug.Assert(playerObjectBuilder != null);

            Health = playerObjectBuilder.Health;
            Money = playerObjectBuilder.Money;
            TimeWithoutOxygen = playerObjectBuilder.WithoutOxygen;

            if (playerObjectBuilder.PlayerStatisticsObjectBuilder != null)
            {
                Statistics.Init(playerObjectBuilder.PlayerStatisticsObjectBuilder);
            }

            if (playerObjectBuilder.ShipObjectBuilder != null)
            {
                // because we want generate playership's id after all other entities will be loaded
                //playerObjectBuilder.ShipObjectBuilder.EntityId = null;                
                Ship = MyEntities.CreateFromObjectBuilderAndAdd(null,
                    playerObjectBuilder.ShipObjectBuilder,
                    playerObjectBuilder.ShipObjectBuilder.PositionAndOrientation.GetMatrix()) as MySmallShip;

                if (playerObjectBuilder.ShipConfigObjectBuilder != null)
                {
                    Ship.Config.Init(playerObjectBuilder.ShipConfigObjectBuilder);
                }
            }

            if (MyFakes.ENABLE_REFILL_PLAYER_TO_MAX)
            {
                SetToMax();
            }
        }

        private void SetToMax()
        {
            Ship.Health = Ship.MaxHealth;
            Ship.ArmorHealth = Ship.MaxArmorHealth;
            Ship.Fuel = Ship.MaxFuel;
            Ship.Oxygen = Ship.MaxOxygen;
            Health = MaxHealth;
            List<MyInventoryItem> ammoInventoryItems = new List<MyInventoryItem>();
            Ship.Inventory.GetInventoryItems(ref ammoInventoryItems, MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, null);
            foreach (var ammoItem in ammoInventoryItems)
            {
                ammoItem.Amount = ammoItem.MaxAmount;
            }
        }

        public MyMwcObjectBuilder_Player GetObjectBuilder(bool getExactCopy)
        {
            return new MyMwcObjectBuilder_Player(Health, Money, TimeWithoutOxygen, Statistics.GetObjectBuilder(), Ship.GetObjectBuilder(getExactCopy) as MyMwcObjectBuilder_SmallShip, Ship.Config.GetObjectBuilder());
        }

        #region Methods

        public void AddHealth(float amount, MyEntity damageSource)
        {
            if (amount < 0 && MyGuiScreenGamePlay.Static.IsCheatEnabled(Gameplay.MyGameplayCheatsEnum.PLAYER_HEALTH_INDESTRUCTIBLE))
            {
                return;
            }

            m_LastDamageSource = damageSource;
            Health = Math.Min(m_health + amount, MaxHealth);
        }

        public void RestoreHealth()
        {
            Health = MaxHealth;
        }

        public bool IsDead()
        {
            return Health <= 0;
        }

        #endregion
    }
}

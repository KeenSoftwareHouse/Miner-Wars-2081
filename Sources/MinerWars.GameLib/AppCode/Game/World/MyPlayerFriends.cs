using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.HUD;
using System.Diagnostics;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWarsMath;
using MinerWars.AppCode.Physics;

namespace MinerWars.AppCode.Game.World
{
    class MyPlayerFriends
    {
        private List<MySmallShip> m_playerFriends;
        private List<MyInventoryItem> m_helperInventoryItems = new List<MyInventoryItem>();

        private Action<MyEntity> m_friendClosing;
        private MyGroupMask m_friendMask;

        public MyGroupMask FriendMask 
        {
            get { return m_friendMask; }
            set
            {
                foreach (var f in m_playerFriends)
                {
                    f.Physics.GroupMask &= ~m_friendMask;
                    f.Physics.GroupMask |= value;
                }
                m_friendMask = value;
            }
        }

        public MyPlayerFriends() 
        {
            m_friendClosing = new Action<MyEntity>(OnFriendClosing);
            m_playerFriends = new List<MySmallShip>();
        }

        /// <summary>
        /// Only use for debug!
        /// </summary>
        public List<MySmallShip> GetDebug()
        {
            return m_playerFriends;
        }

        public void Add(MySmallShip friend) 
        {
            Debug.Assert(!friend.Closed);
            if (!Contains(friend)) 
            {
                friend.OnClosing += m_friendClosing;
                friend.Physics.GroupMask |= FriendMask;
                m_playerFriends.Add(friend);                
                UpdateHUD(friend, true);                
            }
        }

        private void OnFriendClosing(MyEntity friend)
        {
            m_playerFriends.Remove(friend as MySmallShip);
            friend.Physics.GroupMask &= ~FriendMask;
            friend.OnClosing -= m_friendClosing;
        }

        public MySmallShip this[int index]
        {
            get
            {
                return m_playerFriends[index];
            }
        }

        public void Remove(MySmallShip friend) 
        {
            m_playerFriends.Remove(friend);            
            UpdateHUD(friend, false);
            friend.Physics.GroupMask &= ~FriendMask;
            friend.OnClosing -= m_friendClosing;
        }

        public bool Contains(MySmallShip friend) 
        {
            return m_playerFriends.Contains(friend);
        }

        public void Clear() 
        {
            while (m_playerFriends.Count > 0) 
            {
                Remove(m_playerFriends[0]);
            }
        }

        public int Count { get { return m_playerFriends.Count; } }

        private void UpdateHUD(MySmallShip friend, bool isFriend) 
        {
            MyHudEntityParams? gettedHudParams = MyHud.GetHudParams(friend);
            if (gettedHudParams != null)
            {
                MyHudEntityParams hudParams = gettedHudParams.Value;
                if (isFriend)
                {
                    hudParams.FlagsEnum &= ~MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE;
                    hudParams.FlagsEnum &= ~MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR;
                    hudParams.MaxDistance = 0f;
                }
                else
                {
                    hudParams.FlagsEnum |= MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE;
                    hudParams.FlagsEnum |= MyHudIndicatorFlagsEnum.SHOW_ONLY_IF_DETECTED_BY_RADAR;
                    hudParams.MaxDistance = 10000f;
                }
                MyHud.ChangeText(friend, hudParams.Text, hudParams.TargetMode, hudParams.MaxDistance, hudParams.FlagsEnum, hudParams.DisplayFactionRelation, maxDistanceMultiplerType: hudParams.MaxDistanceMultiplerType);
            }
        }

        public void PackFriends()
        {
            MyMwcLog.WriteLine("MyPlayerFriends::PackFriends - START");
            MyMwcLog.IncreaseIndent();

            while (m_playerFriends.Count > 0)
            {
                var friend = m_playerFriends[0];
                if (friend is MySmallShipBot)
                {
                    var inventoryItem = MyInventory.CreateInventoryItemFromObjectBuilder(friend.GetObjectBuilder(true));
                    MySession.Static.Inventory.AddInventoryItem(inventoryItem);
                }
                friend.MarkForClose(); // This causes remove from m_playerFriends
                m_playerFriends.Remove(friend);
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyPlayerFriends::PackFriends - END");
        }

        public void UnpackFriends()
        {
            MyMwcLog.WriteLine("MyPlayerFriends::UnpackFriends - START");
            MyMwcLog.IncreaseIndent();

            m_helperInventoryItems.Clear();
            
            MySession.Static.Inventory.GetInventoryItems(ref m_helperInventoryItems, MyMwcObjectBuilderTypeEnum.SmallShip_Bot, null);
            foreach (var inventoryItem in m_helperInventoryItems)
            {
                MyMwcObjectBuilder_SmallShip friendObjectBuilder = (MyMwcObjectBuilder_SmallShip)inventoryItem.GetInventoryItemObjectBuilder(true);

                // We need to removed id's because mothership container could have same entity id in different sector
                friendObjectBuilder.EntityId = null;
                friendObjectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(
                    MySession.PlayerShip.GetPosition() + new Vector3(-100, 100, 50),
                    MySession.PlayerShip.WorldMatrix.Forward,
                    MySession.PlayerShip.WorldMatrix.Up);

                var friend = (MySmallShipBot)MyEntities.CreateFromObjectBuilderAndAdd(friendObjectBuilder.DisplayName, friendObjectBuilder,
                                                         friendObjectBuilder.PositionAndOrientation.GetMatrix());

                friend.Activate(true, false);
                friend.SpeedModifier = 1.0f;
                friend.Save = false;
                Add(friend);
                friend.Follow(MySession.PlayerShip);

                MySession.Static.Inventory.RemoveInventoryItem(inventoryItem);
            }
            
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyPlayerFriends::UnpackFriends - END");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;
using MinerWars.Resources;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Prefabs
{
    enum MyConnectEntityOperation
    {
        Success,
        NotExists,
        NotSupported,
        AlreadyConnected,
    }

    class MyPrefabSecurityControlHUB : MyPrefabBase, IMyUseableEntity
    {
        private static Dictionary<int, List<MySoundCuesEnum>> m_radioChattersForFactions;

        static MyPrefabSecurityControlHUB() 
        {
            m_radioChattersForFactions = new Dictionary<int, List<MySoundCuesEnum>>();
            var russianChatters = new List<MySoundCuesEnum>() { MySoundCuesEnum.Amb3D_RadioChatterRussian01_04, MySoundCuesEnum.Amb3D_RadioChatterRussian05_08, MySoundCuesEnum.Amb3D_RadioChatterRussian09_12, MySoundCuesEnum.Amb3D_RadioChatterRussian13_16 };
            var chineseChatters = new List<MySoundCuesEnum>() { MySoundCuesEnum.Amb3D_RadioChatterChinese01_04, MySoundCuesEnum.Amb3D_RadioChatterChinese05_08, MySoundCuesEnum.Amb3D_RadioChatterChinese09_12, MySoundCuesEnum.Amb3D_RadioChatterChinese13_16 };
            var alliedChatters = new List<MySoundCuesEnum>() { MySoundCuesEnum.Amb3D_RadioChatterAllied01_04, MySoundCuesEnum.Amb3D_RadioChatterAllied05_08, MySoundCuesEnum.Amb3D_RadioChatterAllied09_12, MySoundCuesEnum.Amb3D_RadioChatterAllied13_16 };
            foreach (MyMwcObjectBuilder_FactionEnum faction in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum))) 
            {
                if(faction == MyMwcObjectBuilder_FactionEnum.China || 
                   faction == MyMwcObjectBuilder_FactionEnum.Japan || 
                   faction == MyMwcObjectBuilder_FactionEnum.FreeAsia)
                {
                    m_radioChattersForFactions.Add((int)faction, chineseChatters);
                }
                else if(faction == MyMwcObjectBuilder_FactionEnum.Russian || 
                   faction == MyMwcObjectBuilder_FactionEnum.Russian_KGB || 
                   faction == MyMwcObjectBuilder_FactionEnum.CSR)
                {
                    m_radioChattersForFactions.Add((int)faction, russianChatters);
                } 
                else
                {
                    m_radioChattersForFactions.Add((int)faction, alliedChatters);
                }                
            }
        }

        //const int NUMBER_OF_CHATTER_SOUNDS_PER_FACTION = 4;
        const int MAX_CHATTER_DISTANCE = 300;
        const float MAX_CHATTER_DISTANCE_SQUARED = MAX_CHATTER_DISTANCE * MAX_CHATTER_DISTANCE;

        //private List<uint> m_connetectedEntitiesIds = new List<uint>();
        private List<IMyUseableEntity> m_connectedEntities = new List<IMyUseableEntity>();
        MySoundCue? chatterCue;
        private Action<MyEntity> m_onEntityClosingHandler;

        public event Action<IMyUseableEntity> OnEntityConnected;
        public event Action<IMyUseableEntity> OnEntityDisconnected;

        public MyPrefabSecurityControlHUB(MyPrefabContainer owner)
            : base(owner)
        {
            m_onEntityClosingHandler = OnEntityClosingHandler;
        }

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            MyMwcObjectBuilder_PrefabSecurityControlHUB objectBuilderSecurityControlHUB = (MyMwcObjectBuilder_PrefabSecurityControlHUB) objectBuilder;
            //m_connetectedEntitiesIds.AddRange(objectBuilderSecurityControlHUB.ConnectedEntities);

            UseProperties = new MyUseProperties(MyUseType.Solo, MyUseType.Solo);
            if (objectBuilderSecurityControlHUB.UseProperties == null)
            {
                UseProperties.Init(MyUseType.Solo, MyUseType.Solo, 1, 4000, false);
            }
            else
            {
                UseProperties.Init(objectBuilderSecurityControlHUB.UseProperties);
            }

            UpdateSound();

            m_needsUpdate = true;
        }

        public override void Link()
        {
            base.Link();
            MyMwcObjectBuilder_PrefabSecurityControlHUB hubBuilder = base.GetObjectBuilderInternal(true) as MyMwcObjectBuilder_PrefabSecurityControlHUB;
            foreach (uint connectedEntityId in hubBuilder.ConnectedEntities)
            {
                ConnectEntity(connectedEntityId);
            }
        }

        private void OnEntityClosingHandler(MyEntity entity) 
        {
            Debug.Assert(entity.EntityId != null);
            DisconnetEntity(entity.EntityId.Value.NumericValue);
        }        

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);

            if (chatterCue.HasValue && chatterCue.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(chatterCue, this.GetPosition(), this.GetForward(), this.GetUp(), Vector3.Zero);
            }
        }

        void UpdateSound()
        {
            var distanceSquared = Vector3.DistanceSquared(MyCamera.Position, this.GetPosition());
            bool closeEnough = distanceSquared < MAX_CHATTER_DISTANCE_SQUARED;
            if (IsWorking() && closeEnough)
            {
                if (!chatterCue.HasValue || !chatterCue.Value.IsPlaying)
                {
                    var soundCueEnum = GetRandomChatterCueForMyFaction();
                    chatterCue = MyAudio.AddCue3D(
                        soundCueEnum,
                        this.GetPosition(),
                        this.GetForward(),
                        this.GetUp(),
                        Vector3.Zero);
                }
            }
            else
            {
                StopChatterCue();
            }
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            UpdateSound();
        }

        MySoundCuesEnum GetRandomChatterCueForMyFaction()
        {            
            List<MySoundCuesEnum> chattersForFaction = m_radioChattersForFactions[(int)Faction];
            return chattersForFaction[MyMwcUtils.GetRandomInt(0, chattersForFaction.Count - 1)];
        }


        public override string GetCorrectDisplayName()
        {
            string displayName = DisplayName;

            if (DisplayName == "Camera HUB")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.CameraHUB).ToString();
            }

            if (DisplayName == "Weapon Control")
            {
                displayName = MyTextsWrapper.Get(MyTextsWrapperEnum.WeaponControl).ToString();
            }

                  
            return displayName;
        }

        protected override void SetHudMarker()
        {

            MyHud.ChangeText(this, new StringBuilder(GetCorrectDisplayName()), MyGuitargetMode.Neutral, 0, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_BORDER_INDICATORS | MyHudIndicatorFlagsEnum.SHOW_HEALTH_BARS | MyHudIndicatorFlagsEnum.SHOW_DISTANCE | MyHudIndicatorFlagsEnum.ALPHA_CORRECTION_BY_DISTANCE | MyHudIndicatorFlagsEnum.SHOW_MISSION_MARKER);            
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            var objectBuilder = (MyMwcObjectBuilder_PrefabSecurityControlHUB) base.GetObjectBuilderInternal(getExactCopy);

            if (objectBuilder.ConnectedEntities == null)
            {
                objectBuilder.ConnectedEntities = new List<uint>();
            }
            else 
            { 
                objectBuilder.ConnectedEntities.Clear(); 
            }
            foreach (IMyUseableEntity connectedEntity in m_connectedEntities) 
            {
                MyEntity entity = connectedEntity as MyEntity;
                Debug.Assert(entity != null);
                Debug.Assert(entity.EntityId != null);
                objectBuilder.ConnectedEntities.Add(entity.EntityId.Value.NumericValue);
            }
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();

            return objectBuilder;
        }

        public List<IMyUseableEntity> ConnectedEntities
        {
            get { return m_connectedEntities; }
        }

        public MyConnectEntityOperation ConnectEntity(uint entityId)
        {
            MyConnectEntityOperation result;

            MyEntity entityById = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(entityId));
            if (entityById == null)
            {
                result = MyConnectEntityOperation.NotExists;
            }
            else
            {
                IMyUseableEntity connectableEntity = entityById as IMyUseableEntity;
                if (connectableEntity == null || (connectableEntity.UseProperties.UseType & MyUseType.FromHUB) == 0 || !(connectableEntity is IMyHasGuiControl))
                {
                    result = MyConnectEntityOperation.NotSupported;
                }
                else
                {
                    if (m_connectedEntities.Contains(connectableEntity))
                    {
                        result = MyConnectEntityOperation.AlreadyConnected;
                    }
                    else
                    {
                        entityById.OnClosing += m_onEntityClosingHandler;
                        m_connectedEntities.Add(connectableEntity);
                        result = MyConnectEntityOperation.Success;

                        if (OnEntityConnected != null) 
                        {
                            OnEntityConnected(connectableEntity);
                        }
                    }
                }
            }

            return result;
        }

        public MyConnectEntityOperation DisconnetEntity(uint entityId) 
        {
            MyEntity entity = MyEntities.GetEntityByIdOrNull(new MyEntityIdentifier(entityId));
            Debug.Assert(entity != null);
            IMyUseableEntity useableEntity = entity as IMyUseableEntity;
            Debug.Assert(useableEntity != null);
            if (entity != null)
            {
                entity.OnClosing -= m_onEntityClosingHandler;
            }
            bool removed = m_connectedEntities.Remove(useableEntity);
            
            if (removed && OnEntityDisconnected != null) 
            {
                OnEntityDisconnected(useableEntity);
            }

            var result = removed ? MyConnectEntityOperation.Success : MyConnectEntityOperation.NotExists;

            return result;
        }

        public void DisconnectAllEntities() 
        {
            while (m_connectedEntities.Count > 0) 
            {
                MyEntity entity = m_connectedEntities[0] as MyEntity;
                Debug.Assert(entity.EntityId != null);
                DisconnetEntity(entity.EntityId.Value.NumericValue);
            }
            Debug.Assert(m_connectedEntities.Count == 0);
        }

        public MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum PrefabSecurityControlHUBType
        {
            get
            {
                return (MyMwcObjectBuilder_PrefabSecurityControlHUB_TypesEnum)m_prefabId;
            }
            set
            {
                m_prefabId = (int)value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefabSecurityControlHUB";
        }

        public void Use(MySmallShip useBy)
        {
            //if (ConnectedEntityIds.Any())
            //{
            //    MyGuiManager.AddScreen(new MyGuiScreenSecurityControlHUB(useBy, this));
            //}
            var screenSecurityControlHUB = new MyGuiScreenSecurityControlHUB(useBy, this);
            MyGuiScreenGamePlay.Static.DisplaySecurityHubScreen(screenSecurityControlHUB);
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference) 
        {
            if (ConnectedEntities.Count > 0)
            {
                Use(useBy);
            }
        }

        public bool CanBeUsed(MySmallShip usedBy)
        {
            return (ConnectedEntities.Count > 0) && IsWorking() && (MyFactions.GetFactionsRelation(usedBy, this) == MyFactionRelationEnum.Friend || UseProperties.IsHacked);
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return IsWorking() && (MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Neutral ||
                MyFactions.GetFactionsRelation(hackedBy, this) == MyFactionRelationEnum.Enemy);
        }

        public MyUseProperties UseProperties { get; set; }

        protected override void WorkingChanged()
        {
            base.WorkingChanged();
            UpdateSound();
        }

        public override void Close()
        {
            StopChatterCue();
            DisconnectAllEntities();
            base.Close();
        }

        void StopChatterCue()
        {
            if (chatterCue.HasValue && chatterCue.Value.IsPlaying)
            {
                chatterCue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }
        }
    }
}

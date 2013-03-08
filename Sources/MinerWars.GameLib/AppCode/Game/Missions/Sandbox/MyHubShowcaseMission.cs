using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Collections.Generic;
using System.Linq;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Audio;
using System.Diagnostics;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Missions.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Missions.Sandbox
{
    class MyHubShowcaseMission : MyMission
    {
        public static readonly MyMwcVector3Int BaseSector = new MyMwcVector3Int(3, 0, 9);

        public MyHubShowcaseMission()
        {
            ID = 0; // Lets try zero for sandbox
            DebugName = new StringBuilder("Hub showcase");
            Name = Localization.MyTextsWrapperEnum.HUB_SHOWCASE;
            Name = Localization.MyTextsWrapperEnum.HUB_SHOWCASE_Description;

            /* sector where the mission is located */
            Location = new MyMissionLocation(BaseSector, 222); // Posledne cislo - ID dummy pointu kde prijimam misiu (v tomto pripade tiez 'player start')

            RequiredActors = new MyActorEnum[] { MyActorEnum.MARCUS, MyActorEnum.MADELYN };
            m_objectives = new List<MyObjective>();
        }

        private MySmallShipBot m_Slave;

        public override void Load()
        {
            base.Load();

            MyScriptWrapper.AlarmLaunched += MyScriptWrapper_AlarmLaunched;

            // Turn off sunwind, meteorwind and icestorm
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.SunWind, false);
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.MeteorWind, false);
            MyScriptWrapper.EnableGlobalEvent(World.Global.MyGlobalEventEnum.IceStorm, false);

            // Add Fourth Reich FalseId to inventory if player already haven't got one
            MyScriptWrapper.AddFalseIdToPlayersInventory(MyMwcObjectBuilder_FactionEnum.FourthReich);

            // Add Radar Jammer to player inventory
            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetPlayerInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER, 1f);

            // Add Hacking Tool to player inventory
            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_2, 1f, true);
            MyScriptWrapper.AddInventoryItem(MyScriptWrapper.GetCentralInventory(), MyMwcObjectBuilderTypeEnum.SmallShip_HackingTool, (int)MyMwcObjectBuilder_SmallShip_HackingTool_TypesEnum.Level_3, 1f, true);

            // Set musicmood right from script start
            MyAudio.ApplyTransition(MyMusicTransitionEnum.TensionBeforeAnAction);

            m_Slave = (MySmallShipBot)MyScriptWrapper.TryGetEntity(328);
            MySession.PlayerFriends.Add(m_Slave);
            m_Slave.Follow(MySession.PlayerShip);
            m_Slave.SetName("Slave");
        }

        void MyScriptWrapper_AlarmLaunched(MyEntity prefabContainer, MyEntity enemy)
        {
            if (prefabContainer.EntityId != null && prefabContainer.EntityId.Value.NumericValue == 155) 
            {
                MyScriptWrapper.ActivateSpawnPoint(153);
            }
        }

        public override void Unload()
        {
            base.Unload();
            MyScriptWrapper.AlarmLaunched -= MyScriptWrapper_AlarmLaunched;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.AppCode.Game.Journal;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWars.AppCode.Game.Gameplay;

namespace MinerWars.AppCode.Game.Managers.Session
{
    using MinerWars.AppCode.App;
    using MinerWars.AppCode.Game.Missions;
    using System.Reflection;
    using KeenSoftwareHouse.Library.Extensions;
    using MinerWars.AppCode.Game.GUI.Core;
    using MinerWars.AppCode.Game.GUI;
    using MinerWars.AppCode.Game.World.Global;


    public enum MySingleplayeSessionState
    {
        Init,
        ChooseDifficulty,
        Running
    }

    /// <summary>
    /// Represent single player game session
    /// </summary>
    internal sealed class MySinglePlayerSession : MySession
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySession"/> class.
        /// </summary>
        public MySinglePlayerSession(MyGameplayDifficultyEnum difficulty)
        {
            MyGameplayConstants.SetGameplayDifficulty(difficulty);
        }
     
        #region Overrides of MySession


        protected override MyMwcObjectBuilder_Session GetObjectBuilder()
        {
            return new MyMwcObjectBuilder_Session(MyGameplayConstants.GameplayDifficultyProfile.GameplayDifficulty);
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        #endregion

        public override void Init()
        {
            base.Init();

            MyMissions.Unload();
            if (MyMissions.ActiveMission != null)
                MyMissions.ActiveMission.Load();

            EventLog = new MyEventLog();
            MyMissions.RefreshAvailableMissions();
        }
    }
}

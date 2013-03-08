using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.Radar;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.GUI;
using System.Diagnostics;
using MinerWars.AppCode.Game.Entities.SubObjects;


namespace MinerWars.AppCode.Game.Audio.Dialogues
{
    static class MyShouts 
    {
        #region Constants
        public const int INTERVAL_BETWEEN_SHOUTS_MIN = 10000; //ms
        public const int INTERVAL_BETWEEN_SHOUTS_MAX = 15000; //ms        
        #endregion

        #region Static members        
        private static MySoundCuesEnum? m_lastShoutCue;
        private static MySoundCue? m_actualShoutCue;
        private static MyMwcObjectBuilder_FactionEnum? m_actualShoutFaction;
        private static Dictionary<int, List<MySoundCuesEnum>> m_shouts = null;
        private static int m_nextShoutTime = 0;

        private static List<MyMwcObjectBuilder_FactionEnum> m_detectedFactions = new List<MyMwcObjectBuilder_FactionEnum>();
        private static List<IMyObjectToDetect> m_detectedEntities = new List<IMyObjectToDetect>();
        #endregion

        public static void LoadData()
        {
            if (m_shouts == null)
            {
                m_shouts = new Dictionary<int, List<MySoundCuesEnum>>();

                // china
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH01);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH02);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH03);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH04);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH05);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH06);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH07);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH08);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH09);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH10);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH11);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH12);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH13);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH14);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH15);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.China, MySoundCuesEnum.Amb2D_RadioChatterCH16);

                // russian
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS01);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS02);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS03);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS04);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS05);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS06);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS07);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS08);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS09);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS10);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS11);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS12);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS13);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS14);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS15);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian, MySoundCuesEnum.Amb2D_RadioChatterRS16);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS01);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS02);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS03);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS04);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS05);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS06);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS07);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS08);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS09);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS10);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS11);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS12);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS13);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS14);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS15);
                AddActorsShouts(MyMwcObjectBuilder_FactionEnum.Russian_KGB, MySoundCuesEnum.Amb2D_RadioChatterRS16);

                // shared for all other factions
                foreach (MyMwcObjectBuilder_FactionEnum faction in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
                {
                    if (faction == MyMwcObjectBuilder_FactionEnum.China ||
                        faction == MyMwcObjectBuilder_FactionEnum.Russian ||
                        faction == MyMwcObjectBuilder_FactionEnum.Russian_KGB)
                    {
                        continue;
                    }
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL01);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL02);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL03);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL04);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL05);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL06);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL07);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL08);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL09);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL10);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL11);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL12);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL13);
                    AddActorsShouts(faction, MySoundCuesEnum.Amb2D_RadioChatterAL14);
                }
            }
        }

        public static void UnloadData() 
        {
            m_detectedEntities.Clear();
            m_detectedFactions.Clear();
            m_lastShoutCue = null;
            m_nextShoutTime = 0;
            StopShout();
        }

        public static void Update() 
        {
            Debug.Assert(MyFakes.ENABLE_SHOUT);            
            m_detectedEntities.Clear();
            m_detectedFactions.Clear();
            if (MySession.PlayerShip == null) return;

            if (MyDialogues.CurrentSentence != null && !MyDialogues.IsCurrentCuePausedAndHidden())
            {
                StopShout();
                return;
            }

            MyRadar.GetDetectedBotsAndLargeWeapons(ref m_detectedEntities);            
            foreach(IMyObjectToDetect detectedObject in m_detectedEntities)
            {
                // check if detected object can shout
                if (CanShout(detectedObject))
                {
                    MyMwcObjectBuilder_FactionEnum factionForShout = GetFactionForShout((MyEntity)detectedObject);
                    // we want add only factions which have any shout
                    if (HasFactionShouts(factionForShout))
                    {
                        // add this faction as detected                            
                        if (!m_detectedFactions.Contains(factionForShout))
                        {
                            m_detectedFactions.Add(factionForShout);
                        }
                    }
                }
            }
            UpdateShouts();
        }

        private static void AddActorsShouts(MyMwcObjectBuilder_FactionEnum faction, MySoundCuesEnum shoutCue)
        {
            if (!m_shouts.ContainsKey((int)faction))
            {
                m_shouts.Add((int)faction, new List<MySoundCuesEnum>());
            }

            m_shouts[(int)faction].Add(shoutCue);
        }

        private static List<MySoundCuesEnum> GetShouts(MyMwcObjectBuilder_FactionEnum faction)
        {
            if (m_shouts.ContainsKey((int)faction))
            {
                return m_shouts[(int)faction];
            }
            return null;
        }

        private static void StopShout() 
        {
            if (m_actualShoutCue != null && m_actualShoutCue.Value.IsPlaying)
            {
                m_actualShoutCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                m_actualShoutCue = null;
                m_actualShoutFaction = null;
            }
        }

        private static bool CanShout(IMyObjectToDetect detectedObject) 
        {
            MyEntity detectedEntity = detectedObject as MyEntity;

            if (detectedEntity != null)
            {
                MyFactionRelationEnum factionRelation = MyFactions.GetFactionsRelation(MySession.PlayerShip, detectedEntity);                
                if (detectedEntity is MySmallShipBot && factionRelation != MyFactionRelationEnum.Friend) 
                {
                    MySmallShipBot bot = detectedEntity as MySmallShipBot;
                    return !bot.IsParked() && !bot.IsPilotDead();
                }
                if (detectedEntity is MyPrefabLargeWeapon && factionRelation == MyFactionRelationEnum.Enemy) 
                {
                    MyPrefabLargeWeapon largeWeapon = detectedEntity as MyPrefabLargeWeapon;
                    return largeWeapon.IsWorking();
                }
            }
            return false;
        }

        private static bool HasFactionShouts(MyMwcObjectBuilder_FactionEnum faction) 
        {
            int factionKey = (int)faction;
            List<MySoundCuesEnum> factionShouts = GetShouts(faction);
            return factionShouts != null && factionShouts.Count > 0;
        }

        private static void UpdateShouts() 
        {
            // check for new shout to start
            if (m_actualShoutCue == null || !m_actualShoutCue.Value.IsPlaying)
            {
                // check if we can start new shout
                if (m_nextShoutTime < MyMinerGame.TotalGamePlayTimeInMilliseconds)
                {
                    // gets random faction to shout (from detected factions by radar)
                    MyMwcObjectBuilder_FactionEnum? factionToShout = GetRandomDetectedFaction();
                    if (factionToShout != null)
                    {
                        MySoundCuesEnum shoutCue = GetRandomShout(factionToShout.Value);
                        m_actualShoutCue = MyAudio.AddCue2D(shoutCue);
                        if (m_actualShoutCue != null)
                        {
                            // because shouts can be ambient cues, so we must set correct volume for these cues
                            if (m_actualShoutCue.Value.IsAmbientSound)
                            {
                                MyAudio.UpdateCueAmbVolume(m_actualShoutCue, 100f);
                            }
                            else 
                            {
                                MyAudio.UpdateCueVolume(m_actualShoutCue, 0.9f);
                            }
                            m_actualShoutFaction = factionToShout;
                            m_lastShoutCue = shoutCue;

                            // calculate new time for next shout
                            m_nextShoutTime = MyMinerGame.TotalGamePlayTimeInMilliseconds + MyMwcUtils.GetRandomInt(INTERVAL_BETWEEN_SHOUTS_MIN, INTERVAL_BETWEEN_SHOUTS_MAX);                            
                        }
                    }
                }
            }
            // check for old shout to stop
            else 
            {
                if (!m_detectedFactions.Contains(m_actualShoutFaction.Value))
                {
                    StopShout();
                }
            }
        }

        private static MyMwcObjectBuilder_FactionEnum? GetRandomDetectedFaction() 
        {
            MyMwcObjectBuilder_FactionEnum? randomDetectedFaction = null;
            if (m_detectedFactions.Count > 0) 
            {
                int randomFactionIndex = MyMwcUtils.GetRandomInt(m_detectedFactions.Count - 1);
                randomDetectedFaction = m_detectedFactions[randomFactionIndex];              
            }
            return randomDetectedFaction;
        }        

        private static MySoundCuesEnum GetRandomShout(MyMwcObjectBuilder_FactionEnum faction)
        {
            int index;
            List<MySoundCuesEnum> shouts = GetShouts(faction);            
            do
            {
                index = MyMwcUtils.GetRandomInt(shouts.Count);
            } while (m_lastShoutCue.HasValue && shouts.Count > 1 && m_lastShoutCue.Value == shouts[index]);

            return shouts[index];
        }

        private static MyMwcObjectBuilder_FactionEnum GetFactionForShout(MyEntity entity)
        {
            if (MyFakes.SHOUTS_PREFERED_FACTION.HasValue)
            {
                return MyFakes.SHOUTS_PREFERED_FACTION.Value;
            }
            else
            {
                return entity.Faction;
            }
        }
    }    
}

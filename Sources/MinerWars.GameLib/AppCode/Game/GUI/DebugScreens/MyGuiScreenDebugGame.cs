#region Using
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.Ships.SubObjects;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Gameplay;
using SysUtils.Utils;

using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Audio.Dialogues;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI.ScreenEditor.Object3D;
using MinerWars.CommonLIB.AppCode.Import;

#endregion

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugGame : MyGuiScreenDebugBase
    {
        List<MyEntity> m_debugFloatingObjects = new List<MyEntity>();
        List<MyEntity> m_debugStaticObjects = new List<MyEntity>();
        List<MyEntity> m_debugStaticAsteroids = new List<MyEntity>();
        List<MyEntity> m_debugPrefabModels = new List<MyEntity>();
        List<MyEntity> m_debugShips = new List<MyEntity>();
        List<MyEntity> m_debugWeapons = new List<MyEntity>();
        List<MyEntity> m_debugItems = new List<MyEntity>();
        List<MyEntity> m_debugPrefabEntities = new List<MyEntity>();

        public MyGuiScreenDebugGame()
            : base(0.35f * Color.Yellow.ToVector4(), false)
        {
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            m_isTopMostScreen = false;
            m_canHaveFocus = false;

            RecreateControls(true);
        }

        public override void RecreateControls(bool contructor)
        {
            Controls.Clear();

            m_scale = 0.65f;

            AddCaption(new System.Text.StringBuilder("Game debug"), Color.Yellow.ToVector4());

            
            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);


            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);


            var profile = MyRenderConstants.RenderQualityProfile; // Obfuscated MemberHelper can't access property, so store it to field

            AddLabel(new StringBuilder("GDC"), Color.Yellow.ToVector4(), 1.2f);
            AddSlider(new StringBuilder("Voxel debris count"), 0.0f, 50.0f, profile, MemberHelper.GetMember(() => profile.ExplosionDebrisCountMultiplier));
            AddSlider(new StringBuilder("Prefab damage multiplier"), 0.01f, 100.0f, null, MemberHelper.GetMember(() => MyPrefabBase.PrefabDamageMultiplier));
         
            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Game"), Color.Yellow.ToVector4(), 1.2f);

            AddCheckBox(new StringBuilder("Show ship trusts"), null, MemberHelper.GetMember(() => MySmallShip.DebugDrawEngineThrusts)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show ship weapons"), null, MemberHelper.GetMember(() => MySmallShip.DebugDrawEngineWeapons)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show HUD"), null, MemberHelper.GetMember(() => MyHud.Visible)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show debug HUD"), null, MemberHelper.GetMember(() => MyHud.ShowDebugHud)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Player simulation"), null, MemberHelper.GetMember(() => MySmallShip.PlayerSimulation)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Trichording"), null, MemberHelper.GetMember(() => MyGuiInput.Trichording)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show waypoints"), MyHud.ShowDebugWaypoints, ShowWayPoints); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Highlight obstructed waypoint edges"), null, MemberHelper.GetMember(() => MyHud.ShowDebugWaypointsCollisions)); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show generated waypoints"), null, MemberHelper.GetMember(() => MyHud.ShowDebugGeneratedWaypoints)); m_currentPosition.Y -= 0.0025f;
            AddButton(new StringBuilder("Load last checkpoint"), LoadLastCheckpoint);
            AddButton(new StringBuilder("Debug dialogues"), OnDebugDialogues);

            m_currentPosition.Y += 0.005f;

            AddButton(new StringBuilder("Shorten ship health"), OnShortenShipHealth);
            AddButton(new StringBuilder("Shorten player health"), OnShortenPlayerHealth);
            AddButton(new StringBuilder("Shorten armor"), OnShortenArmor);
            AddButton(new StringBuilder("Shorten fuel"), OnShortenFuel);
            AddButton(new StringBuilder("Shorten oxygen"), OnShortenOxygen);

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Models debug"), Color.Yellow.ToVector4(), 1.2f);

            AddCheckBox(new StringBuilder("Show floating objects"), m_debugFloatingObjects.Count > 0, ShowFloatingObjectsChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show static objects"), m_debugStaticObjects.Count > 0, ShowStaticObjectsChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show static asteroids"), m_debugStaticAsteroids.Count > 0, ShowStaticAsteroidsChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show prefab models"), m_debugPrefabModels.Count > 0, ShowPrefabModelsChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show ships"), m_debugShips.Count > 0, ShowShipsChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show weapons"), m_debugWeapons.Count > 0, ShowWeaponsChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show debugs"), m_debugItems.Count > 0, ShowDebugChange); m_currentPosition.Y -= 0.0025f;
            AddCheckBox(new StringBuilder("Show prefabs as entities"), m_debugPrefabEntities.Count > 0, ShowPrefabEntitiesChange); m_currentPosition.Y -= 0.0025f;


            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Output"), Color.Yellow.ToVector4(), 1.2f);

            AddButton(new StringBuilder("Generate list of assets"), OnGenerateListOfAssets);
            AddButton(new StringBuilder("Generate list of dialogues"), OnGenerateListOfDialogues);
            AddButton(new StringBuilder("Gen.list of unused dialogues"), OnGenerateListOfUnusedDialogues);
            AddButton(new StringBuilder("Generate list of missions"), OnGenerateListOfMissions);
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugGame";
        }

        private void SimulateLostMessagesChange(MyGuiControlCheckbox sender)
        {
            MyMwcFinalBuildConstants.SIMULATE_LOST_MESSAGES_SENT_OUT = sender.Checked ? (int?)4 : null;
        }

        public class MyDebugDebris : MySmallDebris
        {
            public void Init(StringBuilder hudLabelText, Models.MyModelsEnum? modelLod0Enum, Models.MyModelsEnum? modelLod1Enum, MyMeshDrawTechnique? drawTechnique = null)
            {
                base.Init(hudLabelText, modelLod0Enum, null, null, null, null);

                this.Name = hudLabelText != null ? hudLabelText.ToString() : null;

                if (this.Name != null)
                {
                    MinerWars.AppCode.Game.HUD.MyHud.ChangeText(this, new StringBuilder(this.Name));
                }
              
                this.WorldMatrix = Matrix.Identity;

                if (drawTechnique != null)
                    InitDrawTechniques(drawTechnique.Value); 
            }

            public override bool Draw(MyRenderObject renderObject)
            {
                return base.Draw(renderObject);
            }
        }

        private void ShowFloatingObjectsChange(MyGuiControlCheckbox sender)
        {
            ShowModelsChange(sender, m_debugFloatingObjects, "Models2\\ObjectsFloating");
        }

        private void ShowStaticObjectsChange(MyGuiControlCheckbox sender)
        {
            ShowModelsChange(sender, m_debugStaticObjects, "Models2\\ObjectsStatic");
        }

        private void ShowStaticAsteroidsChange(MyGuiControlCheckbox sender)
        {
            List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> statEnumsA = new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>()
            {
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_A,
            };

            List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> statEnumsB = new List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum>()
            {
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_B,
               MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_B,
            };

            ShowModelsChange(sender, m_debugStaticAsteroids, "Models2\\ObjectsStatic\\Asteroids", MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID, true, MyMwcVoxelMaterialsEnum.Ice_01, statEnumsB);
        }

        private void ShowWayPoints(MyGuiControlCheckbox sender)
        {
            MyHud.ShowDebugWaypoints = !MyHud.ShowDebugWaypoints;
            MyWayPointGraph.SetVisibilityOfAllWaypoints(MyHud.ShowDebugWaypoints);
        }

        private void ShowPrefabModelsChange(MyGuiControlCheckbox sender)
        {
            ShowModelsChange(sender, m_debugPrefabModels, "Models2\\Prefabs");
        }

        private void ShowShipsChange(MyGuiControlCheckbox sender)
        {
            ShowModelsChange(sender, m_debugShips, "Models2\\Ships");
        }

        private void ShowWeaponsChange(MyGuiControlCheckbox sender)
        {
            ShowModelsChange(sender, m_debugWeapons, "Models2\\Weapons");
        }

        private void ShowDebugChange(MyGuiControlCheckbox sender)
        {
            ShowModelsChange(sender, m_debugItems, "Models2\\Debug");
        }

        MyEntity addStaticAsteroid(StringBuilder hudLabel, MyMwcObjectBuilder_StaticAsteroid_TypesEnum staticEnum, ref Vector3 currentPosition, Vector3 forward, MyMeshDrawTechnique? drawTechnique = null, MyMwcVoxelMaterialsEnum? material = null)
        {
            MyStaticAsteroid staticAsteroid = new MyStaticAsteroid();
            MyMwcObjectBuilder_StaticAsteroid objectBuilder = new MyMwcObjectBuilder_StaticAsteroid(staticEnum, material);
            staticAsteroid.Init(hudLabel.ToString(), objectBuilder, Matrix.Identity);
            return addEntity(staticAsteroid, ref currentPosition, forward);
        }

        MyEntity addDebris(StringBuilder hudLabel, MyModelsEnum? lod0, MyModelsEnum? lod1, ref Vector3 currentPosition, Vector3 forward, MyMeshDrawTechnique? drawTechnique = null)
        {
            MyDebugDebris debugDebris = new MyDebugDebris();
            debugDebris.Init(hudLabel, lod0, lod1, drawTechnique);

            return addEntity(debugDebris, ref currentPosition, forward);
        }

        MyEntity addEntity(MyEntity entity, ref Vector3 currentPosition, Vector3 forward)
        {
            float offset = (entity.WorldAABB.Max - entity.WorldAABB.Min).Length() / 2.0f + 15;
            currentPosition += forward * offset;

            Matrix mat = Matrix.CreateTranslation(entity.LocalVolumeOffset) * entity.WorldMatrix;
            entity.SetPosition(currentPosition - mat.Translation);

            currentPosition += forward * offset;

            return entity;
        }


        private void ShowModelsChange(MyGuiControlCheckbox sender, List<MyEntity> entities, string modelPath, MyMeshDrawTechnique? drawTechnique = null, bool staticAsteroids = false, MyMwcVoxelMaterialsEnum? voxelMaterial = null, List<MyMwcObjectBuilder_StaticAsteroid_TypesEnum> asteroidEnums = null)
        {
            if (sender.Checked)
            {
                string[] modelNames = Enum.GetNames(typeof(MyModelsEnum));
                int numModels = modelNames.Length;

                Vector3 forward = MySession.PlayerShip.GetWorldRotation().Forward;
                Vector3 currentPosition = MySession.PlayerShip.GetPosition() + forward * 20;

                if (!staticAsteroids)
                {
                    for (int i = 0; i < numModels; i++)
                    {
                        if (!MyModels.GetModelAssetName((MyModelsEnum)i).StartsWith(modelPath))
                            continue;

                        MyModel model = MyModels.GetModelOnlyData((MyModelsEnum)i);

                        MyEntity debugDebris = addDebris(new StringBuilder(modelNames[i]), (MyModelsEnum)i, null, ref currentPosition, forward, drawTechnique);

                        MyEntities.Add(debugDebris);
                        entities.Add(debugDebris);
                    }
                }
                else
                {
                      //foreach(MyMwcObjectBuilder_StaticAsteroid_TypesEnum statEnum in Enum.GetValues(typeof(MyMwcObjectBuilder_StaticAsteroid_TypesEnum)))
                      foreach(MyMwcObjectBuilder_StaticAsteroid_TypesEnum statEnum in asteroidEnums)
                      {
                          MyEntity debugDebris = addStaticAsteroid(new StringBuilder(statEnum.ToString()), statEnum, ref currentPosition, forward, null, voxelMaterial.Value);
                          MyEntities.Add(debugDebris);
                          entities.Add(debugDebris);
                      }
                }
            }
            else
            {
                foreach (MyEntity entity in entities)
                {
                    entity.MarkForClose();
                }
                entities.Clear();
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();

            m_debugFloatingObjects.Clear();
            m_debugStaticObjects.Clear();
            m_debugPrefabModels.Clear();
            m_debugShips.Clear();
            m_debugWeapons.Clear();
            m_debugItems.Clear();
            m_debugPrefabEntities.Clear();
        }

        private void ShowPrefabEntitiesChange(MyGuiControlCheckbox sender)
        {
            if (sender.Checked)
            {
                Vector3 forward = MySession.PlayerShip.GetWorldRotation().Forward;
                Vector3 currentPosition = MySession.PlayerShip.GetPosition() + forward * 20;                

                foreach (MyMwcObjectBuilderTypeEnum prefabModuleEnum in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
                {
                    foreach (int prefabId in MyMwcObjectBuilder_Base.GetObjectBuilderIDs(prefabModuleEnum))
                    {                                                
                        MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(prefabModuleEnum, prefabId);

                        MyEntity debugDebris = addDebris(new StringBuilder(prefabModuleEnum + "_" + prefabId), config.ModelLod0Enum, config.ModelLod1Enum, ref currentPosition, forward);
                        m_debugPrefabEntities.Add(debugDebris);
                    }                    
                }

                
            }
            else
            {
                foreach (MyEntity entity in m_debugPrefabEntities)
                {
                    entity.MarkForClose();
                }
                m_debugPrefabEntities.Clear();
            }
            
        }

        int DialogueSentencesComparer(MyDialogueSentence a, MyDialogueSentence b)
        {
            if (a.Cue.HasValue && b.Cue.HasValue)
            {
                return Enum.GetName(typeof(MySoundCuesEnum), a.Cue.Value).CompareTo(Enum.GetName(typeof(MySoundCuesEnum), b.Cue.Value));
            }
            else if (a.Cue.HasValue) return 1;
            else if (b.Cue.HasValue) return -1;
            else return 0;
        }

        private void OnGenerateListOfMissions(MyGuiControlButton sender)
        {
            foreach(MyMissionID missionID in Enum.GetValues(typeof(MyMissionID))){
                MyMission mission = MyMissions.GetMissionByID(missionID) as MyMission;
                if (mission != null)
                {
                    if (!System.IO.Directory.Exists(MyFileSystemUtils.GetApplicationUserDataFolder() + "\\MissionTexts"))
                    {
                        System.IO.Directory.CreateDirectory(MyFileSystemUtils.GetApplicationUserDataFolder() + "\\MissionTexts");
                    }
                    using (System.IO.StreamWriter output = new System.IO.StreamWriter(System.IO.File.Open(MyFileSystemUtils.GetApplicationUserDataFolder() + "\\MissionTexts\\" + Enum.GetName(typeof(MyMissionID), missionID) + ".txt", System.IO.FileMode.Create)))
                    {
                        output.WriteLine("Mission: " + mission.DebugName);
                        output.WriteLine();
                        output.WriteLine("MyMissionID." + Enum.GetName(typeof(MyMissionID), mission.ID));
                        output.WriteLine("DebugName: \"" + mission.DebugName + "\"");
                        output.WriteLine("Name: \"" + mission.Name + "\"");
                        output.WriteLine("Description: \"" + mission.Description + "\"");
                        output.WriteLine();
                        output.WriteLine("Objectives:");

                        foreach(MyObjective objective in mission.Objectives){
                            output.WriteLine("MyMissionID." + Enum.GetName(typeof(MyMissionID), objective.ID));
                            output.WriteLine("Name: \"" + objective.Name + "\"");
                            output.WriteLine("Description: \"" + objective.Description + "\"");
                            output.WriteLine();
                        }

                        output.Flush();
                        output.Close();
                    }
                }
            }
        }

        private void OnGenerateListOfDialogues(MyGuiControlButton sender)
        {
/*
            List<string> dialoguesString = new List<string>();

            foreach (MyDialoguesWrapperEnum textEnum in Enum.GetValues(typeof(MyDialoguesWrapperEnum)))
            {
                dialoguesString.Add(textEnum.ToString());
            }

            dialoguesString.Sort();

             var dialoguesPath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Dialogues.txt");
            using (var output = new StreamWriter(File.Open(dialoguesPath, FileMode.Create)))
            {
                StringBuilder line = new StringBuilder();
                foreach (string str in dialoguesString)
                {
                    foreach (MyDialoguesWrapperEnum textEnum in Enum.GetValues(typeof(MyDialoguesWrapperEnum)))
                    {
                        if (textEnum.ToString() == str)
                        {
                            line.Clear();

                            foreach (MyDialogueEnum dialogueEnum in Enum.GetValues(typeof(MyDialogueEnum)))
                            {
                                MyDialogue dialogue = MyDialogueConstants.GetDialogue(dialogueEnum);
                                foreach (MyDialogueSentence sentence in dialogue.Sentences)
                                {
                                    if (sentence.Text == textEnum)
                                    {
                                        line.Append(sentence.Actor.ToString() + " (" + MyDialoguesWrapper.Get(textEnum).ToString() + ")");
                                        break;
                                    }
                                }
                            }

                            output.WriteLine(line);
                            break;
                        }
                    }
                }

                output.Flush();
                output.Close();
            }

           
*/

            
            //get
            List<MyDialogueSentence> sentences = new List<MyDialogueSentence>();
            foreach (MyDialogueEnum dialogueEnum in Enum.GetValues(typeof(MyDialogueEnum)))
            {
                MyDialogue dialogue = MyDialogueConstants.GetDialogue(dialogueEnum);
                foreach(MyDialogueSentence sentence in dialogue.Sentences){
                    if(sentence.Text!=MyDialoguesWrapperEnum.Dlg_DialoguePlaceholder)
                        sentences.Add(sentence);
                }
            }
            //sort
            sentences.Sort(DialogueSentencesComparer);
            //print
            var dialoguesPath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Dialogues.txt");
            using (var output = new StreamWriter(File.Open(dialoguesPath, FileMode.Create)))
            {
                StringBuilder line = new StringBuilder();
                foreach(MyDialogueSentence sentence in sentences)
                {
                    line.Clear();
                    if (sentence.Cue.HasValue)
                    {
                        line.Append("MySoundCuesEnum." + Enum.GetName(typeof(MySoundCuesEnum), sentence.Cue.Value));
                    }
                    else
                    {
                        line.Append("[NULL]");
                    }
                    line.Append(" (");
                    line.Append(MyTextsWrapper.Get(MyActorConstants.GetActorProperties(sentence.Actor).DisplayName).ToString());
                    line.Append("): ");
                    line.Append(MyDialoguesWrapper.Get(sentence.Text).ToString());
                    output.WriteLine(line);
                }
                output.Flush();
                output.Close();
            }
        }


        private void OnGenerateListOfUnusedDialogues(MyGuiControlButton sender)
        {
            //get
            List<MyDialogueSentence> sentences = new List<MyDialogueSentence>();
            foreach (MyDialogueEnum dialogueEnum in Enum.GetValues(typeof(MyDialogueEnum)))
            {
                MyDialogue dialogue = MyDialogueConstants.GetDialogue(dialogueEnum);
                foreach (MyDialogueSentence sentence in dialogue.Sentences)
                {
                    if (sentence.Text != MyDialoguesWrapperEnum.Dlg_DialoguePlaceholder)
                        sentences.Add(sentence);
                }
            }
            //sort
            sentences.Sort(DialogueSentencesComparer);

            HashSet<MySoundCuesEnum> soundCues = new HashSet<MySoundCuesEnum>();
            foreach (MySoundCuesEnum soundEnum in Enum.GetValues(typeof(MySoundCuesEnum)))
            {
                if (Enum.GetName(typeof(MySoundCuesEnum), soundEnum).StartsWith("Dlg_"))
                    soundCues.Add(soundEnum);
            }

            HashSet<MyDialoguesWrapperEnum> dialogueTexts = new HashSet<MyDialoguesWrapperEnum>();
            foreach (MyDialoguesWrapperEnum textEnum in Enum.GetValues(typeof(MyDialoguesWrapperEnum)))
            {
                dialogueTexts.Add(textEnum);
            }

            //Remove used
            foreach (MyDialogueSentence sentence in sentences)
            {
                if (sentence.Cue.HasValue)
                    soundCues.Remove(sentence.Cue.Value);
                dialogueTexts.Remove(sentence.Text);
            }


            //print
            var dialoguesPath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "UnusedDialogues.txt");
            using (var output = new StreamWriter(File.Open(dialoguesPath, FileMode.Create)))
            {
                StringBuilder line = new StringBuilder();
                line.Append("Unused cues: ");
                output.WriteLine(line);
                foreach (MySoundCuesEnum soundCue in soundCues)
                {
                    line.Clear();
                    line.Append("MySoundCuesEnum." + Enum.GetName(typeof(MySoundCuesEnum), soundCue));
                    output.WriteLine(line);
                }

                line.Clear();
                line.AppendLine();
                output.WriteLine(line);
                line.Clear();
                line.Append("Unused texts: ");
                output.WriteLine(line);
                foreach (MyDialoguesWrapperEnum textEnum in dialogueTexts)
                {
                    line.Clear();
                    line.Append("MyDialoguesWrapperEnum." + Enum.GetName(typeof(MyDialoguesWrapperEnum), textEnum));
                    line.Append(":");
                    line.Append(MyDialoguesWrapper.Get(textEnum).ToString());
                    output.WriteLine(line);
                }

                output.Flush();
                output.Close();
            }
        }

        private void OnGenerateListOfAssets(MyGuiControlButton sender)
        {
            string trimPath = @"Models2\";
            foreach (MyMwcObjectBuilderTypeEnum enumValue in MyGuiPrefabHelpers.MyMwcPrefabTypesEnumValues)
            {
                foreach (int prefabId in MyMwcObjectBuilder_Base.GetObjectBuilderIDs(enumValue))
                {
                    MyGuiPrefabHelper prefabModuleHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(enumValue, prefabId) as MyGuiPrefabHelper;
                    MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(enumValue, prefabId);
                    string assetName = MyModels.GetModelAssetName(config.ModelLod0Enum);
                    int trimIndex = assetName.IndexOf(trimPath);
                    if (trimIndex >= 0)
                    {
                        assetName = assetName.Substring(trimIndex + trimPath.Length);
                    }

                    MyMwcLog.WriteLine(string.Format("{0};{1}", prefabModuleHelper.Description, assetName));
                }
            }

            foreach (MyMwcObjectBuilder_SmallShip_TypesEnum enumValue in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_TypesEnumValues)
            {
                MyGuiSmallShipHelperSmallShip smallShipHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip, (int)enumValue) as MyGuiSmallShipHelperSmallShip;
                var properties = MyShipTypeConstants.GetShipTypeProperties(enumValue);
                string assetName = MyModels.GetModelAssetName(properties.Visual.ModelLod0Enum);
                int trimIndex = assetName.IndexOf(trimPath);
                if (trimIndex >= 0)
                {
                    assetName = assetName.Substring(trimIndex + trimPath.Length);
                }

                MyMwcLog.WriteLine(string.Format("{0};{1}", smallShipHelper.Description, assetName));
            }
        }

        private void OnShortenShipHealth(MyGuiControlButton sender)
        {
            if (MySession.PlayerShip != null)
            {                
                MySession.PlayerShip.HealthRatio -= 0.1f;
            }
        }

        private void OnShortenPlayerHealth(MyGuiControlButton sender)
        {
            if (MySession.Static != null && MySession.Static.Player != null)
            {
                MySession.Static.Player.AddHealth(-MySession.Static.Player.MaxHealth * 0.1f, null);
            }
        }

        private void OnShortenArmor(MyGuiControlButton sender)
        {
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.ArmorHealth -= MySession.PlayerShip.MaxArmorHealth * 0.1f;
            }
        }

        private void OnShortenFuel(MyGuiControlButton sender)
        {
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Fuel -= MySession.PlayerShip.MaxFuel * 0.1f;
            }
        }

        private void OnShortenOxygen(MyGuiControlButton sender)
        {
            if (MySession.PlayerShip != null)
            {
                MySession.PlayerShip.Oxygen -= MySession.PlayerShip.MaxOxygen * 0.1f;
            }
        }

        private void LoadLastCheckpoint(MyGuiControlButton sender)
        {
            if (MyGuiScreenGamePlay.Static != null)
                MyGuiScreenGamePlay.Static.Restart();
        }

        private void OnDebugDialogues(MyGuiControlButton sender)
        {
            MyGuiManager.AddScreen(new MyGuiScreenDebugDialogues(null));
        }
    }
}

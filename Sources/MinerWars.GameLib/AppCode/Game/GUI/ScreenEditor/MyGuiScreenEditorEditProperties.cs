using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Entities.CargoBox;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.AppCode.Game.GUI.ScreenEditor
{
    class MyGuiScreenEditorEditProperties : MyGuiScreenEditorDialogBase
    {
        [Flags]
        private enum MyEditPropertyEnum
        {                
            None = 0,
            Name = 1 << 0,
            DisplayName = 1 << 1,
            Faction = 1 << 2,
            HealthRatio = 1 << 3,
            MaxHealth = 1 << 4,
            Indestructible = 1 << 5,
            FactionAppearance = 1 << 6,
            UseProperties = 1 << 7,
            Activated = 1 << 8,
            RequiresEnergy = 1 << 9,
            KinematicPartsHealthAndMaxHealth = 1 << 10,
            DisplayHud = 1 << 11,
            Difficulty = 1 << 12,
            //All = Name | DisplayName | Faction | Health | MaxHealth | Indestructible | FactionAppearance | UseProperties,
        }

        public class MyHealthAndMaxHealth 
        {
            public float Health;
            public float MaxHealth;

            public MyHealthAndMaxHealth(float health, float maxHealth) 
            {
                Health = health;
                MaxHealth = maxHealth;
            }
        }

        class MyEditProperties
        {
            public string Name;
            public string DisplayName;
            public MyMwcObjectBuilder_FactionEnum Faction;
            public float? HealthRatio;
            public float? MaxHealth;
            public bool IsIndestructible;
            public MyUseProperties UseProperties;
            public bool IsActivated;
            public bool? RequiresEnergy;
            public MyHealthAndMaxHealth[] KinematicPartsHealthAndMaxHealth;
            public bool DisplayHud;
            public MyGameplayDifficultyEnum Difficulty;
        }

        private static readonly Dictionary<Type, MyEditPropertyEnum> m_editPropertiesPerEntityType;
        private static readonly List<MyEditPropertyEnum> m_editPropertiesForMultiEdit = new List<MyEditPropertyEnum>()
        {
            MyEditPropertyEnum.DisplayName,
            MyEditPropertyEnum.Faction,
            MyEditPropertyEnum.FactionAppearance,
            MyEditPropertyEnum.HealthRatio,
            MyEditPropertyEnum.Indestructible,
            MyEditPropertyEnum.MaxHealth,
            MyEditPropertyEnum.UseProperties,
            MyEditPropertyEnum.RequiresEnergy,
            MyEditPropertyEnum.Activated,
            MyEditPropertyEnum.DisplayHud,
            MyEditPropertyEnum.Difficulty,
        };

        static MyGuiScreenEditorEditProperties()
        {
            m_editPropertiesPerEntityType = new Dictionary<Type, MyEditPropertyEnum>();
            m_editPropertiesPerEntityType.Add(typeof(MyPrefabContainer), 
                MyEditPropertyEnum.Name | MyEditPropertyEnum.DisplayName | MyEditPropertyEnum.Faction | MyEditPropertyEnum.FactionAppearance | MyEditPropertyEnum.Activated | MyEditPropertyEnum.Difficulty);
            m_editPropertiesPerEntityType.Add(typeof(MyPrefabBase),
                MyEditPropertyEnum.Name | MyEditPropertyEnum.DisplayName | MyEditPropertyEnum.HealthRatio | MyEditPropertyEnum.MaxHealth | MyEditPropertyEnum.Indestructible | MyEditPropertyEnum.Activated | MyEditPropertyEnum.RequiresEnergy | MyEditPropertyEnum.DisplayHud | MyEditPropertyEnum.Difficulty);
            m_editPropertiesPerEntityType.Add(typeof(MySmallShip),
                MyEditPropertyEnum.Name | MyEditPropertyEnum.DisplayName | MyEditPropertyEnum.HealthRatio | MyEditPropertyEnum.MaxHealth | MyEditPropertyEnum.Indestructible | MyEditPropertyEnum.Faction | MyEditPropertyEnum.Activated | MyEditPropertyEnum.DisplayHud | MyEditPropertyEnum.Difficulty);
            m_editPropertiesPerEntityType.Add(typeof(IMyUseableEntity),
                MyEditPropertyEnum.UseProperties | MyEditPropertyEnum.Activated);
            m_editPropertiesPerEntityType.Add(typeof(MyCargoBox),
                MyEditPropertyEnum.Name | MyEditPropertyEnum.DisplayName | MyEditPropertyEnum.HealthRatio | MyEditPropertyEnum.MaxHealth | MyEditPropertyEnum.Indestructible | MyEditPropertyEnum.Activated | MyEditPropertyEnum.DisplayHud | MyEditPropertyEnum.Difficulty);
            m_editPropertiesPerEntityType.Add(typeof(MyPrefabKinematic),
                MyEditPropertyEnum.KinematicPartsHealthAndMaxHealth);

            //m_editPropertiesForMultiEdit = new List<MyEditPropertyEnum>();
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.DisplayName);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.Faction);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.FactionAppearance);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.HealthRatio);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.Indestructible);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.MaxHealth);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.UseProperties);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.RequiresEnergy);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.Activated);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.DisplayHud);
            //m_editPropertiesForMultiEdit.Add(MyEditPropertyEnum.Difficulty);
        }        

        private static MyEditPropertyEnum GetEditProperties(MyEntity entity)
        {
            MyEditPropertyEnum result = MyEditPropertyEnum.None;
            foreach (KeyValuePair<Type, MyEditPropertyEnum> keyValuePair in m_editPropertiesPerEntityType)
            {
                if(keyValuePair.Key.IsInstanceOfType(entity))
                {
                    result |= keyValuePair.Value;
                }
            }
            return result;
        }

        public static bool CanEditProperties(MyEntity entity)
        {
            foreach (KeyValuePair<Type, MyEditPropertyEnum> keyValuePair in m_editPropertiesPerEntityType)
            {
                if (keyValuePair.Key.IsInstanceOfType(entity))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CanEditProperties(List<MyEntity> entities) 
        {
            if (entities.Count == 0) 
            {
                return false;
            }
            foreach (MyEntity entity in entities) 
            {
                if (!CanEditProperties(entity)) 
                {
                    return false;
                }
            }
            return true;
        }

        private static MyEditPropertyEnum GetEditProperties(List<MyEntity> entities) 
        {            
            Array allEditProperties = Enum.GetValues(typeof(MyEditPropertyEnum));
            int[] editPropertiesCount = new int[MyMwcUtils.GetMaxValueFromEnum<MyEditPropertyEnum>() + 1];            
            foreach (MyEntity entity in entities) 
            {
                MyEditPropertyEnum editPropertyForEntity = GetEditProperties(entity);
                foreach (MyEditPropertyEnum editProperty in allEditProperties)
                {                    
                    if ((entities.Count == 1 || m_editPropertiesForMultiEdit.Contains(editProperty)) && 
                        (editPropertyForEntity & editProperty) != 0)
                    {
                        editPropertiesCount[(int)editProperty]++;
                    }                    
                }
            }

            MyEditPropertyEnum result = MyEditPropertyEnum.None;
            foreach (MyEditPropertyEnum editProperty in allEditProperties)
            {
                if (editPropertiesCount[(int)editProperty] == entities.Count)
                {
                    result |= editProperty;
                }
            }

            return result;
        }

        private const int TEXTBOX_MAX_LENGTH = 64;
        private const int TEXTBOX_NUMBERS_MAX_LENGTH = 6;

        //private MyEntity m_entity;
        private List<MyEntity> m_entities;

        private MyGuiControlTextbox m_name;
        private MyGuiControlTextbox m_displayName;
        private MyGuiControlTextbox m_health;
        private MyGuiControlTextbox m_healthPercentage;
        private MyGuiControlTextbox m_maxHealth;
        private MyGuiControlCombobox m_faction;
        private MyGuiControlCheckbox m_indestructible;
        private MyGuiControlCheckbox m_activated;
        private MyGuiControlCombobox m_appearance;
        private MyGuiControlUseProperties m_useProperties;
        private MyGuiControlCheckbox m_requiresEnergyCheckbox;
        private MyGuiControlButton m_kinematicPartsHealthAndMaxHealth;
        private MyGuiControlCheckbox m_displayHud;
        private MyGuiControlCombobox m_difficulty;

        private MyEditProperties m_editProperties;

        private List<StringBuilder> m_errorMessages;
        private StringBuilder m_errorMessage;

        private bool[] m_editPropertiesNotSame;
        private MyEditPropertyEnum m_editPropertiesFlags;

        public MyGuiScreenEditorEditProperties(MyEntity entity)
            : this(new List<MyEntity>{entity})
        {                                                
        }

        public MyGuiScreenEditorEditProperties(List<MyEntity> entities)
            : base(new Vector2(0.5f, 0.5f), new Vector2(0.75f, 1f))
        {
            Debug.Assert(entities.Count > 0);
            Debug.Assert(CanEditProperties(entities));            

            AddCaption(new StringBuilder("Edit properties"), MyGuiConstants.LABEL_TEXT_COLOR);
            
            m_entities = entities;            
            m_editProperties = new MyEditProperties();
            m_editProperties.Name = m_entities[0].Name;
            m_editProperties.DisplayName = m_entities[0].DisplayName;
            m_editProperties.Faction = m_entities[0].Faction;
            m_editProperties.HealthRatio = m_entities[0].HealthRatio;
            m_editProperties.MaxHealth = m_entities[0].MaxHealth;
            m_editProperties.IsIndestructible = !m_entities[0].IsDestructible;
            m_editProperties.RequiresEnergy = m_entities[0] is MyPrefabBase ? ((MyPrefabBase)m_entities[0]).RequiresEnergy : null;
            m_editProperties.UseProperties = m_entities[0] is IMyUseableEntity ? ((IMyUseableEntity)m_entities[0]).UseProperties : null;
            m_editProperties.IsActivated = m_entities[0].Activated;
            m_editProperties.DisplayHud = m_entities[0].DisplayOnHud;
            m_editProperties.Difficulty = m_entities[0].MaxDifficultyToActivated;
            if (m_entities[0] is MyPrefabKinematic)
            {
                MyPrefabKinematic prefabKinematic = m_entities[0] as MyPrefabKinematic;
                MyPrefabConfigurationKinematic prefabKinematicConfig = prefabKinematic.GetConfiguration() as MyPrefabConfigurationKinematic;
                m_editProperties.KinematicPartsHealthAndMaxHealth = new MyHealthAndMaxHealth[prefabKinematicConfig.KinematicParts.Count];
                for (int i = 0; i < m_editProperties.KinematicPartsHealthAndMaxHealth.Length; i++) 
                {
                    MyPrefabKinematicPart kinematicPart = prefabKinematic.Parts[i];                    
                    if (kinematicPart != null)
                    {
                        float health = kinematicPart.Health;
                        float maxHealth = kinematicPart.MaxHealth;
                        m_editProperties.KinematicPartsHealthAndMaxHealth[i] = new MyHealthAndMaxHealth(health, maxHealth);
                    }                    
                }
            }

            m_editPropertiesFlags = GetEditProperties(m_entities);

            DetectNotSameProperties();
            InitControls();

            m_errorMessages = new List<StringBuilder>();
            m_errorMessage = new StringBuilder();
        }

        private void DetectNotSameProperties() 
        {
            m_editPropertiesNotSame = new bool[MyMwcUtils.GetMaxValueFromEnum<MyEditPropertyEnum>() + 1];
            foreach (MyEntity entity in m_entities)
            {
                if ((m_editPropertiesFlags & MyEditPropertyEnum.Name) > 0 && 
                    entity.Name != m_editProperties.Name)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.Name] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.DisplayName) > 0 && 
                    entity.DisplayName != m_editProperties.DisplayName)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.DisplayName] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.Faction) > 0 && 
                    entity.Faction != m_editProperties.Faction)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.Faction] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.HealthRatio) > 0 && 
                    entity.HealthRatio != m_editProperties.HealthRatio)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.HealthRatio] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.MaxHealth) > 0 && 
                    entity.MaxHealth != m_editProperties.MaxHealth)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.MaxHealth] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.Indestructible) > 0 && 
                    !entity.IsDestructible != m_editProperties.IsIndestructible)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.Indestructible] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.UseProperties) > 0 && 
                    entity is IMyUseableEntity && ((IMyUseableEntity)entity).UseProperties != m_editProperties.UseProperties)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.UseProperties] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.RequiresEnergy) > 0 &&
                    entity is MyPrefabBase && ((MyPrefabBase)entity).RequiresEnergy != m_editProperties.RequiresEnergy)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.RequiresEnergy] = true;
                }   
                if ((m_editPropertiesFlags & MyEditPropertyEnum.Activated) > 0 && 
                    entity.Activated != m_editProperties.IsActivated)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.Activated] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.DisplayHud) > 0 &&
                    entity.DisplayOnHud != m_editProperties.DisplayHud)
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.DisplayHud] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.Difficulty) > 0 &&
                    entity.MaxDifficultyToActivated != m_editProperties.Difficulty) 
                {
                    m_editPropertiesNotSame[(int)MyEditPropertyEnum.Difficulty] = true;
                }
                if ((m_editPropertiesFlags & MyEditPropertyEnum.KinematicPartsHealthAndMaxHealth) > 0 &&
                    entity is MyPrefabKinematic) 
                {
                    MyPrefabKinematic prefabKinematic = entity as MyPrefabKinematic;
                    MyPrefabConfigurationKinematic prefabKinematicConfig = prefabKinematic.GetConfiguration() as MyPrefabConfigurationKinematic;
                    for (int i = 0; i < prefabKinematicConfig.KinematicParts.Count; i++) 
                    {
                        MyPrefabKinematicPart prefabKinematicPart = prefabKinematic.Parts[i];
                        if (m_editProperties.KinematicPartsHealthAndMaxHealth == null &&
                            (prefabKinematicConfig.KinematicParts.Count != m_editProperties.KinematicPartsHealthAndMaxHealth.Length ||                            
                            m_editProperties.KinematicPartsHealthAndMaxHealth[i] == null && prefabKinematicPart != null ||
                            m_editProperties.KinematicPartsHealthAndMaxHealth[i] != null && prefabKinematicPart == null ||
                            m_editProperties.KinematicPartsHealthAndMaxHealth[i].Health != prefabKinematicPart.Health ||
                            m_editProperties.KinematicPartsHealthAndMaxHealth[i].MaxHealth != prefabKinematicPart.MaxHealth)) 
                        {
                            m_editPropertiesNotSame[(int)MyEditPropertyEnum.KinematicPartsHealthAndMaxHealth] = true;
                        }
                    }
                }
            }
        }

        private void InitControls()
        {
            Controls.Clear();            

            string entityName;
            if (m_entities.Count == 1)
            {
                entityName = "(" + m_entities[0].GetFriendlyName() + ")";
            }
            else 
            {
                entityName = "(Mutliedit)";
            }

            Controls.Add(new MyGuiControlLabel(this, new Vector2(0f, -m_size.Value.Y / 2f + 0.063f), null, new StringBuilder(entityName),
                MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));

            Vector2 controlsStartPosition = new Vector2(-m_size.Value.X / 2f + 0.05f, -m_size.Value.Y / 2f + 0.12f);
            Vector2 labelPosition = controlsStartPosition;
            Vector2 controlPosition = controlsStartPosition + new Vector2(0.4f, 0f);
            Vector4 notSameColor = Color.Red.ToVector4();

            // name
            if ((m_editPropertiesFlags & MyEditPropertyEnum.Name) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.Name, m_editPropertiesNotSame[(int)MyEditPropertyEnum.Name]);                
                m_name = new MyGuiControlTextbox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM,
                                                 m_editProperties.Name != null ? m_editProperties.Name : string.Empty, TEXTBOX_MAX_LENGTH,
                                                 MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE,
                                                 MyGuiControlTextboxType.NORMAL);
                Controls.Add(m_name);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            // display name
            if ((m_editPropertiesFlags & MyEditPropertyEnum.DisplayName) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.DisplayName, m_editPropertiesNotSame[(int)MyEditPropertyEnum.DisplayName]);                
                m_displayName = new MyGuiControlTextbox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM,
                                                 m_editProperties.DisplayName != null ? m_editProperties.DisplayName : string.Empty, TEXTBOX_MAX_LENGTH,
                                                 MyGuiConstants.TEXTBOX_BACKGROUND_COLOR, MyGuiConstants.LABEL_TEXT_SCALE,
                                                 MyGuiControlTextboxType.NORMAL);
                Controls.Add(m_displayName);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            // display hud
            if ((m_editPropertiesFlags & MyEditPropertyEnum.DisplayHud) != 0)
            {                
                AddLabel(labelPosition, MyTextsWrapperEnum.DisplayHud, m_editPropertiesNotSame[(int)MyEditPropertyEnum.DisplayHud]);
                m_displayHud = new MyGuiControlCheckbox(this, controlPosition, m_editProperties.DisplayHud, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);

                Controls.Add(m_displayHud);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;

            }

            // difficulty
            if ((m_editPropertiesFlags ^ MyEditPropertyEnum.Difficulty) != 0) 
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.ActivatedUpToDifficulty, m_editPropertiesNotSame[(int)MyEditPropertyEnum.Difficulty]);
                m_difficulty = new MyGuiControlCombobox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
                m_difficulty.AddItem((int)MyGameplayDifficultyEnum.EASY, MyTextsWrapperEnum.DifficultyEasy);
                m_difficulty.AddItem((int)MyGameplayDifficultyEnum.NORMAL, MyTextsWrapperEnum.DifficultyNormal);
                m_difficulty.AddItem((int)MyGameplayDifficultyEnum.HARD, MyTextsWrapperEnum.DifficultyHard);
                m_difficulty.SelectItemByKey((int)m_editProperties.Difficulty);

                Controls.Add(m_difficulty);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            // faction
            if ((m_editPropertiesFlags & MyEditPropertyEnum.Faction) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.Faction, m_editPropertiesNotSame[(int)MyEditPropertyEnum.Faction]);
                m_faction = new MyGuiControlCombobox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
                foreach (MyMwcObjectBuilder_FactionEnum faction in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
                {
                    MyGuiHelperBase factionHelper = MyGuiSmallShipHelpers.GetMyGuiSmallShipFactionNationality(faction);
                    m_faction.AddItem((int)faction, factionHelper.Description);
                }
                m_faction.SelectItemByKey((int)m_editProperties.Faction);

                Controls.Add(m_faction);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }            

            // health
            if ((m_editPropertiesFlags & MyEditPropertyEnum.HealthRatio) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.HealthPercentage, m_editPropertiesNotSame[(int)MyEditPropertyEnum.HealthRatio]);
                m_healthPercentage = new MyGuiControlTextbox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM,
                                                   m_editProperties.HealthRatio != null ?
                                                   GetFormatedFloat(
                                                       m_editProperties.HealthRatio.Value * 100f) : string.Empty,
                                                       TEXTBOX_NUMBERS_MAX_LENGTH,
                                                   MyGuiConstants.TEXTBOX_BACKGROUND_COLOR,
                                                   MyGuiConstants.LABEL_TEXT_SCALE,
                                                   MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_healthPercentage);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;

                AddLabel(labelPosition, MyTextsWrapperEnum.Health, m_editPropertiesNotSame[(int)MyEditPropertyEnum.HealthRatio]);
                m_health = new MyGuiControlTextbox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM,
                                                   m_editProperties.HealthRatio != null && m_editProperties.MaxHealth != null ?
                                                   GetFormatedFloat(
                                                       m_editProperties.HealthRatio.Value * m_editProperties.MaxHealth.Value) : string.Empty,
                                                       TEXTBOX_NUMBERS_MAX_LENGTH,
                                                   MyGuiConstants.TEXTBOX_BACKGROUND_COLOR,
                                                   MyGuiConstants.LABEL_TEXT_SCALE,
                                                   MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_health);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;

                m_healthPercentage.TextChanged -= HealthChanged;
                m_healthPercentage.TextChanged += HealthChanged;
                m_health.TextChanged -= HealthChanged;
                m_health.TextChanged += HealthChanged;
            }

            // maxhealth
            if ((m_editPropertiesFlags & MyEditPropertyEnum.MaxHealth) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.MaxHealth, m_editPropertiesNotSame[(int)MyEditPropertyEnum.MaxHealth]);
                m_maxHealth = new MyGuiControlTextbox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM,
                                                      m_editProperties.MaxHealth != null ?
                                                      GetFormatedFloat(
                                                          m_editProperties.MaxHealth.Value) : string.Empty,
                                                          TEXTBOX_NUMBERS_MAX_LENGTH,
                                                      MyGuiConstants.TEXTBOX_BACKGROUND_COLOR,
                                                      MyGuiConstants.LABEL_TEXT_SCALE,
                                                      MyGuiControlTextboxType.DIGITS_ONLY);
                Controls.Add(m_maxHealth);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
                m_maxHealth.TextChanged -= HealthChanged;
                m_maxHealth.TextChanged += HealthChanged;
            }

            // prefab kinematic parts health and max health
            if ((m_editPropertiesFlags & MyEditPropertyEnum.KinematicPartsHealthAndMaxHealth) != 0) 
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.EditPrefabKinematicPartsHealthAndMaxHealth, m_editPropertiesNotSame[(int)MyEditPropertyEnum.KinematicPartsHealthAndMaxHealth]);
                m_kinematicPartsHealthAndMaxHealth = new MyGuiControlButton(this, controlPosition + new Vector2(0.1f, 0f), MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE, MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                MyTextsWrapperEnum.Edit, MyGuiConstants.BUTTON_TEXT_COLOR, MyGuiConstants.BUTTON_TEXT_SCALE,
                    OnEditPrefabKinematicPartsHealthAndMaxHealthClick, MyGuiControlButtonTextAlignment.CENTERED, true, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER, true);
                Controls.Add(m_kinematicPartsHealthAndMaxHealth);
                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            // indestructible
            if ((m_editPropertiesFlags & MyEditPropertyEnum.Indestructible) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.Indestructible, m_editPropertiesNotSame[(int)MyEditPropertyEnum.Indestructible]);
                m_indestructible = new MyGuiControlCheckbox(this, controlPosition, m_editProperties.IsIndestructible, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
                m_indestructible.OnCheck -= OnIndestructibleChecked;
                m_indestructible.OnCheck += OnIndestructibleChecked;

                Controls.Add(m_indestructible);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            // change faction appearance
            if ((m_editPropertiesFlags & MyEditPropertyEnum.FactionAppearance) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.PrefabMaterials, false);

                m_appearance = new MyGuiControlCombobox(this, controlPosition, MyGuiControlPreDefinedSize.MEDIUM, MyGuiConstants.COMBOBOX_BACKGROUND_COLOR, MyGuiConstants.COMBOBOX_TEXT_SCALE);
                foreach (MyMwcObjectBuilder_Prefab_AppearanceEnum factionAppearance in MyGuiPrefabHelpers.MyMwcFactionTextureEnumValues)
                {
                    m_appearance.AddItem((int)factionAppearance, MyGuiPrefabHelpers.GetFactionName(factionAppearance));
                }
                Controls.Add(m_appearance);

                Controls.Add(new MyGuiControlButton(
                    this,
                    controlPosition + new Vector2(.2f, 0),
                    MyGuiConstants.BACK_BUTTON_SIZE,
                    MyGuiConstants.BUTTON_BACKGROUND_COLOR,
                    MyTextsWrapperEnum.Ok,
                    MyGuiConstants.BUTTON_TEXT_COLOR,
                    MyGuiConstants.BUTTON_TEXT_SCALE,
                    OnChangeAppearanceClick,
                    MyGuiControlButtonTextAlignment.CENTERED,
                    true,
                    MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,
                    true));

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            }

            // Requires energy
            if ((m_editPropertiesFlags & MyEditPropertyEnum.RequiresEnergy) != 0)
            {                
                AddLabel(labelPosition, MyTextsWrapperEnum.RequiresEnergy, m_editPropertiesNotSame[(int)MyEditPropertyEnum.RequiresEnergy]);
                m_requiresEnergyCheckbox = new MyGuiControlCheckbox(this, controlPosition, m_editProperties.RequiresEnergy.Value, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
                //m_requiresEnergyCheckbox.OnCheck -= OnRequiresEnergyChecked;
                //m_requiresEnergyCheckbox.OnCheck += OnRequiresEnergyChecked;

                Controls.Add(m_requiresEnergyCheckbox);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;

            }

            // use properties
            if ((m_editPropertiesFlags & MyEditPropertyEnum.UseProperties) != 0) 
            {
                m_useProperties = new MyGuiControlUseProperties(this, controlPosition, m_editProperties.UseProperties, m_editPropertiesNotSame[(int)MyEditPropertyEnum.UseProperties] ? new Vector4(1f, 0f, 0f, 1f) : MyGuiConstants.LABEL_TEXT_COLOR);
                Vector2 usePropertiesSize = m_useProperties.GetSize().Value;
                m_useProperties.SetPosition(labelPosition + usePropertiesSize / 2f);
                Controls.Add(m_useProperties);
                labelPosition += new Vector2(0f, usePropertiesSize.Y) + MyGuiConstants.CONTROLS_DELTA;
                controlPosition += new Vector2(0f, usePropertiesSize.Y) + MyGuiConstants.CONTROLS_DELTA;
            }


            // Activated
            /*  //Temporary disabled because there is no way how to select invisible object
            if ((m_editPropertiesFlags & MyEditPropertyEnum.Activated) != 0)
            {
                AddLabel(labelPosition, MyTextsWrapperEnum.Active, m_editPropertiesNotSame[(int)MyEditPropertyEnum.Activated]);
                m_activated = new MyGuiControlCheckbox(this, controlPosition, m_editProperties.IsActivated, MyGuiConstants.CHECKBOX_BACKGROUND_COLOR);
                m_activated.OnCheck -= OnActivatedChecked;
                m_activated.OnCheck += OnActivatedChecked;

                Controls.Add(m_activated);

                labelPosition += MyGuiConstants.CONTROLS_DELTA;
                controlPosition += MyGuiConstants.CONTROLS_DELTA;
            } */

            foreach (var notSameFlag in m_editPropertiesNotSame)
            {
                if (notSameFlag)
                {
                    Controls.Add(new MyGuiControlLabel(this, new Vector2(-m_size.Value.X / 2f + 0.05f, m_size.Value.Y / 2.0f - MyGuiConstants.MESSAGE_BOX_BORDER_AREA_Y - MyGuiConstants.MESSAGE_BOX_BUTTON_SIZE.Y / 2.0f + 0.03f), null, MyTextsWrapperEnum.RedColoredPropertiesAreNotSame, new Vector4(1f, 0f, 0f, 1f), MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
                    break;
                }
            }

            AddOkAndCancelButtonControls(new Vector2(0, 0.0f));
        }



        void OnEditPrefabKinematicPartsHealthAndMaxHealthClick(MyGuiControlButton sender) 
        {
            MyGuiManager.AddScreen(new MyGuiScreenEditorPrefabKinematicParts(m_editProperties.KinematicPartsHealthAndMaxHealth));
        }

        void HealthChanged(object sender, EventArgs e)
        {
            Debug.Assert(m_health != null && m_healthPercentage != null && m_maxHealth != null);
            m_healthPercentage.TextChanged -= HealthChanged;
            m_health.TextChanged -= HealthChanged;
            float? maxHealth = !String.IsNullOrEmpty(m_maxHealth.Text) ? GetFloatFromString(m_maxHealth.Text) : (float?)null;
            if (maxHealth != null)
            {
                if (sender == m_health)
                {
                    float? health = !String.IsNullOrEmpty(m_health.Text) ? GetFloatFromString(m_health.Text) : (float?)null;
                    if (health != null)
                    {
                        m_healthPercentage.Text = GetFormatedFloat(health.Value / maxHealth.Value * 100f);
                    }
                    else
                    {
                        m_healthPercentage.Text = string.Empty;
                    }
                }
                else if (sender == m_healthPercentage || sender == m_maxHealth)
                {
                    float? healthPercentage = !String.IsNullOrEmpty(m_healthPercentage.Text) ? GetFloatFromString(m_healthPercentage.Text) : (float?)null;
                    if (healthPercentage != null)
                    {
                        m_health.Text = GetFormatedFloat(healthPercentage.Value * maxHealth.Value / 100f);
                    }
                    else
                    {
                        m_health.Text = string.Empty;
                    }
                }                
            }            
            m_healthPercentage.TextChanged += HealthChanged;
            m_health.TextChanged += HealthChanged;
        }

        private void AddLabel(Vector2 labelPosition, MyTextsWrapperEnum text, bool isNotSame) 
        {
            Vector4 color = isNotSame ? new Vector4(1f, 0f, 0f, 1f) : MyGuiConstants.LABEL_TEXT_COLOR;
            Controls.Add(new MyGuiControlLabel(this, labelPosition, null, text, color, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER));
        }

        public void OnChangeAppearanceClick(MyGuiControlButton sender)
        {
            foreach (MyEntity entity in m_entities) 
            {
                var prefabContainer = entity as MyPrefabContainer;
                if (prefabContainer == null)
                    return;

                foreach (MyPrefabBase prefab in prefabContainer.Children)
                {
                    int material = m_appearance.GetSelectedKey();
                    if (material != -1)
                    {
                        if (prefab.HasAvailableFactionMaterial((MyMwcObjectBuilder_Prefab_AppearanceEnum)material))
                        {
                            prefab.MaterialIndex = (ushort)material;
                        }
                    }
                }
            }            
        }

        //private void OnRequiresEnergyChecked(MyGuiControlCheckbox sender)
        //{
        //    SaveTempEditProperties();

        //    InitControls();
        //}

        private void OnIndestructibleChecked(MyGuiControlCheckbox sender)
        {
            SaveTempEditProperties();

            InitControls();
        }

        private void OnActivatedChecked(MyGuiControlCheckbox sender)
        {
            SaveTempEditProperties();

            InitControls();
        }

        private void SaveTempEditProperties()
        {
            if (m_name != null)
            {
                m_editProperties.Name = m_name.Text;
            }

            if (m_displayName != null)
            {
                m_editProperties.DisplayName = m_displayName.Text;
            }

            if (m_faction != null)
            {
                m_editProperties.Faction = (MyMwcObjectBuilder_FactionEnum)m_faction.GetSelectedKey();
            }

            if (m_maxHealth != null)
            {
                if (string.IsNullOrEmpty(m_maxHealth.Text))
                {
                    m_editProperties.MaxHealth = null;
                }
                else
                {
                    m_editProperties.MaxHealth = GetFloatFromString(m_maxHealth.Text);
                }
            }

            //if (m_health != null)
            //{
            //    if (string.IsNullOrEmpty(m_health.Text))
            //    {
            //        m_editProperties.HealthRatio = null;
            //    }
            //    else
            //    {
            //        m_editProperties.HealthRatio = float.Parse(m_health.Text);
            //    }
            //}

            if (m_healthPercentage != null)
            {
                if (string.IsNullOrEmpty(m_healthPercentage.Text))
                {
                    m_editProperties.HealthRatio = null;
                }
                else
                {
                    float? healthPercentage = GetFloatFromString(m_healthPercentage.Text);
                    m_editProperties.HealthRatio = healthPercentage != null ? (healthPercentage.Value / 100f) : (float?)null;
                }
            }

            if (m_indestructible != null)
            {
                m_editProperties.IsIndestructible = m_indestructible.Checked;
                //if (m_editProperties.HealthRatio == -1f)
                //{
                //    m_editProperties.HealthRatio = m_entities[0].GetDefaultMaxHealth();
                //}
                //if (m_editProperties.MaxHealth == -1f)
                //{
                //    m_editProperties.MaxHealth = m_entities[0].GetDefaultMaxHealth();
                //}
            }
            if (m_requiresEnergyCheckbox != null)
            {
                m_editProperties.RequiresEnergy = m_requiresEnergyCheckbox.Checked;
            }
            if (m_activated != null)
            {
                m_editProperties.IsActivated = m_activated.Checked;
            }
            if (m_displayHud != null) 
            {
                m_editProperties.DisplayHud = m_displayHud.Checked;
            }
            if (m_difficulty != null) 
            {
                m_editProperties.Difficulty = (MyGameplayDifficultyEnum)m_difficulty.GetSelectedKey();
            }
        }

        private bool Validate(ref List<StringBuilder> errorMessages) 
        {
            // check healt and max health            
            if (m_editProperties.HealthRatio == null)
            {                
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageYouMustSetHealth));
            }
            if (m_editProperties.MaxHealth == null)
            {
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageYouMustSetMaxHealth));
            }
            if (m_editProperties.HealthRatio != null && m_editProperties.HealthRatio <= 0f)
            {
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageHealthCantBeLessOrEqualZero));
            }
            if (m_editProperties.MaxHealth != null && m_editProperties.MaxHealth <= 0f)
            {
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageMaxHealthCantBeLessOrEqualZero));
            }
            if (m_editProperties.HealthRatio != null && m_editProperties.HealthRatio > 1f)
            {
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageHealthPercentageCantBeGreaterThen100));
            }
            // check entity name
            if (MyEntities.IsNameExists(m_entities[0], m_editProperties.Name))
            {
                errorMessages.Add(MyTextsWrapper.Get(MyTextsWrapperEnum.MessageEntityNameIsAlreadyExists));
            }
            if (m_useProperties != null) 
            {
                m_useProperties.Validate(ref errorMessages);
            }
            return errorMessages.Count == 0;
        }

        protected override void OnOkClick(MyGuiControlButton sender)
        {                        
            SaveTempEditProperties();

            m_errorMessages.Clear();
            m_errorMessage.Clear();
            if (!Validate(ref m_errorMessages)) 
            {
                foreach(StringBuilder errorMessage in m_errorMessages)
                {
                    MyMwcUtils.AppendStringBuilder(m_errorMessage, errorMessage);
                    m_errorMessage.AppendLine();
                }
                StringBuilder caption = MyTextsWrapper.Get(m_errorMessages.Count > 1 ? MyTextsWrapperEnum.CaptionPropertiesAreNotValid : MyTextsWrapperEnum.CaptionPropertyIsNotValid);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, m_errorMessage, caption, MyTextsWrapperEnum.Ok, null));
                return;
            }

            Save();

            base.OnOkClick(sender);
        }

        private void Save() 
        {
            foreach (MyEntity entity in m_entities)
            {
                if (m_name != null)
                {
                    entity.SetName(m_editProperties.Name);
                }

                if (m_displayName != null)
                {
                    entity.DisplayName = m_editProperties.DisplayName;
                }

                if (m_faction != null)
                {
                    entity.Faction = m_editProperties.Faction;
                }

                if (m_requiresEnergyCheckbox != null)
                {
                    ((MyPrefabBase)entity).RequiresEnergy = m_requiresEnergyCheckbox.Checked;
                }
                if (m_maxHealth != null)
                {
                    entity.MaxHealth = m_editProperties.MaxHealth.Value;
                }

                if (m_healthPercentage != null)
                {
                    entity.HealthRatio = m_editProperties.HealthRatio.Value;
                }

                if (m_indestructible != null)
                {
                    entity.IsDestructible = !m_indestructible.Checked;
                }

                if (m_useProperties != null)
                {
                    m_useProperties.SaveTo(((IMyUseableEntity)entity).UseProperties);
                }

                if (m_activated != null)
                {
                    entity.Activate(m_editProperties.IsActivated, false);
                }

                if (m_displayHud != null)
                {
                    entity.DisplayOnHud = m_editProperties.DisplayHud;
                }

                if (m_difficulty != null)
                {
                    entity.MaxDifficultyToActivated = m_editProperties.Difficulty;
                }

                if (m_editProperties.KinematicPartsHealthAndMaxHealth != null && entity is MyPrefabKinematic)
                {
                    MyPrefabKinematic prefabKinematic = entity as MyPrefabKinematic;
                    for (int i = 0; i < m_editProperties.KinematicPartsHealthAndMaxHealth.Length; i++)
                    {
                        MyHealthAndMaxHealth healthAndMaxHealth = m_editProperties.KinematicPartsHealthAndMaxHealth[i];
                        MyPrefabKinematicPart prefabKinematicPart = prefabKinematic.Parts[i];
                        if (healthAndMaxHealth != null && prefabKinematicPart != null)
                        {
                            prefabKinematicPart.MaxHealth = healthAndMaxHealth.MaxHealth;
                            prefabKinematicPart.Health = healthAndMaxHealth.Health;
                        }
                    }
                }
            }
        }

        public override string GetFriendlyName()
        {
            return "Edit properties";
        }

        private static string GetFormatedFloat(float number) 
        {
            return MyValueFormatter.GetFormatedFloat(number, 2, string.Empty);
        }

        private static float? GetFloatFromString(string number) 
        {
            return MyValueFormatter.GetFloatFromString(number, 2, string.Empty);
        }
    }
}

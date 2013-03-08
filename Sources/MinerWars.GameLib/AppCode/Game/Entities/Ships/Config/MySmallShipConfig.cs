using System.Globalization;

namespace MinerWars.AppCode.Game.Entities
{
    using System;
    using System.Text;
    using App;
    using Audio;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using GUI;
    using Localization;
    using MinerWarsMath;
    using Utils;
    using MinerWars.AppCode.Game.Managers.Session;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using System.Diagnostics;

    class MySmallShipConfig
    {
        StringBuilder m_noText = new StringBuilder();
        MyPhysObjectSmallShipConfigItem[] m_items;

        public MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel Engine { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel ReflectorLight { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel ReflectorLongRange { get; private set; }
        public MyPhysObjectSmallShipConfigItemHudRadar RadarType { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch AutoLeveling { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch MovementSlowdown { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch BackCamera { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch HarvestingTool { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch DrillingTool { get; private set; }
        public MyPhysObjectSmallShipConfigItemViewMode ViewMode { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch HealthEnhancingMedicine { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch PerformanceEnhancingMedicine { get; private set; }
        public MyPhysObjectSmallShipConfigItemIntArray TimeBombTimer { get; private set; }
        public MyPhysObjectSmallShipConfigItemBoolSwitch RadarJammer { get; private set; }

        public event Action<MySmallShipConfig> ConfigChanged;

        public MySmallShipConfig(MySmallShip ship)
        {
            Engine = new MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel(MyTextsWrapper.Get(MyTextsWrapperEnum.EngineShutdown), 0.25f, 0f, 1f, true, delegate()
                {
                    ship.SetEngineSound(this.Engine.On);
                    RaiseConfigChanged();
                }, null, MyMinerShipConstants.MINER_SHIP_ENGINE_SWITCH_MIN_REPEAT_TRESHOLD, null); //later it will be good to have repeat threshold changing with engine type for example

            ReflectorLight = new MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel(MyTextsWrapper.Get(MyTextsWrapperEnum.ToggleHeadlights), 0.25f, 0f, 1f, true, delegate()
                {
                    MyAudio.AddCue2D(this.ReflectorLight.On ? MySoundCuesEnum.VehShipaLightsOn : MySoundCuesEnum.VehShipaLightsOff);
                    RaiseConfigChanged();
                }, null, 0, MyGameControlEnums.HEADLIGHTS);

            ReflectorLongRange = new MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel(MyTextsWrapper.Get(MyTextsWrapperEnum.HeadlightsRange),
                MyReflectorConstants.CHANGE_RANGE_INTERVAL_IN_MILISECONDS / 1000, 0f, 1f,
                MyTextsWrapper.Get(MyTextsWrapperEnum.HeadlightsRangeLong), MyTextsWrapper.Get(MyTextsWrapperEnum.HeadlightsRangeClose), false, delegate()
                {
                    MyAudio.AddCue2D(MySoundCuesEnum.SfxHudReflectorRange);
                    RaiseConfigChanged();
                }, null, 0, MyGameControlEnums.HEADLIGTHS_DISTANCE);

            RadarType = new MyPhysObjectSmallShipConfigItemHudRadar();

            if (!(ship is MySmallShipBot))
            {
                AutoLeveling = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.AutoLevel), false, delegate()
                    {
                        MyAudio.AddCue2D(this.AutoLeveling.On ? MySoundCuesEnum.SfxHudAutolevelingOn : MySoundCuesEnum.SfxHudAutolevelingOff);
                    }, null, 0, MyGameControlEnums.AUTO_LEVEL);
            }
            else
            {
                AutoLeveling = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.AutoLevel), false, delegate()
                {
                    ;
                }, null, 0, MyGameControlEnums.AUTO_LEVEL);
            }


            MovementSlowdown = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.MovementSlowdown), true, delegate()
                {
                    MyAudio.AddCue2D(this.MovementSlowdown.On ? MySoundCuesEnum.SfxHudSlowMovementOn : MySoundCuesEnum.SfxHudSlowMovementOff);
                    RaiseConfigChanged();
                }, null, 0, MyGameControlEnums.MOVEMENT_SLOWDOWN);

            BackCamera = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.RearCam), false, delegate()
                {
                    MyAudio.AddCue2D(this.BackCamera.On ? MySoundCuesEnum.SfxHudBackcameraOn : MySoundCuesEnum.SfxHudBackcameraOff);
                    RaiseConfigChanged();
                }, null, 0, MyGameControlEnums.REAR_CAM);

            HarvestingTool = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.Harvest), m_noText, m_noText, false, delegate()
                {
                    MySession.PlayerShip.Weapons.FireHarvester();
                    RaiseConfigChanged();
                }, delegate()
                {
                    if (ship.Weapons.GetWeaponsObjectBuilders(false) == null) return false;
                    if (!ship.Config.Engine.Enable || ship.Fuel == 0) return false;
                    foreach (var item in ship.Weapons.GetWeaponsObjectBuilders(false))
                    {
                        if (item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Harvesting_Device) return true;
                    }
                    return false;
                }, 0, MyGameControlEnums.HARVEST);

            DrillingTool = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.UseDrill), m_noText, m_noText, false, delegate()
                {
                    //This should be implemented somehow different
                    MySession.PlayerShip.Weapons.FireDrill();
                    RaiseConfigChanged();
                }, delegate()
                {
                    if (ship.Weapons.GetWeaponsObjectBuilders(false) == null) return false;
                    if (!ship.Config.Engine.Enable || ship.Fuel == 0) return false;
                    foreach (var item in ship.Weapons.GetWeaponsObjectBuilders(false))
                    {
                        if (item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Crusher ||
                            item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Laser ||
                            item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Nuclear ||
                            item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Saw ||
                            item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Thermal ||
                            item.WeaponType == MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum.Drilling_Device_Pressure)
                            return true;
                    }
                    return false;
                }, 0, MyGameControlEnums.DRILL);

            HealthEnhancingMedicine = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.ActivateHealthEnhancingMedicine), m_noText, m_noText, false, delegate()
            {
                if (!MySession.Static.Player.Medicines[(int)MinerWars.AppCode.Game.Entities.Tools.MyMedicineType.HEALTH_ENHANCING_MEDICINE].IsActive())
                    MySession.Static.Player.Medicines[(int)MinerWars.AppCode.Game.Entities.Tools.MyMedicineType.HEALTH_ENHANCING_MEDICINE].ActivateIfInInventory(MySession.PlayerShip.Inventory);
                RaiseConfigChanged();
            }, delegate()
            {
                var item = MySession.PlayerShip.Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.HEALTH_ENHANCING_MEDICINE);
                return item != null;
            }, 0, null);

            PerformanceEnhancingMedicine = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.ActivatePerformanceEnhancingMedicine), m_noText, m_noText, false, delegate()
            {
                if (!MySession.Static.Player.Medicines[(int)MinerWars.AppCode.Game.Entities.Tools.MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE].IsActive())
                    MySession.Static.Player.Medicines[(int)MinerWars.AppCode.Game.Entities.Tools.MyMedicineType.PERFORMANCE_ENHANCING_MEDICINE].ActivateIfInInventory(MySession.PlayerShip.Inventory);
                RaiseConfigChanged();
            }, delegate()
            {
                var item = MySession.PlayerShip.Inventory.GetInventoryItem(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int?)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.PERFORMANCE_ENHANCING_MEDICINE);
                return item != null;
            }, 0, null);

            ViewMode = new MyPhysObjectSmallShipConfigItemViewMode();

            TimeBombTimer = new MyPhysObjectSmallShipConfigItemIntArray(
                MyTimeBombConstants.TIMEOUT_ARRAY, 1, MyTextsWrapper.Get(MyTextsWrapperEnum.TimeBombTimer),
                delegate()
                {
                    RaiseConfigChanged();
                }, null, null);

            RadarJammer = new MyPhysObjectSmallShipConfigItemBoolSwitch(MyTextsWrapper.Get(MyTextsWrapperEnum.ToolRadarJammerName), true,
                delegate()
                {
                    RaiseConfigChanged();
                },
                delegate()
                {
                    return MySession.PlayerShip.Inventory.Contains(MyMwcObjectBuilderTypeEnum.SmallShip_Tool, (int)MyMwcObjectBuilder_SmallShip_Tool_TypesEnum.RADAR_JAMMER);
                }, 0, null);

            m_items = new MyPhysObjectSmallShipConfigItem[] 
            { 
                Engine, 
                ReflectorLight, 
                ReflectorLongRange, 
                //RadarType, 
                AutoLeveling, 
                MovementSlowdown, 
                BackCamera,
                HarvestingTool,
                DrillingTool,
                ViewMode,
                HealthEnhancingMedicine,
                PerformanceEnhancingMedicine,
                TimeBombTimer,
                RadarJammer,
            };

        }

        void RaiseConfigChanged()
        {
            var handler = ConfigChanged;
            if (handler != null)
            {
                handler(this);
            }
        }

        public void Update()
        {
            foreach (var item in m_items)
            {
                if (item is MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel)
                {
                    ((MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel)item).Update();
                }
            }
        }

        public void Items(int first, ref MyPhysObjectSmallShipConfigItem[] items, ref int count)
        {
            int enabledCount = 0;
            foreach (var item in m_items) if (item.Enable) ++enabledCount;

            // Try get as much items as possible
            int last = Math.Max(enabledCount - items.Length, 0);
            if (last < first)
            {
                first = last;
            }

            count = 0;
            foreach (var item in m_items)
            {
                if (!item.Enable) continue;
                if (first <= count && items.Length > count - first)
                {
                    items[count - first] = item;
                }
                count++;
            }

            for (int i = count - first; i < items.Length; i++)
            {
                items[i] = null;
            }
        }


        public void Items(ref MyPhysObjectSmallShipConfigItem[] items)
        {
            if (items.Length < m_items.Length)
            {
                items = new MyPhysObjectSmallShipConfigItem[m_items.Length];
            }

            int count = 0;
            foreach (var item in m_items)
            {
                items[count] = item.Enable ? item : null;
                count++;
            }

        }


        public MyMwcObjectBuilder_ShipConfig GetObjectBuilder()
        {
            return new MyMwcObjectBuilder_ShipConfig
            (
                Engine.On,
                (byte)RadarType.Current,
                AutoLeveling.On,
                MovementSlowdown.On,
                BackCamera.On,
                (byte)ViewMode.Current
            );
        }

        public void Init(MyMwcObjectBuilder_ShipConfig objectBuilder)
        {
            Engine.SetValue(objectBuilder.Engine);
            RadarType.SetValue((MyHudRadarTypesEnum)objectBuilder.RadarType);
            AutoLeveling.SetValue(objectBuilder.AutoLeveling);
            MovementSlowdown.SetValue(objectBuilder.MovementSlowdown);
            BackCamera.SetValue(objectBuilder.BackCamera);
            ViewMode.SetValue((MyViewModeTypesEnum)objectBuilder.ViewMode);
        }
    }

    abstract class MyPhysObjectSmallShipConfigItem
    {
        protected Action m_changeValueCallback;
        private Func<bool> m_isEnableCallback;

        public StringBuilder Name { get; private set; }
        public MyGameControlEnums? AssociatedControl { get; private set; }

        public abstract StringBuilder CurrentValueText { get; }

        public MyPhysObjectSmallShipConfigItem(StringBuilder name, Action onChangeValueCallback, Func<bool> isEnableCallback, MyGameControlEnums? associatedControl)
        {
            Name = name;
            m_changeValueCallback = onChangeValueCallback;
            m_isEnableCallback = isEnableCallback;
            AssociatedControl = associatedControl;
        }

        public virtual void ChangeValueUp()
        {
            if (m_changeValueCallback != null) m_changeValueCallback();
        }

        public virtual void ChangeValueDown()
        {
            if (m_changeValueCallback != null) m_changeValueCallback();
        }

        public bool Enable
        {
            get
            {
                if (m_isEnableCallback == null)
                    return true;
                else
                    return m_isEnableCallback();
            }
        }
    }

    class MyPhysObjectSmallShipConfigItemBoolSwitch : MyPhysObjectSmallShipConfigItem
    {
        bool m_value;
        StringBuilder m_trueText;
        StringBuilder m_falseText;
        int m_lastTimeChanged;
        int m_minChangeThreshold;  //this can be used to control, how often in time can be item switched on/off(for example engine)

        public override StringBuilder CurrentValueText
        {
            get { return m_value ? m_trueText : m_falseText; }
        }

        public bool On { get { return m_value; } }

        public MyPhysObjectSmallShipConfigItemBoolSwitch(StringBuilder name, bool isDefaultOn, Action onChangeValueCallback, Func<bool> isEnableCallback, int minChangeThreshold, MyGameControlEnums? associatedControl)
            : this(name, MyTextsWrapper.Get(MyTextsWrapperEnum.On), MyTextsWrapper.Get(MyTextsWrapperEnum.Off), isDefaultOn, onChangeValueCallback, isEnableCallback, minChangeThreshold, associatedControl)
        { }

        public MyPhysObjectSmallShipConfigItemBoolSwitch(StringBuilder name, StringBuilder trueText, StringBuilder falseText, bool isDefaultTrue, Action onChangeValue, Func<bool> isEnableCallback, int minChangeThreshold, MyGameControlEnums? associatedControl)
            : base(name, onChangeValue, isEnableCallback, associatedControl)
        {
            m_value = isDefaultTrue;
            m_trueText = trueText;
            m_falseText = falseText;
            m_minChangeThreshold = minChangeThreshold;
        }

        public override void ChangeValueUp()
        {
            System.Diagnostics.Debug.Assert(!MyMinerGame.IsPaused());
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_lastTimeChanged + m_minChangeThreshold)
            {
                m_value = !m_value;
                base.ChangeValueUp();
                m_lastTimeChanged = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        public override void ChangeValueDown()
        {
            System.Diagnostics.Debug.Assert(!MyMinerGame.IsPaused());
            if (MyMinerGame.TotalGamePlayTimeInMilliseconds > m_lastTimeChanged + m_minChangeThreshold)
            {
                m_value = !m_value;
                base.ChangeValueDown();
                m_lastTimeChanged = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        public virtual void SetOn()
        {
            if (!m_value)
            {
                m_value = true;
                m_changeValueCallback();
            }
        }

        public virtual void SetOff()
        {
            if (m_value)
            {
                m_value = false;
                m_changeValueCallback();
            }
        }

        public void SetValue(bool value)
        {
            m_value = value;
        }
    }

    class MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel : MyPhysObjectSmallShipConfigItemBoolSwitch
    {
        public float Level { get; set; } //I think 'set' should be private, but method TrailerUpdate in MySmallShip set level for reflector and engine directly.
        private float m_intervalInSeconds;
        private float m_levelMinValue;
        private float m_levelMaxValue;
        private float m_levelChangePerSecond;

        public MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel(StringBuilder name, float levelIntervalInSeconds, float levelMinValue, float levelMaxValue, bool isDefaultOn, Action onChangeValue, Func<bool> isEnableCallback, int minChangeThreshold, MyGameControlEnums? associatedControl)
            : base(name, isDefaultOn, onChangeValue, isEnableCallback, minChangeThreshold, associatedControl)
        {
            m_intervalInSeconds = levelIntervalInSeconds;
            m_levelMinValue = levelMinValue;
            m_levelMaxValue = levelMaxValue;
            m_levelChangePerSecond = (m_levelMaxValue - m_levelMinValue) / m_intervalInSeconds;
        }

        public MyPhysObjectSmallShipConfigItemBoolSwitchWithLevel(StringBuilder name, float levelIntervalInSeconds, float levelMinValue, float levelMaxValue, StringBuilder trueText, StringBuilder falseText, bool isDefaultTrue, Action onChangeValue, Func<bool> isEnableCallback, int minChangeThreshold, MyGameControlEnums? associatedControl)
            : base(name, trueText, falseText, isDefaultTrue, onChangeValue, isEnableCallback, minChangeThreshold, associatedControl)
        {
            m_intervalInSeconds = levelIntervalInSeconds;
            m_levelMinValue = levelMinValue;
            m_levelMaxValue = levelMaxValue;
            m_levelChangePerSecond = (m_levelMaxValue - m_levelMinValue) / m_intervalInSeconds;
        }

        public void Update()
        {
            float sign = (this.On) ? +1 : -1;
            Level += sign * m_levelChangePerSecond * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            Level = MathHelper.Clamp(Level, m_levelMinValue, m_levelMaxValue);
        }
    }

    class MyPhysObjectSmallShipConfigItemHudRadar : MyPhysObjectSmallShipConfigItem
    {
        protected MyHudRadarTypesEnum m_current;
        protected MyHudRadarTypesEnum m_default;

        public MyHudRadarTypesEnum Current { get { return m_current; } }

        public MyPhysObjectSmallShipConfigItemHudRadar()
            : base(MyTextsWrapper.Get(MyTextsWrapperEnum.RadarType), null, null, null /*MyGameControlEnums.RADAR*/)
        {
            m_default = MyHudRadarTypesEnum.Normal3D;
            m_current = m_default;
        }

        public override void ChangeValueUp()
        {
            MyAudio.AddCue2D(MySoundCuesEnum.SfxHudRadarMode);
            switch (m_current)
            {
                case MyHudRadarTypesEnum.Normal3D:
                    m_current = MyHudRadarTypesEnum.Player2D;
                    break;
                case MyHudRadarTypesEnum.Player2D:
                    m_current = MyHudRadarTypesEnum.Solar2D;
                    break;
                case MyHudRadarTypesEnum.Solar2D:
                    m_current = MyHudRadarTypesEnum.Normal3D;
                    break;
            }
            base.ChangeValueUp();
        }

        public override void ChangeValueDown()
        {
            MyAudio.AddCue2D(MySoundCuesEnum.SfxHudRadarMode);
            switch (m_current)
            {
                case MyHudRadarTypesEnum.Normal3D:
                    m_current = MyHudRadarTypesEnum.Solar2D;
                    break;
                case MyHudRadarTypesEnum.Player2D:
                    m_current = MyHudRadarTypesEnum.Normal3D;
                    break;
                case MyHudRadarTypesEnum.Solar2D:
                    m_current = MyHudRadarTypesEnum.Player2D;
                    break;
            }
            base.ChangeValueDown();
        }

        public override StringBuilder CurrentValueText
        {
            get
            {
                switch (m_current)
                {
                    case MyHudRadarTypesEnum.Normal3D:
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.RadarTypeNormal3D);
                    case MyHudRadarTypesEnum.Player2D:
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.RadarTypePlayer2D);
                    case MyHudRadarTypesEnum.Solar2D:
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.RadarTypeSolar2D);
                    default:
                        return new StringBuilder();
                }
            }
        }

        public void SetValue(MyHudRadarTypesEnum value)
        {
            m_current = value;
        }
    }

    //IMPORTANT: If you add some item to enum below, add case to switches in class MyPhysObjectSmallShipConfigItemHudRadar
    enum MyHudRadarTypesEnum
    {
        //  Standard 3d Radar
        Normal3D = 0,
        //  2D radar where points are projected on to the horizontal
        //  plane in relation to the player
        Player2D = 1,
        //  2D radar where points are projected on to the solar system plane
        Solar2D = 2,
    }

    class MyPhysObjectSmallShipConfigItemViewMode : MyPhysObjectSmallShipConfigItem
    {
        protected MyViewModeTypesEnum m_current;

        public MyViewModeTypesEnum Current { get { return m_current; } }

        public MyPhysObjectSmallShipConfigItemViewMode()
            : base(MyTextsWrapper.Get(MyTextsWrapperEnum.ViewMode), null, null, MyGameControlEnums.VIEW_MODE)
        {
            m_current = MyViewModeTypesEnum.CockpitOn;
        }

        private void OnValueChange(bool playSound = true)
        {
            if (playSound) MyAudio.AddCue2D(MySoundCuesEnum.SfxHudCockpitOff);

            if (m_current == MyViewModeTypesEnum.ThirdPerson)
            {
                MyThirdPersonSpectator.Init(MySession.PlayerShip.GetPosition(),
                                            MySession.PlayerShip.GetWorldRotation());
            }

            MyGuiScreenGamePlay.Static.CameraAttachedTo = GetCameraMode();
        }

        public override void ChangeValueUp()
        {
            if (MyGuiScreenGamePlay.Static.DetachingForbidden)
            {
                return;
            }

            if (MySession.Is25DSector)
            {
                m_current = MyViewModeTypesEnum.ThirdPerson;
            }
            else
            {
                switch (m_current)
                {
                    case MyViewModeTypesEnum.CockpitOn:
                        m_current = MyViewModeTypesEnum.CockpitOff;
                        break;
                    case MyViewModeTypesEnum.CockpitOff:
                        m_current = MyViewModeTypesEnum.ThirdPerson;
                        break;
                    case MyViewModeTypesEnum.ThirdPerson:
                        m_current = MyViewModeTypesEnum.CockpitOn;
                        break;
                }
            }

            OnValueChange();
            base.ChangeValueUp();
        }

        public override void ChangeValueDown()
        {
            if (MyGuiScreenGamePlay.Static.DetachingForbidden)
            {
                return;
            }

            if (MySession.Is25DSector)
            {
                m_current = MyViewModeTypesEnum.ThirdPerson;
            }
            else
            {
                switch (m_current)
                {
                    case MyViewModeTypesEnum.CockpitOn:
                        m_current = MyViewModeTypesEnum.ThirdPerson;
                        break;
                    case MyViewModeTypesEnum.ThirdPerson:
                        m_current = MyViewModeTypesEnum.CockpitOff;
                        break;
                    case MyViewModeTypesEnum.CockpitOff:
                        m_current = MyViewModeTypesEnum.CockpitOn;
                        break;
                }
            }

            OnValueChange();
            base.ChangeValueDown();
        }

        public override StringBuilder CurrentValueText
        {
            get
            {
                switch (m_current)
                {
                    case MyViewModeTypesEnum.CockpitOn:
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.ViewModeCockpitOn);
                    case MyViewModeTypesEnum.CockpitOff:
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.ViewModeCockpitOff);
                    case MyViewModeTypesEnum.ThirdPerson:
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.ViewModeThirdPerson);
                    default:
                        return new StringBuilder();
                }
            }
        }

        public void SetValue(MyViewModeTypesEnum value)
        {
            m_current = value;

            // we dont wanna change camera mode in editor, change camera only in gameplay
            if (MyGuiScreenGamePlay.Static != null && MyGuiScreenGamePlay.Static.IsGameActive() && !MyGuiScreenGamePlay.Static.DetachingForbidden)
            {
                OnValueChange(false);
            }
        }

        public MyCameraAttachedToEnum GetCameraMode()
        {
            switch (m_current)
            {
                case MyViewModeTypesEnum.CockpitOn:
                case MyViewModeTypesEnum.CockpitOff:
                    return MyCameraAttachedToEnum.PlayerMinerShip;
                    break;
                case MyViewModeTypesEnum.ThirdPerson:
                    return MyCameraAttachedToEnum.PlayerMinerShip_ThirdPersonDynamic;
                    break;
            }

            Debug.Fail("SmallShipConfig.ViewMode in incorrect state!");
            return MyCameraAttachedToEnum.PlayerMinerShip;
        }
    }

    class MyPhysObjectSmallShipConfigItemIntArray : MyPhysObjectSmallShipConfigItem
    {
        readonly int[] m_array;
        readonly StringBuilder[] m_texts;
        int m_index;

        public MyPhysObjectSmallShipConfigItemIntArray(int[] array, int defaultIndex, StringBuilder name, Action onChangeValueCallback, Func<bool> isEnableCallback, MyGameControlEnums? associatedControl)
            : base(name, onChangeValueCallback, isEnableCallback, associatedControl)
        {
            m_array = array;
            m_index = defaultIndex;

            m_texts = new StringBuilder[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                m_texts[i] = new StringBuilder(array[i].ToString(CultureInfo.InvariantCulture));
            }
        }

        public override void ChangeValueDown()
        {
            m_index--;
            if (m_index < 0) m_index += m_array.Length;
            m_index %= m_array.Length;
            base.ChangeValueDown();
        }

        public override void ChangeValueUp()
        {
            m_index++;
            m_index %= m_array.Length;
            base.ChangeValueUp();
        }

        public void SetValue(int value)
        {
            for (int i = 0; i < m_array.Length; i++)
            {
                if (m_array[i] == value)
                {
                    m_index = i;
                    break;
                }
            }
        }

        public override StringBuilder CurrentValueText { get { return m_texts[m_index]; } }

        public int CurrentValue { get { return m_array[m_index]; } }
    }

    //IMPORTANT: If you add some item to enum below, add case to switches in class MyPhysObjectSmallShipConfigItemViewMode
    enum MyViewModeTypesEnum
    {
        CockpitOn = 0,
        CockpitOff = 1,
        ThirdPerson = 2,
    }
}

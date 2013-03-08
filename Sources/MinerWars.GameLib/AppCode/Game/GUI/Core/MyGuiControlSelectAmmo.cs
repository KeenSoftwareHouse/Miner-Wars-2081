using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using System;
using System.Text;
using MinerWars.AppCode.Toolkit.Input;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlSelectAmmo : MyGuiControlBase
    {
        List<MyAmmoItem> m_ammoTypesAmounts = new List<MyAmmoItem>(10);
        MyMwcObjectBuilder_AmmoGroupEnum m_selectedGroup;
        MyMwcObjectBuilder_AmmoGroupEnum m_selectedGroupLast;
        int m_selectedIndex;
        MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum m_selectedAmmo;
        //bool m_visible;
        bool m_isPressLast;
        StringBuilder m_text = null;
        StringBuilder m_textValues = null;

        //public bool IsVisible { get { return m_visible; } }
        public bool IsEnabled { get; set; }

        public MyGuiControlSelectAmmo(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4? backgroundColor)
            : base(parent, position, size, backgroundColor, null)
        {
            Visible = false;
            m_isPressLast = false;

            m_textValues = new StringBuilder();
            
            ReloadControlText();
            MyGuiManager.GetInput().ControlsSaved += OnGuiInputControlsSaved;
        }

        void OnGuiInputControlsSaved(MyGuiInput sender)
        {
            ReloadControlText();
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {

            //If this control is not enable, do nothing
            if (!IsEnabled) return false;

            bool isKeyPress = false;
            bool backwardSelectDirection = input.IsKeyPress(Keys.LeftShift) || input.IsKeyPress(Keys.RightShift);
            int deltaWheelPos = input.PreviousMouseScrollWheelValue() - input.MouseScrollWheelValue();
            if (deltaWheelPos != 0) // determine just direction
                deltaWheelPos /= Math.Abs(deltaWheelPos);

            if (input.IsNewGameControlPressed(MyGameControlEnums.SELECT_AMMO_BULLET))
            {
                isKeyPress = true;
                m_selectedGroup = MyMwcObjectBuilder_AmmoGroupEnum.Bullet;
            }

            if (input.IsNewGameControlPressed(MyGameControlEnums.SELECT_AMMO_MISSILE))
            {
                isKeyPress = true;
                m_selectedGroup = MyMwcObjectBuilder_AmmoGroupEnum.Missile;
            }

            if (input.IsNewGameControlPressed(MyGameControlEnums.SELECT_AMMO_CANNON))
            {
                isKeyPress = true;
                m_selectedGroup = MyMwcObjectBuilder_AmmoGroupEnum.Cannon;
            }

            if (input.IsNewGameControlPressed(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_FRONT))
            {
                isKeyPress = true;
                m_selectedGroup = MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront;
            }

            if (input.IsNewGameControlPressed(MyGameControlEnums.SELECT_AMMO_UNIVERSAL_LAUNCHER_BACK))
            {
                isKeyPress = true;
                m_selectedGroup = MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack;
            }

            // in case of wheel direction modify flags to run correct code
            if (deltaWheelPos > 0 && Visible && IsEnabled)
            {
                isKeyPress = true;
                backwardSelectDirection = false;
            }
            // in case of wheel direction modify flags to run correct code
            if (deltaWheelPos < 0 && Visible && IsEnabled)
            {
                isKeyPress = true;
                backwardSelectDirection = true;
            }

            if (isKeyPress)
            {
                if (Visible == false)
                {
                    //I am here when the menu opens
                    LoadAmmoFromShip();
                    //If player change fire commands during game play, we need to show actual buttons names.
                    //ReloadControlText();
                    MyGuiSmallShipHelperAmmo.ResetDescription();
                }
                else
                {
                    if (m_selectedGroup == m_selectedGroupLast)
                    {
                       // if (!MyFakes.MW25D)
                        {
                            if (!backwardSelectDirection)
                                m_selectedIndex++;
                            else
                                m_selectedIndex--;
                        }
                       // else
                         //   m_selectedIndex = 0;
                    }
                    else
                    {
                        //I am here when category is changed
                        LoadAmmoFromShip();
                        MyGuiSmallShipHelperAmmo.ResetDescription();
                    }

                    if (m_selectedIndex >= m_ammoTypesAmounts.Count)
                        m_selectedIndex = 0;
                    if (m_selectedIndex < 0)
                        m_selectedIndex = m_ammoTypesAmounts.Count - 1;
                }

                Visible = true;
                m_selectedGroupLast = m_selectedGroup;
                if (m_selectedIndex < m_ammoTypesAmounts.Count && m_selectedIndex >= 0)
                {
                    m_selectedAmmo = m_ammoTypesAmounts[m_selectedIndex].Type;
                }

                MyAudio.AddCue2D(MySoundCuesEnum.SfxHudWeaponScroll);
            }


            //Because I do not want the gun fired before I made the choice, the menu disappears after the release of key (not press)
            if (Visible)
            {
                if (!MySession.Is25DSector)
                {

                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SECONDARY) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Secondary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_THIRD) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Third, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FOURTH) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Fourth, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FIFTH) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Fifth, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }

                    if (input.IsNewKeyPress(Keys.Escape))
                    {
                        Visible = false;
                    }
                }
                else
                {  //MyFakes.MW25D
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_SECONDARY) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_THIRD) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FOURTH) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }
                    if (input.IsGameControlPressed(MyGameControlEnums.FIRE_FIFTH) && m_selectedAmmo != 0)
                    {
                        MySession.PlayerShip.Weapons.AmmoAssignments.AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.Primary, m_selectedGroup, m_selectedAmmo);
                        m_isPressLast = true;
                    }

                    if (input.IsNewKeyPress(Keys.Escape))
                    {
                        Visible = false;
                    }
                }

                for (int i = m_ammoTypesAmounts.Count - 1; i >= 0; i--)
                {
                    int itemsPerColumn = 7;

                    int orderX = (int)m_selectedGroup + i / itemsPerColumn;
                    int orderY = i % itemsPerColumn;

                    /*
                    Vector2 mousePos = MyGuiManager.MouseCursorPosition;
                    if (MyVideoModeManager.IsTripleHead() == true)
                        mousePos += new Vector2(-1, 0);
                    
                    if (MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(m_ammoTypesAmounts[i].Type).IsPointInMyArea(mousePos))
                    {
                    }
                    */
                    //if (MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(m_ammoTypesAmounts[i].Type).IsPointInMyArea(MyGuiManager.MouseCursorPosition))
                    MyGuiSmallShipHelperAmmo ammoHelper =
                        MyGuiObjectBuilderHelpers.GetGuiHelper(MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int) m_ammoTypesAmounts[i].Type) as MyGuiSmallShipHelperAmmo;
                    if (ammoHelper.IsPointInMyArea(MyGuiManager.MouseCursorPosition))
                    {
                        if (m_selectedIndex != i || m_selectedAmmo != m_ammoTypesAmounts[m_selectedIndex].Type)
                            MyAudio.AddCue2D(MySoundCuesEnum.SfxHudWeaponScroll);
                        m_selectedIndex = i;
                        m_selectedAmmo = m_ammoTypesAmounts[m_selectedIndex].Type;
                        
                    }
                }

                //if (!MyFakes.MW25D)
                {
                    if (m_isPressLast &&
                        !input.IsGameControlPressed(MyGameControlEnums.FIRE_PRIMARY) &&
                        !input.IsGameControlPressed(MyGameControlEnums.FIRE_SECONDARY) &&
                        !input.IsGameControlPressed(MyGameControlEnums.FIRE_THIRD) &&
                        !input.IsGameControlPressed(MyGameControlEnums.FIRE_FOURTH) &&
                        !input.IsGameControlPressed(MyGameControlEnums.FIRE_FIFTH))
                    {
                        //I am here when the menu closes
                        m_isPressLast = false;
                        Visible = false;
                        MyAudio.AddCue2D(MySoundCuesEnum.SfxHudWeaponSelect);
                    }
                }
              /*  else
                {
                    if (isKeyPress)
                        MyAudio.AddCue2D(MySoundCuesEnum.SfxHudWeaponSelect);
                } */
            }
              /*
            if (MyFakes.MW25D)
            {
                return false;
            }   */

            return base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
        }

        private void LoadAmmoFromShip()
        {
            m_selectedAmmo = 0;
            m_selectedIndex = 0;
            m_ammoTypesAmounts.Clear();

            //If there is no ammo or no weapon, do nothing
            if (MySession.PlayerShip.Weapons.AmmoInventoryItems.GetAmmoInventoryItems().Count == 0 ||                 
                MySession.PlayerShip.Weapons.GetWeaponsObjectBuilders(false) == null)
            {
                return;
            }

            foreach (var ammoInventoryItem in MySession.PlayerShip.Weapons.AmmoInventoryItems.GetAmmoInventoryItems())
            {
                MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = (MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum)ammoInventoryItem.ObjectBuilderId.Value;
                if (MyGuiSmallShipHelpers.IsAmmoInGroup(ammoType, m_selectedGroup))
                {
                    int index = -1;
                    for (int i = 0; i < m_ammoTypesAmounts.Count; i++)
                    {
                        if (m_ammoTypesAmounts[i].Type == ammoType)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index >= 0)
                    {
                        m_ammoTypesAmounts[index] = new MyAmmoItem(ammoType, m_ammoTypesAmounts[index].Amount + (int)ammoInventoryItem.Amount);
                    }
                    else
                    {
                        bool isWeapon = false;
                        foreach (var weaponBuilder in MySession.PlayerShip.Weapons.GetWeaponsObjectBuilders(false))
                        {
                            if (MyGuiSmallShipHelpers.GetWeaponType(ammoType, m_selectedGroup).HasValue &&
                                MyGuiSmallShipHelpers.GetWeaponType(ammoType, m_selectedGroup).Value == weaponBuilder.WeaponType)
                            {
                                isWeapon = true;
                                break;
                            }
                        }
                        if (isWeapon) m_ammoTypesAmounts.Add(new MyAmmoItem(ammoType, (int)ammoInventoryItem.Amount));
                    }
                }
            }

            if (m_ammoTypesAmounts.Count > 0)
            {
                m_selectedAmmo = m_ammoTypesAmounts[m_selectedIndex].Type;
            }
        }

        public void ReloadControlText()
        {
            m_text = MyTextsWrapper.Get(MyTextsWrapperEnum.AmmoSelectText);

            MyGuiInput input = MyGuiManager.GetInput();
            m_textValues.Clear();
            m_textValues.AppendFormat(MyTextsWrapper.GetFormatString(MyTextsWrapperEnum.AmmoSelectTextValues),
                input.GetGameControl(MyGameControlEnums.FIRE_PRIMARY).GetControlButtonStringBuilderCombined(" / "),
                input.GetGameControl(MyGameControlEnums.FIRE_SECONDARY).GetControlButtonStringBuilderCombined(" / "),
                input.GetGameControl(MyGameControlEnums.FIRE_THIRD).GetControlButtonStringBuilderCombined(" / "),
                input.GetGameControl(MyGameControlEnums.FIRE_FOURTH).GetControlButtonStringBuilderCombined(" / "),
                input.GetGameControl(MyGameControlEnums.FIRE_FIFTH).GetControlButtonStringBuilderCombined(" / ")
            );
        }
               
        public override void Update()
        {
            base.Update();

            //// if has changed controls?
            //if (((MyGuiScreenGamePlay)m_parent).GetControlsChange())
            //{
            //    ReloadControlText();
            //}

            if (Visible) MyConfig.EditorUseCameraCrosshair = false;
        }

        public override void Draw()
        {
        /*    if (MyFakes.MW25D)
            {
                Visible = true;
                //m_mouseCursorHoverTexture = null;
               // return;
            }
          */
            if (!Visible) return;
            

            MyGuiManager.BeginSpriteBatch();
            var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);

            /*
            // Draw information:
            Vector2 screenSizeNormalize = MyGuiManager.GetNormalizedSize(MyGuiManager.GetAmmoSelectKeyConfirmBorderTexture(), MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_INFO_SCALE);
            screenSizeNormalize.X *= MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_SCALE.X;
            screenSizeNormalize.Y *= MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_SCALE.Y;            
           */

            var screenSizeNormalize = new Vector2(918f / 1600f, 314f / 1200f);
                       
            //if (!MyFakes.MW25D)
            if (Visible) 
            {
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetAmmoSelectKeyConfirmBorderTexture(),
                    MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_POSITION + offset, screenSizeNormalize,
                    MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_COLOR, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

                MyGuiManager.DrawStringCentered(MyGuiManager.GetFontMinerWarsBlue(), m_text,
                    MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_TEXT_INFO_POSITION + offset, MyGuiConstants.AMMO_SELECTION_CONFIRMATION_TEXT_SCALE,
                    MyGuiConstants.AMMO_SELECTION_CONFIRM_INFO_TEXT_COLOR, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_textValues,
                    MyGuiConstants.AMMO_SELECTION_CONFIRM_BORDER_TEXT_POSITION + offset, MyGuiConstants.AMMO_SELECTION_CONFIRMATION_TEXT_INFO_SCALE,
                    MyGuiConstants.AMMO_SELECTION_CONFIRM_INFO_TEXT_COLOR, MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
                         
            MyGuiSmallShipHelperAmmo.DrawSpriteBatchBackground(MyGuiConstants.SELECT_AMMO_BACKGROUND_COLOR);
            
            foreach (MyMwcObjectBuilder_AmmoGroupEnum item in MyGuiSmallShipHelpers.MyMwcObjectBuilder_SmallShip_AssignmentOfAmmo_GroupEnumValues)
            {
                MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(item).DrawSpriteBatchMenuHeader((int)item - 1, (m_selectedGroup == item), m_selectedIndex, m_backgroundColor.Value);
            }
               
            if (!Visible)
            {
                MyGuiManager.EndSpriteBatch();
                return;
            }    

            for (int i = m_ammoTypesAmounts.Count -1 ; i >= 0; i--)
            {
                int itemsPerColumn = 7;

                int orderX = i / itemsPerColumn;
                int orderY = i % itemsPerColumn;    

                MyGuiSmallShipHelperAmmo ammoHelper = MyGuiObjectBuilderHelpers.GetGuiHelper(
                    MyMwcObjectBuilderTypeEnum.SmallShip_Ammo, (int) m_ammoTypesAmounts[i].Type) as MyGuiSmallShipHelperAmmo;
                //MyGuiSmallShipHelpers.GetMyGuiSmallShipHelperAmmo(m_ammoTypesAmounts[i].Type).DrawSpriteBatchMenuItem(
                //    orderX - 1, orderY - 1, (i == m_selectedIndex), m_ammoTypesAmounts[i].Amount, m_backgroundColor.Value, i, (int)m_selectedGroup);
                ammoHelper.DrawSpriteBatchMenuItem(orderX - 1, orderY - 1, (i == m_selectedIndex), m_ammoTypesAmounts[i].Amount, m_backgroundColor.Value, i, (int)m_selectedGroup);
            }


            MyGuiSmallShipHelperAmmo.DrawSpriteBatchTooltip();

            MyGuiManager.EndSpriteBatch();
        }

        private struct MyAmmoItem : IEquatable<MyAmmoItem>
        {
            public MyAmmoItem(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum type, int amount)
            {
                Amount = amount;
                Type = type;
            }

            public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum Type;
            public int Amount;

            public bool Equals(MyAmmoItem other)
            {
                return (this.Type == other.Type);
            }
        }
    }
}

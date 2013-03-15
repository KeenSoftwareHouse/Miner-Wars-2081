using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Game.Sessions;

namespace MinerWars.AppCode.Game.GUI.Prefabs
{
    abstract class MyGuiControlEntityUse : MyGuiControlParent
    {
        public MyGuiSceenEntityUseBase ParentScreen { get; set; }
        
        protected Vector2 m_topLeftPosition;
        protected MyEntity m_entity;
        protected MyTexture2D m_texture;
        protected Color m_textureColor = Color.White;

        //protected MyGuiControlEntityUse(IMyGuiControlsParent parent, Vector2 size, MyEntity entity)
        //    : base(parent, Vector2.Zero, size, Vector4.One, new StringBuilder(entity.DisplayName), MyGuiManager.GetHubItemBackground()) 
        //{
        //    m_entity = entity;
        //    m_topLeftPosition = -m_size.Value / 2f + new Vector2(0.025f, 0.025f);
        //    Controls.Add(new MyGuiControlLabel(this, m_topLeftPosition + new Vector2(0.063f, 0.0f), null, GetEntityName(entity), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
        //    LoadControls();
        //}

        //protected MyGuiControlEntityUse(IMyGuiControlsParent parent, Vector2 size, MyEntity entity, MyTexture2D texture):this(parent,size,entity)
        //{
        //    m_texture = texture;
        //}

        protected MyGuiControlEntityUse(IMyGuiControlsParent parent, Vector2 size, MyEntity entity)
            : this(parent, size, entity, MyGuiObjectBuilderHelpers.GetGuiHelper(entity.GetObjectBuilder(true)).Icon)
        {            
        }

        protected MyGuiControlEntityUse(IMyGuiControlsParent parent, Vector2 size, MyEntity entity, MyTexture2D texture)
            : base(parent, Vector2.Zero, size, Vector4.One, new StringBuilder(entity.DisplayName), MyGuiManager.GetHubItemBackground())            
        {
            m_entity = entity;
            m_topLeftPosition = -m_size.Value / 2f + new Vector2(0.025f, 0.025f);
            Controls.Add(new MyGuiControlLabel(this, m_topLeftPosition + new Vector2(0.063f, 0.0f), null, GetEntityName(entity), MyGuiConstants.LABEL_TEXT_COLOR, MyGuiConstants.LABEL_TEXT_SCALE, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER, MyGuiManager.GetFontMinerWarsBlue()));
            LoadControls();
            m_texture = texture;
        }

        protected Vector2 GetNextControlPosition() 
        {
            float maxY = m_topLeftPosition.Y;
            foreach (MyGuiControlBase control in Controls.GetList())
            {
                Vector2 position = control.GetPosition();
                maxY = Math.Max(maxY, position.Y);
                //Vector2? size = control.GetSize();
                //if (size != null) 
                //{
                //    float maxPositionY = position.Y + size.Value.Y / 2f;
                //    maxY = Math.Max(maxY, maxPositionY);
                //}
            }
            return new Vector2(m_topLeftPosition.X, maxY) + MyGuiConstants.CONTROLS_DELTA;            
        }

        public bool IsControlledByOtherPlayer()
        {
            return MyMultiplayerGameplay.IsRunning && MyMultiplayerGameplay.Static.IsLockedByOtherPlayer(m_entity);
        }

        protected abstract void LoadControls();


        protected void CloseHUBScreen()
        {
            ParentScreen.CloseScreen();
        }

        public virtual void ClearAfterRemove() 
        {

        }

        protected void HideHUBScreen()
        {
            ParentScreen.HideScreen();
            ParentScreen.CanBeUnhidden = false;
        }

        public override void Draw()
        {
            base.Draw();

            if (m_texture != null)
            {
                var position = m_parent.GetPositionAbsolute() + m_position - new Vector2(m_size.Value.X / 2 - 0.01f, 0);
                MyGuiManager.DrawSpriteBatch(m_texture, position, MyGuiConstants.LISTBOX_SMALL_SIZE, m_textureColor, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
            }
        }

        protected StringBuilder GetEntityName(MyEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.DisplayName))
            {
                return new StringBuilder(entity.GetCorrectDisplayName());
            }
            else
            {
                if(entity is MyPrefabLargeWeapon && (entity as MyPrefabLargeWeapon).GetGun() != null){
                    MyLargeShipGunBase gun = (entity as MyPrefabLargeWeapon).GetGun();
                    if (gun is MyLargeShipAutocannon)
                    {
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.Autocannon);
                    }
                    else if (gun is MyLargeShipCIWS)
                    {
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.CIWS);
                    }
                    else if (gun is MyLargeShipMachineGun)
                    {
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.MachineGun);
                    }
                    else if(gun is MyLargeShipMissileLauncherGun)
                    {
                        return MyTextsWrapper.Get(MyTextsWrapperEnum.MissileLauncher);
                    }
                }
                else if(entity is MyPrefabCamera){
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Camera);
                }
                else if(entity is MyPrefabAlarm){
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Alarm);
                }
                else if(entity is MyPrefabBankNode){
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.BankNode);
                }
                else if(entity is MyPrefabGenerator){
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Generator);
                }
                else if (entity is MyPrefabScanner)
                {
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Scanner);
                }
                else if (entity is MyPrefabContainer)
                {
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Alarm);
                }
                else if (entity is MyPrefabKinematic)
                {
                    MyPrefabKinematic pk = (MyPrefabKinematic)entity;
                    return MyTextsWrapper.Get(MyTextsWrapperEnum.Door);
                }
                return new StringBuilder();
            }
        }

        public MyEntity Entity
        {
            get { return m_entity; }
        }
    }
}

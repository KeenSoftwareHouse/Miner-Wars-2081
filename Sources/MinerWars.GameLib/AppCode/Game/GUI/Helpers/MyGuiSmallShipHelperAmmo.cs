using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWarsMath;
using System;
using System.Text;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.App;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.Textures;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.VideoMode;


namespace MinerWars.AppCode.Game.GUI.Helpers
{
    class MyGuiSmallShipHelperAmmo : MyGuiHelperBase
    {
        private static Vector2 topLeftPosition;

        private const float m_menuItemWidth = 49 / 1600f;
        private const float m_itemWidth = MyGuiConstants.AMMO_SELECT_MENU_ITEM_WIDTH * 2.0f * MyGuiConstants.AMMO_SELECT_SCALE;

        private const float m_menuItemDistance = 1/1600f;
        public static float ItemDistance =  1/1600f;

        private static float m_menuItemHeight;
        public static float ItemHeight;

        private static  readonly Vector2 m_offset = new Vector2(0.02f,0.02f);
        private static Vector2 m_backSize = new Vector2(316 / 1600f, 100 / 1200f);
        private Vector4 m_backgroundActualColor;
        private Color m_color;
        private static Vector2 m_diff;
        private Vector2 m_diffMovedPosition;
        private static Vector2 m_tipTextPosition;
        private static Vector2 m_tipFogPosition;
        private static Vector2 m_tipFadeScale;
        private static Vector2 screenSizeNormalize;
        private Vector2 m_lastSpritePosition;

        StringBuilder m_amount = new StringBuilder();
        static StringBuilder m_description = null;



        public MyGuiSmallShipHelperAmmo(MyTexture2D icon, MyTextsWrapperEnum description)
            : this(icon, description, MyInventoryAmountTextAlign.MiddleRight)
        {            
        }

        public MyGuiSmallShipHelperAmmo(MyTexture2D icon, MyTextsWrapperEnum description, MyInventoryAmountTextAlign textAlign)
            : base(icon, description)
        {
            InventoryTextAlign = textAlign;
            Reload();


        }


        public static void Reload()
        {

            Rectangle a = MyGuiManager.GetSafeFullscreenRectangle();
            Rectangle b = MyGuiManager.GetFullscreenRectangle();
            topLeftPosition = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(MyGuiConstants.AMMO_SELECT_LEFT_TOP_POSITION);
            topLeftPosition += new Vector2((a.X-b.X)/a.Width,0);
            m_menuItemHeight = m_menuItemWidth * 4f/3f;
            ItemHeight = m_itemWidth *4f/3f;
        }

        public static void ResetDescription()
        {
            m_description = null;
        }

        public static void DrawSpriteBatchBackground(Vector4? color)
        {
            var offset = new Vector2(VideoMode.MyVideoModeManager.IsTripleHead() ? -1 : 0, 0);
           
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetAmmoSelectLowBackground(),
                MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(MyGuiConstants.AMMO_SELECT_LEFT_TOP_POSITION) + offset, m_backSize,
                new Color(color.Value), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
             
        }


        public static void DrawSpriteBatchTooltip()
        {
            // draw description if any, under background layer:
            if (m_description != null)
            {
                /*
                var bodyBgSize =
                    MyGuiManager.GetNormalizedSizeFromScreenSize(
                        MyGuiManager.GetFontMinerWarsWhite().MeasureString(m_description,
                                                                              MyGuiConstants.AMMO_SELECTION_TOOL_TIP_TEXT_SCALE)) + new Vector2(-0.005f, -0f);
                */


                var bodyBgSize = MyGuiManager.GetNormalizedSize(MyGuiManager.GetFontMinerWarsWhite(), m_description,
                                               MyGuiConstants.AMMO_SELECTION_TOOL_TIP_TEXT_SCALE) * 1.05f;


                //bodyBgSize.Y = 72/1200f;
                m_tipTextPosition += new Vector2(0.01f, 0f);
                m_tipTextPosition.Y -= m_menuItemHeight * 2f;
                var leftPos = m_tipTextPosition + new Vector2(0.005f, 0f);

                MyGUIHelper.FillRectangle(leftPos, bodyBgSize, MyGuiConstants.TOOL_TIP_BACKGROUND_COLOR);
                MyGUIHelper.OutsideBorder(leftPos, bodyBgSize, 1, MyGuiConstants.TOOL_TIP_BORDER_COLOR);

                //MyGuiManager.BeginSpriteBatch();
                //MyGuiManager.DrawSpriteBatchRoundUp(MyGuiManager.GetToolTipLeft(), leftPos, new Vector2(32 / 1600f, 72 / 1200f), new Color(255, 255, 255, 255), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_CENTER);
                //MyGuiManager.EndSpriteBatch();

                //MyGuiManager.EndSpriteBatch();
                //MyGuiManager.EndSpriteBatch();

                //MyGuiManager.DrawStencilMaskRectangleRoundUp(new MyRectangle2D(leftPos, bodyBgSize), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

                //MyGuiManager.BeginSpriteBatch_StencilMask();
                //MyGuiManager.DrawSpriteBatchRoundUp(MyGuiManager.GetToolTipBody(), leftPos, new Vector2(1024 / 1600f, 72 / 1200f), new Color(255, 255, 255, 255), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                //MyGuiManager.EndSpriteBatch_StencilMask();

                //MyGuiManager.BeginSpriteBatch();
                //MyGuiManager.DrawSpriteBatchRoundUp(MyGuiManager.GetToolTipRight(), leftPos + new Vector2(bodyBgSize.X, 0), new Vector2(32 / 1600f, 72 / 1200f), new Color(255, 255, 255, 255), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);
                //MyGuiManager.EndSpriteBatch();

                //MyGuiManager.BeginSpriteBatch();
                //MyGuiManager.BeginSpriteBatch();

                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_description, m_tipTextPosition + new Vector2(0.01f, 0f), MyGuiConstants.AMMO_SELECTION_TOOL_TIP_TEXT_SCALE,
                    Color.White,
                    MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);



            }

        }


        public void DrawSpriteBatchMenuHeader(int orderX, bool selected, int selectedIndex, Vector4? backgroundColor)
        {
            MyGuiManager.BeginSpriteBatch();

            if (backgroundColor.HasValue)
            {
                // selection color:
                m_backgroundActualColor = backgroundColor.Value;
                m_color = new Color(m_backgroundActualColor);

                Vector2 deltaVector = new Vector2(orderX * (m_menuItemWidth + m_menuItemDistance), 0);
                

                Vector2 startPosition =
                    MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(
                        MyGuiConstants.AMMO_SELECT_LEFT_TOP_POSITION) + deltaVector + m_offset; 

                // render textures:
                if (selected)
                {
                    m_color = new Color(m_backgroundActualColor * MyGuiConstants.SELECT_AMMO_ACTIVE_COLOR_MULTIPLIER);
                    if (selectedIndex == 0)
                    {
                        m_backgroundActualColor *= MyGuiConstants.SELECT_AMMO_ACTIVE_COLOR_MULTIPLIER;
                    }
                    /*
                    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetAmmoSelectTexture(), startPosition,
                        new Vector2(m_menuItemWidth, m_menuItemHeight),
                        new Color(m_backgroundActualColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                     * */
                }
                else
                {
                    m_color = new Color(MyGuiConstants.SELECT_AMMO_BACKGROUND_COLOR/*DEFAULT_CONTROL_BACKGROUND_COLOR*/);
                }
                
                base.DrawSpriteBatch(startPosition, new Vector2(m_menuItemWidth, m_menuItemHeight),
                    m_color, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                
            }
            MyGuiManager.EndSpriteBatch();
        }

        public bool IsPointInMyArea(Vector2 pos)
        {
            if (m_lastSpritePosition.X <= pos.X && pos.X <= m_lastSpritePosition.X + m_itemWidth)
            {
                if (m_lastSpritePosition.Y <= pos.Y && pos.Y <= m_lastSpritePosition.Y + ItemHeight)
                {
                    return true;
                }
            }
            return false;
        }

        public void DrawSpriteBatchMenuItem(int orderX, int orderY, bool selected, int amount, Vector4? backgroundColor, int index, int selectedGroup)
        {   
            MyMwcUtils.ClearStringBuilder(m_amount);
            m_amount.AppendInt32(amount);

            orderX += 1;
            
            MyGuiManager.BeginSpriteBatch();

            if (backgroundColor.HasValue)
            {
                // selection color:
                m_backgroundActualColor = backgroundColor.Value;
                if (selected)
                    m_backgroundActualColor *= MyGuiConstants.SELECT_AMMO_ACTIVE_COLOR_MULTIPLIER;
                else
                    m_backgroundActualColor.W *= 1.0f; // move this to constants

                m_color = new Color(m_backgroundActualColor);

                

                //calculate for save of pos
                m_diff.X = topLeftPosition.X + m_offset.X ;
                m_diff.Y = topLeftPosition.Y + m_menuItemHeight + m_offset.Y ;
                m_diffMovedPosition.X = m_diff.X + (orderX * (m_itemWidth + ItemDistance)) + (selectedGroup-1) * (m_menuItemWidth + m_menuItemDistance);
                m_diffMovedPosition.Y = m_diff.Y + ((orderY + 1) * (ItemHeight + ItemDistance)) +0.005f;

                m_lastSpritePosition = m_diffMovedPosition;//buffer last weapon sprite pos for cursor testing
                m_diff = MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate(MyGuiConstants.AMMO_SELECT_LEFT_TOP_POSITION) + m_offset;
                m_diff.Y += m_menuItemHeight;
                m_diffMovedPosition.X = m_diff.X + (orderX * (m_itemWidth + ItemDistance)) + (selectedGroup - 1) * (m_menuItemWidth + m_menuItemDistance);
                m_diffMovedPosition.Y = m_diff.Y + ((orderY + 1) * (ItemHeight + ItemDistance)) + 0.005f;

                /*
                // Select texture based on position:
                MyGuiManager.DrawSpriteBatch((orderY == -1 && index == 0) ? MyGuiManager.GetAmmoSelectBorderFirst() : MyGuiManager.GetAmmoSelectBorderPiece(),
                    m_diffMovedPosition, new Vector2(m_itemWidth, ItemHeight),
                    m_color, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                */
                base.DrawSpriteBatch(m_diffMovedPosition, new Vector2(m_itemWidth, ItemHeight),
                    m_color, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
                
                //m_lastSpritePosition = m_diffMovedPosition;//buffer last weapon sprite pos for cursor testing

                m_diff.X += m_itemWidth * MyGuiConstants.AMMO_SELECT_ITEM_TEXT_RELATIVE_POSITION.X;
                m_diff.Y += ItemHeight * MyGuiConstants.AMMO_SELECT_ITEM_TEXT_RELATIVE_POSITION.Y;
                m_diffMovedPosition.X = m_diff.X + (orderX * (m_itemWidth + ItemDistance)) + (selectedGroup - 1) * (m_menuItemWidth + m_menuItemDistance);
                m_diffMovedPosition.Y = m_diff.Y + ((orderY + 1) * (ItemHeight + ItemDistance));


                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_amount,
                    m_diffMovedPosition, 0.6f, new Color(MyGuiConstants.LABEL_TEXT_COLOR),
                    MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);

                // draw hint:
                if (selected)
                {
                    m_description = MultiLineDescription;
                    m_tipTextPosition = m_diffMovedPosition;
                }
            }

            MyGuiManager.EndSpriteBatch();
        }

        public void HudDrawActualAmmo(Vector2 position, Vector2 scaleToAmmoSelection, int amount, Vector4? backgroundColor, StringBuilder ammoSpecialText)
        {
            if (backgroundColor.HasValue)
            {
                m_color = new Color(backgroundColor.Value);
                MyGuiManager.BeginSpriteBatch();
                base.DrawSpriteBatch(position, new Vector2(m_itemWidth * scaleToAmmoSelection.X, ItemHeight * scaleToAmmoSelection.Y), m_color, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);

                Vector2 textPosition = position;
                textPosition.X += m_itemWidth / 2.6f * 2.0f;
                textPosition.Y += ItemHeight * scaleToAmmoSelection.Y - ItemHeight / 6.0f;
                m_diff.X += m_itemWidth * scaleToAmmoSelection.X * MyGuiConstants.AMMO_SELECT_ITEM_TEXT_RELATIVE_POSITION.X;
                m_diff.Y += ItemHeight * scaleToAmmoSelection.Y * MyGuiConstants.AMMO_SELECT_ITEM_TEXT_RELATIVE_POSITION.Y;

                MyMwcUtils.ClearStringBuilder(m_amount);
                m_amount.AppendInt32(amount);

                var color = new Color(MyGuiConstants.LABEL_TEXT_COLOR);
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_amount,
                    textPosition, 0.7f, color,
                    MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM);

                if (ammoSpecialText != null && ammoSpecialText.Length > 0)
                {
                    textPosition.Y -= ItemHeight / 4.0f;
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), ammoSpecialText, textPosition, .7f,
                                            color, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_BOTTOM);
                }

                MyGuiManager.EndSpriteBatch();
            }
        }

        public void HudDrawActualAmmoComment(Vector4? backgroundColor, StringBuilder text)
        {
            if (backgroundColor.HasValue)
            {
                m_color = new Color(backgroundColor.Value);
                MyGuiManager.BeginSpriteBatch();
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), text, MyNotificationConstants.DEFAULT_NOTIFICATION_MESSAGE_NORMALIZED_POSITION - new Vector2(0, 0.03f), 1, new Color(MyGuiConstants.LABEL_TEXT_COLOR), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM);
                MyGuiManager.EndSpriteBatch();
            }
        }
    }
}

using System.Text;
using Microsoft.Xna.Framework;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.GUI.Core
{
    class MyGuiControlContextMenu : MyGuiControlBase
    {
        StringBuilder m_text;
        MyGuiScreenEditorContextMenu m_contextMenu;
        bool m_enabled;
        public bool IsEnabled { get { return m_enabled;} }

        public MyGuiControlContextMenu(MyGuiScreenBase parentScreen, Vector2 position, Vector2? size)
            : base(parentScreen, position, size, null, null)
        {
            m_enabled = false;
            LoadText();
        }

        private void LoadText()
        {
            m_text = new StringBuilder();
            m_text.Append(MyTextsWrapper.Get(MyTextsWrapperEnum.EnterContextMenu).ToString());
        }

        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {
            if ((MyGuiScreenGamePlay.Static.IsEditorActive() == true) && (MyGuiManager.IsScreenOfTypeOpen(typeof(MyGuiScreenEditorContextMenu)) == false))
            {
                m_enabled = true;
                m_contextMenu = null;
            }

            if (!IsEnabled) return false;

            if (input.IsEditorControlNewPressed(MyEditorControlEnums.ENTER_CONTEXT_MENU))
            {
                m_contextMenu = new MyGuiScreenEditorContextMenu();
                MyGuiManager.AddScreen(m_contextMenu);
                m_enabled = false;
            }

            if (m_contextMenu != null)
            {
                m_contextMenu.HandleInput(input, receivedFocusInThisUpdate);
            }

            return base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, receivedFocusInThisUpdate);
        }

        public override void Draw()
        {
            base.Draw();

            if (!IsEnabled) return;

            MyGuiManager.BeginSpriteBatch();
            if (m_text.Length > 0)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontGuiImpactLarge(), m_text, MyGuiManager.GetScreenTextLeftBottomPosition(), MyGuiConstants.LABEL_TEXT_SCALE * 0.93f,
                            Color.Red, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }
            MyGuiManager.EndSpriteBatch();
        }
    }
}

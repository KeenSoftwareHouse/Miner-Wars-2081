using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Localization;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Textures;
using MinerWars.CommonLIB.AppCode.Utils;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    enum MyInventoryAmountTextAlign
    {
        BottomRight,
        MiddleRight,
    }

    class MyGuiHelperBase
    {
        private const string LINE_SPLIT_SEPARATOR = " | ";
        private static readonly string[] LINE_SEPARATOR = new string[] { System.Environment.NewLine };

        protected MyTexture2D m_icon;

        private MyTextsWrapperEnum m_descriptionEnum;
        public MyTextsWrapperEnum DescriptionEnum 
        {
            get { return m_descriptionEnum; }
            set 
            {
                if (value != m_descriptionEnum) 
                {
                    m_descriptionEnum = value;
                    UpdateMultilineDescription();
                    m_nameIsDirty = true;
                }
            }
        }

        private bool m_nameIsDirty = false;
        private StringBuilder m_multiLineDescription = new StringBuilder();
        public StringBuilder MultiLineDescription
        {
            get
            {
                if (m_nameIsDirty)
                {
                    UpdateMultilineDescription();
                    UpdateName();
                    m_nameIsDirty = false;
                }
                return m_multiLineDescription;
            }
        }

        protected StringBuilder m_name = new StringBuilder();
        public StringBuilder Name 
        { 
            get 
            {
                if (m_nameIsDirty) 
                {
                    UpdateName();
                    m_nameIsDirty = false;
                }
                return m_name; 
            } 
        }

        public bool IsNameDirty
        {
            get
            {
                return m_nameIsDirty;
            }
            set
            {
                m_nameIsDirty = value;
            }
        }

        public StringBuilder Description
        {
            get
            {
                if (m_multiLineDescription != null && m_multiLineDescription.Length > 0)
                    return m_multiLineDescription;
                return MyTextsWrapper.Get(DescriptionEnum);
            }
        }
        public virtual MyTexture2D Icon { get { return m_icon; } set { m_icon = value; } }
        public MyInventoryAmountTextAlign InventoryTextAlign { get; set; }

        public MyGuiHelperBase(MyTextsWrapperEnum description)
        {
            DescriptionEnum = description;
        }

        public MyGuiHelperBase(MyTexture2D icon, MyTextsWrapperEnum description)
        {
            m_icon = icon;
            DescriptionEnum = description;
        }

        public MyGuiHelperBase(MyTexture2D icon, string description)
        {
            m_icon = icon;
            m_multiLineDescription = new StringBuilder(description);
        }

        public virtual void DrawSpriteBatch(Vector2 normalizedCoord, Vector2 normalizedSize, Color color, MyGuiDrawAlignEnum drawAlign)
        {
            if (m_icon != null)
            {
                MyGuiManager.DrawSpriteBatch(m_icon, normalizedCoord, normalizedSize, color, drawAlign);
            }
        }

        private void UpdateMultilineDescription() 
        {
            m_multiLineDescription.Clear();
            MyMwcUtils.SplitStringBuilder(m_multiLineDescription, Description, LINE_SPLIT_SEPARATOR);
        }

        protected virtual void UpdateName() 
        {
            m_name.Clear();
            string firstLine = m_multiLineDescription.ToString().Split(LINE_SEPARATOR, System.StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
            if (firstLine.Length > 0)
            {
                char firstChar = firstLine[0];
                // try make first char as upper
                if (firstChar >= 97 && firstChar <= 122) 
                {
                    firstChar = (char)(firstChar - 32);
                }

                m_name.Append(firstChar);
                for (int i = 1; i < firstLine.Length; i++)
                {
                    m_name.Append(firstLine[i]);
                }
            }
        }
    }
}

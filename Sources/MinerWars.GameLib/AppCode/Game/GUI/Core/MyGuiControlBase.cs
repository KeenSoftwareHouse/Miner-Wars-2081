using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using System.Text;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Textures;
using MinerWars.AppCode.Toolkit.Input;


namespace MinerWars.AppCode.Game.GUI.Core
{
    enum MyGuiControlPreDefinedSize
    {
        SMALL,
        MEDIUM,
        LONGMEDIUM,
        LARGE,
    }

    enum MyGuiControlHighlightType
    {
        NEVER,
        WHEN_CURSOR_OVER,
        WHEN_ACTIVE
    }

    delegate void OnVisibilityChanged(MyGuiControlBase sender, bool isVisible);

    abstract class MyGuiControlBase
    {
        public bool Enabled;        // false to disable control, disabled controls are skipped when switching with Tab key etc., look implemented atm. only in MyGuiControlButton
        public bool DrawWhilePaused = true;
        //public bool Visible;        // false to hide control, control will not handle input and will not call draw        

        protected IMyGuiControlsParent m_parent;                //  Parent that contains this control
        protected Vector2 m_position;                            //  Position of control's center (normalized and relative to parent screen center (not left/top corner!!!))
        protected MyGuiControlPreDefinedSize? m_predefinedSize;  //  Predefined enum that controls size of the control
        protected Vector2? m_size;                               //  Size of control (normalized)
        protected Vector4? m_backgroundColor;
        //protected StringBuilder m_toolTip;
        protected MyToolTips m_toolTip;
        protected object m_UserData;                             //Specific user data for this control
        protected MyGuiControlHighlightType m_highlightType;
        
        protected bool m_canHandleKeyboardActiveControl = false;        //  By default controls can accept keyboard active control
        //protected bool m_hasKeyboardActiveControlInPrevious = false;     //  Set to true so we won't hear false mouse over sound when screen loads. This isn't hack, it's OK.
        protected bool m_hasKeyboardActiveControl = false;               //  Set to true so we won't hear false mouse over sound when screen loads. This isn't hack, it's OK.
        bool m_isActiveControl = true;              // There are some controls, that cannot receive any handle input(control panel for example), thus disable them with this
        bool m_isMouseOver = false;                 // Status of mouse over in this update
        bool m_containsMouse = false;               // Status of mouse over in this update, it's set even for inactive controls
        bool m_isMouseOverInPrevious = false;       // Status of mouse over in previous update
        protected bool m_showToolTip = false;
        int m_showToolTipDelay;
        protected bool m_mouseButtonPressed = false;
        
        protected float m_scale = 1.0f;

        protected MyTexture2D m_controlTexture;
        protected MyTexture2D m_hoverTexture;
        protected MyTexture2D m_pressedTexture;

        protected Vector2 m_toolTipPosition;
        //protected Vector2 m_toolTipSize;
        //protected Vector2 m_toolTipFogPosition;
        //protected Vector2 m_toolTipFadeSize;

        protected MyTexture2D m_mouseCursorHoverTexture;
        protected MyTexture2D m_mouseCursorPressedTexture;
        /*
        System.Drawing.Bitmap m_mouseCursorHoverBitmap;
        System.Drawing.Bitmap m_mouseCursorPressedBitmap;
        */
        private bool m_visible;
        public virtual bool Visible
        {
            get { return m_visible; }
            set
            {
                bool changed = value != m_visible;                
                m_visible = value;

                if (changed && OnVisibilityChanged != null)
                {
                    OnVisibilityChanged(this, m_visible);
                }
            } 
        }

        public event OnVisibilityChanged OnVisibilityChanged;

        private MyGuiControlBase() { }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4? backgroundColor, StringBuilder toolTip) :
            this(parent, position, size, backgroundColor, toolTip, null, null, null, true)
        {

        }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4? backgroundColor, StringBuilder toolTip, bool isActiveControl) :
            this(parent, position, size, backgroundColor, toolTip, null, null, null, isActiveControl)
        {
        }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, Vector4? backgroundColor, StringBuilder toolTip, bool isActiveControl) :
            this(parent, position, predefinedSize, backgroundColor, toolTip, null, null, null, isActiveControl)
        {

        }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, Vector4? backgroundColor, StringBuilder toolTip,
            MyTexture2D controlTexture, MyTexture2D hoverTexture, MyTexture2D pressedTexture, bool isActiveControl) :
            this(parent, position, backgroundColor, toolTip, controlTexture, hoverTexture, pressedTexture, isActiveControl, null, null/*, null, null*/)
        {
            m_predefinedSize = predefinedSize;
            m_size = GetPredefinedControlSize();
        }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4? backgroundColor, StringBuilder toolTip,
            MyTexture2D controlTexture, MyTexture2D hoverTexture, MyTexture2D pressedTexture, bool isActiveControl) :
            this(parent, position, size, backgroundColor, toolTip, controlTexture, hoverTexture, pressedTexture, isActiveControl, MyGuiControlHighlightType.WHEN_ACTIVE)
        {
        }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4? backgroundColor, StringBuilder toolTip,
            MyTexture2D controlTexture, MyTexture2D hoverTexture, MyTexture2D pressedTexture, bool isActiveControl, MyGuiControlHighlightType highlightType) :
            this(parent, position, size, backgroundColor, toolTip, controlTexture, hoverTexture, pressedTexture, isActiveControl, highlightType, null, null/*, null, null*/)
        {            
        }

        public MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, Vector2? size, Vector4? backgroundColor, StringBuilder toolTip,
            MyTexture2D controlTexture, MyTexture2D hoverTexture, MyTexture2D pressedTexture, bool isActiveControl, MyGuiControlHighlightType highlightType,
            MyTexture2D mouseCursorHoverTexture, MyTexture2D mouseCursorPressedTexture/*,
            System.Drawing.Bitmap mouseCursorHoverBitmap, System.Drawing.Bitmap mouseCursorPressedBitmap*/) :
            this(parent, position, backgroundColor, toolTip, controlTexture, hoverTexture, pressedTexture, isActiveControl, mouseCursorHoverTexture, mouseCursorPressedTexture/*, mouseCursorHoverBitmap, mouseCursorPressedBitmap*/)
        {
            m_size = size;
            m_highlightType = highlightType;
        }

        private MyGuiControlBase(IMyGuiControlsParent parent, Vector2 position, Vector4? backgroundColor, StringBuilder toolTip,
            MyTexture2D controlTexture, MyTexture2D hoverTexture, MyTexture2D pressedTexture, bool isActiveControl, 
            MyTexture2D mouseCursorHoverTexture, MyTexture2D mouseCursorPressedTexture/*,
            System.Drawing.Bitmap mouseCursorHoverBitmap, System.Drawing.Bitmap mouseCursorPressedBitmap*/) 
        {
            Visible = true;
            Enabled = true;
            m_parent = parent;
            m_position = position;
            m_backgroundColor = backgroundColor;
            //m_toolTip = toolTip;
            if (toolTip != null)
            {
                m_toolTip = new MyToolTips(toolTip);
            }
            m_controlTexture = controlTexture;
            m_hoverTexture = hoverTexture;
            m_pressedTexture = pressedTexture;
            m_isActiveControl = isActiveControl;
            m_mouseCursorHoverTexture = mouseCursorHoverTexture;
            m_mouseCursorPressedTexture = mouseCursorPressedTexture;
            /*
            m_mouseCursorHoverBitmap = mouseCursorHoverBitmap;
            m_mouseCursorPressedBitmap = mouseCursorPressedBitmap;
            */
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public virtual bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool receivedFocusInThisUpdate)
        {            
            if (m_isActiveControl)
            {
                //m_hasKeyboardActiveControlInPrevious = m_hasKeyboardActiveControl;
                m_hasKeyboardActiveControl = hasKeyboardActiveControl;

                m_isMouseOverInPrevious = m_isMouseOver;
                m_isMouseOver = CheckMouseOver();
                m_mouseButtonPressed = IsMouseOver() && input.IsLeftMousePressed();

                if (((m_isMouseOver == true) && (m_isMouseOverInPrevious == false)) ||
                    ((m_hasKeyboardActiveControl == true) && (hasKeyboardActiveControlPrevious == false)))
                {
                    if (receivedFocusInThisUpdate == false)
                    {
                        MyAudio.AddCue2D(MySoundCuesEnum.GuiMouseOver);
                    }
                }
            }

            // if mouseover this control longer than specified period, show tooltip for the control
            if ((m_isMouseOver == true) && (m_isMouseOverInPrevious == true))
            {
                if (m_showToolTip == false)
                {
                    m_showToolTipDelay = MyMinerGame.TotalGamePlayTimeInMilliseconds + MyGuiConstants.SHOW_CONTROL_TOOLTIP_DELAY;
                    m_showToolTip = true;
                }
            }
            else
            {
                m_showToolTip = false;
            }

            return false;
        }

        public bool CanHandleKeyboardActiveControl()
        {
            return m_canHandleKeyboardActiveControl;
        }

        //  Checks if mouse cursor is over control
        protected virtual bool CheckMouseOver()
        {
            //  If size isn't specified, this test can't be done and that was probably intend
            if (m_size.HasValue == false) return false;

            return CheckMouseOver(m_size.Value * m_scale, m_parent.GetPositionAbsolute());
        }

        protected bool CheckMouseOver(Vector2 size, Vector2 position)
        {
            Vector2 min = new Vector2(position.X + m_position.X - (size.X / 2.0f), position.Y + m_position.Y - (size.Y / 2.0f));
            Vector2 max = new Vector2(position.X + m_position.X + (size.X / 2.0f), position.Y + m_position.Y + (size.Y / 2.0f));

            return ((MyGuiManager.MouseCursorPosition.X >= min.X) && (MyGuiManager.MouseCursorPosition.X <= max.X) && (MyGuiManager.MouseCursorPosition.Y >= min.Y) && (MyGuiManager.MouseCursorPosition.Y <= max.Y));
        }        

        public bool IsMouseOver()
        {
            return m_isMouseOver;
        }

        public virtual bool ContainsMouse()
        {
            return m_containsMouse;
        }

        protected MyGuiScreenBase GetTopMostParentScreen() 
        {
            IMyGuiControlsParent currentParent = m_parent;
            while(!(currentParent is MyGuiScreenBase))
            {
                currentParent = ((MyGuiControlBase)currentParent).GetParent();
            }
            return currentParent as MyGuiScreenBase;
        }

        protected bool IsMouseOverOrKeyboardActive()
        {
            MyGuiScreenBase topMostParentScreen = GetTopMostParentScreen();
            return ((topMostParentScreen is MyGuiScreenBase &&
                ((MyGuiScreenBase)topMostParentScreen).GetState() == MyGuiScreenState.OPENED) && 
                ((m_isMouseOver) || (m_hasKeyboardActiveControl&&m_canHandleKeyboardActiveControl)));
        }

        public virtual void Update()
        {
            m_containsMouse = CheckMouseOver();
        }

        public Color GetColorAfterTransitionAlpha(Vector4 color)
        {
            Vector4 ret = color;
            ret.W *= m_parent.GetTransitionAlpha();
            return new Color(ret);
        }

        public IMyGuiControlsParent GetParent() 
        {
            return m_parent;
        }

        public Vector2 GetPosition()
        {
            return m_position;
        }

        public Vector2? GetSize()
        {
            return m_size;
        }

        public virtual void Draw()
        {
            if (m_backgroundColor.HasValue)
            {
                MyTexture2D controlTexture = MyGuiManager.GetBlankTexture();
                Vector4 backgroundColor = m_backgroundColor.Value;

                if (m_controlTexture != null)
                {
                    controlTexture = m_controlTexture;

                    if (m_mouseButtonPressed && m_pressedTexture != null)
                    {
                        controlTexture = m_pressedTexture;
                    }
                    else if (IsMouseOverOrKeyboardActive())
                    {
                        if ((m_highlightType == MyGuiControlHighlightType.WHEN_ACTIVE) || (m_highlightType == MyGuiControlHighlightType.WHEN_CURSOR_OVER && IsMouseOver()) || (m_hasKeyboardActiveControl && m_canHandleKeyboardActiveControl))
                        {
                            backgroundColor = m_backgroundColor.Value * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
                        }
                    }
                }

                if (!Enabled)
                    backgroundColor = m_backgroundColor.Value * MyGuiConstants.DISABLED_BUTTON_COLOR_VECTOR; 

                MyGuiManager.DrawSpriteBatch(controlTexture, m_parent.GetPositionAbsolute() + m_position, m_size.Value * m_scale, GetColorAfterTransitionAlpha(backgroundColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
        }

        protected void DrawEffectHighlight()
        {
        }


        public virtual void HideToolTip()
        {
            m_showToolTip = false;
        }        

        public virtual void ShowToolTip()
        {
            //show tooltip
            if (m_showToolTip)
            {
                if ((MyMinerGame.TotalGamePlayTimeInMilliseconds > m_showToolTipDelay) && (m_toolTip != null) && (m_toolTip.GetToolTips().Count > 0))
                {
                    m_toolTipPosition = MyGuiManager.MouseCursorPosition + MyGuiConstants.TOOL_TIP_RELATIVE_DEFAULT_POSITION;
                    m_toolTip.Draw(m_toolTipPosition);
                }
            }
        }

        protected virtual Vector2 GetPredefinedControlSize()
        {
            return Vector2.Zero;
        }

        public object UserData
        {
            get { return m_UserData; }
            set { m_UserData = value; }
        }

        protected void DrawBorders(Vector2 topLeftPosition, Vector2 size, Color color, int borderSize)
        {
            Vector2 sizeInPixels = MyGuiManager.GetScreenSizeFromNormalizedSize(size);
            sizeInPixels = new Vector2((int)sizeInPixels.X, (int)sizeInPixels.Y);
            Vector2 leftTopInPixels = MyGuiManager.GetScreenCoordinateFromNormalizedCoordinate(topLeftPosition);
            leftTopInPixels = new Vector2((int)leftTopInPixels.X, (int)leftTopInPixels.Y);
            Vector2 rightTopInPixels = leftTopInPixels + new Vector2(sizeInPixels.X, 0);
            Vector2 leftBottomInPixels = leftTopInPixels + new Vector2(0, sizeInPixels.Y);
            // top
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftTopInPixels.X, (int)leftTopInPixels.Y, (int)sizeInPixels.X, borderSize, color);
            // right
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)rightTopInPixels.X - borderSize, (int)rightTopInPixels.Y + borderSize, borderSize, (int)sizeInPixels.Y - borderSize * 2, color);
            // bottom
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftBottomInPixels.X, (int)leftBottomInPixels.Y - borderSize, (int)sizeInPixels.X, borderSize, color);
            //left
            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), (int)leftTopInPixels.X, (int)leftTopInPixels.Y + borderSize, borderSize, (int)sizeInPixels.Y - borderSize * 2, color);
        }

        public MyTexture2D GetMouseCursorTexture()
        {
            // this is default mouse cursor texture
            MyTexture2D mouseCursorTexture = MyGuiManager.GetMouseCursorTexture();//null;
            if (IsMouseOver())
            {
                // when mouse button pressed and mouse cursor texture for pressed is not null
                if (m_mouseButtonPressed && m_mouseCursorPressedTexture != null)
                {
                    mouseCursorTexture = m_mouseCursorPressedTexture;
                }
                // when mouse over control and mouse cursor texture for hover is not null
                else if(m_mouseCursorHoverTexture != null)
                {
                    mouseCursorTexture = m_mouseCursorHoverTexture;
                }
            }
            return mouseCursorTexture;
        }

        public virtual void SetPosition(Vector2 position)
        {
            m_position = position;
        }
    }
}

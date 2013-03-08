using System.Collections;
using System.Text;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Audio;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.Textures;
using System.Linq;


namespace MinerWars.AppCode.Game.GUI.Core
{
    using System;
    using MinerWars.AppCode.Game.Managers;

    enum MyGuiScreenState
    {
        OPENING,
        OPENED,
        CLOSING,
        CLOSED,
        HIDING,
        UNHIDING,
        HIDDEN
    }

    /// <summary>
    /// Generic screen results
    /// </summary>
    internal enum ScreenResult
    {
        /// <summary>
        /// Ok
        /// </summary>
        Ok,
        /// <summary>
        /// Cancel
        /// </summary>
        Cancel,
    }

    abstract class MyGuiScreenBase : MyGuiManager.Friend, IMyGuiControlsParent
    {
        protected Action OnEnterCallback;

        public delegate void ScreenHandler(MyGuiScreenBase source);
        public bool SkipTransition { get; set; }

        protected float m_transitionAlpha;
        protected MyGuiScreenState m_state;
        //public List<MyGuiControlBase> Controls = new List<MyGuiControlBase>();
        //public MyGuiControlsCollection Controls = new MyGuiControlsCollection();
        private MyGuiControls m_controls = new MyGuiControls();
        protected Vector2 m_position;
        protected Vector4 m_backgroundFadeColor; 

        //  If true then no other screen can be added above this one. They will be always added below.
        protected bool m_isTopMostScreen = false;

        //  Draw fade rectangle under top-most screen?
        protected bool m_enableBackgroundFade;
        
        //  Background color of this screen. If not specified, background rectangle won't be drawn (this is default option).
        protected Vector4? m_backgroundColor;

        // Background texture. If not specified, default screen background texture is used
        protected MyTexture2D m_backgroundTexture;
        protected static MyTexture2D m_minerWarsLogoTexture;
        //protected static MyTexture2D m_minerWarsActorsTexture;

        protected bool m_canCloseInCloseAllScreenCalls = true;

        protected bool m_windowIsMovable = false;
        private bool m_draged = false;
        private Vector2 m_mouseDragEntryPoint;
        
        
        //  Normalized size of this screen. If you don't need size (this is full-screen screen), set it to null.
        protected Vector2? m_size;

        //  Automaticaly closes this screen when ESC is pressed (e.g. for game-play screen we don't want this functionality)
        protected bool m_closeOnEsc = true;

        private bool m_drawMouseCursor = true;    //  Every screen can define if mouse cursor is drawn when this screen has focus. By default true, so who don't want - must set to false.
        protected bool DrawMouseCursor
        {
            set
            {
                /*
                if (value == false && MyGuiManager.GetScreenWithFocus() == this)
                    MyGuiInput.SetMouseToScreenCenter();//preclear this
                */
                m_drawMouseCursor = value;
            }
            get
            {
                return m_drawMouseCursor;
            }
        }

        int m_lastTransitionTime;

        //  Don't change 'm_isLoaded' directly - because it can be used by other threads. Instead use property IsLoaded (it's thread-safe)
        bool m_isLoaded = false;
        object m_isLoadedLock = new object();

        //  Server to check if this screen is before or after first update, so we can do some sort of initialization
        bool m_firstUpdateServed = false;

        protected bool m_drawEvenWithoutFocus = false;  // Some screens must be drawn even if they do not have focus(gameplay screen for example)
        protected bool m_canHaveFocus = true; //Without focus, this screen will not steal input
        protected bool m_canShareInput = false; //If input is going to be shared, screen can refuse it

        protected bool m_screenCanHide;
        protected bool m_allowUnhidePreviousScreen;

        protected MySoundCuesEnum? m_openingCueEnum;
        protected MySoundCuesEnum? m_closingCueEnum;
        private MySoundCue? m_openingCue;
        private MySoundCue? m_closingCue;

        private MyGuiControlBase m_draggingControl;
        private Vector2 m_draggingControlOffset;
        private StringBuilder m_drawPositionSb = new StringBuilder();

        private IList<MyGuiControlBase> m_controlsAll = new List<MyGuiControlBase>();       // collection of all controls (recursive)
        private IList<MyGuiControlBase> m_controlsVisible = new List<MyGuiControlBase>();   // collection of all visible controls (recursive)
        private Queue<MyGuiControlBase> m_controlsQueue = new Queue<MyGuiControlBase>();    // helper queue for finding controls recursive
        private int m_keyboardControlIndex = -1;   //  By default is -1, so first call to HandleKeyboardActiveIndex() will move it to first control that can accept it
        private int m_keyboardControlIndexPrevious = -1;    //  In previous update or handle-input
        private MyGuiControlListboxDragAndDrop m_listboxDragAndDropHandlingNow;
        private MyGuiControlCombobox m_comboboxHandlingNow;
        private MyGuiControlBase m_lastHandlingControl;

        readonly StringBuilder APP_VERSION = new StringBuilder(MyMwcBuildNumbers.ConvertBuildNumberFromIntToString(MyMwcFinalBuildConstants.APP_VERSION).Replace(MyMwcBuildNumbers.SEPARATOR, ".") + " DX, " + MyMwcSecrets.MOD_NAME);
        readonly StringBuilder UNOFFICIAL_MOD = new StringBuilder("Unofficial modification of Miner Wars 2081");

        /// <summary>
        /// Occurs when [closed].
        /// </summary>
        public event ScreenHandler Closed;

        public bool Canceled { get; private set; }

        //  Prohibited!
        private MyGuiScreenBase() { }

        protected MyGuiScreenBase(Vector2 position, Vector4? backgroundColor, Vector2? size) : this(position, backgroundColor, size, false, null)
        {
        }

        protected MyGuiScreenBase(Vector2 position, Vector4? backgroundColor, Vector2? size, bool isTopMostScreen, MyTexture2D backgroundTexture) 
        {
            m_backgroundFadeColor = MyGuiConstants.SCREEN_BACKGROUND_FADE_BLANK_DARK;
            m_position = position;
            m_backgroundColor = backgroundColor;
            m_size = size;
            m_isTopMostScreen = isTopMostScreen;
            m_enableBackgroundFade = false;
            m_backgroundTexture = backgroundTexture;
            m_screenCanHide = true;
            m_allowUnhidePreviousScreen = true;
            m_controls.CollectionChanged += Controls_CollectionChanged;
            m_state = MyGuiScreenState.OPENING;
            m_lastTransitionTime = MyMinerGame.TotalTimeInMilliseconds;
        }

        private void Controls_CollectionChanged(object sender, EventArgs e)
        {
            FillControlsResurcive();
        }

        /// <summary>
        /// For displaying in the list in the debug screen.
        /// </summary>
        /// <returns></returns>
        public abstract string GetFriendlyName();

        //  In all methods overiding this one, call this base LoadContent as last, after child's
        public virtual void LoadContent()
        {
            IsLoaded = true;
            m_lastTransitionTime = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public virtual void LoadData()
        {
        }

        public virtual void UnloadContent()
        {
            MyMwcLog.WriteLine("MyGuiScreenBase.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            IsLoaded = false;
            if (m_backgroundTexture != null && m_backgroundTexture.LoadState == LoadState.Loaded)
            {
                MyTextureManager.UnloadTexture(m_backgroundTexture);
                //m_backgroundTexture = null;
            }
            if (m_minerWarsLogoTexture != null && m_minerWarsLogoTexture.LoadState == Managers.LoadState.Loaded)
            {
                MyTextureManager.UnloadTexture(m_minerWarsLogoTexture);
                //m_minerWarsLogoTexture = null;
            }
            /*if (m_minerWarsActorsTexture != null && m_minerWarsActorsTexture.LoadState == Managers.LoadState.Loaded)
            {
                MyTextureManager.UnloadTexture(m_minerWarsActorsTexture);
                //m_minerWarsLogoTexture = null;
            }*/

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGuiScreenBase.UnloadContent - END");
        }

        public virtual void UnloadData()
        {
        }

        //  This method cab be overrided and used in background thread for loading/unloading the screeen
        public virtual void RunBackgroundThread()
        {
        }

        public MyGuiScreenState GetState()
        {
            return m_state;
        }

        public void SetState(MyGuiScreenState value)
        {
            m_state = value;
        }

        public bool GetDrawMouseCursor()
        {
            return m_drawMouseCursor;
        }

        //  If true then no other screen can be added above this one. They will be always added below.
        public bool IsTopMostScreen()
        {
            return m_isTopMostScreen;
        }

        public bool GetDrawScreenEvenWithoutFocus()
        {
            return m_drawEvenWithoutFocus;
        }

        public void SetControlIndex(int index)
        {
            //Controls.KeyboardControlIndex = index;
            m_keyboardControlIndex = index;
        }
        public void SetPrevControlIndex(int index)
        {
            //Controls.KeyboardControlIndexPrevious = index;
            m_keyboardControlIndexPrevious = index;
        }

        public virtual bool CanHandleInputDuringTransition()
        {
            return false;
        }

        public virtual void HandleInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            //  Here we can make some one-time initialization hidden in update
            bool isThisFirstHandleInput = !m_firstUpdateServed;

            if (m_firstUpdateServed == false && m_keyboardControlIndex == -1) //m_keyboardControlIndex could be set from constructor
            {
                //  Move to first control that can accept it
                //Controls.HandleKeyboardActiveIndex(true);
                HandleKeyboardActiveIndex(true);

                //  Never again call this update-initialization
                m_firstUpdateServed = true;
            }

            // if we are dragging some control, then we don't pass this focus to controls
            if(MyFakes.CONTROLS_MOVE_ENABLED && input.IsKeyPress(Keys.M))
            {
                if(HandleControlMoving(input))
                {
                    return;
                }
            }

            if (MyModelsStatisticsConstants.GET_MODEL_STATISTICS_AUTOMATICALLY)
            {
                if(MyMinerGame.IsGameReady)
                    Models.MyModels.LogUsageInformation();
            }

            if (!HandleControlsInput(input, receivedFocusInThisUpdate))
            {
                bool handled = false;
                //  If input wasn't completely handled or captured by some control, only then we can handle screen's input
                if ((input.IsKeyPress(Keys.LeftShift) && (input.IsNewKeyPress(Keys.Tab))) || (input.IsNewKeyPress(Keys.Up)) || (input.IsNewGamepadKeyUpPressed()))
                {
                    handled = HandleKeyboardActiveIndex(false);
                }
                else if ((input.IsNewKeyPress(Keys.Tab)) || (input.IsNewKeyPress(Keys.Down)) || (input.IsNewGamepadKeyDownPressed()))
                {
                    handled = HandleKeyboardActiveIndex(true);
                }
                else if ((m_closeOnEsc == true) && ((input.IsNewKeyPress(Keys.Escape) || input.IsJoystickButtonNewPressed(MyJoystickButtonsEnum.J02))))
                {
                    Canceling();
                }
                else if ((OnEnterCallback != null) && (input.IsNewKeyPress(Keys.Enter)))
                {
                    OnEnterCallback();
                }

                // Scrolling down/up between controls using mouse scroll wheel
                //if (input.PreviousMouseScrollWheelValue() > input.MouseScrollWheelValue())
                //{
                //    handled = HandleKeyboardActiveIndex(true);
                //}
                //else if (input.PreviousMouseScrollWheelValue() < input.MouseScrollWheelValue())
                //{
                //    handled = HandleKeyboardActiveIndex(false);
                //}

                if (!handled)
                {
                    HandleUnhandledInput(input, receivedFocusInThisUpdate);
                }
            }
        }

        private bool HandleControlMoving(MyGuiInput input)
        {
            bool dragging = false;
            
            if (input.IsLeftMousePressed())
            {
                // if we dragging control, then we set it new position by mouse
                if (m_draggingControl != null)
                {
                    m_draggingControl.SetPosition(MyGuiManager.MouseCursorPosition - m_draggingControl.GetParent().GetPositionAbsolute() - m_draggingControlOffset);
                    dragging = true;
                }
                // if we are not dragging control, then we try find it
                else
                {
                    MyGuiControlBase controlToDrag = null;
                    // first we try find control, which has mouse over
                    controlToDrag = GetMouseOverControl();                    
                    // if there is no control which has mouse over, then we try find control, which is under mouse cursor (because some controls has no mouse over all time)
                    if (controlToDrag == null)
                    {
                        controlToDrag = GetControlUnderMouseCursor();
                    }

                    // if we found any contorl to drag, then we set it to dragging control
                    if(controlToDrag != null)
                    {
                        m_draggingControl = controlToDrag;
                        m_draggingControlOffset = MyGuiManager.MouseCursorPosition - m_draggingControl.GetParent().GetPositionAbsolute() - controlToDrag.GetPosition();
                        dragging = true;
                    }
                }
            }
            else
            {
                m_draggingControl = null;
                if (input.IsNewLeftMouseReleased())
                {
                    dragging = true;
                }
            }            

            return dragging;
        }

        private bool IsControlUnderCursor(MyGuiControlBase control)
        {
            Vector2? size = control.GetSize();
            if (size != null)
            {
                Vector2 min = control.GetPosition() - size.Value / 2;
                Vector2 max = control.GetPosition() + size.Value / 2;
                Vector2 mousePosition = MyGuiManager.MouseCursorPosition - GetPosition();

                return (mousePosition.X >= min.X && mousePosition.X <= max.X &&
                        mousePosition.Y >= min.Y && mousePosition.Y <= max.Y);
            }
            else
            {
                return false;
            }
        }

        protected bool IsMouseOver()
        {
            Vector2 borderOffsetTopLeft = new Vector2(0.07f, 0.05f);
            Vector2 borderOffsetBottomRight = new Vector2(0.07f, 0.1f);
            Vector2 min = m_position - m_size.Value / 2 + borderOffsetTopLeft;
            Vector2 max = m_position + m_size.Value / 2 - borderOffsetBottomRight;

            return (MyGuiManager.MouseCursorPosition.X >= min.X && MyGuiManager.MouseCursorPosition.X <= max.X && 
                MyGuiManager.MouseCursorPosition.Y >= min.Y && MyGuiManager.MouseCursorPosition.Y <= max.Y);
        }

        public virtual void HandleUnhandledInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            // by calling this we are soure that no control handled input already 
            if (m_windowIsMovable)
            {
                if (input.IsNewLeftMousePressed() && IsMouseOver())
                {
                    m_draged = true;
                    m_mouseDragEntryPoint = new Vector2(MyGuiManager.MouseCursorPosition.X, MyGuiManager.MouseCursorPosition.Y);
                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall("drag");
                }

                if (input.IsLeftMousePressed() && m_draged)
                {

                    Vector2 newMousePoint = new Vector2(MyGuiManager.MouseCursorPosition.X, MyGuiManager.MouseCursorPosition.Y);
                    Vector2 deltaPos = newMousePoint - m_mouseDragEntryPoint;
                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall( deltaPos.ToString() );

                    m_mouseDragEntryPoint = newMousePoint;
                    m_position += deltaPos;
                    
                }

                if (input.IsNewLeftMouseReleased())
                {
                    m_draged = false;
                    //KeenSoftwareHouse.Library.Trace.Trace.SendMsgLastCall("undrag");
                }
            }        
        }

        /// <summary>
        /// Called when user presses ESC or clicks on CANCEL - hook to this method so you can do gui-screen-specific event
        /// </summary>
        protected virtual void Canceling()
        {
            Canceled = true;
            MySoundCuesEnum cancelEnum = m_closingCueEnum.HasValue ? m_closingCueEnum.Value : MySoundCuesEnum.GuiMouseClick;
            MyAudio.AddCue2D(cancelEnum);
            CloseScreen();
        }

        protected void AddCaption(MyTextsWrapperEnum textEnum, Vector2? captionOffset = null, float captionScale = MyGuiConstants.SCREEN_CAPTION_TEXT_SCALE)
        {
            AddCaption(textEnum, MyGuiConstants.SCREEN_CAPTION_TEXT_COLOR, captionOffset, captionScale);
        }

        protected void AddCaption(MyTextsWrapperEnum textEnum, Vector4 captionTextColor, Vector2? captionOffset = null, float captionScale = MyGuiConstants.SCREEN_CAPTION_TEXT_SCALE)
        {
            Controls.Add(new MyGuiControlLabel(this, 
                new Vector2(0, -m_size.Value.Y / 2.0f + MyGuiConstants.SCREEN_CAPTION_DELTA_Y) + (captionOffset != null ? captionOffset.Value : Vector2.Zero),
                null, textEnum, captionTextColor, captionScale,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER,MyGuiManager.GetFontMinerWarsBlue()));
        }

        protected void AddCaption(System.Text.StringBuilder text, Vector4 captionTextColor, Vector2? captionOffset = null, float captionScale = MyGuiConstants.SCREEN_CAPTION_TEXT_SCALE)
        {
            Controls.Add(new MyGuiControlLabel(this,
                new Vector2(0, -m_size.Value.Y / 2.0f + MyGuiConstants.SCREEN_CAPTION_DELTA_Y) + (captionOffset != null ? captionOffset.Value : Vector2.Zero),
                null, text, captionTextColor, captionScale,
                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER));
        }

        protected void DrawMinerWarsLogo()
        {
            if (m_minerWarsLogoTexture != null)
            {
                //  Miner Wars logo
                Color colorForeground = new Color(new Vector4(1, 1, 1, 0.85f * m_transitionAlpha));
                MyGuiManager.DrawSpriteBatch(m_minerWarsLogoTexture, new Vector2(0.5f, 0.15f),
                                             new Vector2(1218/1600f, 355/1200f), colorForeground,
                                             MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }
        }

        /*protected void DrawActorsPhotos()
        {
            if (m_minerWarsActorsTexture != null)
            {
                //  Miner Wars actors
                Color colorForeground = new Color(new Vector4(1, 1, 1, 1f * m_transitionAlpha));

                MyGuiManager.DrawSpriteBatch(m_minerWarsActorsTexture, new Vector2(1.1f, -0.01f),
                                             new Vector2(323.3f / 1600f, 394.4f / 1200f), colorForeground,
                                             MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_TOP);
            }
        }*/

        protected void DrawAppVersion()
        {
            Vector2 textRightBottomPosition = MyGuiManager.GetScreenTextRightBottomPosition();
            textRightBottomPosition.X -= 0.03f;
            textRightBottomPosition.Y -= 0.06f;
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), APP_VERSION, textRightBottomPosition, MyGuiConstants.APP_VERSION_TEXT_SCALE,
                new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);

            textRightBottomPosition.Y += 0.03f;
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), UNOFFICIAL_MOD, textRightBottomPosition, MyGuiConstants.APP_VERSION_TEXT_SCALE,
                new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
        }

        protected void DrawGlobalVersionText()
        {
            //  Text "TEST BUILD"
            Color colorBuildType = new Color(230, 230, 230, 230) * m_transitionAlpha;
            Vector2 textRightBottomPosition = MyGuiManager.GetScreenTextRightBottomPosition();
            textRightBottomPosition.X -= 0.03f;
            textRightBottomPosition.Y -= 0.01f;
            Vector2 position = textRightBottomPosition;

            if (MyMwcFinalBuildConstants.IS_PUBLIC)
            {
                // There's no text for public
                //MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.xxx),
                //      position, MyGuiConstants.APP_GLOBAL_TEXT_SCALE, new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha)/*colorBuildType*/, MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
            }
            else if (MyMwcFinalBuildConstants.IS_TEST)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.TestBuild),
                        position, MyGuiConstants.APP_GLOBAL_TEXT_SCALE, new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
            }
            else if (MyMwcFinalBuildConstants.IS_DEVELOP)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), MyTextsWrapper.Get(MyTextsWrapperEnum.DevelopBuild),
                        position, MyGuiConstants.APP_GLOBAL_TEXT_SCALE, new Color(MyGuiConstants.LABEL_TEXT_COLOR * m_transitionAlpha), MyGuiDrawAlignEnum.HORISONTAL_RIGHT_AND_VERTICAL_BOTTOM);
            }
            else
            {
                throw new SysUtils.Utils.MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        //  This will tell us if LoadContent was finished so we can start using this screen
        //  It's used only for screens that have huge LoadContent - e.g. game play screen. Other don't have to use it.
        public bool IsLoaded
        {
            set
            {
                lock (m_isLoadedLock)
                {
                    m_isLoaded = value;
                }
            }
            
            get
            {
                bool ret;
                lock (m_isLoadedLock)
                {
                    ret = m_isLoaded;
                }
                return ret;
            }
        }

        public float GetTransitionAlpha()
        {
            return m_transitionAlpha;
        }

        public Vector2 GetPosition()
        {
            return m_position;
        }

        //  Use this for closing/quiting/existing screens
        //  Returns true or false to let child implementation know if it has to run its own version of close. It's because this method
        //  should be called only once (when screen starts closing) and then never.
        //  This will close screen with fade-out effect
        public virtual bool CloseScreen()
        {
            if ((m_state == MyGuiScreenState.CLOSING) || (m_state == MyGuiScreenState.CLOSED))
            {
                return false;
            }
            else
            {
                m_state = MyGuiScreenState.CLOSING;
                m_lastTransitionTime = MyMinerGame.TotalTimeInMilliseconds;
                return true;
            }
        }

        // Used in case, when screen is not closing, but needs to be transitioned out
        public virtual bool HideScreen()
        {
            if ((m_state == MyGuiScreenState.HIDING) || (m_state == MyGuiScreenState.HIDDEN) || (m_state == MyGuiScreenState.OPENING))
            {
                return false;
            }
            else
            {
                m_state = MyGuiScreenState.HIDING;
                m_lastTransitionTime = MyMinerGame.TotalTimeInMilliseconds;
                return true;
            }
        }

        // Used in case, when screen is hidden, and needs to be transitioned in
        public virtual bool UnhideScreen()
        {
            if ((m_state == MyGuiScreenState.UNHIDING) || (m_state == MyGuiScreenState.OPENED))
            {
                return false;
            }
            else
            {
                m_state = MyGuiScreenState.UNHIDING;
                m_lastTransitionTime = MyMinerGame.TotalTimeInMilliseconds;
                return true;
            }
        }

        //  This will close/remove screen instantly, without fade-out effect
        public virtual void CloseScreenNow()
        {
            if (m_state == MyGuiScreenState.CLOSED)
            {
                return;
            }
            
            m_state = MyGuiScreenState.CLOSED;
            
            //  Notify GUI manager that this screen should be removed from the list
            RemoveScreen(this);

            OnClosed();

            if (Closed != null)
            {
                Closed(this);
            }
        }

        //// Used in case, when new screen is added to hide tooltips of other screens
        //public void HideTooltips()
        //{
        //    Controls.HideTooltips();
        //}

        //  Returns true or false to let child implementation know if it has to run its own version of draw.
        public virtual bool Update(bool hasFocus)
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("GuiScreenBase");

            UpdateTransition();
            //FillControlsResurcive();
            UpdateControls();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return true;
        }

        void UpdateTransition()
        {
            if (m_state == MyGuiScreenState.OPENING || m_state == MyGuiScreenState.UNHIDING)
            {
                int deltaTime = MyMinerGame.TotalTimeInMilliseconds - m_lastTransitionTime;

                // Play opening sound
                if (m_state == MyGuiScreenState.OPENING && m_openingCueEnum != null && m_openingCue == null) m_openingCue = MyAudio.AddCue2D(m_openingCueEnum.Value);

                if (deltaTime >= GetTransitionOpeningTime())
                {
                    //  Opening phase finished, we are now in active state
                    m_state = MyGuiScreenState.OPENED;
                    m_transitionAlpha = MyGuiConstants.TRANSITION_ALPHA_MAX;
                    m_openingCue = null;

                    OnShow();
                }
                else
                {
                    m_transitionAlpha = MathHelper.Lerp(MyGuiConstants.TRANSITION_ALPHA_MIN, MyGuiConstants.TRANSITION_ALPHA_MAX, MathHelper.Clamp((float)deltaTime / (float)GetTransitionOpeningTime(), 0, 1));
                }
            }
            else if (m_state == MyGuiScreenState.CLOSING || m_state == MyGuiScreenState.HIDING)
            {
                int deltaTime = MyMinerGame.TotalTimeInMilliseconds - m_lastTransitionTime;

                if (deltaTime >= GetTransitionClosingTime())
                {
                    m_transitionAlpha = MyGuiConstants.TRANSITION_ALPHA_MIN;

                    //  Closing phase finished, we are now in close state
                    if (m_state == MyGuiScreenState.CLOSING)
                    {
                        CloseScreenNow();
                    }
                    else if (m_state == MyGuiScreenState.HIDING)
                    {
                        m_state = MyGuiScreenState.HIDDEN;

                        OnHide();
                    }
                }
                else
                {
                    //  While closing this screen, check if there is some screen that should unhide now, so that alpha transition goes in parallel with this screen
                    if (m_state == MyGuiScreenState.CLOSING && MyGuiManager.IsAnyScreenOpening() == false && m_transitionAlpha == 1.0f)
                    {
                        MyGuiScreenBase topHiddenScreen = MyGuiManager.GetTopHiddenScreen();
                        if (topHiddenScreen != null)
                        {
                            topHiddenScreen.UnhideScreen();
                        }
                    }
                    m_transitionAlpha = MathHelper.Lerp(MyGuiConstants.TRANSITION_ALPHA_MAX, MyGuiConstants.TRANSITION_ALPHA_MIN, MathHelper.Clamp((float)deltaTime / (float)GetTransitionClosingTime(), 0, 1));
                }
            }
        }

        /// <summary>
        /// Called when [show].
        /// </summary>
        protected virtual void OnShow() {}

        /// <summary>
        /// Called when [show].
        /// </summary>
        protected virtual void OnHide() {}

        /// <summary>
        /// Called when [show].
        /// </summary>
        protected virtual void OnClosed() {}

        //  Changes color according to transition alpha, this when opening the screen - it from 100% transparent to 100% opaque. When closing it's opposite.
        protected Color GetColorAfterTransitionAlpha(Vector4 color)
        {
            Vector4 ret = color;
            ret.W *= m_transitionAlpha;
            return new Color(ret);
        }

        ////  Inverse of GetColorAfterTransitionAlpha
        //protected Color GetColorAfterTransitionInverseAlpha(Vector4 color)
        //{
        //    Vector4 ret = color;
        //    ret.W *= 1 - m_transitionAlpha;
        //    return new Color(ret);
        //}

        public bool GetEnableBackgroundFade()
        {
            return m_enableBackgroundFade;
        }

        //  Returns true or false to let child implementation know if it has to run its own version of draw.
        public virtual bool Draw(float backgroundFadeAlpha)
        {
            //  If this screen has focus and there are other screens too, draw large black-transparent rectangle below it, so all other screens will look darker
            //  Only one screen can draw this sort of background, and it is always top-most screen!
            //if ((hasFocus == true) && (MyGuiManager.GetScreensCount() > 1))
            //if (MyGuiManager.GetScreensCount() > 1)
            if (backgroundFadeAlpha > 0)
            {
                Vector4 fadeColorVec = m_backgroundFadeColor;
                fadeColorVec.W *= backgroundFadeAlpha;
                if (m_backgroundColor.HasValue)
                {
                    fadeColorVec.X *= m_backgroundColor.Value.X;
                    fadeColorVec.Y *= m_backgroundColor.Value.Y;
                    fadeColorVec.Z *= m_backgroundColor.Value.Z;
                }
                Color fadeColor = new Color(fadeColorVec);

                //  Blank background fade
                MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), fadeColor);
            }

            //  This is just background of the screen rectangle
            if ((m_backgroundColor.HasValue) && (m_size.HasValue))
            {
                //  Background texture
                if (m_backgroundTexture == null)
                {
                    if (m_size.HasValue)
                    {
                        //  If this screen doesn't have custom texture, we will use one of the default - but with respect to screen's aspect ratio
                        m_backgroundTexture = MyTextureManager.GetTexture<MyTexture2D>(MyGuiManager.GetBackgroundTextureFilenameByAspectRatio(m_size.Value), loadingMode: LoadingMode.LazyBackground);
                    }
                }

                MyGuiManager.DrawSpriteBatch(m_backgroundTexture, m_position, m_size.Value, GetColorAfterTransitionAlpha(m_backgroundColor.Value), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
            }

            DrawControls();
            //DrawControlsFinal();

            return true;
        }

        public Vector2? GetSize()
        {
            return m_size;
        }

        public bool CanHaveFocus()
        {
            return m_canHaveFocus;
        }

        public bool CanShareInput()
        {
            return m_canShareInput;
        }

        public bool CanHide()
        {
            return m_screenCanHide;
        }

        public bool CanCloseInCloseAllScreenCalls()
        {
            return m_canCloseInCloseAllScreenCalls;
        }

        public virtual void RecreateControls(bool contructor)
        {
        }

        public virtual int GetTransitionOpeningTime()
        {
            if (SkipTransition) return 0;
            return MyGuiConstants.TRANSITION_OPENING_TIME;
        }

        public virtual int GetTransitionClosingTime()
        {
            if (SkipTransition) return 0;
            return MyGuiConstants.TRANSITION_CLOSING_TIME;
        }        

        public MyGuiControls Controls
        {
            get { return m_controls; }
        }

        public IList<MyGuiControlBase> GetList()
        {
            return m_controls.GetList();
        }


        public Vector2 GetPositionAbsolute()
        {
            return m_position;
        }

        private void FillControlsResurcive()
        {
            m_controlsQueue.Clear();
            m_controlsAll.Clear();
            m_controlsVisible.Clear();
            foreach (MyGuiControlBase control in Controls.GetList())
            {
                m_controlsQueue.Enqueue(control);
            }

            while(m_controlsQueue.Count > 0)
            {
                MyGuiControlBase control = m_controlsQueue.Dequeue();
                m_controlsAll.Add(control);
                if(control.Visible)
                {
                    m_controlsVisible.Add(control);
                }
                if(control is IMyGuiControlsParent)
                {
                    foreach (MyGuiControlBase childControl in ((IMyGuiControlsParent)control).Controls.GetList())
                    {
                        m_controlsQueue.Enqueue(childControl);
                    }
                }
            }
        }

        private void UpdateControls()
        {
            //  Update screen controls
            for (int i = 0; i < m_controlsAll.Count; i++)
            {
                MyGuiControlBase control = m_controlsAll[i];
                control.Update();
            }

            m_comboboxHandlingNow = GetComboboxHandlingInputNow();
            m_listboxDragAndDropHandlingNow = GetDragAndDropHandlingNow();
        }        

        private bool HandleControlsInput(MyGuiInput input, bool receivedFocusInThisUpdate)
        {
            MyGuiControlBase inputHandledBySomeControl = null;

            if (m_lastHandlingControl != null && m_lastHandlingControl.Visible) 
            {
                if (m_lastHandlingControl.HandleInput(input, m_controlsAll.IndexOf(m_lastHandlingControl) == m_keyboardControlIndex, m_controlsAll.IndexOf(m_lastHandlingControl) == m_keyboardControlIndexPrevious, receivedFocusInThisUpdate) == true) 
                {
                    inputHandledBySomeControl = m_lastHandlingControl;
                }
            }

            if (inputHandledBySomeControl == null && m_listboxDragAndDropHandlingNow != null)
            {
                if (m_listboxDragAndDropHandlingNow.HandleInput(input, m_controlsAll.IndexOf(m_listboxDragAndDropHandlingNow) == m_keyboardControlIndex, m_controlsAll.IndexOf(m_listboxDragAndDropHandlingNow) == m_keyboardControlIndexPrevious, receivedFocusInThisUpdate) == true)
                {
                    inputHandledBySomeControl = m_listboxDragAndDropHandlingNow;
                }
            }

            if (inputHandledBySomeControl == null && m_comboboxHandlingNow != null)
            {                
                if (m_comboboxHandlingNow.HandleInput(input, m_controlsAll.IndexOf(m_comboboxHandlingNow) == m_keyboardControlIndex, m_controlsAll.IndexOf(m_comboboxHandlingNow) == m_keyboardControlIndexPrevious, receivedFocusInThisUpdate) == true)
                {
                    inputHandledBySomeControl = m_comboboxHandlingNow;
                }                
            }

            //  If opened combobox didn't capture the input, we will try to handle it in remaining controls
            if (inputHandledBySomeControl == null)
            {
                for (int i = 0; i < m_controlsAll.Count; i++)
                {
                    MyGuiControlBase control = m_controlsAll[i];
                    if (control != m_comboboxHandlingNow && control != m_listboxDragAndDropHandlingNow && control.Visible)
                    {
                        if (control.HandleInput(input, i == m_keyboardControlIndex, i == m_keyboardControlIndexPrevious, receivedFocusInThisUpdate) == true)
                        {
                            inputHandledBySomeControl = control;
                            break;
                        }
                    }
                }
            }

            m_keyboardControlIndexPrevious = m_keyboardControlIndex;

            if (inputHandledBySomeControl != null)
            {
                m_keyboardControlIndex = m_controlsAll.IndexOf(inputHandledBySomeControl);
                m_keyboardControlIndexPrevious = m_keyboardControlIndex;    //  We need to reset it, otherwise we will hear two mouseover sounds!!!
            }

            m_lastHandlingControl = inputHandledBySomeControl;

            return inputHandledBySomeControl != null;
        }

        //  Moves active keyboard index to the next control, or previous control, or first control on the screen that can accept it
        //  forwardMovement -> set to TRUE when you want forward movement, set to FALSE when you wasnt backward
        protected bool HandleKeyboardActiveIndex(bool forwardMovement)
        {
            if (m_controlsAll.Count <= 0) return false;

            //  This is for making sure we won't end in infinite loop when screen doesn't contain controls we need
            int infinityProtectionCounter = 0;

            int sign = (forwardMovement == true) ? +1 : -1;
            //m_tempControlIndex = m_keyboardControlIndex;
            int tempControlIndex = m_keyboardControlIndex;

            while (true)
            {
                //  Increase or decrease and than check ranges and do sort of overflow
                tempControlIndex += sign;
                if (tempControlIndex > (m_controlsAll.Count - 1)) tempControlIndex = 0;
                if (tempControlIndex < 0) tempControlIndex = m_controlsAll.Count - 1;

                // Skip disabled controls
                if (m_controlsAll[tempControlIndex].CanHandleKeyboardActiveControl() && m_controlsAll[tempControlIndex].Enabled && m_controlsAll[tempControlIndex].Visible)
                {
                    //m_keyboardControlIndex = m_tempControlIndex;
                    m_keyboardControlIndex = tempControlIndex;
                    return true;
                }
                //  This is for making sure we won't end in infinite loop when screen doesn't contain controls we need
                infinityProtectionCounter++;
                if (infinityProtectionCounter > m_controlsAll.Count) return false;
            }
        }

        private void DrawControls()
        {
            //  Then draw all screen controls, except opened combobox and drag and drop - must be drawn as last
            // foreach (MyGuiControlBase control in Controls.GetVisibleControls())  //dont use this - allocations
            List<MyGuiControlBase> visibleControls = Controls.GetVisibleControls();
            for (int i = 0; i < visibleControls.Count; i++)
            {
                MyGuiControlBase control = visibleControls[i];
                if (control != m_comboboxHandlingNow && control != m_listboxDragAndDropHandlingNow)
                {
                    if (MyMinerGame.IsPaused() && !control.DrawWhilePaused) continue;
                    control.Draw();
                }
            }

            //  Finaly draw opened combobox and dragAndDrop, so it will overdraw all other controls

            if (m_comboboxHandlingNow != null)
            {
                m_comboboxHandlingNow.Draw();
            }

            if (m_listboxDragAndDropHandlingNow != null)
            {
                m_listboxDragAndDropHandlingNow.Draw();
            }

            // draw tooltips only when screen has focus
            if (this == MyGuiManager.GetScreenWithFocus())
            {
                //  Draw tooltips
                for (int i = 0; i < m_controlsVisible.Count; i++)
                {
                    MyGuiControlBase control = m_controlsVisible[i];
                    control.ShowToolTip();
                    if (MyFakes.CONTROLS_MOVE_ENABLED && MyGuiManager.GetInput().IsKeyPress(Keys.M))
                    {
                        DrawControlDebugPosition(control);
                    }
                }
            }
        }

        // Used in case, when new screen is added to hide tooltips of other screens
        public void HideTooltips()
        {
            foreach (var control in m_controlsAll)
            {
                control.HideToolTip();
            }
        }

        //  Returns true or false to let child implementation know if it has to run its own version of draw.
        public bool IsMouseOverAnyControl()
        {
            //  Update screen controls
            for (int i = m_controlsVisible.Count - 1; i >= 0; i--)
            {
                if (m_controlsVisible[i].IsMouseOver()) return true;
            }

            return false;
        }

        //  Returns true if any control contains mouse
        public bool AnyControlContainsMouse()
        {
            //  Update screen controls
            for (int i = m_controlsVisible.Count - 1; i >= 0; i--)
            {
                if (m_controlsVisible[i].ContainsMouse()) return true;
            }

            return false;
        }

        // Returns first control, which has mouse over
        public MyGuiControlBase GetMouseOverControl()
        {
            //  Update screen controls
            for (int i = m_controlsVisible.Count - 1; i >= 0; i--)
            {
                if (m_controlsVisible[i].IsMouseOver()) return m_controlsVisible[i];
            }

            return null;
        }

        public MyGuiControlBase GetControlUnderMouseCursor()
        {
            //  Update screen controls
            for (int i = m_controlsVisible.Count - 1; i >= 0; i--)
            {
                if (IsControlUnderCursor(m_controlsVisible[i])) return m_controlsVisible[i];
            }

            return null;
        }

        //StringBuilder m_drawPositionSb = new StringBuilder();
        private void DrawControlDebugPosition(MyGuiControlBase control)
        {
            m_drawPositionSb.Clear();
            m_drawPositionSb.Append("[");
            m_drawPositionSb.Append(control.GetPosition().X.ToString("0.0000"));
            m_drawPositionSb.Append(",");
            m_drawPositionSb.Append(control.GetPosition().Y.ToString("0.0000"));
            m_drawPositionSb.Append("]");
            float scale = 0.7f;
            Vector2 size = MyGuiManager.GetNormalizedSizeFromScreenSize(MyGuiManager.GetFontMinerWarsBlue().MeasureString(m_drawPositionSb, scale));
            Vector4 color = new Vector4(0f, 0f, 0f, 0.5f);

            MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), control.GetPosition() + control.GetParent().GetPositionAbsolute(),
                                         size, new Color(color),
                                         MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(),
                                                m_drawPositionSb,
                                                control.GetPosition() + control.GetParent().GetPositionAbsolute(), scale,
                                                Color.Green,
                                                MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
        }

        MyGuiControlListboxDragAndDrop GetDragAndDropHandlingNow()
        {
            for (int i = 0; i < m_controlsVisible.Count; i++)
            {
                MyGuiControlBase control = m_controlsVisible[i];

                if (control is MyGuiControlListboxDragAndDrop)
                {
                    MyGuiControlListboxDragAndDrop tempDragAndDrop = (MyGuiControlListboxDragAndDrop)control;
                    if (tempDragAndDrop.IsActive())
                    {
                        return tempDragAndDrop;
                    }
                }
            }

            //  Not found
            return null;
        }

        MyGuiControlCombobox GetComboboxHandlingInputNow()
        {
            for (int i = 0; i < m_controlsVisible.Count; i++)
            {
                MyGuiControlBase control = m_controlsVisible[i];

                if (control is MyGuiControlCombobox)
                {
                    MyGuiControlCombobox tempCombobox = (MyGuiControlCombobox)control;
                    if (tempCombobox.IsHandlingInputNow() == true)
                    {
                        foreach (MyGuiControlBase c in m_controlsAll)
                        {
                            if (c is MyGuiControlCombobox && c != control)
                            {
                                ((MyGuiControlCombobox)c).SetKeyboardActiveControl(false);
                            }
                        }
                        return tempCombobox;
                    }
                }
            }

            //  Not found
            return null;
        }
    }
}

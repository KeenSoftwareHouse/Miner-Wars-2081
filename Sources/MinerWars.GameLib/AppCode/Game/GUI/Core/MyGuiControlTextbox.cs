using System;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Collections.Generic;

//  Textbox GUI control. Supports MaxLength. 
//  Supports horisontaly scrollable texts - if text is longer than textbox width, the text will scroll from left to right.
//  This is accomplished by having sliding window and using stencil buffer to cut out invisible text - although it won't be optimal for extremely long texts.
//  Vertical scroling is not yet supported. Also password asterisks are supported. Selection, copy-paste is not supported yet.

namespace MinerWars.AppCode.Game.GUI.Core
{
    enum MyGuiControlTextboxType : byte
    {
        NORMAL,
        PASSWORD,
        DIGITS_ONLY,
    }

    class MyGuiControlTextbox : MyGuiControlBase
    {
        class MyGuiControlTextboxKeyToString
        {
            public Keys Key;
            public string Character;                //  Char/string when shift isn't pressed
            public string CharacterWhenShift;       //  Char/string when shift IS pressed
            public int LastKeyPressTime;            //  This is not for converting key to string, but for controling repeated key input with delay

            public MyGuiControlTextboxKeyToString(Keys key, string character, string characterWhenShift)
            {
                Key = key;
                Character = character;
                CharacterWhenShift = characterWhenShift;
                LastKeyPressTime = MyConstants.FAREST_TIME_IN_PAST;
            }
        }

        static MyGuiControlTextboxKeyToString[] m_keyToString;

        public EventHandler TextChanged;

        Vector4 m_textColor;
        float m_textScale;
        string m_text;
        int m_carriagePositionIndex;
        int m_carriageBlinkerTimer;
        int m_maxLength;
        MyGuiControlTextboxType m_type;
        Vector2 m_slidingWindowPosition;
        bool m_formattedAlready;
        bool m_drawBackground;

        List<Toolkit.Input.Keys> m_pressedKeys = new List<Toolkit.Input.Keys>(10);

        static MyGuiControlTextbox()
        {
            m_keyToString = new MyGuiControlTextboxKeyToString[MyMwcUtils.GetMaxValueFromEnum<Keys>() + 1];

            //  First set all items to null so later we can assing individual strings only where we want (when we want to map key to string)
            for (int i = 0; i < m_keyToString.Length; i++)
            {
                m_keyToString[i] = null;
            }

            //  This are here only for controling the delay in keypress
            AddKey(Keys.Left, null, null);
            AddKey(Keys.Right, null, null);
            AddKey(Keys.Home, null, null);
            AddKey(Keys.End, null, null);
            AddKey(Keys.Delete, null, null);
            AddKey(Keys.Back, null, null);

            //  For converting key to string
            AddKey(Keys.A, "a", "A");
            AddKey(Keys.B, "b", "B");
            AddKey(Keys.C, "c", "C");
            AddKey(Keys.D, "d", "D");
            AddKey(Keys.E, "e", "E");
            AddKey(Keys.F, "f", "F");
            AddKey(Keys.G, "g", "G");
            AddKey(Keys.H, "h", "H");
            AddKey(Keys.I, "i", "I");
            AddKey(Keys.J, "j", "J");
            AddKey(Keys.K, "k", "K");
            AddKey(Keys.L, "l", "L");
            AddKey(Keys.M, "m", "M");
            AddKey(Keys.N, "n", "N");
            AddKey(Keys.O, "o", "O");
            AddKey(Keys.P, "p", "P");
            AddKey(Keys.Q, "q", "Q");
            AddKey(Keys.R, "r", "R");
            AddKey(Keys.S, "s", "S");
            AddKey(Keys.T, "t", "T");
            AddKey(Keys.U, "u", "U");
            AddKey(Keys.V, "v", "V");
            AddKey(Keys.W, "w", "W");
            AddKey(Keys.X, "x", "X");
            AddKey(Keys.Y, "y", "Y");
            AddKey(Keys.Z, "z", "Z");
            AddKey(Keys.OemOpenBrackets, "[", "{");
            AddKey(Keys.OemCloseBrackets, "]", "}");            
            AddKey(Keys.Multiply, "*", "*");
            AddKey(Keys.Subtract, "-", "-");            
            AddKey(Keys.Add, "+", "+");
            AddKey(Keys.Divide, "/", "/");
            AddKey(Keys.NumPad0, "0", "0");
            AddKey(Keys.NumPad1, "1", "1");
            AddKey(Keys.NumPad2, "2", "2");
            AddKey(Keys.NumPad3, "3", "3");
            AddKey(Keys.NumPad4, "4", "4");
            AddKey(Keys.NumPad5, "5", "5");
            AddKey(Keys.NumPad6, "6", "6");
            AddKey(Keys.NumPad7, "7", "7");
            AddKey(Keys.NumPad8, "8", "8");
            AddKey(Keys.NumPad9, "9", "9");
            AddKey(Keys.Decimal, ".", ".");
            AddKey(Keys.OemBackslash, "\\", "|");
            AddKey(Keys.OemComma, ",", "<");
            AddKey(Keys.OemMinus, "-", "_");
            AddKey(Keys.OemPeriod, ".", ">");
            AddKey(Keys.OemPipe, "\\", "|");
            AddKey(Keys.OemPlus, "=", "+");
            AddKey(Keys.OemQuestion, "/", "?");
            AddKey(Keys.OemQuotes, "\'", "\"");
            AddKey(Keys.OemSemicolon, ";", ":");
            AddKey(Keys.OemTilde, "`", "~");
            AddKey(Keys.Space, " ", " ");
            AddKey(Keys.D0, "0", ")");
            AddKey(Keys.D1, "1", "!");
            AddKey(Keys.D2, "2", "@");
            AddKey(Keys.D3, "3", "#");
            AddKey(Keys.D4, "4", "$");
            AddKey(Keys.D5, "5", "%");
            AddKey(Keys.D6, "6", "^");
            AddKey(Keys.D7, "7", "&");
            AddKey(Keys.D8, "8", "*");
            AddKey(Keys.D9, "9", "(");            
        }

        public MyGuiControlTextbox(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, string defaultText, int maxLength, Vector4 textColor, float textScale, MyGuiControlTextboxType type)
            : this(parent, position, predefinedSize, defaultText, maxLength, textColor, textScale, type, true)
        {
        }

        public MyGuiControlTextbox(IMyGuiControlsParent parent, Vector2 position, MyGuiControlPreDefinedSize predefinedSize, string defaultText, int maxLength, Vector4 textColor, float textScale, MyGuiControlTextboxType type, bool canHandleKeyboardActiveControl, bool drawBackground = true)
            : base(parent, position, predefinedSize, drawBackground ? MyGuiConstants.BUTTON_BACKGROUND_COLOR : Color.Transparent.ToVector4(), null, drawBackground ? MyGuiManager.GetTextboxTexture(predefinedSize) : null, null, null, true)
        {
            Visible = true;
            m_type = type;
            m_size = GetPredefinedControlSize();
            m_carriagePositionIndex = 0;
            m_carriageBlinkerTimer = 0;
            m_canHandleKeyboardActiveControl = canHandleKeyboardActiveControl;
            m_textColor = MyGuiConstants.DEFAULT_CONTROL_NONACTIVE_COLOR;
            m_textScale = textScale;
            m_maxLength = maxLength;
            m_slidingWindowPosition = GetTextAreaPosition();
            Text = defaultText;
        }

        private void OnTextChanged()
        {
            if (TextChanged != null)
            {
                TextChanged(this, EventArgs.Empty);
            }
        }

        static void AddKey(Keys key, string character, string characterWhenShift)
        {
            m_keyToString[(int)key] = new MyGuiControlTextboxKeyToString(key, character, characterWhenShift);
        }

        //  Corrects/fixes string so it will contain only characters that Textbox can display and handle
        //  This method is slow, but I hope it's OK because it should be called only in textbox constructor and that's rare
        string CorrectString(string original)
        {
            StringBuilder res = new StringBuilder(original.Length);

            for (int i = 0; i < original.Length; i++)
            {
                //  Check if actual character is supported by textbox
                bool found = false;
                for (int j = 0; j < m_keyToString.Length; j++)
                {
                    if ((m_keyToString[j] != null) && (m_keyToString[j].Character != null) && (m_keyToString[j].CharacterWhenShift != null))
                    {
                        string orig = original[i].ToString();
                        if ((orig == m_keyToString[j].Character) || (orig == m_keyToString[j].CharacterWhenShift))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                //  Add actual character to result
                if (found == true)
                {
                    res.Append(original[i]);
                }
                else
                {
                    //  This is fall-back character in case we have found not supported character
                    res.Append(MyGuiConstants.TEXTBOX_FALLBACK_CHARACTER);
                }
            }

            return res.ToString();
        }

        bool IsEnoughDelay(Keys key, int forcedDelay)
        {
            MyGuiControlTextboxKeyToString keyEx = m_keyToString[(int)key];
            if (keyEx == null) return true;

            return ((MyMinerGame.TotalTimeInMilliseconds - keyEx.LastKeyPressTime) > forcedDelay);
        }

        //  Call this after some specified keypress was detected so we will make delay
        void UpdateLastKeyPressTimes(Keys? key)
        {
            //  This will reset the counter so it starts blinking whenever we enter the textbox
            //  And also when user presses a lot of keys, it won't blink for a while
            m_carriageBlinkerTimer = 0;

            //  Making delays between one long key press
            if (key.HasValue)
            {
                MyGuiControlTextboxKeyToString keyEx = m_keyToString[(int)key];
                if (keyEx != null)
                {
                    keyEx.LastKeyPressTime = MyMinerGame.TotalTimeInMilliseconds;
                }
            }
        }

        //  Call this every update, it will check if key as UNPRESSED (thus UP), and if yes, we will remove delay, so multiple 
        //  keypresses are possible fast, but not automatic. User must press DOWN/UP.
        void ResetLastKeyPressTimes(MyGuiInput input)
        {
            for (int i = 0; i < m_keyToString.Length; i++)
            {
                MyGuiControlTextboxKeyToString keyEx = m_keyToString[i];
                if (keyEx != null)
                {
                    if (input.IsKeyPress(keyEx.Key) == false)
                    {
                        keyEx.LastKeyPressTime = MyConstants.FAREST_TIME_IN_PAST;
                    }
                }
            }            
        }

        public void MoveCartrigeToEnd()
        {
            m_carriagePositionIndex = m_text.Length;
        }

        //  Method returns true if input was captured by control, so no other controls, nor screen can use input in this update
        public override bool HandleInput(MyGuiInput input, bool hasKeyboardActiveControl, bool hasKeyboardActiveControlPrevious, bool isThisFirstHandleInput)
        {
            if (!Visible)
            {
                return false;
            }

            bool ret = base.HandleInput(input, hasKeyboardActiveControl, hasKeyboardActiveControlPrevious, isThisFirstHandleInput);
            
            if (ret == false)
            {
                if ((IsMouseOver() == true) && (input.IsNewLeftMousePressed() == true))                    
                {
                    m_carriagePositionIndex = GetCarriagePositionFromMouseCursor();
                    ret = true;
                }

                if ((m_hasKeyboardActiveControl == true) && (hasKeyboardActiveControlPrevious == false))
                {
                    UpdateLastKeyPressTimes(null);
                }

                if (hasKeyboardActiveControl == true)
                {
                    bool isShiftPressed = input.IsKeyPress(Keys.LeftShift) || input.IsKeyPress(Keys.RightShift);
                    bool isCapsLockOn = (ushort)MyGuiInput.GetKeyState(0x14) == 1;

                    //(deltaFromLastKeypress > MyGuiConstants.TEXTBOX_KEYPRESS_DELAY)
                    //int deltaFromLastKeypress = MyMinerGame.TotalGameTime_Miliseconds - m_lastKeyPressTime;

                    input.GetPressedKeys(m_pressedKeys);

                    for (int i = 0; i < m_pressedKeys.Count; i++)
                    {
                        System.Diagnostics.Debug.Assert(input.IsKeyPress((Keys)m_pressedKeys[i]));
                    }

                    //  Transform pressed letters, characters, numbers, etc to real character/string
                    for (int i = 0; i < m_pressedKeys.Count; i++)
                    {
                        bool tempShift = isShiftPressed;
                        bool transformText = true;
                        MyGuiControlTextboxKeyToString pressedKey = m_keyToString[(int)m_pressedKeys[i]];

                        //Allow to enter only digits in case such textbox type is set
                        if (m_type == MyGuiControlTextboxType.DIGITS_ONLY)
                        {
                            if ((pressedKey != null) && !input.IsKeyDigit(pressedKey.Key) && pressedKey.Key != Keys.OemPeriod && pressedKey.Key != Keys.Decimal)
                            {
                                transformText = false;
                            }
                        }

                        if (transformText)  
                        {
                            //  If we have mapping from this key to string (that means it's allowed character)
                            if (pressedKey != null && m_text.Length < m_maxLength && pressedKey.Character != null && pressedKey.CharacterWhenShift != null)
                            {
                                ret = true;
                                if (IsEnoughDelay((Keys)m_pressedKeys[i], MyGuiConstants.TEXTBOX_CHANGE_DELAY))
                                {
                                    if (Char.IsLetter(pressedKey.Character, 0))
                                    {
                                        if (isCapsLockOn)
                                            tempShift = !tempShift;//carefull here variable is not used anymore so we can invert it
                                        
                                    }
                                    m_text = m_text.Insert(m_carriagePositionIndex, (tempShift == true) ? pressedKey.CharacterWhenShift : pressedKey.Character);
                                
                                    m_carriagePositionIndex++;
                                    UpdateLastKeyPressTimes((Keys)m_pressedKeys[i]);

                                    OnTextChanged();
                                }
                            }
                        }
                    }

                    //  Move left
                    if ((input.IsKeyPress(Keys.Left)) && (IsEnoughDelay(Keys.Left, MyGuiConstants.TEXTBOX_MOVEMENT_DELAY)))
                    {
                        m_carriagePositionIndex--;
                        if (m_carriagePositionIndex < 0) m_carriagePositionIndex = 0;
                        UpdateLastKeyPressTimes(Keys.Left);
                    }

                    //  Move right
                    if ((input.IsKeyPress(Keys.Right)) && (IsEnoughDelay(Keys.Right, MyGuiConstants.TEXTBOX_MOVEMENT_DELAY)))
                    {
                        m_carriagePositionIndex++;
                        if (m_carriagePositionIndex > m_text.Length) m_carriagePositionIndex = m_text.Length;
                        UpdateLastKeyPressTimes(Keys.Right);
                    }

                    //  Move home
                    if ((input.IsNewKeyPress(Keys.Home)) && (IsEnoughDelay(Keys.Home, MyGuiConstants.TEXTBOX_MOVEMENT_DELAY)))
                    {
                        m_carriagePositionIndex = 0;
                        UpdateLastKeyPressTimes(Keys.Home);
                    }

                    //  Move end
                    if ((input.IsNewKeyPress(Keys.End)) && (IsEnoughDelay(Keys.End, MyGuiConstants.TEXTBOX_MOVEMENT_DELAY)))
                    {
                        m_carriagePositionIndex = m_text.Length;
                        UpdateLastKeyPressTimes(Keys.End);
                    }

                    //  Delete
                    if ((input.IsKeyPress(Keys.Delete)) && (IsEnoughDelay(Keys.Delete, MyGuiConstants.TEXTBOX_MOVEMENT_DELAY)))
                    {
                        if (m_carriagePositionIndex < m_text.Length)
                        {
                            m_text = m_text.Remove(m_carriagePositionIndex, 1);
                            UpdateLastKeyPressTimes(Keys.Delete);

                            OnTextChanged();
                        }
                    }

                    //  Backspace
                    if ((input.IsKeyPress(Keys.Back)) && (IsEnoughDelay(Keys.Back, MyGuiConstants.TEXTBOX_MOVEMENT_DELAY)))
                    {
                        if (m_carriagePositionIndex > 0)
                        {
                            // Handle text scrolling, basicaly try hold carriage on same position in textBox window (avoid textBox with hidden characters)
                            var carriagePositionChange = GetCarriagePosition(m_carriagePositionIndex) - GetCarriagePosition(m_carriagePositionIndex-1); ;
                            var textAreaPosition = GetTextAreaPosition();
                            if (m_slidingWindowPosition.X - carriagePositionChange.X >= textAreaPosition.X)
                            {
                                m_slidingWindowPosition.X -= carriagePositionChange.X;
                            }
                            else
                            {
                                m_slidingWindowPosition.X = textAreaPosition.X;
                            }

                            m_text = m_text.Remove(m_carriagePositionIndex - 1, 1);
                            m_carriagePositionIndex--;
                            UpdateLastKeyPressTimes(Keys.Back);

                            OnTextChanged();
                        }
                    }

                    ResetLastKeyPressTimes(input);
                    m_formattedAlready = false;
                }
                else
                {
                    if (m_type == MyGuiControlTextboxType.DIGITS_ONLY && m_formattedAlready == false && !string.IsNullOrEmpty(m_text))
                    {
                        var number = MyValueFormatter.GetDecimalFromString(m_text, 1);
                        int decimalDigits = (number - (int)number > 0) ? 1 : 0;
                        m_text = MyValueFormatter.GetFormatedFloat((float)number, decimalDigits, "");
                        m_carriagePositionIndex = m_text.Length;
                        m_formattedAlready = true;
                    }
                }
            }

            return ret;
        }

        //  After user clicks on textbox, we will try to set carriage position where the cursor is
        int GetCarriagePositionFromMouseCursor()
        {
            UpdateSlidingWindowPosition();
            Vector2 textAreaPosition = GetTextAreaPosition();
            float slidingWindowPositionDeltaX = m_slidingWindowPosition.X - textAreaPosition.X;

            int closestIndex = 0;
            float closestDistance = float.MaxValue;

            for (int i = 0; i <= m_text.Length; i++)
            {
                float charPositionX = GetCarriagePosition(i).X - slidingWindowPositionDeltaX;
                float charDistance = Math.Abs(MyGuiManager.MouseCursorPosition.X - charPositionX);
                //m_slidingWindowPosition
                if (charDistance < closestDistance)
                {
                    closestDistance = charDistance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        Vector2 GetTextAreaSize()
        {
            float multiplier = (this.m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM ? 1.3f : 1);
            return m_size.Value - 2 * MyGuiConstants.TEXTBOX_TEXT_OFFSET * multiplier;
        }

        Vector2 GetTextAreaPosition()
        {
            float multiplier = (this.m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM ? 1.3f : 1);
            return m_parent.GetPositionAbsolute() + m_position + new Vector2(-m_size.Value.X / 2.0f, 0) + MyGuiConstants.TEXTBOX_TEXT_OFFSET * multiplier;
        }

        //  Converts carriage (or just char) position to normalized coordinates
        Vector2 GetCarriagePosition(int index)
        {
            string leftFromCarriageText = GetTextByType().Substring(0, index);
            Vector2 leftFromCarriageSize = MyGuiManager.GetNormalizedSize(MyGuiManager.GetFontMinerWarsBlue(), new StringBuilder(leftFromCarriageText), m_textScale);
            return GetTextAreaPosition() + new Vector2(leftFromCarriageSize.X, 0);
        }

        //  If type of textbox is password, this method returns asterisk. Otherwise original text.
        string GetTextByType()
        {
            if (m_type == MyGuiControlTextboxType.NORMAL || m_type == MyGuiControlTextboxType.DIGITS_ONLY)
            {
                return m_text;
            }
            else if (m_type == MyGuiControlTextboxType.PASSWORD)
            {
                return new String('*', m_text.Length);
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        //  changes type of this textbox (password/text)
        public void ChangeMyType(MyGuiControlTextboxType type)
        {
            m_type = type;
        }

        //  When setting text to textbox, make sure you won't set it to unsuported charact
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                //  Fix text so it will contain only allowed characters and reduce it if it's too long
                m_text = CorrectString(value.Substring(0, (value.Length > m_maxLength) ? m_maxLength : value.Length));
                if (m_carriagePositionIndex >= m_text.Length) 
                {
                    m_carriagePositionIndex = m_text.Length;
                }
                
                OnTextChanged();
            }
        }

        //  If carriage is in current sliding window, then we don't change it. If it's over its left or right borders, we move sliding window.
        //  Of course on on X axis, Y is ignored at all.
        //  This method could be called from Update() or Draw() - it doesn't matter now
        void UpdateSlidingWindowPosition()
        {
            Vector2 carriagePosition = GetCarriagePosition(m_carriagePositionIndex);
            Vector2 textAreaPosition = GetTextAreaPosition();
            Vector2 textAreaSize = GetTextAreaSize();

            float rightBorderX = textAreaPosition.X + textAreaSize.X;

            if (carriagePosition.X < m_slidingWindowPosition.X)
            {
                m_slidingWindowPosition.X = carriagePosition.X;
            }
            else if (carriagePosition.X > (m_slidingWindowPosition.X + textAreaSize.X))
            {
                m_slidingWindowPosition.X = carriagePosition.X - textAreaSize.X;
            }
        }

        public override void Draw()
        {
            if (!Visible)
            {
                return;
            }

            base.Draw();

            Vector2 textAreaSize = GetTextAreaSize();
            Vector2 textAreaPosition = GetTextAreaPosition();
            Vector4 tempTextColor = (IsMouseOverOrKeyboardActive() == false) ? m_textColor : m_textColor * MyGuiConstants.CONTROL_MOUSE_OVER_BACKGROUND_COLOR_MULTIPLIER;
            Vector2 carriagePosition = GetCarriagePosition(m_carriagePositionIndex);

            //  End our standard sprite batch
            MyGuiManager.EndSpriteBatch();

            //  Draw the rectangle to stencil buffer, so later when we draw, only pixels with stencil=1 will be rendered
            //  Textbox interior must be increased by 1 pixel in all four directions (thus adding 2.0 to its height and width - because it's centered).
            //  Otherwise stencil would cut out something from textbox interior.
            MyGuiManager.DrawStencilMaskRectangle(new MyRectangle2D(textAreaPosition + new Vector2(textAreaSize.X / 2.0f, 0), 
                textAreaSize + MyGuiManager.GetNormalizedSizeFromScreenSize(new Vector2(2.0f, 2.0f))), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);

            //  Set up the stencil operation and parameters
            MyGuiManager.BeginSpriteBatch_StencilMask();

            UpdateSlidingWindowPosition();
            float slidingWindowPositionDeltaX = m_slidingWindowPosition.X - textAreaPosition.X;

            //  Show "sliding window" rectangle - only for debugging
            //MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), m_slidingWindowPosition, textAreaSize, 
            //    GetColorAfterTransitionAlpha(new Vector4(0, 1, 0, 0.3f)), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            //  Draw text in textbox
            MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), new StringBuilder(GetTextByType()), new Vector2(textAreaPosition.X - slidingWindowPositionDeltaX, textAreaPosition.Y),
                m_textScale, GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER);

            //  Draw carriage line
            //  Carriage blinker time is solved here in Draw because I want to be sure it will be drawn even in low FPS
            if (m_hasKeyboardActiveControl == true)
            {
                //  This condition controls "blinking", so most often is carrier visible and blinks very fast
                //  It also depends on FPS, but as we have max FPS set to 60, it won't go faster, nor will it omit a "blink".
                int carriageInterval = m_carriageBlinkerTimer % 20;
                if ((carriageInterval >= 0) && (carriageInterval <= 15))
                {
                    MyGuiManager.DrawSpriteBatch(MyGuiManager.GetBlankTexture(), new Vector2(carriagePosition.X - slidingWindowPositionDeltaX, carriagePosition.Y), 1, m_size.Value.Y * 0.5f,
                        GetColorAfterTransitionAlpha(tempTextColor), MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                }
            }
            m_carriageBlinkerTimer++;

            //  End stencil-mask batch, and restart the standard sprite batch
            //MyGuiManager.EndSpriteBatch();
            MyGuiManager.EndSpriteBatch_StencilMask();
            //MyGuiManager.BeginSpriteBatch_StencilMask();
            MyGuiManager.BeginSpriteBatch();
        }

        protected override Vector2 GetPredefinedControlSize()
        {
            Vector2 size = Vector2.Zero;
            if (m_predefinedSize.HasValue)
            {
                if (m_predefinedSize == MyGuiControlPreDefinedSize.LARGE)
                {
                    size = MyGuiConstants.TEXTBOX_LARGE_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.MEDIUM)
                {
                    size = MyGuiConstants.TEXTBOX_MEDIUM_SIZE;
                }
                else if (m_predefinedSize == MyGuiControlPreDefinedSize.LONGMEDIUM)
                {
                    size = MyGuiConstants.TEXTBOX_MEDIUM_LONG_SIZE;
                }
                else
                {
                    size = MyGuiConstants.TEXTBOX_SMALL_SIZE;
                }
            }

            return size;
        }        
    }
}

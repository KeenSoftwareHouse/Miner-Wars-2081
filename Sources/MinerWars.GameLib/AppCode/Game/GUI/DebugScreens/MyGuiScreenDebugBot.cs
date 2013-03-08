using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI
{

    /// <summary>
    /// This screen gives us info on our bot(s) and let's us edit, at runtime, the parameters to tweak the behavior of the 
    /// bot. For example: how close we must be to anger it, how aggressive it is, how much it fires, adjustment factors to
    /// it's maximum movement and rotation speeds, etc.
    /// </summary>
    class MyGuiScreenDebugBot : MyGuiScreenDebugBase
    {
        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugBot";
        }

        /// <summary>
        /// 
        /// </summary>
        static StringBuilder m_frameDebugText = new StringBuilder(1000);

        /// <summary>
        /// 
        /// </summary>
        //MySmallShipBot m_bot;

        /// <summary>
        /// Reference to keyboard and mouse input instance.
        /// </summary>
        MyGuiInput m_input;

        /// <summary>
        /// The list of parameter names of bot parameters that are adjustable at runtime.
        /// </summary>
        List<BotParam> m_botParams = new List<BotParam>();

        /// <summary>
        /// The index of the parameter that we are currently tweaking.
        /// </summary>
        int m_currentParamIndex;

        /// <summary>
        /// 
        /// </summary>
        BotParam m_currentParam;

        /// <summary>
        /// 
        /// </summary>
        public MyGuiScreenDebugBot()
            : base(new Vector2(0.5f, 0.5f), new Vector2(), null, true)
        {

            // Flag this as top screen.
            m_isTopMostScreen = true;
            
            m_drawEvenWithoutFocus = true;
            m_canHaveFocus = false;

            // Be careful not to try to list bot details before bot exists.
            /*
            if (BotExists())
            {
                this.m_bot = MyGuiScreenGamePlay.Static.BotFriend1;

                BotParam rotSpeed = new BotParam();
                rotSpeed.Name = "Turning Capability";
                rotSpeed.Description = "Increases bot's ability to turn quickly.";
                rotSpeed.IsBoolean = false;
                rotSpeed.Value = this.m_bot.RotationSpeedTuner;
                this.m_botParams.Add(rotSpeed);

                BotParam armed = new BotParam();
                armed.Name = "Armed";
                armed.Description = "Arms/disarms bot.";
                armed.Value = 0f;
                if (this.m_bot.IsArmed)
                {
                    armed.Value = 1.0f;
                }
                armed.IsBoolean = true;
                this.m_botParams.Add(armed);

                BotParam tooClose = new BotParam();
                tooClose.Name = "Collision Sensor Sensitivity";
                tooClose.Description = "Increases distance bot tries to maintain between itself and player.";
                tooClose.Value = this.m_bot.TooCloseTunerAttack;
                tooClose.IsBoolean = false;
                this.m_botParams.Add(tooClose);

                // Select first param in bot param list by default.
                this.m_currentParamIndex = 0;
                this.m_currentParam = this.m_botParams[this.m_currentParamIndex];

            }
             */
        }

        /// <summary>
        /// Pass debug screen parameter values into corresponding variables within the currently
        /// selected bot.
        /// </summary>
        /// <param name="paramIndex">Parameter list index.</param>
        /// <param name="paramValue">Parameter value</param>
        private void PassValueFromDebugScreenToBot(int paramIndex, float paramValue)
        {

            // String name of this parameter index.
            // TODO: shouldn't this be an enum?
            string paramName = this.m_botParams[paramIndex].Name;

            // Based on specified parameter, set the appropriate bot variable
            // with the current parameter value.
            switch (paramName)
            {
                case "Turning Capability":
                    //this.m_bot.RotationSpeedTuner = paramValue;
                    break;

                case "Armed":
                    //this.m_bot.IsArmed = Convert.ToBoolean(paramValue);
                    break;

                case "Projectile Burst Frequency":
                    //this.m_bot.Behavior.FastBurstFrequencyTuner = paramValue;
                    break;

                case "Projectile Burst Duration":
                    //this.m_bot.Behavior.FastBurstDurationTuner = paramValue;
                    break;

                case "Missile Frequency":
                    //this.m_bot.Behavior.MissileFrequencyTuner = paramValue;
                    break;

                case "Firing Cone":
                    //this.m_bot.Behavior.FastFiringConeTuner = paramValue;
                    break;

                case "Collision Sensor Sensitivity":
                    //this.m_bot.TooCloseTunerAttack = paramValue;
                    break;

                default:

                    // Be sure all runtime-settable parameter names are handled within
                    // this case statement.
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }

        }

        /// <summary>
        /// Frame Debug Text - is cleared at the begining of Update and rendered at the end of Draw
        /// </summary>
        /// <param name="s"></param>
        public void AddToFrameDebugText(string s)
        {
            m_frameDebugText.AppendLine(s);
        }

        /// <summary>
        /// Frame Debug Text - is cleared at the begining of Update and rendered at the end of Draw
        /// </summary>
        public void ClearFrameDebugText()
        {
            MyMwcUtils.ClearStringBuilder(m_frameDebugText);
        }

        /// <summary>
        /// Get left top corner of screen, while factoring in the screen safe area.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetScreenLeftTopPosition()
        {
            float deltaPixels = 25 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, deltaPixels));
        }

        public override bool Update(bool hasFocus)
        {
            // Update base class.
            if (base.Update(hasFocus) == false) return false;

            // Don't try to pass values to bot if it doesn't exsit.
            if (BotExists())
            {
                // Handle input for this debug screen: the arrows keys to modify params.
                this.HandleInput();

                // Update bot with current setting selection.
                // TODO: do I need to be calling this every frame instead of in an event-based way?
                this.PassValueFromDebugScreenToBot(this.m_currentParamIndex, this.m_currentParam.Value);
            }

            return true;
        }

        /// <summary>
        /// Watch for arrow keys.
        /// </summary>
        private void HandleInput()
        {

            // Move editing focus to next bot parameter.
            if (this.m_input.IsNewKeyPress(Keys.Down))
            {

                // Select next parameter, but don't go out of range.
                this.m_currentParamIndex += 1;
                this.m_currentParamIndex = Math.Min(this.m_currentParamIndex, (this.m_botParams.Count - 1));

                // Note newly selected parameter.
                this.m_currentParam = this.m_botParams[this.m_currentParamIndex];

            }

            // Move editing focus to previous bot parameter.
            if (this.m_input.IsNewKeyPress(Keys.Up))
            {

                // Select previous parameter, but don't go out of range.
                this.m_currentParamIndex -= 1;
                this.m_currentParamIndex = Math.Max(this.m_currentParamIndex, 0);

                // Note newly selected parameter.
                this.m_currentParam = this.m_botParams[this.m_currentParamIndex];

            }

            // Increment current parameter value.
            if (this.m_input.IsNewKeyPress(Keys.Right))
            {
                this.m_currentParam.Value += 0.1f;
                if (this.m_currentParam.IsBoolean == true)
                {
                    this.m_currentParam.Value = 1f;
                }
            }

            // Decrement current parameter value.
            if (this.m_input.IsNewKeyPress(Keys.Left))
            {
                this.m_currentParam.Value -= 0.1f;
                this.m_currentParam.Value = Math.Max(this.m_currentParam.Value, 0);
                if (this.m_currentParam.IsBoolean == true)
                {
                    this.m_currentParam.Value = 0f;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backgroundFadeAlpha"></param>
        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            const float TEXT_DISTANCE_Y = 0.025f;
            const float TEXT_SCALE = 0.5f;

            List<StringBuilder> texts = new List<StringBuilder>(20);
            texts.Add(new StringBuilder("BOT DEBUG SCREEN"));
            texts.Add(new StringBuilder(String.Empty));

            if (BotExists())
            {
                // Non-persistence disclaimer.
                texts.Add(new StringBuilder("Use this debug screen to test different bot settings in"));
                texts.Add(new StringBuilder("order to find those that 'feel' best. Note that these settings"));
                texts.Add(new StringBuilder("are just for experimentation and don't persist. Final changes"));
                texts.Add(new StringBuilder("must be made in the code itself."));
                texts.Add(new StringBuilder(String.Empty));

                // Camera.
                texts.Add(new StringBuilder("Camera.CameraAttachedTo = " + MyGuiScreenGamePlay.Static.CameraAttachedTo.ToString()));
                texts.Add(new StringBuilder("MySpectator.Position = " + MyUtils.GetFormatedVector3(MySpectator.Position,0)));
                texts.Add(new StringBuilder("Camera.ThirdPersonCamDelta = " + MyUtils.GetFormatedVector3( MyGuiScreenGamePlay.Static.ThirdPersonCameraDelta, 0 )));
                texts.Add(new StringBuilder(String.Empty));

                // Bot info.
                //double angle = this.m_bot.AngleBetweenForwardAndTarget( this.m_pl
                //texts.Add(new StringBuilder("Bot.AngleBetweenForwardAndPlayerPosition = " + angle.ToString("F0")));
                //texts.Add(new StringBuilder(String.Empty));


                // Bot debug key controls.
                texts.Add(new StringBuilder("Use UP/DOWN arrows keys to select a parameter."));
                texts.Add(new StringBuilder("Use RIGHT/LEFT arrows keys to adjust parameter value."));
                texts.Add(new StringBuilder("Press Shift-F12 to close this screen."));
                texts.Add(new StringBuilder(String.Empty));

                // List specific bot that we're modifying.
                StringBuilder botTitle = new StringBuilder();
                botTitle.Append("Selected bot:  ");
                //botTitle.Append(m_bot.Name);
                texts.Add(botTitle);
                texts.Add(new StringBuilder(String.Empty));

                // Walk through all settable parameters and list current names and values.
                // Add either spacing or tick mark prefix to each parameter listed.
                // The little tick marks indicates the currently editting parameter.
                for (int i = 0; i < this.m_botParams.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    BotParam param = this.m_botParams[i];
                    sb.Append((this.GetPrefix(i) + param.Name + " = "));
                    if (param.IsBoolean == false)
                    {
                        sb.Append((param.Value.ToString("F1")));
                    }
                    else
                    {
                        if (param.Value == 0.0f)
                        {
                            sb.Append("False");
                        }
                        else
                        {
                            sb.Append("True");
                        }
                    }
                    texts.Add(sb);
                }
                texts.Add(new StringBuilder(String.Empty));

                // Description what effect changing current parameter will have..
                texts.Add(new StringBuilder("Description:"));
                texts.Add(new StringBuilder(this.m_currentParam.Description));
                texts.Add(new StringBuilder(String.Empty));

            }

            // Where to start drawing text.
            Vector2 origin = this.GetScreenLeftTopPosition();

            // Draw our text on screen.
            for (int i = 0; i < texts.Count; i++)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsWhite(), texts[i], origin + new Vector2(0, i * TEXT_DISTANCE_Y), TEXT_SCALE,
                    Color.Yellow, MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP);
            }

            ClearFrameDebugText();

            return true;
        }

        /// <summary>
        /// Show a little mark in front of the parameter we are editing.
        /// This is so that the developer knows which parameter is being tweaked.
        /// </summary>
        /// <param name="paramIndex"></param>
        /// <returns></returns>
        private string GetPrefix(int paramIndex)
        {
            string prefix = String.Empty;
            if (paramIndex == this.m_currentParamIndex)
            {
                prefix = "> ";
            }
            else
            {
                prefix = "    ";
            }
            return (prefix);

        }

        /// <summary>
        /// Use this to give the bot debug screen a reference to our input manager. Why? So that this screen
        /// can see the key presses that we're using to tweak the bot parameters.
        /// </summary>
        /// <param name="m_input"></param>
        internal void SetGuiInput(MyGuiInput m_input)
        {
            this.m_input = m_input;
        }

        /// <summary>
        /// Does bot exist? For example, don't try to display bot info if there's no bot. That'll throw
        /// exception errors.
        /// </summary>
        /// <returns></returns>
        private static bool BotExists()
        {
            bool botExists = false;
            if (MyGuiScreenGamePlay.Static != null)
            {        /*
                if (MyGuiScreenGamePlay.Static.BotFriend1 != null)
                {
                    botExists = true;
                }  */
                }
            return botExists;
        }

    }

    /// <summary>
    /// This represent a settable bot parameter.
    /// </summary>
    class BotParam
    {

        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// If true: param value toggles between 0f and 1f;
        /// If false: param value ranges from 0.0f to 1.0f;
        /// </summary>
        public bool IsBoolean;

        /// <summary>
        /// 
        /// </summary>
        public float Value;

        /// <summary>
        /// Explanatory text that describes what effect changing this parameter will have/
        /// </summary>
        public string Description;

    }

}

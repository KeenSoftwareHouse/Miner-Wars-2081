using System;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWars.AppCode.Game.Managers.Session;

namespace MinerWars.AppCode.Game.Utils
{
    /// <summary>
    /// This is for all the debug junk needed to develop the bot logic and behavior. This stuff is all extra
    /// debugging code just to visualize what the bot is planning and doing and is not actually used by the bot
    /// in order to function. The idea is to separate out the stuff that is just for debugging so that the bot
    /// code itself (MyPhysObjectBot.cs) is fairly clean and what is there is just the stuff that is actually 
    /// required for the bot to work.
    /// </summary>
    static class MyBotDebugUtils
    {
        /// <summary>
        /// For debugging, draw the local forward axis (-Z) of the bot.
        /// </summary>
        public static void DrawForward( Matrix worldMatrix )
        {
            Vector3 forward = worldMatrix.Forward * 100.0f;
            MyDebugDraw.DrawLine3D(worldMatrix.Translation, (worldMatrix.Translation + forward), Color.White, Color.White);
        }

        /// <summary>
        /// For debugging, and to cross-check our data for where the player is, draw a sphere at that position.
        /// </summary>
        /// <param name="pos">Target position</param>
        public static void DrawTargetPosition(Vector3 pos)
        {
            Vector3 diffuseColor = new Vector3(50, 0, 0);
            float alpha = 0.0f;
            MyDebugDraw.DrawSphereSmooth(pos, 0.25f, diffuseColor, alpha);
        }

        /// <summary>
        /// Draw the world axes, at the specified current position.
        /// </summary>
        /// <param name="pos"></param> 
        /// <param name="length">Default: 15f</param>
        public static void DrawWorldCoordinatesAtBot( Vector3 pos, float length  )
        {
            // Draw red X, green Y, and blue Z world axes.
            const float axisLength = 15;
            MyDebugDraw.DrawLine3D(pos, (pos + new Vector3(axisLength, 0, 0)), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(pos, (pos + new Vector3(0, axisLength, 0)), Color.Green, Color.Green);
            MyDebugDraw.DrawLine3D(pos, (pos + new Vector3(0, 0, axisLength)), Color.Blue, Color.Blue);
        }

        /// <summary>
        /// Draw axes of the bot's local coordinate system.
        /// </summary>
        public static void DrawLocalCoordinateAxes( Matrix worldMatrix )
        {
            // Short-hand references.
            Matrix botWorld = worldMatrix;
            Vector3 botPos = worldMatrix.Translation;

            // Draw red X, green Y, and blue Z world axes.
            const float axisLength = 15;

            // X
            Vector3 startX = botPos;
            Vector3 endX = botPos + (botWorld.Right * axisLength);
            MyDebugDraw.DrawLine3D(startX, endX, Color.Red, Color.Red);

            // Y 
            Vector3 startY = botPos;
            Vector3 endY = botPos + (botWorld.Up * axisLength);
            MyDebugDraw.DrawLine3D(startY, endY, Color.Green, Color.Green);

            // Z
            Vector3 startZ = botPos;
            Vector3 endZ = botPos + (botWorld.Backward * axisLength);
            MyDebugDraw.DrawLine3D(startZ, endZ, Color.Blue, Color.Blue);
        }

        /// <summary>
        /// Get the bot debug mode preference. Since we don't want to add a bunch of the bot-specific
        /// debug info to the debug screen that everyone sees, we're only showing it for the bot AI
        /// developer (Adam Kane). Since it is a programmatic check, there's no risk of me forgetting
        /// to disable the bot debug mode before doing a code checkin.
        /// </summary>
        /// <returns>Debug preference.</returns>
        public static bool GetDebugModeBasedOnUserName()
        {
            // By default, debug info about bots is not written to the debug screen.
            bool debugMode = false;

            // Figure out if we're running on Adam Kane's machine, who is working on the bot code. 
            bool isAdamKanesMachine = (Environment.UserName == "Adam" && Environment.MachineName == "XPS730");

            // If, and only if we're on Adam's machine, enable debug mode.
            if (isAdamKanesMachine)
            {
                debugMode = true;
            }

            // Return our conclusion.
            return debugMode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Write ( string message ) {
            System.Diagnostics.Debug.WriteLine( message );
        }

        /// <summary>
        /// This is a helper method which is here so that we can get a nice view of how the bots are moving
        /// in relationship to the player. For example, we want to be able to observe a friendly bot 
        /// hovering around player in a nice natural-looking way. What this method does is sets the spectator 
        /// position such that we just behind and above the miner ship.  This is turn will result in the 
        /// requested third person camera delta. Here's the logic that the ThirdPersonFollowing cam uses:
        /// m_thirdPersonCameraDelta = MySpectator.Position - MyGuiScreenGameBase.Static.PlayerShip.GetPosition();
        /// </summary>
        /// <param name="foo"></param>
        public static void ThirdPersonCameraDelta(Vector3 desiredCameraOffset)
        {
            //
            Matrix playerMatrix = MySession.PlayerShip.WorldMatrix;

            //            
            MySpectator.Position = playerMatrix.Translation + desiredCameraOffset;
            MySpectator.Target = MySpectator.Position + playerMatrix.Forward;
        }

        /// <summary>
        /// For debugging, add text to the debug screen which details the state of this bot.
        /// TODO: Move this to MyGuiScreenDebugBot. That class is specific to bot so this stuff could/should go there.
        /// </summary>
        /// <param name="bot">Computer-controlled ship.</param>
        /// <param name="player">The bot's target.</param>
        public static void AddToFrameDebugText(MySmallShipBot bot, MySmallShip player)
        {
            // Get a reference to the debug screen.
            MyGuiScreenDebugBot debugScreen = MyGuiManager.GetScreenDebugBot();
            
            if (debugScreen != null)
            {
                // Here's the bot info we're adding to the debug screen text.
                debugScreen.AddToFrameDebugText("MyPhysObjectBot");

                // Hello.
                debugScreen.AddToFrameDebugText("   Message: Hello!");

                // Position.
                debugScreen.AddToFrameDebugText("   Player.Position: " + MyUtils.GetFormatedVector3(player.GetPosition(), 0));

                // Offest between bot and player.
                Vector3 playerPos = player.GetPosition();
                Vector3 botPos = bot.GetPosition();
                Vector3 botToPlayer = botPos - playerPos;
                debugScreen.AddToFrameDebugText("   BotToPlayer: " + botToPlayer.ToString());

                // Offest between player and bot.
                Vector3 playerToBot = playerPos - botPos;
                debugScreen.AddToFrameDebugText("   PlayerToBot: " + playerToBot.ToString());

                // Bot forward
                Vector3 forward = bot.WorldMatrix.Forward;
                debugScreen.AddToFrameDebugText("   Bot.WorldMatrix.Forward: " + MyUtils.GetFormatedVector3(forward, 0));

                // m_playersRelativePosition
                //debugScreen.AddToFrameDebugText("   Bot.m_playersRelativePosition: " + MyUtils.GetFormatedVector3(bot.Behavior.TargetsRelativePosition, 0));

                // Aimed at player?
                //debugScreen.AddToFrameDebugText("   Bot.IsAimedAtPlayer: " + bot.IsBotPrettyMuchAimedAtPlayer());

                // Distance to player.
                //debugScreen.AddToFrameDebugText("   Bot.DistanceToPlayer: " + ((int)(bot.DistanceTo(bot.Decision.Target))).ToString());

                // Rotation indicator.
                //debugScreen.AddToFrameDebugText("   Bot.RotationIndicator: " + MyUtils.GetFormatedVector2(bot.RotationIndicator, 1));

                // Blank line.
                debugScreen.AddToFrameDebugText(" ");
            }
        }
    }
}
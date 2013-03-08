#region Using
using System;
using System.Collections.Generic;
using System.Threading;
using MinerWarsMath;


using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.BackgroundCube;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Weapons;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Trailer;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.World;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Prefabs;
using SysUtils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Localization;
#endregion


namespace MinerWars.AppCode.Game.TransparentGeometry
{
    static class MyFlashes 
    {
        enum FlashState
        {
            INACTIVE,
            FADE_IN,
            FLASH,
            FADE_OUT
        }

        static int m_StartMilliseconds;
        static float m_flashIntensity; //0-1
        static FlashState m_State = FlashState.INACTIVE;

        public static void MakeFlash()
        {
            if (m_State == FlashState.INACTIVE || m_State == FlashState.FADE_OUT)
            {
                m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                m_State = FlashState.FADE_IN;
            }
            else
            if (m_State == FlashState.FLASH)
            {
                m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            }
        }

        /// <summary>
        /// Render method is called directly by renderer. Depending on stage, post process can do various things 
        /// </summary>
        /// <param name="postProcessStage">Stage indicating in which part renderer currently is.</param>public override void RenderAfterBlendLights()
        public static void Draw()
        {   //TODO   
            /*
            switch (m_State)
            {
                case FlashState.FADE_IN:
                    m_flashIntensity = (MyMinerGame.TotalGamePlayTimeInMilliseconds - m_StartMilliseconds) / (float)MyFlashBombConstants.FADE_IN_TIME;
                    if (m_flashIntensity >= 1.0f)
                    {
                        m_State = FlashState.FLASH;
                        m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                    }
                    break;

                case FlashState.FLASH:
                    m_flashIntensity = 1.0f;
                    if (MyMinerGame.TotalGamePlayTimeInMilliseconds > (m_StartMilliseconds + (float)MyFlashBombConstants.FLASH_TIME))
                    {
                        m_StartMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
                        m_State = FlashState.FADE_OUT;
                    }
                    break;

                case FlashState.FADE_OUT:
                    m_flashIntensity = 1.0f - ((MyMinerGame.TotalGamePlayTimeInMilliseconds - m_StartMilliseconds) / (float)MyFlashBombConstants.FADE_OUT_TIME);
                    if (m_flashIntensity <= 0.0f)
                    {
                        m_State = FlashState.INACTIVE;
                    }
                    break;
            }

            if (m_State != FlashState.INACTIVE)
            {
                BlendState bs = MyMinerGame.Static.GraphicsDevice.BlendState;
                MyMinerGame.Static.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

                int flashIntensityInt = (int)(m_flashIntensity * 255.0f);
                MyGuiManager.DrawSpriteFast(MyGuiManager.GetBlankTexture(), 0, 0, MyCamera.Viewport.Width, MyCamera.Viewport.Height, new Color(flashIntensityInt, flashIntensityInt, flashIntensityInt, flashIntensityInt));

                //  Revert alpha-blending
                MyMinerGame.Static.GraphicsDevice.BlendState = bs;
            }   */
        }
    }
}

using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Textures;

using SharpDX.Toolkit;
using MinerWars.GameServices;

namespace MinerWars.AppCode.ExternalEditor
{

    public class MyEditorBase
    {
        internal static MyMinerGame Static;

        public static bool IsEditorActive = false;

        public void Run(ServiceContainer services)
        {
            //MyMinerGame.OnGameInit += new OnInitEvent(Initialize);
            //MyMinerGame.OnGameUpdate += new OnDrawEvent(Update);
            //MyMinerGame.OnGameDraw += new OnDrawEvent(Draw);
            MyGuiScreenGamePlay.OnGameLoaded += new EventHandler(GameLoaded);
            MyParticlesManager.OnDraw += new EventHandler(EffectsDraw);

            Static = new MyMinerGame(services);
            Static.Run();
        }

        public virtual void Initialize(SharpDX.Toolkit.Game game)
        {
        }

        public virtual void Update(GameTime gt)
        {
        }

        public virtual void Draw(GameTime gt)
        {
            MinerWars.AppCode.Game.HUD.MyHudNotification.ClearAllNotifications();
        }

        public virtual void GameLoaded(object sender, EventArgs e)
        {
    }


        ////

        public MyParticleEffect CreateParticle(int id)
        {
            //MyParticleEffect effect = Static.GraphicsManager.ParticlesManager.CreateParticleEffect(id);
            MyParticleEffect effect = MyParticlesLibrary.CreateParticleEffect(id);
            return effect;
}

        public void RemoveParticle(MyParticleEffect effect)
        {
            effect.Clear();
            MyParticlesLibrary.RemoveParticleEffectInstance(effect);
        }

        public Matrix GetSpectatorMatrix()
        {
            Matrix worldMatrix = Matrix.Invert(MySpectator.GetViewMatrix());
            return worldMatrix;
        }

        public MyParticleGeneration AllocateGeneration()
        {
            return MyParticlesManager.GenerationsPool.Allocate();
        }

        //
        public MyParticleEffect CreateLibraryEffect()
        {
            MyParticleEffect effect = MyParticlesManager.EffectsPool.Allocate();
            return effect;
        }

        public void AddParticleToLibrary(MyParticleEffect effect)
        {
            MyParticlesLibrary.AddParticleEffect(effect);
        }

        public void UpdateParticleLibraryID(int ID)
        {
            MyParticlesLibrary.UpdateParticleEffectID(ID);
        }

        public void RemoveParticleFromLibrary(int ID)
        {
            MyParticlesLibrary.RemoveParticleEffect(ID);
        }

        public IEnumerable<MyParticleEffect> GetLibraryEffects()
        {
            return MyParticlesLibrary.GetParticleEffects();
        }


        public void SaveParticlesLibrary(string file)
        {
            MyParticlesLibrary.Serialize(file);
        }

        public void LoadParticlesLibrary(string file)
        {
            MyParticlesLibrary.Deserialize(file);
        }

        public float GetStepInSeconds()
        {
            return MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
        }

        public void UpdateScreenSize(int width, int height)
        {
            //MinerWars.AppCode.Game.VideoMode.MyVideoModeManager.BeginChangeVideoMode(true, new Game.VideoMode.MyVideoModeEx(width, height, height / (float)width), false,
            //    false, MyConfig.HardwareCursor, MyConfig.RenderQuality, MyConfig.FieldOfView, false, null, null);
            //MinerWars.AppCode.Game.VideoMode.MyVideoModeManager.ApplyChanges();
        }

        public void ReloadTextures()
        {
            MyTextureManager.ReloadTextures();
        }

        public virtual void EffectsDraw(object sender, EventArgs e)
        {
        }
    }
}

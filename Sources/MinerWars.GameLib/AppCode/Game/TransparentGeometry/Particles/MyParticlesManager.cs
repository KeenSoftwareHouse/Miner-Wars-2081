#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MinerWarsMath;

using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Generics;
using KeenSoftwareHouse.Library.Trace;
using ParallelTasks;

#endregion

namespace MinerWars.AppCode.Game.TransparentGeometry.Particles
{                       
    internal static class MyParticlesManager 
    {
        public static bool Enabled;
        public static event EventHandler OnDraw = null;

        public static float BirthMultiplierOverall = 1.0f;

        static List<MyParticleEffect> m_effectsToDelete = new List<MyParticleEffect>();

        /// <summary>
        /// Event that occures when we want update particles
        /// </summary>
        //private static readonly AutoResetEvent m_updateParticlesEvent;
        //private static volatile bool m_updateCompleted = true;
        private static bool MultithreadedPrepareForDraw = true;
        static Task m_prepareForDrawTask;


        #region Pools

        public static MyObjectsPool<MyParticleGeneration> GenerationsPool = new MyObjectsPool<MyParticleGeneration>(4096);
        public static MyObjectsPool<MyParticleEffect> EffectsPool = new MyObjectsPool<MyParticleEffect>(2048);


        #endregion

        static List<MyParticleEffect> m_particleEffectsForUpdate = new List<MyParticleEffect>();
        static List<MyParticleEffect> m_particleEffectsAll = new List<MyParticleEffect>();

        /// <summary>
        /// Event that occures when we want update particles
        /// </summary>
        //private static readonly AutoResetEvent m_prepareForDrawEvent;
        //private static volatile bool m_prepareForDrawCompleted = true;
        private static bool MultithreadedUpdater = true;
        static Task m_updaterTask;

        static ActionWork m_updateEffectsWork = new ActionWork(UpdateEffects);
        static ActionWork m_prepareEffectsWork = new ActionWork(PrepareEffectsForDraw);

        static MyParticlesManager() 
        {
            Enabled = true;

#if RENDER_PROFILING
            MultithreadedUpdater = false;
#endif

            if (MultithreadedUpdater)
            {
                //m_updateParticlesEvent = new AutoResetEvent(false);
                //Task.Factory.StartNew(BackgroundUpdater, TaskCreationOptions.PreferFairness);
            }

            MyRender.RegisterRenderModule(MyRenderModuleEnum.AnimatedParticlesPrepare, "Animated particles prepare", PrepareForDraw, MyRenderStage.PrepareForDraw, 0, true);

#if RENDER_PROFILING
            MultithreadedPrepareForDraw = false;
#endif

            if (MultithreadedPrepareForDraw)
            {
                //m_prepareForDrawEvent = new AutoResetEvent(false);
                //Task.Factory.StartNew(PrepareForDrawBackground, TaskCreationOptions.PreferFairness);
            }
        }

        public static MyParticleEffect CreateParticleEffect(int id, bool userDraw = false)
        {
            //Because XNA can call Update() more times per frame
            WaitUntilUpdateCompleted();

            MyParticleEffect effect = MyParticlesLibrary.CreateParticleEffect(id);

            // This could more likely be caused by empty generation pool (which is allowed) then error in xml
            //System.Diagnostics.Debug.Assert(effect.GetGenerations().Count > 0);

            if (effect != null)
            {
                System.Diagnostics.Debug.Assert(m_updaterTask.IsComplete == true);

                if (!userDraw)
                {
                    m_particleEffectsForUpdate.Add(effect);
                }
                else
                {
                    effect.AutoDelete = false;
                }

                effect.UserDraw = userDraw;

                m_particleEffectsAll.Add(effect);
            }
            
            return effect;
        }

        public static void RemoveParticleEffect(MyParticleEffect effect, bool fromBackground = false)
        {
            //System.Diagnostics.Debug.Assert(m_updateCompleted == true);

            //Because XNA can call Update() more times per frame
            if (!fromBackground)
                WaitUntilUpdateCompleted();

            if (!effect.UserDraw /*&& effect.Enabled*/)
            {
                System.Diagnostics.Debug.Assert(m_particleEffectsForUpdate.Contains(effect));
                m_particleEffectsForUpdate.Remove(effect);
            }

            m_particleEffectsAll.Remove(effect);

            MyParticlesLibrary.RemoveParticleEffectInstance(effect);
        }

        public static void CloseAll()
        {
            System.Diagnostics.Debug.Assert(m_updaterTask.IsComplete == true);

            WaitUntilUpdateCompleted();

            foreach (MyParticleEffect effect in m_particleEffectsForUpdate)
            {
                m_effectsToDelete.Add(effect);
            }

            foreach (MyParticleEffect effect in m_effectsToDelete)
            {
                RemoveParticleEffect(effect);
            }
            m_effectsToDelete.Clear();

            System.Diagnostics.Debug.Assert(m_particleEffectsAll.Count == 0);
        }

        public static void Update()
        {
            if (App.MyMinerGame.IsPaused())
                return;
            MyPerformanceCounter.PerCameraDraw.ParticleEffectsTotal = 0;
            MyPerformanceCounter.PerCameraDraw.ParticleEffectsDrawn = 0;

            if (MultithreadedUpdater)
            {
                WaitUntilUpdateCompleted();

                m_updaterTask = Parallel.Start(m_updateEffectsWork, null);
            }
            else
            {
                UpdateEffects();
            }
        }

        private static void UpdateEffects()
        {
            foreach (MyParticleEffect effect in m_particleEffectsForUpdate)
            {
                MyPerformanceCounter.PerCameraDraw.ParticleEffectsTotal++;

                if (effect.Update())
                    m_effectsToDelete.Add(effect);
            }

            foreach (MyParticleEffect effect in m_effectsToDelete)
            {
                RemoveParticleEffect(effect, true);
            }
            m_effectsToDelete.Clear();
        }

        public static void WaitUntilUpdateCompleted()
        {
            m_updaterTask.Wait();
        }

        public static void PrepareForDraw()
        {
            m_effectsForCustomDraw.Clear();

            if (MultithreadedPrepareForDraw)
            {
                m_prepareForDrawTask = Parallel.Start(m_prepareEffectsWork);

                //m_prepareForDrawCompleted = false;
                //m_prepareForDrawEvent.Set();
            }
            else
            {
                PrepareEffectsForDraw();
            }
        }

         private static void PrepareEffectsForDraw()
        {
            WaitUntilUpdateCompleted();

            foreach (MyParticleEffect effect in m_particleEffectsForUpdate)
            {
                effect.PrepareForDraw();
            }
        }

        private static void WaitUntilPrepareForDrawCompleted()
        {
            m_prepareForDrawTask.Wait();
        }


        public static void Draw()
        {
            //MyTrace.Send(TraceWindow.ParallelParticles, "BEGIN MyParticlesManager Draw()");
            WaitUntilPrepareForDrawCompleted();
            //MyTrace.Send(TraceWindow.ParallelParticles, "PrepareForDrawCompleted");

            foreach (MyParticleEffect effect in m_particleEffectsForUpdate)
            {
                effect.Draw();
            }

            if (!App.MyMinerGame.IsPaused())
            {
                foreach (MyParticleEffect effect in m_effectsForCustomDraw)
                {
                    effect.Update();
                    effect.PrepareForDraw();
                    effect.Draw();
                }

                m_effectsForCustomDraw.Clear();
            }

            if (OnDraw != null)
                OnDraw(null, null);

           // MyTrace.Send(TraceWindow.ParallelParticles, "DONE MyParticlesManager Draw()");
            System.Diagnostics.Debug.Assert(m_prepareForDrawTask.IsComplete);
        }


        static List<MyParticleEffect> m_effectsForCustomDraw = new List<MyParticleEffect>();

        public static void CustomDraw(MyParticleEffect effect)
        {
            System.Diagnostics.Debug.Assert(effect != null);

            m_effectsForCustomDraw.Add(effect);
        }
    }
}

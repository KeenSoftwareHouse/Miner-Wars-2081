using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Managers;

namespace MinerWars.AppCode.Game.TransparentGeometry.Particles
{
    public enum LastFrameVisibilityEnum
    {
        AlwaysVisible,
        NotVisibleLastFrame,
        VisibleLastFrame
    }

    public class MyParticleEffect
    {
        public static readonly uint FRAMES_TO_SKIP = 20;

        public event EventHandler OnDelete = null;
        public event EventHandler OnUpdate = null;

        #region Members

        //Version of the effect for serialization
        static readonly int Version = 0;

        int m_particleID; //ID of the particle stored in particles library
        float m_elapsedTime = 0; //Time elapsed from start of the effect
        string m_name; //Name of the effect
        float m_length = 90; //Length of the effect in seconds
        float m_preload; //Time in seconds to preload
        bool m_isPreloading;
        bool m_wasPreloaded;

        float m_birthRate = 0;
        bool m_hasShownSomething = false;
        bool m_isStopped = false;

        Matrix m_worldMatrix;
        Matrix m_lastWorldMatrix;
        int m_particlesCount;
        float m_distance;

        List<MyParticleGeneration> m_generations = new List<MyParticleGeneration>();
        List<MyParticleGeneration> m_sortedGenerations = new List<MyParticleGeneration>();
        List<MyParticleEffect> m_instances;
        BoundingBox m_AABB = new BoundingBox();


        public bool AutoDelete;
        public bool Enabled;
        public bool EnableLods;
        public float UserEmitterScale;
        public float UserBirthMultiplier;
        public float UserRadiusMultiplier;
        public float UserScale;
        public Vector4 UserColorMultiplier;
        public bool UserDraw;

        public bool IsInFrustum { get; private set; }
        public bool CalculateDeltaMatrix;
        public bool Near;
        public Matrix DeltaMatrix;
        //public LastFrameVisibilityEnum WasVisibleLastFrame = LastFrameVisibilityEnum.AlwaysVisible;
        public uint RenderCounter = 0;
        public bool LowRes;
        
        #endregion

        #region Start & Close

        public MyParticleEffect()
        {
            Enabled = true;
        }

        public void Start(int particleID)
        {
            System.Diagnostics.Debug.Assert(m_particlesCount == 0);
            System.Diagnostics.Debug.Assert(m_elapsedTime == 0);

            m_particleID = particleID;
            m_name = "ParticleEffect";

            m_isPreloading = false;
            m_wasPreloaded = false;
            m_isStopped = false;
            m_hasShownSomething = false;
            m_distance = 0;

            UserEmitterScale = 1.0f;
            UserBirthMultiplier = 1.0f;
            UserRadiusMultiplier = 1.0f;
            UserScale = 1.0f;
            UserColorMultiplier = Vector4.One;
            UserDraw = false;
            LowRes = false;

            Enabled = true;
            AutoDelete = true;
            EnableLods = true;
            Near = false;

            //For assigment check
            WorldMatrix = MyUtils.ZeroMatrix;
            DeltaMatrix = Matrix.Identity;
            CalculateDeltaMatrix = false;
            RenderCounter = 0;
        }

        public void Restart()
        {
            m_elapsedTime = 0;
        }

        public void Close(bool done)
        {
            if (!done && OnDelete != null)
                OnDelete(this, null);

            Clear();

            m_name = "ParticleEffect";

            foreach (MyParticleGeneration generation in m_generations)
            {
                if (done)
                    generation.Done();
                else
                    generation.Close();
                MyParticlesManager.GenerationsPool.Deallocate(generation);
            }

            m_generations.Clear();

            if (m_instances != null)
            {
                while (m_instances.Count > 0)
                {
                    MyParticlesManager.RemoveParticleEffect(m_instances[0]);
                }
            }

            OnDelete = null;
            OnUpdate = null;

            Tag = null;
        }

        public void Clear()
        {
            m_elapsedTime = 0;
            m_birthRate = 0;
            m_particlesCount = 0;
            m_wasPreloaded = false;
            m_hasShownSomething = false;

            foreach (MyParticleGeneration generation in m_generations)
            {
                generation.Clear();
            }

            if (m_instances != null)
            {
                foreach (MyParticleEffect effect in m_instances)
                {
                    effect.Clear();
                }
            }
        }

        public MyParticleEffect CreateInstance()
        {
            MyParticleEffect effect = MyParticlesManager.EffectsPool.Allocate();
            effect.Start(m_particleID);

            effect.Name = Name;
            effect.Enabled = Enabled;
            effect.SetLength(GetLength());
            effect.SetPreload(GetPreload());
            effect.LowRes = LowRes;

            foreach (MyParticleGeneration generation in m_generations)
            {
                MyParticleGeneration gen = generation.CreateInstance(effect);
                if (gen != null)
                {
                    effect.AddGeneration(gen);
                }
            }

            if (m_instances == null)
                m_instances = new List<MyParticleEffect>();

            m_instances.Add(effect);

            return effect;
        }

        /// <summary>
        /// This methods stops generating any new particles
        /// </summary>
        public void Stop(bool autodelete = true)
        {
            m_isStopped = true;
            AutoDelete = autodelete ? true : AutoDelete;
        }

        public void RemoveInstance(MyParticleEffect effect)
        {
            if (m_instances != null)
            {
                if (m_instances.Contains(effect))
                    m_instances.Remove(effect);
            }
        }

        public List<MyParticleEffect> GetInstances()
        {
            return m_instances;
        }

        public MyParticleEffect Duplicate()
        {
            MyParticleEffect effect = MyParticlesManager.EffectsPool.Allocate();
            effect.Start(0);

            effect.Name = Name;
            effect.m_preload = m_preload;
            effect.m_length = m_length;

            foreach (MyParticleGeneration generation in m_generations)
            {
                MyParticleGeneration duplicatedGeneration = generation.Duplicate(effect);
                effect.AddGeneration(duplicatedGeneration);
            }

            return effect;
        }

        #endregion

        #region Update

        public bool Update()
        {
            if (!Enabled)
                return AutoDelete; //efect is not enabled at all and must be deleted

            System.Diagnostics.Debug.Assert(WorldMatrix != MyUtils.ZeroMatrix, "Effect world matrix was not set!");

            if (!m_isPreloading && !m_wasPreloaded && m_preload > 0)
            {
                m_isPreloading = true;

                // TODO: Optimize (preload causes lags, depending on preload size, it's from 0 ms to 85 ms)
                //while (m_elapsedTime < m_preload)
                //{
                //    Update();
                //}

                m_isPreloading = false;
                m_wasPreloaded = true;
            }

            MyRender.GetRenderProfiler().StartProfilingBlock("ParticleEffect-Update");

            if (!m_isPreloading && IsInFrustum)
                MyPerformanceCounter.PerCameraDraw.ParticleEffectsDrawn++; 

            MyRender.GetRenderProfiler().EndProfilingBlock();

            MyRender.GetRenderProfiler().StartProfilingBlock("ParticleEffect-UpdateGen");

            m_elapsedTime += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_distance = MyCamera.GetDistanceWithFOV(WorldMatrix.Translation) / (UserScale * 1000.0f);
            m_particlesCount = 0;
            m_birthRate = 0;
            m_AABB = m_AABB.CreateInvalid();


            if (CalculateDeltaMatrix)
            {
                DeltaMatrix = Matrix.Invert(m_lastWorldMatrix) * m_worldMatrix;
            }

            if (RenderCounter == 0 || ((MyRender.RenderCounter - RenderCounter) < FRAMES_TO_SKIP)) //more than FRAMES_TO_SKIP frames consider effect as invisible
            {
                foreach (MyParticleGeneration generation in m_generations)
                {
                    generation.EffectMatrix = WorldMatrix;
                    generation.Update();
                    m_particlesCount += generation.GetParticlesCount();
                    m_birthRate += generation.GetBirthRate();

                    BoundingBox bbox = generation.GetAABB();
                    m_AABB = m_AABB.Include(ref bbox);
                }
                m_lastWorldMatrix = m_worldMatrix;


                if (m_particlesCount > 0)
                    m_hasShownSomething = true;

                IsInFrustum = MyCamera.IsInFrustum(ref m_AABB);
            }

            MyRender.GetRenderProfiler().EndProfilingBlock();

            if (((m_particlesCount == 0 && HasShownSomething())
                || (m_particlesCount == 0 && m_birthRate == 0.0f))
                && AutoDelete && !m_isPreloading)
            {   //Effect was played and has to be deleted
                return true;
            }

            if (!m_isPreloading && OnUpdate != null)
                OnUpdate(this, null);

            return false;
        }

        #endregion

        #region Properties

        public float GetElapsedTime()
        {
            return m_elapsedTime;
        }

        public int GetID()
        {
            return m_particleID;
        }

        public int GetParticlesCount()
        {
            return m_particlesCount;
        }

        public void SetID(int id)
        {
            m_particleID = id;
        }

        public string GetName()
        {
            return m_name;
        }

        public void SetName(string name)
        {
            m_name = name;
        }

        public float GetLength()
        {
            return m_length;
        }

        public void SetLength(float length)
        {
            m_length = length;
        }

        public bool HasShownSomething()
        {
            return m_hasShownSomething;
        } 

        public Matrix WorldMatrix
        {
            get { return m_worldMatrix; }
            set { m_worldMatrix = value; }
        }

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public float GetPreload()
        {
            return m_preload;
        }

        public void SetPreload(float preload)
        {
            m_preload = preload;

            if (m_instances != null)
            {
                foreach (MyParticleEffect effect in m_instances)
                {
                    effect.SetPreload(preload);
                }
            }
        }

        public float Distance
        {
            get { return m_distance; }
        }

        public object Tag { get; set; }

        #endregion

        #region Generations

        public void AddGeneration(MyParticleGeneration generation)
        {
            m_generations.Add(generation);

            if (m_instances != null)
            {
                foreach (MyParticleEffect effect in m_instances)
                {
                    effect.AddGeneration(generation.CreateInstance(effect));
                }
            }
        }

        public void RemoveGeneration(int index)
        {
            MyParticleGeneration generation = m_generations[index];
            m_generations.Remove(generation);

            generation.Close();
            MyParticlesManager.GenerationsPool.Deallocate(generation);

            if (m_instances != null)
            {
                foreach (MyParticleEffect effect in m_instances)
                {
                    effect.RemoveGeneration(index);
                }
            }
        }

        public void RemoveGeneration(MyParticleGeneration generation)
        {
            int index = m_generations.IndexOf(generation);
            RemoveGeneration(index);
        }

        public List<MyParticleGeneration> GetGenerations()
        {
            return m_generations;
        }

        public bool IsStopped
        {
            get { return m_isStopped; }
        }

        public BoundingBox GetAABB()
        {
            return m_AABB;
        }
    

        #endregion
        
        #region Serialization

        public void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement("ParticleEffect");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("version", Version.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("ID", m_particleID.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("Length", m_length.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("Preload", m_preload.ToString(CultureInfo.InvariantCulture));

            writer.WriteElementString("LowRes", LowRes.ToString(CultureInfo.InvariantCulture).ToLower());
            
            writer.WriteStartElement("Generations");

            foreach (MyParticleGeneration generation in m_generations)
            {
                generation.Serialize(writer);
            }

            writer.WriteEndElement(); //Generations

            writer.WriteEndElement(); //ParticleEffect
        }

        public void Deserialize(XmlReader reader)
        {
            m_name = reader.GetAttribute("name");
            int version = Convert.ToInt32(reader.GetAttribute("version"));

            reader.ReadStartElement(); //ParticleEffect
            
            m_particleID = reader.ReadElementContentAsInt();

            m_length = reader.ReadElementContentAsFloat();

            m_preload = reader.ReadElementContentAsFloat();

            if (reader.Name == "LowRes")
                LowRes = reader.ReadElementContentAsBoolean();

            bool isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement(); //Generations

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                MyParticleGeneration generation = MyParticlesManager.GenerationsPool.Allocate();
                generation.Start(this);
                generation.Init();

                generation.Deserialize(reader);

                AddGeneration(generation);
            }

            if (!isEmpty)
                reader.ReadEndElement(); //Generations

            reader.ReadEndElement(); //ParticleEffect
        }

        #endregion

        #region Draw

        public void PrepareForDraw()
        {
            //if (WasVisibleLastFrame != LastFrameVisibilityEnum.NotVisibleLastFrame)
            if (RenderCounter == 0 || ((MyRender.RenderCounter - RenderCounter) < FRAMES_TO_SKIP)) //more than FRAMES_TO_SKIP frames consider effect as invisible
            {
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Sort generations");
                m_sortedGenerations.Clear();

                foreach (MyParticleGeneration generation in m_generations)
                {
                    m_sortedGenerations.Add(generation);
                }

                m_sortedGenerations.Sort();
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

                MyBillboard effectBillboard = null;

                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("PrepareForDraw generations");
                foreach (MyParticleGeneration generation in m_sortedGenerations)
                {
                    generation.PrepareForDraw(ref effectBillboard);
                }
                MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
            }
        }

        public void Draw()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Draw generations");
            foreach (MyParticleGeneration generation in m_sortedGenerations)
            {
                generation.Draw();
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        #endregion

        #region DebugDraw

        public void DebugDraw()
        {
            MyDebugDraw.DrawAxis(WorldMatrix, 1.0f, 1.0f);
            MyDebugDraw.DrawSphereWireframe(WorldMatrix.Translation, 0.1f, Vector3.One, 1.0f);

            foreach (MyParticleGeneration generation in m_generations)
            {
                generation.DebugDraw();
            }

            Color color = !m_isStopped ? Color.White : Color.Red;
            MyDebugDraw.DrawText(WorldMatrix.Translation, new System.Text.StringBuilder(GetID().ToString() + " [" + GetParticlesCount().ToString() + "]") , color, 1.0f);

            // Vector4 colorV = color.ToVector4();
            // MyDebugDraw.DrawAABB(ref m_AABB, ref colorV, 1.0f);
        }

        #endregion
    }


}

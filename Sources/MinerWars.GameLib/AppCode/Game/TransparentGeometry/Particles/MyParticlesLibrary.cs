#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

using MinerWarsMath;

using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;

#endregion

namespace MinerWars.AppCode.Game.TransparentGeometry.Particles
{
    class MyParticlesLibrary
    {
        static Dictionary<int, MyParticleEffect> m_libraryEffects = new Dictionary<int, MyParticleEffect>();
        static readonly int Version = 0;

        static MyParticlesLibrary()
        {
            MyMwcLog.WriteLine(string.Format("MyParticlesLibrary.ctor - START"));
            InitDefault();
            MyMwcLog.WriteLine(string.Format("MyParticlesLibrary.ctor - END"));
        }

        public static void Init()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.AnimatedParticles, "Animated particles", DebugDraw, MyRenderStage.DebugDraw, 200, false);
        }

        public static void InitDefault()
        {
            Deserialize("Content\\Particles\\MyParticlesLibrary.mwl");

            /*
            MyParticleEffect effect = MyParticlesManager.EffectsPool.Allocate();
            effect.Start(666);

            m_libraryEffects.Add(effect.GetID(), effect);
            
            MyParticleGeneration generation = MyParticlesManager.GenerationsPool.Allocate();
            generation.Start(effect);
            generation.Init();
            generation.InitDefault();
            effect.AddGeneration(generation);*/
        }

        public static void AddParticleEffect(MyParticleEffect effect)
        {
            m_libraryEffects.Add(effect.GetID(), effect);
        }

        public static void UpdateParticleEffectID(int ID)
        {
            MyParticleEffect effect;
            m_libraryEffects.TryGetValue(ID, out effect);
            if (effect != null)
            {
                m_libraryEffects.Remove(ID);
                m_libraryEffects.Add(effect.GetID(), effect);
            }
        }

        public static void RemoveParticleEffect(int ID)
        {
            MyParticleEffect effect;
            m_libraryEffects.TryGetValue(ID, out effect);
            if (effect != null)
            {
                effect.Close(true);
                MyParticlesManager.EffectsPool.Deallocate(effect);
            }

            m_libraryEffects.Remove(ID);
        }

        public static void RemoveParticleEffect(MyParticleEffect effect)
        {
            RemoveParticleEffect(effect.GetID());
        }

        public static IEnumerable<MyParticleEffect> GetParticleEffects()
        {
            return m_libraryEffects.Values;
        }

        #region Serialization

        static public void Serialize(string file)
        {
            FileStream fs = File.Create(file);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
            };
            XmlWriter writer = XmlWriter.Create(fs, settings);

            Serialize(writer);

            writer.Flush();

            fs.Dispose();
        }

        static public void Deserialize(string file)
        {
            FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreWhitespace = true,
            };
            XmlReader reader = XmlReader.Create(fs, settings);

            Deserialize(reader);

            fs.Dispose();
        }


        static public void Serialize(XmlWriter writer)
        {
            writer.WriteStartElement("MinerWarsParticlesLibrary");

            writer.WriteElementString("Version", Version.ToString(CultureInfo.InvariantCulture));

            writer.WriteStartElement("ParticleEffects");

            foreach (KeyValuePair<int, MyParticleEffect> pair in m_libraryEffects)
            {
                pair.Value.Serialize(writer);
            }

            writer.WriteEndElement(); //ParticleEffects

            writer.WriteEndElement(); //root
        }

        static void Close()
        {
            foreach (MyParticleEffect effect in m_libraryEffects.Values)
            {
                effect.Close(true);
                MyParticlesManager.EffectsPool.Deallocate(effect);
            }
            
            m_libraryEffects.Clear();
        }

        public static int RedundancyDetected = 0;

        static public void Deserialize(XmlReader reader)
        {
            Close();
            RedundancyDetected = 0;

            reader.ReadStartElement(); //MinerWarsParticlesLibrary

            int version = reader.ReadElementContentAsInt();

            reader.ReadStartElement(); //ParticleEffects

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                MyParticleEffect effect = MyParticlesManager.EffectsPool.Allocate();
                effect.Deserialize(reader);
                m_libraryEffects.Add(effect.GetID(), effect);
            }

            reader.ReadEndElement(); //ParticleEffects

            reader.ReadEndElement(); //root
        }

        #endregion

        static public MyParticleEffect CreateParticleEffect(int id)
        {
            return m_libraryEffects[id].CreateInstance();
        }

        static public void RemoveParticleEffectInstance(MyParticleEffect effect)
        {
            effect.Close(false);
            //if (effect.Enabled)
            {
                if (m_libraryEffects[effect.GetID()].GetInstances().Contains(effect))
                {
                    MyParticlesManager.EffectsPool.Deallocate(effect);
                    m_libraryEffects[effect.GetID()].RemoveInstance(effect);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Effect deleted twice!");
                }
            }
        }

        static public void DebugDraw()
        {
           // if (MinerWars.AppCode.ExternalEditor.MyEditorBase.IsEditorActive)
            {
                foreach (MyParticleEffect effect in m_libraryEffects.Values)
                {
                    List<MyParticleEffect> instances = effect.GetInstances();
                    if (instances != null)
                    {
                        foreach (MyParticleEffect instance in instances)
                        {
                            instance.DebugDraw();
                        }
                    }
                }
            }
        }

    }
}

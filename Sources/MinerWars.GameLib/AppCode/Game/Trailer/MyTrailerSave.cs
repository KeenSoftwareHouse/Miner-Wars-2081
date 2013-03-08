using System;
using System.Collections.Generic;
using System.IO;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//  This save the scene so it can be later played as a trailer. It loads correct voxel maps, textures, ships paths and camera path.

namespace MinerWars.AppCode.Game.Trailer
{
    //  This class represents state of one phys object at one update call
    class MyPhysObjectTrackedTickData
    {
        public Vector3 Position;
        public Matrix Orientation;
        public MyTrailerGunsShotTypeEnum? GunShot;
        public float ReflectorLevel;
        public float EngineLevel;
    }

    static class MyTrailerSave
    {
        class MyTrailerSaveObjectHolder
        {
            public string Name;
            public Dictionary<int, MyPhysObjectTrackedTickData> Ticks;

            private MyTrailerSaveObjectHolder() { }

            public MyTrailerSaveObjectHolder(string name)
            {
                Name = name;
                Ticks = new Dictionary<int, MyPhysObjectTrackedTickData>(MyTrailerConstants.MAX_TRACKED_TICKS);
            }            
        }

        //  Every object we track stores here its per-frame data
        static Dictionary<MyEntity, MyTrailerSaveObjectHolder> m_trackedPhysObjects;
        static Dictionary<string, string> m_uniqueNameDictionary;

        static int m_activeTick;
        

        public static void LoadContent()
        {
            m_activeTick = 0;
            m_trackedPhysObjects = new Dictionary<MyEntity, MyTrailerSaveObjectHolder>();
            m_uniqueNameDictionary = new Dictionary<string, string>();
        }

        //  Add objects to the list so we start tracking him
        public static void AttachPhysObject(string name, MyEntity physObject)
        {
            //  Check if name is unique
            string getName;
            if (m_uniqueNameDictionary.TryGetValue(name, out getName) == true)
            {
                throw new Exception("Name of object attached to TrailerSave must be unique!");
            }

            //  Add to list
            m_trackedPhysObjects.Add(physObject, new MyTrailerSaveObjectHolder(name));
        }

        public static void UpdatePositionsAndOrientations()
        {
            foreach (KeyValuePair<MyEntity, MyTrailerSaveObjectHolder> kvp in m_trackedPhysObjects)
            {
                MyPhysObjectTrackedTickData tickData = InsertActiveTick(kvp.Key);
                tickData.Position = kvp.Key.GetPosition();
                tickData.Orientation = kvp.Key.WorldMatrix;
                tickData.ReflectorLevel = ((MySmallShip)kvp.Key).Config.ReflectorLight.Level;
                tickData.EngineLevel = ((MySmallShip)kvp.Key).Config.Engine.Level;
            }            
        }
        
        public static void UpdateGunShot(MyEntity physObject, MyTrailerGunsShotTypeEnum gunShot)
        {
            //  This method will be called even on not-tracker objects, so here we check if we need to store data about this one
            MyPhysObjectTrackedTickData tickData = InsertActiveTick(physObject);
            if (tickData != null)
            {
                tickData.GunShot = gunShot;
            }
        }

        //  This must be called as last thing in main update method
        public static void IncreaseActiveTick()
        {
            m_activeTick++;
        }

        //  Adds data for active tick into the list. First we check if data for this thick doesn't exist and if not, add it.
        static MyPhysObjectTrackedTickData InsertActiveTick(MyEntity physObject)
        {
            MyTrailerSaveObjectHolder ticks;

            //  This method will be called even on not-tracker objects, so here we check if we need to store data about this one
            if (m_trackedPhysObjects.TryGetValue(physObject, out ticks) == true)
            {
                MyPhysObjectTrackedTickData tickData;

                if (ticks.Ticks.TryGetValue(m_activeTick, out tickData) == true)
                {
                    return tickData;
                }

                tickData = new MyPhysObjectTrackedTickData();
                ticks.Ticks.Add(m_activeTick, tickData);
                return tickData;
            }

            return null;
        }

        public static void RemoveTrackedObjects()
        {
            m_trackedPhysObjects.Clear();
        }

        public static void ResetTicks()
        {
            LoadContent();
        }

        //  Save tracked data to files
        public static void Save()
        {
            if (m_trackedPhysObjects == null) return;

            foreach (KeyValuePair<MyEntity, MyTrailerSaveObjectHolder> kvp in m_trackedPhysObjects)
            {
                var path = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Trailer");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //  Name is unique, that's preserved by Add() method
                using (FileStream fs = File.Create(Path.Combine(path, kvp.Value.Name + ".tracked")))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        foreach (KeyValuePair<int, MyPhysObjectTrackedTickData> kvpTicks in kvp.Value.Ticks)
                        {
                            MyUtils.BinaryWrite(bw, ref kvpTicks.Value.Position);
                            MyUtils.BinaryWrite(bw, ref kvpTicks.Value.Orientation);
                            bw.Write(kvpTicks.Value.ReflectorLevel);
                            bw.Write(kvpTicks.Value.EngineLevel);

                            if (kvpTicks.Value.GunShot == null)
                            {
                                bw.Write((int) -1);
                            }
                            else
                            {
                                bw.Write((int) kvpTicks.Value.GunShot.Value);
                            }
                        }
                    }
                }
            }
        }
    }
}

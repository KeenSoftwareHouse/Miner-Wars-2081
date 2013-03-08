namespace MinerWars.AppCode.Game.Managers.EntityManager.Entities.InfluenceSpheres
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using GUI;
    using Microsoft.Xna.Framework;
    using SysUtils.Utils;
    using Utils;

    //This class holds all influence spheres in a sector and is responsible for adding, removing drawing and updating them.
    class MyInfluenceSpheres
    {
        public bool DrawHelpers;  //  This property helps to control, weather draw bounding spheres of influence spheres(ingame, they should be drawn)
        List<MyInfluenceSphereBase> m_sortedSoundSpheres;  // Each type of influence sphere will has its corresponding list
        List<MyInfluenceSphereBase> m_sortedDustSpheres;
        List<MyInfluenceSphereBase> m_soundSpheresSAP;
        List<MyInfluenceSphereBase> m_dustSpheresSAP;
        public static Vector4 ResultDustColor;  // Property holds dust color, that will be used by MyParticleDustField for drawing dust, if it is Vector4.Zero, MyGuiScreenGamePlay.Static.DustColor is used
        List<Vector4> m_dustColors; // Helper list containing interpolated color, but before changing them based on their weights in multiple dusts
        float m_largestSoundX;
        float m_largestDustX;

        List<MyInfluenceSphereBase> m_spheresIterationHelper;

        // Main Constructor
        public MyInfluenceSpheres()
        {
        }

        public void LoadData()
        {
            MyMwcLog.WriteLine("MyInfluenceSpheres.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            m_sortedSoundSpheres = new List<MyInfluenceSphereBase>(MyInfluenceSphereConstants.MAX_SOUND_SPHERES_COUNT_FOR_SORT);
            m_sortedDustSpheres = new List<MyInfluenceSphereBase>(MyInfluenceSphereConstants.MAX_DUST_SPHERES_COUNT_FOR_SORT);
            m_soundSpheresSAP = new List<MyInfluenceSphereBase>(MyInfluenceSphereConstants.MAX_SOUND_SPHERES_COUNT);
            m_dustSpheresSAP = new List<MyInfluenceSphereBase>(MyInfluenceSphereConstants.MAX_DUST_SPHERES_COUNT);
            m_spheresIterationHelper = new List<MyInfluenceSphereBase>();
            m_dustColors = new List<Vector4>(MyInfluenceSphereConstants.MAX_DUST_SPHERES_COUNT_FOR_SORT);

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyInfluenceSpheres.LoadContent() - END");
        }

        public void UnloadData()
        {
            if (m_sortedSoundSpheres != null) m_sortedSoundSpheres.Clear();
            if (m_sortedDustSpheres != null) m_sortedDustSpheres.Clear();
            if (m_soundSpheresSAP != null) m_soundSpheresSAP.Clear();
            if (m_dustSpheresSAP != null) m_dustSpheresSAP.Clear();
            if (m_spheresIterationHelper != null) m_spheresIterationHelper.Clear();
            if (m_dustColors != null) m_dustColors.Clear();
        }


        //  Use this method to add any type of influence sphere
        public void Add(MyInfluenceSphereBase influenceSphere)
        {
            if (!Exist(influenceSphere))
            {
                if (influenceSphere is MyInfluenceSphereDust)
                {
                    m_dustSpheresSAP.Add(influenceSphere);
                }
                else if (influenceSphere is MyInfluenceSphereSound)
                {
                    m_soundSpheresSAP.Add(influenceSphere);
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }
            }
        }

        //  Use this method to remove any type of influence sphere
        public void Remove(MyInfluenceSphereBase influenceSphere)
        {
            if (Exist(influenceSphere))
            {
                if (influenceSphere is MyInfluenceSphereDust)
                {
                    m_dustSpheresSAP.Remove(influenceSphere);
                }
                else if (influenceSphere is MyInfluenceSphereSound)
                {
                    m_soundSpheresSAP.Remove(influenceSphere);
                }
                else
                {
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                }
            }
        }

        public bool Exist(MyInfluenceSphereBase influenceSphere)
        {
            if (influenceSphere is MyInfluenceSphereDust)
            {
                return m_dustSpheresSAP.Contains(influenceSphere);
            }
            else if (influenceSphere is MyInfluenceSphereSound)
            {
                return m_soundSpheresSAP.Contains(influenceSphere);
            }
            
            return false;
        }

        //  This method must be called on every update.
        public void Update()
        {
            BoundingSphere sphere = new BoundingSphere(MyCamera.Position, 5);
            UpdateDust(ref sphere);
            UpdateSounds(ref sphere);
        }

        int BinarySearchSAP(List<MyInfluenceSphereBase> sapList, float x)
        {
            //  It is up to the caller to make sure this isn't called on an empty collection.
            int top = sapList.Count;
            int bot = 0;
            while (top > bot)
            {
                int mid = (top + bot) >> 1;

                if ((sapList[mid].GetPosition().X - sapList[mid].WorldVolume.Radius) >= x)
                {
                    Debug.Assert(top > mid);
                    top = mid;
                }
                else
                {
                    Debug.Assert(bot <= mid);
                    bot = mid + 1;
                }
            }

            Debug.Assert(top >= 0 && top <= sapList.Count);
            Debug.Assert(top == 0 || (sapList[top - 1].GetPosition().X - sapList[top - 1].WorldVolume.Radius) < x);
            Debug.Assert(top == sapList.Count || (sapList[top].GetPosition().X - sapList[top].WorldVolume.Radius) >= x);
            return top;
        }

        // This method is responsible for getting only nearest spheres and changing color/interpolate it.
        public void UpdateDust(ref BoundingSphere boundingSphere)
        {
            foreach (MyInfluenceSphereBase dustSphere in m_dustSpheresSAP)
            {
                float dx = dustSphere.WorldVolume.Radius * 2;
                if (dx > m_largestDustX) m_largestDustX = dx;
            }

            m_sortedDustSpheres.Clear();
            m_dustColors.Clear();

            float xMin = boundingSphere.Center.X - boundingSphere.Radius;
            float xMax = boundingSphere.Center.X + boundingSphere.Radius;

            //  Take only sounds that can have influence on our bounding sphere
            int search = BinarySearchSAP(m_dustSpheresSAP, xMin - m_largestDustX);
            while (search < m_dustSpheresSAP.Count && (m_dustSpheresSAP[search].GetPosition().X - m_dustSpheresSAP[search].WorldVolume.Radius) < xMax)
            {
                MyInfluenceSphereDust dustSphere = (MyInfluenceSphereDust)m_dustSpheresSAP[search];

                if (dustSphere.Enabled == true)
                {
                    if ((dustSphere.GetPosition().X + dustSphere.WorldVolume.Radius) > xMin)
                    {
                        //  Don't add more sounds into sorting list than our 'sorting list' can handle
                        if (m_sortedDustSpheres.Count >= MyInfluenceSphereConstants.MAX_DUST_SPHERES_COUNT_FOR_SORT) break;

                        float distance = Vector3.Distance(dustSphere.GetPosition(), boundingSphere.Center);
                        float maxDistance = boundingSphere.Radius + dustSphere.WorldVolume.Radius;

                        if (distance <= maxDistance)
                        {
                            m_sortedDustSpheres.Add(dustSphere);
                        }
                    }
                }

                ++search;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Now in 'm_sortedDustSpheres' we have only dust spheres that intersects bounding sphere in SAP list
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Vector4 sectorDustColor = MyGuiScreenGamePlay.Static.SectorDustColor;

            foreach (MyInfluenceSphereDust dustSphere in m_sortedDustSpheres)
            {
                Vector4 dustSphereColorVec = dustSphere.DustColor.ToVector4();
                Vector3 influencedPosition = boundingSphere.Center;
                Vector3 soundSpherePosition = dustSphere.GetPosition();
                float distance = Math.Abs(Vector3.Distance(soundSpherePosition, influencedPosition));
                float radiusMin = dustSphere.InnerBoundingSphere.Radius;
                float radiusMax = dustSphere.WorldVolume.Radius;
               
                bool addToColors = false;
                if (distance < radiusMin)
                {
                    addToColors = true;
                }
                else if ((distance > radiusMin) && (distance <= radiusMax)) // interpolate dust between radiusMin and radiusMax
                {
                    float total = (radiusMax - radiusMin);
                    float normalizedDistance = MathHelper.Clamp((distance - radiusMin) / total, 0, 1);

                    dustSphereColorVec = Vector4.Lerp(dustSphereColorVec, Vector4.Zero, normalizedDistance);
                    addToColors = true;
                }

                if (addToColors == true) m_dustColors.Add(dustSphereColorVec);
            }

            // Now sector color is always full in every part of the sector
            m_dustColors.Add(sectorDustColor);

            // Calculate weight for each color and then calculate result color using weights
            Vector4 colorSum = Vector4.Zero;
            foreach (Vector4 color in m_dustColors)
            {
                colorSum += color;
            }

            Vector4 resultColor = Vector4.Zero;
            foreach (Vector4 color in m_dustColors)
            {
                Vector4 weight = color / colorSum;
                resultColor += color * weight;
            }

            if (resultColor == sectorDustColor) resultColor = Vector4.Zero;
            ResultDustColor = resultColor;
        }

        // This method is responsible for getting only nearest spheres and apply volume change/interpolation of ambient sounds
        public void UpdateSounds(ref BoundingSphere boundingSphere)
        {
            foreach (MyInfluenceSphereSound soundSphere in m_soundSpheresSAP)
            {
                float dx = soundSphere.WorldVolume.Radius * 2;
                if (dx > m_largestDustX) m_largestSoundX = dx;
                soundSphere.Enabled = false;

                if (MyFakes.TEST_MISSION_1_ENABLED)
                {
                    MyTestMission.UpdateInfluenceSphereSoundScript(soundSphere);
                }
            }

            m_sortedSoundSpheres.Clear();

            float xMin = boundingSphere.Center.X - boundingSphere.Radius;
            float xMax = boundingSphere.Center.X + boundingSphere.Radius;

            //  Take only sounds that can have influence on our bounding sphere
            int search = BinarySearchSAP(m_soundSpheresSAP, xMin - m_largestSoundX);
            while (search < m_soundSpheresSAP.Count && (m_soundSpheresSAP[search].GetPosition().X - m_soundSpheresSAP[search].WorldVolume.Radius) < xMax)
            {
                MyInfluenceSphereSound soundSphere = (MyInfluenceSphereSound) m_soundSpheresSAP[search];

                if ((soundSphere.GetPosition().X + soundSphere.WorldVolume.Radius) > xMin)
                {
                    //  Don't add more sounds into sorting list than our 'sorting list' can handle
                    if (m_sortedSoundSpheres.Count >= MyInfluenceSphereConstants.MAX_SOUND_SPHERES_COUNT_FOR_SORT) break;

                    float distance = Vector3.Distance(soundSphere.GetPosition(), boundingSphere.Center);
                    float maxDistance = boundingSphere.Radius + soundSphere.WorldVolume.Radius;

                    if (distance <= maxDistance)
                    {
                        m_sortedSoundSpheres.Add(soundSphere);
                    }
                }

                ++search;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Now in 'm_sortedSoundSpheres' we have only sounds that intersects bounding sphere in SAP list
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //  handle sounds
            foreach (MyInfluenceSphereSound soundSphere in m_sortedSoundSpheres)
            {
                soundSphere.Enabled = true;
                float normalizedVolume = 0.0f;
                Vector3 influencedPosition = boundingSphere.Center;
                Vector3 soundSpherePosition = soundSphere.GetPosition();
                float distance = Math.Abs(Vector3.Distance(soundSpherePosition, influencedPosition));
                float radiusMin = soundSphere.InnerBoundingSphere.Radius;
                float radiusMax = soundSphere.WorldVolume.Radius;

                // if we are located in the center sphere, sound volume must be full
                if (distance <= radiusMin)
                {
                    normalizedVolume = 1.0f;
                }
                else if ((distance > radiusMin) && (distance <= radiusMax)) // interpolate volume between radiusMin and radiusMax
                {
                    float total = (radiusMax - radiusMin);
                    float normalizedDistance = MathHelper.Clamp((distance - radiusMin) / total, 0, 1);
                    normalizedVolume = MathHelper.Lerp(1, 0, normalizedDistance);
                }

                soundSphere.SetVolume(normalizedVolume);
                soundSphere.UpdateAfterIntegration();
            }
        }

        public void Draw()
        {
            foreach (MyInfluenceSphereSound sphereSound in m_soundSpheresSAP)
            {
                sphereSound.Draw();
            }

            foreach (MyInfluenceSphereDust sphereDust in m_dustSpheresSAP)
            {
                sphereDust.Draw();
            }
        }

        void PrepareSpheresIterationHelper()
        {
            m_spheresIterationHelper.Clear();
            foreach (MyInfluenceSphereSound sphereSound in m_soundSpheresSAP)
            {
                m_spheresIterationHelper.Add(sphereSound);
            }
            foreach (MyInfluenceSphereDust sphereDust in m_dustSpheresSAP)
            {
                m_spheresIterationHelper.Add(sphereDust);
            }
        }

        public List<MyInfluenceSphereBase> GetInfluenceSpheres()
        {
            PrepareSpheresIterationHelper();

            return m_spheresIterationHelper;
        }

        public int GetInfluenceSpheresCount()
        {
            return m_spheresIterationHelper.Count;
        }

        public void CloseAll()
        {
            PrepareSpheresIterationHelper();

            foreach (var item in m_spheresIterationHelper)
            {
                item.Close();
            }
        }
    }
}

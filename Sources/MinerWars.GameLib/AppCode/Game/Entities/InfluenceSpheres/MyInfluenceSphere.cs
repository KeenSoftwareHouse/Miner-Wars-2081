using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.World;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Helpers;
using MinerWars.AppCode.Physics.Collisions;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.Entities.InfluenceSpheres
{
    /// <summary>
    /// Abstract class for all influence areas (spheres and boxes).
    /// </summary>
    class MyInfluenceSphere : MyEntity
    {
        /// <summary>
        /// Flags describing which effects if this influence sphere are active.
        /// </summary>
        public MyInfluenceFlags InfluenceFlags;

        #region Radius

        /// <summary>
        /// Entities closer to the center of the influence sphere than this value
        /// will be influenced in full strength.
        /// </summary>
        public float RadiusMin
        {
            get { return m_radiusMin; }
            set
            {
                Debug.Assert(value <= m_radiusMax);
                m_radiusMin = value;
            }
        }

        private float m_radiusMin;

        /// <summary>
        /// Entities closer to the center of the influence sphere than this value
        /// will be influenced. If the entity is between this and RadiusMin, then the
        /// strength of the influence is linearly interpolated.
        /// </summary>
        public float RadiusMax
        {
            get { return m_radiusMax; }
            set
            {
                Debug.Assert(value >= m_radiusMin);
                m_radiusMax = value;
                m_radiusMaxSquared = m_radiusMax * m_radiusMax;
                this.LocalAABB = BoundingBox.CreateFromSphere(new BoundingSphere(Vector3.Zero, m_radiusMax));
             //   this.UpdateRenderobject(this.Visible);
            }
        }
        private float m_radiusMaxSquared;

        private float m_radiusMax;

        #endregion

        private Color m_dustColor;
        public Color DustColor
        {
            set { m_dustColor = value; }
            get { return m_dustColor; Refresh(); }
        }

        /// <summary>
        /// The magnitude of the radioactivity.
        /// </summary>
        public float Magnitude;

        #region Sound stuff

        private MySoundCuesEnum m_cueEnum;
        protected MySoundCue? m_cue;
        private float m_volume;
        private float m_influenceStrengthCamera;
        private float m_influenceStrengthShip;

        //protected MyInfluenceSphereSoundsEnum m_soundType;        

        protected float Volume
        {
            get { return m_volume; }
            set { m_volume = value; Refresh(); }
        }

        #endregion

        public override bool Enabled
        {
            set
            {
                if (Enabled != value)
                {
                    if (!value)
                    {
                        DisableEffects();
                    }
                }

                base.Enabled = value;
                NeedsUpdate = value;
            }
        }

        private void Refresh()
        {
            m_influenceStrengthCamera = m_influenceStrengthShip = -1;
        }

        void DisableEffects()
        {
            RestoreSectorDust();

            if (m_cue.HasValue)
            {
                m_cue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
            }
        }

        #region Influence types

        public bool IsDust
        {
            get
            {
                return (InfluenceFlags & MyInfluenceFlags.Dust) != 0;
            }
            set
            {
                if (value)
                {
                    InfluenceFlags |= MyInfluenceFlags.Dust;
                }
                else
                {
                    InfluenceFlags &= (~MyInfluenceFlags.Dust);
                }

                OnFlagChanged();
            }
        }

        public bool IsRadioactivity
        {
            get
            {
                return (InfluenceFlags & MyInfluenceFlags.Radioactivity) != 0;
            }
            set
            {
                if (value)
                {
                    InfluenceFlags |= MyInfluenceFlags.Radioactivity;
                }
                else
                {
                    InfluenceFlags &= (~MyInfluenceFlags.Radioactivity);
                }

                OnFlagChanged();
            }
        }

        public bool IsSound
        {
            get
            {
                return (InfluenceFlags & MyInfluenceFlags.Sound) != 0;
            }
            set
            {
                if (value)
                {
                    InfluenceFlags |= MyInfluenceFlags.Sound;
                }
                else
                {
                    InfluenceFlags &= (~MyInfluenceFlags.Sound);
                }

                OnFlagChanged();
            }
        }

        #endregion

        #region Init

        public static void Init()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.InfluenceSpheres, "Influence spheres", DrawInGame, MyRenderStage.DebugDraw, false);
        }

        public void Init(StringBuilder hudLabelText, MyMwcObjectBuilder_InfluenceSphere objectBuilder, Matrix matrix)
        {
            // Influence spheres dont have hud label text
            base.Init(hudLabelText, objectBuilder);

            InfluenceFlags = objectBuilder.InfluenceFlags;

            m_radiusMin = objectBuilder.RadiusMin;
            RadiusMax = objectBuilder.RadiusMax;
            DustColor = objectBuilder.DustColor;
            Magnitude = objectBuilder.Magnitude;

            VisibleInGame = false;

            SetWorldMatrix(matrix);

            Enabled = objectBuilder.Enabled;

            NeedsUpdate = Enabled;
            Save = true;

            //m_soundType = objectBuilder.SoundType;
            //MySoundCuesEnum cueEnum = GetSoundCueByObjectBuilderType(m_soundType);
            //m_cueEnum = cueEnum;
            m_cueEnum = (MySoundCuesEnum)objectBuilder.SoundCueId;

            if ((InfluenceFlags & MyInfluenceFlags.Sound) > 0)
            {
                MyGuiInfluenceSphereHelper musicHelper = MyGuiInfluenceSphereHelpers.GetInfluenceSphereSoundHelper(m_cueEnum);
                if (musicHelper == null || !musicHelper.Description.ToString().ToLower().StartsWith("amb2d_"))
                {
                    if (musicHelper == null)
                    {
                        Debug.Fail("Incorrect influence sphere sound doesn't exist");
                        MyMwcLog.WriteLine("Incorrect influence sphere sound doesn't exist");
                    }
                    else
                    {
                        Debug.Fail("Incorrect influence sphere sound: " + musicHelper.Description.ToString() + ", deleted");
                        MyMwcLog.WriteLine("Incorrect influence sphere sound: " + musicHelper.Description.ToString() + ", deleted");
                    }
                }
            }

            Flags |= EntityFlags.EditableInEditor;
        }
        #endregion

        private float CalculateInfluenceStrength(Vector3 position)
        {
            float distanceFromCenterSquared = Vector3.DistanceSquared(position, this.GetPosition());

            if (distanceFromCenterSquared > m_radiusMaxSquared)
                return 0;

            float distanceFromCenter = (float)Math.Sqrt(distanceFromCenterSquared);

            float distanceFromInnerRadius = distanceFromCenter - RadiusMin;
            if (distanceFromInnerRadius < 0)
            {
                return 1;
            }
            else
            {
                float distanceFromOuterRadius = RadiusMax - distanceFromCenter;

                float interpolationAreaLength = RadiusMax - RadiusMin;

                return MathHelper.Clamp(distanceFromOuterRadius / interpolationAreaLength, 0, 1);
            }
        }


        public override bool Draw(MyRenderObject renderObject)
        {
            bool drawDebug = MyRender.IsModuleEnabled(MyRenderStage.DebugDraw, MyRenderModuleEnum.InfluenceSpheres);
            var isEditorOrDebugDraw = MyGuiScreenGamePlay.Static.IsEditorActive() || drawDebug;

            if (isEditorOrDebugDraw && MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0 && !MyEntities.IsTypeHidden(typeof(MyInfluenceSphere)))
            {
                Matrix worldMatrix = WorldMatrix;

                var innerColor = Color.White.ToVector4();
                var innerSphere = new BoundingSphere(worldMatrix.Translation, RadiusMin);
                MySimpleObjectDraw.DrawTransparentSphere(ref worldMatrix, innerSphere.Radius, ref innerColor, true, 12,
                                                         MyTransparentMaterialEnum.ObjectiveDummyFace,
                                                         MyTransparentMaterialEnum.ObjectiveDummyLine);

                var outerColor = 0.5f * innerColor;
                var outerSphere = new BoundingSphere(worldMatrix.Translation, RadiusMax);
                MySimpleObjectDraw.DrawTransparentSphere(ref worldMatrix, outerSphere.Radius, ref outerColor, true, 12,
                                                         MyTransparentMaterialEnum.ObjectiveDummyFace,
                                                         MyTransparentMaterialEnum.ObjectiveDummyLine);
            }

            return true;
        }

        public override void DebugDrawDeactivated()
        {
            if (Activated || !Visible)
            {
                return;
            }

            MyDebugDraw.DrawSphereWireframe(Matrix.CreateScale(RadiusMax) * WorldMatrix, Color.Green.ToVector3(), 1f);
        }

        private static void DrawInGame()
        {
            // dummy method
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            var objectBuilder = (MyMwcObjectBuilder_InfluenceSphere)base.GetObjectBuilderInternal(getExactCopy);

            objectBuilder.InfluenceFlags = InfluenceFlags;

            objectBuilder.RadiusMin = RadiusMin;
            objectBuilder.RadiusMax = RadiusMax;
            objectBuilder.Magnitude = Magnitude;
            objectBuilder.DustColor = DustColor;
            //objectBuilder.SoundType = m_soundType;
            objectBuilder.SoundCueId = (short)m_cueEnum;

            return objectBuilder;
        }

        #region Intersections

        public override bool GetIntersectionWithBoundingFrustum(ref BoundingFrustum boundingFrustum)
        {
            ContainmentType con = boundingFrustum.Contains(WorldAABB);
            return con == ContainmentType.Contains || con == ContainmentType.Intersects;
        }

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            v = null;
            Ray ray = new Ray(line.From, line.Direction);
            float? ds = ray.Intersects(new BoundingSphere(WorldMatrix.Translation, RadiusMax));
            if (ds == null)
                return false;
            v = line.From + line.Direction * ds;
            return true;
        }

        public override bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            var boundingSphere = new BoundingSphere(WorldMatrix.Translation, RadiusMax);
            return boundingSphere.Intersects(sphere);
        }

        #endregion

        private void ChangeDustColor(float influenceStrength)
        {
            Debug.Assert(IsDust);

            float dust = influenceStrength;

            if (influenceStrength <= 0.001f)
            {
                MyParticlesDustField.CustomColor = null;
            }
            else
            {
                Color interpolatedColor = Color.Lerp(
                    MySector.ParticleDustProperties.Color,
                    DustColor,
                    influenceStrength);

                MyParticlesDustField.CustomColor = interpolatedColor;
            }
        }

        protected void ChangeSound(float influenceStrength)
        {
            Debug.Assert(IsSound);
            Volume = influenceStrength * (MyFakes.ENABLE_DEBUG_INFLUENCE_SPHERES_SOUNDS ? m_maxVolume : 100.0f);
            if (m_volume <= 0.0f)
            {
                if (m_cue.HasValue)
                {
                    m_cue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
                }
            }
            else
            {
                if ((m_cue.HasValue == false) || (m_cue.Value.IsPlaying == false))
                {
                    m_cue = MyAudio.AddCue2D(m_cueEnum, m_volume);
                }

                MyAudio.UpdateCueAmbVolume(m_cue, m_volume);
            }

        }
        protected void ChangeRadioactivity(float influenceStrength)
        {
            Debug.Assert(IsRadioactivity);
            if (MyGuiScreenGamePlay.Static.ControlledEntity == MySession.PlayerShip)
            {
                MySession.PlayerShip.RadioactivityAmount = Magnitude * CalculateInfluenceStrength(MySession.PlayerShip.GetPosition());
            }
        }
    


        protected void OnFlagChanged()
        {
            if (!IsDust)
            {
                RestoreSectorDust();
            }

            if (!IsSound)
            {
                Volume = 0;

                if (m_cue.HasValue)
                {
                    m_cue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
            }
        }

        public override void Close()
        {
            DisableEffects();

            base.Close();
        }

        private static void RestoreSectorDust()
        {
            MyParticlesDustField.CustomColor = null;
        }

        public void ChangeCueEnum(MySoundCuesEnum soundCue)
        {
            m_cueEnum = soundCue;
        }

        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            if (IsDust || IsSound)
            {
                float influence = CalculateInfluenceStrength(MyCamera.Position);
                if (m_influenceStrengthCamera != influence)
                {
                    if (IsDust)
                        ChangeDustColor(influence);
                    if (IsSound)
                        ChangeSound(influence);
                    m_influenceStrengthCamera = influence;
                }

                #region DEBUGING
                if (MyFakes.ENABLE_DEBUG_INFLUENCE_SPHERES_SOUNDS && IsSound) 
                {
                    if (influence > 0f)
                    {
                        if (influence > m_nearestInfluenceSphereStrength)
                        {
                            m_nearestInfluenceSphere = this;
                            if (m_notification == null) 
                            {
                                m_notification = new MyHudNotification.MyNotification(m_nearestInfluenceSphere.m_cueEnum.ToString() + ":" + (int)m_maxVolume, MyGuiManager.GetFontMinerWarsBlue());
                                MyHudNotification.AddNotification(m_notification);
                            }                            
                        }
                    }
                    else 
                    {
                        if (m_nearestInfluenceSphere == this) 
                        {
                            m_nearestInfluenceSphere = null;
                            m_nearestInfluenceSphereStrength = 0f;
                            if (m_notification != null) 
                            {
                                m_notification.Disappear();
                                m_notification = null;
                            }
                        }
                    }
                }
                #endregion
            }

            if (IsRadioactivity)
            {
                float influence = CalculateInfluenceStrength(MySession.PlayerShip.GetPosition());
                if (m_influenceStrengthShip != influence)
                {
                    ChangeRadioactivity(influence);
                    m_influenceStrengthShip = influence;
                }
            }
        }

        #region DEBUGING
        private static MyInfluenceSphere m_nearestInfluenceSphere = null;
        private static float m_nearestInfluenceSphereStrength = 0f;
        private static List<MySoundCuesEnum> m_sounds;
        private static MyHudNotification.MyNotification m_notification = null;
        private static float m_maxVolume = 100f;

        public static void UpdateMaxVolume(bool forward) 
        {
            float increment = forward ? 10f : -10f;
            m_maxVolume = MathHelper.Clamp(m_maxVolume + increment, 0f, 100f);
            if (m_nearestInfluenceSphere != null)
            {
                m_nearestInfluenceSphere.ChangeSound(m_nearestInfluenceSphereStrength);
            }
            if (m_notification != null)
            {
                m_notification.Disappear();
                m_notification = null;
            }
        }

        public static void SwitchToNextSound(bool forward) 
        {
            if (m_nearestInfluenceSphere != null) 
            {
                if (m_sounds == null) 
                {
                    LoadSounds();                    
                }
                int currentIndex = m_sounds.IndexOf(m_nearestInfluenceSphere.m_cueEnum);
                if (forward)
                {
                    currentIndex++;
                    if (currentIndex >= m_sounds.Count)
                    {
                        currentIndex = 0;
                    }
                }
                else 
                {
                    currentIndex--;
                    if (currentIndex < 0) 
                    {
                        currentIndex = m_sounds.Count - 1;
                    }
                }
                m_nearestInfluenceSphere.m_cueEnum = m_sounds[currentIndex];
                if (m_nearestInfluenceSphere.m_cue.HasValue) 
                {
                    m_nearestInfluenceSphere.m_cue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                }
                m_nearestInfluenceSphere.ChangeSound(m_nearestInfluenceSphereStrength);
                if (m_notification != null) 
                {
                    m_notification.Disappear();
                    m_notification = null;
                }
            }
        }

        private static void LoadSounds() 
        {
            m_sounds = new List<MySoundCuesEnum>();
            foreach (MySoundCuesEnum enumValue in MyGuiInfluenceSphereHelpers.MyInfluenceSphereSoundHelperTypesEnumValues)
            {
                MyGuiInfluenceSphereHelper musicHelper = MyGuiInfluenceSphereHelpers.GetInfluenceSphereSoundHelper(enumValue);
                if (musicHelper != null && musicHelper.Description.ToString().ToLower().StartsWith("amb2d_"))
                {
                    m_sounds.Add(enumValue);
                }
            }
        }
        #endregion
    }
}

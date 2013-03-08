#region Using

using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities.WayPoints;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Physics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.EntityDetector;
using System.Diagnostics;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.HUD;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Physics.Collisions;
using MinerWars.AppCode.Game.World.Global;

#endregion


namespace MinerWars.AppCode.Game.Entities
{
    class MyDummyPoint : MyEntity
    {
        #region Members

        MyDummyPointFlags m_dummyFlags;
        MyDummyPointType m_type;
        Vector3 m_size;
        Vector4 m_color;
        float m_argument;

        MyDummyEffectHelper m_effectHelper;
        MySoundCue? m_cue;

        BoundingBox m_particleAABB = MyMath.CreateInvalidAABB();

        /// <summary>
        /// This is only used internally for gameplay properties of the particle effect.
        /// </summary>
        private MyEntityDetector m_smallShipDetector;

        public MyTransparentMaterialEnum? LineMaterial = null;
        public MyTransparentMaterialEnum? FaceMaterial = null;

        public uint Tag;

        #endregion

        #region Init


        public MyDummyPoint()
            : base(true)
        {
        }

        public virtual void Init(string hudLabelText, MyMwcObjectBuilder_DummyPoint objectBuilder, Matrix matrix)
        {
            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);
            base.Init(hudLabelTextSb, objectBuilder);

            Type = objectBuilder.Type;
            Size = objectBuilder.Size;

            Flags |= EntityFlags.EditableInEditor;

            DummyFlags = objectBuilder.DummyFlags;
            CastShadows = false;

            if (Type == MyDummyPointType.Sphere)
                Radius = Size.X / 2.0f;

            SetWorldMatrix(matrix);

            Color = objectBuilder.Color.ToVector4();
            Argument = objectBuilder.Argument;

            RespawnPointFaction = objectBuilder.RespawnPointFaction;

            RefreshParticleEffect();

            foreach (MyRenderObject ro in RenderObjects)
            {
                ro.SkipIfTooSmall = false;
            }

            StartEffect();
        }

        #endregion

        public void StartEffect()
        {
            if ((DummyFlags & MyDummyPointFlags.PARTICLE) != 0)
            {
                ParticleEffect = MyParticlesManager.CreateParticleEffect((int)ParticleID, true);
                ParticleEffect.WorldMatrix = WorldMatrix;
                ParticleEffect.UserRadiusMultiplier = 1.0f;
                ParticleEffect.UserScale = UserScale;

                RefreshParticleSound();
            }
        }

        void StartParticleSound()
        {
            var effectShouldEmitSound = Enabled && m_effectHelper != null && m_effectHelper.SoundCueEnum.HasValue;
            var effectCurrentlyEmittingSound = m_cue.HasValue && m_cue.Value.IsPlaying;
            if (effectShouldEmitSound && !effectCurrentlyEmittingSound)
            {
                m_cue = MyAudio.AddCue3D(
                    m_effectHelper.SoundCueEnum.Value,
                    WorldMatrix.Translation,
                    WorldMatrix.Forward,
                    WorldMatrix.Up,
                    Vector3.Zero);
            }
        }

        public void StopEffect()
        {
            if (ParticleEffect != null)
            {
                MyParticlesManager.RemoveParticleEffect(ParticleEffect);
                ParticleEffect = null;
            }

            RefreshParticleSound();
        }

        void StopParticleSound()
        {
            if (m_cue.HasValue)
            {
                m_cue.Value.Stop(SharpDX.XACT3.StopFlags.Release);
            }
        }

        public void RestartEffect()
        {
            StopEffect();
            StartEffect();
        }

        public override void UpdateBeforeSimulation()
        {
            //We dont want to update every frame because of this
            //Visible = IsDummyVisible();

            base.UpdateBeforeSimulation();

            if ((DummyFlags & MyDummyPointFlags.PARTICLE) != 0 && 
                ParticleEffect == null && 
                //MyGuiScreenGamePlay.Static.IsGameActive() && 
                (PersistentFlags & MyPersistentEntityFlags.Enabled) > 0)
            {
                StartEffect();
            }

            if (ParticleEffect != null &&
                //((MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) || 
                (DummyFlags & MyDummyPointFlags.PARTICLE) == 0)
            {
                StopEffect();
            }

            if (m_cue != null && m_cue.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(m_cue, WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero);
            }

            if (DoesParticleEffectNeedDetector())
            {
                RefreshParticleSound();

                if (m_smallShipDetector == null)
                {
                    RefreshParticleEffect();
                }

                foreach (var detectedEntityPair in m_smallShipDetector.GetDetectedEntities())
                {
                    var detectedSmallShip = (MySmallShip)detectedEntityPair.Key;

                    float distance = Vector3.Distance(detectedSmallShip.GetPosition(), this.GetPosition());
                    float influenceStrength = MathHelper.Clamp((m_smallShipDetector.Radius - distance) / m_smallShipDetector.Radius, 0f, 1f);

                    if (m_effectHelper.DamageStrength.HasValue)
                    {
                        DamageShip(detectedSmallShip, influenceStrength);
                    }

                    if (m_effectHelper.DirectionalPushStrength.HasValue)
                    {
                        PushShip(detectedSmallShip, distance, influenceStrength);
                    }
                }
            }
        }

        void RefreshParticleSound()
        {
            if (DummyIsTooFarAway())
            {
                StopParticleSound();
            }
            else
            {
                StartParticleSound();
            }
        }

        bool DummyIsTooFarAway()
        {
            return Vector3.DistanceSquared(MyCamera.Position, this.GetPosition()) > MyDummyPointConstants.MAX_SOUND_DISTANCE_SQUARED;
        }

        void DamageShip(MySmallShip detectedEntity, float influenceStrength)
        {
            Debug.Assert(m_effectHelper.DamageStrength != null);
            var damageAmount = m_effectHelper.DamageStrength.Value * influenceStrength;

            detectedEntity.DoDamage(0, damageAmount * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS, 0, MyDamageType.Unknown, MyAmmoType.None, this);
        }

        void PushShip(MySmallShip detectedEntity, float distance, float influenceStrength)
        {
            var direction = detectedEntity.GetPosition() - this.GetPosition();
            var directionNormalized = (1 / distance) * direction;
            float alignment = Vector3.Dot(directionNormalized, WorldMatrix.Up);

            alignment = MathHelper.Clamp(alignment, 0, 1);
            alignment = alignment * alignment * alignment;

            Debug.Assert(m_effectHelper.DirectionalPushStrength != null);
            detectedEntity.Physics.AddForce(
                MyPhysicsForceType.APPLY_WORLD_IMPULSE_AND_WORLD_ANGULAR_IMPULSE,
                MyDummyPointConstants.PUSH_STRENGTH * m_effectHelper.DirectionalPushStrength.Value *
                alignment * influenceStrength * directionNormalized,
                detectedEntity.GetPosition(),
                Vector3.Zero);

            detectedEntity.IncreaseHeadShake(MyHeadShakeConstants.HEAD_SHAKE_AMOUNT_AFTER_SMOKE_PUSH * alignment * influenceStrength);
        }

        bool IsDummyVisible()
        {
            if (!IsVisible()) return false;

            switch (DummyFlags)
            {
                case MyDummyPointFlags.SAFE_AREA: if (MyEntities.SafeAreasHidden) return false; break;
                case MyDummyPointFlags.DETECTOR: if (MyEntities.DetectorsHidden) return false; break;
                case MyDummyPointFlags.PARTICLE: if (MyEntities.ParticleEffectsHidden) return false; break;
                case MyDummyPointFlags.VOXEL_HAND: return true; break;
                default: break;
            }

            return ((MyGuiScreenGamePlay.Static.IsEditorActive() && !MyGuiScreenGamePlay.Static.IsIngameEditorActive()) ||
                (MyGuiScreenGamePlay.Static.IsGameActive() && (DummyFlags & MyDummyPointFlags.COLOR_AREA) > 0));
        }

        public override bool IsSelectable()
        {
            switch (DummyFlags)
            {
                case MyDummyPointFlags.SAFE_AREA: if (!MyEntities.SafeAreasSelectable) return false; break;
                case MyDummyPointFlags.DETECTOR: if (!MyEntities.DetectorsSelectable) return false; break;
                case MyDummyPointFlags.PARTICLE: if (!MyEntities.ParticleEffectsSelectable) return false; break;
                default: break;
            }

            return base.IsSelectable();
        }

        public bool CanSurvivePrefabDestruction()
        {
            return (DummyFlags & MyDummyPointFlags.SURIVE_PREFAB_DESTRUCTION) > 0;
        }

        public override void UpdateHudMarker(bool enableReset = false)
        {
            if (MyMissions.IsMissionEntity(this))
            {
                base.SetHudMarker();
            }
            else 
            {
                MyHud.RemoveText(this);
            }
        }

        #region Draw

        

        public override bool Draw(MyRenderObject renderObject)
        {
            if (Render.MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0)
            {
                if (IsDummyVisible())
                {
                    base.Draw(renderObject);

                    Vector4 color;
                    switch (DummyFlags)
                    {
                        case MyDummyPointFlags.NONE: color = new Vector4(0.5f, 0.7f, 0.1f, 0.3f); break;
                        case MyDummyPointFlags.SAFE_AREA: color = new Vector4(0.2f, 0.3f, 0.22f, 0.1f); break;
                        case MyDummyPointFlags.DETECTOR: color = new Vector4(0.12f, 0.1f, 0.7f, 0.3f); break;
                        case MyDummyPointFlags.PARTICLE: color = new Vector4(0.6f, 0.05f, 0.1f, 0.3f); break;
                        default: color = new Vector4(0.6f, 0.6f, 0.7f, 0.3f); break;
                    }
                    if ((DummyFlags & MyDummyPointFlags.COLOR_AREA) != 0)
                        color = Color;  // color Color areas with their area color (overriding the default color). I like to write "color".

                    Matrix worldMatrix = WorldMatrix;

                    if ((int)(DummyFlags & MyDummyPointFlags.TEXTURE_QUAD) > 0)
                    {
                        BoundingBox localAABB = LocalAABB;
                        MySimpleObjectDraw.DrawWireFramedBox(ref worldMatrix, ref localAABB, ref color, 0.01f, 1, LineMaterial);

                        if (!string.IsNullOrEmpty(Name))
                        {
                            //var tex = MinerWars.AppCode.Game.Textures.MyTextureManager.GetTexture<MinerWars.AppCode.Game.Textures.MyTexture2D>(Name);
                            int i = 0;
                            foreach (MyTransparentMaterialEnum trEnum in Enum.GetValues(typeof(MyTransparentMaterialEnum)))
                            {
                                if (MyTransparentMaterialConstants.MyTransparentMaterialStrings[i] == Name)
                                {
                                    Vector4 quadColor = Vector4.One;
                                    MyQuad quad;
                                    Vector3 position = GetPosition();
                                    Vector3 zeroPosition = Vector3.Zero;

                                    var texture = MyTransparentGeometry.GetTexture(trEnum);
                                    float ratio = texture.Height / (float)texture.Width;

                                    MyUtils.GenerateQuad(out quad, ref position, WorldAABB.Size().X * ratio, WorldAABB.Size().X, ref worldMatrix);
                                    MyTransparentGeometry.AddQuad(trEnum, ref quad, ref quadColor, ref position); 
                                }
                                i++;
                            }
                        }
                    }
                    else
                    {
                        if (Type == MyDummyPointType.Box)
                        {
                            BoundingBox localAABB = LocalAABB;
                            MySimpleObjectDraw.DrawTransparentBox(ref worldMatrix, ref localAABB, ref color, true, 1, FaceMaterial, LineMaterial);
                        }
                        else
                        {
                            BoundingSphere localSphere = new BoundingSphere(worldMatrix.Translation, Radius);
                            MySimpleObjectDraw.DrawTransparentSphere(ref worldMatrix, localSphere.Radius, ref color, true, 12, FaceMaterial, LineMaterial);
                        }
                    }
                }

                if (ParticleEffect != null && IsVisible() && Enabled)
                {
                    Vector4 particleColor = Color == Vector4.Zero ? Vector4.One : Color;
                    ParticleEffect.UserColorMultiplier = particleColor;
                    ParticleEffect.UserScale = UserScale;

                    UpdateWorldVolume();

                    MyParticlesManager.CustomDraw(ParticleEffect);
                }
            }

            return false;
        }

        protected override void UpdateWorldVolume()
        {
            base.UpdateWorldVolume();
                                
            if (ParticleEffect != null && IsVisible() && Enabled)
            {
                BoundingBox newAABB = BoundingBox.CreateMerged(WorldAABB, ParticleEffect.GetAABB());
                if (m_particleAABB.Contains(newAABB) != ContainmentType.Contains)
                {
                    m_particleAABB = BoundingBox.CreateMerged(m_particleAABB, newAABB);
                    m_worldAABB = m_particleAABB;
                    InvalidateRenderObjects(true);
                }
            }                    
        }

        public override bool DebugDraw()
        {
            Vector4 color = Vector4.One;
            Vector3 color2 = Vector3.One;
            if (ParticleEffect != null && IsVisible() && Enabled)
            {
                
                //MyDebugDraw.DrawAABB(ref m_worldAABB, ref color, 1);
            }

            if (m_smallShipDetector != null)
            {
                MyDebugDraw.DrawSphereWireframe(GetPosition(), m_smallShipDetector.Radius, color2, 1);
            }

            return base.DebugDraw();
        }

        public override void DebugDrawDeactivated()
        {
            if (Activated || !Visible)
            {
                return;
            }

            Vector3 colorToDraw = MinerWarsMath.Color.Green.ToVector3();
            float alpha = 1f;
            if (Type == MyDummyPointType.Box)
            {                                
                MyDebugDraw.DrawHiresBoxWireframe(Matrix.CreateScale(LocalAABB.Size()) * WorldMatrix, colorToDraw, alpha);                
            }
            else
            {
                MyDebugDraw.DrawSphereWireframe(Matrix.CreateScale(Radius) * WorldMatrix, colorToDraw, alpha);
            }
        }

        #endregion

        public override bool GetIntersectionWithLine(ref MyLine line, out Vector3? v, bool useCollisionModel = true, IntersectionFlags flags = IntersectionFlags.ALL_TRIANGLES)
        {
            v = null;
            Ray ray = new Ray(line.From, line.Direction);
            float? ds = null;
            if (Type == MyDummyPointType.Box)
            {
                ds = ray.Intersects(this.WorldAABB);
            }
            else if (Type == MyDummyPointType.Sphere)
            {
                ds = ray.Intersects(new BoundingSphere(WorldMatrix.Translation, Radius));
            }
            
            if (ds == null)
                return false;
            v = line.From + line.Direction * ds;
            return true;
        }

        public override bool GetIntersectionWithSphere(ref BoundingSphere sphere)
        {
            if (Type == MyDummyPointType.Box)
            {
                BoundingBox boundingBox = new BoundingBox(WorldMatrix.Translation - Size, WorldMatrix.Translation + Size);
                return boundingBox.Intersects(sphere);
            }
            else if (Type == MyDummyPointType.Sphere)
            {
                BoundingSphere boundingSphere = new BoundingSphere(WorldMatrix.Translation, Radius);
                return boundingSphere.Intersects(sphere);
            }
            
            return false;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_DummyPoint objectBuilder = (MyMwcObjectBuilder_DummyPoint)base.GetObjectBuilderInternal(getExactCopy);

            objectBuilder.Type = Type;
            if (Type == MyDummyPointType.Box)
            {
                objectBuilder.Size = Size;
            }
            else
            {
                objectBuilder.Size = new Vector3(Radius * 2, 1f, 1f);
            }
            objectBuilder.DummyFlags = DummyFlags;
            objectBuilder.Color = new Color(Color);
            objectBuilder.Argument = Argument;
            objectBuilder.RespawnPointFaction = RespawnPointFaction;

            return objectBuilder;
        }

        public MyEntityDetector GetDetector()
        {
            return ((DummyFlags & MyDummyPointFlags.DETECTOR) > 0 && Children.Count == 1) ? FindChild(0) as MyEntityDetector : null;
        }

        public override void OnWorldPositionChanged(object source)
        {
            base.OnWorldPositionChanged(source);
            if (ParticleEffect != null)
            {
                ParticleEffect.WorldMatrix = this.WorldMatrix;
            }

            if (m_cue != null && m_cue.Value.IsPlaying)
            {
                MyAudio.UpdateCuePosition(m_cue, WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up, Vector3.Zero);
            }
        }

        #region Properties

        public MyDummyPointType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
                if (value == MyDummyPointType.Box)
                    Size = m_size;
                else
                    Radius = m_size.X / 2.0f;
            }
        }

        public Vector3 Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
                LocalAABB = new BoundingBox(-m_size / 2.0f, m_size / 2.0f);
            }
        }

        public float Radius
        {
            get
            {
                return m_size.X / 2.0f;
            }
            set
            {
                m_size = new Vector3(value * 2.0f, value * 2.0f, value * 2.0f);
                LocalVolume = new BoundingSphere(Vector3.Zero, value);
                m_localAABB = BoundingBox.CreateFromSphere(LocalVolume);
            }
        }

        public MyDummyPointFlags DummyFlags
        {
            get
            {
                return m_dummyFlags;
            }
            set
            {
                if (m_dummyFlags != value)
                {
                    m_dummyFlags = value;

                    if ((DummyFlags & MyDummyPointFlags.PARTICLE) != 0)
                    {
                        NeedsUpdate = true;
                        RefreshParticleEffect();
                    }

                    if ((DummyFlags & MyDummyPointFlags.DETECTOR) > 0)
                    {
                        MyEntityDetector detector = new MyEntityDetector();

                        if (Type == MyDummyPointType.Sphere)
                        {
                            detector.Init(null, new MyMwcObjectBuilder_EntityDetector(new Vector3(Radius * 2f, 0f, 0f), MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere), this, WorldMatrix, new List<IMyEntityDetectorCriterium>());
                        }
                        else
                        {
                            detector.Init(null, new MyMwcObjectBuilder_EntityDetector(Size, MyMwcObjectBuilder_EntityDetector_TypesEnum.Box), this, WorldMatrix, new List<IMyEntityDetectorCriterium>());
                        }

                        detector.Save = false;
                        detector.SetSensorDetectRigidBodyTypes(MyConstants.RIGIDBODY_TYPE_SHIP);
                    }

                    if ((DummyFlags & MyDummyPointFlags.NOTE) > 0)
                    {
                        MyHud.AddText(this, new StringBuilder(Name), MyGuitargetMode.Neutral, 0, MyHudIndicatorFlagsEnum.SHOW_TEXT | MyHudIndicatorFlagsEnum.SHOW_DISTANCE, null, null);
                    }
                    else
                    {
                        MyHud.RemoveText(this);
                    }
                }
            }
        }

        private bool DoesParticleEffectNeedDetector()
        {
            return (DummyFlags & MyDummyPointFlags.PARTICLE) != 0 && m_effectHelper != null &&
                   (m_effectHelper.DamageStrength.HasValue || m_effectHelper.DirectionalPushStrength.HasValue) && this.Enabled;
        }

        public float UserScale
        {
            get
            {
                float arg = Argument;
                bool minus = arg < 0;
                if (minus) arg *= -1;
                float fraction = (arg - (float)Math.Floor(arg));
                return ((minus ? fraction : 1 + ( 3 *fraction)));
            }
            set
            {
                System.Diagnostics.Debug.Assert(value > 0.0f && value < 4.0f);

                Argument = Math.Abs((int)(Argument));

                bool minus = false;

                if (value < 1)
                {
                    minus = true;
                }
                else
                    value = (value - 1) / 3;

                value = Math.Abs(value);

                Argument = (Math.Abs(Argument) + value) * (minus ? -1 : 1); 
            }
        }

        public float ParticleID
        {
            get
            {
                return (int)Math.Floor(Math.Abs(Argument));
            }
            set
            {
                bool minus = Argument < 0;
                float frac = Math.Abs(Argument) - (int)Math.Floor(Math.Abs(Argument));
                if (frac < 0) frac *= -1;
                Argument = value;

                Argument += frac;
                if (minus)
                    Argument *= -1;

                RefreshParticleEffect();

                RestartEffect();
            }
        }

        public MyMwcObjectBuilder_FactionEnum RespawnPointFaction { get; set; }

        void RefreshParticleEffect()
        {
            m_effectHelper = MyDummyEffectHelpers.Get((MyParticleEffectsIDEnum) ParticleID);

            if (DoesParticleEffectNeedDetector())
            {
                CreateDetectorForParticleEffect();
            }
        }

        void CreateDetectorForParticleEffect()
        {
            m_smallShipDetector = new MyEntityDetector();

            m_smallShipDetector.Init(
                null,
                new MyMwcObjectBuilder_EntityDetector(
                    new Vector3(MyDummyPointConstants.PARTICLE_DETECTOR_SIZE * UserScale, 0, 0),
                    MyMwcObjectBuilder_EntityDetector_TypesEnum.Sphere),
                this,
                WorldMatrix,
                new List<IMyEntityDetectorCriterium>());

            m_smallShipDetector.Save = false;
            m_smallShipDetector.SetSensorDetectRigidBodyTypes(MyConstants.RIGIDBODY_TYPE_SHIP);

            m_smallShipDetector.On();
        }


        public Vector4 Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        public float Argument
        {
            get { return m_argument; }
            set { m_argument = value; }
        }

        public MyParticleEffect ParticleEffect { get; private set; }

        public string SecretRoomName
        {
            get 
            {
                if (DummyFlags == MyDummyPointFlags.DETECTOR && MySecretRooms.SecretRooms.ContainsKey((int)Argument))
                {
                    return MySecretRooms.SecretRooms[(int)Argument];
                }
                return null;
            }
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (base.Enabled != value)
                {
                    base.Enabled = value;

                    RefreshParticleSound();
                }
            }
        }

        #endregion

        public void DisableParticleEffect()
        {
            DummyFlags &= ~MyDummyPointFlags.PARTICLE;
            StopEffect();
        }

        public override void Close()
        {
            StopParticleSound();
            StopEffect();

            base.Close();
        }
    }
}

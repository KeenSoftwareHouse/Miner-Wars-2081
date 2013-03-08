using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Lights;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Physics;
using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Prefabs;
using MinerWars.AppCode.Game.Explosions;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using MinerWars.AppCode.Game.Voxels;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game.Decals;
using MinerWars.AppCode.Game.Editor;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.AppCode.Game.GUI.Core;
using System.Diagnostics;
using MinerWars.AppCode.Game.Audio;

using MinerWars.AppCode.Game.Missions;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.World.Global;

namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    #region old    
    //class MyPrefab : MyEntity
    //{
    //    class MyPrefabLamp
    //    {
    //        const float BLIC_DURATON_IN_MILISECONDS = 30.0f;

    //        public readonly Vector3 Position;
    //        public readonly float RadiusMin;
    //        public readonly float RadiusMax;
    //        public readonly int TimerForBlic;
    //        public readonly MyLight Light;

    //        public MyPrefabLamp(Vector3 position, float radiusMin, float radiusMax, int timerForBlic)
    //        {
    //            Position = position;
    //            RadiusMin = radiusMin;
    //            RadiusMax = radiusMax;
    //            TimerForBlic = timerForBlic;
    //            Light = MyLights.AddLight();
    //            Light.Start(MyLight.LightTypeEnum.PointLight, position, Vector4.One, 1, radiusMin);
    //            Light.Intensity = 1;
    //            Light.LightOn = true;
    //        }

    //        public void Draw()
    //        {
    //            Vector3 dir = MyMwcUtils.Normalize(MyCamera.Position - Position);

    //            float timeBlic = MyMinerGame.TotalGamePlayTimeInMilliseconds % TimerForBlic;
    //            if (timeBlic > BLIC_DURATON_IN_MILISECONDS) timeBlic = TimerForBlic - timeBlic;
    //            timeBlic = MathHelper.Clamp(1 - timeBlic / BLIC_DURATON_IN_MILISECONDS, 0, 1);

    //            float radius = MathHelper.Lerp(RadiusMin, RadiusMax, timeBlic);

    //            MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.ReflectorGlareAlphaBlended, Vector4.One, Position + dir * 5, radius, 0);
    //            Light.Range = radius * 4;
    //        }

    //        public void Close()
    //        {
    //            MyLights.RemoveLight(Light);
    //        }
    //    }

    //    private List<MyPrefabLamp> m_lamps;
    //    private List<MyLight> m_lights;

    //    protected MyPrefabConfiguration m_config;

    //    protected MyPrefabContainer m_owner = null;  //owner container
    //    private bool m_visible = true;
    //    private bool m_isExploded = false;

    //    public new bool IsVisible { get { return m_visible; } }
    //    public bool SetVisible { set { m_visible = value; } }
    //    //protected MyMwcObjectBuilder_Prefab_TypesEnum m_prefabTypeEnum;

    //    public Matrix LocalOrientation;
    //    public bool Modified;

    //    public List<MyPrefabSnapPoint> SnapPoints { get; set; }

    //    private Matrix[] m_smokeMatrices;
    //    private MyParticleEffect[] m_smokeEffects;        

    //    public MyPrefab(MyPrefabContainer owner)
    //    {
    //        m_owner = owner;
    //        SnapPoints = new List<MyPrefabSnapPoint>();            
    //    }

    //    public virtual void Init(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
    //    {
    //        StringBuilder displayNameSb = (displayName == null) ? null : new StringBuilder(displayName);
    //        //StringBuilder hudLabelTextSb = new StringBuilder(Enum.GetName(typeof(MyModelsEnum), prefabConfig.ModelLod0Enum));

    //        //TODO: Config.MaterialType 
    //        //m_prefabTypeEnum = prefabTypeEnum;
    //        m_config = prefabConfig;

    //        base.Init(displayNameSb, prefabConfig.ModelLod0Enum, prefabConfig.ModelLod1Enum, null, null, objectBuilder);

    //        DisplayName = objectBuilder.DisplayName;

    //        MaterialIndex = (ushort) objectBuilder.FactionAppearance;

    //        this.LocalMatrix = Matrix.CreateWorld(relativePosition, localOrientation.Forward, localOrientation.Up);

    //        //This solves bad saved values in DB   //this solves old prefabs max value
    //        if (objectBuilder.PrefabMaxHealth == 0 || objectBuilder.PrefabMaxHealth == MyGameplayConstants.MAX_HEALTH_MAX)
    //        {
    //            objectBuilder.PrefabMaxHealth = m_gameplayProperties.MaxHealth;
    //        }

    //        //This solves bad saved values in DB   //this solves old prefabs max value
    //        if (objectBuilder.PrefabHealth == 0 || objectBuilder.PrefabHealth == 500 ||
    //            objectBuilder.PrefabHealth == MyGameplayConstants.HEALTH_MAX)
    //        {
    //            objectBuilder.PrefabHealth = m_gameplayProperties.MaxHealth;
    //        }

    //        MaxHealth = objectBuilder.PrefabMaxHealth;
    //        Health = objectBuilder.PrefabHealth;

    //        // create the box
    //        MyPhysicsObjects physobj = MyPhysics.physicsSystem.GetPhysicsObjects();
    //        MyRBTriangleMeshElementDesc trianglemeshDesc = physobj.GetRBTriangleMeshElementDesc();
    //        MyMaterialType materialType = MyMaterialType.METAL;

    //        trianglemeshDesc.SetToDefault();
    //        trianglemeshDesc.m_Model = ModelLod0;
    //        trianglemeshDesc.m_RBMaterial = MyMaterialsConstants.GetMaterialProperties(materialType).PhysicsMaterial;

    //        MyRBTriangleMeshElement trEl = (MyRBTriangleMeshElement)physobj.CreateRBElement(trianglemeshDesc);


    //        // Base rigid body is used to hold static prefabs
    //        this.Physics = new PhysicsManager.Physics.MyGameRigidBody(this, 1.0f, RigidBodyFlag.RBF_RBO_STATIC) { MaterialType = materialType };
    //        this.Physics.Enabled = true;
    //        this.Physics.AddElement(trEl, true);

    //        SnapPoints = GetSnapPoints(GetModelLod0(), this);

    //        m_smokeMatrices = GetSmokePoints(GetModelLod0());
    //        m_smokeEffects = new MyParticleEffect[m_smokeMatrices.Length];
    //        Debug.Assert(m_smokeMatrices != null);
    //        Debug.Assert(m_smokeEffects != null);

    //        InitLampsAndLights();
    //    }        

    //    private void InitLampsAndLights()
    //    {            
    //        Matrix worldMatrix = Matrix.Multiply(m_owner.WorldMatrix, LocalMatrix);            

    //        Dictionary<string, MyModelDummy> dummies = MyModels.GetModelOnlyData(ModelLod0.ModelEnum).Dummies;
    //        foreach (KeyValuePair<string, MyModelDummy> dummy in dummies)
    //        {
    //            float scale = 4 * dummy.Value.Matrix.Left.Length();
    //            if (dummy.Key.StartsWith("LAMP"))
    //            {
    //                if (m_lamps == null)
    //                {
    //                    m_lamps = new List<MyPrefabLamp>();
    //                }
    //                m_lamps.Add(new MyPrefabLamp(dummy.Value.Matrix.Translation + worldMatrix.Translation, 5 * scale, 20 * scale, 980));
    //            }
    //            if (dummy.Key.Contains("LIGHT"))
    //            {
    //                if (m_lights == null)
    //                {             
    //                    m_lights = new List<MyLight>();
    //                }
    //                MyLight light = MyLights.AddLight();
    //                light.Start(MyLight.LightTypeEnum.PointLight, worldMatrix.Translation + dummy.Value.Matrix.Translation, new Vector4(0, 1, 0, 1), 2f, 25 * scale);
    //                m_lights.Add(light);
    //            }
    //        }
    //    }

    //    private static Matrix[] GetSmokePoints(MyModel model)
    //    {
    //        return (from dummy in model.Dummies
    //                where dummy.Key.StartsWith("destruction", StringComparison.InvariantCultureIgnoreCase)
    //                select dummy.Value.Matrix).ToArray();
    //    }

    //    public static List<MyPrefabSnapPoint> GetSnapPoints(MyModel model, MyPrefab prefab)
    //    {
    //        List<MyPrefabSnapPoint> snapPoints = new List<MyPrefabSnapPoint>();

    //        foreach (var dummy in model.Dummies)
    //        {
    //            if (dummy.Key.StartsWith("SNAPPOINT", StringComparison.InvariantCultureIgnoreCase))
    //            {
    //                var customData = dummy.Value.CustomData;
    //                var snapPoint = new MyPrefabSnapPoint(prefab);

    //                // Get rid of scale in rotation part
    //                snapPoint.Matrix = Matrix.CreateWorld(dummy.Value.Matrix.Translation, dummy.Value.Matrix.Forward, dummy.Value.Matrix.Up);
    //                snapPoint.SnapType = new MyPrefabSnapPoint.MyPrefabSnapPointType("OBJECT_", "", dummy.Value.CustomData);
    //                snapPoints.Add(snapPoint);

    //                string targetPostfix = "TARGET_BUILD_TYPE";
    //                foreach (var target in customData)
    //                {
    //                    if (target.Key.StartsWith(targetPostfix))
    //                    {
    //                        string postfix = target.Key.Substring(targetPostfix.Length);
    //                        snapPoint.SnapTargets.Add(new MyPrefabSnapPoint.MyPrefabSnapPointType(
    //                            "TARGET_",
    //                            postfix,
    //                            dummy.Value.CustomData));
    //                    }
    //                }
    //            }
    //        }

    //        return snapPoints;
    //    }

    //    //////////////////////////////////////////////////////////////////////////
    //    // IMyEntity INTERFACE
    //    //////////////////////////////////////////////////////////////////////////

    //    public MyPrefabContainer GetOwner()
    //    {
    //        return m_owner;
    //    }

    //    /// <summary>
    //    /// Draw
    //    /// </summary>
    //    /// <returns></returns>
    //    public override bool Draw()
    //    {
    //        if (!m_visible)
    //            return false;

    //        if (m_lamps != null)
    //        {
    //            for (int i = 0; i < m_lamps.Count; i++)
    //            {
    //                m_lamps[i].Draw();
    //            }
    //        }

    //        return base.Draw();
    //    }

    //    public override bool DebugDraw()
    //    {
    //        //// For Debug purposes
    //        //foreach (var snapPoint in SnapPoints)
    //        //{
    //        //    MyDebugDraw.DrawAxis(snapPoint.Matrix * WorldMatrix, 50, 1);
    //        //}

    //        //base.DebugDrawAABB();

    //        return base.DebugDraw();
    //    }

    //    /// <summary>
    //    /// Gets parent inverted matrix and test whether entity can move to target location
    //    /// </summary>
    //    /// <param name="moveIndicator"></param>
    //    /// <param name="parentWorldInv"></param>
    //    /// <returns></returns>
    //    private bool PrepareMove(Vector3 moveIndicator, out Matrix parentWorldInv)
    //    {
    //        MyPrefabContainer container = Parent as MyPrefabContainer;
    //        System.Diagnostics.Debug.Assert(container != null);

    //        parentWorldInv = Parent.GetWorldMatrixInverted();
    //        Matrix SubeCubeInv = parentWorldInv;

    //        Vector3 relativePosTmp = MyUtils.GetRoundedVector3(MyUtils.GetTransform(moveIndicator, ref parentWorldInv), 1);
    //        if (MyPrefabContainer.IsPrefabOutOfContainerBounds(relativePosTmp) == true)
    //            return false;

    //        return true;
    //    }

    //    public override bool MoveAndRotate(Vector3 moveIndicator, Matrix orientation)
    //    {
    //        Matrix parentWorldInv;
    //        if (PrepareMove(moveIndicator, out parentWorldInv))
    //        {
    //            Vector3 relativePos = MyUtils.GetRoundedVector3(MyUtils.GetTransform(moveIndicator, ref parentWorldInv), 1);
    //            Matrix localOrientation;
    //            Matrix.Multiply(ref orientation, ref parentWorldInv, out localOrientation);
    //            this.LocalMatrix = Matrix.CreateWorld(relativePos, localOrientation.Forward, localOrientation.Up);
    //            LocalOrientation = localOrientation;
    //            Modified = true;
    //            return true;
    //        }
    //        return false;
    //    }

    //    public override bool CanMoveAndRotate(Vector3 moveIndicator, Matrix orientation)
    //    {
    //        Matrix parentMatrixInv;
    //        return PrepareMove(moveIndicator, out parentMatrixInv);
    //    }

    //    protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
    //    {
    //        MyMwcObjectBuilder_Prefab objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_Prefab;
    //        objectBuilder.PositionInContainer = MyPrefabContainer.GetRelativePositionInContainerCoords(this.LocalMatrix.Translation);

    //        float yaw, pitch, roll;
    //        Matrix rot = this.LocalMatrix;
    //        rot.Translation = Vector3.Zero;
    //        MyUtils.RotationMatrixToYawPitchRoll(ref rot, out yaw, out pitch, out roll);
    //        objectBuilder.AnglesInContainer = new Vector3(yaw, pitch, roll);

    //        if (MaxHealth == m_gameplayProperties.MaxHealth)
    //        {
    //            objectBuilder.PrefabMaxHealth = MyGameplayConstants.MAX_HEALTH_MAX;
    //        }
    //        else
    //        {
    //            objectBuilder.PrefabMaxHealth = MaxHealth;
    //        }

    //        if (Health == m_gameplayProperties.MaxHealth)
    //            objectBuilder.PrefabHealth = MyGameplayConstants.HEALTH_MAX;
    //        else
    //            objectBuilder.PrefabHealth = Health;

    //        objectBuilder.DisplayName = DisplayName;

    //        return objectBuilder;
    //    }


    //    /// <summary>
    //    /// Called when [activated] which for entity means that was added to scene.
    //    /// </summary>
    //    /// <param name="source">The source of activation.</param>
    //    protected override void OnActivated(object source)
    //    {
    //        base.OnActivated(source);

    //        this.SetVisible = true;
    //    }

    //    /// <summary>
    //    /// Called when [deactivated] which for entity means that was removed from scene.
    //    /// </summary>
    //    /// <param name="source">The source of deactivation.</param>
    //    protected override void OnDeactivated(object source)
    //    {
    //        this.SetVisible = false;

    //        base.OnDeactivated(source);
    //    }

    //    public override bool IsSelectableAsChild()
    //    {
    //        return true;
    //    }

    //    public override void ResetRotation()
    //    {
    //        MoveAndRotate(this.GetPosition(), Parent.GetOrientation());
    //    }

    //    public override void Close()
    //    {
    //        MyScriptWrapper.OnEntityClose(this);

    //        // Remove possible linked snap points from editor
    //        MyEditor.Static.RemoveLinkedSnapPoints(this);

    //        base.Close();

    //        MyDecals.RemoveModelDecals(this);

    //        // ((MyPrefabContainer)Parent).RemovePrefab(this); // Parent is null for some prefabs (e.g. some large ship weapons)

    //        if (m_smokeEffects != null)
    //        {
    //            for (int i = 0; i < m_smokeEffects.Length; i++)
    //            {
    //                if (m_smokeEffects[i] != null)
    //                {
    //                    m_smokeEffects[i].Stop();
    //                    MyParticlesManager.RemoveParticleEffect(m_smokeEffects[i]);
    //                    m_smokeEffects[i] = null;
    //                }
    //            }
    //        }

    //        if (m_owner != null)
    //            m_owner.RemovePrefab(this);

    //        if (this.Physics != null)
    //            this.Physics.RemoveAllElements();

    //        if (m_lamps != null)
    //        {
    //            foreach (MyPrefabLamp prefabLamp in m_lamps)
    //            {
    //                prefabLamp.Close();
    //            }
    //        }
    //        if (m_lights != null)
    //        {
    //            foreach (MyLight light in m_lights)
    //            {
    //                MyLights.RemoveLight(light);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Generates explosion and debris.
    //    /// </summary>
    //    protected virtual void Explode()
    //    {
    //        DestroyPrefabsInside();

    //        // only if prefab is big enough, make it explode and create debris))
    //        if (WorldVolumeHr.Radius > MyExplosionsConstants.MIN_OBJECT_SIZE_TO_CAUSE_EXPLOSION_AND_CREATE_DEBRIS)
    //        {
    //            MyExplosion newExplosion = MyExplosions.AddExplosion();
    //            if (newExplosion != null)
    //            {
    //                BoundingSphere explosionSphere = WorldVolumeHr;
    //                explosionSphere.Radius *= 0.5f;
    //                explosionSphere.Radius = MathHelper.Min(explosionSphere.Radius, MyExplosionsConstants.EXPLOSION_RADIUS_MAX);
    //                //explosionSphere.Radius = MathHelper.Max(explosionSphere.Radius, MyExplosionsConstants.EXPLOSION_RANDOM_RADIUS_MIN);
    //                MyVoxelMap voxelMap = MyVoxelMaps.GetOverlappingWithSphere(ref explosionSphere);
    //                MyExplosionDebrisModel.CreateExplosionDebris(ref explosionSphere, MyGroupMask.Empty, this, voxelMap);
    //                newExplosion.Start(.03f * WorldVolumeHr.Radius, MyExplosionTypeEnum.SMALL_SHIP_EXPLOSION, explosionSphere, MyExplosionsConstants.EXPLOSION_LIFESPAN);
    //            }
    //        }
    //        else
    //        {
    //            var effect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Explosion_SmallPrefab);
    //            effect.WorldMatrix = WorldMatrix;
    //            effect.UserRadiusMultiplier = .1f * WorldVolumeHr.Radius;
    //        }
    //    }

    //    /// <summary>
    //    /// Using OBBs, detects prefabs that are COMPLETELY contained in this prefab's OBB (size multiplied by
    //    /// MyPrefabConstants.EXPLOSION_DELETE_MULTIPLIER).
    //    /// </summary>
    //    private void DestroyPrefabsInside()
    //    {
    //        var obb = MyEditorBoundingOrientedBox.CreateFromBoundingBox(LocalAABB).Transform(WorldMatrix);
    //        obb.HalfExtent *= MyPrefabConstants.EXPLOSION_DELETE_MULTIPLIER;

    //        var bb = WorldAABBHr;
    //        var intersecting = MyEntities.GetCollisionSkinsInIntersectingBoundingBox(ref bb);
    //        foreach (var rbElement in intersecting)
    //        {
    //            var rb = rbElement.GetRigidBody();
    //            if (rb == null)
    //                continue;
    //            var entity = ((MyGameRigidBody) rb.m_UserData).Entity;
                
    //            if (!(entity is MyPrefab) || entity == this)
    //                continue;

    //            var entityOBB = MyEditorBoundingOrientedBox.CreateFromBoundingBox(entity.LocalAABB).Transform(entity.WorldMatrix);
    //            if (obb.Contains(ref entityOBB) == ContainmentType.Contains)
    //            {
    //                entity.Close();
    //            }
    //        }
    //    }

    //    public override void Update()
    //    {
    //        if (IsDead() && !m_isExploded)
    //        {
    //            m_isExploded = true;
    //            Close();
    //            Explode();
    //        }

    //        if (IsDamaged() && !m_isExploded) 
    //            DrawDestructionEffects(GetDamageRatio());

    //        //UpdateAABBHr();

    //        base.Update();
    //    }

    //    public override bool UpdateAfterIntegration()
    //    {
    //        var result = base.UpdateAfterIntegration();

    //        if (m_smokeEffects != null)
    //        {
    //            if (m_smokeEffects.Length > 0)
    //            {
    //                UpdateSmokeEffects();
    //            }
    //        }

    //        return result;
    //    }

    //    private void UpdateSmokeEffects()
    //    {
    //        float ratio = GetDamageRatio();
    //        if (ratio < MyPrefabConstants.DAMAGED_HEALTH)
    //        {
    //            for (int i = 0; i < m_smokeEffects.Length; i++)
    //            {
    //                if (m_smokeEffects[i] != null)
    //                {
    //                    m_smokeEffects[i].Stop();
    //                    m_smokeEffects[i] = null;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            ratio = (ratio / MyPrefabConstants.DAMAGED_HEALTH) - 1;

    //            int smokesActive = Convert.ToInt32(MathHelper.Lerp(1, m_smokeMatrices.Length, ratio));

    //            // update active effects
    //            for (int i = 0; i < smokesActive; i++)
    //            {
    //                var smokeEffect = m_smokeEffects[i];
    //                if (smokeEffect == null)
    //                {
    //                    // alternate between 3 different effects
    //                    smokeEffect = MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.Damage_SmokeDirectionalA + (i % 3));
    //                    smokeEffect.AutoDelete = false;

    //                    m_smokeEffects[i] = smokeEffect;
    //                }

    //                smokeEffect.UserBirthMultiplier = 1;
    //                smokeEffect.WorldMatrix = m_smokeMatrices[i] * WorldMatrix;
    //            }

    //            // turn off inactive smoke effects
    //            for (int i = smokesActive; i < m_smokeEffects.Length; i++)
    //            {
    //                if (m_smokeEffects[i] != null)
    //                {
    //                    m_smokeEffects[i].UserBirthMultiplier = 0;
    //                }
    //            }
    //        }
    //    }

    //    public override void OnWorldPositionChanged(object source)
    //    {
    //        UpdateAABBHr();
    //        if (m_owner.WorldAABBHr.Contains(WorldAABBHr) != ContainmentType.Contains)
    //        {
    //            m_owner.UpdateAABB();
    //        }

    //        base.OnWorldPositionChanged(source);        
    //    }

    //    /// <summary>
    //    /// IMPORTANT: this is not really 'render' method. It only manipulates particle effects.
    //    /// </summary>
    //    protected void DrawDestructionEffects(float damagedPercentage)
    //    {
    //        float heavyDamagePercentage = (damagedPercentage - MyExplosionsConstants.DAMAGE_SPARKS) / (1 - MyExplosionsConstants.DAMAGE_SPARKS);

    //        if (heavyDamagePercentage < MyMwcMathConstants.EPSILON)
    //            return;

    //        MyModel model = GetModelLod0();

    //        if (MyMwcUtils.GetRandomInt((int)(MyExplosionsConstants.FRAMES_PER_SPARK / (heavyDamagePercentage * System.Math.Min(model.BoundingSphere.Radius, 100) * 0.05f))) > 0)
    //        //if (MyMwcUtils.GetRandomInt((int)(MyExplosionsConstants.FRAMES_PER_SPARK / heavyDamagePercentage)) > 0)
    //            return;

    //        int randomVertexIndex = MyMwcUtils.GetRandomInt(0, model.GetVerticesCount() - 1);
    //        Vector3 randomVertex = model.Vertexes[randomVertexIndex];
    //        var wMatrix = this.WorldMatrix;
    //        Vector3 vertexInWorldSpace = MyUtils.GetTransform(randomVertex, ref wMatrix);
    //        if (MyFakes.ENABLE_NEW_PARTICLES)
    //        {
    //            MyParticleEffect sparksEffect = MyParticlesManager.CreateParticleEffect((int)MyParticleEffectsIDEnum.Damage_Sparks);
    //            sparksEffect.WorldMatrix = Matrix.CreateTranslation(vertexInWorldSpace);
    //            sparksEffect.UserRadiusMultiplier = 0.003f * model.BoundingSphere.Radius;
    //            MyAudio.AddCue3D(MySoundCuesEnum.SfxSpark, WorldMatrix.Translation, WorldMatrix.Forward, WorldMatrix.Up,
    //                             Vector3.Zero);

    //            MyLight light = MyLights.AddLight();
    //            light.Start(MyLight.LightTypeEnum.PointLight, 1.0f);
    //            light.PointOn = false;
    //            light.Range = 1.0f;
    //            sparksEffect.Tag = light;
    //            sparksEffect.OnUpdate += new EventHandler(explosionEffect_OnUpdate);
    //            sparksEffect.OnDelete += new EventHandler(explosionEffect_OnDelete);
    //        }
    //    }

    //    static void explosionEffect_OnDelete(object sender, EventArgs e)
    //    {
    //        MyParticleEffect explosionEffect = sender as MyParticleEffect;
    //        MyLights.RemoveLight(explosionEffect.Tag as MyLight);
    //    }

    //    static void explosionEffect_OnUpdate(object sender, EventArgs e)
    //    {
    //        MyParticleEffect explosionEffect = sender as MyParticleEffect;
    //        MyLight light = explosionEffect.Tag as MyLight;
    //        light.PointOn = true;
    //        light.Position = explosionEffect.WorldMatrix.Translation;
    //        light.Range = MathHelper.Clamp((float)explosionEffect.GetAABB().Size().Length() / 2.0f, 1.0f, MyLightsConstants.MAX_POINTLIGHT_RADIUS);
    //        MyAnimatedPropertyVector4 animatedProperty; float time;
    //        explosionEffect.GetGenerations()[3].Color.GetPreviousValue(0.0f, out animatedProperty, out time);
    //        light.Intensity = 10 * MyMwcUtils.GetRandomFloat(0.6f, 0.9f);

    //        Vector4 lightColor;
    //        animatedProperty.GetInterpolatedValue<Vector4>(0.5f, out lightColor);
    //        light.Color = lightColor;
    //    }

    //    public override MyMwcObjectBuilder_FactionEnum Faction
    //    {
    //        get
    //        {
    //            if (m_owner != null)
    //            {
    //                return m_owner.Faction;
    //            }
    //            return base.Faction;
    //        }
    //    }

    //    //public MyMwcObjectBuilder_Prefab_TypesEnum PrefabType
    //    //{
    //    //    get { return m_prefabTypeEnum; }
    //    //}

    //    public CategoryTypesEnum PrefabCategory
    //    {
    //        get { return m_config.CategoryType; }
    //    }

    //    public SubCategoryTypesEnum? PrefabSubCategory
    //    {
    //        get { return m_config.SubCategoryType; }
    //    }

    //    public PrefabTypesFlagEnum PrefabTypeFlag
    //    {
    //        get { return m_config.PrefabTypeFlag; }
    //    }        
    //}
    #endregion

    class MyPrefab : MyPrefabBase
    {        
        public MyPrefab(MyPrefabContainer owner)
            : base(owner)
        {
        }        

        public MyMwcObjectBuilder_Prefab_TypesEnum PrefabType
        {
            get
            {
                return (MyMwcObjectBuilder_Prefab_TypesEnum) m_prefabId;
            }
            set
            {
                m_prefabId = (int) value;
            }
        }

        public override string GetFriendlyName()
        {
            return "MyPrefab";
        }
    }
}

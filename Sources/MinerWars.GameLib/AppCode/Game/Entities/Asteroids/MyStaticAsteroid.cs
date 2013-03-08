using System.Runtime.Serialization;
using MinerWarsMath;
using MinerWars.AppCode.Game.TransparentGeometry.Particles;
using SysUtils.Utils;
using System.Text;

using System.Collections.Generic;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Models;
using System;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities
{
    using MinerWars.CommonLIB.AppCode.Networking;
    using MinerWars.AppCode.Game.Voxels;
    using System.Diagnostics;
    using MinerWars.CommonLIB.AppCode.ObjectBuilders;
    using MinerWars.AppCode.Game.SolarSystem;
    using MinerWars.AppCode.Game.Render;
    using MinerWars.AppCode.Game.World;
    using MinerWars.CommonLIB.AppCode.Import;

    /// <summary>
    /// Represent static non-destructable asteroid.
    /// </summary>
    class MyStaticAsteroid : MyEntity
    {
        MyMeshMaterial m_meshMaterial;
        MyMeshDrawTechnique m_drawTechnique = MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID;
        public MyMwcVoxelMaterialsEnum? VoxelMaterial1;
        public Vector3? FieldDir;

        #region Properties

        /// <summary>
        /// Gets or sets the type of the asteroid.
        /// </summary>
        /// <value>
        /// The type of the asteroid.
        /// </value>
        public MyMwcObjectBuilder_StaticAsteroid_TypesEnum AsteroidType { get; set; }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="MyStaticAsteroid"/> class.
        /// </summary>
        public MyStaticAsteroid()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyStaticAsteroid"/> class.
        /// </summary>
        public MyStaticAsteroid(MyModel modelLod0)
        {
            m_modelLod0 = modelLod0;
        }

    
        #endregion

        #region Init

        internal struct MyStaticAsteroidModels
        {
            public MyStaticAsteroidModels(MyModelsEnum lod0, MyModelsEnum? lod1, MyModelsEnum? lod2)
            {
                LOD0 = lod0;
                LOD1 = lod1;
                LOD2 = lod2;
            }

            public MyModelsEnum LOD0;
            public MyModelsEnum? LOD1;
            public MyModelsEnum? LOD2;
        }

        /// <summary>
        /// Inits the specified hud label text.
        /// </summary>
        /// <param name="hudLabelText">The hud label text.</param>
        /// <param name="objectBuilder">The object builder.</param>
        public void Init(string hudLabelText, MyMwcObjectBuilder_StaticAsteroid objectBuilder, Matrix matrix)
        {
            MyStaticAsteroidModels models = GetModelsFromType(objectBuilder.AsteroidType);

            StringBuilder hudLabelTextSb = (hudLabelText == null) ? null : new StringBuilder(hudLabelText);

            if (objectBuilder.Generated)
            {
                Flags &= ~EntityFlags.EditableInEditor;
                Flags &= ~EntityFlags.NeedsId;
            }
            else
            {
                Flags |= EntityFlags.EditableInEditor;
                Flags |= EntityFlags.NeedsId;
            }

            CastShadows = !objectBuilder.Generated;

            if (!objectBuilder.AsteroidMaterial1.HasValue && MySector.Area.HasValue)
            {
                var area = MySolarSystemConstants.Areas[MySector.Area.Value];
                objectBuilder.AsteroidMaterial1 = area.SecondaryStaticAsteroidMaterial;
                objectBuilder.FieldDir = MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.GetDirectionToSunNormalized();
            }

            NeedsUpdate = false;

            Init(hudLabelTextSb, models.LOD0, models.LOD1, null, null, objectBuilder, null, models.LOD2);

            AsteroidType = objectBuilder.AsteroidType;

            SetWorldMatrix(matrix);

            FieldDir = objectBuilder.FieldDir;

            if (objectBuilder.AsteroidMaterial.HasValue)
            {
                VoxelMaterial = objectBuilder.AsteroidMaterial.Value;
                VoxelMaterial1 = objectBuilder.AsteroidMaterial1;
                m_drawTechnique = MyMeshDrawTechnique.VOXELS_STATIC_ASTEROID;
            }

            if (objectBuilder.UseModelTechnique)
            {
                m_meshMaterial = MyVoxelMaterials.GetMaterialForMesh(VoxelMaterial);
                m_drawTechnique = MyMeshDrawTechnique.MESH;
            }

            InitDrawTechniques();

            InitPhysics();

            MyModels.OnContentLoaded += InitDrawTechniques;
            InitDrawTechniques();

        }

        /// <summary>
        /// Backup conversion from ob type to models.
        /// </summary>
        /// <param name="type">The type.</param>
        public static MyStaticAsteroidModels GetModelsFromType(MyMwcObjectBuilder_StaticAsteroid_TypesEnum type)
            {
            switch (type)
            {
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10m_A_LOD0, MyModelsEnum.StaticAsteroid10m_A_LOD1, MyModelsEnum.StaticAsteroid10m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid20m_A_LOD0, MyModelsEnum.StaticAsteroid20m_A_LOD1, MyModelsEnum.StaticAsteroid20m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid30m_A_LOD0, MyModelsEnum.StaticAsteroid30m_A_LOD1, MyModelsEnum.StaticAsteroid30m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid50m_A_LOD0, MyModelsEnum.StaticAsteroid30m_A_LOD1, MyModelsEnum.StaticAsteroid30m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid100m_A_LOD0, MyModelsEnum.StaticAsteroid100m_A_LOD1, MyModelsEnum.StaticAsteroid100m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid300m_A_LOD0, MyModelsEnum.StaticAsteroid300m_A_LOD1, MyModelsEnum.StaticAsteroid300m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid500m_A_LOD0, MyModelsEnum.StaticAsteroid500m_A_LOD1, MyModelsEnum.StaticAsteroid500m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid1000m_A_LOD0, MyModelsEnum.StaticAsteroid1000m_A_LOD1, MyModelsEnum.StaticAsteroid1000m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid2000m_A_LOD0, MyModelsEnum.StaticAsteroid2000m_A_LOD1, MyModelsEnum.StaticAsteroid2000m_A_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid5000m_A_LOD0, MyModelsEnum.StaticAsteroid5000m_A_LOD1, MyModelsEnum.StaticAsteroid5000m_A_LOD1);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_A:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10000m_A_LOD0, MyModelsEnum.StaticAsteroid10000m_A_LOD1, MyModelsEnum.StaticAsteroid10000m_A_LOD1);
                //case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid40000m_A:
                  //  return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid40000m_A_LOD0, MyModelsEnum.StaticAsteroid40000m_A_LOD1, MyModelsEnum.StaticAsteroid40000m_A_LOD2);

                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10m_B_LOD0, MyModelsEnum.StaticAsteroid10m_B_LOD1, MyModelsEnum.StaticAsteroid10m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid20m_B_LOD0, MyModelsEnum.StaticAsteroid20m_B_LOD1, MyModelsEnum.StaticAsteroid20m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid30m_B_LOD0, MyModelsEnum.StaticAsteroid30m_B_LOD1, MyModelsEnum.StaticAsteroid30m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid50m_B_LOD0, MyModelsEnum.StaticAsteroid50m_B_LOD1, MyModelsEnum.StaticAsteroid50m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid100m_B_LOD0, MyModelsEnum.StaticAsteroid100m_B_LOD1, MyModelsEnum.StaticAsteroid100m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid300m_B_LOD0, MyModelsEnum.StaticAsteroid300m_B_LOD1, MyModelsEnum.StaticAsteroid300m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid500m_B_LOD0, MyModelsEnum.StaticAsteroid500m_B_LOD1, MyModelsEnum.StaticAsteroid500m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid1000m_B_LOD0, MyModelsEnum.StaticAsteroid1000m_B_LOD1, MyModelsEnum.StaticAsteroid1000m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid2000m_B_LOD0, MyModelsEnum.StaticAsteroid2000m_B_LOD1, MyModelsEnum.StaticAsteroid2000m_B_LOD2);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid5000m_B_LOD0, MyModelsEnum.StaticAsteroid5000m_B_LOD1, MyModelsEnum.StaticAsteroid5000m_B_LOD1);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_B:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10000m_B_LOD0, MyModelsEnum.StaticAsteroid10000m_B_LOD1, MyModelsEnum.StaticAsteroid10000m_B_LOD1);

                    //Removed support
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10m_A_LOD0, MyModelsEnum.StaticAsteroid10m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid20m_A_LOD0, MyModelsEnum.StaticAsteroid20m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid30m_A_LOD0, MyModelsEnum.StaticAsteroid30m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid50m_A_LOD0, MyModelsEnum.StaticAsteroid50m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid100m_A_LOD0, MyModelsEnum.StaticAsteroid100m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid300m_A_LOD0, MyModelsEnum.StaticAsteroid300m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid500m_A_LOD0, MyModelsEnum.StaticAsteroid500m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid1000m_A_LOD0, MyModelsEnum.StaticAsteroid1000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid2000m_A_LOD0, MyModelsEnum.StaticAsteroid2000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid5000m_A_LOD0, MyModelsEnum.StaticAsteroid5000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_C:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10000m_A_LOD0, MyModelsEnum.StaticAsteroid10000m_A_LOD1, null);

                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10m_A_LOD0, MyModelsEnum.StaticAsteroid10m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid20m_A_LOD0, MyModelsEnum.StaticAsteroid20m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid30m_A_LOD0, MyModelsEnum.StaticAsteroid30m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid50m_A_LOD0, MyModelsEnum.StaticAsteroid50m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid100m_A_LOD0, MyModelsEnum.StaticAsteroid100m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid300m_A_LOD0, MyModelsEnum.StaticAsteroid300m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid500m_A_LOD0, MyModelsEnum.StaticAsteroid500m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid1000m_A_LOD0, MyModelsEnum.StaticAsteroid1000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid2000m_A_LOD0, MyModelsEnum.StaticAsteroid2000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid5000m_A_LOD0, MyModelsEnum.StaticAsteroid5000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_D:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10000m_A_LOD0, MyModelsEnum.StaticAsteroid10000m_A_LOD1, null);

                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10m_A_LOD0, MyModelsEnum.StaticAsteroid10m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid20m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid20m_A_LOD0, MyModelsEnum.StaticAsteroid20m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid30m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid30m_A_LOD0, MyModelsEnum.StaticAsteroid30m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid50m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid50m_A_LOD0, MyModelsEnum.StaticAsteroid50m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid100m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid100m_A_LOD0, MyModelsEnum.StaticAsteroid100m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid300m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid300m_A_LOD0, MyModelsEnum.StaticAsteroid300m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid500m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid500m_A_LOD0, MyModelsEnum.StaticAsteroid500m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid1000m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid1000m_A_LOD0, MyModelsEnum.StaticAsteroid1000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid2000m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid2000m_A_LOD0, MyModelsEnum.StaticAsteroid2000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid5000m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid5000m_A_LOD0, MyModelsEnum.StaticAsteroid5000m_A_LOD1, null);
                case MyMwcObjectBuilder_StaticAsteroid_TypesEnum.StaticAsteroid10000m_E:
                    return new MyStaticAsteroidModels(MyModelsEnum.StaticAsteroid10000m_A_LOD0, MyModelsEnum.StaticAsteroid10000m_A_LOD1, null);
                    
                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }
        }

        /// <summary>
        /// Inits the physics.
        /// </summary>
        protected virtual void InitPhysics()
        {
            if (!MySectorGenerator.IsOutsideSector(this.GetPosition(), LocalVolume.Radius))
            {
                // create the box
                InitTrianglePhysics(MyMaterialType.ROCK, 1.0f, ModelCollision, null);
            }
        }

        public override void InitDrawTechniques()
        {
            InitDrawTechniques(m_drawTechnique);

            if (m_meshMaterial != null)
                m_meshMaterial.DrawTechnique = m_drawTechnique;
        }

        #endregion

        public override bool Draw(MyRenderObject renderObject)
        {
            if (MyFakes.STRANGE_PARTICLES_WHEN_DUST_ON_STATIC_ASTEROIDS)
            {
                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Smoke, new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
                    GetPosition(), 800, GetPosition().Length());

                MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Smoke, new Vector4(0.1f, 0.1f, 0.1f, 1.0f),
                    GetPosition() + (Vector3.Normalize(MyCamera.Position - this.GetPosition()) * 250f), 300, GetPosition().Length() * 2.564f);
            }

            return base.Draw(renderObject);
        }

        public override string GetFriendlyName()
        {
            return "MyStaticAsteroid";
        }

        public override MyMeshMaterial GetMaterial(MyMesh mesh)
        {
            if (m_meshMaterial == null)
                return base.GetMaterial(mesh);

            m_meshMaterial.PreloadTexture();
            return m_meshMaterial;
        }

        public override void Close()
        {
            MyModels.OnContentLoaded -= InitDrawTechniques;
            base.Close();
        }

        public override MyMwcVoxelMaterialsEnum VoxelMaterial
        {
            set
            {
                base.VoxelMaterial = value;
                //m_meshMaterial = MyVoxelMaterials.GetMaterialForMesh(voxelMaterial);
            }
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            var staticAsteroidBuilder = (MyMwcObjectBuilder_StaticAsteroid)base.GetObjectBuilderInternal(getExactCopy);

            staticAsteroidBuilder.AsteroidMaterial = VoxelMaterial;

            return staticAsteroidBuilder;
        }

        public void Explode()
        {
            var explosionEffect = MyParticlesManager.CreateParticleEffect((int) MyParticleEffectsIDEnum.Explosion_Bomb);
            explosionEffect.WorldMatrix = this.WorldMatrix;
            explosionEffect.UserScale = .05f * this.WorldVolume.Radius;
            this.MarkForClose();
        }
    }
}

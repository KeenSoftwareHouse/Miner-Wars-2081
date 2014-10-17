using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Render;

using MinerWarsMath;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.Models
{
    using MinerWars.AppCode.App;
    using System.Diagnostics;
    using MinerWars.AppCode.Game.Utils;
    using MinerWars.AppCode.Game.Managers;
    using MinerWars.CommonLIB.AppCode.Import;

    //@ Simple stupid material class which enwrapp 2 textures and generate uniqueID form textures
    internal class MyMeshMaterial
    {
        private const string C_FAKE_NORMAL_TEXTURE = "Textures2\\Models\\fake_n";

        public int HashCode;
        private MyTexture2D m_diffuseTex;
        private MyTexture2D m_normalTex;
        private MyTexture2D m_heightTex;

        private float m_specularIntensity = 1f;
        private float m_specularPower = 1f;
        private Vector3 m_specularColor;
        private Vector3 m_diffuseColor = new Vector3(1f, 1f, 1f);

        private readonly string m_materialName;
        private readonly string m_diffuseName;
        private readonly string m_normalName;
        private readonly bool m_hasNormalTexture;
        private MyMeshDrawTechnique m_drawTechnique;
        private Vector2 m_emissiveUVAnim;
        private bool m_emissivityEnabled = true;

        private bool m_loadedContent;

        Vector2 m_diffuseUVAnim;
        private string p;
        private string assetName;
        private MyTexture2D myTexture2D1;
        private MyTexture2D myTexture2D2;
        private MyTexture2D myTexture2D3;
        private string m_heightName;
     
        public Vector2 DiffuseUVAnim
        {
            get { return m_diffuseUVAnim; }
            set { m_diffuseUVAnim = value; ComputeHashCode(); }
        }

        public Vector2 EmissiveUVAnim 
        {
            get 
            {
                if (m_emissivityEnabled)
                {
                    return m_emissiveUVAnim;
                }
                else 
                {
                    return Vector2.Zero;
                }
            }
            set 
            {
                m_emissiveUVAnim = value;
                ComputeHashCode();
            }
        }
                
        public bool EmissivityEnabled 
        {
            get { return m_emissivityEnabled; }
            set
            {
                if (m_emissivityEnabled != value)
                {
                    m_emissivityEnabled = value;
                    ComputeHashCode();
                }
            }
        }

        public float EmissivityOffset 
        {
            get 
            {
                if (EmissivityEnabled)
                {
                    return MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000.0f;
                }
                else 
                {
                    return 0f;
                }
            }
        }

        public float HoloEmissivity 
        {
            get 
            {
                if (EmissivityEnabled)
                {
                    if (SpecularColor.Y > 0)
                    {  //material with animated emissivity
                        return (((float)Math.Sin(MyMinerGame.TotalGamePlayTimeInMilliseconds / 1000.0f * SpecularColor.Y * 10.0f + SpecularColor.Z * 2 * (float)Math.PI)) + 1.0f) / 2.0f;
                    }
                    else //Holos and decals have multiplied emissivity
                    {
                        return 1f;
                    }
                }
                else
                {
                    return 0f;
                }
            }
        }

        public MyMeshDrawTechnique DrawTechnique
        {
            get { return m_drawTechnique; }
            set { m_drawTechnique = value; }
        }

        public MyTexture2D DiffuseTexture
        {
            get { return m_diffuseTex; }
            set { m_diffuseTex = value; ComputeHashCode(); }
        }
        
        public MyTexture2D HeightTexture
        {
            get { return m_heightTex; }
            set { m_heightTex = value; ComputeHashCode(); }
        }
        public MyTexture2D NormalTexture
        {
            get { return m_normalTex; }
            set { m_normalTex = value; ComputeHashCode(); }
        }
        public float SpecularIntensity
        {
            get { return m_specularIntensity; }
            set { m_specularIntensity = value; ComputeHashCode(); }
        }
        public float SpecularPower
        {
            get { return m_specularPower; }
            set { m_specularPower = value; ComputeHashCode(); }
        }
        public Vector3 SpecularColor
        {
            get { return m_specularColor; }
            set { m_specularColor = value; ComputeHashCode(); }
        }
        public Vector3 DiffuseColor
        {
            get { return m_diffuseColor; }
            set { m_diffuseColor = value; ComputeHashCode(); }
        }

        public string MaterialName
        {
            get { return m_materialName; }
        }
    
        public MyMeshMaterial(string name, string materialName, MyTexture2D diff, MyTexture2D norm, MyTexture2D height)
          {
            if (name!= null)
            {
                m_diffuseName = name + MyMesh.C_POSTFIX_DIFFUSE_EMISSIVE;
                m_normalName = name + MyMesh.C_POSTFIX_NORMAL_SPECULAR;
                m_heightName = name + MyMesh.C_POSTFIX_HEIGHT_MAP;
            }
            m_materialName = materialName;
            m_drawTechnique = MyMeshDrawTechnique.MESH;
            HashCode = 0;
            m_diffuseTex = diff;
            m_normalTex = norm;
            m_heightTex = height;
            m_hasNormalTexture = m_normalTex != null;

            ComputeHashCode();
        }
        /// <summary>
        /// MyMeshMaterial
        /// </summary>
        /// <param name="specularLevel"></param>
        /// <param name="glossiness"></param>
        public MyMeshMaterial(string materialName, string diffuseName, string normalName, float glossiness, bool hasNormalTexture, ref Vector3 diffuseColor, ref Vector3 specularColor)
        {
            if (diffuseColor == Vector3.Zero)
            {
                // Debug.Assert(diffuseColor != Vector3.Zero);
                diffuseColor = Vector3.One;
            }

            m_materialName = materialName;
            m_diffuseName = diffuseName;
            m_normalName = normalName;
            m_specularIntensity = specularColor.X; //because of strange 3DSMAX/FBX conversion, specular level from 3ds max converts to .X component of specular color
            m_specularPower = glossiness;
            m_specularPower = glossiness; 
            m_diffuseColor = diffuseColor;
            m_hasNormalTexture = hasNormalTexture;

            //we are not using specular color directly, we just use it to store extra data (animation of holos)
            m_specularColor = specularColor;
        }
        /// <summary>
        /// Preload textures into manager
        /// </summary>
        public void PreloadTexture(LoadingMode loadingMode = (MyFakes.LOAD_TEXTURES_IMMEDIATELY ? LoadingMode.Immediate : LoadingMode.LazyBackground))
        {
            if (m_loadedContent || m_diffuseName == null)
            {
                return;
            }

            if (m_hasNormalTexture)
            {
                DiffuseTexture = MyTextureManager.GetTexture<MyTexture2D>(m_diffuseName, CheckTexture, loadingMode);

                if (MyRenderConstants.RenderQualityProfile.UseNormals)
                    NormalTexture = MyTextureManager.GetTexture<MyTexture2D>(m_normalName, CheckTexture, loadingMode);
            }
            else
            {
                DiffuseTexture = MyTextureManager.GetTexture<MyTexture2D>(m_diffuseName, CheckTexture, loadingMode);

                if (MyRenderConstants.RenderQualityProfile.UseNormals)
                    NormalTexture = MyTextureManager.GetTexture<MyTexture2D>(C_FAKE_NORMAL_TEXTURE, CheckTexture, loadingMode);
            }

            m_loadedContent = true;
        }

        /// <summary>
        /// Checks the normal map.
        /// </summary>
        /// <param name="texture">The texture.</param>
        private static void CheckTexture(MyTexture texture)
        {
            MyUtils.AssertTexture((MyTexture2D)texture);

            texture.TextureLoaded -= CheckTexture;
        }

        /// <summary>
        /// ComputeHashCode
        /// </summary>
        /// <returns></returns>
        private void ComputeHashCode()
        {
            int result = 1;
            int modCode = 0;

            if (m_diffuseTex != null)
            {
                result = m_diffuseTex.GetHashCode();
                modCode = (1 << 1);
            }

            if (m_normalTex != null)
            {
                result = (result * 397) ^ m_normalTex.GetHashCode();
                modCode += (1 << 2);
            }

            if (m_heightTex != null)
            {
                result = m_heightTex.GetHashCode();
            }

            if (m_specularIntensity != 0)
            {
                result = (result * 397) ^ m_specularIntensity.GetHashCode();
                modCode += (1 << 4);
            }

            if (m_specularPower != 0)
            {
                result = (result * 397) ^ m_specularPower.GetHashCode();
                modCode += (1 << 5);
            }

            if (m_diffuseColor.GetHashCode() != 0)
            {
                result = (result * 397) ^ m_diffuseColor.GetHashCode();
                modCode += (1 << 6);
            }

            if (DiffuseUVAnim.GetHashCode() != 0)
            {
                result = (result * 397) ^ DiffuseUVAnim.GetHashCode();
                modCode += (1 << 7);               
            }

            if (EmissiveUVAnim.GetHashCode() != 0)
            {
                result = (result * 397) ^ EmissiveUVAnim.GetHashCode();
                modCode += (1 << 8);
            }

            result = (result * 397) ^ EmissivityEnabled.GetHashCode();
            modCode += (1 << 9);

            HashCode = (result * 397) ^ modCode;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return HashCode;
            }
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (obj.GetType() != typeof(MyMeshMaterial)) 
                return false;
            return HashCode == obj.GetHashCode();
        }


        /// <summary>
        /// Return modelDrawTechnique based on textures
        /// </summary>
        /// <returns></returns>
        public MyMeshDrawTechnique GenerateDrawTechnique()
        {
            return MyMeshDrawTechnique.MESH;
        }
    }
}

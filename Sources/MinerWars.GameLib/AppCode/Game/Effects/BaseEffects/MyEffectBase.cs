using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;
using System.IO;
                             /*
using MinerWarsMath;

using MinerWarsMath.Graphics;
                               */
using SharpDX.Direct3D;
using SharpDX.Direct3D9;

//  This class is wrapper for XNA effect with added functionality of holding references to commonly used parameters (light positions, fog, camera, matrixes, etc).

namespace MinerWars.AppCode.Game.Effects
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Matrix = MinerWarsMath.Matrix;
    using System;

    //  Base class for all effects
    public abstract class MyEffectBase : IDisposable
    {
        protected readonly Effect m_D3DEffect;

        EffectHandle m_fogDistanceNear;
        EffectHandle m_fogDistanceFar;
        EffectHandle m_fogColor;
        EffectHandle m_fogMultiplier;
        EffectHandle m_fogBacklightMultiplier;

        EffectHandle m_lodCut;
        EffectHandle m_lodBackgroundCut;

        protected MyEffectBase(Effect xnaEffect)
        {
            m_D3DEffect = xnaEffect;

            Init();
        }

        protected MyEffectBase(string asset)
        {
            string curdir = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(MyMinerGame.Static.RootDirectoryEffects + "\\" + asset));

            string sourceFX = Path.GetFileName(asset + ".fx");
            string compiledFX = Path.GetFileName(asset + ".fxo");

            bool needRecompile = false;
            if (File.Exists(compiledFX))
            {
                if (File.Exists(sourceFX))
                {
                    DateTime compiledTime = File.GetLastWriteTime(compiledFX);
                    DateTime sourceTime = File.GetLastWriteTime(sourceFX);
                    if (sourceTime > compiledTime)
                        needRecompile = true;
                }
            }
            else
            {
                if (File.Exists(sourceFX))
                    needRecompile = true;
                else
                {
                    throw new FileNotFoundException("Effect not found: " + asset);
                }
            }

            //Nepouzivat ShaderFlags.PartialPrecision, kurvi to na GeForce6600
            ShaderFlags flags = ShaderFlags.OptimizationLevel3 | ShaderFlags.SkipValidation;
       
            if (needRecompile)
            {
//#if DEBUG
//                flags |= ShaderFlags.Debug;
//#endif
                //m_D3DEffect = Effect.FromFile(MyMinerGameDX.Static.GraphicsDevice, sourceFX, flags);
          
                ShaderBytecode shaderByteCode = ShaderBytecode.CompileFromFile(sourceFX, "fx_2_0", flags);
                shaderByteCode.Save(compiledFX);
                shaderByteCode.Dispose();
            }

            FileStream fs = File.Open(compiledFX, FileMode.Open, FileAccess.Read);
            byte[] m = new byte[fs.Length];
            fs.Read(m, 0, (int)fs.Length);
            fs.Close();
            fs.Dispose();

            m_D3DEffect = Effect.FromMemory(MyMinerGame.Static.GraphicsDevice, m, flags);

            System.IO.Directory.SetCurrentDirectory(curdir);

            Init();
        }

        private void Init()
        {
            EffectHandle nearPlane = m_D3DEffect.GetParameter(null, "NEAR_PLANE_DISTANCE");
            EffectHandle farPlane = m_D3DEffect.GetParameter(null, "FAR_PLANE_DISTANCE");

            if (nearPlane != null && farPlane != null)
            {
                m_D3DEffect.SetValue(farPlane, MyCamera.FAR_PLANE_DISTANCE);
                m_D3DEffect.SetValue(nearPlane, MyCamera.NEAR_PLANE_DISTANCE);
            }

            m_fogDistanceNear = m_D3DEffect.GetParameter(null, "FogDistanceNear");
            m_fogDistanceFar = m_D3DEffect.GetParameter(null, "FogDistanceFar");
            m_fogColor = m_D3DEffect.GetParameter(null, "FogColor");
            m_fogMultiplier = m_D3DEffect.GetParameter(null, "FogMultiplier");
            m_fogBacklightMultiplier = m_D3DEffect.GetParameter(null, "FogBacklightMultiplier");

            m_lodCut = m_D3DEffect.GetParameter(null, "LodCut");
            m_lodBackgroundCut = m_D3DEffect.GetParameter(null, "LodBackgroundCut");

            //UseChannels = false;
        }

        public virtual void Dispose()
        {
            m_D3DEffect.Dispose();
        }

        public virtual void SetTextureNormal(Texture texture2D) { } 
        public virtual void SetTextureDiffuse(Texture texture2D) { }

        public virtual bool IsTextureNormalSet() { return true; }
        public virtual bool IsTextureDiffuseSet() { return true; }
        public virtual bool IsTextureSpecularSet() { return true; }

        public virtual void SetDiffuseColor(MinerWarsMath.Vector3 diffuseColor) { }
        public virtual void SetHighlightColor(MinerWarsMath.Vector3 highlight) { }

        public virtual void SetEmissivity(float emissivity) { }
        public virtual void SetEmissivityOffset(float emissivityOffset) { }
        public virtual void SetEmissivityUVAnim(Vector2 uvAnim) { }
        public virtual void SetDiffuseUVAnim(Vector2 uvAnim) { }


        public virtual void SetSpecularPower(float specularPower) { }
        public virtual void SetSpecularIntensity(float specularIntensity) { }

        public virtual void SetProjectionMatrix(ref Matrix projectionMatrix) { }
        public virtual void SetViewMatrix(ref Matrix matrix) { }


        /*
        public virtual void Apply()
        {
            //m_D3DEffect.Technique.Passes[0].Apply();
        } */

        bool begin = false;
        public virtual void Begin(int pass = 0, FX fx = FX.None)
        {
            System.Diagnostics.Debug.Assert(begin == false);
            m_D3DEffect.Begin(fx);
            m_D3DEffect.BeginPass(pass);
            begin = true;
        }

        public virtual void End()
        {
            System.Diagnostics.Debug.Assert(begin == true);
            m_D3DEffect.EndPass();
            m_D3DEffect.End();
            begin = false;
        } 


        public void SetFogDistanceNear(float fogDistanceNear)
        {
            m_D3DEffect.SetValue(m_fogDistanceNear, fogDistanceNear);
        }

        public void SetFogDistanceFar(float fogDistanceFar)
        {
            m_D3DEffect.SetValue(m_fogDistanceFar, fogDistanceFar);
        }

        public void SetFogColor(Vector3 fogColor)
        {
            m_D3DEffect.SetValue(m_fogColor, fogColor);
        }

        public void SetFogMultiplier(float fogMultiplier)
        {
            m_D3DEffect.SetValue(m_fogMultiplier, fogMultiplier);
        }

        public void SetFogBacklightMultiplier(float multiplier)
        {
            m_D3DEffect.SetValue(m_fogBacklightMultiplier, multiplier);
        }

        public void SetLodCut(float lodCut)
        {
            m_D3DEffect.SetValue(m_lodCut, lodCut);
        }

        public void SetLodBackgroundCut(float lodCut)
        {
            m_D3DEffect.SetValue(m_lodBackgroundCut, lodCut);
        }

        public Effect D3DEffect
        {
            get { return m_D3DEffect; }
        }
    }
}

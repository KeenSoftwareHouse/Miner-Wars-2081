using System;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Renders;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Entities.InfluenceSpheres;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.Game.Render;

//  This class manages virtual dust field surounding the camera. Field doesn't exist, it's generated every draw call.
//  It's variance is random, but persistent.

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    static class MyParticlesDustField
    {
        static float[][][] m_random;
        static Matrix m_helperProjectionMatrix;
        static BoundingFrustum m_helperBoundingFrustum;

        static float animXSpeed = 0.1f;

        static int m_lastDustFieldCountInDirectionHalf = 0;
        static float m_lastDustBillboardRadius = 0;

        static int m_dustFieldCountInDirection;
        static float m_distanceBetweenHalf;

        public static Color? CustomColor = null;

        //public static float DUST_BILLBOARD_RADIUS = 3;


        static MyParticlesDustField()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.ParticlesDustField, "Particles dust field", Draw, Render.MyRenderStage.PrepareForDraw, true);
        }

        public static void LoadData()
        {
            MyRender.GetRenderProfiler().StartProfilingBlock("MyParticlesDustField.LoadData");

            MyMwcLog.WriteLine("MyParticlesDustField.LoadContent() - START");
            MyMwcLog.IncreaseIndent();


            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyParticlesDustField.LoadContent() - END");
            MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        static MyMwcVector3Int GetMetersToDustFieldCoord(ref Vector3 position)
        {
            return new MyMwcVector3Int(
                (int)(position.X / MySector.ParticleDustProperties.DistanceBetween),
                (int)(position.Y / MySector.ParticleDustProperties.DistanceBetween),
                (int)(position.Z / MySector.ParticleDustProperties.DistanceBetween));
        }

        //  This method doesn't really draw. It just creates billboards that are later drawn in MyParticles.Draw()
        public static void Draw()
        {
            if (!MySector.ParticleDustProperties.Enabled)
                return;
            if (MinerWars.AppCode.Game.Render.MyRenderConstants.RenderQualityProfile.ForwardRender)
                return;
           // if (MyRender.CurrentRenderSetup.CallerID.Value != MyRenderCallerEnum.Main)
             //   return;
            if ((int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf == 0)
                return;

            MyRender.GetRenderProfiler().StartProfilingBlock("Dust changed");

            if ((m_lastDustBillboardRadius != MySector.ParticleDustProperties.DustBillboardRadius)
                ||
                m_lastDustFieldCountInDirectionHalf != (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf)
            {
                m_lastDustBillboardRadius = MySector.ParticleDustProperties.DustBillboardRadius;
                m_lastDustFieldCountInDirectionHalf = (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf;
                m_distanceBetweenHalf = MySector.ParticleDustProperties.DistanceBetween / 2.0f;
                m_dustFieldCountInDirection = (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf * 2 + 1;

                //  Bounding frustum is based on camer's bounding frustun, but far plane isn't in such distance, because then bounding box is too large
                //  IMPORTANT: Near plane can't be 0.001 or something small like that. Because than bounding box is weird.
                m_helperProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MyCamera.FieldOfView, MyCamera.ForwardAspectRatio,
                    MyCamera.NEAR_PLANE_DISTANCE,
                    Math.Max(MySector.ParticleDustProperties.DistanceBetween * MySector.ParticleDustProperties.DustFieldCountInDirectionHalf, MyCamera.NEAR_PLANE_DISTANCE + 0.01f));
                m_helperBoundingFrustum = new BoundingFrustum(Matrix.Identity);



                //  Fill 3D array with random values from interval <0..1>
                m_random = new float[m_dustFieldCountInDirection][][];
                for (int x = 0; x < m_random.Length; x++)
                {
                    m_random[x] = new float[m_dustFieldCountInDirection][];

                    for (int y = 0; y < m_random.Length; y++)
                    {
                        m_random[x][y] = new float[m_dustFieldCountInDirection];

                        for (int z = 0; z < m_random.Length; z++)
                        {
                            m_random[x][y][z] = MyMwcUtils.GetRandomFloat(0, 1);
                        }
                    }
                }
            }

            MyRender.GetRenderProfiler().StartNextBlock("computations");

            //  If sun wind is active, we make particle dust more transparent
            float alphaBecauseSunWind = (MySunWind.IsActive == true) ? MySunWind.GetParticleDustFieldAlpha() : 1;

            Vector3 center = MyCamera.Position;
            //Vector3 center = MySession.PlayerShip.GetPosition();

            MyMwcVector3Int cameraCoord = GetMetersToDustFieldCoord(ref center);
            MyMwcVector3Int minCoord = new MyMwcVector3Int(cameraCoord.X - (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf, cameraCoord.Y - (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf, cameraCoord.Z - (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf);
            MyMwcVector3Int maxCoord = new MyMwcVector3Int(cameraCoord.X + (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf, cameraCoord.Y + (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf, cameraCoord.Z + (int)MySector.ParticleDustProperties.DustFieldCountInDirectionHalf);

            //  Update helper frustum and then its bounding box
            //  Bounding frustum is based on camer's bounding frustun, but far plane isn't in such distance, because then bounding box is too large
            m_helperBoundingFrustum.Matrix = MyCamera.ViewMatrix * m_helperProjectionMatrix;
            BoundingBox helperBoundingBox = BoundingBoxHelper.InitialBox;
            //BoundingBoxHelper.AddFrustum(ref m_helperBoundingFrustum, ref helperBoundingBox);
            BoundingBoxHelper.AddSphere(new BoundingSphere(MyCamera.Position, 1000), ref helperBoundingBox);
            MyMwcVector3Int frustumBoundingBoxMinCoord = GetMetersToDustFieldCoord(ref helperBoundingBox.Min);
            MyMwcVector3Int frustumBoundingBoxMaxCoord = GetMetersToDustFieldCoord(ref helperBoundingBox.Max);
            //MyMwcVector3Int frustumBoundingBoxMinCoord = GetMetersToDustFieldCoord(ref MyCamera.BoundingBox.Min);
            //MyMwcVector3Int frustumBoundingBoxMaxCoord = GetMetersToDustFieldCoord(ref MyCamera.BoundingBox.Max);

            //  This is fix for particles that will be near the frustum boundary (or on its other side, but still should be visible)
            //  Think about it like this: particle is defined by its center, but it overlaps spherical area, so we are interested
            //  in particles that are outisde of the frustum (but near it)
            frustumBoundingBoxMinCoord.X--;
            frustumBoundingBoxMinCoord.Y--;
            frustumBoundingBoxMinCoord.Z--;
            frustumBoundingBoxMaxCoord.X++;
            frustumBoundingBoxMaxCoord.Y++;
            frustumBoundingBoxMaxCoord.Z++;

            //  Fix min/max coordinates, so only billboards in frustum are traversed and drawn
            if (minCoord.X < frustumBoundingBoxMinCoord.X) minCoord.X = frustumBoundingBoxMinCoord.X;
            if (minCoord.Y < frustumBoundingBoxMinCoord.Y) minCoord.Y = frustumBoundingBoxMinCoord.Y;
            if (minCoord.Z < frustumBoundingBoxMinCoord.Z) minCoord.Z = frustumBoundingBoxMinCoord.Z;
            if (maxCoord.X > frustumBoundingBoxMaxCoord.X) maxCoord.X = frustumBoundingBoxMaxCoord.X;
            if (maxCoord.Y > frustumBoundingBoxMaxCoord.Y) maxCoord.Y = frustumBoundingBoxMaxCoord.Y;
            if (maxCoord.Z > frustumBoundingBoxMaxCoord.Z) maxCoord.Z = frustumBoundingBoxMaxCoord.Z;

            Matrix rotationMatrix = Matrix.CreateRotationY(animXSpeed);
            animXSpeed += MySector.ParticleDustProperties.AnimSpeed;

            MyRender.GetRenderProfiler().StartNextBlock("for for for + draw");

            MyMwcVector3Int tempCoord;
            for (tempCoord.X = minCoord.X; tempCoord.X <= maxCoord.X; tempCoord.X++)
            {
                for (tempCoord.Y = minCoord.Y; tempCoord.Y <= maxCoord.Y; tempCoord.Y++)
                {
                    for (tempCoord.Z = minCoord.Z; tempCoord.Z <= maxCoord.Z; tempCoord.Z++)
                    {
                        //  Position of this particle
                        Vector3 position;
                        position.X = tempCoord.X * MySector.ParticleDustProperties.DistanceBetween;
                        position.Y = tempCoord.Y * MySector.ParticleDustProperties.DistanceBetween;
                        position.Z = tempCoord.Z * MySector.ParticleDustProperties.DistanceBetween;

                        //  Get pseudo-random number. It's randomness is based on 3D position, so values don't change between draw calls.
                        float pseudoRandomVariationMod = m_random[Math.Abs(tempCoord.X) % m_random.Length][Math.Abs(tempCoord.Y) % m_random.Length][Math.Abs(tempCoord.Z) % m_random.Length];

                        //  Alter position by randomness
                        position.X += MathHelper.Lerp(-m_distanceBetweenHalf, +m_distanceBetweenHalf, pseudoRandomVariationMod);
                        position.Y += MathHelper.Lerp(-m_distanceBetweenHalf, +m_distanceBetweenHalf, pseudoRandomVariationMod);
                        position.Z += MathHelper.Lerp(-m_distanceBetweenHalf, +m_distanceBetweenHalf, pseudoRandomVariationMod);

                        //  Distance to particle
                        float distance;
                        Vector3.Distance(ref center, ref position, out distance);


                        Vector3 delta = position - MyCamera.Position;

                        // delta = Vector3.Transform(delta, rotationMatrix);
                        position = MyCamera.Position + delta;

                        //  Pseudo-random color and alpha
                        float pseudoRandomColor = MathHelper.Lerp(0.1f, 0.2f, pseudoRandomVariationMod); //MathHelper.Lerp(0.2f, 0.3f, pseudoRandomVariationMod);
                        //float pseudoRandomAlpha = 0.5f; //0.4f;  // 0.2f;// MathHelper.Lerp(0.2f, 0.3f, pseudoRandomVariationMod);

                        //  Dust color
                        var sectorDustColor = CustomColor.HasValue ? CustomColor.Value : MySector.ParticleDustProperties.Color;

                        //if (MyGuiScreenGamePlay.Static.ResultDustColor != Vector4.Zero)
                        //{
                        //    sectorDustColor = MyGuiScreenGamePlay.Static.ResultDustColor;
                        //}

                        Vector4 color = sectorDustColor.ToVector4();
                        /*              
                    Vector4 color = new Vector4(
                        pseudoRandomColor * sectorDustColor.X,
                        pseudoRandomColor * sectorDustColor.Y,
                        pseudoRandomColor * sectorDustColor.Z,
                        sectorDustColor.W);
                             */
                        //color = Vector4.One; new Vector4(0.0f, 0.0f, 0.0f, 1.0f);

                        //color = new Vector4(MinerWars.AppCode.Game.World.MySector.FogProperties.FogColor.X, MinerWars.AppCode.Game.World.MySector.FogProperties.FogColor.Y, MinerWars.AppCode.Game.World.MySector.FogProperties.FogColor.Z, 1);

                        //  Color+Alpha based on distance to camera (we use pre-multiplied alpha)
                        float maxDistance = MySector.ParticleDustProperties.DustFieldCountInDirectionHalf * MySector.ParticleDustProperties.DistanceBetween;
                        float DistanceAlpha1 = 0.7f * maxDistance;
                        float DistanceAlpha2 = 0.85f * maxDistance;
                        float DistanceAlpha3 = 1.0f * maxDistance;
                        if (distance < DistanceAlpha1)
                        {
                            color *= 0;
                        }
                        else if ((distance >= DistanceAlpha1) && (distance < DistanceAlpha2))
                        {
                            color *= MathHelper.Clamp((distance - DistanceAlpha1) / (DistanceAlpha2 - DistanceAlpha1), 0, 1);
                        }
                        else if ((distance >= DistanceAlpha2) && (distance < DistanceAlpha3))
                        {
                            color *= 1 - MathHelper.Clamp((distance - DistanceAlpha2) / (DistanceAlpha3 - DistanceAlpha2), 0, 1);
                        }
                        else
                        {
                            color *= 0;
                        }

                        //  Sun wind influence
                        color *= alphaBecauseSunWind;

                        //  Do not draw totaly transparent particles
                        if ((color.X <= 0) && (color.Y <= 0) && (color.Z <= 0) && (color.W <= 0))
                            continue;

                        //if (color.W <= 0) continue;

                        //  Radius
                        //float radius = DUST_BILLBOARD_RADIUS;
                        //float radius = MathHelper.Lerp(1, 30, pseudoRandomVariationMod);
                        float radius = MySector.ParticleDustProperties.DustBillboardRadius;// MathHelper.Lerp(100, 200, pseudoRandomVariationMod);

                        //  Angle - see comments, I tried to do some rotation based on time
                        //angle += pseudoRandomVariationMod * ((MyMinerGame.TotalGamePlayTimeInMilliseconds % 10000.0f) / 1000.0f);
                        //angle += pseudoRandomVariationMod * (MyMinerGame.TotalGamePlayTimeInMilliseconds / 10000.0f);
                        float angle = pseudoRandomVariationMod + animXSpeed;

                        color *= 2;

                        //color = Vector4.One;

                        //MyTransparentGeometry.AddPointBillboard(MyTransparentMaterialEnum.Stardust, color, position, radius, angle);
                        MyTransparentGeometry.AddPointBillboard(MySector.ParticleDustProperties.Texture, color, position, radius, angle, 0, false, false, true);
                    }
                }
            }

            MyRender.GetRenderProfiler().EndProfilingBlock();
        }
    }
}
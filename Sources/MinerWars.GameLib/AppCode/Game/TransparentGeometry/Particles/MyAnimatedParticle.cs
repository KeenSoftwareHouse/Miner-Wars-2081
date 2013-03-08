using System;
using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Runtime.InteropServices;
using MinerWars.AppCode.Game.Render;

//  This is unified particle class. It can be used for point/billboard particles, or for line/polyline particles.
//  Reason I put it into one class is that their are similar but most important is, that I need to have only
//  one preallocated list of particles. Ofcourse I will waste a lot of value per particle by storing parameters I
//  don't need for that particular particle, but at the end, having two or three buffers can be bigger wasting.

namespace MinerWars.AppCode.Game.TransparentGeometry.Particles
{
    public enum MyParticleTypeEnum : byte
    {
        Point = 0,
        Line = 1, 
        Trail = 2,
    }


    class MyAnimatedParticle
    {
        [System.Flags]
        public enum ParticleFlags : byte
        {
            BlendTextures = 1 << 0,
            IsInFrustum =   1 << 1
        }

        public object Tag;

        float m_elapsedTime;  //secs
        

        MyParticleGeneration m_generation;

        public MyParticleTypeEnum Type;

        public MyQuad Quad = new MyQuad();

        //Start values
        public Vector3 StartPosition;
        public Vector3 Velocity;
        public float Life;  //secs
        public float Angle;
        public float RotationSpeed;
        public float Thickness;
        public ParticleFlags Flags;

        //Per life values
        public MyAnimatedPropertyFloat Radius = new MyAnimatedPropertyFloat();
        public MyAnimatedPropertyVector4 Color = new MyAnimatedPropertyVector4();
        public MyAnimatedPropertyInt Material = new MyAnimatedPropertyInt();

        Vector3 m_actualPosition;
        float m_actualAngle;

        float m_elapsedTimeDivider;
        float m_normalizedTime;

        //  Parameter-less constructor - because particles are stored in object pool
        public MyAnimatedParticle()
        {
            //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
            //  So don't initialize members here, do it in Start()
        }

        public void Start(MyParticleGeneration generation)
        {
            System.Diagnostics.Debug.Assert(Life > 0);

            m_elapsedTime = 0;
            m_normalizedTime = 0.0f;
            m_elapsedTimeDivider = MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS / Life;
            m_generation = generation;

            MyUtils.AssertIsValid(StartPosition);
            MyUtils.AssertIsValid(Angle);
            MyUtils.AssertIsValid(Velocity);
            MyUtils.AssertIsValid(RotationSpeed);            

            m_actualPosition = StartPosition;
            m_actualAngle = Angle;
        }

        public bool Update()
        {
            m_elapsedTime += MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            if (m_elapsedTime >= Life)
                return false;

            m_normalizedTime += m_elapsedTimeDivider;

            m_actualPosition.X += Velocity.X * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_actualPosition.Y += Velocity.Y * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;
            m_actualPosition.Z += Velocity.Z * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            m_actualAngle += RotationSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS;

            MyUtils.AssertIsValid(m_actualPosition);
            MyUtils.AssertIsValid(m_actualAngle);

            return true;
        }



        //  Update position, check collisions, etc. and draw if particle still lives.
        //  Return false if particle dies/timeouts in this tick.
        public bool Draw(MyBillboard billboard)
        {
            
            MyTransparentGeometry.StartParticleProfilingBlock("Distance calculation");
            //  This time is scaled according to planned lifespan of the particle

            // Distance for sorting
            Vector3 campos = MyCamera.Position;
            Vector3.DistanceSquared(ref campos, ref m_actualPosition, out billboard.DistanceSquared);

            MyTransparentGeometry.EndParticleProfilingBlock();

            // If distance to camera is really small don't draw it.
            if (billboard.DistanceSquared <= 1)
            {
                return false;
            }

            MyTransparentGeometry.StartParticleProfilingBlock("Quad calculation");

            MyTransparentGeometry.StartParticleProfilingBlock("actualRadius");
            float actualRadius = 1;
            Radius.GetInterpolatedValue<float>(m_normalizedTime, out actualRadius);
            MyTransparentGeometry.EndParticleProfilingBlock();

            billboard.ContainedBillboards.Clear();

            billboard.Near = m_generation.GetEffect().Near;
            billboard.Lowres = m_generation.GetEffect().LowRes || MyRenderConstants.RenderQualityProfile.LowResParticles;

            float alpha = 1;

            if (Type == MyParticleTypeEnum.Point)
            {
                MyTransparentGeometry.StartParticleProfilingBlock("GetBillboardQuadRotated");
                MyUtils.GetBillboardQuadRotated(billboard, ref m_actualPosition, actualRadius, m_actualAngle);
                MyTransparentGeometry.EndParticleProfilingBlock();
            }
            else if (Type == MyParticleTypeEnum.Line)
            {
                if (MyMwcUtils.IsZero(Velocity.LengthSquared()))
                    Velocity = MyMwcUtils.GetRandomVector3Normalized();

                MyQuad quad = new MyQuad();

                MyPolyLine polyLine = new MyPolyLine();
                polyLine.LineDirectionNormalized = MyMwcUtils.Normalize(Velocity);

                if (m_actualAngle > 0)
                {
                    polyLine.LineDirectionNormalized = Vector3.TransformNormal(polyLine.LineDirectionNormalized, Matrix.CreateRotationY(MathHelper.ToRadians(m_actualAngle)));
                }

                polyLine.Point0 = m_actualPosition;
                polyLine.Point1.X = m_actualPosition.X + polyLine.LineDirectionNormalized.X * actualRadius;
                polyLine.Point1.Y = m_actualPosition.Y + polyLine.LineDirectionNormalized.Y * actualRadius;
                polyLine.Point1.Z = m_actualPosition.Z + polyLine.LineDirectionNormalized.Z * actualRadius;

                if (m_actualAngle > 0)
                { //centerize
                    polyLine.Point0.X = polyLine.Point0.X - polyLine.LineDirectionNormalized.X * actualRadius * 0.5f;
                    polyLine.Point0.Y = polyLine.Point0.Y - polyLine.LineDirectionNormalized.Y * actualRadius * 0.5f;
                    polyLine.Point0.Z = polyLine.Point0.Z - polyLine.LineDirectionNormalized.Z * actualRadius * 0.5f;
                    polyLine.Point1.X = polyLine.Point1.X - polyLine.LineDirectionNormalized.X * actualRadius * 0.5f;
                    polyLine.Point1.Y = polyLine.Point1.Y - polyLine.LineDirectionNormalized.Y * actualRadius * 0.5f;
                    polyLine.Point1.Z = polyLine.Point1.Z - polyLine.LineDirectionNormalized.Z * actualRadius * 0.5f;
                }

                polyLine.Thickness = Thickness;
                MyUtils.GetPolyLineQuad(out quad, ref polyLine);

                if (this.m_generation.AlphaAnisotropic)
                {
                    float angle = 1 - Math.Abs(Vector3.Dot(MyMwcUtils.Normalize(MyCamera.ForwardVector), polyLine.LineDirectionNormalized));
                    float alphaCone = (float)Math.Pow(angle, 0.5f);
                    alpha = alphaCone;
                }

                billboard.Position0 = quad.Point0;
                billboard.Position1 = quad.Point1;
                billboard.Position2 = quad.Point2;
                billboard.Position3 = quad.Point3;
            }
            else if (Type == MyParticleTypeEnum.Trail)
            {
                if (Quad.Point0 == Quad.Point2) //not moving particle
                    return false;
                if (Quad.Point1 == Quad.Point3) //not moving particle was previous one
                    return false;
                if (Quad.Point0 == Quad.Point3) //not moving particle was previous one
                    return false;

                billboard.Position0 = Quad.Point0;
                billboard.Position1 = Quad.Point1;
                billboard.Position2 = Quad.Point2;
                billboard.Position3 = Quad.Point3;

                //if (this.m_generation.AlphaAnisotropic)
             /*   { //Trails are anisotropic by default (nobody wants them to see ugly)
                    Vector3 lineDir = Vector3.Normalize(Quad.Point1 - Quad.Point0);
                    float angle = 1 - Math.Abs(Vector3.Dot(MyMwcUtils.Normalize(MyCamera.ForwardVector), lineDir));
                    float alphaCone = (float)Math.Pow(angle, 0.3f);
                    alpha = alphaCone;
                }*/
            }
            else
            {
                throw new NotSupportedException(Type + " is not supported particle type");
            }

            MyTransparentGeometry.EndParticleProfilingBlock();

            MyTransparentGeometry.StartParticleProfilingBlock("Material calculation");

            Vector4 color;
            Color.GetInterpolatedValue<Vector4>(m_normalizedTime, out color);

            int material1 = (int)MyTransparentMaterialEnum.Test;
            int material2 = (int)MyTransparentMaterialEnum.Test;
            float textureBlendRatio = 0;
            if ((Flags & ParticleFlags.BlendTextures) != 0)
            {
                float prevTime, nextTime, difference;
                Material.GetPreviousValue(m_normalizedTime, out material1, out prevTime);
                Material.GetNextValue(m_normalizedTime, out material2, out nextTime, out difference);

                if (prevTime != nextTime)
                    textureBlendRatio = (m_normalizedTime - prevTime) * difference;
            }
            else
            {
                Material.GetInterpolatedValue<int>(m_normalizedTime, out material1);
            }

            MyTransparentGeometry.EndParticleProfilingBlock();
                     
            //This gets 0.44ms for 2000 particles
            MyTransparentGeometry.StartParticleProfilingBlock("billboard.Start");


            billboard.MaterialEnum = (MyTransparentMaterialEnum)material1;
            billboard.BlendMaterial = (MyTransparentMaterialEnum)material2;
            billboard.BlendTextureRatio = textureBlendRatio;
            billboard.EnableColorize = false;

            billboard.Color = color * alpha * m_generation.GetEffect().UserColorMultiplier;

            MyTransparentGeometry.EndParticleProfilingBlock();

            return true;
        }

        public void AddMotionInheritance(ref float motionInheritance, ref Matrix deltaMatrix)
        {
            Vector3 newPosition = Vector3.Transform(m_actualPosition, deltaMatrix);

            m_actualPosition = m_actualPosition + (newPosition - m_actualPosition) * motionInheritance;

            Velocity = Vector3.TransformNormal(Velocity, deltaMatrix);
        }

        public Vector3 ActualPosition
        {
            get { return m_actualPosition; }
        }
    }
}
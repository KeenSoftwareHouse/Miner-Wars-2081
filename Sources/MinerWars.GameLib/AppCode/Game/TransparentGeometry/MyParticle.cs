using MinerWarsMath;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using KeenSoftwareHouse.Library.Trace;
using System;

//  This is unified particle class. It can be used for point/billboard particles, or for line/polyline particles.
//  Reason I put it into one class is that their are similar but most important is, that I need to have only
//  one preallocated list of particles. Ofcourse I will waste a lot of value per particle by storing parameters I
//  don't need for that particular particle, but at the end, having two or three buffers can be bigger wasting.

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    enum MyParticleType : byte
    {
        POINT_PARTICLE,
        LINE_PARTICLE,
        POINT_PARTICLE_RELATIVE_TO_PHYS_OBJECT,
        LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT_COCKPIT_GLASS,
        LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT
    }

    class MyParticle
    {
        //  Parameters common for all type of particles
        int m_lifespanInMiliseconds;            //  This is the time particle will live (in miliseconds)
        Vector4 m_startColor;
        Vector4 m_endColor;
        MyParticleType m_type;
        MyTransparentMaterialEnum m_materialEnum;
        Vector3 m_velocity;
        Vector3 m_startPosition;
        int m_timeStarted;
        
        //  Parameters specific for point-particles
        float m_pointSpecific_startAngle;
        float m_pointSpecific_rotationSpeed;
        float m_pointSpecific_startRadius;
        float m_pointSpecific_endRadius;

        //  Parameters specific for line-particles
        Vector3 m_lineSpecific_directionNormalized;
        float m_lineSpecific_thickness;
        float m_lineSpecific_startLength;
        float m_lineSpecific_endLength;

        //  Parameters specific for 'relative to phys object' particles (line or point)
        MyEntity m_physObject;


        //  Parameter-less constructor - because particles are stored in object pool
        public MyParticle()
        {
            //  IMPORTANT: This class isn't realy inicialized by constructor, but by Start()
            //  So don't initialize members here, do it in Start()
        }

        //  This method initializes parameters common for all type of particle
        void Start(MyTransparentMaterialEnum material, int lifespanInMiliseconds,
            Vector3 startPosition, Vector3 velocity, Vector4 startColor, Vector4 endColor)
        {
            m_timeStarted = MyMinerGame.TotalGamePlayTimeInMilliseconds;
            m_lifespanInMiliseconds = lifespanInMiliseconds;
            m_startPosition = startPosition;
            m_velocity = velocity;
            m_startColor = startColor;
            m_endColor = endColor;
            m_materialEnum = material;
        }

        //  This method realy initiates/starts the particle
        //  IMPORTANT: Direction vector must be normalized!
        public void StartPointParticle(
            MyTransparentMaterialEnum material, int lifespanInMiliseconds, Vector3 startPosition, Vector3 velocity, 
            Vector4 startColor, Vector4 endColor, float startAngle, float startRadius, float endRadius, float rotationSpeed)
        {
            Start(material, lifespanInMiliseconds, startPosition, velocity, startColor, endColor);

            m_type = MyParticleType.POINT_PARTICLE;
            m_pointSpecific_startAngle = startAngle;
            m_pointSpecific_startRadius = startRadius;
            m_pointSpecific_endRadius = endRadius;
            m_pointSpecific_rotationSpeed = rotationSpeed;
        }
        
        //  This method realy initiates/starts the particle
        //  IMPORTANT: Direction vector must be normalized!
        public void StartPointParticleRelativeToPhysObject(
            MyTransparentMaterialEnum material, int lifespanInMiliseconds, Vector3 startPosition, Vector3 velocity, 
            Vector4 startColor, Vector4 endColor, float startAngle, float startRadius, float endRadius, float rotationSpeed, 
            MyEntity physObject)
        {
            Start(material, lifespanInMiliseconds, startPosition, velocity, startColor, endColor);

            m_type = MyParticleType.POINT_PARTICLE_RELATIVE_TO_PHYS_OBJECT;
            m_pointSpecific_startAngle = startAngle;
            m_pointSpecific_startRadius = startRadius;
            m_pointSpecific_endRadius = endRadius;
            m_pointSpecific_rotationSpeed = rotationSpeed;

            m_physObject = physObject;
        }

        //  This method realy initiates/starts the particle
        //  IMPORTANT: Direction vector must be normalized!
        public void StartLineParticle(
            MyTransparentMaterialEnum material, int lifespanInMiliseconds, Vector3 startPosition, Vector3 velocity, 
            Vector4 startColor, Vector4 endColor, Vector3 directionNormalized, float thickness, float startLength, float endLength)
        {
            Start(material, lifespanInMiliseconds, startPosition, velocity, startColor, endColor);

            m_type = MyParticleType.LINE_PARTICLE;
            m_lineSpecific_directionNormalized = directionNormalized;
            m_lineSpecific_thickness = thickness;
            m_lineSpecific_startLength = startLength;
            m_lineSpecific_endLength = endLength;
        }

        //  This method realy initiates/starts the particle
        //  IMPORTANT: Direction vector must be normalized!
        public void StartLineParticleRelativeToPhysObjectCockpitGlass(
            MyTransparentMaterialEnum material, int lifespanInMiliseconds, Vector3 startPosition, Vector3 velocity, 
            Vector4 startColor, Vector4 endColor, Vector3 directionNormalized, float thickness, float startLength, float endLength,  
            MyEntity physObject)
        {
            Start(material, lifespanInMiliseconds, startPosition, velocity, startColor, endColor);

            m_type = MyParticleType.LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT_COCKPIT_GLASS;
            m_lineSpecific_directionNormalized = directionNormalized;
            m_lineSpecific_thickness = thickness;
            m_lineSpecific_startLength = startLength;
            m_lineSpecific_endLength = endLength;
            m_physObject = physObject;
        }

        //  This method realy initiates/starts the particle
        //  IMPORTANT: Direction vector must be normalized!
        public void StartLineParticleRelativeToPhysObject(
            MyTransparentMaterialEnum material, int lifespanInMiliseconds, Vector3 startPosition, Vector3 velocity,
            Vector4 startColor, Vector4 endColor, Vector3 directionNormalized, float thickness, float startLength, float endLength,  
            MyEntity physObject)
        {
            Start(material, lifespanInMiliseconds, startPosition, velocity, startColor, endColor);

            m_type = MyParticleType.LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT;
            m_lineSpecific_directionNormalized = directionNormalized;
            m_lineSpecific_thickness = thickness;
            m_lineSpecific_startLength = startLength;
            m_lineSpecific_endLength = endLength;
            m_physObject = physObject;
        }

        public MyParticleType GetParticleType()
        {
            return m_type;
        }

        //  Update position, check collisions, etc. and draw if particle still lives.
        //  Return false if particle dies/timeouts in this tick.
        public bool Draw(MyBillboard billboard)
        {
            //  Check for timeout
            int elapsedMiliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds - m_timeStarted;
            if (elapsedMiliseconds >= m_lifespanInMiliseconds) return false;

            //  This time is scaled according to planned lifespan of the particle
            float normalizedTimeElapsed = (float)elapsedMiliseconds / (float)m_lifespanInMiliseconds;

            MyQuad quad = new MyQuad();
            Vector3 actualPosition;

            if (m_type == MyParticleType.LINE_PARTICLE)
            {
                actualPosition = m_startPosition;
                actualPosition += m_velocity * ((float)elapsedMiliseconds / 1000.0f);
            }
            else if (m_type == MyParticleType.LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT)
            {
                Matrix worldMatrix = m_physObject.WorldMatrix;
                actualPosition = MyUtils.GetTransform(ref m_startPosition, ref worldMatrix);
                actualPosition += m_velocity * ((float)elapsedMiliseconds / 1000.0f);
            }
            else if (m_type == MyParticleType.LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT_COCKPIT_GLASS)
            {
                actualPosition = MyUtils.GetTransform(ref m_startPosition, ref ((MySmallShip)m_physObject).PlayerHeadForCockpitInteriorWorldMatrix);
                actualPosition += m_velocity * ((float)elapsedMiliseconds / 1000.0f);
            }            
            else if (m_type == MyParticleType.POINT_PARTICLE)
            {
                actualPosition = m_startPosition + m_velocity * ((float)elapsedMiliseconds / 1000.0f);
            }
            else if (m_type == MyParticleType.POINT_PARTICLE_RELATIVE_TO_PHYS_OBJECT)
            {
                Matrix worldMatrix = m_physObject.WorldMatrix;
                actualPosition = MyUtils.GetTransform(ref m_startPosition, ref worldMatrix);
                actualPosition += m_velocity * ((float)elapsedMiliseconds / 1000.0f);
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            // Distance for sorting
            Vector3 campos = MyCamera.Position;
            Vector3.DistanceSquared(ref campos, ref actualPosition, out billboard.DistanceSquared);

            // If distance to camera is really small don't draw it.
            if (billboard.DistanceSquared <= MyMwcMathConstants.EPSILON)
            {
                return true;
            }

            if ((m_type == MyParticleType.LINE_PARTICLE) ||
                (m_type == MyParticleType.LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT) ||
                (m_type == MyParticleType.LINE_PARTICLE_RELATIVE_TO_PHYS_OBJECT_COCKPIT_GLASS))
            {
                float actualLength = MathHelper.Lerp(m_lineSpecific_startLength, m_lineSpecific_endLength, normalizedTimeElapsed);

                MyPolyLine polyLine;
                polyLine.LineDirectionNormalized = m_lineSpecific_directionNormalized;
                polyLine.Point0 = actualPosition;
                polyLine.Point1 = actualPosition + polyLine.LineDirectionNormalized * actualLength;
                polyLine.Thickness = m_lineSpecific_thickness;

                //  Billboard vertexes
                MyUtils.GetPolyLineQuad(out quad, ref polyLine);
            }
            else if ((m_type == MyParticleType.POINT_PARTICLE) || (m_type == MyParticleType.POINT_PARTICLE_RELATIVE_TO_PHYS_OBJECT))
            {
                //  Billboard vertexes
                float actualRadius = MathHelper.Lerp(m_pointSpecific_startRadius, m_pointSpecific_endRadius, normalizedTimeElapsed);
                float angle = m_pointSpecific_startAngle + normalizedTimeElapsed * m_pointSpecific_rotationSpeed;
                MyUtils.GetBillboardQuadRotated(billboard, ref actualPosition, actualRadius, angle);
            }
            else
            {
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            //  Color and alpha depend on time
            Vector4 color;
            color.X = MathHelper.Lerp(m_startColor.X, m_endColor.X, normalizedTimeElapsed);
            color.Y = MathHelper.Lerp(m_startColor.Y, m_endColor.Y, normalizedTimeElapsed);
            color.Z = MathHelper.Lerp(m_startColor.Z, m_endColor.Z, normalizedTimeElapsed);
            color.W = MathHelper.Lerp(m_startColor.W, m_endColor.W, normalizedTimeElapsed);
            //billboard.Color.W *= 1.0f - normalizedTimeElapsed;
            //billboard.Color.W *= 1 - (float)Math.Pow(normalizedTimeElapsed, 2);            
            //billboard.Color.W *= normalizedTimeElapsed * (1 - normalizedTimeElapsed) * (1 - normalizedTimeElapsed) * 6.7f;

            billboard.Start(ref quad, m_materialEnum, ref color, ref m_startPosition);

            //  Yes, draw this particle
            return true;
        }
    }
}
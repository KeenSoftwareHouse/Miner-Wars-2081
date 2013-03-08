using System;
using System.Collections.Generic;

using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.AppCode.Game.Models;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;

//using MinerWarsMath;
//using MinerWarsMath.Graphics;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;



namespace MinerWars.AppCode.Game.Utils
{
    using Vector2 = MinerWarsMath.Vector2;
    using Vector3 = MinerWarsMath.Vector3;
    using Vector4 = MinerWarsMath.Vector4;
    using Rectangle = MinerWarsMath.Rectangle;
    using Matrix = MinerWarsMath.Matrix;
    using Color = MinerWarsMath.Color;
    using BoundingBox = MinerWarsMath.BoundingBox;
    using BoundingSphere = MinerWarsMath.BoundingSphere;
    using BoundingFrustum = MinerWarsMath.BoundingFrustum;
    using MathHelper = MinerWarsMath.MathHelper;

    static class MySimpleObjectDraw
    {
        static MyModel m_modelBoxLowRes;
        static MyModel m_modelBoxHiRes;
        static MyModel m_modelSphere;
        static MyModel m_modelLightSphere;
        static MyModel m_modelCone;
        static MyModel m_modelHemisphere;
        static MyModel m_modelHemisphereLowRes;
        static MyModel m_modelCapsule;

        private static List<MyLine> m_lineBuffer = new List<MyLine>(2000);   //max capacity of rendered lines        


        private static List<Vector3> m_verticesBuffer = new List<Vector3>(2000);   //max capacity of rendered lines



        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MySimpleObjectDraw.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            m_modelBoxHiRes = MyModels.GetModelForDraw(MyModelsEnum.BoxHiRes);
            m_modelBoxLowRes = MyModels.GetModelForDraw(MyModelsEnum.BoxLowRes);
            m_modelSphere = MyModels.GetModelForDraw(MyModelsEnum.Sphere);
            m_modelLightSphere = MyModels.GetModelForDraw(MyModelsEnum.Sphere_low);
            m_modelCone = MyModels.GetModelForDraw(MyModelsEnum.Cone);
            m_modelHemisphere = MyModels.GetModelForDraw(MyModelsEnum.Hemisphere);
            m_modelHemisphereLowRes = MyModels.GetModelForDraw(MyModelsEnum.Hemisphere_low);
            m_modelCapsule = MyModels.GetModelForDraw(MyModelsEnum.Capsule); 

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySimpleObjectDraw.LoadContent() - END");
        }

        public static void UnloadContent()
        {

        }

        /// <summary>
        /// Draw occlusion bounding box method with our premade effect and box.
        /// </summary>
        /// <param name="bbox"></param>
        /// <param name="scale"></param>
        /// <param name="enableDepthTesting"></param>
        /// <param name="billboardLike">Indicates whether the occlusion object (box) is rotated to face the camera or not.</param>
        public static void DrawOcclusionBoundingBox(BoundingBox bbox, float scale, bool enableDepthTesting, bool billboardLike = false, bool useDepthTarget = true)
        {
            useDepthTarget &= !MyRenderConstants.RenderQualityProfile.ForwardRender;

            var cameraToBBox = bbox.GetCenter() - MyCamera.Position;
            Matrix worldMatrix = billboardLike ? Matrix.CreateWorld(Vector3.Zero, MyMwcUtils.Normalize(cameraToBBox), MyMwcUtils.Normalize(MyCamera.UpVector + MyCamera.LeftVector)) : Matrix.Identity;
            
            Vector3 scaleV = (bbox.Max - bbox.Min) * scale;
            worldMatrix *= Matrix.CreateScale(scaleV);
            worldMatrix.Translation = cameraToBBox;

                  
            
            MyEffectOcclusionQueryDraw effectOQ = MyRender.GetEffect(MyEffects.OcclusionQueryDrawMRT) as MyEffectOcclusionQueryDraw;

            if (enableDepthTesting && !MyRenderConstants.RenderQualityProfile.ForwardRender)
                effectOQ.SetTechnique(MyEffectOcclusionQueryDraw.Technique.DepthTestEnabled);
            else
                effectOQ.SetTechnique(MyEffectOcclusionQueryDraw.Technique.DepthTestDisabled);
            

            effectOQ.SetWorldMatrix(worldMatrix);
            effectOQ.SetViewMatrix(MyCamera.ViewMatrixAtZero);
            effectOQ.SetProjectionMatrix(MyCamera.ProjectionMatrix);

            if (useDepthTarget)
            {
                var depthRenderTarget = MyRender.GetRenderTarget(MyRenderTargets.Depth);
                effectOQ.SetDepthRT(depthRenderTarget);
                effectOQ.SetScale(MyRender.GetScaleForViewport(depthRenderTarget));
            }

            effectOQ.Begin();

            //draw
            m_modelBoxLowRes.Render();

            effectOQ.End();
        }

        /// <summary>
        /// Only to be called with FastOcclusionBoundingBoxDraw
        /// </summary>
        public static void PrepareFastOcclusionBoundingBoxDraw()
        {
            MyEffectOcclusionQueryDraw effectOQ = MyRender.GetEffect(MyEffects.OcclusionQueryDrawMRT) as MyEffectOcclusionQueryDraw;
            effectOQ.SetTechnique(MyEffectOcclusionQueryDraw.Technique.DepthTestEnabled);

            effectOQ.SetViewMatrix(MyCamera.ViewMatrixAtZero);
            effectOQ.SetProjectionMatrix(MyCamera.ProjectionMatrix);

            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                var depthRenderTarget = MyRender.GetRenderTarget(MyRenderTargets.Depth);
                effectOQ.SetDepthRT(depthRenderTarget);
                effectOQ.SetScale(MyRender.GetScaleForViewport(depthRenderTarget));
            }
        }

        // Vertices must be in triangle strip order
        // 0--1
        // | /|
        // |/ |
        // 2--3
        public static void OcclusionPlaneDraw(Vector3[] quad)
        {
            Matrix worldMatrix = Matrix.Identity;
            worldMatrix.Translation = -MyCamera.Position;

            MyEffectOcclusionQueryDraw effectOQ = MyRender.GetEffect(MyEffects.OcclusionQueryDrawMRT) as MyEffectOcclusionQueryDraw;
            effectOQ.SetWorldMatrix(worldMatrix);
            effectOQ.SetViewMatrix(MyCamera.ViewMatrixAtZero);
            effectOQ.SetProjectionMatrix(MyCamera.ProjectionMatrix);

            if (!MyRenderConstants.RenderQualityProfile.ForwardRender)
            {
                var depthRenderTarget = MyRender.GetRenderTarget(MyRenderTargets.Depth);
                effectOQ.SetDepthRT(depthRenderTarget);
                effectOQ.SetScale(MyRender.GetScaleForViewport(depthRenderTarget));
                effectOQ.SetTechnique(MyEffectOcclusionQueryDraw.Technique.DepthTestEnabledNonMRT);
            }
            else
            {
                effectOQ.SetTechnique(MyEffectOcclusionQueryDraw.Technique.DepthTestDisabledNonMRT);
            }
        }

        /// <summary>
        /// Needs to have PrepareFastOcclusionBoundingBoxDraw() called first
        /// </summary>
        public static void FastOcclusionBoundingBoxDraw(BoundingBox bbox, float scale)
        {
            Vector3 scaleV = (bbox.Max - bbox.Min) * scale;
            Matrix worldMatrix = Matrix.CreateScale(scaleV);
            worldMatrix.Translation = bbox.GetCenter() - MyCamera.Position;

            MyEffectOcclusionQueryDraw effectOQ = MyRender.GetEffect(MyEffects.OcclusionQueryDrawMRT) as MyEffectOcclusionQueryDraw;
            effectOQ.SetWorldMatrix(worldMatrix);
            effectOQ.SetTechnique(MyRenderConstants.RenderQualityProfile.ForwardRender ? MyEffectOcclusionQueryDraw.Technique.DepthTestDisabled : MyEffectOcclusionQueryDraw.Technique.DepthTestEnabled);

            effectOQ.Begin();

            //draw
            m_modelBoxLowRes.Render();
            MyPerformanceCounter.PerCameraDraw.TotalDrawCalls++;

            effectOQ.End();
        }


        public static void DrawSphereForLight(MyEffectPointLight effect, ref Matrix worldMatrix, ref Vector3 diffuseColor, float alpha)
        {
            Matrix worldViewProjection;
            Matrix.Multiply(ref worldMatrix, ref MyCamera.ViewProjectionMatrix, out worldViewProjection);

            effect.SetWorldViewProjMatrix(ref worldViewProjection);
            effect.SetWorldMatrix(ref worldMatrix);

            effect.Begin();

            m_modelLightSphere.Render();

            effect.End();
        }

        public static void DrawSphereForLight(MyEffectPointLight effect, ref Vector3 position, float radius, ref Vector3 diffuseColor, float alpha)
        {
            Matrix scaleMatrix;
            Matrix.CreateScale(radius, out scaleMatrix);
            Matrix positionMatrix;
            Matrix.CreateTranslation(ref position, out positionMatrix);
            Matrix lightMatrix;
            Matrix.Multiply(ref scaleMatrix, ref positionMatrix, out lightMatrix);

            DrawSphereForLight(effect, ref lightMatrix, ref diffuseColor, alpha);
        }

        public static void DrawHemisphereForLight(MyEffectPointLight effect, ref Matrix worldMatrix, ref Vector3 diffuseColor, float alpha)
        {
            Matrix worldViewProjMatrix;
            Matrix.Multiply(ref worldMatrix, ref MyCamera.ViewProjectionMatrix, out worldViewProjMatrix);

            effect.SetWorldViewProjMatrix(ref worldViewProjMatrix);
            effect.SetWorldMatrix(ref worldMatrix);

            effect.Begin();

            m_modelHemisphereLowRes.Render();

            effect.End();
        }

        public static void DrawHemisphereForLight(MyEffectPointLight effect, ref Vector3 position, float radius, ref Vector3 diffuseColor, float alpha)
        {
            Matrix scaleMatrix;
            Matrix.CreateScale(radius, out scaleMatrix);
            Matrix positionMatrix;
            Matrix.CreateTranslation(ref position, out positionMatrix);
            Matrix lightMatrix;
            Matrix.Multiply(ref scaleMatrix, ref positionMatrix, out lightMatrix);

            DrawHemisphereForLight(effect, ref lightMatrix, ref diffuseColor, alpha);
        }

        public static void DrawConeForLight(MyEffectPointLight effect, Matrix worldMatrix)
        {
            Matrix worldViewProjMatrix;
            Matrix.Multiply(ref worldMatrix, ref MyCamera.ViewProjectionMatrix, out worldViewProjMatrix);

            effect.SetWorldViewProjMatrix(ref worldViewProjMatrix);
            effect.SetWorldMatrix(ref worldMatrix);

            effect.Begin();

            m_modelCone.Render();

            effect.End();
        }

        public static void DrawConeForLight(MyEffectPointLight effect, Vector3 position, Vector3 direction, Vector3 upVector, float coneLength, float coneCosAngle)
        {
            // Cone is oriented backwards
            float scaleZ = -coneLength;

            // Calculate cone side (hypotenuse of triangle)
            float side = coneLength / coneCosAngle;

            // Calculate cone bottom scale (Pythagoras theorem)
            float scaleXY = (float)System.Math.Sqrt(side * side - coneLength * coneLength);

            // Calculate world matrix as scale * light world matrix
            Matrix world = Matrix.CreateScale(scaleXY, scaleXY, scaleZ) * Matrix.CreateWorld(position, direction, upVector);
            DrawConeForLight(effect, world);
        }

        /// <summary>
        /// GenerateLines
        /// </summary>
        /// <param name="vctStart"></param>
        /// <param name="vctEnd"></param>
        /// <param name="vctSideStep"></param>
        /// <param name="worldMatrix"></param>
        /// <param name="m_lineBuffer"></param>
        /// <param name="divideRatio"></param>
        private static void GenerateLines(Vector3 vctStart, Vector3 vctEnd, ref Vector3 vctSideStep, ref Matrix worldMatrix, ref List<MyLine> m_lineBuffer, int divideRatio)
        {
            for (int i = 0; i <= divideRatio; ++i)
            {
                Vector3 transformedStart = Vector3.Transform(vctStart, worldMatrix);
                Vector3 transformedEnd = Vector3.Transform(vctEnd, worldMatrix);

                if (m_lineBuffer.Count < m_lineBuffer.Capacity)
                {
                    MyLine line = new MyLine(transformedStart, transformedEnd, false);
                    //@ generate Line
                    m_lineBuffer.Add(line);

                    vctStart += vctSideStep;
                    vctEnd += vctSideStep;
                }
            }
        }


        /// <summary>
        /// DrawTransparentBox
        /// </summary>
        public static void DrawTransparentBox(ref Matrix worldMatrix, ref BoundingBox localbox, ref Vector4 vctColor, bool bWireFramed, int wireDivideRatio, MyTransparentMaterialEnum? faceMaterial = null, MyTransparentMaterialEnum? lineMaterial = null)
        {
            if (!faceMaterial.HasValue)
            {
                faceMaterial = MyTransparentMaterialEnum.ContainerBorder;
            }

            Vector3 vctMin = localbox.Min;
            Vector3 vctMax = localbox.Max;

            //@ CreateQuads
            Vector3 translation = worldMatrix.Translation;
            MyQuad quad;

            Matrix orientation = Matrix.Identity;
            orientation.Forward = worldMatrix.Forward;
            orientation.Up = worldMatrix.Up;
            orientation.Right = worldMatrix.Right;

            float halfWidth = (localbox.Max.X - localbox.Min.X) / 2f;
            float halfHeight = (localbox.Max.Y - localbox.Min.Y) / 2f;
            float halfDeep = (localbox.Max.Z - localbox.Min.Z) / 2f;

            //@ Front side
            Vector3 faceNorm = Vector3.Transform(Vector3.Forward, orientation);
            faceNorm *= halfDeep;
            Vector3 vctPos = translation + faceNorm;
            MyUtils.GenerateQuad(out quad, ref vctPos, halfWidth, halfHeight, ref worldMatrix);
            MyTransparentGeometry.AddQuad(faceMaterial.Value, ref quad, ref vctColor, ref translation);

            //@ Back side
            vctPos = translation - faceNorm;
            MyUtils.GenerateQuad(out quad, ref vctPos, halfWidth, halfHeight, ref worldMatrix);
            MyTransparentGeometry.AddQuad(faceMaterial.Value, ref quad, ref vctColor, ref translation);

            //@ Left side
            Matrix rotMat = Matrix.CreateRotationY(MathHelper.ToRadians(90f));
            Matrix rotated = rotMat * worldMatrix;
            faceNorm = Vector3.Transform(Vector3.Left, orientation);
            faceNorm *= halfWidth;
            vctPos = translation + faceNorm;
            MyUtils.GenerateQuad(out quad, ref vctPos, halfDeep, halfHeight, ref rotated);
            MyTransparentGeometry.AddQuad(faceMaterial.Value, ref quad, ref vctColor, ref translation);

            //@ Right side
            vctPos = translation - faceNorm;
            MyUtils.GenerateQuad(out quad, ref vctPos, halfDeep, halfHeight, ref rotated);
            MyTransparentGeometry.AddQuad(faceMaterial.Value, ref quad, ref vctColor, ref translation);

            //@ Top side
            rotMat = Matrix.CreateRotationX(MathHelper.ToRadians(90f));
            rotated = rotMat * worldMatrix;
            faceNorm = Vector3.Transform(Vector3.Up, orientation);
            faceNorm *= ((localbox.Max.Y - localbox.Min.Y) / 2f);
            vctPos = translation + faceNorm;
            MyUtils.GenerateQuad(out quad, ref vctPos, halfWidth, halfDeep, ref rotated);
            MyTransparentGeometry.AddQuad(faceMaterial.Value, ref quad, ref vctColor, ref translation);

            //@ Bottom side
            vctPos = translation - faceNorm;
            MyUtils.GenerateQuad(out quad, ref vctPos, halfWidth, halfDeep, ref rotated);
            MyTransparentGeometry.AddQuad(faceMaterial.Value, ref quad, ref vctColor, ref translation);

            if (bWireFramed)
            {
                Vector4 vctWireColor = vctColor;
                vctWireColor *= 1.3f;
                DrawWireFramedBox(ref worldMatrix, ref localbox, ref vctWireColor, 0.02f, wireDivideRatio, lineMaterial);
            }
        }

        /// <summary>
        /// DrawWireFramedBox
        /// </summary>
        /// <param name="worldMatrix"></param>
        /// <param name="localbox"></param>
        /// <param name="vctColor"></param>
        /// <param name="bWireFramed"></param>
        /// <param name="wireDivideRatio"></param>
        /// <param name="wireDivideRatio"></param>
        public static void DrawWireFramedBox(ref Matrix worldMatrix, ref BoundingBox localbox, ref Vector4 vctColor, float fThickRatio, int wireDivideRatio, MyTransparentMaterialEnum? lineMaterial = null)
        {
            if (!lineMaterial.HasValue)
            {
                lineMaterial = MyTransparentMaterialEnum.ProjectileTrailLine;
            }

            m_lineBuffer.Clear();

            //@ generate linnes for Front Side
            Vector3 translation = worldMatrix.Translation;

            Matrix orientation = Matrix.Identity;
            orientation.Forward = worldMatrix.Forward;
            orientation.Up = worldMatrix.Up;
            orientation.Right = worldMatrix.Right;

            float width = System.Math.Max(0.1f, localbox.Max.X - localbox.Min.X);
            float height = System.Math.Max(0.1f, localbox.Max.Y - localbox.Min.Y);
            float deep = System.Math.Max(0.1f, localbox.Max.Z - localbox.Min.Z);

            //@ ForntSide
            Vector3 vctStart = localbox.Min;
            Vector3 vctEnd = vctStart + Vector3.Up * height;
            Vector3 vctSideStep = Vector3.Right * (width / wireDivideRatio);
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
            // BackSide
            vctStart += Vector3.Backward * deep;
            vctEnd = vctStart + Vector3.Up * height;
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);

            //@ FrontSide
            vctStart = localbox.Min;
            vctEnd = vctStart + Vector3.Right * width;
            vctSideStep = Vector3.Up * (height / wireDivideRatio);
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
            //@ BactSide
            vctStart += Vector3.Backward * deep;
            vctEnd += Vector3.Backward * deep;
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);



            Matrix rotMat = Matrix.CreateRotationY(MathHelper.ToRadians(90f));
            Matrix rotated = rotMat * worldMatrix;

            //@ LeftSide
            vctStart = localbox.Min;
            vctEnd = vctStart + Vector3.Backward * deep;
            vctSideStep = Vector3.Up * (height / wireDivideRatio);
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
            // RightSide
            vctStart = localbox.Min;
            vctStart += Vector3.Right * width;
            vctEnd = vctStart + Vector3.Backward * deep;
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);

            //@ LeftSide
            vctStart = localbox.Min;
            vctEnd = vctStart + Vector3.Up * height;
            vctSideStep = Vector3.Backward * (deep / wireDivideRatio);
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
            // RightSide
            vctStart += Vector3.Right * width;
            vctEnd += Vector3.Right * width;
            GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);


            if (wireDivideRatio > 1)
            {
                //@ TopSide
                vctStart = localbox.Min;
                vctEnd = vctStart + Vector3.Right * width;
                vctSideStep = Vector3.Backward * (deep / wireDivideRatio);
                GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
                // BottomSide
                vctStart += Vector3.Up * height;
                vctEnd += Vector3.Up * height;
                GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);

                //@ TopSide
                vctStart = localbox.Min;
                vctEnd = vctStart + Vector3.Backward * deep;
                vctSideStep = Vector3.Right * (width / wireDivideRatio);
                GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
                // BottomSide
                vctStart += Vector3.Up * height;
                vctEnd += Vector3.Up * height;
                GenerateLines(vctStart, vctEnd, ref vctSideStep, ref worldMatrix, ref m_lineBuffer, wireDivideRatio);
            }


            Vector3 size = new Vector3(localbox.Max.X - localbox.Min.X, localbox.Max.Y - localbox.Min.Y, localbox.Max.Z - localbox.Min.Z);
            float thickness = MathHelper.Max(1, MathHelper.Min(MathHelper.Min(size.X, size.Y), size.Z));
            thickness *= fThickRatio;
            //billboard
            foreach (MyLine line in m_lineBuffer)
            {

                //@ 16 - lifespan for 1 update in 60FPS
                MyTransparentGeometry.AddLineBillboard(lineMaterial.Value, vctColor, line.From, line.Direction, line.Length, thickness);
                
            }
        }



        /// <summary>
        /// DrawTransparentSphere
        /// </summary>
        /// <param name="vctPos"></param>
        /// <param name="radius"></param>
        /// <param name="vctColor"></param>
        /// <param name="bWireFramed"></param>
        /// <param name="wireDivideRatio"></param>
        public static void DrawTransparentSphere(ref Matrix worldMatrix, float radius, ref Vector4 vctColor, bool bWireFramed, int wireDivideRatio, MyTransparentMaterialEnum? faceMaterial = null, MyTransparentMaterialEnum? lineMaterial = null)
        {
            if (!lineMaterial.HasValue)
            {
                lineMaterial = MyTransparentMaterialEnum.ProjectileTrailLine;
            }

            m_verticesBuffer.Clear();
            MyMeshHelper.GenerateSphere(ref worldMatrix, radius, wireDivideRatio, m_verticesBuffer);
            Vector3 vctZero = Vector3.Zero;

            float thickness = radius * 0.01f;
            int i = 0;
            for (i = 0; i < m_verticesBuffer.Count; i += 4)
            {
                MyQuad quad;
                quad.Point0 = m_verticesBuffer[i + 1];
                quad.Point1 = m_verticesBuffer[i + 3];
                quad.Point2 = m_verticesBuffer[i + 2];
                quad.Point3 = m_verticesBuffer[i];

                MyTransparentGeometry.AddQuad(faceMaterial ?? MyTransparentMaterialEnum.ContainerBorder, ref quad, ref vctColor, ref vctZero);
                if (bWireFramed)
                {

                    //@ 20 - lifespan for 1 update in 60FPPS
                    Vector3 start = quad.Point0;
                    Vector3 dir = quad.Point1 - start;
                    float len = dir.Length();
                    if (len > 0.1f)
                    {
                        dir = MyMwcUtils.Normalize(dir);

                        MyTransparentGeometry.AddLineBillboard(lineMaterial.Value, vctColor, start, dir,len,thickness);
                    }

                    start = quad.Point1;
                    dir = quad.Point2 - start;
                    len = dir.Length();
                    if (len > 0.1f)
                    {
                        dir = MyMwcUtils.Normalize(dir);

                        MyTransparentGeometry.AddLineBillboard(lineMaterial.Value, vctColor, start, dir, len, thickness);
                    }

                    /*start = quad.Point2;
                    dir = quad.Point3 - start;
                    len = dir.Length();
                    if (len > 0.1f)
                    {
                        dir = MyMwcUtils.Normalize(dir);
                        newParticle3.StartLineParticle(lineMaterial.Value, false, 20, start, Vector3.Zero, vctColor, vctColor, dir, thickness, len, len, false, MyParticlesConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES);
                    }

                    start = quad.Point3;
                    dir = quad.Point1 - start;
                    len = dir.Length();
                    if (len > 0.1f)
                    {
                        dir = MyMwcUtils.Normalize(dir);
                        newParticle4.StartLineParticle(lineMaterial.Value, false, 20, start, Vector3.Zero, vctColor, vctColor, dir, thickness, len, len, false, MyParticlesConstants.SOFT_PARTICLE_DISTANCE_SCALE_FOR_HARD_PARTICLES);
                    }*/
                }
            }
        }

        public static void DrawTransparentCone(ref Matrix worldMatrix, float coneLength, float radiusBackSide, ref Vector4 vctColor, bool bWireFramed, int wireDivideRatio, float thickness, MyTransparentMaterialEnum? lineMaterial = null) 
        {            
            Vector3 vertex;            
            Vector3 start = Vector3.Transform(Vector3.Zero, worldMatrix);

            float angleStep = 360.0f / (float)wireDivideRatio;
            float alpha = 0;

            for (int i = 0; i <= wireDivideRatio; i++) 
            {   
                alpha = (float)i * angleStep;

                vertex.X = (float)(radiusBackSide * Math.Cos(MathHelper.ToRadians(alpha)));
                vertex.Y = (float)(radiusBackSide * Math.Sin(MathHelper.ToRadians(alpha)));
                vertex.Z = coneLength;
                vertex = Vector3.Transform(vertex, worldMatrix);

                DrawLine(start, vertex, lineMaterial ?? MyTransparentMaterialEnum.ProjectileTrailLine, ref vctColor, thickness);
            }            
        }

        public static void DrawTransparentCuboid(ref Matrix worldMatrix, MyCuboid cuboid, ref Vector4 vctColor, bool bWireFramed, float thickness, MyTransparentMaterialEnum? lineMaterial = null)
        {
            foreach (MyLine line in cuboid.UniqueLines)
            {
                Vector3 from = Vector3.Transform(line.From, worldMatrix);
                Vector3 to = Vector3.Transform(line.To, worldMatrix);
                DrawLine(from, to, lineMaterial ?? MyTransparentMaterialEnum.ProjectileTrailLine, ref vctColor, thickness);
            }
        }

        public static void DrawLine(Vector3 start, Vector3 end, MyTransparentMaterialEnum? material, ref Vector4 color, float thickness)
        {
            Vector3 dir = end - start;
            float len = dir.Length();
            if (len > 0.1f)
            {
                dir = MyMwcUtils.Normalize(dir);

                MyTransparentGeometry.AddLineBillboard(material ?? MyTransparentMaterialEnum.ProjectileTrailLine, color, start, dir, len, thickness);
            }        
        }

        public static void DrawTransparentCylinder(ref Matrix worldMatrix, float radius1, float radius2, float length, ref Vector4 vctColor, bool bWireFramed, int wireDivideRatio, float thickness, MyTransparentMaterialEnum? lineMaterial = null)
        {
            Vector3 vertexEnd = Vector3.Zero;
            Vector3 vertexStart = Vector3.Zero;

            Vector3 previousEnd = Vector3.Zero;
            Vector3 previousStart = Vector3.Zero;

            float angleStep = 360.0f / (float)wireDivideRatio;
            float alpha = 0;

            for (int i = 0; i <= wireDivideRatio; i++)
            {
                alpha = (float)i * angleStep;

                vertexEnd.X = (float)(radius1 * Math.Cos(MathHelper.ToRadians(alpha)));
                vertexEnd.Y = length/2;
                vertexEnd.Z = (float)(radius1 * Math.Sin(MathHelper.ToRadians(alpha)));

                vertexStart.X = (float)(radius2 * Math.Cos(MathHelper.ToRadians(alpha)));
                vertexStart.Y = -length / 2;
                vertexStart.Z = (float)(radius2 * Math.Sin(MathHelper.ToRadians(alpha)));

                vertexEnd = Vector3.Transform(vertexEnd, worldMatrix);
                vertexStart = Vector3.Transform(vertexStart, worldMatrix);

                DrawLine(vertexStart, vertexEnd, lineMaterial ?? MyTransparentMaterialEnum.ProjectileTrailLine, ref vctColor, thickness);

                if (i > 0)
                {
                    DrawLine(previousStart, vertexStart, lineMaterial ?? MyTransparentMaterialEnum.ProjectileTrailLine, ref vctColor, thickness);
                    DrawLine(previousEnd, vertexEnd, lineMaterial ?? MyTransparentMaterialEnum.ProjectileTrailLine, ref vctColor, thickness);
                }

                previousStart = vertexStart;
                previousEnd = vertexEnd;
            }
        }

        public static void DrawTransparentPyramid(ref Vector3 start, ref MyQuad backQuad, ref Vector4 vctColor, int divideRatio, float thickness, MyTransparentMaterialEnum? lineMaterial = null) 
        {
            Vector3 vctZero = Vector3.Zero;            
            m_lineBuffer.Clear();
            GenerateLines(start, backQuad.Point0, backQuad.Point1, ref m_lineBuffer, divideRatio);
            GenerateLines(start, backQuad.Point1, backQuad.Point2, ref m_lineBuffer, divideRatio);
            GenerateLines(start, backQuad.Point2, backQuad.Point3, ref m_lineBuffer, divideRatio);
            GenerateLines(start, backQuad.Point3, backQuad.Point0, ref m_lineBuffer, divideRatio);

            foreach (MyLine line in m_lineBuffer) 
            {
                Vector3 dir = line.To - line.From;
                float len = dir.Length();
                if (len > 0.1f)
                {
                    dir = MyMwcUtils.Normalize(dir);

                    MyTransparentGeometry.AddLineBillboard(lineMaterial ?? MyTransparentMaterialEnum.ProjectileTrailLine, vctColor, line.From, dir, len, thickness);
                }
            }
        }

        private static void GenerateLines(Vector3 start, Vector3 end1, Vector3 end2, ref List<MyLine> lineBuffer, int divideRatio) 
        {
            Vector3 dirStep = (end2 - end1) / (float)divideRatio;
            for (int i = 0; i < divideRatio; i++)
            {
                MyLine line = new MyLine(start, end1 + (float)i * dirStep, false);
                lineBuffer.Add(line);
            }
        }
        public static MyModel ModelCone
        {
            get { return m_modelCone; }
        }
        public static MyModel ModelHemisphereLowRes
        {
            get { return m_modelHemisphereLowRes; }
        }
        public static MyModel ModelSphere
        {
            get { return m_modelSphere; }
        }
        public static MyModel LightSphere
        {
            get { return m_modelLightSphere; }
        }
        public static MyModel ModelBoxLowRes
        {
            get { return m_modelBoxLowRes; }
        }
        public static MyModel ModelBoxHiRes
        {
            get { return m_modelBoxHiRes; }
        }
        public static MyModel Capsule
        {
            get { return m_modelCapsule; }
        }


    }
}

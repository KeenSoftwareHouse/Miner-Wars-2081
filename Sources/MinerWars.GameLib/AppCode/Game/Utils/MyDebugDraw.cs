using MinerWars.AppCode.App;
using SysUtils.Utils;
using System.Collections.Generic;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Effects;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Models;

using System;

using SharpDX.Direct3D9;
using SharpDX.Toolkit.Graphics;

//  Class used for drawing 3D debug lines. All methods are optimised and can be used freely - of course only for debugging.

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

    

    static class MyDebugDraw
    {
        static MyVertexFormatPositionColor[] m_verticesLine = null;
        static MyVertexFormatPositionColor[] m_triangleVertices = null;

        static List<MyVertexFormatPositionColor> m_moints;
        static List<BoundingSphere> m_spheres;

                                          /*
        static Model m_modelSphere;
        static Model m_modelSphereLowRes;
        static Model m_modelHemisphere;
        static Model m_modelBoxHiRes;
        static Model m_modelBoxLowRes;
        static Model m_modelCapsule;    */

        static MyLineBatch m_lineBatch;
        private static Vector3[] m_frustumCorners;
        private static List<Vector3> m_coneVertex;

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyDebugDraw.LoadContent() - START");
            MyMwcLog.IncreaseIndent();

            //  Line
            m_verticesLine = new MyVertexFormatPositionColor[2];
            m_verticesLine[0] = new MyVertexFormatPositionColor();
            m_verticesLine[1] = new MyVertexFormatPositionColor();

            //  Triangle
            m_triangleVertices = new MyVertexFormatPositionColor[3];
            m_triangleVertices[0] = new MyVertexFormatPositionColor();
            m_triangleVertices[1] = new MyVertexFormatPositionColor();
            m_triangleVertices[2] = new MyVertexFormatPositionColor();

            m_moints = new List<MyVertexFormatPositionColor>();  
            m_spheres = new List<BoundingSphere>();

            /*
            m_modelSphere = MyMinerGame.Static.Content.Load<Model>("Models2\\Debug\\Sphere");
            m_modelSphereLowRes = MyMinerGame.Static.Content.Load<Model>("Models2\\Debug\\Sphere_low");
            m_modelHemisphere = MyMinerGame.Static.Content.Load<Model>("Models2\\Debug\\Hemisphere");
            m_modelBoxHiRes = MyMinerGame.Static.Content.Load<Model>("Models2\\Debug\\BoxHiRes");
            m_modelBoxLowRes = MyMinerGame.Static.Content.Load<Model>("Models2\\Debug\\BoxLowRes");
            m_modelCapsule = MyMinerGame.Static.Content.Load<Model>("Models2\\Debug\\Capsule");

            m_lowResBoxEffect = (BasicEffect)m_modelBoxLowRes.Meshes[0].Effects[0];
                                   */
            m_lineBatch = new MyLineBatch(Matrix.Identity, Matrix.Identity, 128);

            m_frustumCorners = new Vector3[8];
            m_coneVertex = new List<Vector3>(32);

            MyDebugDrawCoordSystem.LoadContent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyDebugDraw.LoadContent() - END");
        }

        public static void UnloadContent()
        {
        }

        public static void DrawLine3D(Vector3 pointFrom, Vector3 pointTo, Color colorFrom, Color colorTo)
        {
            DrawLine3D(ref pointFrom, ref pointTo, ref colorFrom, ref colorTo);
        }

        public static void DrawLine3D(ref Vector3 pointFrom, ref Vector3 pointTo, ref Color colorFrom, ref Color colorTo)
        {                 
            Device graphicsDevice = MyMinerGame.Static.GraphicsDevice;
            
            //  Create the line vertices
            m_verticesLine[0].Position = pointFrom;
            m_verticesLine[0].Color = colorFrom.ToVector4();
            m_verticesLine[1].Position = pointTo;
            m_verticesLine[1].Color = colorTo.ToVector4();

            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            effect.SetProjectionMatrix(Matrix.CreatePerspectiveFieldOfView(MyCamera.FovWithZoom, MyCamera.AspectRatio, 0.01f, 1000000));
            effect.SetViewMatrix(MyCamera.ViewMatrix);
            effect.SetWorldMatrix(Matrix.Identity);
            effect.SetTechnique(MyEffectModelsDiffuse.Technique.PositionColor);

            graphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;
            //  Draw the line
            effect.Begin();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, 0, 1, m_verticesLine);
            effect.End();                
        }

        public static void DrawLine2D(Vector2 pointFrom, Vector2 pointTo, Color colorFrom, Color colorTo)
        {   
            Device graphicsDevice = MyMinerGame.Static.GraphicsDevice;

            //  Create the line vertices
            m_verticesLine[0].Position = new Vector3(pointFrom, 0);
            m_verticesLine[0].Color = colorFrom.ToVector4();
            m_verticesLine[1].Position = new Vector3(pointTo, 0);
            m_verticesLine[1].Color = colorTo.ToVector4();

            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            effect.SetProjectionMatrix(Matrix.CreateOrthographicOffCenter(0.0F, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0.0F, 0.0F, -1.0F));
            effect.SetViewMatrix(Matrix.Identity);
            effect.SetWorldMatrix(Matrix.Identity);
            effect.SetTechnique(MyEffectModelsDiffuse.Technique.PositionColor);

            graphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;

            //  Draw the line
            effect.Begin();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, 0, 1, m_verticesLine);
            effect.End();
        }


        public static void AddDrawTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Color color)
        {       
            m_moints.Add(new MyVertexFormatPositionColor(vertex1, color.ToVector4()));
            m_moints.Add(new MyVertexFormatPositionColor(vertex2, color.ToVector4()));
            m_moints.Add(new MyVertexFormatPositionColor(vertex3, color.ToVector4())); 
        }

        public static void AddDrawSphereWireframe(BoundingSphere bs)
        {
            m_spheres.Add(bs);
        }

        public static void ClearAll()
        {
            m_spheres.Clear();
            m_moints.Clear();
        }

        public static void Draw()
        {                   
            for (int i = 0; i < m_moints.Count / 3; i++)
            {
                MyVertexFormatPositionColor v1 = m_moints[i * 3 + 0];
                MyVertexFormatPositionColor v2 = m_moints[i * 3 + 1];
                MyVertexFormatPositionColor v3 = m_moints[i * 3 + 2];
                DrawTriangle(v1.Position, v2.Position, v3.Position, v1.Color, v2.Color, v3.Color);
            }

            for (int i = 0; i < m_spheres.Count; i++)
                DrawSphereWireframe(m_spheres[i].Center, m_spheres[i].Radius, Color.Red.ToVector3(), 1.0f);
            
            ClearAll();   
        }

        public static void DrawTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Color color1, Color color2, Color color3)
        {
            DrawTriangle(vertex1, vertex2, vertex3, color1.ToVector4(), color2.ToVector4(), color3.ToVector4());
        }

        public static void DrawTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector4 color1, Vector4 color2, Vector4 color3)
        {     
            Device graphicsDevice = MyMinerGame.Static.GraphicsDevice;

            //  Create triangleVertexes vertices
            m_triangleVertices[0] = new MyVertexFormatPositionColor(vertex1, color1);
            m_triangleVertices[1] = new MyVertexFormatPositionColor(vertex2, color2);
            m_triangleVertices[2] = new MyVertexFormatPositionColor(vertex3, color3);

            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            // Initialise the effect
            effect.SetProjectionMatrix(MyCamera.ProjectionMatrix);
            effect.SetViewMatrix(MyCamera.ViewMatrix);
            effect.SetTechnique(MyEffectModelsDiffuse.Technique.PositionColor);
            graphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;

            // Draw the line
            effect.Begin();
            graphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, 0, 1, m_triangleVertices);
            effect.End();
        }

        public static void DrawSphereWireframe(Vector3 position, float radius, Vector3 diffuseColor, float alpha)
        {                                                      
            MyStateObjects.WireframeRasterizerState.Apply();
            Matrix m = Matrix.Identity * radius;
            m.M44 = 1;
            m.Translation = position;
            DrawModel(MySimpleObjectDraw.ModelSphere, m, diffuseColor, alpha);
        }

        public static void DrawSphereWireframe(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {
            MyStateObjects.WireframeRasterizerState.Apply();
            DrawModel(MySimpleObjectDraw.LightSphere, worldMatrix, diffuseColor, alpha);
        }

        public static void DrawHemisphereWireframe(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {
            MyStateObjects.WireframeRasterizerState.Apply();
            DrawModel(MySimpleObjectDraw.ModelHemisphereLowRes, worldMatrix, diffuseColor, alpha);
        }

        public static void DrawSphereSmooth(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {
            RasterizerState.CullNone.Apply();
            DrawModel(MySimpleObjectDraw.ModelSphere, worldMatrix, diffuseColor, alpha);
        }

        public static void DrawCapsuleWireframe(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {
            MyStateObjects.WireframeRasterizerState.Apply();
            DrawModel(MySimpleObjectDraw.Capsule, worldMatrix, diffuseColor, alpha);
        }

        public static void DrawCapsuleSmooth(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {
            RasterizerState.CullNone.Apply();
            DrawModel(MySimpleObjectDraw.Capsule, worldMatrix, diffuseColor, alpha);
        }

        public static void DrawSphereSmooth(Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Vector3 diffuseColor, float alpha)
        {
            RasterizerState.CullNone.Apply();
            DrawModel(MySimpleObjectDraw.ModelSphere, worldMatrix, viewMatrix, projectionMatrix, diffuseColor, alpha);
        }

        public static void DrawSphereSmooth(Vector3 position, float radius, Vector3 diffuseColor, float alpha)
        {
            DrawSphereSmooth(Matrix.CreateScale(radius) * Matrix.CreateTranslation(position), diffuseColor, alpha);
        }
        
        //  Draws hi-res wireframe box (its surface is split into many small triangles)
        public static void DrawHiresBoxWireframe(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {                                                                    
            MyStateObjects.WireframeRasterizerState.Apply();
            DrawModel(MySimpleObjectDraw.ModelBoxHiRes, worldMatrix, diffuseColor, alpha);  
        }

        //  Draws hi-res smooth box (its surface is split into many small triangles)
        public static void DrawHiresBoxSmooth(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {      
            RasterizerState.CullNone.Apply();
            DrawModel(MySimpleObjectDraw.ModelBoxHiRes, worldMatrix, diffuseColor, alpha);
        }

        //@ Draw world debug aabb
        public static void DrawAABB(ref BoundingBox worldAABB, ref Vector4 color, float fScale)
        {     
            Vector3 size = worldAABB.Max - worldAABB.Min;
            Vector3 center = size / 2f;
            center = center + worldAABB.Min;
            Matrix mat = Matrix.CreateWorld(center, new Vector3(0,0,-1), new Vector3(0,1,0));
            MyDebugDraw.DrawHiresBoxWireframe(Matrix.CreateScale(size * fScale) * mat, new Vector3(color.X, color.Y, color.Z), color.W);            
        }

        //@ Draw world debug aabb
        public static void DrawAABBLowRes(ref BoundingBox worldAABB, ref Vector4 color, float fScale)
        { 
            Vector3 size = worldAABB.Max - worldAABB.Min;
            Vector3 center = size / 2f;
            center = center + worldAABB.Min;
            Matrix mat = Matrix.CreateWorld(center, new Vector3(0, 0, -1), new Vector3(0, 1, 0));
            MyDebugDraw.DrawLowresBoxWireframe(Matrix.CreateScale(size * fScale) * mat, new Vector3(color.X, color.Y, color.Z), color.W);  
        }
                           
        public static void DrawAABBLine(ref BoundingBox worldAABB, ref Vector4 color, float fScale)
        {
            Color colorC = new Color(color);
            Vector3 center = worldAABB.GetCenter();
            Vector3 halfSize = worldAABB.Size() * fScale * 0.5f;

            Vector3 v0 = new Vector3(center.X - halfSize.X, center.Y - halfSize.Y, center.Z - halfSize.Z);
            Vector3 v1 = new Vector3(center.X + halfSize.X, center.Y - halfSize.Y, center.Z - halfSize.Z);
            Vector3 v2 = new Vector3(center.X - halfSize.X, center.Y + halfSize.Y, center.Z - halfSize.Z);
            Vector3 v3 = new Vector3(center.X + halfSize.X, center.Y + halfSize.Y, center.Z - halfSize.Z);
            Vector3 v4 = new Vector3(center.X - halfSize.X, center.Y - halfSize.Y, center.Z + halfSize.Z);
            Vector3 v5 = new Vector3(center.X + halfSize.X, center.Y - halfSize.Y, center.Z + halfSize.Z);
            Vector3 v6 = new Vector3(center.X - halfSize.X, center.Y + halfSize.Y, center.Z + halfSize.Z);
            Vector3 v7 = new Vector3(center.X + halfSize.X, center.Y + halfSize.Y, center.Z + halfSize.Z);

            MyDebugDraw.DrawLine3D(ref v0, ref v1, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v0, ref v2, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v2, ref v3, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v3, ref v1, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v4, ref v5, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v4, ref v6, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v6, ref v7, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v5, ref v7, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v0, ref v4, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v1, ref v5, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v2, ref v6, ref colorC, ref colorC);
            MyDebugDraw.DrawLine3D(ref v3, ref v7, ref colorC, ref colorC);
        }
                             
        //@ Draw world debug aabb
        public static void DrawAABBSolidLowRes(BoundingBox worldAABB, Vector4 color, float fScale)
        {                    
            Vector3 size = worldAABB.Max - worldAABB.Min;
            Vector3 center = size / 2f;
            center = center + worldAABB.Min;
            Matrix mat = Matrix.CreateWorld(center, new Vector3(0, 0, -1), new Vector3(0, 1, 0));
            MyDebugDraw.DrawLowresBoxSmooth(Matrix.CreateScale(size * fScale) * mat, new Vector3(color.X, color.Y, color.Z), color.W);            
        }
                              
        //  Draws low-res wireframe box (12 triangles)
        public static void DrawLowresBoxWireframe(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {     
            MyStateObjects.WireframeRasterizerState.Apply();
            DrawModel(MySimpleObjectDraw.ModelBoxLowRes, worldMatrix, diffuseColor, alpha);
        }

        //  Draws low-res smooth box (12 triangles)
        public static void DrawLowresBoxSmooth(Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {                                                                   
            RasterizerState.CullNone.Apply();
            DrawModel(MySimpleObjectDraw.ModelBoxLowRes, worldMatrix, diffuseColor, alpha);    
        }
    
        //  Draws low-res smooth box (12 triangles)
        public static void DrawLowresBoxSmooth(Vector3 position, Vector3 scale, Vector3 diffuseColor, float alpha)
        {       
            DrawLowresBoxSmooth(Matrix.CreateScale(scale) * Matrix.CreateTranslation(position), diffuseColor, alpha);            
        }
          
        public static void DrawModel(MyModel model, Matrix worldMatrix, Vector3 diffuseColor, float alpha)
        {
            DrawModel(model, worldMatrix, MyCamera.ViewMatrix, MyCamera.ProjectionMatrix, diffuseColor, alpha);
        }

        public static void DrawModel(MyModel model, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Vector3 diffuseColor, float alpha)
        {
            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            effect.SetWorldMatrix(worldMatrix);
            effect.SetViewMatrix(ref viewMatrix);
            effect.SetProjectionMatrix(ref projectionMatrix);
            effect.SetDiffuseColor(new Vector4(diffuseColor, alpha));
            effect.SetTechnique(MyEffectModelsDiffuse.Technique.Position);

            effect.Begin();
            model.Render();
            effect.End();
        }       

        public static void DrawAxis(Matrix matrix, float axisLength, float alpha)
        {
            Vector3 pos = matrix.Translation;

            MyDebugDraw.DrawLine3D(pos, pos + (matrix.Right * axisLength), Color.Red, Color.Red);
            MyDebugDraw.DrawLine3D(pos, pos + (matrix.Up * axisLength), Color.Green, Color.Green);
            MyDebugDraw.DrawLine3D(pos, pos + (matrix.Forward * axisLength), Color.Blue, Color.Blue);
        }






        /// <summary>
        /// Draw debug text
        /// </summary>
        /// <param name="screenCoord"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="scale"></param>
        public static float DrawText(Vector2 screenCoord, StringBuilder text, Color color, float scale)
        {
            MyGuiManager.BeginSpriteBatch();
            float textLenght = MyGuiManager.GetFontMinerWarsWhite().DrawString(screenCoord, color, text, scale);
            MyGuiManager.EndSpriteBatch();

            return textLenght;
        }

        public static void DrawText(Vector3 worldCoord, StringBuilder text, Color color, float scale)
        {
            Vector4 screenCoord = Vector4.Transform(worldCoord, MyCamera.ViewProjectionMatrix);

            if (screenCoord.Z > 0)
            {
                Vector2 projectedPoint2D = new Vector2(screenCoord.X / screenCoord.W / 2.0f + 0.5f, -screenCoord.Y / screenCoord.W / 2.0f + 0.5f);
                projectedPoint2D = MyGuiManager.GetHudPixelCoordFromNormalizedCoord(projectedPoint2D);

                DrawText(projectedPoint2D, text, color, scale);
            }
        }

        public static void DrawBoundingFrustum(BoundingFrustum boundingFrustum, Color color)
        {               
            boundingFrustum.GetCorners(m_frustumCorners);

            DrawCorners(m_frustumCorners, color);
        }

        public static void DrawCorners(Vector3[] corners, Color color)
        {
            m_lineBatch.Begin();

            // near face            
            m_lineBatch.DrawLine(corners[0], corners[1], color);
            m_lineBatch.DrawLine(corners[1], corners[2], color);
            m_lineBatch.DrawLine(corners[2], corners[3], color);
            m_lineBatch.DrawLine(corners[3], corners[0], color);

            // far face            
            m_lineBatch.DrawLine(corners[4], corners[5], color);
            m_lineBatch.DrawLine(corners[5], corners[6], color);
            m_lineBatch.DrawLine(corners[6], corners[7], color);
            m_lineBatch.DrawLine(corners[7], corners[4], color);

            // top,right,bottom,left face            
            m_lineBatch.DrawLine(corners[0], corners[4], color);
            m_lineBatch.DrawLine(corners[1], corners[5], color);
            m_lineBatch.DrawLine(corners[2], corners[6], color);
            m_lineBatch.DrawLine(corners[3], corners[7], color);

            m_lineBatch.End();
        }

        public static void DrawCone(Vector3 start, Vector3 end, float radius, Color color) 
        {            
            m_coneVertex.Clear();
            Vector3 forward = Vector3.Normalize(end - start);
            Vector3 up = Vector3.Cross(forward, Vector3.Right);
            float length = Vector3.Distance(end, start);
            Matrix world = Matrix.CreateWorld(start, forward, up);            

            float angleStep = MathHelper.TwoPi / 32;
            
            for (int i = 0; i < 32; i++) 
            {
                Vector3 vertex = new Vector3();
                vertex.X = (float)Math.Cos(i * angleStep) * radius;
                vertex.Y = (float)Math.Sin(i * angleStep) * radius;
                vertex.Z = length;
                vertex = Vector3.Transform(vertex, world);
                m_coneVertex.Add(vertex);
            }

            m_lineBatch.Begin();
            // draw lines from start to ends
            for (int i = 0; i < m_coneVertex.Count; i++) 
            {
                m_lineBatch.DrawLine(start, m_coneVertex[i], color);                
            }            
            m_lineBatch.End();   
        }

        static public class TextBatch
        {
            struct TextData
            {
                public TextData(Vector2 screenCoord, StringBuilder text, Color color, float scale)
                {
                    this.screenCoord = screenCoord;
                    this.color = color;
                    this.text = text;
                    this.scale = scale;
                }

                public Vector2 screenCoord;
                public Color color;
                public StringBuilder text;
                public float scale;
            }

            static List<TextData> m_data = new List<TextData>();

            static public void AddText(Vector2 screenCoord, StringBuilder text, Color color, float scale)
            {
                m_data.Add(new TextData(screenCoord, text, color, scale));
            }

            public static void AddText(Vector3 worldCoord, StringBuilder text, Color color, float scale)
            {
                Vector4 screenCoord = Vector4.Transform(worldCoord, MyCamera.ViewProjectionMatrix);

                if (screenCoord.Z > 0)
                {
                    Vector2 projectedPoint2D = new Vector2(screenCoord.X / screenCoord.W / 2.0f + 0.5f, -screenCoord.Y / screenCoord.W / 2.0f + 0.5f);
                    projectedPoint2D = MyGuiManager.GetHudPixelCoordFromNormalizedCoord(projectedPoint2D);

                    AddText(projectedPoint2D, text, color, scale);
                }
            }

            static public void Draw()
            {
                MyGuiManager.BeginSpriteBatch();

                for (int i = 0; i < m_data.Count; i++)
                {
                    TextData data = m_data[i];
                    MyGuiManager.GetFontMinerWarsWhite().DrawString(data.screenCoord, data.color, data.text, data.scale);
                }
                    
                MyGuiManager.EndSpriteBatch();

                m_data.Clear();
            }
        }
      
    }
}

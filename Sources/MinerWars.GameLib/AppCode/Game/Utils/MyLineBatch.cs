using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MinerWars.AppCode.Game.Utils.VertexFormats;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;

using SharpDX.Direct3D9;
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


    public class MyLineBatch
    {
        Matrix view;
        Matrix projection;
        int maxSize = 0;
        int numVertices = 0;
        MyVertexFormatPositionColor[] lineData;

        int numOSVertices = 0;
        MyVertexFormatPositionColor[] onScreenLineData;


        public MyLineBatch(Matrix view, Matrix projection, int size)
        {
            maxSize = 2 * size;
            lineData = new MyVertexFormatPositionColor[maxSize];
            onScreenLineData = new MyVertexFormatPositionColor[maxSize];
        }

        public void Begin()
        {
            numVertices = 0;
            numOSVertices = 0;
        }

        public void End()
        {
            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            if (numVertices > 0)
            {
                //effect.Projection = Matrix.CreateOrthographicOffCenter(0.0F, device.Viewport.Width, device.Viewport.Height, 0.0F, 0.0F, -1.0F);
                //effect.View = Matrix.Identity;
                effect.SetWorldMatrix(Matrix.Identity);
                effect.SetProjectionMatrix(ref MyCamera.ProjectionMatrix);
                effect.SetViewMatrix(ref MyCamera.ViewMatrix);
                effect.SetTechnique(MyEffectModelsDiffuse.Technique.PositionColor);

                //  Draw the line
                effect.Begin();

                MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;
                MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, 0, numVertices / 2, lineData);

                effect.End();
            }

            if (numOSVertices > 0)
            {
                //device.Clear(ClearOptions.DepthBuffer, Color.BlueViolet, 0, 0);

                // you have to set these parameters for the basic effect to be able to draw on the screen
                effect.SetWorldMatrix(Matrix.Identity);
                effect.SetViewMatrix(Matrix.Identity);
                effect.SetProjectionMatrix(Matrix.Identity);
                effect.SetTechnique(MyEffectModelsDiffuse.Technique.PositionColor);
                MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;

                //  Draw the line
                effect.Begin();
                MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, 0, numOSVertices / 2, onScreenLineData);
                effect.End();
            }
        }

        public void DrawLine(Vector3 v0, Vector3 v1, Color color)
        {
            if (numVertices + 2 < maxSize)
            {
                lineData[numVertices].Position = v0;
                lineData[numVertices].Color = color.ToVector4();
                lineData[numVertices + 1].Position = v1;
                lineData[numVertices + 1].Color = color.ToVector4();
                numVertices += 2;
            }
        }

        public void DrawOnScreenLine(Vector3 v0, Vector3 v1, Color color)
        {
            if (numOSVertices + 2 < maxSize)
            {
                onScreenLineData[numOSVertices].Position = v0;
                onScreenLineData[numOSVertices].Color = color.ToVector4();
                onScreenLineData[numOSVertices + 1].Position = v1;
                onScreenLineData[numOSVertices + 1].Color = color.ToVector4();
                numOSVertices += 2;
            }
        }

        public Vector3 Mul(Matrix m, Vector3 v)
        {
            return new Vector3(v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31,
                                v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32,
                                v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33);
        }

        public void DrawLines(Vector3[] lineData, Color color)
        {
            // drawing in 3D requires you to define vertices of a certain type
            // VertexPositionColor simply means a vertex with a position and color
            MyVertexFormatPositionColor[] line = new MyVertexFormatPositionColor[lineData.Length];

            for (int i = 0; i < lineData.Length; i++)
            {
                line[i] = new MyVertexFormatPositionColor(lineData[i], color.ToVector4());
            }

            var effect = (MyEffectModelsDiffuse)MyRender.GetEffect(MyEffects.ModelDiffuse);

            // you have to set these parameters for the basic effect to be able to draw
            // on the screen
            effect.SetProjectionMatrix(MyCamera.ProjectionMatrix);
            effect.SetViewMatrix(MyCamera.ViewMatrix);

            // graphics card should use basic effect shader
            effect.Begin();

            // you have to tell the graphics card what kind of vertices it will be receiving
            MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.VertexDeclaration = MyVertexFormatPositionColor.VertexDeclaration;
            MinerWars.AppCode.App.MyMinerGame.Static.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, 0, lineData.Length, line);

            effect.End();
        }
         
    }
}

using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using MinerWars.AppCode.Game.Render;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyDebugDrawCoordSystem
    {
        static MyDebugDrawCoordSystem()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.DrawCoordSystem, "Draw coord system", MyDebugDrawCoordSystem.Draw, Render.MyRenderStage.DebugDraw, false);
        }

        public static void LoadContent()
        {
        }

        public static void Draw()
        {
            const float axisLength = 100;
            MyDebugDraw.DrawLine3D(new Vector3(0, 0, 0), new Vector3(axisLength, 0, 0), Color.Black, Color.Red);
            MyDebugDraw.DrawLine3D(new Vector3(0, 0, 0), new Vector3(0, axisLength, 0), Color.Black, Color.Green);
            MyDebugDraw.DrawLine3D(new Vector3(0, 0, 0), new Vector3(0, 0, axisLength), Color.Black, Color.Blue);

            MyDebugDrawCurve.DrawCurve();
        }
    }
}

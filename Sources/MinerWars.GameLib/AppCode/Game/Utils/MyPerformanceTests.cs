using MinerWarsMath;

namespace MinerWars.AppCode.Game.Utils
{
    static class MyPerformanceTests
    {
        public static void Run()
        {
            Vector3 vector = new Vector3(1, 2, 3);
            Matrix m = Matrix.CreateRotationX(5);
            Matrix m2 = Matrix.CreateRotationY(5);
            Vector3 resTotal = new Vector3();
            int numIterations = 10000;

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Vector transform managed");
            for (int i = 0; i < numIterations; i++)
            {
                Vector3 res;
                Vector3.Transform(ref vector, ref m, out res);
                resTotal += res;
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Vector transform unmanaged");
            for (int i = 0; i < numIterations; i++)
            {
                Vector3 res;
                Vector3.Transform_Native(ref vector, ref m, out res);
                resTotal += res;
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            Matrix resTotalM = new Matrix();

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Matrix multiply managed");
            for (int i = 0; i < numIterations; i++)
            {
                Matrix res;
                Matrix.Multiply(ref m, ref m2, out res);
                resTotalM += res;
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();


            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("Matrix multiply unmanaged");
            for (int i = 0; i < numIterations; i++)
            {
                Matrix res;
                Matrix.Multiply_Native(ref m, ref m2, out res);
                resTotalM += res;
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }
    }
}

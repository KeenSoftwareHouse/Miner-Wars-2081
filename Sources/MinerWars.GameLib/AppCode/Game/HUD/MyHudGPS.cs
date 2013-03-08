using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;

//  Draws the GPS.
namespace MinerWars.AppCode.Game.HUD
{
    static class MyHudGPS
    {
        static MyHudGPS()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.GPS, "GPS", Draw, MyRenderStage.LODDrawStart, true);
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyHudGPS.LoadContent() - START");
            MyMwcLog.IncreaseIndent();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyHudGPS::LoadContent");

            //MyModel model = MyModels.GetModelForDraw(MyModelsEnum.);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();    
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHudGPS.LoadContent() - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyHudGPS.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyHudGPS.UnloadContent - END");
        }

        public static void Draw()
        {
            if (MyRender.GetCurrentLodDrawPass() == MyLodTypeEnum.LOD0)
                MyHud.DrawGPS();
        }
    }
}

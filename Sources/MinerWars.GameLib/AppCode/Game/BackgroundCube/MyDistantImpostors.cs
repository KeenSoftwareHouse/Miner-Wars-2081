using MinerWars.AppCode.Game.Utils;
using System.Collections.Generic;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.BackgroundCube
{
    internal static class MyDistantImpostors
    {
        static readonly MyDistantObjectImpostors m_objectImpostors;
        static readonly MyVoxelMapImpostors m_voxelImpostors;

        static MyDistantImpostors()
        {
            MyMwcLog.WriteLine("MyDistantImpostors()");

            const float defaultSize = 200000;

            m_objectImpostors = new MyDistantObjectImpostors();
            m_objectImpostors.Scale = MyMwcSectorConstants.SECTOR_SIZE / defaultSize;

            m_voxelImpostors = new MyVoxelMapImpostors();

            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.DistantImpostors, "Distant impostors", PrepareForDraw, Render.MyRenderStage.PrepareForDraw);
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.DistantImpostors, "Distant impostors", Draw, Render.MyRenderStage.Background);
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyDistantImpostors.LoadData");
            m_objectImpostors.LoadData();
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            m_objectImpostors.UnloadData();
        }

        public static void LoadContent(List<MyMwcObjectBuilder_Base> enterSectorResponses, bool createRealImpostors)
        {
            m_objectImpostors.LoadContent();
            m_voxelImpostors.LoadContent(enterSectorResponses, createRealImpostors);
        }

        public static void ReloadContent()
        {
            m_voxelImpostors.LoadContent(null, false);
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyDistantImpostors.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            m_objectImpostors.UnloadContent();
            m_voxelImpostors.UnloadContent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyDistantImpostors.UnloadContent - END");
        }

        public static void Update()
        {
            m_objectImpostors.Update();
        }

        public static void PrepareForDraw()
        {
            if (MyRenderConstants.RenderQualityProfile.EnableDistantImpostors)
            {
                m_voxelImpostors.PrepareForDraw(
                    MyRender.GetEffect(MyEffects.DistantImpostors) as MyEffectDistantImpostors);
            }
        }

        public static void Draw()
        {
            if (MyRenderConstants.RenderQualityProfile.EnableDistantImpostors)
            {
                m_objectImpostors.Draw(MyRender.GetEffect(MyEffects.DistantImpostors) as MyEffectDistantImpostors);
                m_voxelImpostors.Draw(MyRender.GetEffect(MyEffects.DistantImpostors) as MyEffectDistantImpostors);
            }
        }
    }
}

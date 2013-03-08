using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Cockpit;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Utils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Entities.Ships.SubObjects
{
    class MyCockpitWeapons
    {
        static MyCockpitWeapons()
        {
            MyRender.RegisterRenderModule(MyRenderModuleEnum.CockpitWeapons, "Cockpit weapons", Draw, MyRenderStage.LODDrawStart);
        }

        public static void LoadContent()
        {
            MyMwcLog.WriteLine("MyCockpitWeapons.LoadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyCockpitWeapons.LoadContent - END");
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyCockpitWeapons.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyCockpitWeapons.UnloadContent - END");
        }

        public static bool CanDrawCockpitWeapons()
        {
            return MyGuiScreenGamePlay.Static.IsGameActive() == true &&
              MyCamera.ActualCameraDirection == MyCameraDirection.FORWARD &&
              MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip &&
              MySession.PlayerShip != null &&
              !MySession.PlayerShip.IsDead() &&
              MySession.Static != null &&
              MySession.Static.Player != null &&
              !MySession.Static.Player.IsDead();
        }

        public static void Draw()
        {
            if (MyRender.GetCurrentLodDrawPass() == Utils.MyLodTypeEnum.LOD_NEAR && CanDrawCockpitWeapons() && MyFakes.DRAW_WEAPONS)
            {
                var old = MySession.PlayerShip.Weapons.Visible;
                MySession.PlayerShip.Weapons.Visible = true;
                MySession.PlayerShip.Weapons.Draw();
                MySession.PlayerShip.Weapons.Visible = old;

                MySession.PlayerShip.CubeBuilder.Draw();
            }
        }
    }
}

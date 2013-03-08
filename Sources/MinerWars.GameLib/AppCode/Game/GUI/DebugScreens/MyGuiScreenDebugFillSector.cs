#region Using
using System;
using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Render;
using MinerWars.AppCode.Game.Effects;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.Models;
using MinerWars.AppCode.Game;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.TransparentGeometry;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;
using MinerWars.AppCode.Game.SolarSystem;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.BackgroundCube;
#endregion

namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenDebugFillSector : MyGuiScreenDebugBase
    {
        List<MyEntity> m_allEntities = new List<MyEntity>();

        MySectorObjectCounts m_sectorObjectCounts = new MySectorObjectCounts();

        public MyGuiScreenDebugFillSector()
            : base(0.35f * Color.Yellow.ToVector4(), false)
        {
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            m_isTopMostScreen = false;
            m_canHaveFocus = false;

            RecreateControls(true);
        }

        public override void RecreateControls(bool contructor)
        {
            Controls.Clear();

            AddCaption(new System.Text.StringBuilder("Filling sector"), Color.Yellow.ToVector4());

            MyGuiControlLabel label = new MyGuiControlLabel(this, new Vector2(0.01f, -m_size.Value.Y / 2.0f + 0.07f), null, new System.Text.StringBuilder("(press ALT to share focus)"), Color.Yellow.ToVector4(), MyGuiConstants.LABEL_TEXT_SCALE * 0.7f,
                               MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_TOP);
            Controls.Add(label);

            m_scale = 0.7f;
            m_currentPosition = -m_size.Value / 2.0f + new Vector2(0.02f, 0.10f);
            m_currentPosition.Y += 0.01f;

            AddLabel(new StringBuilder("Sector world"), Color.Yellow.ToVector4(), 1.2f);
            AddButton(new StringBuilder("Clear sector"), delegate { ClearSector(); });
            AddButton(new StringBuilder("Clear generated"), delegate { ClearGenerated(); });

            AddSlider(new StringBuilder("Voxels 64"), 0.0f, 200.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.Voxels64));
            AddSlider(new StringBuilder("Voxels 128"), 0.0f, 100.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.Voxels128));
            AddSlider(new StringBuilder("Voxels 256"), 0.0f, 50.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.Voxels256));
            AddSlider(new StringBuilder("Voxels 512"), 0.0f, 50.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.Voxels512));
            AddSlider(new StringBuilder("Static asteroids small"), 0.0f, 50000.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.StaticAsteroidSmall));
            AddSlider(new StringBuilder("Static asteroids medium"), 0.0f, 50000.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.StaticAsteroidMedium));
            AddSlider(new StringBuilder("Static asteroids large"), 0.0f, 3000.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.StaticAsteroidLarge));
            AddSlider(new StringBuilder("Motherships"), 0.0f, 1000.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.Motherships));
            AddSlider(new StringBuilder("Large debris"), 0.0f, 1000.0f, m_sectorObjectCounts, MemberHelper.GetMember(() => m_sectorObjectCounts.StaticDebrisFields));
            AddButton(new StringBuilder("Create custom world (STONE)"), delegate { CreateCustomWorldStone(); });
            AddButton(new StringBuilder("Create custom world (ICE)"), delegate { CreateCustomWorldIce(); });
            AddButton(new StringBuilder("Create generated world"), delegate { CreateGeneratedWorld(); });
            AddButton(new StringBuilder("Remove generated world"), delegate { RemoveGeneratedWorld(); });
            AddButton(new StringBuilder("Create solar areas"), delegate { MySolarSystemConstants.CreateAreas(); });

            m_currentPosition.Y += 0.01f;
            AddLabel(new StringBuilder("Impostors"), Color.Yellow.ToVector4(), 1.2f);
            AddButton(new StringBuilder("Reload"), delegate { MyDistantImpostors.ReloadContent(); });

            m_currentPosition.Y += 0.01f;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenDebugFillSector";
        }

        void ClearSector()
        {
            MyEntities.CloseAll(false);
        }

        void ClearGenerated()
        {
            List<MyEntity> entitiesToRemove = new List<MyEntity>();
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                if (entity.IsGenerated)
                    entitiesToRemove.Add(entity);
            }

            foreach (MyEntity entity in entitiesToRemove)
            {
                entity.MarkForClose();
            }
        }

        void CreateCustomWorldStone()
        {
            List<BoundingSphere> safeAreas = new List<BoundingSphere>();
            foreach (MyEntity entity in MyEntities.GetEntities())
            {
                MyDummyPoint dummyPoint = entity as MyDummyPoint;

                if (dummyPoint != null && dummyPoint.DummyFlags.HasFlag(MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.MyDummyPointFlags.SAFE_AREA))
                {
                    safeAreas.Add(dummyPoint.WorldVolume);
                }
            }

            BoundingSphere playerSafeArea = new BoundingSphere(MySession.PlayerShip.GetPosition(), 150); //smallship + mothership
            safeAreas.Add(playerSafeArea);

            //MyEntities.CloseAll(false);
            var generator = new MySectorGenerator((int)DateTime.Now.Ticks, safeAreas);
            var sectorObjects = generator.GenerateObjectBuilders(MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position, m_sectorObjectCounts, false);
            MyGuiScreenGamePlay.CreateFromSectorObjectBuilder(sectorObjects);

            MyEntities.UpdateAfterSimulation(); //Updates AABBs of objects
            MinerWars.AppCode.Game.Render.MyRender.RebuildCullingStructure();
        }

        void CreateCustomWorldIce()
        {
            //MyEntities.CloseAll(false);
            var generator = new MySectorGenerator((int)DateTime.Now.Ticks);
            var sectorObjects = generator.GenerateObjectBuilders(MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position, m_sectorObjectCounts, false);
            MyGuiScreenGamePlay.CreateFromSectorObjectBuilder(sectorObjects);

            MyEntities.UpdateAfterSimulation(); //Updates AABBs of objects
            MinerWars.AppCode.Game.Render.MyRender.RebuildCullingStructure();
        }

        void CreateGeneratedWorld()
        {
          //  MyEntities.CloseAll(false);
            List<MyMwcObjectBuilder_Base> objectBuilders = new List<MyMwcObjectBuilder_Base>();
            
            List<BoundingSphere> safeAreas = new List<BoundingSphere>();
            foreach (MyEntity existingEntity in MyEntities.GetEntities())
            {
                MyDummyPoint dummy = existingEntity as MyDummyPoint;
                if (dummy != null && dummy.DummyFlags.HasFlag(MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.MyDummyPointFlags.SAFE_AREA))
                {
                    safeAreas.Add(existingEntity.WorldVolume);
                }
            }

            MyGuiScreenGamePlay.Static.GenerateSector(MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position, objectBuilders, safeAreas, false);
            MyGuiScreenGamePlay.CreateFromObjectBuilders(objectBuilders);

            MyEntities.UpdateAfterSimulation(); //Updates AABBs of objects
            MinerWars.AppCode.Game.Render.MyRender.RebuildCullingStructure();
        }

        void RemoveGeneratedWorld()
        {
            List<MyEntity> toDelete = new List<MyEntity>();
            foreach (MyEntity existingEntity in MyEntities.GetEntities())
            {
                if (existingEntity.IsGenerated)
                    toDelete.Add(existingEntity);
            }

            foreach (MyEntity entity in toDelete)
            {
                entity.MarkForClose();
            }
        }
    }
}

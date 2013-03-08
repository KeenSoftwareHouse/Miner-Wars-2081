using KeenSoftwareHouse.Library.Memory;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.AppCode.Game.Physics;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Physics;
using MinerWars.AppCode.Physics.Collisions;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using ParallelTasks;
using MinerWars.AppCode.Game.Render;
using System.Collections.Generic;
using System.Diagnostics;
using MinerWars.CommonLIB.AppCode.Utils;
using KeenSoftwareHouse.Library.Parallelization.Threading;

namespace MinerWars.AppCode.Game.TransparentGeometry
{
    class MyRayCastOcclusionJob: IWork
    {
        MyLightGlare m_glare;
        Vector3 m_cameraPosition;
        MyEntity m_ignoreEntity1;
        MyEntity m_ignoreEntity2;

        public bool Visible { get; private set; }

        public bool IsDone { get; set; }

        public void Start(MyLightGlare glare, Vector3 cameraPosition, MyEntity ignoreEntity1 = null, MyEntity ignoreEntity2 = null)
        {
            m_glare = glare;
            m_cameraPosition = cameraPosition;

            m_ignoreEntity1 = ignoreEntity1;
            m_ignoreEntity2 = ignoreEntity2;

            IsDone = false;
        }

        public void Clear()
        {
            m_glare = null;
            m_ignoreEntity1 = null;
            m_ignoreEntity2 = null;
            IsDone = false;
        }

        public void DoWork()
        {
            var glare = m_glare; // we copy variable because other thread can null it
            if (glare == null)
                return;

            if (Vector3.DistanceSquared(glare.Position, m_cameraPosition) < MyMwcMathConstants.EPSILON)
                return;

            var directLine = new MyLine(glare.Position, m_cameraPosition);
            m_helperCollection.Clear();

            using (MyEntities.EntityCloseLock.AcquireSharedUsing())
            {

                var intersectionResult = MyEntities.GetIntersectionWithLine(ref directLine, m_ignoreEntity1, m_ignoreEntity2, false, false, false, false, true);

                //var intersectionResult = MyRender.GetAnyIntersectionWithLine(ref directLine, m_ignoreEntity1, m_ignoreEntity2, true);

                if (intersectionResult == null)
                {
                    if (MySession.PlayerShip == null || MySession.PlayerShip.Weapons == null)
                    {
                        IsDone = true;
                        m_helperCollection.Clear();
                        return;
                    }

                    MyIntersectionResultLineTriangleEx? cockpitIntersection = GetIntersectionInNearSpace(MySession.PlayerShip.GetShipCockpit(), ref directLine, true);
                    Visible = !cockpitIntersection.HasValue;

                    Debug.Assert(m_helperCollection.Count == 0);
                    m_helperCollection.AddRange(MySession.PlayerShip.Weapons.GetMountedWeaponsWithHarvesterAndDrill());
                    foreach (var weapon in m_helperCollection)
                    {
                        MyIntersectionResultLineTriangleEx? intersection = GetIntersectionInNearSpace(weapon, ref directLine, true);
                        if (intersection.HasValue)
                        {
                            Visible = false;
                            IsDone = true;
                            m_helperCollection.Clear();
                            return;
                        }
                    }
                }
                else
                {
                    Visible = false;
                }

                IsDone = true;

                m_ignoreEntity1 = null;
                m_ignoreEntity2 = null;
                m_glare = null;
            }
        }

        void ConvertLineToNearWorldCoordinates(ref MyLine worldLine)
        {
            Matrix commonCameraMatrix = MyCamera.ViewMatrixAtZero * MyCamera.ProjectionMatrix;
            Vector3 from2 = worldLine.From - MyCamera.Position;
            Vector4 fromPositionByNear = Vector4.Transform(new Vector4(from2.X, from2.Y, from2.Z, 1), commonCameraMatrix);
            Matrix normalCam = MyCamera.ViewMatrixAtZero * MyCamera.ProjectionMatrixForNearObjects;
            normalCam = Matrix.Invert(normalCam);
            Vector4 fromInWorldNear4 = Vector4.Transform(fromPositionByNear, normalCam);
            Vector3 fromInWorldNear = new Vector3(fromInWorldNear4.X, fromInWorldNear4.Y, fromInWorldNear4.Z) / fromInWorldNear4.W;

            Vector3 to = worldLine.To - MyCamera.Position;
            Vector4 toPositionByNear = Vector4.Transform(new Vector4(to.X, to.Y, to.Z, 1), commonCameraMatrix);
            Vector4 toInWorldNear4 = Vector4.Transform(toPositionByNear, normalCam);
            Vector3 toInWorldNear = new Vector3(toInWorldNear4.X, toInWorldNear4.Y, toInWorldNear4.Z) / toInWorldNear4.W;

            worldLine.From = fromInWorldNear + MyCamera.Position;
            worldLine.To = toInWorldNear + MyCamera.Position;
        }

        public MyIntersectionResultLineTriangleEx? GetIntersectionInNearSpace(MyEntity entity, ref MyLine worldLine, bool convertLine)
        {
            if (!entity.IsVisible())
                 return null;

            MyLine line = worldLine;

            if (convertLine)
            {
                ConvertLineToNearWorldCoordinates(ref line);
            }

            Matrix drawMatrix = entity.GetWorldMatrixForDraw();
            drawMatrix.Translation += MyCamera.Position;
            Matrix worldInv = Matrix.Invert(drawMatrix);

            MyIntersectionResultLineTriangleEx? ret = entity.ModelLod0.GetTrianglePruningStructure().GetIntersectionWithLine(entity, ref line, ref worldInv, IntersectionFlags.ALL_TRIANGLES);
            if (ret == null)
            {
                foreach (MyEntity child in entity.Children)
                {
                    if (!child.IsVisible())
                        continue;

                    drawMatrix = child.GetWorldMatrixForDraw();
                    drawMatrix.Translation += MyCamera.Position;
                    worldInv = Matrix.Invert(drawMatrix);

                    System.Diagnostics.Debug.Assert(!float.IsNaN(worldInv.M11));

                    ret = child.ModelLod0.GetTrianglePruningStructure().GetIntersectionWithLine(child, ref line, ref worldInv, IntersectionFlags.ALL_TRIANGLES);
                    if (ret != null)
                        return ret;
                }
            }

            return ret;
        }
        private readonly WorkOptions m_workOptions = new WorkOptions { MaximumThreads = 1 };
        public WorkOptions Options { get { return m_workOptions; } }
        private List<MyEntity> m_helperCollection = new List<MyEntity>(20);
    }
}
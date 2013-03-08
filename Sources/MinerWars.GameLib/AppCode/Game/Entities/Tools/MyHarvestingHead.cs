namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using MinerWarsMath;
    using Models;
    using MinerWars.AppCode.Game.Render;

    sealed class MyHarvestingHead : MyEntity
    {
        Matrix m_worldMatrixForRenderingFromCockpitView = Matrix.Identity;

        public void Init(MyEntity parent)
        {
            base.Init(null, MyModelsEnum.HarvestingHead, null, parent, null, null);

            //  For performance optimizations, we don't check if this small objects are in a frustum and draw them directly
            m_frustumCheckBeforeDrawEnabled = false;
            Save = false;
        }

        public bool Update(Vector3 currentPosition, Vector3 direction, Vector3 up)
        {
            SetWorldMatrix(Matrix.CreateWorld(currentPosition, direction, up));
            
            return true;
        }

        public void SetData(ref Matrix worldMatrixForRenderingFromCockpitView)
        {
            m_worldMatrixForRenderingFromCockpitView = worldMatrixForRenderingFromCockpitView;
        }

        public override bool Draw(MyRenderObject renderObject)
        {
            if (base.Draw(renderObject) == false) return false;

            return true;
        }

        public override Matrix GetWorldMatrixForDraw()
        {
            if (MinerWars.AppCode.Game.GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == MinerWars.AppCode.Game.GUI.MyCameraAttachedToEnum.PlayerMinerShip)
            {
                return LocalMatrix * m_worldMatrixForRenderingFromCockpitView;
            }

            return base.GetWorldMatrixForDraw();
        }
    }
}

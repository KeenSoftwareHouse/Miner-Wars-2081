using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using MinerWarsMath;
    using Models;

    class MyDrillHead : MyEntity
    {
        public float RotationSpeed;
        Matrix m_worldMatrixForRenderingFromCockpitView;

        public virtual void Init(Vector3 offset, MyEntity parentObject, MyModelsEnum model)
        {
            base.Init(null, model, null, parentObject, null, null);
            LocalMatrix = Matrix.CreateTranslation(offset);
            RotationSpeed = 0;
            Save = false;
        }

        public void SetWorldMatrixForCockpit(ref Matrix worldMatrixForRenderingFromCockpitView)
        {
            m_worldMatrixForRenderingFromCockpitView = worldMatrixForRenderingFromCockpitView;
        }

        /// <summary>
        /// Updates resource.
        /// </summary>
        public override void UpdateBeforeSimulation()
        {
            base.UpdateBeforeSimulation();

            Matrix rotationMatrix = Matrix.CreateRotationZ(RotationSpeed * MyConstants.PHYSICS_STEP_SIZE_IN_SECONDS);

            LocalMatrix = rotationMatrix * LocalMatrix;
        }

        public override Matrix GetWorldMatrixForDraw()
        {
            if (GUI.MyGuiScreenGamePlay.Static.CameraAttachedTo == GUI.MyCameraAttachedToEnum.PlayerMinerShip)
            {
                return LocalMatrix * m_worldMatrixForRenderingFromCockpitView;
            }

            return base.GetWorldMatrixForDraw();
        }
    }
}

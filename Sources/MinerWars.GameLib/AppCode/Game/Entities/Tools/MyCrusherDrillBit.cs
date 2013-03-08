namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using System.Text;
    using MinerWarsMath;
    using Models;
    using Utils;
    using GUI;

    class MyDrillBit : MyEntity
    {
        float m_rotateSpeed;
        Matrix m_worldMatrixForRenderingFromCockpitView;

        public virtual void Init(StringBuilder hudLabelText, Matrix localMatrix, Vector3 offset, float rotateSpeed, MyCrusherDrill parentObject, MyModelsEnum model)
        {
            base.Init(hudLabelText, model, null, parentObject, null, null);
            m_rotateSpeed = rotateSpeed;
            this.LocalMatrix = Matrix.CreateTranslation(offset) * localMatrix;
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

            if (Parent.IsVisible())
            {
                Matrix rotationMatrix = Matrix.CreateRotationZ(m_rotateSpeed);// * Matrix.CreateFromYawPitchRoll(m_rotation.Y, m_rotation.X, m_rotation.Z);

                LocalMatrix = rotationMatrix * LocalMatrix;
            }
        }

        public override Matrix GetWorldMatrixForDraw()
        {
            if (MyGuiScreenGamePlay.Static.CameraAttachedTo == MyCameraAttachedToEnum.PlayerMinerShip && GetTopMostParent() == MinerWars.AppCode.Game.Managers.Session.MySession.PlayerShip)
            {
                return Matrix.CreateRotationZ(m_rotateSpeed) * LocalMatrix * m_worldMatrixForRenderingFromCockpitView;
            }

            return base.GetWorldMatrixForDraw();
        }

        public override bool DebugDraw()
        {
            return false;

            if (!base.DebugDraw())
                return false;

            MyDebugDraw.DrawAxis(WorldMatrix, 1.0f, 1);            

            return false;
        }
    }
}

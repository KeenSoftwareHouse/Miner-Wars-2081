using MinerWars.AppCode.Game.Entities;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Editor
{
    /// <summary>
    /// Holds transformation data used for entity transform action
    /// </summary>
    struct MyEditorTransformData
    {
        MyEntity m_entity;
        Vector3 m_position;
        Matrix m_orientation;

        public MyEditorTransformData(MyEntity entity, Vector3 position, Matrix orientation)
        {
            m_entity = entity;
            m_position = position;
            m_orientation = orientation;
        }

        public MyEntity GetEntity()
        {
            return m_entity;
        }

        public Vector3 GetPosition()
        {
            return m_position;
        }

        public Matrix GetOrientation()
        {
            return m_orientation;
        }
    }
}

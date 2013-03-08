namespace MinerWars.AppCode.Game.Entities.SubObjects
{
    using MinerWarsMath;
    using Utils;

    /// <summary>
    /// Configuration of small ship reflector.
    /// </summary>
    class MyReflectorConfig
    {
        public Vector4 CurrentReflectorLightColor;
        public float CurrentReflectorRangeForward;
        public float CurrentReflectorRangeBackward;
        public float CurrentReflectorConeAngleForward;
        public float CurrentReflectorConeAngleBackward;
        public float CurrentBillboardLength;
        public float CurrentBillboardThickness;

        MySmallShip m_ship;

        public MyReflectorConfig(MySmallShip ship)
        {
            m_ship = ship;
        }

        public void Update()
        {
            CurrentReflectorLightColor = Vector4.Lerp(MyReflectorConstants.SHORT_REFLECTOR_LIGHT_COLOR, MyReflectorConstants.LONG_REFLECTOR_LIGHT_COLOR, m_ship.Config.ReflectorLongRange.Level);
            CurrentReflectorRangeForward = MathHelper.Lerp(MyReflectorConstants.SHORT_REFLECTOR_RANGE_FORWARD, MyReflectorConstants.LONG_REFLECTOR_RANGE_FORWARD, m_ship.Config.ReflectorLongRange.Level);
            CurrentReflectorRangeBackward = MathHelper.Lerp(MyReflectorConstants.SHORT_REFLECTOR_RANGE_BACKWARD, MyReflectorConstants.LONG_REFLECTOR_RANGE_BACKWARD, m_ship.Config.ReflectorLongRange.Level);
            CurrentReflectorConeAngleForward = MathHelper.Lerp(MyReflectorConstants.SHORT_REFLECTOR_CONE_ANGLE_FORWARD, MyReflectorConstants.LONG_REFLECTOR_CONE_ANGLE_FORWARD, m_ship.Config.ReflectorLongRange.Level);
            CurrentReflectorConeAngleBackward = MathHelper.Lerp(MyReflectorConstants.SHORT_REFLECTOR_CONE_ANGLE_BACKWARD, MyReflectorConstants.LONG_REFLECTOR_CONE_ANGLE_BACKWARD, m_ship.Config.ReflectorLongRange.Level);
            CurrentBillboardLength = MathHelper.Lerp(MyReflectorConstants.SHORT_REFLECTOR_BILLBOARD_LENGTH, MyReflectorConstants.LONG_REFLECTOR_BILLBOARD_LENGTH, m_ship.Config.ReflectorLongRange.Level);
            CurrentBillboardThickness = MathHelper.Lerp(MyReflectorConstants.SHORT_REFLECTOR_BILLBOARD_THICKNESS, MyReflectorConstants.LONG_REFLECTOR_BILLBOARD_THICKNESS, m_ship.Config.ReflectorLongRange.Level);

        }
    }
}

namespace MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons
{
    class MyLargeShipWeaponCIWS //: MyPhysObjectBaseNoJLX
    {

        //float m_rotationAngle;                          //  Actual rotation angle (not rotation speed) around Z axis
        //Vector3 m_forward;
        //Vector3 m_up;

        //List<MyLargeShipGunBase> m_gunParts;

        //public virtual void Init(StringBuilder hudLabelText, Vector3 position, MyLargeShip parentObject, MyMwcObjectBuilder_LargeShip_Weapon objectBuilder)
        //{
        //    base.Init(hudLabelText, MyModelsEnum.Turret_CIWS_Base, MyPhysObjectMaterialType.METAL, parentObject, position, objectBuilder);
        //    m_positionRelative = position;
        //    m_forward = objectBuilder.PositionAndOrientation.Forward;
        //    m_up = objectBuilder.PositionAndOrientation.Up;
        //    m_gunParts = new List<MyLargeShipGunBase>();
        //    MyLargeShipWeaponCiwsGun m_turretGun = new MyLargeShipWeaponCiwsGun();
        //    m_turretGun.Init(null, m_positionRelative, this, objectBuilder);
        //    m_gunParts.Add(m_turretGun);
        //}

        //public override bool IsVisible()
        //{
        //    return base.IsVisible();
        //}

        ////  Draw muzzle flash not matter if in frustum (it's because it's above the frustum)
        //public override bool Draw()
        //{
        //    if (base.Draw() == false) return false;
        //    foreach(MyGunBase gunPart in m_gunParts)
        //    {
        //        gunPart.Draw();
        //    }

        //    return true;
        //}

        //public override void UpdateAfterIntegration()
        //{
        //    base.UpdateAfterIntegration();
        //    if (IsInVisibleRange(TargetPosition()))
        //    {
        //        Vector2 rotIndicator = RotationTowardsTarget(WorldMatrixInverted, TargetWorldMatrix());
        //        Vector2 normalizedRotationIndicator = Vector2.Normalize(rotIndicator);
        //        m_rotationAngle -= normalizedRotationIndicator.Y;
        //    }
        //    else
        //    {
        //        //TODO temporary
        //        m_rotationAngle -= 0.01f;
        //    }
        //    UpdateWorldMatrix();

        //    foreach (MyGunBase gunPart in m_gunParts)
        //    {
        //        gunPart.UpdateAfterIntegration();
        //    }
        //}

        //public override void UpdateWorldMatrix()
        //{    
        //    base.UpdateWorldMatrix();
        //    Matrix rotationMatrix = Matrix.CreateRotationY(m_rotationAngle) * Matrix.CreateFromYawPitchRoll(m_forward.Y, m_forward.X, m_forward.Z);
        //    WorldMatrix = rotationMatrix * Matrix.CreateTranslation(m_positionRelative) * Parent.WorldMatrix;
        //    UpdateWorldMatrixInvertedAndBoundingSphere();
        //    foreach (MyGunBase gunPart in m_gunParts)
        //    {
        //        gunPart.UpdateWorldMatrix();
        //    }
        //}

        //public override MyLineTriangleIntersectionResult GetIntersectionWithLine(ref MyLine line)
        //{
        //    MyLineTriangleIntersectionResult result = ModelLod0.GetOctree().GetIntersectionWithLine(this, ref line);
        //    //  Test against childs of this phys object (in this case gun parts)
        //    foreach (MyGunBase gunPart in m_gunParts)
        //    {
        //        MyLineTriangleIntersectionResult intersectionGunPart = gunPart.GetIntersectionWithLine(ref line);

        //        if (((result.Found == false) && (intersectionGunPart.Found == true)) ||
        //            ((result.Found == true) && (intersectionGunPart.Found == true) && (intersectionGunPart.Distance < result.Distance)))
        //        {
        //            result = intersectionGunPart;
        //        }
        //    }
        //    return result;
        //}
    }
}
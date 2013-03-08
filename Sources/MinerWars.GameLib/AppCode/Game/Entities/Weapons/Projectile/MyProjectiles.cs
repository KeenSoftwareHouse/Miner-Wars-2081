using MinerWars.CommonLIB.AppCode.Generics;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Collections.Generic;
    using MinerWarsMath;
    using SysUtils.Utils;
    using Utils;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.CommonLIB.AppCode.Utils;
    using MinerWars.AppCode.Game.TransparentGeometry;
    using MinerWars.AppCode.Game.Render;

    static class MyProjectiles
    {
        static MyObjectsPool<MyProjectile> m_projectiles = null;

        static MyProjectiles()
        {
            Render.MyRender.RegisterRenderModule(MyRenderModuleEnum.Projectiles, "Projectiles", Draw, Render.MyRenderStage.PrepareForDraw);
        }

        public static void LoadData()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyProjectiles.LoadData");
            if (m_projectiles == null)
            {
                m_projectiles = new MyObjectsPool<MyProjectile>(MyProjectilesConstants.MAX_PROJECTILES_COUNT);
            }
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadData()
        {
            m_projectiles.DeallocateAll();
        }

        //  Add new projectile to the list
        public static void Add(MyAmmoProperties ammoProperties, MyEntity ignorePhysObject, Vector3 origin, Vector3 initialVelocity, Vector3 directionNormalized, bool groupStart, float thicknessMultiplier, MyEntity weapon, MyEntity ownerEntity = null)
        {
            MyProjectile newProjectile = m_projectiles.Allocate();
            if (newProjectile != null)
            {
                newProjectile.Start(
                    ammoProperties,
                    ignorePhysObject, 
                    origin, 
                    initialVelocity, 
                    directionNormalized, 
                    groupStart, 
                    thicknessMultiplier,
                    weapon
                    );
                newProjectile.OwnerEntity = ownerEntity != null ? ownerEntity : ignorePhysObject;
            }
        }

        //  Add new projectile to the list
        public static void AddShotgun(MyAmmoProperties ammoProperties, MyEntity ignorePhysObject, Vector3 origin, Vector3 initialVelocity, Vector3 directionNormalized, bool groupStart, float thicknessMultiplier, MyEntity weapon, float frontBillboardSize, MyEntity ownerEntity = null)
        {
            MyProjectile newProjectile = m_projectiles.Allocate();
            if (newProjectile != null)
            {
                newProjectile.Start(
                    ammoProperties,
                    ignorePhysObject,
                    origin,
                    initialVelocity,
                    directionNormalized,
                    groupStart,
                    thicknessMultiplier,
                    weapon
                    );

                newProjectile.BlendByCameraDirection = true;
                newProjectile.FrontBillboardMaterial = MyTransparentMaterialEnum.ShotgunParticle;
                newProjectile.LengthMultiplier = 2;
                newProjectile.FrontBillboardSize = frontBillboardSize;
                newProjectile.OwnerEntity = ownerEntity != null ? ownerEntity : ignorePhysObject;
            }
        }

        //  Update active projectiles. If projectile dies/timeouts, remove it from the list.
        public static void Update()
        {
            foreach (LinkedListNode<MyProjectile> item in m_projectiles)
            {
                if (item.Value.Update() == false)
                {
                    item.Value.Close();
                    m_projectiles.MarkForDeallocate(item);
                }
            }

            m_projectiles.DeallocateAllMarked();
        }

        //  Draw active projectiles
        public static void Draw()
        {
            foreach (LinkedListNode<MyProjectile> item in m_projectiles)
            {
                item.Value.Draw();
            }
        }

        public static int GetActiveCount()
        {
            return m_projectiles.GetActiveCount();
        }
    }
}

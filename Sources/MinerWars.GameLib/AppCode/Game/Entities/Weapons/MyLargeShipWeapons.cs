namespace MinerWars.AppCode.Game.Managers.EntityManager.Entities.Weapons
{
    using System.Collections.Generic;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.LargeShipTools;
    using Microsoft.Xna.Framework;
    using Models;
    using SysUtils.Utils;
    using Utils;
    using System.Text;

    class MyLargeShipWeapons
    {
        MyLargeShip m_ship;

        List<MyGunBase> m_guns;
        List<MyMwcObjectBuilder_LargeShip_Weapon> m_weaponsObjectBuilders;
        List<MyMwcObjectBuilder_LargeShip_Ammo> m_ammoObjectBuilders;

        public int MuzzleFlashLastTime { get; set; }

        public MyLargeShipWeapons(MyLargeShip ship, 
            List<MyMwcObjectBuilder_LargeShip_Weapon> weaponsObjectBuilders, List<MyMwcObjectBuilder_LargeShip_Ammo> ammoObjectBuilders)
        {
            m_ship = ship;
            MuzzleFlashLastTime = MyConstants.FAREST_TIME_IN_PAST;

            //  This object builders are needed even after ship is initialized because we may need to know what is in the ship in later time
            m_weaponsObjectBuilders = weaponsObjectBuilders;
            m_ammoObjectBuilders = ammoObjectBuilders;

            //  Create phys-objects weapons from weapon object builders
            m_guns = new List<MyGunBase>();
            if (weaponsObjectBuilders != null)
            {
                foreach (MyMwcObjectBuilder_LargeShip_Weapon weaponObjectBuilder in weaponsObjectBuilders)
                {
                    AddWeapon(weaponObjectBuilder);
                }
            }
        }

        //  Adds one weapon or device to this large ship
        void AddWeapon(MyMwcObjectBuilder_LargeShip_Weapon weaponObjectBuilder)
        {
            Vector3 position =  new Vector3(weaponObjectBuilder.PositionAndOrientation.Position.X, weaponObjectBuilder.PositionAndOrientation.Position.Y, weaponObjectBuilder.PositionAndOrientation.Position.Z);

            switch (weaponObjectBuilder.WeaponType)
            {
                case MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.CIWS:
                    {
                        //MyLargeShipWeaponCIWS newGun = new MyLargeShipWeaponCIWS();
                        //newGun.Init(new StringBuilder("CIWS gun"), position, m_ship, weaponObjectBuilder);
                        //m_guns.Add(newGun);
                        break;
                    }
                case MyMwcObjectBuilder_LargeShip_Weapon_TypesEnum.MachineGun:
                    {
                        MyLargeShipMachineGun newGun = new MyLargeShipMachineGun();
                        newGun.Init(new StringBuilder("Large machine gun"), m_ship, position, m_ship.WorldMatrix.Forward, m_ship.WorldMatrix.Up, null);
                        m_guns.Add(newGun);
                    }
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
                    break;
            }
        }



        public List<MyMwcObjectBuilder_LargeShip_Weapon> GetWeaponsObjectBuilders()
        {
            return m_weaponsObjectBuilders;
        }

        public List<MyMwcObjectBuilder_LargeShip_Ammo> GetAmmoObjectBuilders()
        {
            return m_ammoObjectBuilders;
        }

        public MyIntersectionResultLineTriangleEx? GetIntersectionWithLine(ref MyLine line)
        {
            MyIntersectionResultLineTriangleEx? result = null;

            //  Test against childs of this phys object (in this case guns)
            foreach (MyGunBase gun in m_guns)
            {
                MyIntersectionResultLineTriangleEx? intersectionGun = gun.GetIntersectionWithLine(ref line);

                result = MyIntersectionResultLineTriangleEx.GetCloserIntersection(ref result, ref intersectionGun);
            }

            return result;
        }

        public List<MyGunBase> GetGuns()
        {
            return m_guns;
        }

        public void UpdateAfterIntegration()
        {
            foreach (MyGunBase gun in m_guns)
            {
                gun.UpdateAfterIntegration();
            }
        }

        public void Draw()
        {
            foreach (MyGunBase gun in m_guns)
            {
                gun.Draw();
            }
        }

        public void DrawNormalVectors()
        {
            foreach (MyGunBase gun in m_guns)
            {
                gun.DebugDrawNormalVectors();
            }
        }

        public void Close()
        {
            for (int i = 0; i < m_guns.Count; i++)
            {
                m_guns[i].Close();
            }
        }
    }
}

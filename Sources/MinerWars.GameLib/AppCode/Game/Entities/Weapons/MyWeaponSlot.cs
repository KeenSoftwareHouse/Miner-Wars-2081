using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Models;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using System.Reflection;
using KeenSoftwareHouse.Library.Extensions;
using SysUtils.Utils;
using System.Diagnostics;

namespace MinerWars.AppCode.Game.Entities.Weapons
{
    delegate void OnWeaponMounting(MyWeaponSlot sender, MySmallShipGunBase weapon);
    delegate void OnWeaponDismouting(MyWeaponSlot sender, MySmallShipGunBase weapon);

    public enum MySlotLocationEnum
    {
        None,
        LeftSide,
        RightSide,
    }

    class MyWeaponSlot
    {
        private const string SUB_OBJECT_Y_OFFSET_KEY = "YOffset";
        private const string SUB_OBJECT_Z_OFFSET_KEY = "ZOffset";
        const int MINIMUM_DISTANCE_FOR_SIDE_SLOT = 1;

        private MySmallShipGunBase m_mountedWeapon;

        /// <summary>
        /// Mounted weapon in this slot
        /// </summary>
        public MySmallShipGunBase MountedWeapon 
        {
            get 
            {
                return m_mountedWeapon;
            }
            private set 
            {
                m_mountedWeapon = value;
            }
        }

        MyModelSubObject m_weaponSubObject;

        /// <summary>
        /// Weapon's subobject
        /// </summary>
        public MyModelSubObject WeaponSubObject
        {
            get { return m_weaponSubObject; }
            set
            {
                m_weaponSubObject = value;
                if (value != null)
                {
                    UpdateSlotLocation();
                }
            }
        }

        public string SlotDescriptor
        {
            get { return WeaponSubObject.Name; }
        }

        public MySlotLocationEnum SlotLocation { get; set; }

        void UpdateSlotLocation()
        {
            if (WeaponSubObject.Position.X > 0.95f)
            {
                SlotLocation = MySlotLocationEnum.RightSide;
            }
            else if (WeaponSubObject.Position.X < -0.95f)
            {
                SlotLocation = MySlotLocationEnum.LeftSide;
            }
        }

        public MyWeaponSlot()
        {

        }

        /// <summary>
        /// Called when weapon mounted on slot
        /// </summary>
        public event OnWeaponMounting OnWeaponMounting;

        /// <summary>
        /// Called when weapon dismounted from slot
        /// </summary>
        public event OnWeaponDismouting OnWeaponDismouting;

        /// <summary>
        /// Returns true if weapon is mounted in this slot
        /// </summary>
        /// <returns></returns>
        public bool IsMounted()
        {
            return MountedWeapon != null;
        }

        /// <summary>
        /// Dismounts weapon from this slot
        /// </summary>
        public void Dismount()
        {
            if (OnWeaponDismouting != null)
            {
                OnWeaponDismouting(this, MountedWeapon);
            }
            MountedWeapon = null;
        }

        /// <summary>
        /// Mounts weapon to this slot
        /// </summary>
        /// <param name="weapon"></param>
        public void Mount(MySmallShipGunBase weapon)
        {
            if (OnWeaponMounting != null)
            {
                OnWeaponMounting(this, weapon);
            }
            MountedWeapon = weapon;            
        }

        /// <summary>
        /// Inits weapon and mount to this slot
        /// </summary>
        /// <param name="weapon">Weapon</param>
        /// <param name="objectBuilder">Weapon's objectbuilder</param>
        /// <param name="parentShip">Parent ship</param>
        public void InitAndMount(MySmallShipGunBase weapon, MyMwcObjectBuilder_SmallShip_Weapon objectBuilder, MySmallShip parentShip)
        {
            Debug.Assert(WeaponSubObject != null);
            weapon.Init(null, parentShip, GetPosition(), GetForwardVector(), GetUpVector(), objectBuilder);
            float? yOffset = GetOffsetFromWeaponSubObject(SUB_OBJECT_Y_OFFSET_KEY);
            if (yOffset != null)
            {
                weapon.YOffset = yOffset.Value;
            }

            float? zOffset = GetOffsetFromWeaponSubObject(SUB_OBJECT_Z_OFFSET_KEY);
            if (zOffset != null)
            {
                weapon.ZOffset = zOffset.Value;
            }
            Mount(weapon);
        }

        /// <summary>
        /// Returns z-offset from weapon's subobject
        /// </summary>
        /// <param name="offsetKey"> </param>
        /// <returns></returns>
        private float? GetOffsetFromWeaponSubObject(string offsetKey)
        {
            Debug.Assert(WeaponSubObject != null);
            if (WeaponSubObject != null && WeaponSubObject.CustomData != null)
            {
                object offsetValue;
                if (WeaponSubObject.CustomData.TryGetValue(offsetKey, out offsetValue)/* && zOffsetValue is float*/)
                {
                    try
                    {
                        CultureInfo ci = (CultureInfo) CultureInfo.CurrentCulture.Clone();
                        ci.NumberFormat.CurrencyDecimalSeparator = ".";
                        return float.Parse(offsetValue.ToString(), NumberStyles.Any, ci);                                                
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns weapon's position from weapon's subobject
        /// </summary>
        /// <returns></returns>
        private Vector3 GetPosition()
        {
            Debug.Assert(WeaponSubObject != null);
            if (WeaponSubObject == null)
                return Vector3.Zero;
            return WeaponSubObject.Position;
        }

        /// <summary>
        /// Returns up vector from weapon's subobject
        /// </summary>
        /// <returns></returns>
        private Vector3 GetUpVector()
        {
            Debug.Assert(WeaponSubObject != null);
            if (WeaponSubObject == null)
                return Vector3.Up;
            return WeaponSubObject.UpVector.Value;            
        }

        /// <summary>
        /// Returns forward vector from weapon's subobject
        /// </summary>
        /// <returns></returns>
        private Vector3 GetForwardVector()
        {
            Debug.Assert(WeaponSubObject != null);
            if (WeaponSubObject == null)
                return Vector3.Forward;
            return WeaponSubObject.ForwardVector.Value;
        }

        /// <summary>
        /// Returns prefered weapon's type in this slot
        /// </summary>
        /// <returns></returns>
        public MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum GetPreferedWeaponType()
        {
            Debug.Assert(WeaponSubObject != null);
            return (MyMwcObjectBuilder_SmallShip_Weapon_TypesEnum)WeaponSubObject.AuxiliaryParam0;            
        }

        /// <summary>
        /// Returns weapon's object builder
        /// </summary>
        /// <returns></returns>
        public MyMwcObjectBuilder_SmallShip_Weapon GetWeaponObjectBuilder(bool getExactCopy)
        {
            if (MountedWeapon == null)
            {
                return null;
            }
            else
            {
                return (MyMwcObjectBuilder_SmallShip_Weapon)MountedWeapon.GetObjectBuilder(getExactCopy);
            }
        }

        public void Close() 
        {
            if (MountedWeapon != null) 
            {
                MountedWeapon.Close();
                MountedWeapon = null;
            }
            WeaponSubObject = null;
        }

        public bool IsSlotEligibleForLocation(MySlotLocationEnum slotLocation)
        {
            if (slotLocation == MySlotLocationEnum.None)
            {
                return true;
            }

            return slotLocation == this.SlotLocation;
        }
    }
}

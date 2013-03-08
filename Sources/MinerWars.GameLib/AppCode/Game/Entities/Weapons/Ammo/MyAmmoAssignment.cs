using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
using MinerWars.AppCode.Game.Entities.Weapons.UniversalLauncher;
using System.Reflection;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.Entities.Weapons.Ammo
{
    class MyAmmoAssignment
    {
        private MyMwcObjectBuilder_FireKeyEnum m_fireKey;

        private MyMwcObjectBuilder_AmmoGroupEnum m_ammoGroup;

        private MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum m_ammoType;

        /// <summary>
        /// Fire key
        /// </summary>
        public MyMwcObjectBuilder_FireKeyEnum FireKey 
        {
            get 
            {
                return m_fireKey;
            }
            set 
            {
                m_fireKey = value;
            }
        }

        /// <summary>
        /// Ammo group
        /// </summary>
        public MyMwcObjectBuilder_AmmoGroupEnum AmmoGroup 
        {
            get 
            {
                return m_ammoGroup;
            }
            set 
            {
                m_ammoGroup = value;
            }
        }

        /// <summary>
        /// Ammo type
        /// </summary>
        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum AmmoType 
        {
            get 
            {
                return m_ammoType;
            }
            set 
            {
                m_ammoType = value;
            }
        }

        /// <summary>
        /// Creates new instance of ammo assignment
        /// </summary>
        /// <param name="fireKey">Fire key</param>
        /// <param name="ammoGroup">Ammo group</param>
        /// <param name="ammoType">Ammo type</param>
        public MyAmmoAssignment(MyMwcObjectBuilder_FireKeyEnum fireKey, MyMwcObjectBuilder_AmmoGroupEnum ammoGroup, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            FireKey = fireKey;
            AmmoGroup = ammoGroup;
            AmmoType = ammoType;
        }

        /// <summary>
        /// Creates new instance of ammo assignment
        /// </summary>
        /// <param name="assignmentOfAmmo">Assignment of ammo object builder</param>        
        public MyAmmoAssignment(MyMwcObjectBuilder_AssignmentOfAmmo assignmentOfAmmo)
            : this(assignmentOfAmmo.FireKey, assignmentOfAmmo.Group, assignmentOfAmmo.AmmoType)
        {
        }

        public MyAmmoAssignment() { }

        public override int GetHashCode()
        {
            return (int)FireKey ^ 3 + (int)AmmoGroup ^ 7 + (int)AmmoType ^ 11;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MyAmmoAssignment);
        }

        public bool Equals(MyAmmoAssignment other) 
        {
            if (other == null) return false;

            return GetHashCode() == other.GetHashCode();
        }
    }

    class MyAmmoAssignments
    {
        // collection of ammo assignment        
        private Dictionary<int, MyAmmoAssignment> m_ammoAssignmentCollection;
        private MySmallShipWeapons m_smallShipWeapons;

        public MyAmmoAssignments(MySmallShipWeapons smallShipWeapons)
        {
            m_smallShipWeapons = smallShipWeapons;
            m_ammoAssignmentCollection = new Dictionary<int, MyAmmoAssignment>();
        }
        
        /// <summary>
        /// Initialize assignments of ammo from objectbuilders
        /// </summary>
        /// <param name="assignmentOfAmmoObjectBuilders">Assignment of ammo objectbuilders collection</param>
        public void Init(List<MyMwcObjectBuilder_AssignmentOfAmmo> assignmentOfAmmoObjectBuilders)
        {
            Debug.Assert(assignmentOfAmmoObjectBuilders != null);
            m_ammoAssignmentCollection.Clear();
            foreach (MyMwcObjectBuilder_AssignmentOfAmmo objectBuilder in assignmentOfAmmoObjectBuilders)
            {                
                AssignAmmo(objectBuilder.FireKey, objectBuilder.Group, objectBuilder.AmmoType);
            }

            // non-remappable assignments
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.HologramFront, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.HologramBack, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Hologram);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.BasicMineFront, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.BasicMineBack, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Basic);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.SmartMineFront, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.SmartMineBack, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Mine_Smart);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.FlashBombFront, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.FlashBombBack, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Flash_Bomb);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.DecoyFlareFront, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.DecoyFlareBack, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Decoy_Flare);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.SmokeBombFront, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherFront, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb);
            AssignAmmo(MyMwcObjectBuilder_FireKeyEnum.SmokeBombBack, MyMwcObjectBuilder_AmmoGroupEnum.UniversalLauncherBack, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Smoke_Bomb);            
        }        

        /// <summary>
        /// Returns collection of assignment of ammo objectbuilders
        /// </summary>
        /// <returns></returns>
        public List<MyMwcObjectBuilder_AssignmentOfAmmo> GetObjectBuilder()
        {
            List<MyMwcObjectBuilder_AssignmentOfAmmo> objectBuilders = new List<MyMwcObjectBuilder_AssignmentOfAmmo>();
            foreach (var ammoAssignmentKVP in m_ammoAssignmentCollection)
            {
                MyAmmoAssignment ammoAssignment = ammoAssignmentKVP.Value;
                objectBuilders.Add(new MyMwcObjectBuilder_AssignmentOfAmmo(ammoAssignment.FireKey, ammoAssignment.AmmoGroup, ammoAssignment.AmmoType));
            }
            return objectBuilders;
        }

        /// <summary>
        /// Assign ammo group, ammo type to fire key
        /// </summary>
        /// <param name="fireKey">Fire key</param>
        /// <param name="ammoGroup">Ammo group</param>
        /// <param name="ammoType">Ammmo type</param>
        public void AssignAmmo(MyMwcObjectBuilder_FireKeyEnum fireKey, MyMwcObjectBuilder_AmmoGroupEnum ammoGroup, MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            MyAmmoAssignment ammoAssignment = GetAmmoAssignment(fireKey);
            if (ammoAssignment != null)
            {
                ammoAssignment.AmmoGroup = ammoGroup;
                ammoAssignment.AmmoType = ammoType;
            }
            else
            {
                m_ammoAssignmentCollection.Add((int)fireKey, new MyAmmoAssignment(fireKey, ammoGroup, ammoType));
            }
        }

        /// <summary>
        /// Returns ammo assignment by fire key (ammo group, ammo type)
        /// </summary>
        /// <param name="fireKey">Fire key</param>
        /// <param name="ammoGroup">Ammo group</param>
        /// <param name="ammoType">Ammmo type</param>
        /// <returns></returns>
        public MyAmmoAssignment GetAmmoAssignment(MyMwcObjectBuilder_FireKeyEnum fireKey)
        {
            if (m_ammoAssignmentCollection.ContainsKey((int)fireKey)) 
            {
                return m_ammoAssignmentCollection[(int)fireKey];
            }
            return null;            
        }

        /// <summary>
        /// Returns ammo type by fire key
        /// </summary>
        /// <param name="key">Fire key</param>
        /// <returns></returns>
        public MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum GetAmmoType(MyMwcObjectBuilder_FireKeyEnum key)
        {
            MyAmmoAssignment ammoAssignment = GetAmmoAssignment(key);
            if (ammoAssignment == null)
            {
                return 0;
            }
            else
            {
                return ammoAssignment.AmmoType;
            }
        }

        /// <summary>
        /// Returns count of ammo assignment
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return m_ammoAssignmentCollection.Count;
        }

        /// <summary>
        /// Returns ammo special text by fire key
        /// </summary>
        /// <param name="key">Fire key</param>
        /// <returns></returns>
        public StringBuilder GetAmmoSpecialText(MyMwcObjectBuilder_FireKeyEnum key)
        {
            //Get ammo type
            MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType = GetAmmoType(key);
            if (ammoType == 0) return null;

            return GetAmmoSpecialText(ammoType);
        }

        /// <summary>
        /// Close method
        /// </summary>
        public void Close() 
        {
            m_ammoAssignmentCollection.Clear();
            m_ammoAssignmentCollection = null;
            m_smallShipWeapons = null;
        }

        /// <summary>
        /// Returns ammo special text from ammo type
        /// </summary>
        /// <param name="ammoType">Ammo type</param>
        /// <returns></returns>
        private StringBuilder GetAmmoSpecialText(MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum ammoType)
        {
            switch (ammoType)
            {
                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Time_Bomb:
                    return MyTimeBomb.GetAmmoSpecialText();
                    break;

                case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Bomb:
                    return MyRemoteBomb.GetAmmoSpecialText();
                    break;

                //case MyMwcObjectBuilder_SmallShip_Ammo_TypesEnum.Universal_Launcher_Remote_Camera:
                //    return MyRemoteCamera.GetAmmoSpecialText();
                //    break;

                default:
                    break;
            }

            return MyMwcUtils.EmptyStringBuilder;
        }
    }    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.AppCode.Game.GUI;
using System.Diagnostics;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Sessions
{
    class MyMotherShipPosition 
    {
        public const string MADELYN_NAME = "MDHangar";
        private const string MADELYN_DISPLAY_NAME = "Madelyn's Sapho";
        private const string MADELYN_POSITION_DICTIONARY_KEY = "MadelynPosition";

        private StringBuilder m_madelynName = new StringBuilder(MADELYN_DISPLAY_NAME);
        public StringBuilder Name 
        { 
            get 
            { 
                return m_madelynName; 
            } 
        }

        private MyMwcVector3Int? m_sectorPosition;
        private MyMwcVector3Int? SectorPosition 
        {
            get 
            {
                return m_sectorPosition;
            }
            set 
            {
                if (m_sectorPosition != value) 
                {
                    m_sectorPosition = value;
                    m_hudPositionDirty = true;
                }
            }
        }

        private Vector3? m_positionInSector;
        private Vector3? PositionInSector 
        {
            get 
            {
                return m_positionInSector;
            }
            set 
            {
                if (m_positionInSector != value) 
                {
                    m_positionInSector = value;
                    m_hudPositionDirty = true;
                }
            }
        }

        private bool m_hudPositionDirty;
        private Vector3 m_hudPosition;
        public Vector3 HudPosition
        {
            get 
            {
                if (m_hudPositionDirty)
                {
                    Debug.Assert(Exist);

                    MyMwcVector3Int currentSectorPosition = MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position;
                    Vector3 sectorPositionVector =
                        new Vector3(
                            SectorPosition.Value.X - currentSectorPosition.X,
                            SectorPosition.Value.Y - currentSectorPosition.Y,
                            SectorPosition.Value.Z - currentSectorPosition.Z) * MyMwcSectorConstants.SECTOR_SIZE;

                    m_hudPosition = sectorPositionVector + PositionInSector.Value;
                    m_hudPositionDirty = false;
                }
                return m_hudPosition;
            }
        }

        public bool Exist 
        {
            get 
            {
                return SectorPosition != null && PositionInSector != null;
            }
        }

        private MyEntity m_madelynEntity;

        public MyMotherShipPosition() 
        {
            MyEntities.OnEntityNameSet += MyEntities_OnEntityNameSet;
            m_hudPositionDirty = true;
        }

        public void Load(Dictionary<string, string> checkpointDictionary)
        {
            if(checkpointDictionary.ContainsKey(MADELYN_POSITION_DICTIONARY_KEY))
            {
                Deserialize(checkpointDictionary[MADELYN_POSITION_DICTIONARY_KEY]);
            }
            m_hudPositionDirty = true;
        }

        public void Save(Dictionary<string, string> checkpointDictionary)
        {
            if (Exist)
            {
                checkpointDictionary[MADELYN_POSITION_DICTIONARY_KEY] = Serialize();
            }
            else 
            {
                checkpointDictionary.Remove(MADELYN_POSITION_DICTIONARY_KEY);
            }
        }

        public void Update() 
        {
            if (m_madelynEntity != null) 
            {
                SectorPosition = MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position;
                PositionInSector = m_madelynEntity.GetPosition();
            }
        }

        public bool IsNotInSameSector() 
        {
            Debug.Assert(Exist);
            MyMwcVector3Int currentSector = MyGuiScreenGamePlay.Static.GetSectorIdentifier().Position;
            return SectorPosition.Value.X != currentSector.X ||
                   SectorPosition.Value.Y != currentSector.Y ||
                   SectorPosition.Value.Z != currentSector.Z;
        }

        // serialization format: spx,spy,spz;pisx,pisy,pisz
        private void Deserialize(string serialize) 
        {
            try
            {
                string[] positions = serialize.Split(';');
                // sector positions
                string[] sectorPositions = positions[0].Split(',');
                MyMwcVector3Int sectorPosition = new MyMwcVector3Int(int.Parse(sectorPositions[0]), int.Parse(sectorPositions[1]), int.Parse(sectorPositions[2]));

                // positions in sector
                string[] positionsInSector = positions[1].Split(',');
                Vector3 positionInSector = new Vector3(int.Parse(positionsInSector[0]), int.Parse(positionsInSector[1]), int.Parse(positionsInSector[2]));

                SectorPosition = sectorPosition;
                PositionInSector = positionInSector;
            }
            catch (Exception ex)
            {
                MyMwcLog.WriteLine("Wrong madelyn's position deserialization! value:" + serialize + " " + ex.Message);
                Debug.Fail("Wrong madelyn's position deserialization!");
            }
        }

        private string Serialize() 
        {
            Debug.Assert(Exist);
            string serializeValue = string.Empty;
            serializeValue += SectorPosition.Value.X + "," + SectorPosition.Value.Y + "," + SectorPosition.Value.Z + ";" +
                              ((int)PositionInSector.Value.X) + "," + ((int)PositionInSector.Value.Y) + "," + ((int)PositionInSector.Value.Z);
            return serializeValue;
        }
            
        void MyEntities_OnEntityNameSet(MyEntity entity, string oldName, string newName)
        {
            if (newName == MADELYN_NAME) 
            {
                if (m_madelynEntity == null)
                {
                    m_madelynEntity = entity;
                    m_madelynEntity.OnClose += m_madelynEntity_OnClose;
                }
            }
            else if (oldName == MADELYN_NAME) 
            {
                if (m_madelynEntity != null)
                {
                    m_madelynEntity.OnClose -= m_madelynEntity_OnClose;
                    m_madelynEntity = null;
                }
            }
        }

        void m_madelynEntity_OnClose(MyEntity obj)
        {
            m_madelynEntity.OnClose -= m_madelynEntity_OnClose;
            m_madelynEntity = null;
        }
    }
}

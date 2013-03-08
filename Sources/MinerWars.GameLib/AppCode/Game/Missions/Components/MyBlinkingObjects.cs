using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.Utils;

namespace MinerWars.AppCode.Game.Missions.Components
{
    class MyBlinkingObjects : MyMissionComponent
    {

        private List<MyEntity> m_entities;
        private List<uint> m_entitiesIds;
        private Vector3 m_color;
        private int m_nextChange;
        private bool m_highlited;
        private int m_blinkingPeriod;
        private bool m_enabled;


        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value;
                m_highlited = false;
                UpdateHighlighting();
            }
        }

        public MyBlinkingObjects(List<uint> objects, int blinkingTime = 250, Vector3 ?color = null)
        {
            if (color == null) m_color = MyMissionsConstants.OBJECT_HIGHTLIGHT_COLOR;
            else m_color = color.Value;
            m_entitiesIds = objects;
            m_blinkingPeriod = blinkingTime;
            m_enabled = true;
            m_entities = new List<MyEntity>();
        }


        public override void Update(MyMissionBase sender)
        
        {
            base.Update(sender);
            if (m_enabled)
            {
                if (sender.MissionTimer.ElapsedTime > m_nextChange)
                {
                    m_nextChange = sender.MissionTimer.ElapsedTime + m_blinkingPeriod;
                    
                    m_highlited = !m_highlited;
                    if (m_highlited) m_nextChange = sender.MissionTimer.ElapsedTime + 2*m_blinkingPeriod;
                    UpdateHighlighting();
                }
            }
        }

        private void UpdateHighlighting()
        {   

            foreach (var myEntity in m_entities)
            {
                if (m_highlited)
                {
                    myEntity.HighlightEntity(ref m_color);
                    if (!myEntity.Closed && myEntity.Activated)
                    {
                        myEntity.Visible = true;
                    }
                    
                }
                else
                {
                    myEntity.ClearHighlightning();
                    //myEntity.Visible = false;
                }
            }
        }


        public void AddBlinkingObject(MyEntity entity)
        {
            if (entity == null) return;

            //If not handled here, it will make harvester+drill visible on highlighted ship
            if ((entity is MyDrillBase) || (entity is MinerWars.AppCode.Game.Entities.SubObjects.MyHarvestingDevice))
                return;

            m_entities.Add(entity);
            if (entity.Children != null)
            {
                foreach (var child in entity.Children)
                {
                    AddBlinkingObject(child);
                }
            }
        }

        public void RemoveBlinkingObject(MyEntity entity)
        {   
            entity.ClearHighlightning();
            
            while (m_entities.Remove(entity)) { };  // Remove all
            
            if (entity.Children != null)
            {
                foreach (var child in entity.Children)
                {
                    RemoveBlinkingObject(child);
                }
            }
        }


        public override void Success(MyMissionBase sender)
        {
            base.Success(sender);
        }

        public override void Load(MyMissionBase sender)
        {
            base.Load(sender);
           /*
            m_entities = new List<MyEntity>();
            foreach (var entitiesId in m_entitiesIds)
            {
                var entity = MyScriptWrapper.TryGetEntity(entitiesId);
                if (entity!=null)
                {
                    m_entities.Add(entity);
                }
            }
            * */
        }

        public override void Unload(MyMissionBase sender)
        {
            m_entities.Clear();
            m_nextChange = 0;
            Enabled = true;
        }
    }
}

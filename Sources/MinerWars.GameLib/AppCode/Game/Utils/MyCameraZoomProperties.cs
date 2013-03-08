using System;
using MinerWarsMath;

using MinerWars.AppCode.Game.Audio;

namespace MinerWars.AppCode.Game.Utils
{
    enum MyCameraZoomOperationType
    {
        ZoomOut = 1,
        ZoomIn = -1,
        NoZoom = 0
    }

    //  Zooming 
    class MyCameraZoomProperties
    {
        float m_zoomTime; //    Zoom time
        MyCameraZoomOperationType m_zoomType = MyCameraZoomOperationType.NoZoom;
        float m_FOV; // Current fov
        float m_FOVForNearObjects;
        float m_zoomLevel; //    Fov ratio
        //float m_slowDownFactor; //  Slow down factor when zoom is ending
        MySoundCue? m_zoomRelCue; //  Zoom release sound
        MySoundCue? m_zoomALoopCue;    //  Attack and Loop zoom sound
        bool m_zoomReachedEnd;   //  Infomration for sound cues!

        //  Some basic options
        public MyCameraZoomProperties()
        {
            m_zoomTime = 90.0f;
            m_zoomLevel = 1.0f;
            //m_slowDownFactor = 0.2f;
            m_zoomReachedEnd = false;
            Update();
        }

        //  Update!
        public void Update()
        {
            if (m_zoomType != MyCameraZoomOperationType.NoZoom)
            {
                float tmp = (1 / m_zoomTime);
                float cur_time = MathHelper.Lerp(0, m_zoomTime, m_zoomLevel);
                float mod_time = Math.Abs(cur_time - 45);
                tmp = (1 / (m_zoomTime + mod_time)) * (int)m_zoomType;

                m_zoomLevel += tmp;

                ResumeCue(m_zoomALoopCue);
                ResumeCue(m_zoomRelCue);                

                if (m_zoomLevel > 1)
                {
                    m_zoomLevel = 1;
                    m_zoomType = MyCameraZoomOperationType.NoZoom;
                    StopZoomingALoopSound();
                    m_zoomReachedEnd = true;

                }
                else if (m_zoomLevel < 0)
                {
                    m_zoomLevel = 0;
                    m_zoomType = MyCameraZoomOperationType.NoZoom;
                    StopZoomingALoopSound();
                    m_zoomReachedEnd = true;
                }
                else
                    m_zoomReachedEnd = false;
            }

            m_FOV = MathHelper.Lerp(MyConstants.FIELD_OF_VIEW_MIN, MyCamera.FieldOfView, m_zoomLevel);
            m_FOVForNearObjects = MathHelper.Lerp(MyConstants.FIELD_OF_VIEW_MIN, MyCamera.FieldOfViewForNearObjects, m_zoomLevel);
        }

        //reset zoom
        public void ResetZoom()
        {
            StopZoomingALoopSound();
            m_zoomReachedEnd = true;
            m_zoomLevel = 1;
        }

        //  Set zoom : 1 - zoom in, 0 - no zoom, -1 - zoom out
        public void SetZoom(MyCameraZoomOperationType inZoomType)
        {
            // we don't want call zoom, if we can't zoom
            if (inZoomType == MyCameraZoomOperationType.ZoomOut && GetZoomLevel() >= 1.0f ||
               inZoomType == MyCameraZoomOperationType.ZoomIn && GetZoomLevel() <= 0.0f)
            {
                return;
            }
            m_zoomType = inZoomType;
            PlaySound(inZoomType);
        }

        //  Return zoom level. 0 = 100% zoom in, 1 = no zoom;
        public float GetZoomLevel()
        {
            return m_zoomLevel;
        }

        public float GetFOV()
        {
            return m_FOV;
        }

        public float GetFOVForNearObjects()
        {
            return m_FOVForNearObjects;
        }

        public bool IsZooming()
        {
            return m_zoomType != MyCameraZoomOperationType.NoZoom;
        }

        public void PauseZoomCue()
        {
            PauseCue(m_zoomALoopCue);
            PauseCue(m_zoomRelCue);            
        }

        void PauseCue(MySoundCue? cue)
        {
            if (cue != null && cue.Value.IsPlaying)
            {
                cue.Value.Pause();
            }
        }

        void ResumeCue(MySoundCue? cue)
        {
            if (cue != null && cue.Value.IsValid && cue.Value.IsPaused)
            {
                cue.Value.Resume();
            }
        }

        //  Will stop zooming loop sound and play zooming rel sound
        void StopZoomingALoopSound()
        {
            if ((m_zoomALoopCue != null) && (m_zoomALoopCue.Value.IsPlaying == true))
            {
                m_zoomALoopCue.Value.Stop(SharpDX.XACT3.StopFlags.Immediate);
                PlayZoomingRelSound();
            }
        }

        void PlayZoomingALoopSound()
        {
            if ((m_zoomALoopCue == null) || (m_zoomALoopCue.Value.IsPlaying == false))
            {
                m_zoomALoopCue = MyAudio.AddCue2D(MySoundCuesEnum.WepSniperScopeZoomALoop);
            }
        }

        void PlayZoomingRelSound()
        {
            if ((m_zoomRelCue == null) || (m_zoomRelCue.Value.IsPlaying == false))
            {
                m_zoomRelCue = MyAudio.AddCue2D(MySoundCuesEnum.WepSniperScopeZoomRel);
            }
        }

        //  Plays correct sound
        void PlaySound(MyCameraZoomOperationType inZoomType)
        {
            switch (inZoomType)
            {
                case MyCameraZoomOperationType.ZoomIn:
                    //if (!m_zoomReachedEnd)
                    //{
                        PlayZoomingALoopSound();
                    //}
                    break;
                case MyCameraZoomOperationType.ZoomOut:
                    //if (!m_zoomReachedEnd)
                    //{
                        PlayZoomingALoopSound();
                    //}
                    break;
                case MyCameraZoomOperationType.NoZoom:
                    StopZoomingALoopSound();
                    break;
            }
        }
    }
}

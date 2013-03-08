using System;
using System.Threading;
using System.Runtime.InteropServices;
//using MinerWarsMath;
//using MinerWarsMath.Graphics;
using DShowNET;

using SharpDX;
using SharpDX.Direct3D9;

namespace DShowNET
{
    /// <summary>
    /// Describes the state of a video player
    /// </summary>
    public enum VideoState
    {
        Playing,
        Paused,
        Stopped
    }

    /// <summary>
    /// Enables Video Playback in Microsoft XNA
    /// </summary>
    public class VideoPlayer : ISampleGrabberCB, IDisposable
    {
        #region Media Type GUIDs
        private Guid MEDIATYPE_Video = new Guid(0x73646976, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);
        private Guid MEDIASUBTYPE_RGB24 = new Guid(0xe436eb7d, 0x524f, 0x11ce, 0x9f, 0x53, 0x00, 0x20, 0xaf, 0x0b, 0xa7, 0x70);
        private Guid FORMAT_VideoInfo = new Guid(0x05589f80, 0xc356, 0x11ce, 0xbf, 0x01, 0x00, 0xaa, 0x00, 0x55, 0x59, 0x5a);
        #endregion

        #region Private Fields

        private object m_comObject = null;
        /// <summary>
        /// The GraphBuilder interface ref
        /// </summary>
        private IGraphBuilder m_graphBuilder = null;

        /// <summary>
        /// The MediaControl interface ref
        /// </summary>
        private IMediaControl m_mediaControl = null;

        /// <summary>
        /// The MediaEvent interface ref
        /// </summary>
        private IMediaEventEx m_mediaEvent = null;

        /// <summary>
        /// The MediaPosition interface ref
        /// </summary>
        private IMediaPosition m_mediaPosition = null;

        private IBasicAudio m_basicAudio = null;

        /// <summary>
        /// The MediaSeeking interface ref
        /// </summary>
        private IMediaSeeking m_mediaSeeking = null;

        /// <summary>
        /// Thread used to update the video's Texture2D data
        /// </summary>
        private Thread updateThread;

        /// <summary>
        /// Thread used to wait untill the video is complete, then invoke the OnVideoComplete EventHandler
        /// </summary>
        private Thread waitThread;

        /// <summary>
        /// The Video File to play
        /// </summary>
        private string filename;

        /// <summary>
        /// Is a new frame avaliable to update?
        /// </summary>
        private bool frameAvailable = false;

        /// <summary>
        /// Array to hold the raw data from the DirectShow video stream.
        /// </summary>
        private byte[] bgrData;

        /// <summary>
        /// The RGBA frame bytes used to set the data in the Texture2D Output Frame
        /// </summary>
        private byte[] videoFrameBytes;

        /// <summary>
        /// Private Video Width
        /// </summary>
        private int videoWidth = 0;

        /// <summary>
        /// Private Video Height
        /// </summary>
        private int videoHeight = 0;

        /// <summary>
        /// Private Texture2D to render video to. Created in the Video Player Constructor.
        /// </summary> 
        private Texture outputFrame;

        /// <summary>
        /// Average Time per Frame in milliseconds
        /// </summary>
        private long avgTimePerFrame;

        /// <summary>
        /// BitRate of the currently loaded video
        /// </summary>
        private int bitRate;

        /// <summary>
        /// Current state of the video player
        /// </summary>
        private VideoState currentState;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        /// Current time position
        /// </summary>
        private long currentPosition;

        /// <summary>
        /// Video duration
        /// </summary>
        private long videoDuration;

        /// <summary>
        /// How transparent the video frame is.
        /// Takes effect on the next frame after this is updated
        /// Max Value: 255 - Opaque
        /// Min Value: 0   - Transparent
        /// </summary>
        private byte alphaTransparency = 255;
        #endregion

        #region Public Properties
        /// <summary>
        /// Automatically updated video frame. Render this to the screen using a SpriteBatch.
        /// </summary>
        public Texture OutputFrame
        {
            get
            {
                return outputFrame;
            }
        }

        /// <summary>
        /// Width of the loaded video
        /// </summary>
        public int VideoWidth
        {
            get
            {
                return videoWidth;
            }
        }

        /// <summary>
        /// Height of the loaded video
        /// </summary>
        public int VideoHeight
        {
            get
            {
                return videoHeight;
            }
        }

        /// <summary>
        /// Gets or Sets the current position of playback in seconds
        /// </summary>
        public double CurrentPosition
        {
            get
            {
                return (double)currentPosition / 10000000;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (value > Duration)
                    value = Duration;

                m_mediaPosition.put_CurrentPosition(value);
                currentPosition = (long)value * 10000000;
            }
        }

        /// <summary>
        /// Returns the current position of playback, formatted as a time string (HH:MM:SS)
        /// </summary>
        public string CurrentPositionAsTimeString
        {
            get
            {
                double seconds = (double)currentPosition / 10000000;

                double minutes = seconds / 60;
                double hours = minutes / 60;

                int realHours = (int)Math.Floor(hours);
                minutes -= realHours * 60;

                int realMinutes = (int)Math.Floor(minutes);
                seconds -= realMinutes * 60;

                int realSeconds = (int)Math.Floor(seconds);

                return (realHours < 10 ? "0" + realHours.ToString() : realHours.ToString()) + ":" + (realMinutes < 10 ? "0" + realMinutes.ToString() : realMinutes.ToString()) + ":" + (realSeconds < 10 ? "0" + realSeconds.ToString() : realSeconds.ToString());
            }
        }

        /// <summary>
        /// Total duration in seconds
        /// </summary>
        public double Duration
        {
            get
            {
                return (double)videoDuration / 10000000;
            }
        }

        /// <summary>
        /// Returns the duration of the video, formatted as a time string (HH:MM:SS)
        /// </summary>
        public string DurationAsTimeString
        {
            get
            {
                double seconds = (double)videoDuration / 10000000;

                double minutes = seconds / 60;
                double hours = minutes / 60;

                int realHours = (int)Math.Floor(hours);
                minutes -= realHours * 60;

                int realMinutes = (int)Math.Floor(minutes);
                seconds -= realMinutes * 60;

                int realSeconds = (int)Math.Floor(seconds);

                return (realHours < 10 ? "0" + realHours.ToString() : realHours.ToString()) + ":" + (realMinutes < 10 ? "0" + realMinutes.ToString() : realMinutes.ToString()) + ":" + (realSeconds < 10 ? "0" + realSeconds.ToString() : realSeconds.ToString());
            }
        }

        /// <summary>
        /// Currently Loaded Video File
        /// </summary>
        public string FileName
        {
            get
            {
                return filename;
            }
        }

        /// <summary>
        /// Gets or Sets the current state of the video player
        /// </summary>
        public VideoState CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                switch (value)
                {
                    case VideoState.Playing:
                        Play();
                        break;
                    case VideoState.Paused:
                        Pause();
                        break;
                    case VideoState.Stopped:
                        Stop();
                        break;
                }
            }
        }

        /// <summary>
        /// Event which occurs when the video stops playing once it has reached the end of the file
        /// </summary>
        public event EventHandler OnVideoComplete;

        /// <summary>
        /// Is Disposed?
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        /// <summary>
        /// Number of Frames Per Second in the video file.
        /// Returns -1 if this cannot be calculated.
        /// </summary>
        public int FramesPerSecond
        {
            get
            {
                if (avgTimePerFrame == 0)
                    return -1;

                float frameTime = (float)avgTimePerFrame / 10000000.0f;
                float framesPS = 1.0f / frameTime;
                return (int)Math.Round(framesPS, 0, MidpointRounding.ToEven);
            }
        }

        /// <summary>
        /// The number of milliseconds between each frame
        /// Returns -1 if this cannot be calculated
        /// </summary>
        public float MillisecondsPerFrame
        {
            get
            {
                if (avgTimePerFrame == 0)
                    return -1;

                return (float)avgTimePerFrame / 10000.0f;
            }
        }

        /// <summary>
        /// Gets or sets how transparent the video frame is.
        /// Takes effect on the next frame after this is updated
        /// Max Value: 255 - Opaque
        /// Min Value: 0   - Transparent
        /// </summary>
        public byte AlphaTransparency
        {
            get
            {
                return alphaTransparency;
            }
            set
            {
                alphaTransparency = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new Video Player. Automatically creates the required Texture2D on the specificied GraphicsDevice.
        /// </summary>
        /// <param name="FileName">The video file to open</param>
        /// <param name="graphicsDevice">XNA Graphics Device</param>
        public VideoPlayer(string FileName, SharpDX.Direct3D9.Device graphicsDevice)
        {
            try
            {
                // Set video state
                currentState = VideoState.Stopped;

                // Store Filename
                filename = FileName;

                // Open DirectShow Interfaces
                InitInterfaces();

                // Create a SampleGrabber Filter and add it to the FilterGraph
                //SampleGrabber sg = new SampleGrabber();
                var comtype = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                if (comtype == null)
                    throw new NotSupportedException("DirectX (8.1 or higher) not installed?");
                m_comObject = Activator.CreateInstance(comtype);

                ISampleGrabber sampleGrabber = (ISampleGrabber)m_comObject;
                m_graphBuilder.AddFilter((IBaseFilter)m_comObject, "Grabber");

                // Setup Media type info for the SampleGrabber
                AMMediaType mt = new AMMediaType();
                mt.majorType = MEDIATYPE_Video;     // Video
                mt.subType = MEDIASUBTYPE_RGB24;    // RGB24
                mt.formatType = FORMAT_VideoInfo;   // VideoInfo
                sampleGrabber.SetMediaType(mt);

                // Construct the rest of the FilterGraph
                m_graphBuilder.RenderFile(filename, null);

                // Set SampleGrabber Properties
                sampleGrabber.SetBufferSamples(true);
                sampleGrabber.SetOneShot(false);
                sampleGrabber.SetCallback((ISampleGrabberCB)this, 1);

                // Hide Default Video Window
                IVideoWindow pVideoWindow = (IVideoWindow)m_graphBuilder;
                //pVideoWindow.put_AutoShow(OABool.False);
                pVideoWindow.put_AutoShow(0);

                // Create AMMediaType to capture video information
                AMMediaType MediaType = new AMMediaType();
                sampleGrabber.GetConnectedMediaType(MediaType);
                VideoInfoHeader pVideoHeader = new VideoInfoHeader();
                Marshal.PtrToStructure(MediaType.formatPtr, pVideoHeader);

                // Store video information
                videoHeight = pVideoHeader.BmiHeader.Height;
                videoWidth = pVideoHeader.BmiHeader.Width;
                avgTimePerFrame = pVideoHeader.AvgTimePerFrame;
                bitRate = pVideoHeader.BitRate;
                m_mediaSeeking.GetDuration(out videoDuration);

                // Create byte arrays to hold video data
                videoFrameBytes = new byte[(videoHeight * videoWidth) * 4]; // RGBA format (4 bytes per pixel)
                bgrData = new byte[(videoHeight * videoWidth) * 3];         // BGR24 format (3 bytes per pixel)

                // Create Output Frame Texture2D with the height and width of the video
                outputFrame = new Texture(graphicsDevice, videoWidth, videoHeight, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
            }
            catch
            {
                throw new Exception("Unable to Load or Play the video file");
            }
        }
        #endregion

        #region DirectShow Interface Management
        /// <summary>
        /// Initialises DirectShow interfaces
        /// </summary>
        private void InitInterfaces()
        {
            var comtype = Type.GetTypeFromCLSID(Clsid.FilterGraph);
            if (comtype == null)
                throw new NotSupportedException("DirectX (8.1 or higher) not installed?");
            var fg = Activator.CreateInstance(comtype);

            m_graphBuilder = (IGraphBuilder)fg;
            m_mediaControl = (IMediaControl)fg;
            m_mediaEvent = (IMediaEventEx)fg;
            m_mediaSeeking = (IMediaSeeking)fg;
            m_mediaPosition = (IMediaPosition)fg;
            m_basicAudio = (IBasicAudio)fg;

            fg = null;
        }

        /// <summary>
        /// Closes DirectShow interfaces
        /// </summary>
        private void CloseInterfaces()
        {
            if (m_mediaEvent != null)
            {
                m_mediaControl.Stop();
                //0x00008001 = WM_GRAPHNOTIFY
                m_mediaEvent.SetNotifyWindow(IntPtr.Zero, 0x00008001, IntPtr.Zero);
            }
            m_mediaControl = null;
            m_mediaEvent = null;
            m_graphBuilder = null;
            m_mediaSeeking = null;
            m_mediaPosition = null;
            m_basicAudio = null;
            
            if (m_comObject != null)
                Marshal.ReleaseComObject(m_comObject);
            m_comObject = null;
        }
        #endregion

        #region Update and Media Control
        /// <summary>
        /// Updates the Output Frame data using data from the video stream. Call this in Game.Update().
        /// </summary>
        public void Update()
        {
            // Set video data into the Output Frame
            DataStream ds;        
            DataRectangle dr = outputFrame.LockRectangle(0, LockFlags.Discard, out ds);

            int bytesPerRow = videoWidth * 4;
            int skipBytesInRow = dr.Pitch - bytesPerRow;
            int offset = 0;
            
            for (int j = 0; j < videoHeight; j++)
            {
                ds.WriteRange(videoFrameBytes, offset, bytesPerRow);
                ds.Seek(skipBytesInRow, System.IO.SeekOrigin.Current);
                offset += bytesPerRow;
            }     

            //ds.WriteRange(videoFrameBytes);
                   /*
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < dr.Pitch; i++)
                {
                    if (i < w * 4)
                        ds.WriteByte(videoFrameBytes[j * w * 4 + i]);
                    else
                        ds.WriteByte(255);
                }
            }     */

            outputFrame.UnlockRectangle(0);

            //BaseTexture.ToFile(outputFrame, "c:\\video3.png", ImageFileFormat.Png);
                      
            /*
            System.IO.FileStream fs = System.IO.File.Create("c:\\video2.rgb");
            fs.Write(videoFrameBytes, 0, videoFrameBytes.Length);
            fs.Dispose();
              */

            // Update current position read-out
            m_mediaSeeking.GetCurrentPosition(out currentPosition);
        }

        /// <summary>
        /// Starts playing the video
        /// </summary>
        public void Play()
        {
            if (currentState != VideoState.Playing)
            {
                // Create video threads
                updateThread = new Thread(new ThreadStart(UpdateBuffer));
                updateThread.IsBackground = true;
                waitThread = new Thread(new ThreadStart(WaitForCompletion));
                waitThread.IsBackground = true;

                // Start the FilterGraph
                m_mediaControl.Run();

                // Start Threads
                updateThread.Start();
                waitThread.Start();

                // Update VideoState
                currentState = VideoState.Playing;
            }
        }

        /// <summary>
        /// Pauses the video
        /// </summary>
        public void Pause()
        {
            // End threads
            if (updateThread != null)
                updateThread.Abort();
            updateThread = null;

            if (waitThread != null)
                waitThread.Abort();
            waitThread = null;

            // Stop the FilterGraph (but remembers the current position)
            m_mediaControl.Stop();

            // Update VideoState
            currentState = VideoState.Paused;
        }

        /// <summary>
        /// Stops playing the video
        /// </summary>
        public void Stop()
        {
            // End Threads
            if (updateThread != null)
                updateThread.Abort();
            updateThread = null;

            if (waitThread != null)
                waitThread.Abort();
            waitThread = null;

            // Stop the FilterGraph
            m_mediaControl.Stop();

            // Reset the current position
            m_mediaSeeking.SetPositions(new DsOptInt64(0), SeekingFlags.AbsolutePositioning, new DsOptInt64(0), SeekingFlags.NoPositioning);

            // Update VideoState
            currentState = VideoState.Stopped;
        }

        /// <summary>
        /// Rewinds the video to the start and plays it again
        /// </summary>
        public void Rewind()
        {
            Stop();
            Play();
        }
        #endregion

        #region ISampleGrabberCB Members and Helpers
        /// <summary>
        /// Required public callback from DirectShow SampleGrabber. Do not call this method.
        /// </summary>
        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            // Copy raw data into bgrData byte array
            Marshal.Copy(pBuffer, bgrData, 0, BufferLen);

            // Flag the new frame as available
            frameAvailable = true;

            // Return S_OK
            return 0;
        }

        /// <summary>
        /// Required public callback from DirectShow SampleGrabber. Do not call this method.
        /// </summary>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            // Return S_OK
            return 0;
        }

        /// <summary>
        /// Worker to copy the BGR data from the video stream into the RGBA byte array for the Output Frame.
        /// </summary>
        private void UpdateBuffer()
        {
            int waitTime = avgTimePerFrame != 0 ? (int)((float)avgTimePerFrame / 10000) : 20;

            int samplePosRGBA = 0;
            int samplePosRGB24 = 0;

            while (true)
            {
                for (int y = 0, y2 = videoHeight - 1; y < videoHeight; y++, y2--)
                {
                    for (int x = 0; x < videoWidth; x++)
                    {
                        samplePosRGBA = ((y2 * videoWidth) + x) * 4;
                        samplePosRGB24 = ((y * videoWidth) + x) * 3;

                        videoFrameBytes[samplePosRGBA + 0] = bgrData[samplePosRGB24 + 0];
                        videoFrameBytes[samplePosRGBA + 1] = bgrData[samplePosRGB24 + 1];
                        videoFrameBytes[samplePosRGBA + 2] = bgrData[samplePosRGB24 + 2];
                        videoFrameBytes[samplePosRGBA + 3] = alphaTransparency;
                    }
                }      

                frameAvailable = false;
                while (!frameAvailable)
                { Thread.Sleep(waitTime); }
            }
        }

        /// <summary>
        /// Waits for the video to finish, then calls the OnVideoComplete event
        /// </summary>
        private void WaitForCompletion()
        {
            int waitTime = avgTimePerFrame != 0 ? (int)((float)avgTimePerFrame / 10000) : 20;

            try
            {
                while (videoDuration > currentPosition)
                {
                    Thread.Sleep(waitTime);
                }

                if (OnVideoComplete != null)
                    OnVideoComplete.Invoke(this, EventArgs.Empty);

                currentState = VideoState.Stopped;
            }
            catch { }
        }

        //convert from 0..1f to -10000..0i
        public float Volume
        {
            get
            {
                int volume;
                m_basicAudio.get_Volume(out volume);
                return (volume / 10000.0f) + 1.0f;
            }
            set
            {
                m_basicAudio.put_Volume((int)((value - 1.0f) * 10000.0f));
            }
        }


        #endregion

        #region IDisposable Members
        /// <summary>
        /// Cleans up the Video Player. Must be called when finished with the player.
        /// </summary>
        public void Dispose()
        {
            isDisposed = true;

            Stop();
            CloseInterfaces();

            outputFrame.Dispose();
            outputFrame = null;

        }
        #endregion
    }
}
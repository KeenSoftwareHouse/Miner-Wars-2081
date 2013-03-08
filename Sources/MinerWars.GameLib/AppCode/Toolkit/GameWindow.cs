using System;

using SharpDX.Toolkit.Graphics;
using SharpDX.Windows;
using System.Windows.Forms;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// An abstract window.
    /// </summary>
    public abstract class GameWindow : Component
    {
        #region Fields

        private string title;

        #endregion

        #region Public Events

        /// <summary>
        /// Occurs when this window is activated.
        /// </summary>
        public event EventHandler<EventArgs> Activated;

        /// <summary>
        /// Occurs, when device client size is changed.
        /// </summary>
        public event EventHandler<EventArgs> ClientSizeChanged;

        /// <summary>
        /// Occurs when this window is deactivated.
        /// </summary>
        public event EventHandler<EventArgs> Deactivated;

        /// <summary>
        /// Occurs, when device orientation is changed.
        /// </summary>
        public event EventHandler<EventArgs> OrientationChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets, user possibility to resize this window.
        /// </summary>
        public abstract bool AllowUserResizing { get; set; }

        /// <summary>
        /// Gets the client bounds.
        /// </summary>
        /// <value>The client bounds.</value>
        public abstract DrawingRectangle ClientBounds { get; }

     
        /// <summary>
        /// Gets a value indicating whether this window is only suitable to run in full-screen mode.
        /// </summary>
        /// <value><c>true</c> if this window is only suitable to run in full-screen mode is full screen mandatory; otherwise, <c>false</c>.</value>
        public abstract bool IsFullScreenMandatory { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is minimized.
        /// </summary>
        /// <value><c>true</c> if this instance is minimized; otherwise, <c>false</c>.</value>
        public abstract bool IsMinimized { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse pointer is visible over this window.
        /// </summary>
        /// <value><c>true</c> if this instance is mouse visible; otherwise, <c>false</c>.</value>
        public abstract bool IsMouseVisible { get; set; }

        /// <summary>
        /// Gets the native window.
        /// </summary>
        /// <value>The native window.</value>
        public abstract Control NativeWindow { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GameWindow" /> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public abstract bool Visible { get; set; }

        public abstract void UpdateFullscreen(bool fullscreen);

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (title != value)
                {
                    title = value;
                    SetTitle(title);
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        public abstract void BeginScreenDeviceChange(bool willBeFullScreen);

        public void EndScreenDeviceChange()
        {
            EndScreenDeviceChange(ClientBounds.Width, ClientBounds.Height);
        }

        public abstract void EndScreenDeviceChange(int clientWidth, int clientHeight);

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the GameWindow with the specified window context.
        /// </summary>
        /// <param name="windowContext">The window context.</param>
        internal abstract void Initialize(object windowContext);

        protected void OnActivated(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Activated;
            if (handler != null)
            {
                handler(source, e);
            }
        }

        protected void OnClientSizeChanged(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = ClientSizeChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnDeactivated(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = Deactivated;
            if (handler != null)
            {
                handler(source, e);
            }
        }

        protected void OnOrientationChanged(object source, EventArgs e)
        {
            EventHandler<EventArgs> handler = OrientationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected abstract void SetTitle(string title);

        #endregion
    }
}
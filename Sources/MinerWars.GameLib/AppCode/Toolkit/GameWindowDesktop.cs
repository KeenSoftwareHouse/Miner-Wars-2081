using System;
using System.Windows.Forms;

using SharpDX.Toolkit.Graphics;
using SharpDX.Windows;

namespace SharpDX.Toolkit
{
    /// <summary>
    /// An abstract window.
    /// </summary>
    internal class GameWindowDesktop : GameWindow
    {
        private bool isInitialized = false;
        private bool isMouseVisible = true;
        private bool isMouseCurrentlyHidden;

        public Control Control;

        private GameWindowForm gameWindowForm;


        internal GameWindowDesktop()
        {
        }

        public bool IsForm
        {
            get
            {
                return Control is Form;
            }
        }

        public override Control NativeWindow
        {
            get
            {
                return Control;
            }
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            
        }

        public override void EndScreenDeviceChange(int clientWidth, int clientHeight)
        {
            
        }

        public override bool IsFullScreenMandatory
        {
            get
            {
                return false;
            }
        }


        internal override void Initialize(object windowContext)
        {
            if (isInitialized)
            {
                throw new InvalidOperationException("GameWindow is already initialized");
            }

            windowContext = windowContext ?? new GameWindowForm("SharpDX.Toolkit.Game");
            Control = windowContext as Control;
            if (Control == null)
            {
                throw new NotSupportedException("Unsupported window context. Unable to create game window. Only System.Windows.Control subclass are supported");
            }

            //Control.Focus();

            //Control.MouseEnter += GameWindowForm_MouseEnter;
            //Control.MouseLeave += GameWindowForm_MouseLeave;

            gameWindowForm = windowContext as GameWindowForm;
            if (gameWindowForm != null)
            {
                gameWindowForm.Activated += OnActivated;
                gameWindowForm.Deactivate += OnDeactivated;
                //gameWindowForm.UserResized += OnClientSizeChanged;
            }
            else
            {
                //Control.Resize += OnClientSizeChanged;
            }

            isInitialized = true;
        }

        void GameWindowForm_MouseEnter(object sender, System.EventArgs e)
        {
            if (!isMouseVisible && !isMouseCurrentlyHidden)
            {
                Cursor.Hide();
                isMouseCurrentlyHidden = true;
            }
        }

        void GameWindowForm_MouseLeave(object sender, System.EventArgs e)
        {
            if (isMouseCurrentlyHidden)
            {
                Cursor.Show();
                isMouseCurrentlyHidden = false;
            }
        }

        public override bool IsMouseVisible
        {
            get
            {
                return isMouseVisible;
            }
            set
            {
                if (isMouseVisible != value)
                {
                    isMouseVisible = value;
                    if (isMouseVisible)
                    {
                        if (isMouseCurrentlyHidden)
                        {
                            Cursor.Show();
                            isMouseCurrentlyHidden = false;
                        }
                    }
                    else if (!isMouseCurrentlyHidden)
                    {
                        Cursor.Hide();
                        isMouseCurrentlyHidden = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GameWindow" /> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public override bool Visible
        {
            get
            {
                return Control.Visible;
            }
            set
            {
                Control.Visible = value;
            }
        }

        protected override void SetTitle(string title)
        {
            var form = Control as Form;
            if (form != null)
            {
                form.Text = title;
            }
        }

        public override bool AllowUserResizing
        {
            get
            {
                return (Control is GameWindowForm && ((GameWindowForm)Control).AllowUserResizing);
            }
            set
            {
                if (Control is GameWindowForm)
                {
                    ((GameWindowForm)Control).AllowUserResizing = value;
                }
            }
        }

        public override DrawingRectangle ClientBounds
        {
            get
            {
                return new DrawingRectangle(0, 0, Control.ClientSize.Width, Control.ClientSize.Height);
            }
        }

        public override bool IsMinimized
        {
            get
            {
                var form  = Control as Form;
                if (form != null)
                {
                    return form.WindowState == FormWindowState.Minimized;
                }

                // Check for non-form control
                return false;
            }
        }

        public override void UpdateFullscreen(bool fullscreen)
        {
            var form  = Control as Form;
            if (fullscreen)
                form.FormBorderStyle = FormBorderStyle.None;
            else
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}

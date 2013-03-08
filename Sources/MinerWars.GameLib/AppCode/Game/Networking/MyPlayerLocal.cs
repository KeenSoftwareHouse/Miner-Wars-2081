using System.Text;

namespace MinerWars.AppCode.Game.World
{
    class MyPlayerLocal : MyPlayerBase
    {
        readonly bool m_canAccessDemo;
        readonly bool m_canSave;
        readonly bool m_canAccessEditorForStory;
        readonly bool m_canAccessEditorForMMO;
        readonly bool m_canUseCheats;
        readonly bool m_canAccessMMO;
        readonly bool m_canAccess25d;
        bool m_hasAnyCheckpoints; // Changed when player saves the game

        // Remember to use with WCF for subsequent logins
        readonly StringBuilder m_userName;
        readonly StringBuilder m_passwordHash;

        public bool PlayingMMO;

        public const int OFFLINE_MODE_USERID = 1;

        public Networking.SectorService.MyAdditionalUserInfo AdditionalInfo { get; set; }

        public MyPlayerLocal(StringBuilder displayName, int userId, bool canAccessDemo, bool canSave, bool canAccessEditorForStory, bool canAccessEditorForMMO, bool hasAnyCheckpoints, bool canUseCheats, bool canAccessMMO, StringBuilder userName, StringBuilder passwordHash, bool canAccess25d)
            : base(displayName, userId)
        {
            m_canAccessDemo = canAccessDemo;
            m_canSave = canSave;
            m_canAccessEditorForStory = canAccessEditorForStory;
            m_canAccessEditorForMMO = canAccessEditorForMMO;
            m_hasAnyCheckpoints = hasAnyCheckpoints;
            m_canUseCheats = canUseCheats;
            m_canAccessMMO = canAccessMMO;
            m_canAccess25d = canAccess25d;

            m_userName = userName;
            m_passwordHash = passwordHash;
        }

        public static MyPlayerLocal CreateOfflineDemoUser()
        {
            return new MyPlayerLocal(new StringBuilder("DEMO"), OFFLINE_MODE_USERID, true, false, false, false, false, false, false, new StringBuilder("DEMO"), new StringBuilder(), true)
                {
                    AdditionalInfo = new Networking.SectorService.MyAdditionalUserInfo()
                };
        }

        public bool IsDemoUser()
        {
            return m_canAccessDemo && !m_canSave;
        }

        public bool GetCanAccessDemo()
        {
            return m_canAccessDemo;
        }

        public bool GetCanSave()
        {
            return m_canSave;
        }

        public bool GetCanAccessEditorForStory()
        {
            return m_canAccessEditorForStory;
        }

        public bool GetCanAccessEditorForMMO()
        {
            return m_canAccessEditorForMMO;
        }

        public bool GetCanAccessMMO()
        {
            return m_canAccessMMO;
        }

        public bool GetCanAccess25d()
        {
            return m_canAccess25d;
        }

        public bool GetUseCheats()
        {
            return m_canUseCheats;
        }

        public bool HasAnyCheckpoints
        {
            get { return m_hasAnyCheckpoints; }
            set { m_hasAnyCheckpoints = value; }
        }

        public StringBuilder UserName
        {
            get
            {
                return m_userName;
            }
        }

        public StringBuilder PasswordHash
        {
            get
            {
                return m_passwordHash;
            }
        }

        public bool LoggedUsingSteam { get; set; }
    }
}

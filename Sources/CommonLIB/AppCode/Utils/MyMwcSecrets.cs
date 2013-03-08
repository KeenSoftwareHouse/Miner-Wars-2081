using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Utils
{
    /// <summary>
    /// Secret constants, should not be published when releasing source codes
    /// </summary>
    public static class MyMwcSecrets
    {
        public static readonly string MOD_NAME = "UNNAMED MOD";
        public static readonly string GAME_HASH = "";

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Final build constants
        //////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly string MASTER_SERVER_ADDRESS_DEVELOP = "";
        public static readonly string UPDATER_URL_DEVELOP = "";

        //  For upload to S3
        public static readonly string UPDATE_BUCKET_NAME_DEVELOP = "";

        //  For upload to S3 Download mirror
        public static readonly string DOWNLOAD_BUCKET_NAME_DEVELOP = "";

        public static readonly string ANNOUNCE_URL_DEVELOP = "";
        public static readonly int TRACKER_PORT_DEVELOP = 0;

        public static readonly string UPDATER_CDN_MIRROR_URL_DEVELOP = "";

        //  Url of MinerWarsSetup.torrent file
        public static readonly string SETUPTORRENT_URL_DEVELOP = "";
        public static readonly string SETUPTORRENTMIRROR_URL_DEVELOP = "";
        public static readonly string SETUPTORRENT_BUCKET_NAME_DEVELOP = "";

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // Networking constants
        //////////////////////////////////////////////////////////////////////////////////////////////////

        public static readonly string WCF_SECTOR_SERVER_HASH = "";
        public static readonly string WCF_MASTER_SERVER_HASH = "";

        // Editor and "Multiplayer"/"Host"/"My sectors" won't work, modders must change it to save that locally
        public static readonly string WCF_MS_PUBLIC_USERNAME = "";
        public static readonly string WCF_MS_PUBLIC_PASSWORD = "";

        public static readonly int NETWORKING_PORT_MASTER_SERVER_NEW_MEX = 0;
        public static readonly int NETWORKING_PORT_MASTER_SERVER_USER_VALIDATION_MEX = 0;
    }
}

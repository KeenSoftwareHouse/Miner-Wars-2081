using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using System.IO;
using SysUtils.Utils;
using SysUtils;
using MinerWars.CommonLIB.AppCode.Networking;
using System.Diagnostics;
using KeenSoftwareHouse.Library.Trace;
using MinerWars.AppCode.Game.Missions;
using KeenSoftwareHouse.Library.Security;
using System.Security.Cryptography;
using KeenSoftwareHouse.Library.Extensions;
using MinerWars.AppCode.Game.World;
using MinerWars.AppCode.App;

namespace MinerWars.AppCode.Networking
{
    class MyLocalCache
    {
        public static bool IsSupported(MyMwcSectorTypeEnum sectorType, int? userId)
        {
            return sectorType == CommonLIB.AppCode.Networking.MyMwcSectorTypeEnum.STORY && userId != null;
        }

        public static bool CanBeSaved(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            // Story checkpoint (STORY && UserId != null)
            return IsSupported(checkpoint.CurrentSector.SectorType, checkpoint.CurrentSector.UserId);
        }

        public static string CachePath { get { return Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Cache", MyMwcFinalBuildConstants.TYPE_AS_STRING); } }
        public static string SavesPath
        {
            get
            {
                string basePath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Saves");

                if (MySteam.IsActive)
                {
                    return Path.Combine(basePath, "STEAM", MySteam.UserId.ToString());
                }
                else
                {
                    return Path.Combine(basePath, MyMwcFinalBuildConstants.TYPE_AS_STRING, MyClientServer.LoggedPlayer.UserName.ToString());
                }
            }
        }

        public static string CurrentChapterName = "Current";
        public static string CurrentSavePath { get { return Path.Combine(SavesPath, CurrentChapterName); } }
        public static string ContentSectorPath { get { return Path.Combine(MyMinerGame.Static.RootDirectory, "Sectors"); } }

        public static string GetChapterDirectory(string chapterName)
        {
            return Path.Combine(SavesPath, chapterName);
        }

        static bool IsSave(MyMwcSectorIdentifier sector)
        {
            return sector.UserId != null && sector.SectorType == MyMwcSectorTypeEnum.STORY;
        }

        public static string GetSectorPath(MyMwcSectorIdentifier sector)
        {
            string dir;
            string file;

            if (IsSave(sector))
            {
                dir = CurrentSavePath;
                file = GetSectorName(sector, false) + ".mws"; // Saves does not include UserId
            }
            else
            {
                dir = CachePath;
                file = GetSectorName(sector, true) + ".mws"; // Cache include UserId
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return Path.Combine(dir, file);
        }

        public static string GetSectorNameLong(MyMwcSectorIdentifier sector)
        {
            var name = "";
            if (!string.IsNullOrEmpty(sector.SectorName))
            {
                name = String.Format(" ({0})", sector.SectorName);
            }
            return GetSectorName(sector, true) + name;
        }

        public static string GetSectorName(MyMwcSectorIdentifier sector, bool includeUserId)
        {
            string name;
            switch (sector.SectorType)
            {
                case MyMwcSectorTypeEnum.STORY:
                    name = "STORY";
                    break;

                case MyMwcSectorTypeEnum.SANDBOX:
                    name = "SANDBOX";
                    break;

                default:
                    throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            string userId = sector.UserId.HasValue ? sector.UserId.Value.ToString() : "NULL";

            if (includeUserId)
            {
                return String.Format("{0}_{1}_{2}_{3}_{4}_", name, userId, sector.Position.X, sector.Position.Y, sector.Position.Z);
            }
            else
            {
                return String.Format("{0}_{1}_{2}_{3}_", name, sector.Position.X, sector.Position.Y, sector.Position.Z);
            }
        }

        public static List<MyMwcSectorIdentifier> GetOfficialMultiplayerSectorIdentifiers()
        {
            string[] officialMaps = new string[]
            {
                "SANDBOX_-100_0_20_.mws",
                "SANDBOX_-15_0_-42_.mws",
                "SANDBOX_-30_0_-61_.mws",
                "SANDBOX_-38_0_71_.mws",
                "SANDBOX_-43_0_86_.mws",
                "SANDBOX_-46_0_48_.mws",
                "SANDBOX_-4_0_23_.mws",	
                "SANDBOX_-66_0_-44_.mws",
                "SANDBOX_-78_0_35_.mws",
                "SANDBOX_-79_0_70_.mws",
                "SANDBOX_-8_0_-5_.mws",	
                "SANDBOX_-93_0_46_.mws",
                "SANDBOX_17_0_67_.mws",	
                "SANDBOX_18_0_96_.mws",	
                "SANDBOX_19_0_-92_.mws",
                "SANDBOX_1_0_-58_.mws",	
                "SANDBOX_3_0_9_.mws",	
                "SANDBOX_47_0_-24_.mws",
                "SANDBOX_53_0_84_.mws",	
                "SANDBOX_61_0_-65_.mws",
                "SANDBOX_64_0_-20_.mws",
                "SANDBOX_76_0_29_.mws",	
                "SANDBOX_83_0_46_.mws",	
                "SANDBOX_88_0_-58_.mws",
                "SANDBOX_93_0_14_.mws",	
                "SANDBOX_93_0_73_.mws",	
            };

            List<MyMwcSectorIdentifier> sectors = new List<MyMwcSectorIdentifier>();

            var dir = new DirectoryInfo(ContentSectorPath);
            foreach (var file in dir.GetFiles("SANDBOX*.mws"))
            {
                if (officialMaps.Contains(file.Name))
                {
                    var sector = LoadData<MyMwcObjectBuilder_Sector>(file.FullName);
                    sectors.Add(new MyMwcSectorIdentifier(MyMwcSectorTypeEnum.SANDBOX, null, sector.Position, sector.Name));
                }
            }

            return sectors;
        }

        /// <summary>
        /// Clears cache (new game, etc)
        /// </summary>
        public static void ClearCache()
        {
            if (Directory.Exists(CachePath))
            {
                Directory.Delete(CachePath, true);
                Log("CACHE: Cleared");
            }
        }

        public static void ClearCurrentSave()
        {
            if (Directory.Exists(CurrentSavePath))
            {
                Directory.Delete(CurrentSavePath, true);
                Log("SAVES: Cleared");
            }
        }

        //public static bool SectorExists(MyMwcSectorIdentifier sector)
        //{
        //    return File.Exists(GetSectorPath(sector));
        //}

        public static void SaveCheckpoint(MyMwcObjectBuilder_Checkpoint checkpoint, bool createChapter = false)
        {
            Save(checkpoint, checkpoint.SectorObjectBuilder, checkpoint.CurrentSector, createChapter);
        }

        public static void Save(MyMwcObjectBuilder_Checkpoint checkpoint, MyMwcObjectBuilder_Sector sector, MyMwcSectorIdentifier sectorIdentifier, bool createChapter = false)
        {
            if (sector == null) // Nothing to save, saving only checkpoint
                return;

            string sectorPath = GetSectorPath(sectorIdentifier);
            string storeDirectory = Path.GetDirectoryName(sectorPath);

            // Store current sector
            SaveData(sector, sectorPath);


            // Store checkpoint
            if (checkpoint != null)
            {
                string checkpointPath = Path.Combine(storeDirectory, "Checkpoint.mwc");

                var oldValue = checkpoint.SectorObjectBuilder;
                checkpoint.SectorObjectBuilder = null;

                SaveData(checkpoint, checkpointPath);

                checkpoint.SectorObjectBuilder = oldValue;
            }

            if (createChapter)
            {
                var chapterDirectory = GetChapterDirectory(GetChapterName(checkpoint));
                if (Directory.Exists(chapterDirectory))
                {
                    Directory.Delete(chapterDirectory, true);
                }
                DirectoryExtensions.CopyAll(storeDirectory, chapterDirectory);
            }

            Log("update", sectorIdentifier, sector.Version);
        }


        public static T LoadData<T>(string path)
            where T : MyMwcObjectBuilder_Base
        {
            try
            {
                var hashAlgorithm = new Crc32();
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (CryptoStream cryptoStream = new CryptoStream(stream, hashAlgorithm, CryptoStreamMode.Read))
                using (BinaryReader reader = new BinaryReader(cryptoStream))
                {
                    var version = reader.ReadInt32();
                    //TODO: Enable when we have correct backward compatibility https://mantis.keenswh.com/view.php?id=7584
                    if (version < MyMwcObjectBuilder_Base.FIRST_COMPATIBILITY_VERSION)
                    {
                        MyMwcLog.WriteLine("Loading data from " + path + " failed because of incompatible version, save version: " + version);
                        return null;
                    }

                    byte[] bytes = reader.ReadBytes((int)(stream.Length - stream.Position - hashAlgorithm.HashSize / 8));

                    cryptoStream.FlushFinalBlock();

                    byte[] computedHash = hashAlgorithm.Hash;
                    byte[] storedHash = new byte[hashAlgorithm.HashSize / 8];
                    stream.Read(storedHash, 0, storedHash.Length);
                    if (computedHash.Length != storedHash.Length)
                        return null;

                    for (int i = 0; i < storedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHash[i])
                        {
                            MyMwcLog.WriteLine("Loading data from " + path + " failed because of incorrect hash");
                            return null;
                        }
                    }

                    var result = MyMwcObjectBuilder_Base.FromBytes<T>(bytes, version);
                    if (result == null)
                    {
                        MyMwcLog.WriteLine("Loading data from " + path + " failed because of invalid object builder");
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                MyMwcLog.WriteLine("Exception occured when loading data from local cache");
                MyMwcLog.WriteLine(e);
            }

            return null;
        }

        public static void SaveData(MyMwcObjectBuilder_Base objectBuilder, string path)
        {
            var hashAlgorithm = new Crc32();

            using (FileStream stream = new FileStream(path, FileMode.Create))
            using (CryptoStream cryptoStream = new CryptoStream(stream, hashAlgorithm, CryptoStreamMode.Write))
            using (BinaryWriter writer = new BinaryWriter(cryptoStream))
            {
                writer.Write(MyMwcFinalBuildConstants.SERVER_PROTOCOL_VERSION);
                writer.Write(objectBuilder.ToBytes());

                cryptoStream.FlushFinalBlock();

                var computedHash = hashAlgorithm.Hash;
                for (int i = 0; i < computedHash.Length; i++)
                {
                    writer.Write(computedHash[i]);
                }
            }
        }

        public static string GetChapterName(MyMwcObjectBuilder_Checkpoint checkpoint)
        {
            StringBuilder chapterName = new StringBuilder();
            chapterName.Append(checkpoint.ActiveMissionID.ToString());

            var lastEvent = checkpoint.EventLogObjectBuilder.Where(e => e.EventType == (int)MinerWars.AppCode.Game.Journal.EventTypeEnum.SubmissionFinished).OrderByDescending(e => e.Time).FirstOrDefault();
            if (lastEvent != null)
            {
                chapterName.Append("_");
                chapterName.Append(lastEvent.EventTypeID);
            }
            return chapterName.ToString();
        }

        public static void ReplaceCurrentChapter(string chapterName)
        {
            string chapterDirectory = GetChapterDirectory(chapterName);

            ClearCurrentSave();
            DirectoryExtensions.CopyAll(chapterDirectory, CurrentSavePath);
        }

        public static List<Tuple<string, DateTime, MyMwcObjectBuilder_Checkpoint>> LoadChapters()
        {
            var result = new List<Tuple<string, DateTime, MyMwcObjectBuilder_Checkpoint>>();

            var savesDirectory = new System.IO.DirectoryInfo(SavesPath);
            if (!savesDirectory.Exists)
                return result;

            foreach (var file in savesDirectory.GetFiles("Checkpoint.mwc", SearchOption.AllDirectories))
            {
                if (file.Directory.Name.EndsWith(CurrentChapterName))
                    continue;

                var checkpointBuilder = LoadData<MyMwcObjectBuilder_Checkpoint>(file.FullName);
                if (checkpointBuilder != null)
                {
                    result.Add(Tuple.Create(file.Directory.Name, file.LastWriteTimeUtc, checkpointBuilder));
                }
            }

            return result;
        }

        public static DateTime GetLastChapterTimestamp()
        {
            var max = new DateTime();

            var savesDirectory = new System.IO.DirectoryInfo(SavesPath);
            if (!savesDirectory.Exists)
                return max;

            foreach (var file in savesDirectory.GetFiles("Checkpoint.mwc", SearchOption.AllDirectories))
            {
                if (file.Directory.Name.EndsWith(CurrentChapterName))
                    continue;

                if (file.LastWriteTime > max)
                    max = file.LastWriteTime;
            }

            return max;
        }

        public static MyMwcObjectBuilder_Checkpoint NewGameCheckpoint()
        {
            return LoadCheckpoint(Path.Combine(ContentSectorPath, "story.mwc"));
        }

        public static MyMwcObjectBuilder_Checkpoint MultiplayerCheckpoint()
        {
            return LoadCheckpoint(Path.Combine(ContentSectorPath, "sandbox.mwc"));
        }

        public static MyMwcObjectBuilder_GlobalData LoadGlobalData()
        {
            string path = Path.Combine(ContentSectorPath, "global.mwg");
            if (File.Exists(path))
            {
                return LoadData<MyMwcObjectBuilder_GlobalData>(path);
            }
            return null;
        }

        public static MyMwcObjectBuilder_Checkpoint LoadCheckpoint()
        {
            string storeDirectory = CurrentSavePath;
            string checkpointPath = Path.Combine(storeDirectory, "Checkpoint.mwc");
            return LoadCheckpoint(checkpointPath);
        }

        public static MyMwcObjectBuilder_Checkpoint LoadCheckpoint(string checkpointPath)
        {
            // TODO: This should be replaced by LoadData<> method
            MyMwcObjectBuilder_Checkpoint result = null;

            if (File.Exists(checkpointPath))
            {
                using (BinaryReader checkpointReader = new BinaryReader(new FileStream(checkpointPath, FileMode.Open, FileAccess.Read)))
                {
                    var version = checkpointReader.ReadInt32();
                    if (version < MyMwcObjectBuilder_Base.FIRST_COMPATIBILITY_VERSION)
                    {
                        MyMwcLog.WriteLine("Loading data from " + checkpointPath + " failed because of incompatible version, save version: " + version);
                        return null;
                    }
                    result = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Checkpoint>(checkpointReader.ReadBytes((int)checkpointReader.BaseStream.Length), version);
                    //result.SectorObjectBuilder = LoadSector(result.CurrentSector);
                }
            }

            return result;
        }

        public static MyMwcObjectBuilder_Sector LoadSector(MyMwcSectorIdentifier sectorId)
        {
            MyMwcObjectBuilder_Sector userdataSector = LoadSector(sectorId, GetSectorPath(sectorId));
            MyMwcObjectBuilder_Sector contentSector = null;

            if (sectorId.UserId == null) // Only non-user sectors loads from Content
            {
                var sectorPath = Path.Combine(ContentSectorPath, GetSectorName(sectorId, false) + ".mws");
                contentSector = LoadSector(sectorId, sectorPath);
            }

            int userVersion = userdataSector != null ? userdataSector.Version : -1;
            int contentVersion = contentSector != null ? contentSector.Version : -1;
            if (userVersion > contentVersion)
            {
                Log("using USERDATA: ", sectorId, userVersion);
                return userdataSector;
            }
            else
            {
                if (contentSector != null)
                {
                    Log("using CONTENT: ", sectorId, contentVersion);
                }
                return contentSector;
            }
        }

        private static MyMwcObjectBuilder_Sector LoadSector(MyMwcSectorIdentifier sectorId, string filename)
        {
            if (File.Exists(filename))
            {
                MyMwcObjectBuilder_Sector result = LoadData<MyMwcObjectBuilder_Sector>(filename);
                if (result == null)
                {
                    MyMwcLog.WriteLine("Incorrect save data");
                    return null;
                }
                Log("hit", sectorId, result.Version);
                return result;
            }
            else
            {
                Log("miss", sectorId);
                return null;
            }
        }

        private static void Log(string text, MyMwcSectorIdentifier sectorId, int? version = null)
        {
            text = (sectorId.UserId == null ? "CACHE: " : "SAVES: ") + text;
            text = text + " " + GetSectorNameLong(sectorId);
            if (version.HasValue)
            {
                text += ", version: " + version;
            }
            Log(text);
        }

        private static void Log(string text)
        {
            MyMwcLog.WriteLine(text);
            MyTrace.Send(TraceWindow.Server, text);
        }

        public static bool HasChapters()
        {
            return LoadChapters().Any();
        }

        internal static bool HasCurrentSave()
        {
            UpgradeSaves();
            return LoadCheckpoint() != null;
            //return File.Exists(Path.Combine(CurrentSavePath, "Checkpoint.mwc"));
        }

        private static void UpgradeSaves()
        {
            var oldCurrentDir = "Current";
            var oldSavePath = Path.Combine(MyFileSystemUtils.GetApplicationUserDataFolder(), "Saves", MyMwcFinalBuildConstants.TYPE_AS_STRING);
            DirectoryInfo dir = new DirectoryInfo(oldSavePath);

            if (!dir.Exists)
                return;

            // Remove files directly in save dir
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (IOException e)
                {
                    MyMwcLog.WriteLine(e);
                }
            }

            // Move subdirs to current save dir
            foreach (var subdir in dir.GetDirectories())
            {
                var parts = subdir.Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                int part0;

                if ((subdir.Name == oldCurrentDir || int.TryParse(parts[0], out part0)) && IsCheckpointMwcInDir(subdir))
                {
                    try
                    {
                        string destDirName = Path.Combine(SavesPath, subdir.Name);
                        if (Directory.Exists(destDirName))
                            Directory.Delete(subdir.FullName, true);
                        else
                            Directory.Move(subdir.FullName, destDirName);
                    }
                    catch (IOException e)
                    {
                        MyMwcLog.WriteLine(e);
                    }
                }
            }

            // Rename saves to remove UserId from filenames
            var savesDir = new DirectoryInfo(SavesPath);
            if (!savesDir.Exists)
                return;

            foreach (var chapterDir in savesDir.GetDirectories())
            {
                foreach (var file in chapterDir.GetFiles("*.mws"))
                {
                    var parts = Path.GetFileName(file.FullName).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (parts.Count == 6 && parts[1] == MyClientServer.LoggedPlayer.GetUserId().ToString())
                    {
                        parts.RemoveAt(1);
                        string newName = parts.Aggregate(CombineSectorName);
                        try
                        {
                            file.MoveTo(Path.Combine(Path.GetDirectoryName(file.FullName), newName));
                        }
                        catch (IOException e)
                        {
                            MyMwcLog.WriteLine(e);
                        }
                    }
                }
            }
        }

        static string CombineSectorName(string a, string b)
        {
            return a + "_" + b;
        }

        private static bool IsCheckpointMwcInDir(DirectoryInfo dir)
        {
            return dir.GetFiles().Any(s => String.Compare(s.Name, "Checkpoint.mwc", true) == 0);
        }
    }
}

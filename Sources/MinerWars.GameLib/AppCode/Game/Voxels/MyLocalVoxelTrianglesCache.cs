using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Voxels;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.Voxels
{
    /// <summary>
    /// Handles local saving and loading voxel triangle cache to disk.
    /// When any changes are made to the cache file format, increase the version in MyMwcFinalBuildConstants.VOXEL_CACHE_FILE_VERSION
    /// </summary>
    static class MyLocalVoxelTrianglesCache
    {
        public static void SaveAllVoxels()
        {
            foreach (var voxelMap in MyVoxelMaps.GetVoxelMaps())
            {
                SaveVoxel(voxelMap);
            }
        }

        static void SaveVoxel(MyVoxelMap voxelMap)
        {
            Debug.Assert(voxelMap.EntityId != null);

            using (var voxelStream = new FileStream(GetVoxelPath(voxelMap), FileMode.Create))
            using (var voxelWriter = new BinaryWriter(voxelStream))
            {
                voxelWriter.Write(MyMwcFinalBuildConstants.VOXEL_CACHE_FILE_VERSION);

                MyMwcVector3Int cellCoord;
                for (cellCoord.X = 0; cellCoord.X < voxelMap.DataCellsCount.X; cellCoord.X++)
                {
                    for (cellCoord.Y = 0; cellCoord.Y < voxelMap.DataCellsCount.Y; cellCoord.Y++)
                    {
                        for (cellCoord.Z = 0; cellCoord.Z < voxelMap.DataCellsCount.Z; cellCoord.Z++)
                        {
                            var dataCell = MyVoxelCacheData.GetCell(voxelMap, ref cellCoord, false);
                            if (dataCell != null/* && dataCell.VoxelTrianglesCount > 0*/)
                            {
                                voxelWriter.Write(cellCoord.X);
                                voxelWriter.Write(cellCoord.Y);
                                voxelWriter.Write(cellCoord.Z);

                                dataCell.Write(voxelWriter);

                                //Log("update", sectorIdentifier, checkpointName, sector.Version);
                            }
                        }
                    }
                }
            }
        }

        static string GetVoxelPath(MyVoxelMap voxelMap)
        {
            var sectorIdentifier = MyGuiScreenGamePlay.Static.GetSectorIdentifier();
            int sectorVersion = MyGuiScreenGamePlay.Static.SectorVersion;

            string path = MyLocalCache.CachePath;
            if (sectorIdentifier.UserId != null) // TODO change?
            {
                path = MyLocalCache.CurrentSavePath;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, GetVoxelName(voxelMap, sectorIdentifier, sectorVersion) + ".mwv");
        }

        static string GetVoxelName(MyVoxelMap voxelMap, MyMwcSectorIdentifier sectorIdentifier, int sectorVersion)
        {
            string name;
            switch (sectorIdentifier.SectorType)
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

            string userId = sectorIdentifier.UserId.HasValue ? sectorIdentifier.UserId.Value.ToString(CultureInfo.InvariantCulture) : "NULL";

            Debug.Assert(voxelMap.EntityId != null, "voxelMap.EntityId != null");
            return String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}", name, userId, sectorIdentifier.Position.X, sectorIdentifier.Position.Y, sectorIdentifier.Position.Z, sectorVersion, voxelMap.VoxelMapId, voxelMap.EntityId.Value.NumericValue);
        }

        public static bool LoadAllVoxels()
        {
            bool foundAll = true;
            foreach (var voxelMap in MyVoxelMaps.GetVoxelMaps())
            {
                var foundVoxelMap = LoadVoxel(voxelMap);

                foundAll &= foundVoxelMap;
            }
            return foundAll;
        }

        static bool LoadVoxel(MyVoxelMap voxelMap)
        {
            var voxelFile = GetVoxelPath(voxelMap);

            if (File.Exists(voxelFile))
            {
                using (FileStream voxelStream = new FileStream(voxelFile, FileMode.Open))
                using (BinaryReader voxelReader = new BinaryReader(voxelStream))
                {
                    var fileVersion = voxelReader.ReadInt32();
                    
                    if (fileVersion != MyMwcFinalBuildConstants.VOXEL_CACHE_FILE_VERSION)
                    {
                        //Log("outdated file", sector, checkpointName, result.Version);
                        File.Delete(voxelFile);
                        return false;
                    }

                    while (voxelStream.Length != voxelStream.Position)
                    {
                        MyMwcVector3Int cellCoord;
                        cellCoord.X = voxelReader.ReadInt32();
                        cellCoord.Y = voxelReader.ReadInt32();
                        cellCoord.Z = voxelReader.ReadInt32();
                        
                        var dataCell = MyVoxelCacheData.AddCell(voxelMap.VoxelMapId, ref cellCoord);

                        var fakeEndpoint = new IPEndPoint(0, 0);
                        //MyMwcMessageIn.ReadObjectBuilderTypeEnumEx(voxelReader, fakeEndpoint);
                        var readSuccessfully = dataCell.Read(voxelReader, fakeEndpoint);

                        if (!readSuccessfully)
                        {
                            MyVoxelCacheData.RemoveCell(voxelMap.VoxelMapId, ref cellCoord);
                            File.Delete(voxelFile);
                            //Log("corrupted file", sector, checkpointName, result.Version);
                            return false;
                        }

                        //= MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_VoxelDataCell>(voxelReader.ReadBytes((int)sectorStream.Length));

                        //Log("update", sectorIdentifier, checkpointName, sector.Version);
                    }
                    //result = MyMwcObjectBuilder_Base.FromBytes<MyMwcObjectBuilder_Sector>(sectorReader.ReadBytes((int)sectorStream.Length));
                }
                //Log("hit", sector, checkpointName, result.Version);
            }
            else
            {
                //Log("miss", sector, checkpointName);
                return false;
            }

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Utils;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;

namespace MinerWars.AppCode.Game.SolarSystem
{
    static class MySolarSystemUtils
    {
        #region SECTORS_KM

        public static float SectorsToKm(int sector)
        {
            return sector * MyBgrCubeConsts.SECTOR_SIZE;
        }

        public static Vector3 SectorsToKm(MyMwcVector3Int sector)
        {
            return new Vector3(SectorsToKm(sector.X), SectorsToKm(sector.Y), SectorsToKm(sector.Z));
        }

        public static int KmToSectors(float position)
        {
            return (int)Math.Round(position / MyBgrCubeConsts.SECTOR_SIZE);
        }

        public static MyMwcVector3Int KmToSectors(Vector3 positionKm)
        {
            return new MyMwcVector3Int(KmToSectors(positionKm.X), KmToSectors(positionKm.Y), KmToSectors(positionKm.Z));
        }

        public static MyMwcVector3Int KmToSectors(Vector3 positionKm, out Vector3 offsetKm)
        {
            MyMwcVector3Int sector = KmToSectors(positionKm);
            offsetKm = (positionKm - SectorsToKm(sector));

            // This is necessary due to float rounding errors
            offsetKm = Vector3.Clamp(offsetKm, new Vector3(-MyMwcSectorConstants.SECTOR_SIZE_HALF / 1000), new Vector3(MyMwcSectorConstants.SECTOR_SIZE_HALF / 1000));

            return sector;
        }

        #endregion

        #region SECTORS_MILLION_KM

        public static float SectorsToMillionKm(int sector)
        {
            return SectorsToKm(sector) / MyBgrCubeConsts.MILLION_KM;
        }

        public static Vector3 SectorsToMillionKm(MyMwcVector3Int sector)
        {
            return new Vector3(SectorsToMillionKm(sector.X), SectorsToMillionKm(sector.Y), SectorsToMillionKm(sector.Z));
        }

        public static int MillionKmToSectors(float positionMillKm)
        {
            return KmToSectors(positionMillKm * MyBgrCubeConsts.MILLION_KM);
        }

        public static MyMwcVector3Int MillionKmToSectors(Vector3 positionMillKm)
        {
            return new MyMwcVector3Int(MillionKmToSectors(positionMillKm.X), MillionKmToSectors(positionMillKm.Y), MillionKmToSectors(positionMillKm.Z));
        }

        public static MyMwcVector3Int MillionKmToSectors(Vector3 positionMillKm, out Vector3 offsetKm)
        {
            MyMwcVector3Int sector = MillionKmToSectors(positionMillKm);
            offsetKm = (positionMillKm - SectorsToMillionKm(sector)) * MyBgrCubeConsts.MILLION_KM;

            // This is necessary due to float rounding errors
            offsetKm = Vector3.Clamp(offsetKm, new Vector3(-MyMwcSectorConstants.SECTOR_SIZE_HALF / 1000), new Vector3(MyMwcSectorConstants.SECTOR_SIZE_HALF / 1000));

            return sector;
        }

        #endregion

        #region UNSCALING DISTANCE RADIUS
        public static float CalculateDistanceUnscaling(Vector3 origin, float radiusWhen1mFromCamera)
        {
            return (origin).Length() * radiusWhen1mFromCamera;
        }

        public static float CalculateDistanceUnscalingFrom(Vector3 origin, float radius, float fixedSizeDistance)
        {
            float cameraDist = (origin).Length();
            if (cameraDist > fixedSizeDistance)
            {
                radius *= cameraDist / fixedSizeDistance;
            }
            return radius;
        }

        public static float CalculateDistanceUnscalingTo(Vector3 origin, float radius, float fixedSizeDistance)
        {
            float cameraDist = (origin).Length();
            if (cameraDist < fixedSizeDistance)
            {
                radius *= cameraDist / fixedSizeDistance;
            }
            return radius;
        }
        #endregion
    }
}

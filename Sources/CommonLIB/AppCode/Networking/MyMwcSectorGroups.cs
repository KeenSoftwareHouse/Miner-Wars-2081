using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using System.Reflection;
using SysUtils;


//  Sector group is a origin/pivot point for a group of sectors in some area. In fact we don't
//  need this if all sector will be relative to sun. But because I want more flexibility when
//  size of sector changes (can happen in future during game-play-testing), I want to have areas
//  where are sectors and those sectors are relative to their sector group.
//  I assume player can't travel in his ship from sector group to other sector group. Distance will
//  be just too large (assuming that traveling from sun to earth will take 7 years).

namespace MinerWars.CommonLIB.AppCode.Networking
{
    //  IMPORTANT: Never delete or change numeric values from this enum. They are used in database too and we don't want inconsistency.
    [System.Obsolete("Use session type")]
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyMwcSectorTypeEnum : short
    {
        STORY = 1,
        MMO = 2,
        SANDBOX = 3
    }

}
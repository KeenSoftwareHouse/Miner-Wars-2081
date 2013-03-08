using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SysUtils
{ 
    //  What game type we start when quick launch is enabled
    public enum MyMwcQuickLaunchType : byte
    {
        NEW_STORY,
        EDITOR_SANDBOX,
        SANDBOX_RANDOM,
        LOAD_CHECKPOINT,
        LAST_SANDBOX
    }

    public enum MyMwcFinalBuildType : byte
    {
        TEST,        //  For our internal testing, not for public (although not ONLY for developers)
        PUBLIC,      //  For everyone in the world
        DEVELOP,      //  Only for developers, not for public nor testing
    }

    [Flags]
    public enum LoggingOptions
    {
        NONE = 1 << 0,
        ENUM_CHECKING = 1 << 1,
        LOADING_MODELS = 1 << 2,
        LOADING_TEXTURES = 1 << 3,
        LOADING_CUSTOM_ASSETS = 1 << 4,
        LOADING_SPRITE_VIDEO = 1 << 5,
        VALIDATING_CUE_PARAMS = 1 << 6,
        CONFIG_ACCESS = 1 << 7,
        SIMPLE_NETWORKING = 1 << 8,
        VOXEL_MAPS = 1 << 9,
        MISC_RENDER_ASSETS = 1 << 10, // Decals, fonts, debug draw objects, simple draw objects
        AUDIO = 1 << 11,
        TRAILERS = 1 << 12,
    }
}

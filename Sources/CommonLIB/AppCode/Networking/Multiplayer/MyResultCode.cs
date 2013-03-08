using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinerWars.CommonLIB.AppCode.Networking.Multiplayer
{
    public enum MyResultCodeEnum: byte
    {
        OK = 1,
        WRONG_USERNAME_OR_PASSWORD = 2,
        WRONG_CLIENT_VERSION = 3,
        ACCESS_RESTRICTED = 4,
        GENERAL_FAILURE = 5,
        GAME_NOT_EXISTS = 6,
        GAME_ALREADY_JOINED = 7,
        GAME_FULL = 8,
        GAME_INVALID_PASSWORD = 9,
        TIMEOUT = 10,
    }
}

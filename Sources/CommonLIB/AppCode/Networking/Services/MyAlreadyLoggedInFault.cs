using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Networking.Services
{
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    [DataContract]
    public class MyAlreadyLoggedInFault
    {
    }
}

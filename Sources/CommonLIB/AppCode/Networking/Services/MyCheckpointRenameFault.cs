using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Networking.Services
{
    public enum MyCheckpointRenameFaultReason
    {
        NameAlreadyExists,
        CheckpointNotFound
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    [DataContract]
    public class MyCheckpointRenameFault
    {
        [DataMember(Name="Reason")]
        public MyCheckpointRenameFaultReason Reason;
    }
}

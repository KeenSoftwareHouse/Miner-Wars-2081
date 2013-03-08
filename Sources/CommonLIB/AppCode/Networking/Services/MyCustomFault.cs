using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;
using SysUtils;

namespace MinerWars.CommonLIB.AppCode.Networking.Services
{
    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    public enum MyCustomFaultCode
    {
        Unknown = 0,
        SectorNameExists,
        UserNotFound,
        SectorNotFound,
        CheckpointTemplateNotExists,
    }

    [Obfuscation(Feature = MyMwcFinalBuildConstants.OBFUSCATION_EXCEPTION_FEATURE, Exclude = true, ApplyToMembers = true)]
    [DataContract]
    public class MyCustomFault
    {
        public MyCustomFault()
        {
            ErrorCode = MyCustomFaultCode.Unknown;
        }

        public MyCustomFault(string message)
            : this()
        {
            Message = message;
        }

        [DataMember]
        public MyCustomFaultCode ErrorCode;

        [DataMember]
        public string Message;

        [DataMember]
        public string StackTrace;

        [DataMember]
        public string FullText;

        public override string ToString()
        {
            return String.Format("{0}: {1}", ErrorCode, Message) + Environment.NewLine + String.Format("Stack trace: {0}", StackTrace) + Environment.NewLine + FullText;
        }
    }
}

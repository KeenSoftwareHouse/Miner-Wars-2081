using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.Networking;
using SysUtils.Utils;

namespace MinerWars.CommonLIB.AppCode.ObjectBuilders
{
    public enum MyGameplayDifficultyEnum
    {
        EASY = 0,
        NORMAL = 1,
        HARD = 2,
    }

    public class MyMwcObjectBuilder_Session : MyMwcObjectBuilder_Base
    {
        public MyGameplayDifficultyEnum Difficulty { get; set; }

        internal MyMwcObjectBuilder_Session()
            : base()
        {
        }

        public MyMwcObjectBuilder_Session(MyGameplayDifficultyEnum difficulty)
        {
            Difficulty = difficulty;
        }

        public override MyMwcObjectBuilderTypeEnum GetObjectBuilderType()
        {
            return MyMwcObjectBuilderTypeEnum.Session;
        }

        internal override bool Read(System.IO.BinaryReader binaryReader, System.Net.EndPoint senderEndPoint, int gameVersion)
        {
            if (!base.Read(binaryReader, senderEndPoint, gameVersion)) return NetworkError();

            int? difficulty = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndPoint);
            if (!difficulty.HasValue) return NetworkError();

            Difficulty = (MyGameplayDifficultyEnum) difficulty.Value;

            return true;
        }

        internal override void Write(System.IO.BinaryWriter binaryWriter)
        {
            base.Write(binaryWriter);

            MyMwcMessageOut.WriteInt32((int)this.Difficulty, binaryWriter);
            MyMwcLog.IfNetVerbose_AddToLog("Difficulty: " + this.Difficulty);
        }
    }
}

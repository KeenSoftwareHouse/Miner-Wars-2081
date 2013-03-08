using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace MinerWars.CommonLIB.AppCode.Networking
{
    static class MyMessageHelper
    {
        public static void WriteStringDictionary(Dictionary<string, string> dictionary, BinaryWriter binaryWriter)
        {
            MyMwcMessageOut.WriteInt32(dictionary.Count, binaryWriter);
            foreach (var item in dictionary)
            {
                MyMwcMessageOut.WriteString(item.Key, binaryWriter);
                MyMwcMessageOut.WriteNullableString(item.Value, binaryWriter);
            }
        }

        public static Dictionary<string, string> ReadStringDictionary(BinaryReader binaryReader, EndPoint senderEndpoint)
        {
            int? count = MyMwcMessageIn.ReadInt32Ex(binaryReader, senderEndpoint);
            if (!count.HasValue) return null;

            var result = new Dictionary<string, string>();
            for (int i = 0; i < count; i++)
            {
                string key = MyMwcMessageIn.ReadStringEx(binaryReader, senderEndpoint);
                if (key == null) return null;

                string value;
                if (!MyMwcMessageIn.ReadNullableStringEx(binaryReader, senderEndpoint, out value)) return null;
                result.Add(key, value);
            }
            return result;
        }
    }
}

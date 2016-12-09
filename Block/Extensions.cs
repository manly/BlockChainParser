using System;
using System.IO;
using System.Text;

namespace BlockChain 
{

    public static class Extensions {

        public static DateTime ReadUnixTimestamp(this BinaryReader reader) {
            return ReadUnixTimestamp(reader.ReadUInt32());
        }
        public static DateTime ReadUnixTimestamp(uint value) {
            // DateTimeOffset.FromUnixTimeSeconds() 4.6+ only
            return DateTime.SpecifyKind(new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(value), DateTimeKind.Utc);
        }

        public static ulong ReadVariableUInt(this BinaryReader reader) {
            byte first = reader.ReadByte();
            if(first <  0xFD) return first;
            if(first == 0xFD) return reader.ReadUInt16();
            if(first == 0xFE) return reader.ReadUInt32();
            return                   reader.ReadUInt64();
        }

    }
}

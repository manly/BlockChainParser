using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
using System.Linq;

namespace BlockChain 
{

    public static class Helper {
        // byte -> hex, to massively speed up byte.ToString("x2")
        private static readonly string[] m_hexValues;

        static Helper() {
            m_hexValues = new string[256];
            for(int i = 0; i < 256; i++)
                m_hexValues[i] = i.ToString("x2", CultureInfo.InvariantCulture);
        }

        public static Hash SHA256(byte[] value, int offset, int count) {
            var sha256 = SHA256Managed.Create();
            var hash = sha256.ComputeHash(value, offset, count);
            return new Hash(hash);
        }
        /// <summary>
        ///     SHA256 ran twice.
        /// </summary>
        public static Hash SHA256_2(byte[] value, int offset, int count) {
            var sha256 = SHA256Managed.Create();
            var hash = sha256.ComputeHash(value, offset, count);
            hash = sha256.ComputeHash(hash);
            return new Hash(hash);
        }

        public static DateTime ReadUnixTimestamp(this BinaryReader reader) {
            return ReadUnixTimestamp(reader.ReadUInt32());
        }

        public static DateTime ReadUnixTimestamp(uint value) {
            // DateTimeOffset.FromUnixTimeSeconds() 4.6+ only
            return DateTime.SpecifyKind(new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(value), DateTimeKind.Utc);
        }

        public static uint ToUnixTimestamp(DateTime value) {
            return (uint)(value - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static ulong ReadVariableUInt(this BinaryReader reader) {
            byte first = reader.ReadByte();
            if(first < 0xFD)  return first;
            if(first == 0xFD) return reader.ReadUInt16();
            if(first == 0xFE) return reader.ReadUInt32();
            return reader.ReadUInt64();
        }
        public static void WriteVariableUInt(ulong value, byte[] buffer, ref int index) {
            if(value < 0xFD)
                buffer[index++] = (byte)value;
            else if(value <= 0xFFFF) {
                buffer[index++] = (byte)0xFD;
                buffer[index++] = (byte)((value >> 0) & 0xFF);
                buffer[index++] = (byte)((value >> 8) & 0xFF);
            } else if(value <= 0xFFFFFFFF) {
                buffer[index++] = (byte)0xFE;
                buffer[index++] = (byte)((value >> 0) & 0xFF);
                buffer[index++] = (byte)((value >> 8) & 0xFF);
                buffer[index++] = (byte)((value >> 16) & 0xFF);
                buffer[index++] = (byte)((value >> 24) & 0xFF);
            } else {
                buffer[index++] = (byte)0xFF;
                buffer[index++] = (byte)((value >> 0) & 0xFF);
                buffer[index++] = (byte)((value >> 8) & 0xFF);
                buffer[index++] = (byte)((value >> 16) & 0xFF);
                buffer[index++] = (byte)((value >> 24) & 0xFF);
                buffer[index++] = (byte)((value >> 32) & 0xFF);
                buffer[index++] = (byte)((value >> 40) & 0xFF);
                buffer[index++] = (byte)((value >> 48) & 0xFF);
                buffer[index++] = (byte)((value >> 56) & 0xFF);
            }
        }

        public static string Hex(byte[] value) {
            //return string.Join(string.Empty, value.Select(o => o.ToString("x2", CultureInfo.InvariantCulture)));
            var sb = new StringBuilder(value.Length << 1);
            for(int i = 0; i < value.Length; i++)
                sb.Append(m_hexValues[value[i]]);
            return sb.ToString();
        }

    }
}

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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

        public static Hash SHA256(byte[] buffer, int offset, int count) {
            var sha256 = SHA256Managed.Create();
            var hash = sha256.ComputeHash(buffer, offset, count);
            return new Hash(hash);
        }
        /// <summary>
        ///     SHA256 ran twice.
        /// </summary>
        public static Hash SHA256_2(byte[] buffer, int offset, int count) {
            var sha256 = SHA256Managed.Create();
            var hash = sha256.ComputeHash(buffer, offset, count);
            hash = sha256.ComputeHash(hash);
            return new Hash(hash);
        }

        public static DateTime ReadUnixTimestamp(byte[] buffer, ref int index) {
            return ReadUnixTimestamp(
                ((uint)buffer[index++] << 0)  |
                ((uint)buffer[index++] << 8)  |
                ((uint)buffer[index++] << 16) |
                ((uint)buffer[index++] << 24));
        }

        public static DateTime ReadUnixTimestamp(uint value) {
            // DateTimeOffset.FromUnixTimeSeconds() 4.6+ only
            return DateTime.SpecifyKind(new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(value), DateTimeKind.Utc);
        }

        public static uint ToUnixTimestamp(DateTime value) {
            return (uint)(value - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static ulong ReadVariableUInt(byte[] buffer, ref int index) {
            byte first = buffer[index++];
            if(first < 0xFD)
                return first;
            if(first == 0xFD)
                return 
                    ((uint)buffer[index++] << 0) |
                    ((uint)buffer[index++] << 8);
            if(first == 0xFE)
                return
                    ((uint)buffer[index++] << 0)  |
                    ((uint)buffer[index++] << 8)  |
                    ((uint)buffer[index++] << 16) |
                    ((uint)buffer[index++] << 24);
            return
                ((ulong)buffer[index++] << 0)  |
                ((ulong)buffer[index++] << 8)  |
                ((ulong)buffer[index++] << 16) |
                ((ulong)buffer[index++] << 24) |
                ((ulong)buffer[index++] << 32) |
                ((ulong)buffer[index++] << 40) |
                ((ulong)buffer[index++] << 48) |
                ((ulong)buffer[index++] << 56);
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

        private static Regex m_scriptPattern = new Regex(@"\b[0-9a-fA-F]+\b", RegexOptions.Compiled);
        public static string DetectScriptPattern(string script) {
            return m_scriptPattern.Replace(script, m => $"[hex size={m.Length.ToString(CultureInfo.InvariantCulture)}]").Trim();
        }
    }
}

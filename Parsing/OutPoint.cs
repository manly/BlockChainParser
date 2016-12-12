using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class OutPoint {
        public Hash Hash;
        public uint Index;

        public override string ToString() {
            return string.Format("{{out_point: {0}}}", this.Index == 0xFFFFFFFF ? "0xffffffff" : this.Index.ToString(CultureInfo.InvariantCulture));
        }

        public static OutPoint Parse(byte[] buffer, ref int index) {
            return new OutPoint() {
                Hash = Hash.Parse(buffer, ref index, 32),
                Index =
                    ((uint)buffer[index++] << 0)  |
                    ((uint)buffer[index++] << 8)  |
                    ((uint)buffer[index++] << 16) |
                    ((uint)buffer[index++] << 24),
            };
        }
    }

}
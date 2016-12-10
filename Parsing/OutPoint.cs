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
            return string.Format(CultureInfo.InvariantCulture, "{{out_point: {0}}}", this.Index);
        }

        public static OutPoint Parse(BinaryReader reader) {
            return new OutPoint() {
                Hash = Hash.Parse(reader, 32),
                Index = reader.ReadUInt32(),
            };
        }
    }

}
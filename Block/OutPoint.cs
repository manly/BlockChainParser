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
            var res = new OutPoint();

            res.Hash = Hash.Parse(reader, 32);
            res.Index = reader.ReadUInt32();
            
            return res;
        }
    }

}
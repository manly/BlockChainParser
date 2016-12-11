using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain 
{

    public class Hash : ByteArrayContainerBase {
        private const bool IS_LITTLE_ENDIAN = true;

        private Hash(BinaryReader reader, int bytes) : base(reader, bytes, IS_LITTLE_ENDIAN) { }
        public Hash(byte[] hash) : base(hash, IS_LITTLE_ENDIAN) { }


        public static Hash Parse(BinaryReader reader, int bytes) {
            return new Hash(reader, bytes);
        }
    }

}

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

        private Hash(byte[] buffer, ref int index, int bytes) : base(buffer, ref index, bytes, IS_LITTLE_ENDIAN) { }
        public Hash(byte[] hash) : base(hash, IS_LITTLE_ENDIAN) { }


        public static Hash Parse(byte[] buffer, ref int index, int bytes) {
            return new Hash(buffer, ref index, bytes);
        }
    }

}

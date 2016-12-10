using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain 
{

    public class Hash : ByteArrayContainerBase {
        private Hash(BinaryReader reader, int bytes) : base(reader, bytes, true) { }

        public static Hash Parse(BinaryReader reader, int bytes) {
            return new Hash(reader, bytes);
        }
    }

}

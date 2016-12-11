using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class SignatureScript : ByteArrayContainerBase {
        private const bool IS_LITTLE_ENDIAN = false;

        private SignatureScript(BinaryReader reader, int size) : base(reader, size, IS_LITTLE_ENDIAN) { }

        public static SignatureScript Parse(BinaryReader reader, int size) {
            return new SignatureScript(reader, size);
        }
    }

}
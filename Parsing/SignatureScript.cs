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

        private SignatureScript(byte[] buffer, ref int index, int size) : base(buffer, ref index, size, IS_LITTLE_ENDIAN) { }

        public static SignatureScript Parse(byte[] buffer, ref int index, int size) {
            return new SignatureScript(buffer, ref index, size);
        }
    }

}
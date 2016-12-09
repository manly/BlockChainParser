using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class SignatureScript : ByteArrayContainerBase {
        private SignatureScript(BinaryReader reader, int size) : base(reader, size, false) { }

        public static SignatureScript Parse(BinaryReader reader, int size) {
            return new SignatureScript(reader, size);
        }
    }

}
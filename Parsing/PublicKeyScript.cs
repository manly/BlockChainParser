using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class PublicKeyScript : ByteArrayContainerBase {
        private PublicKeyScript(BinaryReader reader, int size) : base(reader, size, false) { }

        public override string ToString() {
            return NBitcoin.Script.FromBytesUnsafe(this.Raw).ToString();
        }

        public static PublicKeyScript Parse(BinaryReader reader, int size) {
            return new PublicKeyScript(reader, size);
        }
    }

}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class PublicKeyScript : ByteArrayContainerBase {
        private const bool IS_LITTLE_ENDIAN = false;

        private PublicKeyScript(byte[] buffer, ref int index, int bytes) : base(buffer, ref index, bytes, IS_LITTLE_ENDIAN) { }

        public override string ToString() {
            return NBitcoin.Script.FromBytesUnsafe(this.Raw).ToString();
        }

        public static PublicKeyScript Parse(byte[] buffer, ref int index, int size) {
            return new PublicKeyScript(buffer, ref index, size);
        }
    }

}
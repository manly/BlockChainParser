using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class TransactionOut {
        public BitcoinValue    Value;
        public PublicKeyScript Script;

        public override string ToString() {
            return string.Format("{{tx_out: {0}}}", this.Value);
        }

        public static TransactionOut Parse(BinaryReader reader) {
            var res = new TransactionOut();

            res.Value = BitcoinValue.FromSatoshis(reader.ReadInt64());

            var script_length = reader.ReadVariableUInt();
            res.Script = PublicKeyScript.Parse(reader, (int)script_length);
            return res;
        }
    }
}
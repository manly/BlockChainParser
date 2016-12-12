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

        public static TransactionOut Parse(byte[] buffer, ref int index) {
            var res = new TransactionOut();

            res.Value = BitcoinValue.Read(buffer, ref index);

            var script_length = Helper.ReadVariableUInt(buffer, ref index);
            res.Script = PublicKeyScript.Parse(buffer, ref index, (int)script_length);
            return res;
        }
    }
}
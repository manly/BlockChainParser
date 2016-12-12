using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain {

    public class TransactionIn {
        public OutPoint        PreviousOutput;
        public SignatureScript Script;
        public uint            Sequence;

        public override string ToString() {
            return string.Format("{{tx_in: {0}}}", this.Sequence == 0xFFFFFFFF ? "0xffffffff" : this.Sequence.ToString(CultureInfo.InvariantCulture));
        }

        public static TransactionIn Parse(byte[] buffer, ref int index) {
            var res = new TransactionIn();

            res.PreviousOutput = OutPoint.Parse(buffer, ref index);

            var script_length = Helper.ReadVariableUInt(buffer, ref index);
            res.Script = SignatureScript.Parse(buffer, ref index, (int)script_length);

            // see http://bitcoin.stackexchange.com/questions/2025/what-is-txins-sequence
            // Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
            res.Sequence = 
                ((uint)buffer[index++] << 0)  |
                ((uint)buffer[index++] << 8)  |
                ((uint)buffer[index++] << 16) |
                ((uint)buffer[index++] << 24);
            return res;
        }
    }
}
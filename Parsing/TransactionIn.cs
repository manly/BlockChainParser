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

        public static TransactionIn Parse(BinaryReader reader) {
            var res = new TransactionIn();

            res.PreviousOutput = OutPoint.Parse(reader);

            var script_length = reader.ReadVariableUInt();
            res.Script = SignatureScript.Parse(reader, (int)script_length);

            // see http://bitcoin.stackexchange.com/questions/2025/what-is-txins-sequence
            res.Sequence = reader.ReadUInt32(); // Transaction version as defined by the sender. Intended for "replacement" of transactions when information is updated before inclusion into a block.
            return res;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public class Transaction {
        public int              Version;
        public TransactionIn[]  Ins;
        public TransactionOut[] Outs;
        public LockTime         LockTime;

        public override string ToString() {
            return string.Format(CultureInfo.InvariantCulture, "{{tx: {0}}}", BitcoinValue.FromSatoshis(this.Outs.Sum(o => o.Value.Satoshis)));
        }

        public static Transaction Parse(BinaryReader reader) {
            var res = new Transaction();

            res.Version = reader.ReadInt32();

            var transactionCount = reader.ReadVariableUInt();
            res.Ins = new TransactionIn[transactionCount];
            for(ulong i = 0; i < transactionCount; i++)
                res.Ins[i] = TransactionIn.Parse(reader);

            transactionCount = reader.ReadVariableUInt();
            res.Outs = new TransactionOut[transactionCount];
            for(ulong i = 0; i < transactionCount; i++)
                res.Outs[i] = TransactionOut.Parse(reader);

            // the doc makes special mention of this:
            // "If all TxIn inputs have final (0xffffffff) sequence numbers then lock_time is irrelevant. Otherwise, the transaction may not be added to a block until after lock_time (see NLockTime)."
            // as such, this might hide data under those context
            var is_lock_time_irrelevant = res.Ins.All(o => o.Sequence == 0xFFFFFFFF);
            res.LockTime = LockTime.Parse(reader, is_lock_time_irrelevant);

            return res;
        }
    }

}
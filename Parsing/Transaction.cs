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

        private Hash m_hash = null;
        public Hash TransactionHash {
            get {
                if(m_hash == null)
                    m_hash = CalculateTransactionHash();
                return m_hash;
            }
        }
        
        private Hash CalculateTransactionHash() {
            var raw = new byte[26 + this.Ins.Sum(o => o.Script.Length + 49) + this.Outs.Sum(o => o.Script.Length + 13)];
            int index = 0;

            raw[index++] = (byte)((this.Version >> 0)  & 0xFF);
            raw[index++] = (byte)((this.Version >> 8)  & 0xFF);
            raw[index++] = (byte)((this.Version >> 16) & 0xFF);
            raw[index++] = (byte)((this.Version >> 24) & 0xFF);

            var in_count = this.Ins.Length;
            Helper.WriteVariableUInt((ulong)in_count, raw, ref index);

            foreach(var tx in this.Ins) {
                Buffer.BlockCopy(tx.PreviousOutput.Hash.GetOriginalRaw(), 0, raw, index, 32);
                index += 32;
                raw[index++] = (byte)((tx.PreviousOutput.Index >> 0)  & 0xFF);
                raw[index++] = (byte)((tx.PreviousOutput.Index >> 8)  & 0xFF);
                raw[index++] = (byte)((tx.PreviousOutput.Index >> 16) & 0xFF);
                raw[index++] = (byte)((tx.PreviousOutput.Index >> 24) & 0xFF);

                var scriptLength = tx.Script.Length;
                Helper.WriteVariableUInt((ulong)scriptLength, raw, ref index);
                Buffer.BlockCopy(tx.Script.GetOriginalRaw(), 0, raw, index, scriptLength);
                index += scriptLength;

                raw[index++] = (byte)((tx.Sequence >> 0)  & 0xFF);
                raw[index++] = (byte)((tx.Sequence >> 8)  & 0xFF);
                raw[index++] = (byte)((tx.Sequence >> 16) & 0xFF);
                raw[index++] = (byte)((tx.Sequence >> 24) & 0xFF);
            }

            var out_count = this.Outs.Length;
            Helper.WriteVariableUInt((ulong)out_count, raw, ref index);

            foreach(var tx in this.Outs) {
                raw[index++] = (byte)((tx.Value.Satoshis >> 0)  & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 8)  & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 16) & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 24) & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 32) & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 40) & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 48) & 0xFF);
                raw[index++] = (byte)((tx.Value.Satoshis >> 56) & 0xFF);

                var scriptLength = tx.Script.Length;
                Helper.WriteVariableUInt((ulong)scriptLength, raw, ref index);
                Buffer.BlockCopy(tx.Script.GetOriginalRaw(), 0, raw, index, scriptLength);
                index += scriptLength;
            }

            var locktime = this.LockTime.GetRaw();
            raw[index++] = (byte)((locktime >> 0)  & 0xFF);
            raw[index++] = (byte)((locktime >> 8)  & 0xFF);
            raw[index++] = (byte)((locktime >> 16) & 0xFF);
            raw[index++] = (byte)((locktime >> 24) & 0xFF);

            return Helper.SHA256_2(raw, 0, index);
        }

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
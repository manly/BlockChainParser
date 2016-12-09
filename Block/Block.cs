using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{
    public class Block {
        public int           ByteLength;
        public int           Version;
        public Hash          PrevBlockHash;
        public Hash          MerkleRoot;
        public DateTime      Timestamp;
        public uint          Bits; 
        public uint          Nonce;
        public Transaction[] Transactions;

        public override string ToString() {
            var total_btc = BitcoinValue.FromSatoshis(this.Transactions
                .SelectMany(o => o.Outs)
                .Select(o => o.Value.Satoshis)
                .Sum());
            return string.Format(CultureInfo.InvariantCulture, "{{{0}: {1} tx ({2})}}", this.Timestamp, this.Transactions.Length, total_btc);
        }

        public static Block Parse(Stream stream) {
            var res = new Block();
            var stream_start = stream.Position;

            // important: BinaryReader reads in little-endian no matter the platform
            using(var reader = new BinaryReader(stream, Encoding.UTF8, true)) { // keep open
                // identifies the blockchain network (main/testnet/testnet3/namecoin)
                var magic_signature = reader.ReadUInt32();
                if(magic_signature != 0xD9B4BEF9)
                    throw new InvalidOperationException("this code only works on the main blockchain network");

                res.ByteLength = (int)reader.ReadUInt32();
                res.Version = reader.ReadInt32();
                res.PrevBlockHash = Hash.Parse(reader, 32);
                res.MerkleRoot = Hash.Parse(reader, 32);
                res.Timestamp = reader.ReadUnixTimestamp();
                res.Bits = reader.ReadUInt32(); // difficulty
                res.Nonce = reader.ReadUInt32();

                var transactionCount = reader.ReadVariableUInt();
                res.Transactions = new Transaction[transactionCount];

                for(ulong i = 0; i < transactionCount; i++)
                    res.Transactions[i] = Transaction.Parse(reader);
            }
            
            if(stream.Position != stream_start + res.ByteLength + 8)
                throw new InvalidOperationException("parse error");

            return res;
        }

        public static IEnumerable<Block> ParseAll(string path) {
            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                while(stream.Position < stream.Length) {
                    yield return Block.Parse(stream);
                }
            }
        }
    }

}

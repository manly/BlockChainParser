using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{
    public class Block {
        public int           Height; // index from 0
        public int           ByteLength;
        public int           Version;
        public Hash          PrevBlockHash;
        public Hash          MerkleRoot;
        public DateTime      Timestamp;
        public uint          Bits; 
        public uint          Nonce;
        public Transaction[] Transactions;

        private Hash m_hash = null;
        public Hash BlockHash {
            get {
                if(m_hash == null)
                    m_hash = CalculateBlockHash();
                return m_hash;
            }
        }

        private Hash CalculateBlockHash() {
            var raw = new byte[80];
            
            raw[0] = (byte)((this.Version >> 0)    & 0xFF);
            raw[1] = (byte)((this.Version >> 8)    & 0xFF);
            raw[2] = (byte)((this.Version >> 16)   & 0xFF);
            raw[3] = (byte)((this.Version >> 24)   & 0xFF);
            Buffer.BlockCopy(this.PrevBlockHash.GetOriginalRaw(), 0, raw, 4, 32);
            Buffer.BlockCopy(this.MerkleRoot.GetOriginalRaw(), 0, raw, 36, 32);
            var unixTimestamp = Helper.ToUnixTimestamp(this.Timestamp);
            raw[68] = (byte)((unixTimestamp >> 0)  & 0xFF);
            raw[69] = (byte)((unixTimestamp >> 8)  & 0xFF);
            raw[70] = (byte)((unixTimestamp >> 16) & 0xFF);
            raw[71] = (byte)((unixTimestamp >> 24) & 0xFF);
            raw[72] = (byte)((this.Bits >> 0)      & 0xFF);
            raw[73] = (byte)((this.Bits >> 8)      & 0xFF);
            raw[74] = (byte)((this.Bits >> 16)     & 0xFF);
            raw[75] = (byte)((this.Bits >> 24)     & 0xFF);
            raw[76] = (byte)((this.Nonce >> 0)     & 0xFF);
            raw[77] = (byte)((this.Nonce >> 8)     & 0xFF);
            raw[78] = (byte)((this.Nonce >> 16)    & 0xFF);
            raw[79] = (byte)((this.Nonce >> 24)    & 0xFF);

            return Helper.SHA256_2(raw, 0, raw.Length);
        }

        public override string ToString() {
            var total_btc = BitcoinValue.FromSatoshis(this.Transactions
                .SelectMany(o => o.Outs)
                .Select(o => o.Value.Satoshis)
                .Sum());
            return string.Format(CultureInfo.InvariantCulture, "{{{0}  {1}: {2} tx ({3})}}", this.BlockHash, this.Timestamp, this.Transactions.Length, total_btc);
        }

        public static Block Parse(Stream stream, int block_height = -1) {
            var res = new Block();
            var stream_start = stream.Position;

            // important: BinaryReader reads in little-endian no matter the platform
            using(var reader = new BinaryReader(stream, Encoding.UTF8, true)) { // keep open
                // identifies the blockchain network (main/testnet/testnet3/namecoin)
                var magic_signature = reader.ReadUInt32();
                if(magic_signature != 0xD9B4BEF9)
                    throw new InvalidOperationException("this code only works on the main blockchain network");

                res.Height = block_height;
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

        public static IEnumerable<Block> ParseAll(string path, int block_height = -1) {
            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                while(stream.Position < stream.Length) {
                    yield return Block.Parse(stream, block_height >= 0 ? block_height++ : -1);
                }
            }
        }
    }

}

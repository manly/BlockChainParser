using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{
    public class Block {
        public Hash          BlockHash;

        public int           Height = -1; // this is set afterward parsing because the .dat files ordering != Height ordering
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

            // read header and length
            var header = new byte[8];
            if(stream.Read(header, 0, 8) < 8)
                throw new ApplicationException("The block isn't fully downloaded yet.");

            // sometimes the blockfiles have zeros padded extra data that we skip
            // this is mostly an issue with old bitcoin clients
            if(header[0] == 0) {
                var temp = new byte[4096];
                stream.Position = stream_start;
                int read;
                while((read = stream.Read(temp, 0, 4096)) > 0){
                    bool found_nonzero = false;
                    for(int i = 0; i < read; i++) {
                        if(temp[i] != 0) {
                            found_nonzero = true;
                            stream_start += i;
                            stream.Position = stream_start;
                            if(stream.Read(header, 0, 8) < 8)
                                throw new ApplicationException("The block isn't fully downloaded yet.");
                            break;
                        }
                    }
                    if(found_nonzero)
                        break;
                    stream_start += read;
                }
            }

            int index = 0;
            var magic_signature = 
                ((uint)header[index++] << 0)  |
                ((uint)header[index++] << 8)  |
                ((uint)header[index++] << 16) |
                ((uint)header[index++] << 24);

            //NETWORK     MAGIC VALUE
            //main        0xD9B4BEF9
            //testnet     0xDAB5BFFA
            //testnet3    0x0709110B
            //namecoin    0xFEB4BEF9
            if(magic_signature != 0xD9B4BEF9)
                throw new InvalidOperationException("this code only works on the main blockchain network");

            var block_length =
                ((uint)header[index++] << 0)  |
                ((uint)header[index++] << 8)  |
                ((uint)header[index++] << 16) |
                ((uint)header[index++] << 24);

            if(block_length < 0 || block_length > 1000000)
                throw new InvalidOperationException();

            byte[] buffer = new byte[block_length];
            if(stream.Read(buffer, 0, (int)block_length) < block_length)
                throw new ApplicationException("The block isn't fully downloaded yet.");

            index = 0;
            res.BlockHash = Helper.SHA256_2(buffer, 0, 80);

            res.Version =
                ((int)buffer[index++] << 0)  |
                ((int)buffer[index++] << 8)  |
                ((int)buffer[index++] << 16) |
                ((int)buffer[index++] << 24);
            res.PrevBlockHash = Hash.Parse(buffer, ref index, 32);
            res.MerkleRoot = Hash.Parse(buffer, ref index, 32);
            res.Timestamp = Helper.ReadUnixTimestamp(buffer, ref index);
            res.Bits = // difficulty
                ((uint)buffer[index++] << 0)  |
                ((uint)buffer[index++] << 8)  |
                ((uint)buffer[index++] << 16) |
                ((uint)buffer[index++] << 24);
            res.Nonce =
                ((uint)buffer[index++] << 0)  |
                ((uint)buffer[index++] << 8)  |
                ((uint)buffer[index++] << 16) |
                ((uint)buffer[index++] << 24);

            var transaction_count = Helper.ReadVariableUInt(buffer, ref index);
            res.Transactions = new Transaction[transaction_count];

            for(ulong i = 0; i < transaction_count; i++)
                res.Transactions[i] = Transaction.Parse(buffer, ref index);

            if(index != block_length)
                throw new ApplicationException("parse error");
            
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

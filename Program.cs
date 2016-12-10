using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlockChain {
    class Program {
        static void Main(string[] args) {
            var folder = GetBlocksFolder();

            var files = System.IO.Directory.GetFiles(folder, "*.dat");

            // this code is intentionally not parallel to remain simple
            // it used to be, but the bottleneck is IO anyway and wasn't that much faster
            int block_id = 0;
            foreach(var file in files) {
                ParseFile(file, ref block_id);
            }
        }

        private static void ParseFile(string file, ref int block_id) {
            var sb = new StringBuilder();

            try {
                foreach(var block in Block.ParseAll(file)) {
                    int tx_id = 0;
                    // todo: do something here with the blocks
                    foreach(var transaction in block.Transactions) {
                        foreach(var txout in transaction.Outs) {
                            var text_script = txout.Script.ToString();
                            if(DataChunkExtractor.Filter(text_script))
                                continue;
                            sb.AppendFormat("block {0}", block_id);
                            sb.AppendLine();
                            sb.AppendFormat("transaction {0}", tx_id);
                            sb.AppendLine();
                            sb.AppendLine(text_script);
                        }
                        tx_id++;
                    }
                    block_id++;
                }
            } catch(Exception ex) {
                throw new FormatException($"An error occured reading the file {file}. File might be corrupted or contain extra data.", ex);
            }

            System.IO.File.WriteAllText(System.IO.Path.ChangeExtension(file, "txt"), sb.ToString());
        }

        private static string GetBlocksFolder() {
            var author_bitcoin_folder = @"E:\BitCoinCore BlockChain\blocks";
            if(System.IO.Directory.Exists(author_bitcoin_folder))
                return author_bitcoin_folder;

            // change this to your "BitCoin Core" folder. ie: "%appdata%/Bitcoin/data" ?
            return @".\blocks\";
        }
    }
}

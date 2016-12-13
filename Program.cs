using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;


namespace BlockChain {
    class Program {
        static void Main(string[] args) {
            var folder = GetBlocksFolder();

            // point to your "bitcoin core" blocks folder
            var files = Directory.GetFiles(folder, "blk*.dat");


            // this code is intentionally not parallel to remain simple
            // it used to be, but the bottleneck is IO anyway and wasn't that much faster

            //LogNonStdTransactions(files);
            //LogScriptPatterns(files, @"e:\patterns.txt");
            //IndexBlocksAndTransactions(files, @"e:\blocks_index.csv", @"e:\tx_index.csv");
            LogTransactionScriptsTexts(files, @"e:\inputs_scripts_as_text.txt", @"e:\outputs_scripts_as_text.txt");
        }

        private static string GetBlocksFolder() {
            var author_bitcoin_folder = @"c:\blocks";
            if(Directory.Exists(author_bitcoin_folder))
                return author_bitcoin_folder;

            // change this to your "BitCoin Core" folder. ie: "%appdata%/Bitcoin/data" ?
            return @".\blocks\";
        }

        #region private static LogNonStdTransactions()
        /// <summary>
        ///     Logs all non-standard transactions.
        ///     This is useful for detecting which blockfiles/block are likely to contain hidden data.
        /// </summary>
        private static void LogNonStdTransactions(string[] files) {
            var sb = new StringBuilder();
            var sb_transactions = new StringBuilder();
            string current_file = null;
            
            try {
                int block_height = 0;
                foreach(var file in files) {
                    current_file = file;
                    foreach(var block in Block.ParseAll(file)) {
                        block.Height = block_height++;
                        Console.WriteLine(string.Format("processing block #{0}", block.Height));

                        foreach(var transaction in block.Transactions) {
                            foreach(var txout in transaction.Outs) {
                                var text_script = txout.Script.ToString();

                                if(DataChunkExtractor.Filter(text_script))
                                    continue;
                                sb.AppendFormat("block {0} - transaction https://blockchain.info/tx/{1}", block.Height, transaction.TransactionHash);
                                sb.AppendLine();
                                sb.AppendLine(text_script);
                                sb.AppendLine();

                                sb_transactions.AppendLine(transaction.TransactionHash.ToString());
                            }
                        }
                    }
                    var folder = Path.Combine(Path.GetDirectoryName(file), "non-std scripts");
                    if(!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    var newfile = Path.Combine(folder, Path.ChangeExtension(Path.GetFileName(file), "txt"));
                    File.WriteAllText(newfile, sb.ToString());

                    newfile = Path.ChangeExtension(newfile, ".transactions.txt");
                    File.WriteAllText(newfile, sb_transactions.ToString());
                }
            } catch(Exception ex) {
                throw new FormatException($"An error occured reading the file {current_file}. File might be corrupted or contain extra data. This also occurs when reading block files from old version of bitcoin core.", ex);
            }
        }
        #endregion
        #region private static LogScriptPatterns()
        /// <summary>
        ///     Logs all transaction scripts formats, so that this way we can identify 
        ///     the different ways data might be encoded on the blockchain.
        /// </summary>
        private static void LogScriptPatterns(string[] files, string output_pattern_file = @"e:\patterns.txt") {
            var sb = new StringBuilder();
            var patterns = new Dictionary<string, ScriptPattern>();
            string current_file = null;

            try {
                int block_height = 0;
                foreach(var file in files) {
                    current_file = file;
                    foreach(var block in Block.ParseAll(file)) {
                        block.Height = block_height++;
                        Console.WriteLine(string.Format("processing block #{0}", block.Height));
                        foreach(var transaction in block.Transactions) {
                            foreach(var txout in transaction.Outs) {
                                var text_script = txout.Script.ToString();

                                var pattern = Helper.DetectScriptPattern(text_script);
                                ScriptPattern temp;
                                if(!patterns.TryGetValue(pattern, out temp)) {
                                    temp = new ScriptPattern() {
                                        Pattern = pattern,
                                        Example = text_script,
                                        TxId = transaction.TransactionHash.ToString(),
                                    };
                                    patterns.Add(pattern, temp);
                                }
                                temp.Count++;
                            }
                        }
                    }
                    File.WriteAllText(output_pattern_file, string.Join(Environment.NewLine, patterns.Select(o => o.Value)));
                }
            } catch(Exception ex) {
                throw new FormatException($"An error occured reading the file {current_file}. File might be corrupted or contain extra data. This also occurs when reading block files from old version of bitcoin core.", ex);
            }
        }

        private class ScriptPattern {
            public long Count;
            public string Pattern;
            public string Example;
            public string TxId;
            public override string ToString() {
                var sb = new StringBuilder();
                sb.Append(this.Count.ToString(CultureInfo.InvariantCulture));
                sb.Append(": ");
                sb.AppendLine(this.Pattern);
                sb.AppendFormat("example: {0}", this.Example);
                sb.AppendLine();
                sb.AppendFormat("transaction: https://blockchain.info/tx/{0}", this.TxId);
                sb.AppendLine();
                return sb.ToString();
            }
        }
        #endregion
        #region private static IndexBlocksAndTransactions()
        /// <summary>
        ///     Logs all non-standard transactions.
        ///     This is useful for detecting which blockfiles/block are likely to contain hidden data.
        /// </summary>
        private static void IndexBlocksAndTransactions(string[] files, string output_block_index_file = @"e:\blocks_index.csv", string output_tx_index_file = @"e:\tx_index.csv") {
            string current_file = null;

            try {
                int block_height = 0;
                foreach(var file in files) {
                    current_file = file;
                    foreach(var block in Block.ParseAll(file)) {
                        block.Height = block_height++;
                        Console.WriteLine(string.Format("processing block #{0}", block.Height));

                        using(var block_index_stream = File.Open(output_block_index_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) {
                            using(var writer = new StreamWriter(block_index_stream, Encoding.UTF8))
                                writer.WriteLine(string.Format("{0},{1},{2:yyyy-MM-dd HH:mm:ss}", block.BlockHash.ToString(), block.Height, block.Timestamp));
                        }

                        using(var tx_index_stream = File.Open(output_tx_index_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) {
                            using(var writer = new StreamWriter(tx_index_stream, Encoding.UTF8))
                                writer.WriteLine(string.Join(Environment.NewLine, block.Transactions.Select(o => string.Format("{0},{1}", o.TransactionHash, block.Height))));
                        }
                    }
                }
            } catch(Exception ex) {
                throw new FormatException($"An error occured reading the file {current_file}. File might be corrupted or contain extra data. This also occurs when reading block files from old version of bitcoin core.", ex);
            }
        }
        #endregion
        #region private static LogTransactionScriptsTexts()
        /// <summary>
        ///     Logs all transactions that contains text strings.
        /// </summary>
        private static void LogTransactionScriptsTexts(string[] files, string output_file_inputscripts = @"e:\inputs_scripts_as_text.txt", string output_file_outputscripts = @"e:\outputs_scripts_as_text.txt") {
            string current_file = null;

            foreach(var file in new[] { output_file_inputscripts, output_file_outputscripts }) {
                if(File.Exists(file))
                    File.Delete(file);
            }
            var sb_in = new StringBuilder();
            var sb_out = new StringBuilder();

            try {
                int block_height = 0;
                foreach(var file in files) {
                    current_file = file;
                    sb_in = new StringBuilder();
                    sb_out = new StringBuilder();

                    foreach(var block in Block.ParseAll(file)) {
                        block.Height = block_height++;
                        Console.WriteLine(string.Format("processing block #{0}", block.Height));

                        foreach(var transaction in block.Transactions) {
                            foreach(var txin in transaction.Ins) {
                                // input scripts have to be valid, however, there is no doc for how they are formatted 
                                // so we do this awful hack of attempting to extract text from it
                                var text = Helper.ExtractText(txin.Script.Raw);
                                if(text == null)
                                    continue;
                                sb_in.AppendFormat("block {0} - https://blockchain.info/tx/{1} - {2}", block.Height, transaction.TransactionHash, text);
                                sb_in.AppendLine();
                            }

                            foreach(var txout in transaction.Outs) {
                                // output scripts are not required to be valid, but any currency going to them will end up lost if they arent
                                // because of this, there are 2 interpretation of every output script:
                                // 1- the script is valid (money is received), meaning the data will be encoded within relevant chunks
                                // 2- the script is invalid (money will be lost), meaning to interpret the script as raw binary data

                                // read the raw binary, convert to text
                                var text = Helper.ExtractText(txout.Script.Raw);
                                if(text != null && text.Length > 3) {
                                    text = text.Substring(3);
                                    // they always begin with 'v??' here, so remove that
                                    sb_out.AppendFormat("block {0} - https://blockchain.info/tx/{1} - {2}", block.Height, transaction.TransactionHash, text);
                                    sb_out.AppendLine();
                                }

                                var text_script = txout.Script.ToString();
                                if(DataChunkExtractor.Filter(text_script))
                                    continue;

                                // merge all chunks of data, that are hex encoded, into just one chunk
                                var all_chunks = Helper.ExtractScriptChunks(text_script);
                                text = Helper.ExtractText(Helper.HexDecode(all_chunks));

                                if(text != null) {
                                    sb_out.AppendFormat("block {0} - https://blockchain.info/tx/{1} - {2}", block.Height, transaction.TransactionHash, text);
                                    sb_out.AppendLine();
                                }
                            }
                        }
                    }
                    File.AppendAllText(output_file_inputscripts, sb_in.ToString(), Encoding.UTF8);
                    File.AppendAllText(output_file_outputscripts, sb_out.ToString(), Encoding.UTF8);
                }
            } catch(Exception ex) {
                throw new FormatException($"An error occured reading the file {current_file}. File might be corrupted or contain extra data. This also occurs when reading block files from old version of bitcoin core.", ex);
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlockChain {
    class Program {
        static void Main(string[] args) {
            // change this to your "BitCoin Core" folder. ie: "%appdata%/Bitcoin/data" ?
            var folder = @".\blocks\";

            var files = System.IO.Directory.GetFiles(folder, "*.dat");
            foreach(var file in files) {
                try {
                    var blocks = Block.ParseAll(file).ToList();
                } catch(Exception ex) {
                    throw new FormatException($"An error occured reading the file {file}", ex);
                }
            }
        }
    }
}

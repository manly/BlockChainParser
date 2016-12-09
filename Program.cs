using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlockChain {
    class Program {
        static void Main(string[] args) {
            var folder = @".\blocks\";

            var files = System.IO.Directory.GetFiles(folder, "*.dat");
            foreach(var file in files) {
                var blocks = Block.ParseAll(file).ToList();
            }
        }
    }
}

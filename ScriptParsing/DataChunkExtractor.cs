using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlockChain
{

    public class DataChunkExtractor {
        private static Regex m_filter = new Regex(
            "^OP_DUP OP_HASH160 [^ ]+ OP_EQUALVERIFY OP_CHECKSIG$|" +
            "^OP_HASH160 [^ ]+ OP_EQUAL$|" +
            "^[^ ]{130} OP_CHECKSIG$",
            RegexOptions.Compiled);

        public static bool Filter(string script) {
            return m_filter.IsMatch(script);
        }

        public static IEnumerable<string> Extract(string script) {
            // todo: code
            yield break;
        }
    }

}

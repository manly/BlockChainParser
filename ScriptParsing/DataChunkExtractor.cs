using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlockChain
{

    public class DataChunkExtractor {
        // OP_DUP OP_HASH160 fef1ae394fa74e7a75520b7848797e348b53ee15 OP_EQUALVERIFY OP_CHECKSIG
        // OP_HASH160 9876b3b755b6112a672d89bcbed2a3376919f165 OP_EQUAL

        // 1 204e000068c6b35f9000c049288f883e225e33df0fc211e00d04d2af4254256be983c79c803dd13aa3af8ff3e2a9e408916456673e3dd9141bb6269b73371eaf41 42cee9d84641c6855517a476b64105703c1cf5e16325518aad75e927b3598b69ee1b1150c2894f9dac9c0bbef78f6428698ad555b3468f7f299b95d6f9b0b7de48 dc5f309cb2743037fca57ab485d8c42cab15958615c90934006f0669d208fd277cd341cceb0ddd1c3c27a816b20f3ef1d74e391f62183974c939dd107663550b0e 3 OP_CHECKMULTISIG
        // 04a39b9e4fbd213ef24bb9be69de4a118dd0644082e47c01fd9159d38637b83fbcdc115a5d6e970586a012d1cfe3e3a8b1a3d04e763bdc5a071c0e827c0bd834a5 OP_CHECKSIG

        private static Regex m_filter = new Regex(
            "^OP_DUP OP_HASH160 [^ ]+ OP_EQUALVERIFY OP_CHECKSIG$|" +
            "^OP_HASH160 [^ ]+ OP_EQUAL$|" +
            "^[^ ]{130} OP_CHECKSIG$",
            RegexOptions.Compiled);

        public static bool Filter(string script) {
            return m_filter.IsMatch(script);
        }

        public static IEnumerable<string> Extract(string script) {
            yield break;

        }
    }

}

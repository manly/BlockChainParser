using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlockChain {

    // https://en.bitcoin.it/wiki/OP_RETURN
    public class OPReturnPrefix {
        public string Service;
        public string Prefix;
    }

    public class OPReturnPrefixes {
        private static OPReturnPrefix[] m_list = null;
        
        public static IEnumerable<OPReturnPrefix> GetList() {
            if(m_list == null) {
                m_list = ("SPK,CoinSpark|DOCPROOF,Proof of Existence|CryptoTests-,Crypto Copyright|CryptoProof-,Crypto Copyright|BS,BlockSign|OA,Open Assets|STAMPD##,stampd|Factom!!,Factom|FACTOM00,Factom|Fa,Factom|FA,Factom|tradle,Tradle|LaPreuve,LaPreuve|" + (char)0x58 + (char)0x88 + ",Blockstore|" + (char)0x58 + (char)0x08 + ",Blockstore|id,Blockstore|BITPROOF,Bitproof|S1,Stampery|S2,Stampery|S3,Stampery|S4,Stampery|S5,Stampery|ASCRIBE,Ascribe|ProveBit,ProveBit|EW,Eternity Wall|CC,Colu|omni,Omni Layer|MG,Monegraph|RMBd,Remembr|RMBe,Remembr|ORIGMY,OriginalMy|BID,Identity")
                    .Split('|')
                    .Select(o => {
                        var split = o.Split(',');
                        return new OPReturnPrefix() { Prefix = split[0], Service = split[1] };
                    }).ToArray();
            }
            return m_list;
        }
    }
}
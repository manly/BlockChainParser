using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public struct LockTime {
        private uint m_raw;
        public bool IsIrrelevant;

        public bool ContainsHiddenData {
            get {
                // If all TxIn inputs have final (0xffffffff) sequence numbers then lock_time is irrelevant. Otherwise, the transaction may not be added to a block until after lock_time (see NLockTime).
                return this.IsIrrelevant && m_raw != 0;
            }
        }

        public bool IsLocked {
            get {
                return m_raw != 0;
            }
        }

        /// <summary>
        ///     Returns -1 if it doesnt unlock at a block.
        /// </summary>
        public int UnlocksAtBlock {
            get {
                if(m_raw < 500000000)
                    return (int)m_raw;
                return -1;
            }
        }

        /// <summary>
        ///     Returns the date at which it unlocks.
        ///     Returns MinValue if not applicable.
        /// </summary>
        public DateTime UnlocksAt {
            get {
                if(m_raw >= 500000000)
                    return Extensions.ReadUnixTimestamp(m_raw);
                return DateTime.MinValue;
            }
        }

        public override string ToString() {
            if(this.ContainsHiddenData)
                return $"{{HIDDEN DATA FOUND: (uint32) 0x{m_raw.ToString("x2", CultureInfo.InvariantCulture)}}}";
            if(this.IsIrrelevant)
                return "{irrelevant}";
            if(!this.IsLocked)
                return "{not locked}";
            if(this.UnlocksAtBlock >= 0)
                return $"{{unlocks at block {this.UnlocksAtBlock.ToString(CultureInfo.InvariantCulture)}}}";
            return $"{{unlocks at {this.UnlocksAt}}}";
        }


        public static LockTime Parse(BinaryReader reader, bool is_lock_time_irrelevant) {
            var res = new LockTime() {
                m_raw = reader.ReadUInt32(),
                IsIrrelevant = is_lock_time_irrelevant,
            };
            return res;
        }
    }

}
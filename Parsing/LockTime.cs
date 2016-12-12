using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain
{

    public struct LockTime {
        private uint m_value;
        public bool IsIrrelevant;

        public bool ContainsHiddenData {
            get {
                // If all TxIn inputs have final (0xffffffff) sequence numbers then lock_time is irrelevant. Otherwise, the transaction may not be added to a block until after lock_time (see NLockTime).
                return this.IsIrrelevant && m_value != 0;
            }
        }

        public uint GetRaw() {
            return m_value;
        }

        public bool IsLocked {
            get {
                return m_value != 0;
            }
        }

        /// <summary>
        ///     Returns -1 if it doesnt unlock at a block.
        /// </summary>
        public int UnlocksAtBlock {
            get {
                if(m_value < 500000000)
                    return (int)m_value;
                return -1;
            }
        }

        /// <summary>
        ///     Returns the date at which it unlocks.
        ///     Returns MinValue if not applicable.
        /// </summary>
        public DateTime UnlocksAt {
            get {
                if(m_value >= 500000000)
                    return Helper.ReadUnixTimestamp(m_value);
                return DateTime.MinValue;
            }
        }

        public override string ToString() {
            if(this.ContainsHiddenData)
                return $"{{HIDDEN DATA FOUND: (uint32) 0x{m_value.ToString("x2", CultureInfo.InvariantCulture)}}}";
            if(this.IsIrrelevant)
                return "{irrelevant}";
            if(!this.IsLocked)
                return "{not locked}";
            if(this.UnlocksAtBlock >= 0)
                return $"{{unlocks at block {this.UnlocksAtBlock.ToString(CultureInfo.InvariantCulture)}}}";
            return $"{{unlocks at {this.UnlocksAt}}}";
        }


        public static LockTime Parse(byte[] buffer, ref int index, bool is_lock_time_irrelevant) {
            return new LockTime() {
                m_value = 
                    ((uint)buffer[index++] << 0)  |
                    ((uint)buffer[index++] << 8)  |
                    ((uint)buffer[index++] << 16) |
                    ((uint)buffer[index++] << 24),
                IsIrrelevant = is_lock_time_irrelevant,
            };
        }
    }

}
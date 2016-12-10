using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain 
{

    public abstract class ByteArrayContainerBase {
        private readonly byte[] m_raw;
        private bool m_needsReverse = true;

        public byte[] Raw {
            get {
                if(m_needsReverse) {
                    Array.Reverse(m_raw); // little endian; dont reverse until read
                    m_needsReverse = false;
                }
                return m_raw;
            }
        }

        public ByteArrayContainerBase(BinaryReader reader, int bytes, bool is_little_endian) {
            if(bytes < 0 || bytes > 1000000)
                throw new InvalidOperationException();

            // weird pattern with reader in order to avoid needless array.reverse() on most data
            m_raw = new byte[bytes];
            reader.Read(m_raw, 0, bytes);
            m_needsReverse = is_little_endian;
        }

        public override string ToString() {
            return string.Join(string.Empty, this.Raw.Select(o => o.ToString("x2", CultureInfo.InvariantCulture)));
        }
    }

}

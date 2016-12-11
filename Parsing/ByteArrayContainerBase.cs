using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain 
{

    public abstract class ByteArrayContainerBase : IComparable, IComparable<ByteArrayContainerBase> {
        private readonly byte[] m_raw;
        private bool m_needsReverse = true;
        private readonly bool m_is_little_endian;

        public byte[] Raw {
            get {
                if(m_needsReverse) {
                    Array.Reverse(m_raw); // little endian; dont reverse until read
                    m_needsReverse = false;
                }
                return m_raw;
            }
        }
        public int Length {
            get {
                return m_raw.Length;
            }
        }

        public byte[] GetOriginalRaw() {
            if(!m_is_little_endian)
                return this.Raw;

            if(m_needsReverse)
                // if the data was never reversed, then return directly
                return m_raw;

            var raw = new byte[this.Raw.Length];
            Buffer.BlockCopy(this.Raw, 0, raw, 0, this.Raw.Length);
            Array.Reverse(raw);
            return raw;
        }

        public ByteArrayContainerBase(byte[] raw, bool is_little_endian) {
            m_raw = raw;
            m_needsReverse = is_little_endian;
            m_is_little_endian = is_little_endian;
        }
        public ByteArrayContainerBase(BinaryReader reader, int bytes, bool is_little_endian) {
            if(bytes < 0 || bytes > 1000000)
                throw new InvalidOperationException();

            // weird pattern with reader in order to avoid needless array.reverse() on most data
            m_raw = new byte[bytes];
            reader.Read(m_raw, 0, bytes);
            m_needsReverse = is_little_endian;
            m_is_little_endian = is_little_endian;
        }

        public override string ToString() {
            return string.Join(string.Empty, Helper.Hex(this.Raw));
        }

        public int CompareTo(object obj) {
            return this.CompareTo((ByteArrayContainerBase)obj);
        }
        public int CompareTo(ByteArrayContainerBase other) {
            var diff = this.Length - other.Length;
            if(diff != 0)
                return diff;
            byte[] raw1;
            byte[] raw2;
            if(this.m_needsReverse ^ other.m_needsReverse) {
                raw1 = this.Raw;
                raw2 = other.Raw;
            } else {
                raw1 = m_raw;
                raw2 = other.m_raw;
            }
            for(int i = 0; i < this.Length; i++) {
                diff = m_raw[i] - other.m_raw[i];
                if(diff != 0)
                    return diff;
            }
            return 0;
        }
    }

}

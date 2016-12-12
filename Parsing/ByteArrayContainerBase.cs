using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BlockChain 
{

    public abstract class ByteArrayContainerBase : IComparable, IComparable<ByteArrayContainerBase>, IEquatable<ByteArrayContainerBase> {
        public readonly byte[] Raw;

        public ByteArrayContainerBase(byte[] raw, bool is_little_endian) {
            this.Raw = raw;

            if(is_little_endian)
                Array.Reverse(this.Raw);
        }
        public ByteArrayContainerBase(byte[] buffer, ref int index, int bytes, bool is_little_endian) {
            this.Raw = new byte[bytes];

            if(is_little_endian) {
                for(int i = 0; i < bytes; i++)
                    this.Raw[i] = buffer[index + bytes - i - 1];
            } else
                Buffer.BlockCopy(buffer, index, this.Raw, 0, bytes);

            index += bytes;
        }

        public override string ToString() {
            return string.Join(string.Empty, Helper.Hex(this.Raw));
        }


        public bool Equals(ByteArrayContainerBase other) {
            return this.CompareTo(other) == 0;
        }
        public override bool Equals(object obj) {
            if(obj is ByteArrayContainerBase)
                return this.Equals((ByteArrayContainerBase)obj);
            return false;
        }
        public override int GetHashCode() {
            return this.Raw.GetHashCode();
        }
        public int CompareTo(object obj) {
            return this.CompareTo((ByteArrayContainerBase)obj);
        }
        public int CompareTo(ByteArrayContainerBase other) {
            var diff = this.Raw.Length - other.Raw.Length;
            if(diff != 0)
                return diff;
            for(int i = 0; i < this.Raw.Length; i++) {
                diff = this.Raw[i] - other.Raw[i];
                if(diff != 0)
                    return diff;
            }
            return 0;
        }
    }

}

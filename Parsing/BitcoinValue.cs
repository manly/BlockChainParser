using System;
using System.Globalization;

namespace BlockChain
{

    public struct BitcoinValue : IComparable, IComparable<BitcoinValue>, IEquatable<BitcoinValue> {
        public const int SatoshisPerBitcoin  = 100000000;
        public const int BitsPerBitcoin      = 1000000;
        public const int MilliBitsPerBitcoin = 1000;

        public static readonly BitcoinValue Zero = new BitcoinValue();


        private readonly decimal m_btc;

        public BitcoinValue(decimal btc) {
            m_btc = btc;
        }

        public decimal Btc       => m_btc;
        public decimal MilliBits => m_btc * BitcoinValue.MilliBitsPerBitcoin;
        public decimal Bits      => m_btc * BitcoinValue.BitsPerBitcoin;
        public long Satoshis     => (long)(m_btc * BitcoinValue.SatoshisPerBitcoin);

        

        public static BitcoinValue FromSatoshis(long satoshis) => new BitcoinValue((decimal)satoshis / BitcoinValue.SatoshisPerBitcoin);
        public static BitcoinValue FromBits(decimal bits)      => new BitcoinValue(bits / BitcoinValue.BitsPerBitcoin);
        public static BitcoinValue FromMilliBits(decimal mBtc) => new BitcoinValue(mBtc / BitcoinValue.MilliBitsPerBitcoin);
        public static BitcoinValue FromBtc(decimal btc)        => new BitcoinValue(btc);

        public static BitcoinValue Read(byte[] buffer, ref int index) {
            return FromSatoshis(
                ((long)buffer[index++] << 0)  |
                ((long)buffer[index++] << 8)  |
                ((long)buffer[index++] << 16) |
                ((long)buffer[index++] << 24) |
                ((long)buffer[index++] << 32) |
                ((long)buffer[index++] << 40) |
                ((long)buffer[index++] << 48) |
                ((long)buffer[index++] << 56));
        }


        public static BitcoinValue operator +(BitcoinValue x, BitcoinValue y) {
            return new BitcoinValue(x.Btc + y.Btc);
        }
        public static BitcoinValue operator -(BitcoinValue x, BitcoinValue y) {
            return new BitcoinValue(x.Btc - y.Btc);
        }

        public bool Equals(BitcoinValue other) {
            return this.Btc == other.Btc;
        }
        public override bool Equals(object obj) {
            if(obj is BitcoinValue)
                return this.Equals((BitcoinValue)obj);
            return false;
        }
        public override int GetHashCode() {
            return m_btc.GetHashCode();
        }
        public int CompareTo(object obj) {
            return this.CompareTo((BitcoinValue)obj);
        }
        public int CompareTo(BitcoinValue other) {
            return this.m_btc.CompareTo(other.m_btc);
        }

        public override string ToString() {
            return string.Format(CultureInfo.InvariantCulture, "{{{0} btc}}", this.Btc);
        }
    }

}

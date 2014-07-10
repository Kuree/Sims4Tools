using System;
using System.Collections.Generic;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Calculate the CRC of a data chunk stored in a Sims3Pack file.
    /// </summary>
    public class Sims3PackCRC : HashAlgorithm
    {
        #region table
        private static readonly UInt64[] table = new UInt64[]
        {
            0x0000000000000000, 0x9336EAA9EBE1F042, 0x266DD453D7C3E185, 0xB55B3EFA3C2211C7,
            0xDFEC420E45663349, 0x4CDAA8A7AE87C30B, 0xF981965D92A5D2CC, 0x6AB77CF47944228E,
            0xBED9851C8ACC6692, 0x2DEF6FB5612D96D0, 0x98B4514F5D0F8717, 0x0B82BBE6B6EE7755,
            0x6135C712CFAA55DB, 0xF2032DBB244BA599, 0x475813411869B45E, 0xD46EF9E8F388441C,
            0xEF85E190FF783D66, 0x7CB30B391499CD24, 0xC9E835C328BBDCE3, 0x5ADEDF6AC35A2CA1,
            0x3069A39EBA1E0E2F, 0xA35F493751FFFE6D, 0x160477CD6DDDEFAA, 0x85329D64863C1FE8,
            0x515C648C75B45BF4, 0xC26A8E259E55ABB6, 0x7731B0DFA277BA71, 0xE4075A7649964A33,
            0x8EB0268230D268BD, 0x1D86CC2BDB3398FF, 0xA8DDF2D1E7118938, 0x3BEB18780CF0797A,
            0xDE0BC321FFF17ACC, 0x4D3D298814108A8E, 0xF866177228329B49, 0x6B50FDDBC3D36B0B,
            0x01E7812FBA974985, 0x92D16B865176B9C7, 0x278A557C6D54A800, 0xB4BCBFD586B55842,
            0x60D2463D753D1C5E, 0xF3E4AC949EDCEC1C, 0x46BF926EA2FEFDDB, 0xD58978C7491F0D99,
            0xBF3E0433305B2F17, 0x2C08EE9ADBBADF55, 0x9953D060E798CE92, 0x0A653AC90C793ED0,
            0x318E22B1008947AA, 0xA2B8C818EB68B7E8, 0x17E3F6E2D74AA62F, 0x84D51C4B3CAB566D,
            0xEE6260BF45EF74E3, 0x7D548A16AE0E84A1, 0xC80FB4EC922C9566, 0x5B395E4579CD6524,
            0x8F57A7AD8A452138, 0x1C614D0461A4D17A, 0xA93A73FE5D86C0BD, 0x3A0C9957B66730FF,
            0x50BBE5A3CF231271, 0xC38D0F0A24C2E233, 0x76D631F018E0F3F4, 0xE5E0DB59F30103B6,
            0x2F216CEA150205DA, 0xBC178643FEE3F598, 0x094CB8B9C2C1E45F, 0x9A7A52102920141D,
            0xF0CD2EE450643693, 0x63FBC44DBB85C6D1, 0xD6A0FAB787A7D716, 0x4596101E6C462754,
            0x91F8E9F69FCE6348, 0x02CE035F742F930A, 0xB7953DA5480D82CD, 0x24A3D70CA3EC728F,
            0x4E14ABF8DAA85001, 0xDD2241513149A043, 0x68797FAB0D6BB184, 0xFB4F9502E68A41C6,
            0xC0A48D7AEA7A38BC, 0x539267D3019BC8FE, 0xE6C959293DB9D939, 0x75FFB380D658297B,
            0x1F48CF74AF1C0BF5, 0x8C7E25DD44FDFBB7, 0x39251B2778DFEA70, 0xAA13F18E933E1A32,
            0x7E7D086660B65E2E, 0xED4BE2CF8B57AE6C, 0x5810DC35B775BFAB, 0xCB26369C5C944FE9,
            0xA1914A6825D06D67, 0x32A7A0C1CE319D25, 0x87FC9E3BF2138CE2, 0x14CA749219F27CA0,
            0xF12AAFCBEAF37F16, 0x621C456201128F54, 0xD7477B983D309E93, 0x44719131D6D16ED1,
            0x2EC6EDC5AF954C5F, 0xBDF0076C4474BC1D, 0x08AB39967856ADDA, 0x9B9DD33F93B75D98,
            0x4FF32AD7603F1984, 0xDCC5C07E8BDEE9C6, 0x699EFE84B7FCF801, 0xFAA8142D5C1D0843,
            0x901F68D925592ACD, 0x03298270CEB8DA8F, 0xB672BC8AF29ACB48, 0x25445623197B3B0A,
            0x1EAF4E5B158B4270, 0x8D99A4F2FE6AB232, 0x38C29A08C248A3F5, 0xABF470A129A953B7,
            0xC1430C5550ED7139, 0x5275E6FCBB0C817B, 0xE72ED806872E90BC, 0x741832AF6CCF60FE,
            0xA076CB479F4724E2, 0x334021EE74A6D4A0, 0x861B1F144884C567, 0x152DF5BDA3653525,
            0x7F9A8949DA2117AB, 0xECAC63E031C0E7E9, 0x59F75D1A0DE2F62E, 0xCAC1B7B3E603066C,
            0xCD74327DC0E5FAF6, 0x5E42D8D42B040AB4, 0xEB19E62E17261B73, 0x782F0C87FCC7EB31,
            0x129870738583C9BF, 0x81AE9ADA6E6239FD, 0x34F5A4205240283A, 0xA7C34E89B9A1D878,
            0x73ADB7614A299C64, 0xE09B5DC8A1C86C26, 0x55C063329DEA7DE1, 0xC6F6899B760B8DA3,
            0xAC41F56F0F4FAF2D, 0x3F771FC6E4AE5F6F, 0x8A2C213CD88C4EA8, 0x191ACB95336DBEEA,
            0x22F1D3ED3F9DC790, 0xB1C73944D47C37D2, 0x049C07BEE85E2615, 0x97AAED1703BFD657,
            0xFD1D91E37AFBF4D9, 0x6E2B7B4A911A049B, 0xDB7045B0AD38155C, 0x4846AF1946D9E51E,
            0x9C2856F1B551A102, 0x0F1EBC585EB05140, 0xBA4582A262924087, 0x2973680B8973B0C5,
            0x43C414FFF037924B, 0xD0F2FE561BD66209, 0x65A9C0AC27F473CE, 0xF69F2A05CC15838C,
            0x137FF15C3F14803A, 0x80491BF5D4F57078, 0x3512250FE8D761BF, 0xA624CFA6033691FD,
            0xCC93B3527A72B373, 0x5FA559FB91934331, 0xEAFE6701ADB152F6, 0x79C88DA84650A2B4,
            0xADA67440B5D8E6A8, 0x3E909EE95E3916EA, 0x8BCBA013621B072D, 0x18FD4ABA89FAF76F,
            0x724A364EF0BED5E1, 0xE17CDCE71B5F25A3, 0x5427E21D277D3464, 0xC71108B4CC9CC426,
            0xFCFA10CCC06CBD5C, 0x6FCCFA652B8D4D1E, 0xDA97C49F17AF5CD9, 0x49A12E36FC4EAC9B,
            0x231652C2850A8E15, 0xB020B86B6EEB7E57, 0x057B869152C96F90, 0x964D6C38B9289FD2,
            0x422395D04AA0DBCE, 0xD1157F79A1412B8C, 0x644E41839D633A4B, 0xF778AB2A7682CA09,
            0x9DCFD7DE0FC6E887, 0x0EF93D77E42718C5, 0xBBA2038DD8050902, 0x2894E92433E4F940,
            0xE2555E97D5E7FF2C, 0x7163B43E3E060F6E, 0xC4388AC402241EA9, 0x570E606DE9C5EEEB,
            0x3DB91C999081CC65, 0xAE8FF6307B603C27, 0x1BD4C8CA47422DE0, 0x88E22263ACA3DDA2,
            0x5C8CDB8B5F2B99BE, 0xCFBA3122B4CA69FC, 0x7AE10FD888E8783B, 0xE9D7E57163098879,
            0x836099851A4DAAF7, 0x1056732CF1AC5AB5, 0xA50D4DD6CD8E4B72, 0x363BA77F266FBB30,
            0x0DD0BF072A9FC24A, 0x9EE655AEC17E3208, 0x2BBD6B54FD5C23CF, 0xB88B81FD16BDD38D,
            0xD23CFD096FF9F103, 0x410A17A084180141, 0xF451295AB83A1086, 0x6767C3F353DBE0C4,
            0xB3093A1BA053A4D8, 0x203FD0B24BB2549A, 0x9564EE487790455D, 0x065204E19C71B51F,
            0x6CE57815E5359791, 0xFFD392BC0ED467D3, 0x4A88AC4632F67614, 0xD9BE46EFD9178656,
            0x3C5E9DB62A1685E0, 0xAF68771FC1F775A2, 0x1A3349E5FDD56465, 0x8905A34C16349427,
            0xE3B2DFB86F70B6A9, 0x70843511849146EB, 0xC5DF0BEBB8B3572C, 0x56E9E1425352A76E,
            0x828718AAA0DAE372, 0x11B1F2034B3B1330, 0xA4EACCF9771902F7, 0x37DC26509CF8F2B5,
            0x5D6B5AA4E5BCD03B, 0xCE5DB00D0E5D2079, 0x7B068EF7327F31BE, 0xE830645ED99EC1FC,
            0xD3DB7C26D56EB886, 0x40ED968F3E8F48C4, 0xF5B6A87502AD5903, 0x668042DCE94CA941,
            0x0C373E2890088BCF, 0x9F01D4817BE97B8D, 0x2A5AEA7B47CB6A4A, 0xB96C00D2AC2A9A08,
            0x6D02F93A5FA2DE14, 0xFE341393B4432E56, 0x4B6F2D6988613F91, 0xD859C7C06380CFD3,
            0xB2EEBB341AC4ED5D, 0x21D8519DF1251D1F, 0x94836F67CD070CD8, 0x07B585CE26E6FC9A,
        };
        #endregion

        private UInt64 seed64;
        private UInt64 hash64;

        /// <summary>
        /// Create a new CRC algorithm with an optional seed.
        /// </summary>
        /// <param name="seed">Optional CRC algorithm seed.</param>
        public Sims3PackCRC(UInt64 seed = 0x00FFFFFFFFFFFFFF) { seed64 = seed; Initialize(); }

        #region HashAlgorithm implementation
        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize { get { return 64; } }

        /// <summary>
        /// Update the running hash with the data passed.
        /// </summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            UInt64 crc = SwapEndian(hash64);
            for (int i = ibStart; i < cbSize; i++)
                crc = (crc >> 8) ^ table[(array[i] ^ crc) & 0xFF];
            hash64 = SwapEndian(crc);
        }

        /// <summary>
        /// Finalize the computation of the hash value.
        /// </summary>
        /// <returns>The computed hash code.</returns>
        protected override byte[] HashFinal() { HashSizeValue = 64; HashValue = BitConverter.GetBytes(~this.hash64); return HashValue; }

        /// <summary>
        /// Initialize the Sims3PackCRC by setting the current CRC value to the seed.
        /// </summary>
        public override void Initialize() { hash64 = seed64; }
        #endregion

        /// <summary>
        /// Calculates the Sims3Pack CRC value for the specified region of the specified byte array.
        /// </summary>
        /// <param name="buffer">The input to calculate the CRC for.</param>
        /// <param name="offset">The offset into the byte array from which to begin using data.</param>
        /// <param name="count">The number of bytes in the array to use as data.</param>
        /// <returns>The calculated CRC.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="count"/> is an invalid value.  -or- <paramref name="buffer"/> length is invalid.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="offset"/> is out of range. This parameter requires a non-negative number.</exception>
        public static UInt64 CalculateCRC(byte[] buffer, int offset = 0, int count = -1)
        {
            return BitConverter.ToUInt64(new Sims3PackCRC().ComputeHash(buffer, offset, count == -1 ? buffer.Length : count), 0);
        }

        /// <summary>
        /// Calculate the CRC for the specified <see cref="System.IO.Stream"/>.
        /// </summary>
        /// <param name="stream">The input to calculate the CRC for.</param>
        /// <returns>CRC of <paramref name="stream"/>.</returns>
        public static UInt64 CalculateCRC(System.IO.Stream stream)
        {
            return BitConverter.ToUInt64(new Sims3PackCRC().ComputeHash(stream), 0);
        }

        #region Helpers
        private static UInt64 SwapEndian(UInt64 value)
        {
            if (!BitConverter.IsLittleEndian) return value;

            byte[] res = BitConverter.GetBytes(value);
            Array.Reverse(res);
            return BitConverter.ToUInt64(res, 0);
        }
        #endregion
    }
}

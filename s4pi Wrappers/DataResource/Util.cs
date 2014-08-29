using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace s4pi.DataResource
{
    public static class Util
    {
        public const uint Zero32 = 0;
        public const uint NullOffset = 0x80000000;

        public static string GetString(BinaryReader r, long nameOffset)
        {
            if (nameOffset == Util.NullOffset) return "";
            long startPosition = r.BaseStream.Position;
            r.BaseStream.Position = nameOffset;
            List<byte> array = new List<byte>();
            byte c = r.ReadByte();
            while(c != 0x00)
            {
                array.Add(c);
                c = r.ReadByte();
            }
            
            r.BaseStream.Position = startPosition;
            return Encoding.ASCII.GetString(array.ToArray());
        }

        public static void WriteString(BinaryWriter w, string str)
        {
            byte[] array = new byte[str.Length + 1];
            Encoding.ASCII.GetBytes(str, 0, str.Length, array, 0);
            array[str.Length] = 0;
            w.Write(array);
        }

        public static bool GetOffset(BinaryReader r, out uint offset)
        {
            offset = r.ReadUInt32();
            if (offset == Util.NullOffset) return false;
            offset += (uint)r.BaseStream.Position - 4;
            return true;
        }

        public static void Padding(BinaryWriter w, long count)
        {
            for (int i = 0; i < count; i++) w.Write((byte)0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace s4pi.DataResource
{
    public static class Util
    {
        public static string GetString(BinaryReader r, long nameOffset)
        {
            if (nameOffset == 0x80000000) return "";
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

        public static bool GetOffset(BinaryReader r, out uint offset)
        {
            offset = r.ReadUInt32();
            if (offset == 0x80000000) return false;
            offset += (uint)r.BaseStream.Position - 4;
            return true;
        }
    }
}

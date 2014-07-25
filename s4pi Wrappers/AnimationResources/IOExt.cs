using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Animation
{
    public static class IOExt
    {
        public static string ReadString32(this BinaryReader br)
        {
            var c = br.ReadInt32();
            var s = "";
            for (int i = 0; i < c; i++)
            {
                s += br.ReadChar();
            }
            return s;
        }

        public static void WriteString32(this BinaryWriter bw,String s)
        {
            bw.Write(s.Length);
            bw.Write(Encoding.ASCII.GetBytes(s));
        }
        public static string ReadStringFixed(this BinaryReader br, int length = 0x80)
        {
            var e = "";
            for (var i = 0; i < 0x80; i++)
            {
                var ch = br.ReadChar();
                if (ch != (char)0)
                    e += ch;
            }
            e = e.Trim();
            return e;
        }

        public static void WriteStringFixed(this BinaryWriter bw, string s, int length = 0x80)
        {
            var padding = length - s.Length;
            bw.Write(Encoding.ASCII.GetBytes(s));
            for(var i =0;i<padding;i++)
                bw.Write((byte)0);

        }
        public static string ReadZString(this BinaryReader br)
        {
            return ReadZString(br, 0);
        }

        public static string ReadZString(this BinaryReader br, int padLength)
        {
            String s = "";
            byte b = br.ReadByte();
            while (b != 0)
            {
                s += Encoding.ASCII.GetString(new byte[1] { b });
                b = br.ReadByte();
            }
            if (padLength != 0)
            {
                int count = padLength - 1 - s.Length;
                br.BaseStream.Seek(count, SeekOrigin.Current);
            }
            return s;
        }

        public static void WriteZString(this BinaryWriter bw, String s)
        {
            WriteZString(bw, s, 0x00, 0);
        }

        public static void WriteZString(this BinaryWriter bw, String s, byte paddingChar, int padLength)
        {
            if (!string.IsNullOrEmpty(s))
            {
                bw.Write(Encoding.ASCII.GetBytes(s));
            }

            bw.Write((byte)0x00);
            if (padLength > 0)
            {
                int count = padLength - 1;
                if (!string.IsNullOrEmpty(s)) count -= s.Length;
                for (int i = 0; i < count; i++)
                {
                    bw.Write(paddingChar);
                }
            }
        }
    }
}

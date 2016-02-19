/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *  Modified 2015 by cmar                                                  *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Animation
{
    public static class IOExt
    {
        public const int FixedStringLength = 0x80;

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

        public static void WriteString32(this BinaryWriter bw, String s)
        {
            bw.Write(s.Length);
            bw.Write(Encoding.ASCII.GetBytes(s));
        }

        public static string ReadStringFixed(this BinaryReader br, int length = FixedStringLength)
        {
            var e = "";
            for (var i = 0; i < length; i++)
            {
                var ch = br.ReadChar();
                if (ch != (char)0)
                    e += ch;
            }
            e = e.Trim();
            return e;
        }

        public static void WriteStringFixed(this BinaryWriter bw, string s, int length = FixedStringLength)
        {
            var padding = length - s.Length;
            bw.Write(Encoding.ASCII.GetBytes(s));
            for(var i =0; i<padding; i++)
                bw.Write((byte)0);

        }
        public static string ReadZString(this BinaryReader br)
        {
            return ReadZString(br, 0);
        }

        public static string ReadZString(this BinaryReader br, int padLength)
        {
            List<byte> l = new List<byte>();
            byte b = br.ReadByte();
            while (b != 0)
            {
                l.Add(b);
                b = br.ReadByte();
            }
            for (int i = 0; i < padLength; i++)
            {
                br.ReadByte();
            }
            return Encoding.UTF8.GetString(l.ToArray()); ;
        }

        public static void WriteZString(this BinaryWriter bw, String s)
        {
            WriteZString(bw, s, 0x00, 0);
        }

        public static void WriteZString(this BinaryWriter bw, String s, byte paddingChar, int padLength)
        {
            if (!string.IsNullOrEmpty(s))
            {
                bw.Write(Encoding.UTF8.GetBytes(s));
            }

            bw.Write((byte)0x00);
            for (int i = 0; i < padLength; i++)
            {
                bw.Write(paddingChar);
            }
        }
    }
}

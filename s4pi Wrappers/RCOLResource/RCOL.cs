/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;
using System.Reflection;

namespace RCOLResource
{
    public class RCOL : AResource
    {
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        #region Attributes
        const int recommendedApiVersion = 1;
        private RCOLHeader rcolHeader { get; set; }
        public RCOLChunk[] rcolChunkList { get; private set; }
        #endregion

        #region Constructor
        public RCOL(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.rcolHeader = new RCOLHeader(RecommendedApiVersion, OnResourceChanged, s);
            this.OnChunkListChanged += this.rcolHeader.OnChunkListChanged;
            this.rcolChunkList = new RCOLChunk[this.rcolHeader.internalCount];
            for (int i = 0; i < this.rcolHeader.internalCount; i++)
            {
                uint position = r.ReadUInt32();
                int size = r.ReadInt32();
                long tempPosition = s.Position;
                s.Position = position;
                MemoryStream ms = new MemoryStream(r.ReadBytes(size));
                BinaryReader headerReader = new BinaryReader(ms);
                uint fourcc = headerReader.ReadUInt32();
                Type rcolType = GetRCOLChunk(fourcc);
                ms.Position = 0;
                RCOLChunk chunk = (RCOLChunk)Activator.CreateInstance(rcolType, new object[] { 1, null, ms, this.rcolHeader.internalTGIList[i] });    // this part needs to be fixed
                this.OnChunkListChanged += chunk.OnRCOLListChanged;
                chunk.OnChunkVisibilityTypeUpdated+= this.UpdateChunkVisibilityType;
                this.rcolChunkList[i] = chunk;
                s.Position = tempPosition;
            }

            // update visibility
            foreach (var rcol in this.rcolChunkList) rcol.UpdateChunkVisibility();

        }

        private static Type GetRCOLChunk(uint fourcc)
        {
            if (!Enum.IsDefined(typeof(RCOLChunkType), fourcc))
            {
                return typeof(RCOLChunk);
            }
            else
            {
                foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (t.IsSubclassOf(typeof(RCOLChunk)))
                    {
                        if ((uint)(t.GetProperty("RCOLType").GetValue(null, null)) == fourcc)
                        {
                            return t;
                        }
                    }
                }
                return typeof(RCOLChunk);
            }
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            if (this.rcolChunkList == null) this.rcolChunkList = new RCOLChunk[0];
            this.rcolHeader.UnParse(this.rcolChunkList, ms);

            long indexPosition = ms.Position;

            for (int i = 0; i < this.rcolChunkList.Length; i++)
            {
                w.Write(0); // position
                w.Write(0); //length
            }

            long dataStartPosiiton = ms.Position;

            for (int i = 0; i < this.rcolChunkList.Length; i++)
            {
                long tmpPosiiton = ms.Position;
                ms.Position = indexPosition;
                w.Write(dataStartPosiiton);
                indexPosition += 4;
                ms.Position = tmpPosiiton;
                this.rcolChunkList[i].UnParse(ms);
                long size = ms.Position - tmpPosiiton;
                tmpPosiiton = ms.Position;
                dataStartPosiiton = tmpPosiiton;
                ms.Position = indexPosition;
                w.Write((int)size);
                indexPosition += 4;
                ms.Position = tmpPosiiton;
            }
            ms.Position = 0;
            return ms;
        }

        #endregion

        #region Content Fields
        public string Value { get { return ValueBuilder; } }
        #endregion

        #region Sub-Class
        public enum RCOLChunkType :uint
        {
            GEOM = 0x4d4f4547u,
            MODL = 0x4c444f4d,
            MATD = 0x4454414d,
            /// <summary>
            /// This is used only for developing.
            /// It will be removed once all the resource has been implemented
            /// </summary>
            None = 0,
            //MLOD = FOURCC("MLOD"),
            MTST = 0x5453544D,
            //TREE = FOURCC("TREE"),
            //S_SM = FOURCC("S_SM"),
            //TkMk = FOURCC("TkMk"),
            //BOND = FOURCC("BOND"),
            //LITE = FOURCC("LITE"),
            //ANIM = FOURCC("ANIM"),
            //VPXY = FOURCC("VPXY"),
            //RSLT = FOURCC("RSLT"),
            FTPT = 0x54505446U
        }

        public class RCOLListChangeEventArg : EventArgs
        {
            public IList<RCOLChunk> ParentList { get; private set; }
            public RCOLChangeType ChangeType { get; private set; }
            public RCOLChunk ChangedChunk { get; private set; }
            public int Index { get; private set; }

            public RCOLListChangeEventArg(IList<RCOLChunk> chunkList, RCOLChangeType type, RCOLChunk changedChunk, int index)
            {
                this.ParentList = chunkList;
                this.ChangeType = type;
                this.ChangedChunk = changedChunk;
                this.Index = index;
            }
        }


        public enum RCOLChangeType
        {
            Delete,
            Add
        }
        #endregion

        #region Chunk Interface
        public delegate void ChunkListChangeHandler(object sender, RCOLListChangeEventArg e);
        public event ChunkListChangeHandler OnChunkListChanged;

        public void AddChunk(RCOLChunk newChunk)
        {
            if (OnChunkListChanged == null) return;
            List<RCOLChunk> chunklist = new List<RCOLChunk>(this.rcolChunkList);
            chunklist.Add(newChunk);
            this.rcolChunkList = chunklist.ToArray();
            RCOLListChangeEventArg e = new RCOLListChangeEventArg(this.rcolChunkList, RCOLChangeType.Add, newChunk, this.rcolChunkList.Length - 1);
            OnChunkListChanged(this, e);
        }

        public void RemoveChunk(RCOLChunk chunk)
        {
            if (OnChunkListChanged == null) return;
            List<RCOLChunk> chunkList = new List<RCOLChunk>(this.rcolChunkList);
            if (!chunkList.Contains(chunk)) return;
            int index = chunkList.IndexOf(chunk);
            chunkList.RemoveAt(index);
            this.rcolChunkList = chunkList.ToArray();
            RCOLListChangeEventArg e = new RCOLListChangeEventArg(this.rcolChunkList, RCOLChangeType.Delete, chunk, index);
            OnChunkListChanged(this, e);
        }

        public void UpdateChunkVisibilityType(object sender, RCOLChunk.ChunkVisibilityTypeUpdateEventArg e)
        {
            var find = this.rcolChunkList[e.Index];
            find.visibilityType = e.Type;
        }

        #endregion
    }

    public class RCOLResourceHandler : AResourceHandler
    {
        public RCOLResourceHandler()
        {
            this.Add(typeof(RCOL), new List<string>(new string[] { "0x015A1849", "0xD382BF57", "0x01D10F34", "0x01661233", "0x01D0E75D" , "0x02019972",}));
        }
    }
}

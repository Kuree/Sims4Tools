/***************************************************************************
 *  Copyright (C) 2016 by Cmar, Peter Jones                                *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
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
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using s4pi.Interfaces;

namespace s4pi.Animation
{
    public class IkConfiguration : DependentList<SlotAssignment>
    {
        public IkConfiguration(EventHandler handler) : base(handler) { }
        public IkConfiguration(EventHandler handler, Stream s) : base(handler) { Parse(s); }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
                base.Add(new SlotAssignment(1, handler, s));
        }

        public override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(base.Count);
            foreach (var reference in this)
                reference.UnParse(s);
        }
        #endregion

        protected override SlotAssignment CreateElement(Stream s) { return new SlotAssignment(1, handler, s); }
        protected override void WriteElement(Stream s, SlotAssignment element) { element.UnParse(s); }

        public String Value
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                foreach (var item in this)
                {
                    sb.AppendLine(item.Value);
                }
                sb.AppendLine();
                return sb.ToString();
            }
        }
    }
    public class SlotAssignment : AHandlerElement, IEquatable<SlotAssignment>
    {
        public SlotAssignment(int apiVersion, EventHandler handler)
            : this(apiVersion, handler, 0, 0, "", "")
        {
        }

        public SlotAssignment(int apiVersion, EventHandler handler, Stream s)
            : this(apiVersion, handler)
        {
            Parse(s);
        }

        public SlotAssignment(int apiVersion, EventHandler handler, SlotAssignment basis)
            : this(apiVersion, handler, basis.chainId, basis.slotID, basis.targetObjectNamespace, basis.targetJointName)
        {
        }

        public SlotAssignment(int APIversion, EventHandler handler, UInt16 chain_id, UInt16 chain_sequence, string targetObject, string target)
            : base(APIversion, handler)
        {
            this.chainId = chain_id;
            this.slotID = chain_sequence;
            this.targetObjectNamespace = targetObject;
            this.targetJointName = target;
        }


        private UInt16 chainId;
        private UInt16 slotID;
        private string targetObjectNamespace;
        private string targetJointName;

        public UInt16 ChainID
        {
            get { return chainId; }
            set { if (chainId != value) { chainId = value; this.OnElementChanged(); } }
        }

        public UInt16 SlotID
        {
            get { return slotID; }
            set { if (slotID != value) { slotID = value; this.OnElementChanged(); } }
        }

        public string TargetObjectNamespace
        {
            get { return targetObjectNamespace; }
            set { if (targetObjectNamespace != value) { targetObjectNamespace = value; this.OnElementChanged(); } }
        }

        public string TargetJointName
        {
            get { return targetJointName; }
            set { if (targetJointName != value) { targetJointName = value; this.OnElementChanged(); } }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }
        public void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.chainId = br.ReadUInt16();
            this.slotID = br.ReadUInt16();
            this.targetObjectNamespace = br.ReadString32();
            this.targetJointName = br.ReadString32();

        }
        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            bw.Write(this.chainId);
            bw.Write(this.slotID);
            bw.WriteString32(this.targetObjectNamespace);
            bw.WriteString32(this.targetJointName);
        }
        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        public bool Equals(SlotAssignment other)
        {
            return base.Equals(other);
        }
    }
}

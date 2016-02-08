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
    public class ClipResource : AResource
    {
        uint version;               // currently 14
        private uint flags;
        private float duration;
        private Quaternion initialOffsetQ;
        private Vector3 initialOffsetT;
        private uint referenceNamespaceHash;
        private uint surfaceNamespaceHash;
        private uint surfaceJointNameHash;
        private uint surfacechildNamespaceHash;
        private string clip_name;
        private string rigNameSpace;
        private string[] explicitNamespaces;
        IkConfiguration slot_assignments;
        ClipEventList clip_events;
        private uint codecDataLength;
        private S3CLIP codecData;

        public string Value
        {
            get
            {
                var vb = new StringBuilder(this.ValueBuilder);
                vb.AppendFormat("\n\nIkConfiguration:{0}", this.slot_assignments.Value);
                vb.AppendFormat("ClipEvents:{0}", this.clip_events.Value);
                vb.AppendFormat("CodecData:\n{0}", this.codecData.Value);
                return vb.ToString();
            }
        }

        protected override List<string> ValueBuilderFields
        {
            get
            {
                List<string> fields = base.ValueBuilderFields;
                fields.Remove("SlotAssignments");
                fields.Remove("ClipEvents");
                fields.Remove("CodecData");
                return fields;
            }
        }

        [ElementPriority(0)]
        public UInt32 Version {
            get { return this.version; }
            set { if (this.version != value) { this.version = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(1)]
        public UInt32 Flags
        {
            get { return this.flags; }
            set { if (this.flags != value) { this.flags = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(2)]
        public float Duration
        { 
            get { return duration; } 
            set { if (this.duration != value) { duration = value; this.OnResourceChanged(this, EventArgs.Empty); } } 
        }
        [ElementPriority(3)]
        public Quaternion InitialOffsetQ
        {
            get { return this.initialOffsetQ; }
            set { if (this.initialOffsetQ != value) { initialOffsetQ = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public Vector3 InitialOffsetT
        {
            get { return this.initialOffsetT; }
            set { if (this.initialOffsetT != value) { initialOffsetT = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public UInt32 ReferenceNamespaceHash
        {
            get { return this.referenceNamespaceHash; }
            set { if (this.referenceNamespaceHash != value) { this.referenceNamespaceHash = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public UInt32 SurfaceNamespaceHash
        {
            get { return this.surfaceNamespaceHash; }
            set { if (this.surfaceNamespaceHash != value) { this.surfaceNamespaceHash = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public UInt32 SurfaceJointNameHash
        {
            get { return this.surfaceJointNameHash; }
            set { if (this.surfaceJointNameHash != value) { this.surfaceJointNameHash = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public UInt32 SurfacechildNamespaceHash
        {
            get { return this.surfacechildNamespaceHash; }
            set { if (this.surfacechildNamespaceHash != value) { this.surfacechildNamespaceHash = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public string ClipName { 
            get { return clip_name; } 
            set { if (this.clip_name != value) { clip_name = value; this.OnResourceChanged(this, EventArgs.Empty); } } 
        }
        [ElementPriority(10)]
        public string RigNameSpace
        {
            get { return rigNameSpace; }
            set { if (this.rigNameSpace != value) { rigNameSpace = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public string[] ExplicitNamespaces
        {
            get { return explicitNamespaces; }
            set { if (this.explicitNamespaces != value) { explicitNamespaces = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(12)]
        public IkConfiguration SlotAssignments
        {
            get { return this.slot_assignments; }
            set { if (this.slot_assignments != value) { this.slot_assignments = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(13)]
        public ClipEventList ClipEvents
        {
            get { return this.clip_events; }
            set { if (this.clip_events != value) { this.clip_events = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(14)]
        public S3CLIP CodecData
        {
            get { return this.codecData; }
            set { if (this.codecData != value) { this.codecData = value; OnResourceChanged(this, EventArgs.Empty); } }
        }

        public ClipResource(int apiVersion)
            : base(apiVersion,null) 
        {
            this.initialOffsetQ = new Quaternion(RecommendedApiVersion, OnResourceChanged);
            this.initialOffsetT = new Vector3(RecommendedApiVersion, OnResourceChanged);
            this.slot_assignments = new IkConfiguration(this.OnResourceChanged);
            this.clip_events = new ClipEventList(this.OnResourceChanged);
            this.codecData = new S3CLIP(RecommendedApiVersion, OnResourceChanged);
        }
        public ClipResource(int apiVersion, Stream s)
            : base(apiVersion, s)
        {
            
            if (base.stream == null)
            {
                base.stream = UnParse();
                OnResourceChanged(this, new EventArgs());
            }
            base.stream.Position = 0L;
            Parse(s);
        }
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.flags = br.ReadUInt32();
            this.duration = br.ReadSingle();
            this.initialOffsetQ = new Quaternion(RecommendedApiVersion, OnResourceChanged, s);
            this.initialOffsetT = new Vector3(RecommendedApiVersion, OnResourceChanged, s);
            if (version >= 5)
            {
                this.referenceNamespaceHash = br.ReadUInt32();
            }
            if (version >= 10)
            {
                this.surfaceNamespaceHash = br.ReadUInt32();
                this.surfaceJointNameHash = br.ReadUInt32();
            }

            if (version >= 11)
            {
                this.surfacechildNamespaceHash = br.ReadUInt32();
            }
            if (version >= 7)
            {
                this.clip_name = br.ReadString32();
            }
            this.rigNameSpace = br.ReadString32();
            if (version >= 4)
            {
                int explicitNamespaceCount = br.ReadInt32();
                explicitNamespaces = new string[explicitNamespaceCount];
                for (int i = 0; i < explicitNamespaceCount; i++)
                {
                    explicitNamespaces[i] = br.ReadString32();
                }
            }
            this.slot_assignments = new IkConfiguration(OnResourceChanged, s);
            uint clipEventLength = br.ReadUInt32();
            var events = new List<ClipEvent>();
            for (int i = 0; i < clipEventLength; i++)
            {
                ClipEventType clipType = (ClipEventType)br.ReadUInt32();
                uint size = br.ReadUInt32();
                ClipEvent evt = ClipEvent.Create(clipType, this.OnResourceChanged, size);       
                evt.Parse(s);
                events.Add(evt);
            }
            this.clip_events = new ClipEventList(this.OnResourceChanged, events);
            this.codecDataLength = br.ReadUInt32();
            if (this.codecDataLength > 0)
            {
                this.codecData = new S3CLIP(RecommendedApiVersion, OnResourceChanged, s);
            }
        }

        protected override Stream UnParse()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(this.version);
            bw.Write(this.flags);
            bw.Write(this.duration);
            this.initialOffsetQ.UnParse(ms);
            this.initialOffsetT.UnParse(ms);
            if (version >= 5)
            {
                bw.Write(this.referenceNamespaceHash);
            }
            if (version >= 10)
            {
                bw.Write(this.surfaceNamespaceHash);
                bw.Write(this.surfaceJointNameHash);
            }

            if (version >= 11)
            {
                bw.Write(this.surfacechildNamespaceHash);
            }
            if (version >= 7)
            {
                bw.WriteString32(this.clip_name);
            }
            bw.WriteString32(this.rigNameSpace);
            if (version >= 4)
            {
                if (this.explicitNamespaces == null) this.explicitNamespaces = new string[0];
                bw.Write(this.explicitNamespaces.Length);
                for (int i = 0; i < this.explicitNamespaces.Length; i++)
                {
                    bw.WriteString32(explicitNamespaces[i]);
                }
            }
            this.slot_assignments.UnParse(ms);
            bw.Write(this.clip_events.Count);
            foreach (var clip_event in clip_events)
            {
                bw.Write((uint)clip_event.TypeId);
                bw.Write(clip_event.Size);
                clip_event.UnParse(ms);
            }
            bw.Write(this.codecDataLength);
            this.codecData.UnParse(ms);
            ms.Position = 0L;
            return ms;
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public class Vector3 : AHandlerElement, IEquatable<Vector3>
        {
            float x;
            float y;
            float z;

            public Vector3(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public Vector3(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }
            public Vector3(int apiVersion, EventHandler handler, float x, float y, float z)
                : base(apiVersion, handler) 
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public void Parse(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                this.x = br.ReadSingle();
                this.y = br.ReadSingle();
                this.z = br.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(this.x);
                bw.Write(this.y);
                bw.Write(this.z);
            }

            public bool Equals(Vector3 other)
            {
                return this.x == other.x && this.y == other.y && this.z == other.z;
            }

            [ElementPriority(0)]
            public float X { get { return this.x; } set { if (this.x != value) { this.x = value; OnElementChanged(); } } }
            [ElementPriority(1)]
            public float Y { get { return this.y; } set { if (this.y != value) { this.y = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Z { get { return this.z; } set { if (this.z != value) { this.z = value; OnElementChanged(); } } }

            public string Value
            {
                get { return "X: " + this.x.ToString() + ", Y: " + this.y.ToString() + ", Z: " + this.z.ToString(); }
            }
        }

        public class Quaternion : AHandlerElement, IEquatable<Quaternion>
        {
            float x;
            float y;
            float z;
            float w;

            public Quaternion(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public Quaternion(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }
            public Quaternion(int apiVersion, EventHandler handler, float x, float y, float z, float w)
                : base(apiVersion, handler) 
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public void Parse(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                this.x = br.ReadSingle();
                this.y = br.ReadSingle();
                this.z = br.ReadSingle();
                this.w = br.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(this.x);
                bw.Write(this.y);
                bw.Write(this.z);
                bw.Write(this.w);
            }

            public bool Equals(Quaternion other)
            {
                return this.x == other.x && this.y == other.y && this.z == other.z && this.w == other.w;
            }

            [ElementPriority(0)]
            public float X { get { return this.x; } set { if (this.x != value) { this.x = value; OnElementChanged(); } } }
            [ElementPriority(1)]
            public float Y { get { return this.y; } set { if (this.y != value) { this.y = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Z { get { return this.z; } set { if (this.z != value) { this.z = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float W { get { return this.w; } set { if (this.w != value) { this.w = value; OnElementChanged(); } } }

            public string Value
            {
                get { return "X: " + this.x.ToString() + ", Y: " + this.y.ToString() + ", Z: " + this.z.ToString() + ", W: " + this.w.ToString(); }
            }
        } 
    }

}

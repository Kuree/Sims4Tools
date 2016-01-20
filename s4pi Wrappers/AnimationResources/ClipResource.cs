/***************************************************************************
 *  Copyright (C) 2009, 2015 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L Jones (pljones@users.sf.net)                                   *
 *  Keyi Zhang                                                             *
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

namespace s4pi.Animation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using s4pi.Interfaces;
    using s4pi.Settings;

    public class ClipResource : AResource
    {
        uint version;
        IKTargetList ik_targets;
        ClipEventList clip_events;
        string clip_name;

        private string actor_name;
        private string actor_list;
        private int ik_chain_count;
        private byte[] s3clip;
        private uint unknown1;
        private float duration;
        private float[] unknown_floats;
        private uint[] unknown_hashes;

        public string Value
        {
            get
            {
                var vb = new StringBuilder(this.ValueBuilder);
                vb.AppendFormat("IK Targets:{0}", this.ClipEvents.Value);
                vb.AppendFormat("Events:{0}", this.IKTargets.Value);

                return vb.ToString();
            }
        }

        protected override List<string> ValueBuilderFields
        {
            get
            {
                List<string> fields = base.ValueBuilderFields;
                fields.Remove("S3CLIP");
                fields.Remove("ClipEvents");
                fields.Remove("IKTargets");
                return fields;
            }
        }

        public float Duration
        {
            get { return this.duration; }
            set
            {
                if (Math.Abs(this.duration - value) > float.MinValue)
                {
                    this.duration = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public uint Unknown1
        {
            get { return this.unknown1; }
            set
            {
                if (this.unknown1 != value)
                {
                    this.unknown1 = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public string ClipName
        {
            get { return this.clip_name; }
            set
            {
                if (this.clip_name != value)
                {
                    this.clip_name = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public uint[] UnknownHashes
        {
            get { return this.unknown_hashes; }
            set
            {
                if (this.unknown_hashes != value)
                {
                    this.unknown_hashes = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public float[] UnknownFloats
        {
            get { return this.unknown_floats; }
            set
            {
                if (this.unknown_floats != value)
                {
                    this.unknown_floats = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public Stream S3CLIP
        {
            get { return new MemoryStream(this.s3clip); }
            set
            {
                this.s3clip = new byte[value.Length];
                value.Position = 0L;
                value.Write(this.s3clip, 0, this.s3clip.Length);
                this.OnResourceChanged(this, EventArgs.Empty);
            }
        }

        public int IKChainCount
        {
            get { return this.ik_chain_count; }
            set
            {
                if (this.ik_chain_count != value)
                {
                    this.ik_chain_count = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public string ActorList
        {
            get { return this.actor_list; }
            set
            {
                if (this.actor_list != value)
                {
                    this.actor_list = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public string ActorName
        {
            get { return this.actor_name; }
            set
            {
                if (this.actor_name != value)
                {
                    this.actor_name = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public uint Version
        {
            get { return this.version; }
            set
            {
                if (this.version != value)
                {
                    this.version = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public IKTargetList IKTargets
        {
            get { return this.ik_targets; }
            set
            {
                if (this.ik_targets != value)
                {
                    this.ik_targets = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public ClipEventList ClipEvents
        {
            get { return this.clip_events; }
            set
            {
                if (this.clip_events != value)
                {
                    this.clip_events = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public ClipResource(int apiVersion)
            : base(apiVersion, null)
        {
            this.ik_targets = new IKTargetList(this.OnResourceChanged);
            this.clip_events = new ClipEventList(this.OnResourceChanged);
        }
        public ClipResource(int apiVersion, Stream s)
            : base(apiVersion, s)
        {

            if (this.stream == null)
            {
                this.stream = this.UnParse();
                this.OnResourceChanged(this, new EventArgs());
            }
            this.stream.Position = 0L;
            this.Parse(s);
        }
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.unknown1 = br.ReadUInt32();
            this.duration = br.ReadSingle();
            this.unknown_floats = new float[8];
            for (int i = 0; i < this.unknown_floats.Length; i++)
            {
                this.unknown_floats[i] = br.ReadSingle();
            }
            this.unknown_hashes = new uint[3];

            for (int i = 0; i < this.unknown_hashes.Length; i++)
            {
                this.unknown_hashes[i] = br.ReadUInt32();
            }
            this.clip_name = br.ReadString32();
            this.actor_name = br.ReadString32();
            var extraActorCount = br.ReadInt32();
            var extraActors = new string[extraActorCount];
            for (int i = 0; i < extraActorCount; i++)
            {
                extraActors[i] = br.ReadString32();
            }
            this.actor_list = string.Join(",", extraActors);
            var ikTargetCount = br.ReadInt32();
            this.IKChainCount = br.ReadInt32();
            var ikTargets = new IKTarget[ikTargetCount];
            for (int i = 0; i < ikTargetCount; i++)
            {
                ikTargets[i] = new IKTarget(this.RecommendedApiVersion, this.OnResourceChanged, s);
            }
            this.ik_targets = new IKTargetList(this.OnResourceChanged, ikTargets);
            uint next = br.ReadUInt32();
            var events = new List<ClipEvent>();
            while (this.stream.Position + next != this.stream.Length)
            {
                var size = br.ReadUInt32();
                var evt = ClipEvent.Create(next, this.OnResourceChanged, size);

                var evtEnd = size + br.BaseStream.Position;
                evt.Parse(s);
                events.Add(evt);
                if (Settings.Checking && br.BaseStream.Position != evtEnd)
                {
                    throw new InvalidDataException();
                }

                next = br.ReadUInt32();
            }
            this.clip_events = new ClipEventList(this.OnResourceChanged, events);
            this.s3clip = new byte[next];
            s.Read(this.s3clip, 0, this.s3clip.Length);

        }

        protected override Stream UnParse()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write(this.version);
            bw.Write(this.unknown1);
            bw.Write(this.duration);
            foreach (float t in this.unknown_floats) bw.Write(t);
            foreach (var unknownHash in this.unknown_hashes) bw.Write(unknownHash);
            bw.WriteString32(this.clip_name);
            bw.WriteString32(this.actor_name);
            var actors = this.actor_list.Split(',');
            bw.Write(actors.Length);
            foreach (var actor in actors) bw.WriteString32(actor);
            bw.Write(this.ik_targets.Count);
            bw.Write(this.ik_chain_count);
            foreach (var ikTarget in this.ik_targets) ikTarget.UnParse(ms);
            foreach (var clipEvent in this.clip_events)
            {
                byte[] eventBuffer;
                using (var eventStream = new MemoryStream())
                {
                    clipEvent.UnParse(eventStream);
                    eventBuffer = new byte[eventStream.Length];
                    eventStream.Read(eventBuffer, 0, eventBuffer.Length);
                }
                bw.Write(clipEvent.TypeIdA);
                bw.Write(eventBuffer.Length);
                bw.Write(eventBuffer);
            }
            bw.Write(this.s3clip.Length);
            bw.Write(this.s3clip);
            ms.Position = 0L;
            return ms;
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public class ClipEventList : AHandlerList<ClipEvent>, IGenericAdd
        {
            public ClipEventList(EventHandler handler, IEnumerable<ClipEvent> items)
                : base(handler, items)
            {
            }
            public ClipEventList(EventHandler handler)
                : base(handler)
            {
            }

            public void Add()
            {
                throw new NotImplementedException();
            }

            public void Add(Type instanceType)
            {
                base.Add((ClipEvent)Activator.CreateInstance(instanceType, 0, this.handler));
            }

            public string Value
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

        public class IKTargetList : AHandlerList<IKTarget>, IGenericAdd
        {
            public IKTargetList(EventHandler handler)
                : base(handler)
            {
            }
            public IKTargetList(EventHandler handler, IEnumerable<IKTarget> items)
                : base(handler, items)
            {
            }

            public void Add()
            {
                this.Add(new IKTarget(0, this.handler));
            }

            public void Add(Type instanceType)
            {
                this.Add();
            }

            public string Value
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
        public class IKTarget : AHandlerElement, IEquatable<IKTarget>
        {
            public IKTarget(int apiVersion, EventHandler handler)
                : this(apiVersion, handler, 0, 0, "", "")
            {
            }

            public IKTarget(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }

            public IKTarget(int apiVersion, EventHandler handler, IKTarget basis)
                : this(apiVersion, handler, basis.chainId, basis.chainSequence, basis.actor, basis.target)
            {
            }

            public IKTarget(int APIversion, EventHandler handler, ushort chainId, ushort chainSequence, string actor, string target)
                : base(APIversion, handler)
            {
                this.chainId = chainId;
                this.chainSequence = chainSequence;
                this.actor = actor;
                this.target = target;
            }


            private ushort chainId;
            private ushort chainSequence;
            private string actor;
            private string target;

            public ushort ChainID
            {
                get { return this.chainId; }
                set
                {
                    this.chainId = value; if (this.chainId != value) { this.OnElementChanged(); }
                }
            }

            public ushort ChainSequence
            {
                get { return this.chainSequence; }
                set
                {
                    this.chainSequence = value; if (this.chainSequence != value) { this.OnElementChanged(); }
                }
            }

            public string Actor
            {
                get { return this.actor; }
                set
                {
                    this.actor = value; if (this.actor != value) { this.OnElementChanged(); }
                }
            }

            public string Target
            {
                get { return this.target; }
                set
                {
                    this.target = value; if (this.target != value) { this.OnElementChanged(); }
                }
            }

            public string Value
            {
                get { return this.ValueBuilder; }
            }

            public void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.actor = br.ReadString32();
                this.target = br.ReadString32();
                this.chainId = br.ReadUInt16();
                this.chainSequence = br.ReadUInt16();

            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.WriteString32(this.actor);
                bw.WriteString32(this.target);
                bw.Write(this.chainId);
                bw.Write(this.chainSequence);
            }

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, this.GetType()); }
            }

            public bool Equals(IKTarget other)
            {
                return base.Equals(other);
            }
        }
        public class ClipEvent3 : ClipEvent
        {
            private string soundName;

            public string SoundName
            {
                get { return this.soundName; }
                set { this.soundName = value; }
            }

            public ClipEvent3(int apiVersion, EventHandler handler)
                : base(apiVersion, handler, 3)
            {
            }

            public ClipEvent3(int apiVersion, EventHandler handler, uint typeId, Stream s)
                : base(apiVersion, handler, typeId, s)
            {
            }

            public ClipEvent3(int apiVersion, EventHandler handler, ClipEvent3 basis)
                : base(apiVersion, handler, basis)
            {
                this.soundName = basis.soundName;
            }

            protected override void ReadTypeData(Stream ms)
            {
                var br = new BinaryReader(ms);
                this.soundName = br.ReadStringFixed();
            }

            protected override void WriteTypeData(Stream ms)
            {
                var bw = new BinaryWriter(ms);
                bw.WriteStringFixed(this.soundName);
            }
        }

        public class ClipEvent5 : ClipEvent
        {
            private string slot_name;
            private string effect_name;
            private uint actor_name_hash;
            private uint slot_name_hash;
            private byte[] unknown;

            public byte[] UnknownBytes
            {
                get { return this.unknown; }
                set { this.unknown = value; }
            }

            public uint SlotNameHash
            {
                get { return this.slot_name_hash; }
                set { this.slot_name_hash = value; }
            }

            public uint ActorNameHash
            {
                get { return this.actor_name_hash; }
                set { this.actor_name_hash = value; }
            }

            public string EffectName
            {
                get { return this.effect_name; }
                set { this.effect_name = value; }
            }

            public string SlotName
            {
                get { return this.slot_name; }
                set { this.slot_name = value; }
            }

            public ClipEvent5(int apiVersion, EventHandler handler)
                : base(apiVersion, handler, 5)
            {
            }

            public ClipEvent5(int apiVersion, EventHandler handler, uint typeId, Stream s)
                : base(apiVersion, handler, typeId, s)
            {
            }

            public ClipEvent5(int apiVersion, EventHandler handler, ClipEvent5 basis)
                : base(apiVersion, handler, basis)
            {
                this.unknown = basis.unknown;
                this.slot_name_hash = basis.slot_name_hash;
                this.actor_name_hash = basis.actor_name_hash;
                this.effect_name = basis.effect_name;
                this.slot_name = basis.slot_name;
            }

            protected override void ReadTypeData(Stream ms)
            {
                var br = new BinaryReader(ms);
                this.slot_name = br.ReadStringFixed();
                this.actor_name_hash = br.ReadUInt32();
                this.slot_name_hash = br.ReadUInt32();
                this.unknown = br.ReadBytes(16);
                this.effect_name = br.ReadStringFixed();

            }

            protected override void WriteTypeData(Stream ms)
            {
                var bw = new BinaryWriter(ms);
                bw.WriteStringFixed(this.slot_name);
                bw.Write(this.actor_name_hash);
                bw.Write(this.slot_name_hash);
                bw.Write(this.unknown);
                bw.WriteStringFixed(this.effect_name);
            }
        }

        public class ClipEvent14 : ClipEvent
        {
            private string unknown_3;
            private uint actor_name;
            private uint slot_name;

            public uint SlotNameHash
            {
                get { return this.slot_name; }
                set { this.slot_name = value; }
            }

            public uint ActorNameHash
            {
                get { return this.actor_name; }
                set { this.actor_name = value; }
            }

            public string Unknown3
            {
                get { return this.unknown_3; }
                set { this.unknown_3 = value; }
            }

            public ClipEvent14(int apiVersion, EventHandler handler)
                : base(apiVersion, handler, 14)
            {
            }

            public ClipEvent14(int apiVersion, EventHandler handler, uint typeId, Stream s)
                : base(apiVersion, handler, typeId, s)
            {
            }

            public ClipEvent14(int apiVersion, EventHandler handler, ClipEvent14 basis)
                : base(apiVersion, handler, basis)
            {
                this.slot_name = basis.slot_name;
                this.actor_name = basis.actor_name;
                this.unknown_3 = basis.unknown_3;
            }

            protected override void ReadTypeData(Stream ms)
            {
                var br = new BinaryReader(ms);
                this.unknown_3 = br.ReadStringFixed();
                this.actor_name = br.ReadUInt32();
                this.slot_name = br.ReadUInt32();
            }

            protected override void WriteTypeData(Stream ms)
            {
                var bw = new BinaryWriter(ms);
                bw.Write(this.unknown_3);

            }
        }
        public class ClipEvent19 : ClipEvent
        {
            private float unknown_3;

            public float Unknown3
            {
                get { return this.unknown_3; }
                set { this.unknown_3 = value; }
            }

            public ClipEvent19(int apiVersion, EventHandler handler)
                : base(apiVersion, handler, 19)
            {
            }

            public ClipEvent19(int apiVersion, EventHandler handler, uint typeId, Stream s)
                : base(apiVersion, handler, typeId, s)
            {
            }

            public ClipEvent19(int apiVersion, EventHandler handler, ClipEvent19 basis)
                : base(apiVersion, handler, basis)
            {
                this.unknown_3 = basis.unknown_3;
            }

            protected override void ReadTypeData(Stream ms)
            {
                var br = new BinaryReader(ms);
                this.unknown_3 = br.ReadSingle();
            }

            protected override void WriteTypeData(Stream ms)
            {

                var bw = new BinaryWriter(ms);
                bw.Write(this.unknown_3);
            }
        }
        public class ClipEventUnknown : ClipEvent
        {
            private byte[] data;

            public byte[] Data
            {
                get { return this.data; }
                set { this.data = value; }
            }

            public ClipEventUnknown(int apiVersion, EventHandler handler)
                : this(apiVersion, handler, 0, 0)
            {
            }
            public ClipEventUnknown(int apiVersion, EventHandler handler, uint typeId, uint size)
                : base(apiVersion, handler, typeId)
            {
                this.data = new byte[size - 4 * 3];
            }


            protected override void ReadTypeData(Stream ms)
            {
                ms.Read(this.data, 0, this.data.Length);
            }
            protected override void WriteTypeData(Stream ms)
            {
                ms.Write(this.data, 0, this.data.Length);
            }
        }
        public abstract class ClipEvent : AHandlerElement, IEquatable<ClipEvent>
        {
            protected ClipEvent(int apiVersion, EventHandler handler, uint typeId)
                : this(apiVersion, handler, typeId, 0, 0, 0)
            {
            }

            protected ClipEvent(int apiVersion, EventHandler handler, uint typeId, Stream s)
                : this(apiVersion, handler, typeId)
            {
                this.Parse(s);
            }

            protected ClipEvent(int apiVersion, EventHandler handler, ClipEvent basis)
                : this(apiVersion, handler, basis.typeId, basis.unknown1, basis.unknown2, basis.timecode)
            {
            }

            protected ClipEvent(int APIversion, EventHandler handler, uint type, uint unknown1, uint unknown2, float timecode)
                : base(APIversion, handler)
            {
                this.typeId = type;
                this.unknown1 = unknown1;
            }
            protected uint typeId;
            private uint unknown1;
            private uint unknown2;
            private float timecode;

            public string Value
            {
                get { return this.ValueBuilder; }
            }
            internal uint TypeIdA
            {
                get { return this.typeId; }
            }
            public uint Unknown1
            {
                get { return this.unknown1; }
                set { this.unknown1 = value; }
            }

            public uint Unknown2
            {
                get { return this.unknown2; }
                set { this.unknown2 = value; }
            }

            public float Timecode
            {
                get { return this.timecode; }
                set { this.timecode = value; }
            }


            public void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.unknown1 = br.ReadUInt32();
                this.unknown2 = br.ReadUInt32();
                this.timecode = br.ReadSingle();
                this.ReadTypeData(s);
            }
            protected abstract void ReadTypeData(Stream ms);

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.unknown1);
                bw.Write(this.unknown2);
                bw.Write(this.timecode);
                this.WriteTypeData(s);

            }
            protected abstract void WriteTypeData(Stream ms);
            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
            }

            public bool Equals(ClipEvent other)
            {
                return base.Equals(other);
            }
            public static ClipEvent Create(uint typeId, EventHandler handler, uint size)
            {
                switch (typeId)
                {
                    case 3:
                        return new ClipEvent3(0, handler);
                    case 5:
                        return new ClipEvent5(0, handler);
                    case 14:
                        return new ClipEvent14(0, handler);
                    case 19:
                        return new ClipEvent19(0, handler);
                    default:
                        return new ClipEventUnknown(0, handler, typeId, size);
                }
            }

        }
    }

}

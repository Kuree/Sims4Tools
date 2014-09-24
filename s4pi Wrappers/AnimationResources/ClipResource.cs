/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
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
using s4pi.Animation.S3CLIP;
using s4pi.Interfaces;

namespace s4pi.Animation
{
    public class ClipResource : AResource
    {
        uint version;
        IKTargetList ik_targets;
        ClipEventList clip_events;
        string clip_name;

        private string actor_name;
        private String actor_list;
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
            get { return duration; }
            set
            {
                if (this.duration != value)
                {
                    duration = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public uint Unknown1
        {
            get { return unknown1; }
            set 
            {
                if (this.unknown1 != value)
                {
                    unknown1 = value; this.OnResourceChanged(this,EventArgs.Empty);
                }
            }
        }

        public string ClipName
        {
            get { return clip_name; }
            set
            {
                if (this.clip_name != value)
                {
                    clip_name = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public uint[] UnknownHashes
        {
            get { return unknown_hashes; }
            set
            {
                if (this.unknown_hashes != value)
                {
                    unknown_hashes = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public float[] UnknownFloats
        {
            get { return unknown_floats; }
            set
            {
                if (this.unknown_floats != value)
                {
                    unknown_floats = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public Stream S3CLIP
        {
            get { return new MemoryStream(this.s3clip); }
            set
            {
                s3clip = new byte[value.Length];
                value.Position = 0L;
                value.Write(this.s3clip,0,this.s3clip.Length);
                OnResourceChanged(this,EventArgs.Empty);
            }
        }

        public int IKChainCount
        {
            get { return ik_chain_count; }
            set
            {
                if (this.ik_chain_count != value)
                {
                    ik_chain_count = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        
        public String ActorList
        {
            get { return actor_list; }
            set
            {
                if (this.actor_list != value)
                {
                    actor_list = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public string ActorName
        {
            get { return actor_name; }
            set
            {
                if (this.actor_name != value)
                {
                    actor_name = value; this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public UInt32 Version {
            get { return this.version; }
            set { if (this.version != value) { this.version = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        public IKTargetList IKTargets
        {
            get { return this.ik_targets; }
            set { if (this.ik_targets != value) { this.ik_targets = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        public ClipEventList ClipEvents
        {
            get { return this.clip_events; }
            set { if (this.clip_events != value) { this.clip_events = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        public ClipResource(int apiVersion)
            : base(apiVersion,null) 
        {
            this.ik_targets = new IKTargetList(this.OnResourceChanged);
            this.clip_events = new ClipEventList(this.OnResourceChanged);
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
            var extra_actor_count = br.ReadInt32();
            var extra_actors = new string[extra_actor_count];
            for (int i = 0; i < extra_actor_count; i++)
            {
                extra_actors[i] = br.ReadString32();
            }
            this.actor_list = string.Join(",", extra_actors);
            var ik_target_count = br.ReadInt32();
            this.IKChainCount = br.ReadInt32();
            var ik_targets = new IKTarget[ik_target_count];
            for (int i = 0; i < ik_target_count; i++)
            {
                ik_targets[i] = new IKTarget(RecommendedApiVersion, this.OnResourceChanged,s);
            }
            this.ik_targets = new IKTargetList(this.OnResourceChanged, ik_targets);
            UInt32 next = br.ReadUInt32();
            bool end = false;
            var events = new List<ClipEvent>();
            while (stream.Position+ next != stream.Length)
            {
                var size = br.ReadUInt32();
                var evt = ClipEvent.Create(next,this.OnResourceChanged,size);       
             
                var evt_end =size+ br.BaseStream.Position ;
                evt.Parse(s);
                events.Add(evt);
                if (Settings.Settings.Checking && br.BaseStream.Position != evt_end)
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
            foreach (float t in unknown_floats)bw.Write(t);
            foreach (var unknown_hash in this.unknown_hashes) bw.Write(unknown_hash);
            bw.WriteString32(this.clip_name);
            bw.WriteString32(this.actor_name);
            var actors = this.actor_list.Split(',');
            bw.Write(actors.Length);
            foreach(var actor in actors) bw.WriteString32(actor);
            bw.Write(this.ik_targets.Count);
            bw.Write(this.ik_chain_count);
            foreach (var ik_target in this.ik_targets)ik_target.UnParse(ms);
            foreach (var clip_event in clip_events)
            {
                byte[] event_buffer;
                using (var event_stream = new MemoryStream())
                {
                    clip_event.UnParse(event_stream);
                    event_buffer = new byte[event_stream.Length];
                    event_stream.Read(event_buffer, 0, event_buffer.Length);
                }
                bw.Write(clip_event.TypeIdA);
                bw.Write(event_buffer.Length);
                bw.Write(event_buffer);
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




        public class ClipEventList : AHandlerList<ClipEvent>,IGenericAdd
        {
            public ClipEventList(EventHandler handler, IEnumerable<ClipEvent> items)
                : base(handler,items)
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
                base.Add((ClipEvent)Activator.CreateInstance(instanceType, 0, this.handler)) ;
            }

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

        public class IKTargetList : AHandlerList<IKTarget>, IGenericAdd
        {
            public IKTargetList(EventHandler handler)
                : base(handler)
            {
            }
            public IKTargetList(EventHandler handler,IEnumerable<IKTarget> items)
                : base(handler,items)
            {
            }

            public void Add()
            {
                this.Add(new IKTarget(0,handler));
            }

            public void Add(Type instanceType)
            {
                this.Add();
            }

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
        public class IKTarget : AHandlerElement, IEquatable<IKTarget>
        {
            public IKTarget(int apiVersion, EventHandler handler)
                : this(apiVersion, handler, 0, 0, "", "")
            {
            }

            public IKTarget(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                Parse(s);
            }

            public IKTarget(int apiVersion, EventHandler handler, IKTarget basis)
                : this(apiVersion, handler, basis.chainId, basis.chainSequence, basis.actor, basis.target)
            {
            }

            public IKTarget(int APIversion, EventHandler handler, UInt16 chain_id, UInt16 chain_sequence, string actor, string target)
                : base(APIversion, handler)
            {
                this.chainId = chain_id;
                this.chainSequence = chain_sequence;
                this.actor = actor;
                this.target = target;
            }


            private UInt16 chainId;
            private UInt16 chainSequence;
            private string actor;
            private string target;

            public UInt16 ChainID
            {
                get { return chainId; }
                set { chainId = value; if (chainId != value) { this.OnElementChanged(); } }
            }

            public UInt16 ChainSequence
            {
                get { return chainSequence; }
                set { chainSequence = value; if (chainSequence != value) { this.OnElementChanged(); } }
            }

            public string Actor
            {
                get { return actor; }
                set { actor = value; if (actor != value) { this.OnElementChanged(); } }
            }

            public string Target
            {
                get { return target; }
                set { target = value; if (target != value) { this.OnElementChanged(); } }
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
                get { return GetContentFields(0, GetType()); }
            }

            public bool Equals(IKTarget other)
            {
                return base.Equals(other);
            }
        }
        public class ClipEvent3 : ClipEvent
        {
            private string sound_name;

            public string SoundName
            {
                get { return sound_name; }
                set { sound_name = value; }
            }
            
            public ClipEvent3(int apiVersion, EventHandler handler)
                : base(apiVersion, handler, 3)
            {
            }

            public ClipEvent3(int apiVersion, EventHandler handler, uint typeId, Stream s)
                : base(apiVersion, handler, typeId,s)
            {
            }

            public ClipEvent3(int apiVersion, EventHandler handler, ClipEvent3 basis)
                : base(apiVersion, handler, basis)
            {
                this.sound_name = basis.sound_name;
            }

            protected override void ReadTypeData(Stream ms)
            {
                var br = new BinaryReader(ms);
                this.sound_name = br.ReadStringFixed();
            }

            protected override void WriteTypeData(Stream ms)
            {
                var bw = new BinaryWriter(ms);
                bw.WriteStringFixed(this.sound_name);
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
                get { return unknown; }
                set { unknown = value; }
            }

            public uint SlotNameHash
            {
                get { return slot_name_hash; }
                set { slot_name_hash = value; }
            }

            public uint ActorNameHash
            {
                get { return actor_name_hash; }
                set { actor_name_hash = value; }
            }

            public string EffectName
            {
                get { return effect_name; }
                set { effect_name = value; }
            }

            public string SlotName
            {
                get { return slot_name; }
                set { slot_name = value; }
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
                get { return slot_name; }
                set { slot_name = value; }
            }

            public uint ActorNameHash
            {
                get { return actor_name; }
                set { actor_name = value; }
            }

            public string Unknown3
            {
                get { return unknown_3; }
                set { unknown_3 = value; }
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
                get { return unknown_3; }
                set { unknown_3 = value; }
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
        public class ClipEventUnknown : ClipEvent {
            private byte[] data;

            public byte[] Data
            {
                get { return data; }
                set { data = value; }
            }

            public ClipEventUnknown(int apiVersion, EventHandler handler)
                : this(apiVersion, handler,0,0)
            {
            }
            public ClipEventUnknown(int apiVersion, EventHandler handler, uint typeId,uint size)
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
                Parse(s);
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
                get { return ValueBuilder; }
            }
            internal uint TypeIdA
            {
                get { return typeId; }
            }
            public uint Unknown1
            {
                get { return unknown1; }
                set { unknown1 = value; }
            }

            public uint Unknown2
            {
                get { return unknown2; }
                set { unknown2 = value; }
            }

            public float Timecode
            {
                get { return timecode; }
                set { timecode = value; }
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
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public bool Equals(ClipEvent other)
            {
                return base.Equals(other);
            }
            public static ClipEvent Create(uint typeId,EventHandler handler,uint size) {
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
                        return new ClipEventUnknown(0,handler,typeId,size);
                }
            }

        }
    }

}

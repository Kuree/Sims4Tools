/***************************************************************************
 *  Copyright (C) 2016 by Cmar, Peter Jones                                *
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
using System.Text;
using System.Linq;
using s4pi.Interfaces;

namespace s4pi.Animation
{
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

    public class ClipEventSound : ClipEvent
    {
        private string sound_name;

        public string SoundName
        {
            get { return sound_name; }
            set { if (this.sound_name != value) { this.sound_name = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return IOExt.FixedStringLength; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventSound)
            {
                ClipEventSound f = other as ClipEventSound;
                return String.Compare(this.sound_name, f.sound_name) == 0;
            }
            else
            {
                return false;
            }
        }

        public ClipEventSound(int apiVersion, EventHandler handler)
            : base(apiVersion, handler, ClipEventType.SOUND)
        {
        }

        public ClipEventSound(int apiVersion, EventHandler handler, ClipEventType typeId, Stream s)
            : base(apiVersion, handler, typeId, s)
        {
        }

        public ClipEventSound(int apiVersion, EventHandler handler, ClipEventSound basis)
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

    public class ClipEventScript : ClipEvent
    {
        private byte[] data;

        public byte[] Data
        {
            get { return data; }
            set { if (this.data != value) { this.data = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return (uint)this.data.Length; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventScript)
            {
                ClipEventScript f = other as ClipEventScript;
                return Enumerable.SequenceEqual(this.data, f.data);
            }
            else
            {
                return false;
            }
        }

        public ClipEventScript(int apiVersion, EventHandler handler)
            : this(apiVersion, handler, 0, 12)
        {
        }
        public ClipEventScript(int apiVersion, EventHandler handler, ClipEventType typeId, uint size)
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

    public class ClipEventEffect : ClipEvent
    {
        private string slot_name;
        private uint actor_name_hash;
        private uint slot_name_hash;
        private byte[] unknown;
        private string effect_name;

        [ElementPriority(0)]
        public string SlotName
        {
            get { return slot_name; }
            set { if (this.slot_name != value) { this.slot_name = value; OnElementChanged(); } }
        }
        [ElementPriority(1)]
        public uint ActorNameHash
        {
            get { return actor_name_hash; }
            set { if (this.actor_name_hash != value) { this.actor_name_hash = value; OnElementChanged(); } }
        }
        [ElementPriority(2)]
        public uint SlotNameHash
        {
            get { return slot_name_hash; }
            set { if (this.slot_name_hash != value) { this.slot_name_hash = value; OnElementChanged(); } }
        }
        [ElementPriority(3)]
        public byte[] UnknownBytes
        {
            get { return unknown; }
            set { if (this.unknown != value) { this.unknown = value; OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public string EffectName
        {
            get { return effect_name; }
            set { if (this.effect_name != value) { this.effect_name = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return (2 * IOExt.FixedStringLength) + 24; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventEffect)
            {
                ClipEventEffect f = other as ClipEventEffect;
                return (String.Compare(this.slot_name, f.slot_name) == 0 && String.Compare(this.effect_name, f.effect_name) == 0 &&
                    this.actor_name_hash == f.actor_name_hash && this.slot_name_hash == f.slot_name_hash &&
                    Enumerable.SequenceEqual(this.unknown, f.unknown));
            }
            else
            {
                return false;
            }
        }

        public ClipEventEffect(int apiVersion, EventHandler handler)
            : base(apiVersion, handler, ClipEventType.EFFECT)
        {
        }

        public ClipEventEffect(int apiVersion, EventHandler handler, ClipEventType typeId, Stream s)
            : base(apiVersion, handler, typeId, s)
        {
        }

        public ClipEventEffect(int apiVersion, EventHandler handler, ClipEventEffect basis)
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

    public class ClipEventSNAP : ClipEvent
    {
        private byte[] data;

        public byte[] Data
        {
            get { return data; }
            set { if (this.data != value) { this.data = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return (uint)this.data.Length; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventSNAP)
            {
                ClipEventSNAP f = other as ClipEventSNAP;
                return Enumerable.SequenceEqual(this.data, f.data);
            }
            else
            {
                return false;
            }
        }

        public ClipEventSNAP(int apiVersion, EventHandler handler)
            : this(apiVersion, handler, 0, 12)
        {
        }
        public ClipEventSNAP(int apiVersion, EventHandler handler, ClipEventType typeId, uint size)
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

    public class ClipEventDoubleModifierSound : ClipEvent
    {
        private string unknown_3;
        private uint actor_name;
        private uint slot_name;

        [ElementPriority(0)]
        public string Unknown3
        {
            get { return unknown_3; }
            set { if (this.unknown_3 != value) { this.unknown_3 = value; OnElementChanged(); } }
        }
        [ElementPriority(1)]
        public uint ActorNameHash
        {
            get { return actor_name; }
            set { if (this.actor_name != value) { this.actor_name = value; OnElementChanged(); } }
        }
        [ElementPriority(2)]
        public uint SlotNameHash
        {
            get { return slot_name; }
            set { if (this.slot_name != value) { this.slot_name = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return IOExt.FixedStringLength + 8; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventDoubleModifierSound)
            {
                ClipEventDoubleModifierSound f = other as ClipEventDoubleModifierSound;
                return (String.Compare(this.unknown_3, f.unknown_3) == 0 &&
                    this.actor_name == f.actor_name && this.slot_name == f.slot_name);
            }
            else
            {
                return false;
            }
        }

        public ClipEventDoubleModifierSound(int apiVersion, EventHandler handler)
            : base(apiVersion, handler, (ClipEventType)14)
        {
        }

        public ClipEventDoubleModifierSound(int apiVersion, EventHandler handler, ClipEventType typeId, Stream s)
            : base(apiVersion, handler, typeId, s)
        {
        }

        public ClipEventDoubleModifierSound(int apiVersion, EventHandler handler, ClipEventDoubleModifierSound basis)
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
            bw.WriteStringFixed(this.unknown_3);
            bw.Write(this.actor_name);
            bw.Write(this.slot_name);
        }
    }

    public class ClipEventCensor : ClipEvent
    {
        private float unknown_3;

        public float Unknown3
        {
            get { return unknown_3; }
            set { if (this.unknown_3 != value) { this.unknown_3 = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return 4; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventCensor)
            {
                ClipEventCensor f = other as ClipEventCensor;
                return this.unknown_3 == f.unknown_3;
            }
            else
            {
                return false;
            }
        }

        public ClipEventCensor(int apiVersion, EventHandler handler)
            : base(apiVersion, handler, (ClipEventType)19)
        {
        }

        public ClipEventCensor(int apiVersion, EventHandler handler, ClipEventType typeId, Stream s)
            : base(apiVersion, handler, typeId, s)
        {
        }

        public ClipEventCensor(int apiVersion, EventHandler handler, ClipEventCensor basis)
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
            get { return data; }
            set { if (this.data != value) { this.data = value; OnElementChanged(); } }
        }

        protected override uint typeSize
        {
            get { return (uint)this.data.Length; }
        }

        protected override bool isEqual(ClipEvent other)
        {
            if (other is ClipEventUnknown)
            {
                ClipEventUnknown f = other as ClipEventUnknown;
                return Enumerable.SequenceEqual(this.data, f.data);
            }
            else
            {
                return false;
            }
        }

        public ClipEventUnknown(int apiVersion, EventHandler handler)
            : this(apiVersion, handler, 0, 12)
        {
        }
        public ClipEventUnknown(int apiVersion, EventHandler handler, ClipEventType typeId, uint size)
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
        protected ClipEvent(int apiVersion, EventHandler handler, ClipEventType typeId)
            : this(apiVersion, handler, typeId, 0, 0, 0)
        {
        }

        protected ClipEvent(int apiVersion, EventHandler handler, ClipEventType typeId, Stream s)
            : this(apiVersion, handler, typeId)
        {
            Parse(s);
        }

        protected ClipEvent(int apiVersion, EventHandler handler, ClipEvent basis)
            : this(apiVersion, handler, basis.typeId, basis.unknown1, basis.unknown2, basis.timecode)
        {
        }

        protected ClipEvent(int APIversion, EventHandler handler, ClipEventType type, uint unknown1, uint unknown2, float timecode)
            : base(APIversion, handler)
        {
            this.typeId = type;
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.timecode = timecode;
        }

        protected ClipEventType typeId;
        private uint unknown1;
        private uint unknown2;
        private float timecode;

        public string Value
        {
            get { return ValueBuilder; }
        }
        public ClipEventType TypeId
        {
            get { return typeId; }
        }

        [ElementPriority(0)]
        public uint Unknown1
        {
            get { return unknown1; }
            set { if (this.unknown1 != value) { this.unknown1 = value; OnElementChanged(); } }
        }
        [ElementPriority(1)]
        public uint Unknown2
        {
            get { return unknown2; }
            set { if (this.unknown2 != value) { this.unknown2 = value; OnElementChanged(); } }
        }
        [ElementPriority(2)]
        public float Timecode
        {
            get { return timecode; }
            set { if (this.timecode != value) { this.timecode = value; OnElementChanged(); } }
        }

        protected abstract uint typeSize { get; }
        internal uint Size
        {
            get { return this.typeSize + 12; }
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
            if (this.GetType() != other.GetType() || this.unknown1 != other.unknown1 || this.unknown2 != other.unknown2) return false;
            return this.isEqual(other);
        }
        protected abstract bool isEqual(ClipEvent other);

        public static ClipEvent Create(ClipEventType typeId, EventHandler handler, uint size)
        {
            switch ((uint)typeId)
            {
                case 3:
                    return new ClipEventSound(0, handler);
                case 4:
                    return new ClipEventScript(0, handler, typeId, size);
                case 5:
                    return new ClipEventEffect(0, handler);
                case 12:
                    return new ClipEventSNAP(0, handler, typeId, size);
                case 14:
                    return new ClipEventDoubleModifierSound(0, handler);
                case 19:
                    return new ClipEventCensor(0, handler);
                default:
                    return new ClipEventUnknown(0, handler, typeId, size);
            }
        }
    }
    public enum ClipEventType : uint
    {
        INVALID = 0,
        PARENT,
        UNPARENT,
        SOUND,
        SCRIPT,
        EFFECT,
        VISIBILITY,
        DEPRECATED_6,
        CREATE_PROP,
        DESTROY_PROP,
        STOP_EFFECT,
        BLOCK_TRANSITION,
        SNAP,
        REACTION,
        DOUBLE_MODIFIER_SOUND,
        DSP_INTERVAL,
        MATERIAL_STATE,
        FOCUS_COMPATIBILITY,
        SUPPRESS_LIP_SYNC,
        CENSOR,
        SIMULATION_SOUND_START,
        SIMULATION_SOUND_STOP,
        ENABLE_FACIAL_OVERLAY,
        FADE_OBJECT,
        DISABLE_OBJECT_HIGHLIGHT,
        THIGH_TARGET_OFFSET
    }
}

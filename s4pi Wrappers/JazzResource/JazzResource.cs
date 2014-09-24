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
using s4pi.Interfaces;
using s4pi.GenericRCOLResource;

namespace s4pi.GenericRCOLResource
{
    public abstract class JazzChunk : ARCOLBlock
    {
        protected static bool checking = s4pi.Settings.Settings.Checking;
        protected static int recommendedApiVersion = 1;

        protected static class DEADBEEF
        {
            public static void Parse(Stream s)
            {
                uint filler = new BinaryReader(s).ReadUInt32();
                if (checking) if (filler != 0xDEADBEEF)
                        throw new InvalidDataException(String.Format("Invalid filler read: 0x{0:X8}; expected: 0x{1:X8}; at 0x{2:X8}", filler, 0xDEADBEEF, s.Position));
            }
            public static void UnParse(Stream s) { new BinaryWriter(s).Write(0xDEADBEEF); }
        }

        protected static class CloseDGN
        {
            static UInt32 closeDGN = (uint)FOURCC("/DGN");
            public static void Parse(Stream s)
            {
                uint filler = new BinaryReader(s).ReadUInt32();
                if (checking) if (filler != closeDGN)
                        throw new InvalidDataException(String.Format("Invalid filler read: 0x{0:X8}; expected: 0x{1:X8}; at 0x{2:X8}", filler, closeDGN, s.Position));
            }
            public static void UnParse(Stream s) { new BinaryWriter(s).Write(closeDGN); }
        }

        protected static void ExpectZero(Stream s, int size = sizeof(uint))
        {
            while (s.Position % size != 0)
            {
                int b = s.ReadByte();
                if (b == -1) break;
                if (b != 0)
                    throw new InvalidDataException(String.Format("Invalid padding read: 0x{0:X2}; expected: 0x00; at 0x{1:X8}", b, s.Position));
            }
        }
        protected static void PadZero(Stream s, int size = sizeof(uint)) { while (s.Position % size != 0) s.WriteByte(0); }

        public enum AnimationPriority : uint
        {
            Default = 0xfffffffe,//-2,
            Broadcast = 0xffffffff,//-1,
            Unset = 0,
            Low = 6000,
            LowPlus = 8000,
            Normal = 10000,
            NormalPlus = 15000,
            FacialIdle = 17500,
            High = 20000,
            HighPlus = 25000,
            CarryRight = 30000,
            CarryRightPlus = 35000,
            CarryLeft = 40000,
            CarryLeftPlus = 45000,
            Ultra = 50000,
            UltraPlus = 55000,
            LookAt = 60000,
        }

        public enum AwarenessLevel
        {
            ThoughtBubble,
            OverlayFace,
            OverlayHead,
            OverlayBothArms,
            OverlayUpperbody,
            OverlayNone,
            Unset
        }

        public class ChunkReferenceList : DependentList<GenericRCOLResource.ChunkReference>
        {
            #region Constructors
            public ChunkReferenceList(EventHandler handler) : base(handler) { }
            public ChunkReferenceList(EventHandler handler, IEnumerable<GenericRCOLResource.ChunkReference> ilt) : base(handler, ilt) { }
            public ChunkReferenceList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region DependentList<Animation>
            protected override GenericRCOLResource.ChunkReference CreateElement(Stream s) { return new GenericRCOLResource.ChunkReference(0, handler, s); }
            protected override void WriteElement(Stream s, GenericRCOLResource.ChunkReference element) { element.UnParse(s); }
            #endregion
        }

        #region Constructors
        public JazzChunk(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzChunk(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        #endregion

        protected override void Parse(Stream s)
        {
            uint tag = new BinaryReader(s).ReadUInt32();
            if (checking) if (tag != (uint)FOURCC(Tag))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{2:X8}", FOURCC(tag), Tag, s.Position));
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            new BinaryWriter(ms).Write((uint)FOURCC(Tag));
            return ms;
        }
    }

    #region Definition Chunks
    public class JazzStateMachine : JazzChunk
    {
        const string TAG = "S_SM";

        [Flags]
        public enum Flags : uint
        {
            Default = 0x01,
            UnilateralActor = 0x01,
            PinAllResources = 0x02,
            BlendMotionAccumulation = 0x04,
            HoldAllPoses = 0x08
        }

        #region Attributes
        uint version = 0x0202;
        uint nameHash;
        ChunkReferenceList actorDefinitionIndexes;
        ChunkReferenceList propertyDefinitionIndexes;
        ChunkReferenceList stateIndexes;
        AnimationList animations;
        //0xDEADBEEF
        Flags properties;
        AnimationPriority automationPriority;
        AwarenessLevel awarenessOverlayLevel;
        uint unknown2;
        uint unknown3;
        uint unknown4;
        uint unknown5;
        #endregion

        #region Constructors
        public JazzStateMachine(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzStateMachine(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzStateMachine(int APIversion, EventHandler handler, JazzStateMachine basis)
            : this(APIversion, handler
            , basis.version, basis.nameHash, basis.actorDefinitionIndexes, basis.propertyDefinitionIndexes, basis.stateIndexes, basis.animations
            , basis.properties, basis.automationPriority, basis.awarenessOverlayLevel, basis.unknown2, basis.unknown3, basis.unknown4, basis.unknown5
            )
        { }
        public JazzStateMachine(int APIversion, EventHandler handler
            , uint version
            , uint nameHash
            , IEnumerable<GenericRCOLResource.ChunkReference> actorDefinitionIndexes
            , IEnumerable<GenericRCOLResource.ChunkReference> propertyDefinitionIndexes
            , IEnumerable<GenericRCOLResource.ChunkReference> stateIndexes
            , IEnumerable<Animation> animations
            , Flags properties
            , AnimationPriority automationPriority
            , AwarenessLevel awarenessOverlayLevel
            , uint unknown2
            , uint unknown3
            , uint unknown4
            , uint unknown5
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.nameHash = nameHash;
            this.actorDefinitionIndexes = new ChunkReferenceList(handler, actorDefinitionIndexes);
            this.propertyDefinitionIndexes = new ChunkReferenceList(handler, propertyDefinitionIndexes);
            this.stateIndexes = new ChunkReferenceList(handler, stateIndexes);
            this.animations = new AnimationList(handler, animations);
            this.properties = properties;
            this.automationPriority = automationPriority;
            this.awarenessOverlayLevel = awarenessOverlayLevel;
            this.unknown2 = unknown2;
            this.unknown3 = unknown3;
            this.unknown4 = unknown4;
            this.unknown5 = unknown5;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02D5DF13; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.nameHash = r.ReadUInt32();
            this.actorDefinitionIndexes = new ChunkReferenceList(handler, s);
            this.propertyDefinitionIndexes = new ChunkReferenceList(handler, s);
            this.stateIndexes = new ChunkReferenceList(handler, s);
            this.animations = new AnimationList(handler, s);
            DEADBEEF.Parse(s);
            this.properties = (Flags)r.ReadUInt32();
            this.automationPriority = (AnimationPriority)r.ReadUInt32();
            this.awarenessOverlayLevel = (AwarenessLevel)r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadUInt32();
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write(nameHash);
            if (actorDefinitionIndexes == null) actorDefinitionIndexes = new ChunkReferenceList(handler);
            actorDefinitionIndexes.UnParse(ms);
            if (propertyDefinitionIndexes == null) propertyDefinitionIndexes = new ChunkReferenceList(handler);
            propertyDefinitionIndexes.UnParse(ms);
            if (stateIndexes == null) stateIndexes = new ChunkReferenceList(handler);
            stateIndexes.UnParse(ms);
            if (animations == null) animations = new AnimationList(handler);
            animations.UnParse(ms);
            DEADBEEF.UnParse(ms);
            w.Write((uint)properties);
            w.Write((uint)automationPriority);
            w.Write((uint)awarenessOverlayLevel);
            w.Write(unknown2);
            w.Write(unknown3);
            w.Write(unknown4);
            w.Write(unknown5);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzStateMachine(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public class Animation : AHandlerElement, IEquatable<Animation>
        {
            #region Attributes
            uint nameHash;
            uint actor1Hash;
            uint actor2Hash;
            #endregion

            #region Constructors
            public Animation(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Animation(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Animation(int APIversion, EventHandler handler, Animation basis)
                : this(APIversion, handler
                , basis.nameHash
                , basis.actor1Hash
                , basis.actor2Hash
                ) { }
            public Animation(int APIversion, EventHandler handler
                , uint nameHash
                , uint actor1Hash
                , uint actor2Hash
                )
                : base(APIversion, handler)
            {
                this.nameHash = nameHash;
                this.actor1Hash = actor1Hash;
                this.actor2Hash = actor2Hash;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                nameHash = r.ReadUInt32();
                actor1Hash = r.ReadUInt32();
                actor2Hash = r.ReadUInt32();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(nameHash);
                w.Write(actor1Hash);
                w.Write(actor2Hash);
            }
            #endregion

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new Animation(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Animation>
            public bool Equals(Animation other)
            {
                return nameHash.Equals(other.nameHash) && actor1Hash.Equals(other.actor1Hash) && actor2Hash.Equals(other.actor2Hash);
            }
            #endregion

            #region ContentFields
            [ElementPriority(1)]
            public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint Actor1Hash { get { return actor1Hash; } set { if (actor1Hash != value) { actor1Hash = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public uint Actor2Hash { get { return actor2Hash; } set { if (actor2Hash != value) { actor2Hash = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
            #endregion
        }
        public class AnimationList : DependentList<Animation>
        {
            #region Constructors
            public AnimationList(EventHandler handler) : base(handler) { }
            public AnimationList(EventHandler handler, IEnumerable<Animation> ilt) : base(handler, ilt) { }
            public AnimationList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region DependentList<Animation>
            protected override Animation CreateElement(Stream s) { return new Animation(0, handler, s); }
            protected override void WriteElement(Stream s, Animation element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public ChunkReferenceList ActorDefinitionIndexes { get { return actorDefinitionIndexes; } set { if (actorDefinitionIndexes != value) { actorDefinitionIndexes = new ChunkReferenceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public ChunkReferenceList PropertyDefinitionIndexes { get { return propertyDefinitionIndexes; } set { if (propertyDefinitionIndexes != value) { propertyDefinitionIndexes = new ChunkReferenceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public ChunkReferenceList StateIndexes { get { return stateIndexes; } set { if (stateIndexes != value) { stateIndexes = new ChunkReferenceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public AnimationList Animations { get { return animations; } set { if (animations != value) { animations = new AnimationList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public Flags Properties { get { return properties; } set { if (properties != value) { properties = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public AnimationPriority AutomationPriority { get { return automationPriority; } set { if (automationPriority != value) { automationPriority = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(19)]
        public AwarenessLevel AwarenessOverlayLevel { get { return awarenessOverlayLevel; } set { if (awarenessOverlayLevel != value) { awarenessOverlayLevel = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(20)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(21)]
        public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22)]
        public uint Unknown4 { get { return unknown4; } set { if (unknown4 != value) { unknown4 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public uint Unknown5 { get { return unknown5; } set { if (unknown5 != value) { unknown5 = value; OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzState : JazzChunk
    {
        const string TAG = "S_St";

        [Flags]
        public enum Flags : uint
        {
            None = 0x0000,
            Public = 0x0001,
            Entry = 0x0002,
            Exit = 0x0004,
            Loop = 0x0008,
            OneShot = 0x0010,
            OneShotHold = 0x0020,
            Synchronized = 0x0040,
            Join = 0x0080,
            Explicit = 0x0100
        }

        #region Attributes
        uint version = 0x0101;
        uint nameHash;
        Flags flags;
        GenericRCOLResource.ChunkReference decisionGraphIndex;
        ChunkReferenceList outboundStateIndexes;
        AwarenessLevel awarenessOverlayLevel;
        #endregion

        #region Constructors
        public JazzState(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzState(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzState(int APIversion, EventHandler handler, JazzState basis)
            : this(APIversion, handler
            , basis.version
            , basis.nameHash
            , basis.flags
            , basis.decisionGraphIndex
            , basis.outboundStateIndexes
            , basis.awarenessOverlayLevel
            )
        { }
        public JazzState(int APIversion, EventHandler handler
            , uint version
            , uint nameHash
            , Flags flags
            , GenericRCOLResource.ChunkReference decisionGraphIndex
            , ChunkReferenceList outboundStateIndexes
            , AwarenessLevel awarenessOverlayLevel
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.nameHash = nameHash;
            this.flags = flags;
            this.decisionGraphIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, decisionGraphIndex);
            this.outboundStateIndexes = new ChunkReferenceList(handler, outboundStateIndexes);
            this.awarenessOverlayLevel = awarenessOverlayLevel;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDAFE; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.nameHash = r.ReadUInt32();
            this.flags = (Flags)r.ReadUInt32();
            this.decisionGraphIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            this.outboundStateIndexes = new ChunkReferenceList(handler, s);
            this.awarenessOverlayLevel = (AwarenessLevel)r.ReadUInt32();
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write(nameHash);
            w.Write((uint)flags);
            if (decisionGraphIndex == null) decisionGraphIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
            decisionGraphIndex.UnParse(ms);
            if (outboundStateIndexes == null) outboundStateIndexes = new ChunkReferenceList(handler);
            outboundStateIndexes.UnParse(ms);
            w.Write((uint)awarenessOverlayLevel);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzState(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public GenericRCOLResource.ChunkReference DecisionGraphIndex { get { return decisionGraphIndex; } set { if (decisionGraphIndex != value) { decisionGraphIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public Flags Properties { get { return flags; } set { if (flags != value) { flags = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public ChunkReferenceList OutboundStateIndexes { get { return outboundStateIndexes; } set { if (outboundStateIndexes != value) { outboundStateIndexes = new ChunkReferenceList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public AwarenessLevel AwarenessOverlayLevel { get { return awarenessOverlayLevel; } set { if (awarenessOverlayLevel != value) { awarenessOverlayLevel = value; OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzDecisionGraph : JazzChunk
    {
        const string TAG = "S_DG";

        #region Attributes
        uint version = 0x0101;
        uint unknown1;
        ChunkReferenceList outboundDecisionGraphIndexes;
        ChunkReferenceList inboundDecisionGraphIndexes;
        //0xDEADBEEF
        #endregion

        #region Constructors
        public JazzDecisionGraph(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzDecisionGraph(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzDecisionGraph(int APIversion, EventHandler handler, JazzDecisionGraph basis)
            : this(APIversion, handler
            , basis.version
            , basis.unknown1
            , basis.outboundDecisionGraphIndexes
            , basis.inboundDecisionGraphIndexes
            )
        { }
        public JazzDecisionGraph(int APIversion, EventHandler handler
            , uint version
            , uint unknown1
            , IEnumerable<GenericRCOLResource.ChunkReference> outboundDecisionGraphIndexes
            , IEnumerable<GenericRCOLResource.ChunkReference> inboundDecisionGraphIndexes
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.unknown1 = unknown1;
            this.outboundDecisionGraphIndexes = new ChunkReferenceList(handler, outboundDecisionGraphIndexes);
            this.inboundDecisionGraphIndexes = new ChunkReferenceList(handler, inboundDecisionGraphIndexes);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDB18; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.unknown1 = r.ReadUInt32();
            this.outboundDecisionGraphIndexes = new ChunkReferenceList(handler, s);
            this.inboundDecisionGraphIndexes = new ChunkReferenceList(handler, s);
            DEADBEEF.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write(unknown1);
            if (outboundDecisionGraphIndexes == null) outboundDecisionGraphIndexes = new ChunkReferenceList(handler);
            outboundDecisionGraphIndexes.UnParse(ms);
            if (inboundDecisionGraphIndexes == null) inboundDecisionGraphIndexes = new ChunkReferenceList(handler);
            inboundDecisionGraphIndexes.UnParse(ms);
            DEADBEEF.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzDecisionGraph(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public ChunkReferenceList OutboundDecisionGraphIndexes { get { return outboundDecisionGraphIndexes; } set { if (outboundDecisionGraphIndexes != value) { outboundDecisionGraphIndexes = new ChunkReferenceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public ChunkReferenceList InboundDecisionGraphIndexes { get { return inboundDecisionGraphIndexes; } set { if (inboundDecisionGraphIndexes != value) { inboundDecisionGraphIndexes = new ChunkReferenceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzActorDefinition : JazzChunk
    {
        const string TAG = "S_AD";

        #region Attributes
        uint version = 0x0100;
        uint nameHash;
        uint unknown1;
        #endregion

        #region Constructors
        public JazzActorDefinition(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzActorDefinition(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzActorDefinition(int APIversion, EventHandler handler, JazzActorDefinition basis)
            : this(APIversion, handler
            , basis.version
            , basis.nameHash
            , basis.unknown1
            )
        { }
        public JazzActorDefinition(int APIversion, EventHandler handler
            , uint version
            , uint nameHash
            , uint unknown1
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.nameHash = nameHash;
            this.unknown1 = unknown1;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDB2F; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.nameHash = r.ReadUInt32();
            this.unknown1 = r.ReadUInt32();
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write(nameHash);
            w.Write(unknown1);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzActorDefinition(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzParameterDefinition : JazzChunk
    {
        const string TAG = "S_PD";

        #region Attributes
        uint version = 0x0100;
        uint nameHash;
        uint defaultValue;
        #endregion

        #region Constructors
        public JazzParameterDefinition(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzParameterDefinition(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzParameterDefinition(int APIversion, EventHandler handler, JazzParameterDefinition basis)
            : this(APIversion, handler
            , basis.version
            , basis.nameHash
            , basis.defaultValue
            )
        { }
        public JazzParameterDefinition(int APIversion, EventHandler handler
            , uint version
            , uint nameHash
            , uint defaultValue
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.nameHash = nameHash;
            this.defaultValue = defaultValue;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDB46; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.nameHash = r.ReadUInt32();
            this.defaultValue = r.ReadUInt32();
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write(nameHash);
            w.Write(defaultValue);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzParameterDefinition(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public uint DefaultValue { get { return defaultValue; } set { if (defaultValue != value) { defaultValue = value; OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
    #endregion

    #region Decision Graph Node Chunks
    [Flags]
    public enum JazzAnimationFlags : uint
    {
        TimingNormal = 0x00,
        Default = 0x01,
        AtEnd = 0x01,
        LoopAsNeeded = 0x02,
        OverridePriority = 0x04,
        Mirror = 0x08,
        OverrideMirror = 0x10,
        OverrideTiming0 = 0x20,
        OverrideTiming1 = 0x40,
        TimingMaster = 0x20,
        TimingSlave = 0x40,
        TimingIgnored = 0x60,
        TimingMask = 0x60,
        Interruptible = 0x80,
        ForceBlend = 0x100,
        UseTimingPriority = 0x200,
        UseTimingPriorityAsClockMaster = 0x400,
        BaseClipIsSocial = 0x800,
        AdditiveClipIsSocial = 0x1000,
        BaseClipIsObjectOnly = 0x2000,
        AdditiveClipIsObjectOnly = 0x4000,
        HoldPose = 0x8000,
        BlendMotionAccumulation = 0x10000
    }

    public class JazzPlayAnimationNode : JazzChunk
    {
        const string TAG = "Play";

        #region Attributes
        uint version = 0x0105;
        TGIBlock clipResource;
        TGIBlock tkmkResource;
        //actorSlots count is here
        uint unknown1;
        uint unknown2;
        uint unknown3;
        ActorSlotList actorSlots;
        ActorSuffixList actorSuffixes;
        //0xDEADBEEF
        TGIBlock additiveClipResource;
        string animation = "";
        //followed by padding to next DWORD
        string additiveAnimation = "";
        //followed by padding to next DWORD
        //0xDEADBEEF
        JazzAnimationFlags animationNodeFlags;
        AnimationPriority animationPriority;
        float unknown9;
        float blendInTime;
        float blendOutTime;
        float unknown11;
        float speed;
        GenericRCOLResource.ChunkReference actorDefinitionIndex;
        AnimationPriority timingPriority;
        uint unknown13;
        uint unknown14;
        uint unknown15;
        uint unknown16;
        uint unknown17;
        uint unknown18;
        //0xDEADBEEF
        ChunkReferenceList decisionGraphIndexes;
        //'/DGN'
        #endregion

        #region Constructors
        public JazzPlayAnimationNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzPlayAnimationNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzPlayAnimationNode(int APIversion, EventHandler handler, JazzPlayAnimationNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.clipResource
            , basis.tkmkResource
            , basis.unknown1
            , basis.unknown2
            , basis.unknown3
            , basis.actorSlots
            , basis.actorSuffixes
            , basis.additiveClipResource
            , basis.animation
            , basis.additiveAnimation
            , basis.animationNodeFlags
            , basis.animationPriority
            , basis.unknown9
            , basis.blendInTime
            , basis.blendOutTime
            , basis.unknown11
            , basis.speed
            , basis.actorDefinitionIndex
            , basis.timingPriority
            , basis.unknown13
            , basis.unknown14
            , basis.unknown15
            , basis.unknown16
            , basis.unknown17
            , basis.unknown18
            , basis.decisionGraphIndexes
            )
        { }
        public JazzPlayAnimationNode(int APIversion, EventHandler handler
            , uint version
            , IResourceKey clipResource
            , IResourceKey tkmkResource
            , uint unknown1
            , uint unknown2
            , uint unknown3
            , IEnumerable<ActorSlot> actorSlots
            , IEnumerable<ActorSuffix> actorSuffixes
            , IResourceKey additiveClipResource
            , string animation
            , string additiveAnimation
            , JazzAnimationFlags animationNodeFlags
            , AnimationPriority animationPriority
            , float unknown9
            , float blendInTime
            , float blendOutTime
            , float unknown11
            , float speed
            , GenericRCOLResource.ChunkReference actorDefinitionIndex
            , AnimationPriority timingPriority
            , uint unknown13
            , uint unknown14
            , uint unknown15
            , uint unknown16
            , uint unknown17
            , uint unknown18
            , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.clipResource = new TGIBlock(requestedApiVersion, handler, "ITG", clipResource);
            this.tkmkResource = new TGIBlock(requestedApiVersion, handler, "ITG", tkmkResource);
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.unknown3 = unknown3;
            this.actorSlots = actorSlots == null ? null : new ActorSlotList(handler, actorSlots);
            this.actorSuffixes = actorSuffixes == null ? null : new ActorSuffixList(handler, actorSuffixes);
            this.additiveClipResource = new TGIBlock(requestedApiVersion, handler, "ITG", additiveClipResource);
            this.animation = animation;
            this.additiveAnimation = additiveAnimation;
            this.animationNodeFlags = animationNodeFlags;
            this.animationPriority = animationPriority;
            this.unknown9 = unknown9;
            this.blendInTime = blendInTime;
            this.blendOutTime = blendOutTime;
            this.unknown11 = unknown11;
            this.speed = speed;
            this.actorDefinitionIndex = actorDefinitionIndex;
            this.timingPriority = timingPriority;
            this.unknown13 = unknown13;
            this.unknown14 = unknown14;
            this.unknown15 = unknown15;
            this.unknown16 = unknown16;
            this.unknown17 = unknown17;
            this.unknown18 = unknown18;
            this.decisionGraphIndexes = decisionGraphIndexes == null ? null : new ChunkReferenceList(handler, decisionGraphIndexes);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDB5F; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            int actorSlotCount;
            BinaryReader r = new BinaryReader(s);

            this.version = r.ReadUInt32();
            this.clipResource = new TGIBlock(requestedApiVersion, handler, "ITG", s);
            this.tkmkResource = new TGIBlock(requestedApiVersion, handler, "ITG", s);
            actorSlotCount = r.ReadInt32();
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.actorSlots = new ActorSlotList(handler, actorSlotCount, s);
            this.actorSuffixes = new ActorSuffixList(handler, s);
            DEADBEEF.Parse(s);
            this.additiveClipResource = new TGIBlock(requestedApiVersion, handler, "ITG", s);
            this.animation = System.Text.Encoding.Unicode.GetString(r.ReadBytes(r.ReadInt32() * 2));
            if (this.animation.Length > 0) r.ReadUInt16();
            ExpectZero(s);
            this.additiveAnimation = System.Text.Encoding.Unicode.GetString(r.ReadBytes(r.ReadInt32() * 2));
            if (this.additiveAnimation.Length > 0) r.ReadUInt16();
            ExpectZero(s);
            DEADBEEF.Parse(s);
            this.animationNodeFlags = (JazzAnimationFlags)r.ReadUInt32();
            this.animationPriority = (AnimationPriority)r.ReadUInt32();
            this.unknown9 = r.ReadSingle();
            this.blendInTime = r.ReadSingle();
            this.blendOutTime = r.ReadSingle();
            this.unknown11 = r.ReadSingle();
            this.speed = r.ReadSingle();
            this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, s);
            this.timingPriority = (AnimationPriority)r.ReadUInt32();
            this.unknown13 = r.ReadUInt32();
            this.unknown14 = r.ReadUInt32();
            this.unknown15 = r.ReadUInt32();
            this.unknown16 = r.ReadUInt32();
            this.unknown17 = r.ReadUInt32();
            this.unknown18 = r.ReadUInt32();
            DEADBEEF.Parse(s);
            this.decisionGraphIndexes = new ChunkReferenceList(handler, s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (clipResource == null) clipResource = new TGIBlock(requestedAPIversion, handler, "ITG");
            clipResource.UnParse(ms);
            if (tkmkResource == null) tkmkResource = new TGIBlock(requestedAPIversion, handler, "ITG");
            tkmkResource.UnParse(ms);
            if (actorSlots == null) actorSlots = new ActorSlotList(handler);
            w.Write(actorSlots.Count);
            w.Write(unknown1);
            w.Write(unknown2);
            w.Write(unknown3);
            actorSlots.UnParse(ms);
            if (actorSuffixes == null) actorSuffixes = new ActorSuffixList(handler);
            actorSuffixes.UnParse(ms);
            DEADBEEF.UnParse(ms);
            if (additiveClipResource == null) additiveClipResource = new TGIBlock(requestedApiVersion, handler, "ITG");
            additiveClipResource.UnParse(ms);
            w.Write(animation.Length);
            w.Write(System.Text.Encoding.Unicode.GetBytes(animation));
            if (this.animation.Length > 0) w.Write((UInt16)0);
            PadZero(ms);
            w.Write(additiveAnimation.Length);
            w.Write(System.Text.Encoding.Unicode.GetBytes(additiveAnimation));
            if (this.additiveAnimation.Length > 0) w.Write((UInt16)0);
            PadZero(ms);
            DEADBEEF.UnParse(ms);
            w.Write((uint)animationNodeFlags);
            w.Write((uint)animationPriority);
            w.Write(unknown9);
            w.Write(blendInTime);
            w.Write(blendOutTime);
            w.Write(unknown11);
            w.Write(speed);
            if (actorDefinitionIndex == null) actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, 0);
            actorDefinitionIndex.UnParse(ms);
            w.Write((uint)timingPriority);
            w.Write(unknown13);
            w.Write(unknown14);
            w.Write(unknown15);
            w.Write(unknown16);
            w.Write(unknown17);
            w.Write(unknown18);
            DEADBEEF.UnParse(ms);
            if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
            decisionGraphIndexes.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzPlayAnimationNode(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public class ActorSlot : AHandlerElement, IEquatable<ActorSlot>
        {
            #region Attributes
            uint chainId;
            uint slotId;
            uint actorNameHash;
            uint slotNameHash;
            #endregion

            #region Constructors
            public ActorSlot(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public ActorSlot(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public ActorSlot(int APIversion, EventHandler handler, ActorSlot basis)
                : this(APIversion, handler
                , basis.chainId
                , basis.slotId
                , basis.actorNameHash
                , basis.slotNameHash
                )
            {
            }
            public ActorSlot(int APIversion, EventHandler handler
                , uint chainId
                , uint slotId
                , uint actorNameHash
                , uint slotNameHash
                )
                : base(APIversion, handler)
            {
                this.chainId = chainId;
                this.slotId = slotId;
                this.actorNameHash = actorNameHash;
                this.slotNameHash = slotNameHash;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                chainId = r.ReadUInt32();
                slotId = r.ReadUInt32();
                actorNameHash = r.ReadUInt32();
                slotNameHash = r.ReadUInt32();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(chainId);
                w.Write(slotId);
                w.Write(actorNameHash);
                w.Write(slotNameHash);
            }
            #endregion

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new ActorSlot(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Animation>
            public bool Equals(ActorSlot other)
            {
                return chainId.Equals(other.chainId) && slotId.Equals(other.slotId) && actorNameHash.Equals(other.actorNameHash) && slotNameHash.Equals(other.slotNameHash);
            }
            #endregion

            #region ContentFields
            [ElementPriority(1)]
            public uint ChainId { get { return chainId; } set { if (chainId != value) { chainId = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint SlotId { get { return slotId; } set { if (slotId != value) { slotId = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public uint ActorNameHash { get { return actorNameHash; } set { if (actorNameHash != value) { actorNameHash = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public uint SlotNameHash { get { return slotNameHash; } set { if (slotNameHash != value) { slotNameHash = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class ActorSlotList : DependentList<ActorSlot>
        {
            int count;

            #region Constructors
            public ActorSlotList(EventHandler handler) : base(handler) { }
            public ActorSlotList(EventHandler handler, IEnumerable<ActorSlot> basis) : base(handler, basis) { }
            public ActorSlotList(EventHandler handler, int count, Stream s) : base(handler) { elementHandler = handler; this.count = count; Parse(s); this.handler = handler; }
            #endregion

            #region DependentList<ActorSlot>
            protected override int ReadCount(Stream s) { return count; }
            protected override void WriteCount(Stream s, int count) { }

            protected override ActorSlot CreateElement(Stream s) { return new ActorSlot(0, handler, s); }

            protected override void WriteElement(Stream s, ActorSlot element) { element.UnParse(s); }
            #endregion
        }
        public class ActorSuffix : AHandlerElement, IEquatable<ActorSuffix>
        {
            #region Attributes
            uint actorNameHash;
            uint suffixHash;
            #endregion

            #region Constructors
            public ActorSuffix(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public ActorSuffix(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public ActorSuffix(int APIversion, EventHandler handler, ActorSuffix basis)
                : this(APIversion, handler
                , basis.actorNameHash
                , basis.suffixHash
                )
            {
            }
            public ActorSuffix(int APIversion, EventHandler handler
                , uint actorNameHash
                , uint suffixHash
                )
                : base(APIversion, handler)
            {
                this.actorNameHash = actorNameHash;
                this.suffixHash = suffixHash;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                actorNameHash = r.ReadUInt32();
                suffixHash = r.ReadUInt32();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(actorNameHash);
                w.Write(suffixHash);
            }
            #endregion

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new ActorSuffix(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Animation>
            public bool Equals(ActorSuffix other)
            {
                return actorNameHash.Equals(other.actorNameHash) && suffixHash.Equals(other.suffixHash);
            }
            #endregion

            #region ContentFields
            [ElementPriority(1)]
            public uint ActorNameHash { get { return actorNameHash; } set { if (actorNameHash != value) { actorNameHash = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint SuffixHash { get { return suffixHash; } set { if (suffixHash != value) { suffixHash = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
            #endregion
        }
        public class ActorSuffixList : DependentList<ActorSuffix>
        {
            #region Constructors
            public ActorSuffixList(EventHandler handler) : base(handler) { }
            public ActorSuffixList(EventHandler handler, IEnumerable<ActorSuffix> basis) : base(handler, basis) { }
            public ActorSuffixList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region DependentList<ActorSuffix>
            protected override ActorSuffix CreateElement(Stream s) { return new ActorSuffix(0, handler, s); }

            protected override void WriteElement(Stream s, ActorSuffix element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public IResourceKey ClipResource { get { return clipResource; } set { if (clipResource != value) { clipResource = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public IResourceKey TkmkResource { get { return tkmkResource; } set { if (tkmkResource != value) { tkmkResource = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public ActorSlotList ActorSlots { get { return actorSlots; } set { if (actorSlots != value) { actorSlots = value == null ? null : new ActorSlotList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public ActorSuffixList ActorSuffixes { get { return actorSuffixes; } set { if (actorSuffixes != value) { actorSuffixes = value == null ? null : new ActorSuffixList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(19)]
        public IResourceKey AdditiveClipResource { get { return additiveClipResource; } set { if (additiveClipResource != value) { additiveClipResource = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public string Animation { get { return animation; } set { if (animation != value) { animation = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24)]
        public string AdditiveAnimation { get { return additiveAnimation; } set { if (additiveAnimation != value) { additiveAnimation = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public JazzAnimationFlags AnimationNodeFlags { get { return animationNodeFlags; } set { if (animationNodeFlags != value) { animationNodeFlags = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public AnimationPriority AnimationPriority1 { get { return animationPriority; } set { if (animationPriority != value) { animationPriority = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27)]
        public float Unknown9 { get { return unknown9; } set { if (unknown9 != value) { unknown9 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(28)]
        public float BlendInTime { get { return blendInTime; } set { if (blendInTime != value) { blendInTime = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(29)]
        public float BlendOutTime { get { return blendOutTime; } set { if (blendOutTime != value) { blendOutTime = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(30)]
        public float Unknown11 { get { return unknown11; } set { if (unknown11 != value) { unknown11 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(31)]
        public float Speed { get { return speed; } set { if (speed != value) { speed = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(32)]
        public GenericRCOLResource.ChunkReference ActorDefinitionIndex { get { return actorDefinitionIndex; } set { if (actorDefinitionIndex != value) { actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(33)]
        public AnimationPriority TimingPriority { get { return timingPriority; } set { if (timingPriority != value) { timingPriority = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(34)]
        public uint Unknown13 { get { return unknown13; } set { if (unknown13 != value) { unknown13 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(35)]
        public uint Unknown14 { get { return unknown14; } set { if (unknown14 != value) { unknown14 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(36)]
        public uint Unknown15 { get { return unknown15; } set { if (unknown15 != value) { unknown15 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(37)]
        public uint Unknown16 { get { return unknown16; } set { if (unknown16 != value) { unknown16 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(38)]
        public uint Unknown17 { get { return unknown17; } set { if (unknown17 != value) { unknown17 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(39)]
        public uint Unknown18 { get { return unknown18; } set { if (unknown18 != value) { unknown18 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(40)]
        public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = value == null ? null : new ChunkReferenceList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzRandomNode : JazzChunk
    {
        const string TAG = "Rand";

        [Flags]
        public enum Flags : uint
        {
            None = 0x00,
            AvoidRepeats = 0x01
        }

        #region Attributes
        uint version = 0x0101;
        OutcomeList outcomes;
        //0xDEADBEEF
        Flags flags;
        //'/DGN'
        #endregion

        #region Constructors
        public JazzRandomNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzRandomNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzRandomNode(int APIversion, EventHandler handler, JazzRandomNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.outcomes
            , basis.flags
            )
        { }
        public JazzRandomNode(int APIversion, EventHandler handler
            , uint version
            , IEnumerable<Outcome> outcomes
            , Flags flags
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.outcomes = new OutcomeList(handler, outcomes);
            this.flags = flags;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDB70; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.outcomes = new OutcomeList(handler, s);
            DEADBEEF.Parse(s);
            this.flags = (Flags)r.ReadUInt32();
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (outcomes == null) outcomes = new OutcomeList(handler);
            outcomes.UnParse(ms);
            DEADBEEF.UnParse(ms);
            w.Write((uint)flags);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzRandomNode(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public class Outcome : AHandlerElement, IEquatable<Outcome>
        {
            #region Attributes
            float weight;
            ChunkReferenceList decisionGraphIndexes;
            #endregion

            #region Constructors
            public Outcome(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Outcome(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Outcome(int APIversion, EventHandler handler, Outcome basis)
                : this(APIversion, handler
                , basis.weight
                , basis.decisionGraphIndexes
                )
            {
            }
            public Outcome(int APIversion, EventHandler handler
                , float weight
                , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
                )
                : base(APIversion, handler)
            {
                this.weight = weight;
                this.decisionGraphIndexes = new ChunkReferenceList(handler, decisionGraphIndexes);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                weight = new BinaryReader(s).ReadSingle();
                decisionGraphIndexes = new ChunkReferenceList(handler, s);
            }

            internal void UnParse(Stream s)
            {
                new BinaryWriter(s).Write(weight);
                if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
                decisionGraphIndexes.UnParse(s);
            }
            #endregion

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new Outcome(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Animation>
            public bool Equals(Outcome other)
            {
                return weight.Equals(other.weight) && decisionGraphIndexes.Equals(other.decisionGraphIndexes);
            }
            #endregion

            #region ContentFields
            [ElementPriority(1)]
            public float Weight { get { return weight; } set { if (weight != value) { weight = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = new ChunkReferenceList(handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class OutcomeList : DependentList<Outcome>
        {
            #region Constructors
            public OutcomeList(EventHandler handler) : base(handler) { }
            public OutcomeList(EventHandler handler, IEnumerable<Outcome> basis) : base(handler, basis) { }
            public OutcomeList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region DependentList<Outcome>
            protected override Outcome CreateElement(Stream s) { return new Outcome(0, handler, s); }

            protected override void WriteElement(Stream s, Outcome element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public OutcomeList Outcomes { get { return outcomes; } set { if (outcomes != value) { outcomes = new OutcomeList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public Flags Properties { get { return flags; } set { if (flags != value) { flags = value; OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzSelectOnParameterNode : JazzChunk
    {
        const string TAG = "SoPn";

        #region Attributes
        uint version = 0x0101;
        GenericRCOLResource.ChunkReference parameterDefinitionIndex;
        MatchList matches;
        //0xDEADBEEF
        //'/DGN'
        #endregion

        #region Constructors
        public JazzSelectOnParameterNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzSelectOnParameterNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzSelectOnParameterNode(int APIversion, EventHandler handler, JazzSelectOnParameterNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.parameterDefinitionIndex
            , basis.matches
            )
        { }
        public JazzSelectOnParameterNode(int APIversion, EventHandler handler
            , uint version
            , GenericRCOLResource.ChunkReference parameterDefinitionIndex
            , IEnumerable<Match> matches
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, parameterDefinitionIndex);
            this.matches = new MatchList(handler, matches);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDB92; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, s);
            this.matches = new MatchList(handler, s);
            DEADBEEF.Parse(s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (parameterDefinitionIndex == null) parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, 0);
            parameterDefinitionIndex.UnParse(ms);
            if (matches == null) matches = new MatchList(handler);
            matches.UnParse(ms);
            DEADBEEF.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzSelectOnParameterNode(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public class Match : AHandlerElement, IEquatable<Match>
        {
            #region Attributes
            uint testValue;
            ChunkReferenceList decisionGraphIndexes;
            #endregion

            #region Constructors
            public Match(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Match(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Match(int APIversion, EventHandler handler, Match basis)
                : this(APIversion, handler
                , basis.testValue
                , basis.decisionGraphIndexes
                )
            {
            }
            public Match(int APIversion, EventHandler handler
                , uint testValue
                , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
                )
                : base(APIversion, handler)
            {
                this.testValue = testValue;
                this.decisionGraphIndexes = new ChunkReferenceList(handler, decisionGraphIndexes);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                testValue = new BinaryReader(s).ReadUInt32();
                decisionGraphIndexes = new ChunkReferenceList(handler, s);
            }

            internal void UnParse(Stream s)
            {
                new BinaryWriter(s).Write(testValue);
                if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
                decisionGraphIndexes.UnParse(s);
            }
            #endregion

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new Match(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Animation>
            public bool Equals(Match other)
            {
                return testValue.Equals(other.testValue) && decisionGraphIndexes.Equals(other.decisionGraphIndexes);
            }
            #endregion

            #region ContentFields
            [ElementPriority(1)]
            public uint TestValue { get { return testValue; } set { if (testValue != value) { testValue = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = new ChunkReferenceList(handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class MatchList : DependentList<Match>
        {
            #region Constructors
            public MatchList(EventHandler handler) : base(handler) { }
            public MatchList(EventHandler handler, IEnumerable<Match> basis) : base(handler, basis) { }
            public MatchList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region DependentList<Outcome>
            protected override Match CreateElement(Stream s) { return new Match(0, handler, s); }

            protected override void WriteElement(Stream s, Match element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public GenericRCOLResource.ChunkReference ParameterDefinitionIndex { get { return parameterDefinitionIndex; } set { if (parameterDefinitionIndex != value) { parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public MatchList Matches { get { return matches; } set { if (matches != value) { matches = new MatchList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzSelectOnDestinationNode : JazzChunk
    {
        const string TAG = "DG00";

        #region Attributes
        uint version = 0x0101;
        MatchList matches;
        //0xDEADBEEF
        //'/DGN'
        #endregion

        #region Constructors
        public JazzSelectOnDestinationNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzSelectOnDestinationNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzSelectOnDestinationNode(int APIversion, EventHandler handler, JazzSelectOnDestinationNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.matches
            )
        { }
        public JazzSelectOnDestinationNode(int APIversion, EventHandler handler
            , uint version
            , IEnumerable<Match> matches
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.matches = new MatchList(handler, matches);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEDBA5; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.matches = new MatchList(handler, s);
            DEADBEEF.Parse(s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (matches == null) matches = new MatchList(handler);
            matches.UnParse(ms);
            DEADBEEF.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzSelectOnParameterNode(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public class Match : AHandlerElement, IEquatable<Match>
        {
            #region Attributes
            GenericRCOLResource.ChunkReference stateIndex;
            ChunkReferenceList decisionGraphIndexes;
            #endregion

            #region Constructors
            public Match(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Match(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Match(int APIversion, EventHandler handler, Match basis)
                : this(APIversion, handler
                , basis.stateIndex
                , basis.decisionGraphIndexes
                )
            {
            }
            public Match(int APIversion, EventHandler handler
                , GenericRCOLResource.ChunkReference stateIndex
                , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
                )
                : base(APIversion, handler)
            {
                this.stateIndex = stateIndex;
                this.decisionGraphIndexes = new ChunkReferenceList(handler, decisionGraphIndexes);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                stateIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
                decisionGraphIndexes = new ChunkReferenceList(handler, s);
            }

            internal void UnParse(Stream s)
            {
                if (stateIndex == null) stateIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
                stateIndex.UnParse(s);
                if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
                decisionGraphIndexes.UnParse(s);
            }
            #endregion

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new Match(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Animation>
            public bool Equals(Match other)
            {
                return stateIndex.Equals(other.stateIndex) && decisionGraphIndexes.Equals(other.decisionGraphIndexes);
            }
            #endregion

            #region ContentFields
            [ElementPriority(1)]
            public GenericRCOLResource.ChunkReference StateIndex { get { return stateIndex; } set { if (stateIndex != value) { stateIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = new ChunkReferenceList(handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class MatchList : DependentList<Match>
        {
            #region Constructors
            public MatchList(EventHandler handler) : base(handler) { }
            public MatchList(EventHandler handler, IEnumerable<Match> basis) : base(handler, basis) { }
            public MatchList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region DependentList<Outcome>
            protected override Match CreateElement(Stream s) { return new Match(0, handler, s); }

            protected override void WriteElement(Stream s, Match element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public MatchList Matches { get { return matches; } set { if (matches != value) { matches = new MatchList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzNextStateNode : JazzChunk
    {
        const string TAG = "SNSN";

        #region Attributes
        uint version = 0x0101;
        GenericRCOLResource.ChunkReference stateIndex;
        //'/DGN'
        #endregion

        #region Constructors
        public JazzNextStateNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzNextStateNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzNextStateNode(int APIversion, EventHandler handler, JazzNextStateNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.stateIndex
            )
        { }
        public JazzNextStateNode(int APIversion, EventHandler handler
            , uint version
            , GenericRCOLResource.ChunkReference stateIndex
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.stateIndex = stateIndex;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEEBDC; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.stateIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (stateIndex == null) stateIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
            stateIndex.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzNextStateNode(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public GenericRCOLResource.ChunkReference StateIndex { get { return stateIndex; } set { if (stateIndex != value) { stateIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzCreatePropNode : JazzChunk
    {
        const string TAG = "Prop";

        #region Attributes
        uint version = 0x0100;
        GenericRCOLResource.ChunkReference actorDefinitionIndex;
        GenericRCOLResource.ChunkReference parameterDefinitionIndex;
        TGIBlock propResource;
        uint unknown2;
        uint unknown3;
        uint unknown4;
        uint unknown5;
        //uint unknown6;
        ChunkReferenceList decisionGraphIndexes;
        //'/DGN'
        #endregion

        #region Constructors
        public JazzCreatePropNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzCreatePropNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzCreatePropNode(int APIversion, EventHandler handler, JazzCreatePropNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.actorDefinitionIndex
            , basis.parameterDefinitionIndex
            , basis.propResource
            , basis.unknown2
            , basis.unknown3
            , basis.unknown4
            , basis.unknown5
                //, basis.unknown6
            , basis.decisionGraphIndexes
            )
        { }
        public JazzCreatePropNode(int APIversion, EventHandler handler
            , uint version
            , GenericRCOLResource.ChunkReference actorDefinitionIndex
            , GenericRCOLResource.ChunkReference parameterDefinitionIndex
            , IResourceKey propResource
            , uint unknown2
            , uint unknown3
            , uint unknown4
            , uint unknown5
            //, uint unknown6
            , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, actorDefinitionIndex);
            this.parameterDefinitionIndex = parameterDefinitionIndex;
            this.propResource = new TGIBlock(requestedApiVersion, handler, "ITG", propResource);
            this.unknown2 = unknown2;
            this.unknown3 = unknown3;
            this.unknown4 = unknown4;
            this.unknown5 = unknown5;
            //this.unknown6 = unknown6;
            this.decisionGraphIndexes = new ChunkReferenceList(handler, decisionGraphIndexes);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEEBDD; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);

            this.version = r.ReadUInt32();
            this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, s);
            this.parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            this.propResource = new TGIBlock(requestedApiVersion, handler, "ITG", s);
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadUInt32();
            //this.unknown6 = r.ReadUInt32();
            this.decisionGraphIndexes = new ChunkReferenceList(handler, s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (actorDefinitionIndex == null) actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedAPIversion, handler, 0);
            actorDefinitionIndex.UnParse(ms);
            if (parameterDefinitionIndex == null) parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
            parameterDefinitionIndex.UnParse(ms);
            if (propResource == null) propResource = new TGIBlock(requestedAPIversion, handler, "ITG");
            propResource.UnParse(ms);
            w.Write(unknown2);
            w.Write(unknown3);
            w.Write(unknown4);
            w.Write(unknown5);
            //w.Write(unknown6);
            if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
            decisionGraphIndexes.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzCreatePropNode(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public GenericRCOLResource.ChunkReference ActorDefinitionIndex { get { return actorDefinitionIndex; } set { if (actorDefinitionIndex != value) { actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public GenericRCOLResource.ChunkReference ParameterDefinitionIndex { get { return parameterDefinitionIndex; } set { if (parameterDefinitionIndex != value) { parameterDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public IResourceKey PropResource { get { return propResource; } set { if (propResource != value) { propResource = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public uint Unknown4 { get { return unknown4; } set { if (unknown4 != value) { unknown4 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public uint Unknown5 { get { return unknown5; } set { if (unknown5 != value) { unknown5 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        //[ElementPriority(19)]
        //public uint Unknown6 { get { return unknown6; } set { if (unknown6 != value) { unknown6 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(20)]
        public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = new ChunkReferenceList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzActorOperationNode : JazzChunk
    {
        const string TAG = "AcOp";

        #region Attributes
        uint version = 0x0100;
        GenericRCOLResource.ChunkReference actorDefinitionIndex;
        ActorOperation actorOp;
        uint operand;
        uint unknown1;
        uint unknown2;
        uint unknown3;
        ChunkReferenceList decisionGraphIndexes;
        //'/DGN'
        #endregion

        #region Constructors
        public JazzActorOperationNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzActorOperationNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzActorOperationNode(int APIversion, EventHandler handler, JazzActorOperationNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.actorDefinitionIndex
            , basis.actorOp
            , basis.operand
            , basis.unknown1
            , basis.unknown2
            , basis.unknown3
            , basis.decisionGraphIndexes
            )
        { }
        public JazzActorOperationNode(int APIversion, EventHandler handler
            , uint version
            , GenericRCOLResource.ChunkReference actorDefinitionIndex
            , ActorOperation actorOp
            , uint operand
            , uint unknown1
            , uint unknown2
            , uint unknown3
            , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.actorDefinitionIndex = actorDefinitionIndex;
            this.actorOp = actorOp;
            this.operand = operand;
            this.unknown1 = unknown1;
            this.unknown2 = unknown2;
            this.unknown3 = unknown3;
            this.decisionGraphIndexes = new ChunkReferenceList(handler, decisionGraphIndexes);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02EEEBDE; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            this.actorOp = (ActorOperation)r.ReadUInt32();
            this.operand = r.ReadUInt32();
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.decisionGraphIndexes = new ChunkReferenceList(handler, s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            if (this.actorDefinitionIndex == null) this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
            this.actorDefinitionIndex.UnParse(ms);
            w.Write((uint)actorOp);
            w.Write(operand);
            w.Write(unknown1);
            w.Write(unknown2);
            w.Write(unknown3);
            if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
            decisionGraphIndexes.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzActorOperationNode(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public enum ActorOperation : uint
        {
            None = 0,
            SetMirror = 1,
        }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public GenericRCOLResource.ChunkReference ActorDefinitionIndex { get { return actorDefinitionIndex; } set { if (actorDefinitionIndex != value) { actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public ActorOperation ActorOp { get { return actorOp; } set { if (actorOp != value) { actorOp = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public uint Operand { get { return operand; } set { if (operand != value) { operand = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = new ChunkReferenceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class JazzStopAnimationNode : JazzChunk
    {
        const string TAG = "Stop";

        #region Attributes
        uint version = 0x0104;
        JazzAnimationFlags animationFlags;
        AnimationPriority animationPriority;
        float unknown1;
        float blendInTime;
        float blendOutTime;
        float unknown4;
        float speed;
        GenericRCOLResource.ChunkReference actorDefinitionIndex;
        AnimationPriority timingPriority;
        uint unknown6;
        uint unknown7;
        uint unknown8;
        uint unknown9;
        uint unknown10;
        uint unknown11;
        //0xDEADBEEF
        ChunkReferenceList decisionGraphIndexes;
        //'/DGN'
        #endregion

        #region Constructors
        public JazzStopAnimationNode(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public JazzStopAnimationNode(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public JazzStopAnimationNode(int APIversion, EventHandler handler, JazzStopAnimationNode basis)
            : this(APIversion, handler
            , basis.version
            , basis.animationFlags
            , basis.animationPriority
            , basis.unknown1
            , basis.blendInTime
            , basis.blendOutTime
            , basis.unknown4
            , basis.speed
            , basis.actorDefinitionIndex
            , basis.timingPriority
            , basis.unknown6
            , basis.unknown7
            , basis.unknown8
            , basis.unknown9
            , basis.unknown10
            , basis.unknown11
            , basis.decisionGraphIndexes
            )
        { }
        public JazzStopAnimationNode(int APIversion, EventHandler handler
            , uint version
            , JazzAnimationFlags animationFlags
            , AnimationPriority animationPriority
            , float unknown1
            , float blendInTime
            , float blendOutTime
            , float unknown4
            , float speed
            , GenericRCOLResource.ChunkReference actorDefinitionIndex
            , AnimationPriority timingPriority
            , uint unknown6
            , uint unknown7
            , uint unknown8
            , uint unknown9
            , uint unknown10
            , uint unknown11
            , IEnumerable<GenericRCOLResource.ChunkReference> decisionGraphIndexes
            )
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.animationFlags = animationFlags;
            this.animationPriority = animationPriority;
            this.unknown1 = unknown1;
            this.blendInTime = blendInTime;
            this.blendOutTime = blendOutTime;
            this.unknown4 = unknown4;
            this.speed = speed;
            this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, actorDefinitionIndex);
            this.timingPriority = timingPriority;
            this.unknown6 = unknown6;
            this.unknown7 = unknown7;
            this.unknown8 = unknown8;
            this.unknown9 = unknown9;
            this.unknown10 = unknown10;
            this.unknown11 = unknown11;
            this.decisionGraphIndexes = new ChunkReferenceList(handler, decisionGraphIndexes);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x0344D438; } }

        protected override void Parse(Stream s)
        {
            base.Parse(s);

            BinaryReader r = new BinaryReader(s);

            this.version = r.ReadUInt32();
            this.animationFlags = (JazzAnimationFlags)r.ReadUInt32();
            this.animationPriority = (AnimationPriority)r.ReadUInt32();
            this.unknown1 = r.ReadSingle();
            this.blendInTime = r.ReadSingle();
            this.blendOutTime = r.ReadSingle();
            this.unknown4 = r.ReadSingle();
            this.speed = r.ReadSingle();
            this.actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            this.timingPriority = (AnimationPriority)r.ReadUInt32();
            this.unknown6 = r.ReadUInt32();
            this.unknown7 = r.ReadUInt32();
            this.unknown8 = r.ReadUInt32();
            this.unknown9 = r.ReadUInt32();
            this.unknown10 = r.ReadUInt32();
            this.unknown11 = r.ReadUInt32();
            DEADBEEF.Parse(s);
            this.decisionGraphIndexes = new ChunkReferenceList(handler, s);
            CloseDGN.Parse(s);
        }

        public override Stream UnParse()
        {
            Stream ms = base.UnParse();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write((uint)animationFlags);
            w.Write((uint)animationPriority);
            w.Write(unknown1);
            w.Write(blendInTime);
            w.Write(blendOutTime);
            w.Write(unknown4);
            w.Write(speed);
            if (actorDefinitionIndex == null) actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
            actorDefinitionIndex.UnParse(ms);
            w.Write((uint)timingPriority);
            w.Write(unknown6);
            w.Write(unknown7);
            w.Write(unknown8);
            w.Write(unknown9);
            w.Write(unknown10);
            w.Write(unknown11);
            DEADBEEF.UnParse(ms);
            if (decisionGraphIndexes == null) decisionGraphIndexes = new ChunkReferenceList(handler);
            decisionGraphIndexes.UnParse(ms);
            CloseDGN.UnParse(ms);

            return ms;
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new JazzStopAnimationNode(requestedApiVersion, handler, this); }
        #endregion

        #region ContentFields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public JazzAnimationFlags AnimationFlags { get { return animationFlags; } set { if (animationFlags != value) { animationFlags = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public AnimationPriority AnimationPriority1 { get { return animationPriority; } set { if (animationPriority != value) { animationPriority = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public float Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public float BlendInTime { get { return blendInTime; } set { if (blendInTime != value) { blendInTime = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public float BlendOutTime { get { return blendOutTime; } set { if (blendOutTime != value) { blendOutTime = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public float Unknown4 { get { return unknown4; } set { if (unknown4 != value) { unknown4 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public float Speed { get { return speed; } set { if (speed != value) { speed = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(19)]
        public GenericRCOLResource.ChunkReference ActorDefinitionIndex { get { return actorDefinitionIndex; } set { if (actorDefinitionIndex != value) { actorDefinitionIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(20)]
        public AnimationPriority TimingPriority { get { return timingPriority; } set { if (timingPriority != value) { timingPriority = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(21)]
        public uint Unknown6 { get { return unknown6; } set { if (unknown6 != value) { unknown6 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22)]
        public uint Unknown7 { get { return unknown7; } set { if (unknown7 != value) { unknown7 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public uint Unknown8 { get { return unknown8; } set { if (unknown8 != value) { unknown8 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24)]
        public uint Unknown9 { get { return unknown9; } set { if (unknown9 != value) { unknown9 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public uint Unknown10 { get { return unknown10; } set { if (unknown10 != value) { unknown10 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public uint Unknown11 { get { return unknown11; } set { if (unknown11 != value) { unknown11 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27)]
        public ChunkReferenceList DecisionGraphIndexes { get { return decisionGraphIndexes; } set { if (decisionGraphIndexes != value) { decisionGraphIndexes = new ChunkReferenceList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
    #endregion
}

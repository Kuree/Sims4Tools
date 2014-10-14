using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace s4pi.Miscellaneous
{
    public class SimModifierResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;


        private ContexData contexData { get; set; }
        private uint version { get; set; }
        private uint gender { get; set; }
        private uint region { get; set; }
        private uint linkTag { get; set; }
        private TGIBlock bonePoseKey { get; set; }
        private TGIBlock deformerMapShapeKey { get; set; }
        private TGIBlock deformerMapNormalKey { get; set; }
        private BoneEntryLIst boneEntryList { get; set; }

        
        public SimModifierResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.contexData = new ContexData(recommendedApiVersion, OnResourceChanged, s);
            this.version = r.ReadUInt32();
            this.gender = r.ReadUInt32();
            this.region = r.ReadUInt32();
            this.linkTag = r.ReadUInt32();
            this.bonePoseKey = new TGIBlock(recommendedApiVersion, OnResourceChanged, "TGI", s);
            this.deformerMapShapeKey = new TGIBlock(recommendedApiVersion, OnResourceChanged, "TGI", s);
            this.deformerMapNormalKey = new TGIBlock(recommendedApiVersion, OnResourceChanged, "TGI", s);
            this.boneEntryList = new BoneEntryLIst(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            if (this.contexData == null) this.contexData = new ContexData(recommendedApiVersion, OnResourceChanged);
            this.contexData.UnParse(ms);
            w.Write(this.version);
            w.Write(this.gender);
            w.Write(this.region);
            w.Write(this.linkTag);
            if (this.bonePoseKey == null) this.bonePoseKey = new TGIBlock(recommendedApiVersion, OnResourceChanged);
            this.bonePoseKey.UnParse(ms);
            if (this.deformerMapShapeKey == null) this.deformerMapShapeKey = new TGIBlock(recommendedApiVersion, OnResourceChanged);
            this.deformerMapShapeKey.UnParse(ms);
            if (this.deformerMapNormalKey == null) this.deformerMapNormalKey = new TGIBlock(recommendedApiVersion, OnResourceChanged);
            this.deformerMapNormalKey.UnParse(ms);
            if (this.boneEntryList == null) this.boneEntryList = new BoneEntryLIst(OnResourceChanged);
            this.boneEntryList.UnParse(ms);
            return ms;
        }
        #endregion

        #region Sub-Class
        public class ContexData: AHandlerElement
        {
            public uint contexVersion { get; set; }
            public uint publicKeyCount { get; set; }
            public uint externalKeyCount { get; set; }
            public uint delayLoadKeyCount { get; set; }
            public uint objectKeyCount { get; set; }

            public CountedTGIBlockList publicKey { get; set; }
            public CountedTGIBlockList externalKey { get; set; }
            public CountedTGIBlockList delayLoadKey { get; set; }
            public ObjectDataLIst objectKey { get; set; }
            public ContexData(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public ContexData(int APIversion, EventHandler handler) : base(APIversion, handler) { }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.contexVersion = r.ReadUInt32();
                this.publicKeyCount = r.ReadUInt32();
                this.externalKeyCount = r.ReadUInt32();
                this.delayLoadKeyCount = r.ReadUInt32();
                this.objectKeyCount = r.ReadUInt32();
                List<TGIBlock> tempList = new List<TGIBlock>();
                for (int i = 0; i < publicKeyCount; i++) tempList.Add(new TGIBlock(1, handler, "ITG", s));
                this.publicKey = new CountedTGIBlockList(null, tempList);
                tempList.Clear();
                for (int i = 0; i < externalKeyCount; i++) tempList.Add(new TGIBlock(1, handler, "ITG", s));
                this.externalKey = new CountedTGIBlockList(null, tempList);
                tempList.Clear();
                for (int i = 0; i < delayLoadKeyCount; i++) tempList.Add(new TGIBlock(1, handler, "ITG", s));
                this.delayLoadKey = new CountedTGIBlockList(null, tempList);
                this.objectKey = new ObjectDataLIst(handler, s, objectKeyCount);
                
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.contexVersion);
                if (this.publicKey == null) this.publicKey = new CountedTGIBlockList(handler);
                w.Write(this.publicKey.Count);
                if (this.externalKey == null) this.externalKey = new CountedTGIBlockList(handler);
                w.Write(this.externalKey.Count);
                if (this.delayLoadKey == null) this.delayLoadKey = new CountedTGIBlockList(handler);
                w.Write(this.delayLoadKey.Count);
                if (this.objectKey == null) this.objectKey = new ObjectDataLIst(handler);
                w.Write(this.objectKey.Count);
                foreach (var tgi in this.publicKey) tgi.UnParse(s);
                foreach (var tgi in this.externalKey) tgi.UnParse(s);
                foreach (var tgi in this.delayLoadKey) tgi.UnParse(s);
                foreach (var obj in this.objectKey) obj.UnParse(s);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Sub Class
            public class ObjectData : AHandlerElement, IEquatable<ObjectData>
            {
                public uint position { get; set; }
                public uint length { get; set; }
                public ObjectData(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
                public void Parse(Stream s)
                {
                    BinaryReader r = new BinaryReader(s);
                    this.position = r.ReadUInt32();
                    this.length = r.ReadUInt32();
                }

                public void UnParse(Stream s)
                {
                    BinaryWriter w = new BinaryWriter(s);
                    w.Write(this.position);
                    w.Write(this.length);
                }

                #region AHandlerElement Members
                public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
                public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
                #endregion

                #region IEquatable
                public bool Equals(ObjectData other)
                {
                    return this.position == other.position && this.length == other.length;
                }
                #endregion

                public string Value { get { return ValueBuilder; } }
            }

            public class ObjectDataLIst : DependentList<ObjectData>
            {
                public ObjectDataLIst(EventHandler handler) : base(handler) { }
                public ObjectDataLIst(EventHandler handler, Stream s, uint count) : base(handler) { Parse(s, count); }

                #region Data I/O
                protected void Parse(Stream s, uint count)
                {
                    for (uint i = 0; i < count; i++) this.Add(new ObjectData(recommendedApiVersion, handler, s));
                }

                public override void UnParse(Stream s)
                {
                    foreach (var bone in this) bone.UnParse(s);
                }

                protected override ObjectData CreateElement(Stream s) { return new ObjectData(1, handler, s); }
                protected override void WriteElement(Stream s, ObjectData element) { element.UnParse(s); }
                #endregion
            }

            #endregion

            public string Value { get { return ValueBuilder; } }
        }

        public class BoneEntryLIst:DependentList<BoneEntry>
        {
            public BoneEntryLIst(EventHandler handler) : base(handler) { }
            public BoneEntryLIst(EventHandler handler, Stream s) : base(handler) { Parse(s); }

            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint count = r.ReadUInt32();
                for (uint i = 0; i < count; i++) this.Add(new BoneEntry(recommendedApiVersion, handler, s));
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.Count);
                foreach (var bone in this) bone.UnParse(s);
            }

            protected override BoneEntry CreateElement(Stream s) { return new BoneEntry(1, handler, s); }
            protected override void WriteElement(Stream s, BoneEntry element) { element.UnParse(s); }
            #endregion
        }

        public class BoneEntry: AHandlerElement, IEquatable<BoneEntry>
        {
            public uint boneHash { get; set; }
            public float multiplier { get; set; }

            public BoneEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.boneHash = r.ReadUInt32();
                this.multiplier = r.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.boneHash);
                w.Write(this.multiplier);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable
            public bool Equals(BoneEntry other)
            {
                return this.boneHash == other.boneHash && this.multiplier == other.multiplier;
            }
            #endregion

            public string Value { get { return ValueBuilder; } }
        }

        #endregion

        #region Content Fields
        [ElementPriority(0)]
        public ContexData Contexdata { get { return this.contexData; } set { if (!contexData.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.contexData = value; } } }
        [ElementPriority(1)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.version = value; } } }
        [ElementPriority(2)]
        public uint Gender { get { return this.gender; } set { if (!this.gender.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.gender = value; } } }
        [ElementPriority(3)]
        public uint Region { get { return this.region; } set { if (!this.region.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.region = value; } } }
        [ElementPriority(4)]
        public uint LinkTag { get { return this.linkTag; } set { if (!this.linkTag.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.linkTag = value; } } }
        [ElementPriority(5)]
        public TGIBlock BonePostKey { get { return this.bonePoseKey; } set { if (!this.bonePoseKey.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.bonePoseKey = value; } } }
        [ElementPriority(6)]
        public TGIBlock DeformerMapShapeKey { get { return this.deformerMapShapeKey; } set { if (!this.deformerMapShapeKey.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.deformerMapShapeKey = value; } } }
        [ElementPriority(7)]
        public TGIBlock DeformerMapNormalKey { get { return this.deformerMapNormalKey; } set { if (!this.deformerMapNormalKey.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.deformerMapNormalKey = value; } } }
        [ElementPriority(8)]
        public BoneEntryLIst BoneEntryList { get { return this.boneEntryList; } set { if (!this.boneEntryList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.boneEntryList = value; } } }
        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class SimModifierHandler : AResourceHandler
    {
        public SimModifierHandler()
        {
            if (s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(SimModifierResource), new List<string>(new string[] { "0xC5F6763E", }));
        }
    }
}

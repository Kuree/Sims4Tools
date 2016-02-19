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
using System.Linq;
using s4pi.Interfaces;

namespace CASPartResource
{
    public class BoneResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version;
        BoneList bones;
        #endregion

        public BoneResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            version = br.ReadUInt32();

            string[] names = new string[br.ReadInt32()];
            for (int l = 0; l < names.Length; l++)
                names[l] = System.Text.BigEndianUnicodeString.Read(s);

            int i = br.ReadInt32();
            if (checking && i != names.Length)
                throw new InvalidDataException(String.Format("Unequal counts for bone names and matrices.  Bone name count {0}, matrix count {1}.  Position {2:X8}.",
                    names.Length, i, s.Position));

            Matrix4x3[] matrices = new Matrix4x3[i];
            for (int l = 0; l < matrices.Length; l++)
                matrices[l] = new Matrix4x3(0, null, s);

            bones = new BoneList(OnResourceChanged, names.Zip(matrices, (name, matrix) => new Bone(requestedApiVersion, null, name, matrix)));
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(s);

            bw.Write(version);

            if (bones == null) bones = new BoneList(OnResourceChanged);

            bw.Write(bones.Count);
            foreach (var bone in bones)
                System.Text.BigEndianUnicodeString.Write(s, bone.Name);

            bw.Write(bones.Count);
            foreach (var bone in bones)
                bone.InverseBindPose.UnParse(s);

            return s;
        }
        #endregion

        #region Sub-types
        public class MatrixRow : AHandlerElement, IEquatable<MatrixRow>
        {
            #region Attributes
            float x;
            float y;
            float z;
            #endregion

            #region Constructors
            public MatrixRow(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public MatrixRow(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            public MatrixRow(int apiVersion, EventHandler handler, MatrixRow basis) : this(apiVersion, handler, basis.x, basis.y, basis.z) { }
            public MatrixRow(int apiVersion, EventHandler handler, float x, float y, float z) : base(apiVersion, handler) { this.x = x; this.y = y; this.z = z; }
            #endregion

            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                x = br.ReadSingle();
                y = br.ReadSingle();
                z = br.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(x);
                bw.Write(y);
                bw.Write(z);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<MatrixRow> Members

            public bool Equals(MatrixRow other)
            {
                return
                    x.Equals(other.x)
                    && y.Equals(other.y)
                    && z.Equals(other.z)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as MatrixRow != null ? this.Equals(obj as MatrixRow) : false;
            }
            public override int GetHashCode()
            {
                return
                    x.GetHashCode()
                    ^ y.GetHashCode()
                    ^ z.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public float X { get { return x; } set { if (!x.Equals(value)) { x = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Y { get { return y; } set { if (!y.Equals(value)) { y = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float Z { get { return z; } set { if (!z.Equals(value)) { z = value; OnElementChanged(); } } }

            public string Value { get { return "{ " + ValueBuilder.Replace("\n", "; ") + " }"; } }
            #endregion
        }

        public class Matrix4x3 : AHandlerElement, IEquatable<Matrix4x3>
        {
            #region Attributes
            MatrixRow right;
            MatrixRow up;
            MatrixRow back;
            MatrixRow translate;
            #endregion

            #region Constructors
            public Matrix4x3(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public Matrix4x3(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            public Matrix4x3(int apiVersion, EventHandler handler, Matrix4x3 basis) : this(apiVersion, handler, basis.right, basis.up, basis.back, basis.translate) { }
            public Matrix4x3(int apiVersion, EventHandler handler, MatrixRow row1, MatrixRow row2, MatrixRow row3, MatrixRow row4) : base(apiVersion, handler)
            {
                this.right = new MatrixRow(requestedApiVersion, handler, row1);
                this.up = new MatrixRow(requestedApiVersion, handler, row2);
                this.back = new MatrixRow(requestedApiVersion, handler, row3);
                this.translate = new MatrixRow(requestedApiVersion, handler, row4);
            }
            #endregion

            #region Data I/O
            private void Parse(Stream s)
            {
                right = new MatrixRow(requestedApiVersion, handler, s);
                up = new MatrixRow(requestedApiVersion, handler, s);
                back = new MatrixRow(requestedApiVersion, handler, s);
                translate = new MatrixRow(requestedApiVersion, handler, s);
            }

            internal void UnParse(Stream s)
            {
                right.UnParse(s);
                up.UnParse(s);
                back.UnParse(s);
                translate.UnParse(s);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Matrix4x3> Members

            public bool Equals(Matrix4x3 other)
            {
                return
                    right.Equals(other.right)
                    && up.Equals(other.up)
                    && back.Equals(other.back)
                    && translate.Equals(other.translate)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as Matrix4x3 != null ? this.Equals(obj as Matrix4x3) : false;
            }
            public override int GetHashCode()
            {
                return
                    right.GetHashCode()
                    ^ up.GetHashCode()
                    ^ back.GetHashCode()
                    ^ translate.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public MatrixRow Right { get { return right; } set { if (!right.Equals(value)) { right =new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public MatrixRow Up { get { return up; } set { if (!up.Equals(value)) { up = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(3)]
            public MatrixRow Back { get { return back; } set { if (!back.Equals(value)) { back = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(4)]
            public MatrixRow Translate { get { return translate; } set { if (!translate.Equals(value)) { translate = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }

        public class Bone : AHandlerElement, IEquatable<Bone>
        {
            #region Attributes
            string name;
            Matrix4x3 inverseBindPose;
            #endregion

            #region Constructors
            public Bone(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public Bone(int apiVersion, EventHandler handler, Bone basis) : this(apiVersion, handler, basis.name, basis.inverseBindPose) { }
            public Bone(int apiVersion, EventHandler handler, string name, Matrix4x3 inverseBindPose) : base(apiVersion, handler) { this.name = name; this.inverseBindPose = new Matrix4x3(requestedApiVersion, handler, inverseBindPose); }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Bone> Members

            public bool Equals(Bone other)
            {
                return
                    name.Equals(other.name)
                    && inverseBindPose.Equals(other.inverseBindPose)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as MatrixRow != null ? this.Equals(obj as MatrixRow) : false;
            }
            public override int GetHashCode()
            {
                return
                    name.GetHashCode()
                    ^ inverseBindPose.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public string Name { get { return name; } set { if (!name.Equals(value)) { name = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public Matrix4x3 InverseBindPose { get { return inverseBindPose; } set { if (!inverseBindPose.Equals(value)) { inverseBindPose = new Matrix4x3(requestedApiVersion, handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }

        public class BoneList : DependentList<Bone>
        {
            #region Constructors
            public BoneList(EventHandler handler) : base(handler) { }
            public BoneList(EventHandler handler, IEnumerable<Bone> le) : base(handler, le) { }
            #endregion

            #region Data I/O (or not)
            protected override int ReadCount(Stream s) { throw new InvalidOperationException("BoneList cannot be automatically parsed."); }
            protected override Bone CreateElement(Stream s) { throw new InvalidOperationException("BoneList cannot be automatically parsed."); }
            protected override void WriteCount(Stream s, int count) { throw new InvalidOperationException("BoneList cannot be automatically un-parsed."); }
            protected override void WriteElement(Stream s, Bone element) { throw new InvalidOperationException("BoneList cannot be automatically un-parsed."); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (!version.Equals(value)) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public BoneList Bones { get { return bones; } set { if (!bones.Equals(value)) { bones = new BoneList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for BlendGeometryResource wrapper
    /// </summary>
    public class BoneResourceResourceHandler : AResourceHandler
    {
        public BoneResourceResourceHandler()
        {
            this.Add(typeof(BoneResource), new List<string>(new string[] { "0x00AE6C67" }));
        }
    }
}
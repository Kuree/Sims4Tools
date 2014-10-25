using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using meshExpImp.ModelBlocks;
using s4pi.GenericRCOLResource;
using s4pi.Interfaces;
using Vertex = meshExpImp.ModelBlocks.Vertex;

namespace s3piwrappers.ModelViewer
{
    public partial class MainWindow : Window
    {
        private class SceneGeostate
        {
            public SceneGeostate(SceneMesh owner, MLOD.GeometryState state, GeometryModel3D model)
            {
                Owner = owner;
                State = state;
                Model = model;
            }

            public SceneMesh Owner { get; set; }
            public MLOD.GeometryState State { get; set; }
            public GeometryModel3D Model { get; set; }

            public override string ToString()
            {
                if (State == null)
                {
                    return "None";
                }
                else
                {
                    string stateName = "0x" + State.Name.ToString("X8");
                    if (GeostateDictionary.ContainsKey(State.Name))
                    {
                        stateName = GeostateDictionary[State.Name];
                    }
                    return stateName;
                }
            }
        }

        private class SceneMesh
        {
            public SceneMesh(GeometryModel3D model)
            {
                Model = model;
                States = new SceneGeostate[0];
            }

            public SceneGeostate[] States { get; set; }
            public SceneGeostate SelectedState { get; set; }

            public GeometryModel3D Model { get; set; }
            public ShaderType Shader { get; set; }
        }

        private class SceneMlodMesh : SceneMesh
        {
            public SceneMlodMesh(MLOD.Mesh mesh, GeometryModel3D model)
                : base(model)
            {
                Mesh = mesh;
            }

            public MLOD.Mesh Mesh { get; set; }

            public override string ToString()
            {
                string meshName = "0x" + Mesh.Name.ToString("X8");
                if (MeshDictionary.ContainsKey(Mesh.Name))
                {
                    meshName = MeshDictionary[Mesh.Name];
                }
                return meshName;
            }
        }

        private class SceneGeomMesh : SceneMesh
        {
            public SceneGeomMesh(GEOM mesh, GeometryModel3D model)
                : base(model)
            {
                Mesh = mesh;
            }

            public GEOM Mesh { get; set; }

            public override string ToString()
            {
                return "GEOM Mesh";
            }
        }

        private readonly List<SceneMesh> mSceneMeshes;
        private SceneMesh mSelectedMesh;
        private readonly GenericRCOLResource rcol;
        private readonly Material mHiddenMaterial = new DiffuseMaterial();
        private readonly MaterialGroup mNonSelectedMaterial = new MaterialGroup();
        private readonly MaterialGroup mSelectedMaterial = new MaterialGroup();
        private Material mXrayMaterial;
        private readonly Material mCheckerMaterial;
        private Material mTexturedMaterial;
        private readonly MaterialGroup mGlassMaterial = new MaterialGroup();
        private readonly Material mShadowMapMaterial;
        private readonly ImageBrush mCheckerBrush;

        public String TextureSource
        {
            set
            {
                if (File.Exists(value))
                {
                    try
                    {
                        var bmp = new BitmapImage();
                        var memstream = new MemoryStream();
                        using (var fs = File.OpenRead(value))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            memstream.Write(buffer,0, buffer.Length);
                        }
                        bmp.BeginInit();
                        bmp.StreamSource = memstream;
                        bmp.EndInit();
                        var imgBrush = new ImageBrush(bmp);
                        mTexturedMaterial = new DiffuseMaterial(imgBrush);
                        imgBrush.Stretch = Stretch.Fill;
                        imgBrush.TileMode = TileMode.Tile;
                        imgBrush.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
                        imgBrush.ViewportUnits = BrushMappingMode.Absolute;
                        rbTextured.Visibility = Visibility.Visible;
                        rbTextured.IsChecked = true;
                    }
                    catch (Exception ex)
                    {
                        mTexturedMaterial = null;
                        MessageBox.Show("Unable to load texture " + value);
                    }
                }
                else
                {
                    mTexturedMaterial = null;
                    rbTextured.Visibility = Visibility.Collapsed;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            mSceneMeshes = new List<SceneMesh>();

            mNonSelectedMaterial.Children.Add(new DiffuseMaterial(Brushes.LightGray));
            mNonSelectedMaterial.Children.Add(new SpecularMaterial(Brushes.GhostWhite, 20d));
            mSelectedMaterial.Children.Add(new DiffuseMaterial(Brushes.Red));
            mSelectedMaterial.Children.Add(new SpecularMaterial(Brushes.Red, 40d));
            mXrayMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromScRgb(0.4f, 1f, 0f, 0f)));


            mCheckerBrush = new ImageBrush
                {
                    Stretch = Stretch.Fill,
                    TileMode = TileMode.Tile,
                    ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                    ViewportUnits = BrushMappingMode.Absolute
                };

            mCheckerBrush.ImageSource = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(typeof (MainWindow).Assembly.Location), "checkers.png")));
            mCheckerMaterial = new DiffuseMaterial(mCheckerBrush);


            mGlassMaterial.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromScRgb(0.6f, .9f, .9f, 1f))));
            mGlassMaterial.Children.Add(new SpecularMaterial(Brushes.White, 100d));

            var shadowBrush = new ImageBrush
                {
                    Stretch = Stretch.Fill,
                    TileMode = TileMode.Tile,
                    ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Transform = new ScaleTransform(1, 1)
                };

            try
            {
                shadowBrush.ImageSource = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(typeof (MainWindow).Assembly.Location), "dropShadow.png")));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to load DropShadow.");
            }
            mShadowMapMaterial = new DiffuseMaterial(shadowBrush);
            GeostateDictionary = LoadDictionary("Geostates");
            MeshDictionary = LoadDictionary("MeshNames");
            Help.Text =
                @"
Zoom:
1. Mouse Wheel
2. Ctrl+RMB+Drag
3. PageUp/PageDown

Pan:
1. MouseWheel+Drag
2. Shift+RMB+Drag
3. Shift+Arrow Keys

Rotate:
1. RMB+Drag
2. Arrow Keys
";
        }

        public MainWindow(GenericRCOLResource s)
            : this()
        {
            rcol = s;
            rbTextured.Visibility = Visibility.Collapsed;
            InitScene();
            rbSolid.IsChecked = true;
        }

        public static Dictionary<uint, string> GeostateDictionary;
        public static Dictionary<uint, string> MeshDictionary;

        private static Dictionary<uint, string> LoadDictionary(string name)
        {
            var dict = new Dictionary<uint, string>();
            string geostatePath = Path.Combine(Path.GetDirectoryName(typeof (App).Assembly.Location), name + ".txt");
            if (File.Exists(geostatePath))
            {
                using (var sr = new StreamReader(File.OpenRead(geostatePath)))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Contains("#"))
                        {
                            dict[FNV32.GetHash(line)] = line;
                        }
                    }
                }
            }
            return dict;
        }

        private void InitScene()
        {
            GeostatesPanel.Visibility = Visibility.Collapsed;
            GenericRCOLResource.ChunkEntry chunk = rcol.ChunkEntries.FirstOrDefault(x => x.RCOLBlock is MLOD);

            int polyCount = 0;
            int vertCount = 0;


            if (chunk != null)
            {
                var mlod = chunk.RCOLBlock as MLOD;
                foreach (MLOD.Mesh m in mlod.Meshes)
                {
                    try
                    {
                        vertCount += m.VertexCount;
                        polyCount += m.PrimitiveCount;
                        var vbuf = (VBUF)GenericRCOLResource.ChunkReference.GetBlock(rcol, m.VertexBufferIndex);
                        var ibuf = (IBUF)GenericRCOLResource.ChunkReference.GetBlock(rcol, m.IndexBufferIndex);
                        VRTF vrtf = (VRTF)GenericRCOLResource.ChunkReference.GetBlock(rcol, m.VertexFormatIndex) ?? VRTF.CreateDefaultForMesh(m);
                        IRCOLBlock material = GenericRCOLResource.ChunkReference.GetBlock(rcol, m.MaterialIndex);

                        MATD matd = FindMainMATD(rcol, material);

                        float[] uvscale = GetUvScales(matd);
                        if (uvscale != null)
                            Debug.WriteLine(string.Format("{0} - {1} - {2}", uvscale[0], uvscale[2], uvscale[2]));
                        else
                            Debug.WriteLine("No scales");

                        GeometryModel3D model = DrawModel(vbuf.GetVertices(m, vrtf, uvscale), ibuf.GetIndices(m), mNonSelectedMaterial);

                        var sceneMesh = new SceneMlodMesh(m, model);
                        if (matd != null)
                        {
                            sceneMesh.Shader = matd.Shader;
                            switch (matd.Shader)
                            {
                                case ShaderType.ShadowMap:
                                case ShaderType.DropShadow:
                                    break;
                                default:
                                    var maskWidth = GetMATDParam<ElementInt>(matd, FieldType.MaskWidth);
                                    var maskHeight = GetMATDParam<ElementInt>(matd, FieldType.MaskHeight);
                                    if (maskWidth != null && maskHeight != null)
                                    {
                                        float scalar = Math.Max(maskWidth.Data, maskHeight.Data);
                                        mCheckerBrush.Transform = new ScaleTransform(maskHeight.Data / scalar, maskWidth.Data / scalar);
                                    }
                                    break;
                            }
                        }
                        try
                        {
                            var sceneGeostates = new SceneGeostate[m.GeometryStates.Count];
                            for (int i = 0; i < sceneGeostates.Length; i++)
                            {
                                GeometryModel3D state = DrawModel(vbuf.GetVertices(m, vrtf, m.GeometryStates[i], uvscale),
                                                                  ibuf.GetIndices(m, m.GeometryStates[i]), mHiddenMaterial);
                                mGroupMeshes.Children.Add(state);
                                sceneGeostates[i] = new SceneGeostate(sceneMesh, m.GeometryStates[i], state);
                            }
                            sceneMesh.States = sceneGeostates;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Unable to load Geostates.  You may have some corrupted data: " + ex.ToString(),
                                            "Unable to load Geostates...");
                        }
                        mGroupMeshes.Children.Add(model);
                        mSceneMeshes.Add(sceneMesh);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show( String.Format("Unable to load mesh id 0x{0:X8}",m.Name));
                    }
                }
            }
            else
            {
                GenericRCOLResource.ChunkEntry geomChunk = rcol.ChunkEntries.FirstOrDefault();
                var geom = new GEOM(0, null, geomChunk.RCOLBlock.Stream);
                var verts = new List<Vertex>();
                polyCount = geom.Faces.Count;
                vertCount = geom.VertexData.Count;
                foreach (GEOM.VertexDataElement vd in geom.VertexData)
                {
                    var v = new Vertex();

                    var pos = (GEOM.PositionElement) vd.Vertex.FirstOrDefault(e => e is GEOM.PositionElement);
                    if (pos != null)
                    {
                        v.Position = new[] {pos.X, pos.Y, pos.Z};
                    }


                    var norm = (GEOM.NormalElement) vd.Vertex.FirstOrDefault(e => e is GEOM.NormalElement);
                    if (norm != null)
                    {
                        v.Normal = new[] {norm.X, norm.Y, norm.Z};
                    }


                    var uv = (GEOM.UVElement) vd.Vertex.FirstOrDefault(e => e is GEOM.UVElement);
                    if (uv != null)
                    {
                        v.UV = new[] {new[] {uv.U, uv.V}};
                    }
                    verts.Add(v);
                }
                var facepoints = new List<int>();
                foreach (GEOM.Face face in geom.Faces)
                {
                    facepoints.Add(face.VertexDataIndex0);
                    facepoints.Add(face.VertexDataIndex1);
                    facepoints.Add(face.VertexDataIndex2);
                }

                GeometryModel3D model = DrawModel(verts.ToArray(), facepoints.ToArray(), mNonSelectedMaterial);
                var sceneMesh = new SceneGeomMesh(geom, model);
                mGroupMeshes.Children.Add(model);
                mSceneMeshes.Add(sceneMesh);
            }
            foreach (SceneMesh s in mSceneMeshes)
            {
                mMeshListView.Items.Add(s);
            }
            if (mSceneMeshes.Count <= 1)
            {
                MeshesPanel.Visibility = Visibility.Collapsed;
            }
            VertexCount.Text = String.Format("Vertices: {0}", vertCount);
            PolygonCount.Text = String.Format("Polygons: {0}", polyCount);
        }

        private static MATD FindMainMATD(GenericRCOLResource rcol, IRCOLBlock material)
        {
            float[] scales = null;
            if (material == null) return null;
            if (material is MATD)
            {
                return material as MATD;
            }
            else if (material is MTST)
            {
                var mtst = material as MTST;
                try
                {
                    material = GenericRCOLResource.ChunkReference.GetBlock(rcol, mtst.Index);
                }
                catch (NotImplementedException e)
                {
                    MessageBox.Show("Material is external, unable to locate UV scales.");
                    return null;
                }


                if (material is MATD)
                {
                    var matd = (MATD) material;
                    return matd;
                }
            }
            else
            {
                throw new ArgumentException("Material must be of type MATD or MTST", "material");
            }

            return null;
        }

        private static T GetMATDParam<T>(MATD matd, FieldType type) where T : class
        {
            return matd == null ? null : (matd.Mtrl != null ? matd.Mtrl.SData : matd.Mtrl.SData).FirstOrDefault(x => x.Field == type) as T;
        }

        private static float[] GetUvScales(MATD matd)
        {
            var param = GetMATDParam<ElementFloat3>(matd, FieldType.UVScales);
            return param != null ? new[] {param.Data0, param.Data1, param.Data2} : new[] {1f/short.MaxValue, 1f/short.MaxValue, 1f/short.MaxValue};
        }

        private static GeometryModel3D DrawModel(Vertex[] verts, Int32[] indices, Material material)
        {
            var mesh = new MeshGeometry3D();
            for (int k = 0; k < verts.Length; k++)
            {
                Vertex v = verts[k];

                if (v.Position != null) mesh.Positions.Add(new Point3D(v.Position[0], -v.Position[2], v.Position[1]));
                if (v.Normal != null) mesh.Normals.Add(new Vector3D(v.Normal[0], v.Normal[1], v.Normal[2]));
                if (v.UV != null && v.UV.Length > 0) mesh.TextureCoordinates.Add(new Point(v.UV[0][0], v.UV[0][1]));
            }
            for (int i = 0; i < indices.Length; i++)
            {
                mesh.TriangleIndices.Add(indices[i]);
            }
            return new GeometryModel3D(mesh, material);
        }

        private void mMeshListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mStateListView.Items.Clear();
            SceneMesh m;
            if (e.AddedItems.Count > 0)
            {
                m = (SceneMesh) e.AddedItems[0];
                mSelectedMesh = m;
            }
            else
            {
                mSelectedMesh = null;
            }
            m = mSelectedMesh;
            GeostatesPanel.Visibility = m == null || m.States.Count() == 0 ? Visibility.Collapsed : Visibility.Visible;
            if (m != null)
            {
                mStateListView.Items.Add(new SceneGeostate(m, null, null));
                foreach (SceneGeostate s in m.States)
                {
                    mStateListView.Items.Add(s);
                }
                mStateListView.SelectedIndex = 0;
            }
            else
            {
                GeostatesPanel.Visibility = Visibility.Collapsed;
            }
            UpdateMaterials();
        }

        private void UpdateMaterials()
        {
            foreach (SceneMesh sceneMesh in mSceneMeshes)
            {
                Material meshMaterial = null;
                switch (mDrawMode)
                {
                case "Solid":
                    meshMaterial = mSelectedMesh == sceneMesh ? mSelectedMaterial : mNonSelectedMaterial;
                    break;
                case "Textured":
                    switch (sceneMesh.Shader)
                    {
                    case ShaderType.GlassForFences:
                    case ShaderType.GlassForObjects:
                    case ShaderType.GlassForObjectsTranslucent:
                    case ShaderType.GlassForPortals:
                    case ShaderType.GlassForRabbitHoles:
                        meshMaterial = mGlassMaterial;
                        break;
                    case ShaderType.ShadowMap:
                    case ShaderType.DropShadow:
                        meshMaterial = mShadowMapMaterial;
                        break;
                    default:
                        meshMaterial = mTexturedMaterial;
                        break;
                    }
                    break;
                case "UV":
                    switch (sceneMesh.Shader)
                    {
                    case ShaderType.ShadowMap:
                    case ShaderType.DropShadow:
                        meshMaterial = mShadowMapMaterial;
                        break;
                    default:
                        meshMaterial = mCheckerMaterial;
                        break;
                    }
                    break;
                }
                if (sceneMesh.SelectedState != null && sceneMesh.SelectedState.Model != null)
                {
                    sceneMesh.Model.Material = mHiddenMaterial;
                    sceneMesh.SelectedState.Model.Material = meshMaterial;
                }
                else
                {
                    sceneMesh.Model.Material = meshMaterial;
                }
                foreach (SceneGeostate sceneState in sceneMesh.States)
                {
                    if (sceneState != sceneMesh.SelectedState)
                    {
                        sceneState.Model.Material = mHiddenMaterial;
                    }
                }
            }
        }

        private void mStateListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var s = (SceneGeostate) e.AddedItems[0];
                mSelectedMesh.SelectedState = s;
            }
            UpdateMaterials();
        }

        private string mDrawMode;

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton) sender;
            mDrawMode = rb.Content.ToString();
            UpdateMaterials();
        }
    }
}

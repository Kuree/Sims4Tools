using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using s4pi.ImageResource;

namespace System.Windows.Forms
{
    /// <summary>
    /// Displays and manipulates a DDS image
    /// </summary>
    public partial class DDSPanel : UserControl
    {
        #region Attributes
        DdsFile ddsFile = new DdsFile();
        bool loaded = false;
        bool supportHSV = false;
        Image image;

        bool fit = false;
        Size maxSize = new Size(Size.Empty.Width, Size.Empty.Height);

        DateTime now = DateTime.UtcNow;
        HSVShift hsvShift;
        DdsFile ddsMask = null;
        #endregion

        /// <summary>
        /// Displays and manipulates a DDS image
        /// </summary>
        public DDSPanel()
        {
            InitializeComponent();
            tlpSize.Visible = false;
        }

        #region Properties
        /// <summary>
        /// When true, the image will resize to the control bounds.
        /// </summary>
        [DefaultValue(false), Description("Set to true to have the image resize to the control bounds")]
        public bool Fit { get { return fit; } set { fit = value; pictureBox1.Image = doResize(); OnChanged(FitChanged); } }

        /// <summary>
        /// When non-zero, indicates the maximum width and height to constrain the image size.
        /// </summary>
        [DefaultValue(typeof(Size), "0, 0"), Description("Set non-zero bounds to constrain the image size")]
        public Size MaxSize { get { return maxSize; } set { maxSize = value; pictureBox1.Image = doResize(); OnChanged(MaxSizeChanged); } }

        /// <summary>
        /// The "Red" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 1 as Red")]
        public bool Channel1 { get { return ckbR.Checked; } set { ckbR.Checked = value; OnChanged(Channel1Changed); } }
        /// <summary>
        /// The "Green" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 2 as Green")]
        public bool Channel2 { get { return ckbG.Checked; } set { ckbG.Checked = value; OnChanged(Channel2Changed); } }
        /// <summary>
        /// The "Blue" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 3 as Blue")]
        public bool Channel3 { get { return ckbB.Checked; } set { ckbB.Checked = value; OnChanged(Channel3Changed); } }
        /// <summary>
        /// The "Alpha" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(true), Description("Display channel 4 as Alpha")]
        public bool Channel4 { get { return ckbA.Checked; } set { ckbA.Checked = value; OnChanged(Channel4Changed); } }
        /// <summary>
        /// The "Invert" checkbox Checked state.
        /// </summary>
        /// <remarks>The DDS Image is retrieved using the Channel1 to 4 and InvertCh4 values.</remarks>
        [DefaultValue(false), Description("Invert channel 4 values")]
        public bool InvertCh4 { get { return ckbI.Checked; } set { ckbI.Checked = value; OnChanged(InvertCh4Changed); } }

        /// <summary>
        /// When true, the Channel Selection checkboxes will be displayed above the image.
        /// </summary>
        [DefaultValue(true), Description("Show the Channel Selection check boxes")]
        public bool ShowChannelSelector { get { return flowLayoutPanel1.Visible; } set { flowLayoutPanel1.Visible = value; pictureBox1.Image = doResize(); OnChanged(ShowChannelSelectorChanged); } }

        /// <summary>
        /// When true, enables use of HSV-related methods.
        /// </summary>
        /// <remarks>Requires an increase in stored data whilst true.</remarks>
        [DefaultValue(false), Description("Enables use of HSV-related methods.  Requires an increase in stored data whilst true.")]
        public bool SupportsHSV
        {
            get { return loaded && ddsFile.SupportsHSV; }
            set
            {
                if (!loaded) return;

                if (ddsFile.SupportsHSV != value)
                {
                    ddsFile.SupportsHSV = value;
                    ckb_CheckedChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Hue shift to be applied to the image.
        /// </summary>
        /// <remarks>Only effective when HSV processing is enabled.</remarks>
        [DefaultValue(0f), Description("Hue shift to be applied to the image, when HSV processing is enabled.")]
        public float HueShift
        {
            get { return hsvShift.h; }
            set { if (hsvShift.h != value) { hsvShift.h = value; if (SupportsHSV) ckb_CheckedChanged(null, null); } }
        }

        /// <summary>
        /// Saturation shift to be applied to the image.
        /// </summary>
        /// <remarks>Only effective when HSV processing is enabled.</remarks>
        [DefaultValue(0f), Description("Saturation shift to be applied to the image, when HSV processing is enabled.")]
        public float SaturationShift
        {
            get { return hsvShift.s; }
            set { if (hsvShift.s != value) { hsvShift.s = value; if (SupportsHSV) ckb_CheckedChanged(null, null); } }
        }

        /// <summary>
        /// Value shift to be applied to the image.
        /// </summary>
        /// <remarks>Only effective when HSV processing is enabled.</remarks>
        [DefaultValue(0f), Description("Value shift to be applied to the image, when HSV processing is enabled.")]
        public float ValueShift
        {
            get { return hsvShift.v; }
            set { if (hsvShift.v != value) { hsvShift.v = value; if (SupportsHSV) ckb_CheckedChanged(null, null); } }
        }

        /// <summary>
        /// Returns the image that is displayed by the DDSPanel.
        /// </summary>
        [ReadOnly(true), Description("Returns the image that is displayed by the DDSPanel")]
        public Image Image { get { return pictureBox1.Image; } }

        /// <summary>
        /// The size of the current image (or <see cref="Size.Empty"/> if not loaded).
        /// </summary>
        [ReadOnly(true), Description("The size of the current image (or Size.Empty if not loaded).")]
        public Size ImageSize
        {
            get
            {
                return loaded ? ddsFile.Size : Size.Empty;
            }
            set
            {
                if (!loaded) return;
                if (ImageSize == value) return;

                try
                {
                    this.Enabled = false;
                    Application.UseWaitCursor = true;
                    Application.DoEvents();
                    ddsFile = ddsFile.Resize(value);
                }
                finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }

                ckb_CheckedChanged(null, null);
            }
        }

        /// <summary>
        /// The number of alpha channel bits per pixel in the encoded DDS image or the DXT compression mode.
        /// </summary>
        [DefaultValue(8), Description("The number of alpha channel bits per pixel in the encoded DDS image or the DXT compression mode.")]
        public int AlphaDepth { get { return loaded ? ddsFile.AlphaDepth : -1; } set { if (loaded) { ddsFile.AlphaDepth = value; pictureBox1.Image = doResize(); } } }

        /// <summary>
        /// If true, use DXT-type image compression for storage.  The mode will depend on the <see cref="AlphaDepth"/>.
        /// </summary>
        /// <remarks>
        /// Setting to false (from true) will default to A8B8G8R8 format.
        /// </remarks>
        [DefaultValue(true), Description("If true, use DXT-type image compression for storage.  The mode will depend on the AlphaDepth.")]
        public bool UseDXT { get { return loaded && ddsFile.UseDXT; } set { if (loaded) { ddsFile.UseDXT = value; pictureBox1.Image = doResize(); } } }

        /// <summary>
        /// If true, treat the image as a luminance (plus alpha) map for storage.
        /// </summary>
        /// <remarks>
        /// Currently only A8L8, 16bit coding is supported.
        /// Setting to false (from true) will default to A8B8G8R8 (non-DXT) format.
        /// Setting to true does not turn the image into a greyscale.
        /// This only happens on saving the image (and will not affect the displayed values until they are read back in).
        /// </remarks>
        [DefaultValue(true), Description("If true, treat the image as a luminance (plus alpha) map for storage.")]
        public bool UseLuminance { get { return loaded && ddsFile.UseLuminance; } set { if (loaded) { ddsFile.UseLuminance = value; pictureBox1.Image = doResize(); } } }

        /// <summary>
        /// Indicates that a Mask is currently loaded.
        /// </summary>
        [ReadOnly(true), Description("Indicates that a Mask is currently loaded.")]
        public bool MaskLoaded { get { return ddsMask != null; } }

        /// <summary>
        /// The size of the current mask (or <see cref="Size.Empty"/> if no mask loaded).
        /// </summary>
        [ReadOnly(true), Description("The size of the current mask (or Size.Empty if no mask loaded).")]
        public Size MaskSize { get { return MaskLoaded ? ddsMask.Size : Size.Empty; } }
        #endregion

        #region Events
        /// <summary>
        /// Raised to indicate Fit value changed.
        /// </summary>
        [Description("Raised to indicate Fit value changed")]
        public event EventHandler FitChanged;
        /// <summary>
        /// Raised to indicate MaxSize value changed.
        /// </summary>
        [Description("Raised to indicate MaxSize value changed")]
        public event EventHandler MaxSizeChanged;
        /// <summary>
        /// Raised to indicate Channel1 value changed.
        /// </summary>
        [Description("Raised to indicate Channel1 value changed")]
        public event EventHandler Channel1Changed;
        /// <summary>
        /// Raised to indicate Channel2 value changed.
        /// </summary>
        [Description("Raised to indicate Channel2 value changed")]
        public event EventHandler Channel2Changed;
        /// <summary>
        /// Raised to indicate Channel3 value changed.
        /// </summary>
        [Description("Raised to indicate Channel3 value changed")]
        public event EventHandler Channel3Changed;
        /// <summary>
        /// Raised to indicate Channel4 value changed.
        /// </summary>
        [Description("Raised to indicate Channel4 value changed")]
        public event EventHandler Channel4Changed;
        /// <summary>
        /// Raised to indicate InvertCh4 value changed.
        /// </summary>
        [Description("Raised to indicate InvertCh4 value changed")]
        public event EventHandler InvertCh4Changed;
        /// <summary>
        /// Raised to indicate ShowChannelSelector value changed.
        /// </summary>
        [Description("Raised to indicate ShowChannelSelector value changed")]
        public event EventHandler ShowChannelSelectorChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Load a DDS image from a <see cref="System.IO.Stream"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="stream">The <see cref="System.IO.Stream"/> containing the DDS image to display,<br/>
        /// - or -<br/>
        /// <c>null</c> to clear the image and free resources.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void DDSLoad(Stream stream, bool supportHSV = false)
        {
            if (stream != null)
            {
                try
                {
                    this.Enabled = false;
                    Application.UseWaitCursor = true;
                    Application.DoEvents();

                    // Load RLE Image
                    BinaryReader r = new BinaryReader(stream);
                    string header = new string(r.ReadChars(7));
                    if (header.Substring(0, 3) == "DXT" && header.Substring(4, 3) == "RLE")
                    {
                        stream.Position = 0;
                        RLEResource rle = new RLEResource(1, stream);
                        ddsFile.Load(rle.ToDDS(), supportHSV);
                    }
                    else
                    {
                        stream.Position = 0;
                        ddsFile.Load(stream, supportHSV);
                    }
                    loaded = true;
                }
                finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }
                this.supportHSV = supportHSV;
                ckb_CheckedChanged(null, null);
            }
            else
                Clear();
        }

        /// <summary>
        /// Sets the DDS image for this <see cref="DDSPanel"/> from the given <paramref name="ddsfile"/>.
        /// <see cref="SupportsHSV"/> is determined from the <see cref="DdsFile.SupportsHSV"/> value.
        /// </summary>
        /// <param name="ddsfile">A <see cref="DdsFile"/> to display in this <see cref="DDSPanel"/>.</param>
        public void DDSLoad(DdsFile ddsfile)
        {
            this.ddsFile = ddsfile;
            loaded = true;
            this.supportHSV = ddsfile.SupportsHSV;
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Sets the DDS image for this <see cref="DDSPanel"/> from the given <paramref name="image"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="image">A <see cref="Bitmap"/> to display in this <see cref="DDSPanel"/>.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void DDSLoad(Bitmap image, bool supportHSV = false)
        {
            ddsFile.CreateImage(image, supportHSV);
            loaded = true;
            this.supportHSV = supportHSV;
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Imports an <see cref="System.Drawing.Image"/> from the specified file using embedded color
        /// management information in that file and uses the file as the DDS image to work on;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="filename">A string that contains the name of the file from which to read the <see cref="System.Drawing.Image"/>.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        /// <exception cref="System.OutOfMemoryException">
        /// The file does not have a valid image format.<br/>
        /// -or-<br/>
        /// GDI+ does not support the pixel format of the file.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">The specified file does not exist.</exception>
        /// <exception cref="System.ArgumentException">filename is a System.Uri.</exception>
        public void Import(string filename, bool supportHSV = false)
        {
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                Image import = Image.FromFile(filename, true);
                ddsFile.CreateImage(import, supportHSV);
                loaded = true;
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }
            this.supportHSV = supportHSV;
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Saves the current image to a DDS in <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public void DDSSave(Stream stream)
        {
            if (stream != null)
                this.ddsFile.Save(stream);
        }

        /// <summary>
        /// Sets the DDSPanel to an unloaded state, freeing resources.
        /// </summary>
        public void Clear()
        {
            ddsFile = new DdsFile();
            loaded = false;
            ddsMask = null;
            pictureBox1.Image = image = null;
            pictureBox1.Size = (this.MaxSize == Size.Empty) ? new Size(0x80, 0x80) : Min(new Size(0x80, 0x80), this.MaxSize);
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Get a copy of the current DDS image as a <see cref="DdsFile"/>.
        /// </summary>
        /// <returns>A new <see cref="DdsFile"/> copy of the current DDS image.</returns>
        public DdsFile GetDdsFile()
        {
            if (!this.loaded || ddsFile == null) return null;

            DdsFile res = new DdsFile();
            res.CreateImage(ddsFile, false);
            return res;
        }

        /// <summary>
        /// Creates an image of a specified <paramref name="colour"/> and <paramref name="size"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="colour">ARGB colour.  If <c>null</c>, method returns with no action.</param>
        /// <param name="size">Size of image.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void CreateImage(uint? colour, Size size, bool supportHSV = false)
        {
            CreateImage(colour, size.Width, size.Height, supportHSV);
        }

        /// <summary>
        /// Creates an image of a specified <paramref name="colour"/> and given
        /// <paramref name="width"/> and <paramref name="height"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="colour">ARGB colour.  If <c>null</c>, method returns with no action.</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void CreateImage(uint? colour, int width, int height, bool supportHSV = false)
        {
            if (!colour.HasValue) return;
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                ddsFile.CreateImage(colour.Value, width, height, supportHSV);
                loaded = true;
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }

            if (!loaded) return;
            this.supportHSV = supportHSV;
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Creates an image of a specified colour (with alpha set to full on) and <paramref name="size"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="red">Amount of red per pixel.</param>
        /// <param name="green">Amount of green per pixel.</param>
        /// <param name="blue">Amount of blue per pixel.</param>
        /// <param name="size">Size of image.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void CreateImage(byte red, byte green, byte blue, Size size, bool supportHSV = false)
        {
            CreateImage(red, green, blue, 255, size.Width, size.Height, supportHSV);
        }

        /// <summary>
        /// Creates an image of a specified colour (with alpha set to full on) and given
        /// <paramref name="width"/> and <paramref name="height"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="red">Amount of red per pixel.</param>
        /// <param name="green">Amount of green per pixel.</param>
        /// <param name="blue">Amount of blue per pixel.</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void CreateImage(byte red, byte green, byte blue, int width, int height, bool supportHSV = false)
        {
            CreateImage(red, green, blue, 255, width, height, supportHSV);
        }

        /// <summary>
        /// Creates an image of a specified colour and <paramref name="size"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="red">Amount of red per pixel.</param>
        /// <param name="green">Amount of green per pixel.</param>
        /// <param name="blue">Amount of blue per pixel.</param>
        /// <param name="alpha">Amount of alpha per pixel.</param>
        /// <param name="size">Size of image.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void CreateImage(byte red, byte green, byte blue, byte alpha, Size size, bool supportHSV = false)
        {
            CreateImage(red, green, blue, alpha, size.Width, size.Height, supportHSV);
        }

        /// <summary>
        /// Creates an image of a specified colour and given
        /// <paramref name="width"/> and <paramref name="height"/>;
        /// if <paramref name="supportHSV"/> is passed and true (default is false), the image will
        /// support HSV shift operations.
        /// </summary>
        /// <param name="red">Amount of red per pixel.</param>
        /// <param name="green">Amount of green per pixel.</param>
        /// <param name="blue">Amount of blue per pixel.</param>
        /// <param name="alpha">Amount of alpha per pixel.</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        /// <param name="supportHSV">Optional; when true, HSV operations will be supported on the image.</param>
        public void CreateImage(byte red, byte green, byte blue, byte alpha, int width, int height, bool supportHSV = false)
        {
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                ddsFile.CreateImage(red, green, blue, alpha, width, height, supportHSV);
                loaded = true;
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }

            if (!loaded) return;
            this.supportHSV = supportHSV;
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Apply a hue, saturation and value shift to the image.
        /// </summary>
        /// <param name="h">Hue shift, default 0</param>
        /// <param name="s">Saturation shift, default 0</param>
        /// <param name="v">Value shift, default 0</param>
        public void HSVShift(decimal h = 0, decimal s = 0, decimal v = 0)
        {
            hsvShift = new HSVShift { h = (float)h, s = (float)s, v = (float)v, };
            if (SupportsHSV) ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Set the alpha channel of the current image from the given DDS file stream.
        /// </summary>
        /// <param name="image"><see cref="Stream"/> containing a DDS image.</param>
        public void SetAlphaFromGreyscale(Stream image)
        {
            DdsFile greyscale = new DdsFile();
            greyscale.Load(image, false);
            ddsFile.SetAlphaFromGreyscale(greyscale);
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Set the alpha channel of the current image from the given <paramref name="greyscale"/>.
        /// </summary>
        /// <param name="greyscale"><see cref="Image"/> to use to set alpha channel.</param>
        public void SetAlphaFromGreyscale(Image greyscale)
        {
            try
            {
                base.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                if (greyscale.Width != image.Width || greyscale.Height != image.Height)
                    greyscale = greyscale.GetThumbnailImage(image.Width, image.Height, () => false, System.IntPtr.Zero);
                ddsFile.SetAlphaFromGreyscale(greyscale);
            }
            finally { base.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }
            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Set the alpha channel of the current image from the given <paramref name="greyscale"/>.
        /// </summary>
        /// <param name="greyscale"><see cref="Bitmap"/> to use to set alpha channel.</param>
        public void SetAlphaFromGreyscale(Bitmap greyscale)
        {
            SetAlphaFromGreyscale(greyscale as Image);
        }

        /// <summary>
        /// Converts the alpha channel of the currently loaded DDS image into a greyscale bitmap.
        /// </summary>
        /// <returns>A greyscale bitmap representing the current DDS image alpha channel.</returns>
        public Bitmap GetGreyscaleFromAlpha()
        {
            if (!this.loaded) return null;

            return ddsFile.GetGreyscaleFromAlpha();
        }

        /// <summary>
        /// Load a mask to use for HSV shifting or masked application of colours.
        /// Clears any mask currently applied.
        /// </summary>
        /// <param name="mask"></param>
        public void LoadMask(Stream mask)
        {
            ClearMask();
            try
            {
                base.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                this.ddsMask = new DdsFile();
                this.ddsMask.Load(mask, false);//only want the pixmap data
            }
            finally { base.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        /// <summary>
        /// Removes any previously applied masked shifts
        /// </summary>
        public void ClearMask()
        {
            if (SupportsHSV)
                ddsFile.ClearMask();
            ddsMask = null;
            if (loaded)
                ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Creates a mask with given <paramref name="maskChannels"/> active and of given <paramref name="size"/>.
        /// </summary>
        /// <param name="maskChannels">Which channels in the mask should be activated.</param>
        /// <param name="size">Size of image.</param>
        public void CreateMask(MaskChannel maskChannels, Size size)
        {
            CreateMask(maskChannels, size.Width, size.Height);
        }

        /// <summary>
        /// Creates a mask with given <paramref name="maskChannels"/> active and of given <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="maskChannels">Which channels in the mask should be activated.</param>
        /// <param name="width">Width of image.</param>
        /// <param name="height">Height of image.</param>
        public void CreateMask(MaskChannel maskChannels, int width, int height)
        {
            ClearMask();
            ddsMask = new DdsFile();
            ddsMask.CreateImage(
                MaskChannelToByte(maskChannels, MaskChannel.C1),
                MaskChannelToByte(maskChannels, MaskChannel.C2),
                MaskChannelToByte(maskChannels, MaskChannel.C3),
                MaskChannelToByte(maskChannels, MaskChannel.C4),
                width, height, false);
        }

        /// <summary>
        /// Apply <see cref="HSVShift"/> values to the image, based on the
        /// channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Shift">A shift to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Shift">A shift to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Shift">A shift to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Shift">A shift to apply to the image when the fourth channel of the mask is active.</param>
        /// <param name="blend">When true, each channel's shift adds; when false, each channel's shift overrides.</param>
        public void ApplyHSVShift(Stream mask, HSVShift ch1Shift, HSVShift ch2Shift, HSVShift ch3Shift, HSVShift ch4Shift, bool blend)
        {
            if (!SupportsHSV) return;
            LoadMask(mask);
            ApplyHSVShift(ch1Shift, ch2Shift, ch3Shift, ch4Shift, blend);
        }

        /// <summary>
        /// Apply <see cref="HSVShift"/> values to the image, based on the
        /// channels in the currently loaded mask.
        /// </summary>
        /// <param name="ch1Shift">A shift to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Shift">A shift to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Shift">A shift to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Shift">A shift to apply to the image when the fourth channel of the mask is active.</param>
        /// <param name="blend">When true, each channel's shift adds; when false, each channel's shift overrides.</param>
        public void ApplyHSVShift(HSVShift ch1Shift, HSVShift ch2Shift, HSVShift ch3Shift, HSVShift ch4Shift, bool blend)
        {
            if (!SupportsHSV || !MaskLoaded) return;
            if (blend)
                ddsFile.MaskedHSVShift(ddsMask, ch1Shift, ch2Shift, ch3Shift, ch4Shift);
            else
                ddsFile.MaskedHSVShiftNoBlend(ddsMask, ch1Shift, ch2Shift, ch3Shift, ch4Shift);

            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Set the colour of the image based on the channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Colour">(Nullable) ARGB colour to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Colour">(Nullable) ARGB colour to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Colour">(Nullable) ARGB colour to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Colour">(Nullable) ARGB colour to the image when the fourth channel of the mask is active.</param>
        public void ApplyColours(Stream mask, uint? ch1Colour, uint? ch2Colour, uint? ch3Colour, uint? ch4Colour)
        {
            if (!loaded) return;
            LoadMask(mask);
            ApplyColours(ch1Colour, ch2Colour, ch3Colour, ch4Colour);
        }

        /// <summary>
        /// Set the colour of the image based on the channels in the currently loaded mask.
        /// </summary>
        /// <param name="ch1Colour">(Nullable) ARGB colour to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Colour">(Nullable) ARGB colour to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Colour">(Nullable) ARGB colour to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Colour">(Nullable) ARGB colour to the image when the fourth channel of the mask is active.</param>
        public void ApplyColours(uint? ch1Colour, uint? ch2Colour, uint? ch3Colour, uint? ch4Colour)
        {
            if (!loaded || !MaskLoaded) return;
            ddsFile.MaskedSetColour(ddsMask, ch1Colour, ch2Colour, ch3Colour, ch4Colour);

            ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Apply the supplied images to the areas of the base image defined by the
        /// channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="T:System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the fourth channel of the mask is active.</param>
        public void ApplyImage(Stream mask, Stream ch1Image, Stream ch2Image, Stream ch3Image, Stream ch4Image)
        {
            if (!this.loaded) return;

            LoadMask(mask);
            ApplyImage(ch1Image, ch2Image, ch3Image, ch4Image);
        }

        /// <summary>
        /// Apply the supplied images to the areas of the base image defined by the
        /// channels in the currently loaded mask.
        /// </summary>
        /// <param name="ch1Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Image">The <see cref="T:System.IO.Stream"/> containing the DDS image to apply to the image when the fourth channel of the mask is active.</param>
        public void ApplyImage(Stream ch1Image, Stream ch2Image, Stream ch3Image, Stream ch4Image)
        {
            if (!this.loaded || !this.MaskLoaded) return;

            DdsFile ch1dds = null, ch2dds = null, ch3dds = null, ch4dds = null;
            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                if (ch1Image != null) { ch1dds = new DdsFile(); ch1dds.Load(ch1Image, false); }
                if (ch2Image != null) { ch2dds = new DdsFile(); ch2dds.Load(ch2Image, false); }
                if (ch3Image != null) { ch3dds = new DdsFile(); ch3dds.Load(ch3Image, false); }
                if (ch4Image != null) { ch4dds = new DdsFile(); ch4dds.Load(ch4Image, false); }
                ddsFile.MaskedApplyImage(this.ddsMask, ch1dds, ch2dds, ch3dds, ch4dds);
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }
            this.ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Apply the supplied images to the areas of the base image defined by the
        /// channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="T:System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the fourth channel of the mask is active.</param>
        public void ApplyImage(Stream mask, Image ch1Image, Image ch2Image, Image ch3Image, Image ch4Image)
        {
            if (!this.loaded) return;

            LoadMask(mask);
            ApplyImage(ch1Image, ch2Image, ch3Image, ch4Image);
        }

        /// <summary>
        /// Apply the supplied images to the areas of the base image defined by the
        /// channels in the currently loaded mask.
        /// </summary>
        /// <param name="ch1Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Image">The <see cref="T:System.Drawing.Image"/> to apply to the image when the fourth channel of the mask is active.</param>
        public void ApplyImage(Image ch1Image, Image ch2Image, Image ch3Image, Image ch4Image)
        {
            if (!this.loaded || !this.MaskLoaded) return;

            ddsFile.MaskedApplyImage(this.ddsMask, ch1Image, ch2Image, ch3Image, ch4Image);
            this.ckb_CheckedChanged(null, null);
        }

        /// <summary>
        /// Apply the supplied images to the areas of the base image defined by the
        /// channels in the <paramref name="mask"/>.
        /// </summary>
        /// <param name="mask">The <see cref="T:System.IO.Stream"/> containing the DDS image to use as a mask.</param>
        /// <param name="ch1Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the fourth channel of the mask is active.</param>
        public void ApplyImage(Stream mask, Bitmap ch1Image, Bitmap ch2Image, Bitmap ch3Image, Bitmap ch4Image)
        {
            if (!this.loaded) return;

            LoadMask(mask);
            ApplyImage(ch1Image, ch2Image, ch3Image, ch4Image);
        }

        /// <summary>
        /// Apply the supplied images to the areas of the base image defined by the
        /// channels in the currently loaded mask.
        /// </summary>
        /// <param name="ch1Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the first channel of the mask is active.</param>
        /// <param name="ch2Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the second channel of the mask is active.</param>
        /// <param name="ch3Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the third channel of the mask is active.</param>
        /// <param name="ch4Image">The <see cref="T:System.Drawing.Bitmap"/> to apply to the image when the fourth channel of the mask is active.</param>
        public void ApplyImage(Bitmap ch1Image, Bitmap ch2Image, Bitmap ch3Image, Bitmap ch4Image)
        {
            if (!this.loaded || !this.MaskLoaded) return;

            ddsFile.MaskedApplyImage(this.ddsMask, ch1Image, ch2Image, ch3Image, ch4Image);
            this.ckb_CheckedChanged(null, null);
        }
        #endregion

        #region Enumerations
        /// <summary>
        /// Used to indicate which channels in a mask should be activated when creating a new mask.
        /// </summary>
        [Flags]
        public enum MaskChannel
        {
            /// <summary>
            /// Activates channel 1 ("Red")
            /// </summary>
            C1 = 1,
            /// <summary>
            /// Activates channel 2 ("Green")
            /// </summary>
            C2 = 2,
            /// <summary>
            /// Activates channel 3 ("Blue")
            /// </summary>
            C3 = 4,
            /// <summary>
            /// Activates channel 4 ("Alpha")
            /// </summary>
            C4 = 8,
        }
        #endregion

        #region Implementation
        void OnChanged(EventHandler h) { if (h != null) h(this, EventArgs.Empty); }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            try
            {
                this.Enabled = false;
                Application.UseWaitCursor = true;
                Application.DoEvents();
                ddsFile.HSVShift = hsvShift;
                image = ddsFile.GetImage(ckbR.Checked, ckbG.Checked, ckbB.Checked, ckbA.Checked, ckbI.Checked);
                pictureBox1.Image = doResize();
            }
            finally { this.Enabled = true; Application.UseWaitCursor = false; Application.DoEvents(); }

            if (sender != null)
            {
                if (sender == ckbR) Channel1 = ckbR.Checked;
                if (sender == ckbG) Channel2 = ckbG.Checked;
                if (sender == ckbB) Channel3 = ckbB.Checked;
                if (sender == ckbA) Channel4 = ckbA.Checked;
                if (sender == ckbI) InvertCh4 = ckbI.Checked;
            }
        }

        private void control_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        Image doResize()
        {
            if (loaded)
            {
                tlpSize.Visible = true;
                lbSize.Text = image.Size.Width + ", " + image.Size.Height;
                lbDDSFmt.Text = UseDXT ? "DXT: " + AlphaDepth : "Non-DXT";
            }
            else tlpSize.Visible = false;

            Size targetSize = loaded ? image.Size : new Size(128, 128);

            if (Fit) targetSize = ScaleToFit(targetSize, panel1.ClientSize);

            targetSize = ScaleToFit(targetSize, maxSize);

            pictureBox1.Size = targetSize;

            if (!this.loaded) return null;

            Image targetImage = image;

            Size minSize = Min(targetImage.Size, targetSize);
            if (targetImage.Size.Width > targetSize.Width || targetImage.Size.Height > targetSize.Height)
                targetImage = targetImage.GetThumbnailImage(minSize.Width, minSize.Height, () => false, System.IntPtr.Zero);

            return targetImage;
        }

        Size ScaleToFit(Size from, Size constraint)
        {
            double scaleWidth = constraint.Width <= 0 ? 1 : Math.Min(from.Width, constraint.Width) / (double)from.Width;
            double scaleHeight = constraint.Height <= 0 ? 1 : Math.Min(from.Height, constraint.Height) / (double)from.Height;
            double scale = Math.Min(scaleWidth, scaleHeight);

            // Prevent scaling to zero pixels, thus allowing distortion.
            return new Size((int)Math.Max(1, Math.Round(from.Width * scale - 0.5, MidpointRounding.AwayFromZero)),
                (int)Math.Max(1,Math.Round(from.Height * scale - 0.5, MidpointRounding.AwayFromZero)));
        }

        private static Size Max(Size left, Size right)
        {
            return new Size(Math.Max(left.Width, right.Width), Math.Max(left.Height, right.Height));
        }

        private static Size Min(Size left, Size right)
        {
            return new Size(Math.Min(left.Width, right.Width), Math.Min(left.Height, right.Height));
        }

        private void DDSPanel_Resize(object sender, EventArgs e)
        {
            if (fit && DateTime.UtcNow > now.AddMilliseconds(25))
            {
                pictureBox1.Image = doResize();
                now = DateTime.UtcNow;
            }
            else
                timer1.Interval = 10;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            pictureBox1.Image = doResize();
        }

        byte MaskChannelToByte(MaskChannel value, MaskChannel test, byte bTrue = 255, byte bFalse = 0) { return (value & test) != 0 ? bTrue : bFalse; }
        #endregion
    }
}
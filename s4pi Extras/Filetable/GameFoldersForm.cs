using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;

namespace s4pi.Filetable
{
    /// <summary>
    /// Provides a common control for enabling editing of the install locations of game folders.
    /// </summary>
    /// <remarks>
    /// Note that it is left to the containing application to pass values between this form and the Filetable.
    /// This is to allow any additional processing, such as storing user preferences, to be handled.
    /// </remarks>
    public partial class GameFoldersForm : Form
    {
        /// <summary>
        /// Instantiate the form.
        /// </summary>
        public GameFoldersForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The folder containing user-created ("custom") content.
        /// </summary>
        public string CustomContent
        {
            get { return tbCCFolder.Text; }
            set { tbCCFolder.Text = value; }
        }

        /// <summary>
        /// Whether to display the option to allow user-created content to be included in the file table.
        /// </summary>
        public bool CCSettable
        {
            get { return tlpCustomContent.Enabled; }
            set { tlpCustomContent.Enabled = value; }
        }

        /// <summary>
        /// Whether the user wants to have their user-created content available.
        /// </summary>
        public bool CCEnabled
        {
            get { return ckbCustomContent.Checked; }
            set { btnCCEdit.Enabled = ckbCustomContent.Checked = value; }
        }

        private Dictionary<string, string> gameDirs = new Dictionary<string, string>();
        /// <summary>
        /// A semi-colon delimited string of game name / folder pairs (delimiter by an equals sign), representing
        /// where the user has installed a game (if not in the default location).
        /// </summary>
        public string InstallDirs
        {
            get { return string.Join(";", gameDirs.Select(kvp => kvp.Key + "=" + kvp.Value).ToArray()); }
            set
            {
                gameDirs = new Dictionary<string, string>();
                foreach (string s in value.Split(';'))
                {
                    string[] p = s.Split(new char[] { '=' }, 2);
                    if (GameFolders.byName(p[0]) != null && Directory.Exists(p[1]))
                        gameDirs.Add(p[0], p[1]);
                }
            }
        }

        private List<string> ePsDisabled = new List<string>();
        /// <summary>
        /// A semi-colon delimited string of game names the user does not want considered.
        /// </summary>
        public string EPsDisabled
        {
            get { return string.Join(";", ePsDisabled.ToArray()); }
            set
            {
                ePsDisabled = new List<string>(value.Split(';')
                    .Distinct()
                    .Where(x => { Game g = GameFolders.byName(x); return g != null && g.Suppressed.HasValue; }));
            }
        }

        /* ------------------------- */

        Dictionary<int, Game> RowToGame = null;
        private void GameFoldersForm_Load(object sender, EventArgs e)
        {
            Size size = this.Size;
            Size sizeTLP = tlpGameFolders.Size;

            if (RowToGame == null)
            {
                RowToGame = new Dictionary<int, Game>();
                foreach (Game game in GameFolders.Games.OrderByDescending(x => x.RGVersion))
                {
                    int row = tlpGameFolders.RowCount - 1;
                    RowToGame.Add(row - 1, game);

                    tlpGameFolders.RowCount++;
                    tlpGameFolders.RowStyles.Insert(row, new RowStyle(SizeType.AutoSize));

                    Label lbGameID = new Label();
                    CheckBox ckbEnabled = new CheckBox();
                    TextBox tbInstFolder = new TextBox();
                    Button btnEdit = new Button();

                    lbGameID.Anchor = AnchorStyles.Right;
                    lbGameID.AutoSize = true;
                    lbGameID.Text = game.RGVersion > 0
                        ? "(" + game.Longname.Replace("The Sims 3 ", "").Replace(" Stuff", "") + ") " + game.Name.ToUpper()
                        : game.Longname;

                    ckbEnabled.Anchor = AnchorStyles.None;
                    ckbEnabled.AutoSize = true;
                    ckbEnabled.Visible = game.Suppressed.HasValue;
                    ckbEnabled.Text = " ";
                    ckbEnabled.Checked = !ePsDisabled.Contains(game.Name);
                    ckbEnabled.CheckedChanged += new EventHandler(ckbEnabled_CheckedChanged);

                    tbInstFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                    tbInstFolder.Text = this[game] == null ? "(not set)" : this[game];
                    tbInstFolder.ReadOnly = true;
                    tbInstFolder.BackColor = tbInstFolder.Text == "(not set)" ? SystemColors.ControlDark : SystemColors.Control;

                    btnEdit.Anchor = AnchorStyles.None;
                    btnEdit.AutoSize = true;
                    btnEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    btnEdit.Enabled = ckbEnabled.Checked;
                    btnEdit.Text = "Edit";
                    btnEdit.Click += new EventHandler(btnEdit_Click);

                    tlpGameFolders.Controls.Add(lbGameID, 0, row);
                    tlpGameFolders.Controls.Add(ckbEnabled, 1, row);
                    tlpGameFolders.Controls.Add(btnEdit, 3, row);
                    tlpGameFolders.Controls.Add(tbInstFolder, 2, row);
                }
            }
            else
            {
                foreach (var kvp in RowToGame)
                {
                    int row = kvp.Key + 1;
                    Game game = kvp.Value;
                    Label lbGameID = (Label)tlpGameFolders.GetControlFromPosition(0, row);
                    CheckBox ckbEnabled = (CheckBox)tlpGameFolders.GetControlFromPosition(1, row);
                    TextBox tbInstFolder = (TextBox)tlpGameFolders.GetControlFromPosition(2, row);
                    Button btnEdit = (Button)tlpGameFolders.GetControlFromPosition(3, row);

                    if (game.Suppressed.HasValue)
                    {
                        if (ckbEnabled == null)
                        {
                            ckbEnabled = new CheckBox();
                            ckbEnabled.Anchor = AnchorStyles.None;
                            ckbEnabled.AutoSize = true;
                            ckbEnabled.CheckedChanged += new EventHandler(ckbEnabled_CheckedChanged);
                        }
                        ckbEnabled.Checked = !ePsDisabled.Contains(game.Name);
                        ckbEnabled.Visible = true;
                    }
                    else
                        if (ckbEnabled != null)
                            ckbEnabled.Visible = false;
                    tbInstFolder.Text = this[game] == null ? "(not set)" : this[game];
                    tbInstFolder.BackColor = tbInstFolder.Text == "(not set)" ? SystemColors.ControlDark : SystemColors.Control;
                    btnEdit.Enabled = ckbEnabled == null || !ckbEnabled.Visible || ckbEnabled.Checked;
                }
            }

            this.Size = new Size(size.Width, Math.Min(Screen.GetWorkingArea(this).Height * 4 / 5, size.Height - sizeTLP.Height + tlpGameFolders.Height));
            this.Location = new Point((Screen.GetWorkingArea(this).Width - this.Width) / 2, (Screen.GetWorkingArea(this).Height - this.Height) / 2);
        }

        private void label2_LocationChanged(object sender, EventArgs e)
        {
            tlpCustomContent.ColumnStyles[0].Width = label2.Location.X - label2.Margin.Left;
        }

        Game GameFromControl(Control c)
        {
            int row = tlpGameFolders.GetCellPosition(c).Row - 1;
            if (!RowToGame.ContainsKey(row)) return null;
            return RowToGame[row];
        }

        private void ckbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Game game = GameFromControl((Control)sender);
            if (game == null) return;
            if (!game.Suppressed.HasValue) return;

            if (ePsDisabled.Contains(game.Name))
                ePsDisabled.Remove(game.Name);
            else
                ePsDisabled.Add(game.Name);

            Button btn = tlpGameFolders.GetControlFromPosition(3, tlpGameFolders.GetCellPosition((Control)sender).Row) as Button;
            if (btn != null) btn.Enabled = !ePsDisabled.Contains(game.Name);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Game game = GameFromControl((Control)sender);
            if (game == null) return;

            folderBrowserDialog1.SelectedPath = this[game] == null ? "" : this[game];
            folderBrowserDialog1.Description = String.Format("Select the folder where \"{0}\" is installed.", game.Longname);
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            this[game] = folderBrowserDialog1.SelectedPath;

            TextBox tb = tlpGameFolders.GetControlFromPosition(2, tlpGameFolders.GetCellPosition((Control)sender).Row) as TextBox;
            tb.Text = this[game] == null ? "(not set)" : this[game];
            tb.BackColor = tb.Text == "(not set)" ? SystemColors.ControlDark : SystemColors.Control;
        }

        private void btnCCEdit_Click(object sender, EventArgs e)
        {
            string path = CustomContent;

            if (path == null)
                try { path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                catch { }
            folderBrowserDialog1.SelectedPath = path;
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            CustomContent = folderBrowserDialog1.SelectedPath;
        }

        private void ckbCustomContent_CheckedChanged(object sender, EventArgs e)
        {
            btnCCEdit.Enabled = ckbCustomContent.Checked;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }

        /* ------------------------- */

        string this[Game index]
        {
            get
            {
                string possiblePath = gameDirs.ContainsKey(index.Name) ? gameDirs[index.Name] : index.UserInstallDir;
                return Directory.Exists(possiblePath) ? possiblePath : null;
            }
            set
            {
                if (safeGetFullPath(gameDirs.ContainsKey(index.Name) ? gameDirs[index.Name] : null) == safeGetFullPath(value)) return;

                if (gameDirs.ContainsKey(index.Name))
                {
                    if (safeGetFullPath(index.DefaultInstallDir) == safeGetFullPath(value))
                        gameDirs.Remove(index.Name);
                    else
                        gameDirs[index.Name] = value == null ? "" : value;
                }
                else
                    gameDirs.Add(index.Name, value);
            }
        }
        static string safeGetFullPath(string value) { return value == null ? null : Path.GetFullPath(value); }
    }
}

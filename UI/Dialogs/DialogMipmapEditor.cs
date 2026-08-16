using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RE4_PS2_TPL_Manager.Core.Services;
using RE4_PS2_TPL_Manager.UI.Theming;
using TplModel = RE4_PS2_TPL_Manager.TPLDefinition.TPL;

namespace RE4_PS2_TPL_Manager
{
    /// <summary>
    /// Dedicated texture/mipmap viewer and editor. Mip replacement always reuses the
    /// parent CLUT; main replacement regenerates existing mipmaps against the new CLUT.
    /// </summary>
    public sealed class DialogMipmapEditor : Form
    {
        private readonly string path;
        private readonly int textureIndex;
        private readonly TplReader reader;
        private readonly MipmapService mipmaps;
        private readonly Action ensureBackup;
        private readonly TexturePreviewBox mainPreview = new TexturePreviewBox();
        private readonly TexturePreviewBox mip1Preview = new TexturePreviewBox();
        private readonly TexturePreviewBox mip2Preview = new TexturePreviewBox();
        private readonly Label mainInfo = new Label();
        private readonly Label mip1Info = new Label();
        private readonly Label mip2Info = new Label();
        private readonly Button replaceMain = new Button();
        private readonly Button replaceMip1 = new Button();
        private readonly Button replaceMip2 = new Button();
        private readonly Button regenerate = new Button();
        private readonly Button addMipmaps = new Button();
        private readonly Button removeMipmaps = new Button();
        private readonly Label sharedClut = new Label();

        public bool Modified { get; private set; }
        public string LastAction { get; private set; }

        public DialogMipmapEditor(string path, int textureIndex, TplReader reader, MipmapService mipmaps, Action ensureBackup)
        {
            this.path = path;
            this.textureIndex = textureIndex;
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.mipmaps = mipmaps ?? throw new ArgumentNullException(nameof(mipmaps));
            this.ensureBackup = ensureBackup ?? delegate { };
            BuildUi();
            DarkTheme.ApplyDialog(this);
            LoadTextureFamily();
        }

        private void BuildUi()
        {
            Text = "Texture & Mipmap Editor";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1040, 650);
            MinimumSize = new Size(900, 560);
            ShowIcon = false;

            TableLayoutPanel root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            Controls.Add(root);

            TableLayoutPanel previews = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            previews.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            previews.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            previews.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334f));
            root.Controls.Add(previews, 0, 0);

            previews.Controls.Add(BuildCard("MAIN", mainPreview, mainInfo, replaceMain), 0, 0);
            previews.Controls.Add(BuildCard("MIP 1", mip1Preview, mip1Info, replaceMip1), 1, 0);
            previews.Controls.Add(BuildCard("MIP 2", mip2Preview, mip2Info, replaceMip2), 2, 0);

            sharedClut.Dock = DockStyle.Fill;
            sharedClut.TextAlign = ContentAlignment.MiddleLeft;
            sharedClut.Padding = new Padding(6, 0, 0, 0);
            root.Controls.Add(sharedClut, 0, 1);

            FlowLayoutPanel actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
            regenerate.Text = "Regenerate Mipmaps"; regenerate.AutoSize = true;
            addMipmaps.Text = "Add Mipmaps"; addMipmaps.AutoSize = true;
            removeMipmaps.Text = "Remove Mipmaps"; removeMipmaps.AutoSize = true;
            Button close = new Button { Text = "Close", AutoSize = true };
            actions.Controls.Add(close); actions.Controls.Add(removeMipmaps); actions.Controls.Add(addMipmaps); actions.Controls.Add(regenerate);
            root.Controls.Add(actions, 0, 2);

            replaceMain.Text = "Replace Main...";
            replaceMip1.Text = "Replace Mip 1...";
            replaceMip2.Text = "Replace Mip 2...";
            replaceMain.Click += delegate { ReplaceMain(); };
            replaceMip1.Click += delegate { ReplaceMip(0); };
            replaceMip2.Click += delegate { ReplaceMip(1); };
            regenerate.Click += delegate { RunAction("Mipmaps regenerated", delegate { mipmaps.Regenerate(path, textureIndex); }); };
            addMipmaps.Click += delegate { RunAction("Mipmaps added", delegate { mipmaps.AddMipmaps(path, textureIndex); }); };
            removeMipmaps.Click += delegate
            {
                if (MessageBox.Show(this, "Remove both mipmap levels from this texture?", "Remove Mipmaps", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    RunAction("Mipmaps removed", delegate { mipmaps.RemoveMipmaps(path, textureIndex); });
            };
            close.Click += delegate { Close(); };
        }

        private Control BuildCard(string title, TexturePreviewBox preview, Label info, Button button)
        {
            GroupBox group = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(8) };
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            preview.Dock = DockStyle.Fill;
            preview.NavigationHint = "Wheel: Zoom";
            info.Dock = DockStyle.Fill; info.TextAlign = ContentAlignment.MiddleCenter;
            button.Dock = DockStyle.Fill;
            layout.Controls.Add(preview, 0, 0); layout.Controls.Add(info, 0, 1); layout.Controls.Add(button, 0, 2);
            group.Controls.Add(layout);
            return group;
        }

        private void LoadTextureFamily()
        {
            DisposePreview(mainPreview); DisposePreview(mip1Preview); DisposePreview(mip2Preview);
            TplModel texture = reader.ReadTexture(path, textureIndex);
            Text = "Texture & Mipmap Editor - " + textureIndex + ".tpl";
            mainPreview.Image = mipmaps.DecodeMain(path, textureIndex);
            mainInfo.Text = FormatInfo(texture.width, texture.height, texture.bitDepth, texture.interlace);

            int count = Math.Min((ushort)2, texture.mipmapCount);
            if (count > 0)
            {
                mip1Preview.Image = mipmaps.DecodeMip(path, textureIndex, 0);
                FillMipInfo(texture.mipmapHeader1, mip1Info);
            }
            else mip1Info.Text = "No mipmap";

            if (count > 1)
            {
                mip2Preview.Image = mipmaps.DecodeMip(path, textureIndex, 1);
                FillMipInfo(texture.mipmapHeader2, mip2Info);
            }
            else mip2Info.Text = "No mipmap";

            replaceMip1.Enabled = count > 0;
            replaceMip2.Enabled = count > 1;
            regenerate.Enabled = count > 0;
            addMipmaps.Enabled = count == 0;
            removeMipmaps.Enabled = count > 0;
            sharedClut.Text = texture.bitDepth == 0x08 || texture.bitDepth == 0x09
                ? "Shared CLUT: Main + Mip 1 + Mip 2 use the same " + (texture.bitDepth == 0x08 ? "16-color" : "256-color") + " palette. Mip replacements do not create a new palette."
                : "This texture format does not use an indexed CLUT.";
        }

        private void ReplaceMain()
        {
            string file = PickImage("Replace main texture");
            if (file == null) return;
            using (Bitmap image = LoadBitmap(file))
                RunAction("Main texture replaced and mipmaps regenerated", delegate { mipmaps.ReplaceMainAndRegenerate(path, textureIndex, image); });
        }

        private void ReplaceMip(int mipIndex)
        {
            string file = PickImage("Replace mipmap " + (mipIndex + 1));
            if (file == null) return;
            using (Bitmap image = LoadBitmap(file))
                RunAction("Mip " + (mipIndex + 1) + " replaced using the shared CLUT", delegate { mipmaps.ReplaceMip(path, textureIndex, mipIndex, image); });
        }

        private void RunAction(string success, Action action)
        {
            try
            {
                ensureBackup();
                Cursor = Cursors.WaitCursor;
                action();
                Modified = true;
                LastAction = success;
                LoadTextureFamily();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Mipmap Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally { Cursor = Cursors.Default; }
        }

        private string PickImage(string title)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = title;
                dialog.Filter = "Image Files (*.png;*.bmp;*.tga;*.jpg;*.jpeg)|*.png;*.bmp;*.tga;*.jpg;*.jpeg";
                return dialog.ShowDialog(this) == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private static Bitmap LoadBitmap(string file)
        {
            if (Path.GetExtension(file).Equals(".tga", StringComparison.OrdinalIgnoreCase))
            {
                TGASharpLib.TGA tga = new TGASharpLib.TGA(file);
                return new Bitmap(tga.ToBitmap());
            }
            using (Image image = Image.FromFile(file)) return new Bitmap(image);
        }

        private static void FillMipInfo(byte[] header, Label label)
        {
            if (header == null || header.Length < 8) { label.Text = "Invalid mipmap"; return; }
            ushort width = BitConverter.ToUInt16(header, 0);
            ushort height = BitConverter.ToUInt16(header, 2);
            ushort depth = BitConverter.ToUInt16(header, 4);
            ushort interlace = BitConverter.ToUInt16(header, 6);
            label.Text = FormatInfo(width, height, depth, interlace);
        }

        private static string FormatInfo(ushort width, ushort height, ushort bitDepth, ushort interlace)
        {
            string depth = bitDepth == 0x08 ? "4-bit" : bitDepth == 0x09 ? "8-bit" : bitDepth == 0x06 ? "32-bit" : "0x" + bitDepth.ToString("X");
            string layout = interlace == 0 ? "BGRA" : interlace == 1 ? "BGRA Inverted" : interlace == 2 ? "PS2" : interlace == 3 ? "PS2 Inverted" : "Interlace " + interlace;
            return width + "×" + height + "  •  " + depth + "  •  " + layout;
        }

        private static void DisposePreview(TexturePreviewBox preview)
        {
            Image old = preview.Image;
            preview.Image = null;
            if (old != null) old.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposePreview(mainPreview); DisposePreview(mip1Preview); DisposePreview(mip2Preview);
            }
            base.Dispose(disposing);
        }
    }
}

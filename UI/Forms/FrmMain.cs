using ImageConvertor;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using Microsoft.WindowsAPICodePack.Dialogs;
using RE4_PS2_TPL_Manager.Dialog;
using RE4_PS2_TPL_Manager.Core.Services;
using RE4_PS2_TPL_Manager.Helpers;
using RE4_PS2_TPL_Manager.UI.Theming;
using SimplePaletteQuantizer.Extensions;
using SimplePaletteQuantizer.Helpers;
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.Octree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RE4_PS2_TPL_Manager.TPLDefinition;

namespace RE4_PS2_TPL_Manager
{
    public partial class FrmMain : Form
    {
        private const string AppTitle = "RE4 PS2 TPL Manager v1.3.2";
        // Structs
        TPLDefinition.TPL TPL;
        MipMap MipMap;
        TPLDefinition TPLClass = new TPLDefinition();
        readonly TplReader tplReader = new TplReader();
        readonly TplWriter tplWriter;
        readonly TextureDecoder textureDecoder = new TextureDecoder();
        readonly TextureEncoder textureEncoder = new TextureEncoder();
        readonly InterlaceConverter interlaceConverter = new InterlaceConverter();
        readonly MipmapService mipmapService;

        // Global
        OpenFileDialog dialog = new OpenFileDialog();
        string filepath = "";
        int selectedRowIndexGlobal = 0;
        System.Timers.Timer timer;
        private ToolStripMenuItem recentFilesMenu;
        private readonly List<string> recentFiles = new List<string>();
        private bool backupCreatedForCurrentFile = false;

        // v1.3.2 non-destructive quick editor state
        private Bitmap editorBaseImage;
        private bool suppressEditorEvents;
        private bool editorHasChanges;

        // Legacy mipmap metadata kept for compatibility with older editor routines
        public ushort mipmapCount { get; set; }
        public uint mipmapOffset1 { get; set; }
        public uint mipmapOffset2 { get; set; }
        public uint mipmapUnk1 { get; set; }
        public uint mipmapUnk2 { get; set; }


        public FrmMain()
        {
            InitializeComponent();
            tplWriter = new TplWriter(tplReader);
            mipmapService = new MipmapService(tplReader, tplWriter, textureEncoder);
            ApplyModernTheme();
            InitializeRecentFilesMenu();
            InitializeInterlaceMenu();
        }
        public FrmMain(string tplFile)
        {
            InitializeComponent();
            tplWriter = new TplWriter(tplReader);
            mipmapService = new MipmapService(tplReader, tplWriter, textureEncoder);
            ApplyModernTheme();
            InitializeRecentFilesMenu();
            InitializeInterlaceMenu();
            filepath = tplFile;
            AddRecentFile(filepath);
            try
            {
                FillTable();
                btnOpenFile.Dispose();
                btnCreateNewFile.Dispose();
                lblDropHere.Dispose();
                this.Text = AppTitle + " - " + Path.GetFileName(filepath);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void InitializeInterlaceMenu()
        {
            ToolStripMenuItem item = new ToolStripMenuItem("Convert Interlace...");
            item.ToolTipText = "Losslessly convert the selected indexed texture between BGRA/linear and PS2/swizzled pixel layouts.";
            item.Click += convertInterlaceToolStripMenuItem_Click;
            toolsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            toolsToolStripMenuItem.DropDownItems.Add(item);
        }

        private void convertInterlaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(filepath) || table.Rows.Count == 0 || selectedRowIndexGlobal < 0)
            {
                MessageBox.Show("Open a TPL and select a texture first.", "Convert Interlace", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                TPLDefinition.TPL source = tplReader.ReadTexture(filepath, selectedRowIndexGlobal);
                string currentName = source.interlace == 0 ? "BGRA" : source.interlace == 1 ? "BGRA Inverted" : source.interlace == 2 ? "PS2" : source.interlace == 3 ? "PS2 Inverted" : "Unknown (" + source.interlace + ")";
                if (source.mipmapCount > 0)
                    throw new NotSupportedException("Convert Interlace does not modify mipmaps yet. Use it on textures without mipmaps.");

                using (DialogInterlaceConversion dialogInterlace = new DialogInterlaceConversion(currentName))
                {
                    if (dialogInterlace.ShowDialog(this) != DialogResult.OK)
                    {
                        UpdateStatusText("Interlace conversion cancelled");
                        return;
                    }

                    EnsureBackup();
                    UpdateStatusText("Converting texture interlace layout...");
                    TPLDefinition.TPL converted = interlaceConverter.ConvertFamily(source, dialogInterlace.TargetPs2);
                    tplWriter.ReplaceTexture(filepath, selectedRowIndexGlobal, converted);

                    string resultName = converted.interlace == 0 ? "BGRA" : converted.interlace == 1 ? "BGRA Inverted" : converted.interlace == 2 ? "PS2" : "PS2 Inverted";
                    RefreshTableAndKeepSelection(selectedRowIndexGlobal);
                    UpdateStatusText("Texture " + selectedRowIndexGlobal + " interlace converted: " + currentName + " -> " + resultName);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusText("Interlace conversion failed: " + ex.Message);
                MessageBox.Show(ex.Message, "Convert Interlace", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ApplyModernTheme()
        {
            DarkTheme.Apply(this, menuStrip1, statusStrip1, ctxMenuTable, ctxPreviewImage);
            texturePreview.SizeMode = PictureBoxSizeMode.Normal;
            table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            table.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            table.MultiSelect = true;
        }

        private void CreateEmptyTPL()
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Create empty TPL";
                saveDialog.Filter = "RE4 PS2 TPL Files (*.tpl)|*.tpl";
                saveDialog.DefaultExt = "tpl";
                saveDialog.AddExtension = true;
                saveDialog.OverwritePrompt = true;
                saveDialog.FileName = "new_texture_pack.tpl";

                if (!String.IsNullOrWhiteSpace(filepath))
                {
                    string currentDirectory = Path.GetDirectoryName(filepath);
                    if (!String.IsNullOrWhiteSpace(currentDirectory) && Directory.Exists(currentDirectory))
                        saveDialog.InitialDirectory = currentDirectory;
                }

                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    UpdateStatusText("Create empty TPL cancelled");
                    return;
                }

                string newPath = saveDialog.FileName;
                tplWriter.CreateEmpty(newPath);
                filepath = newPath;
                backupCreatedForCurrentFile = false;
                AddRecentFile(filepath);

                if (!btnCreateNewFile.IsDisposed) btnCreateNewFile.Dispose();
                if (!btnOpenFile.IsDisposed) btnOpenFile.Dispose();
                if (!lblDropHere.IsDisposed) lblDropHere.Dispose();

                this.Text = AppTitle + " - " + Path.GetFileName(filepath);
                FillTable();
                UpdateStatusText("Empty TPL created • " + Path.GetFileName(filepath));
            }
        }
        private void UpdateAllOffsets(string tplFilename)
        {
            tplWriter.UpdateAllOffsets(tplFilename);
        }
        private void ExtractTPL()
        {
            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            TPL.magic = br.ReadUInt32();
            TPL.tplCount = br.ReadUInt32();
            br.Close();

            UpdateStatusText($"Extracting {TPL.tplCount} textures...");

            for (int i = 0; i < TPL.tplCount; i++)
            {
                // Returns to header position on each iteration
                ReadTexture(filepath, i);

                // Getting texture data (pixels and palette)
                byte[] mipmapPixels = new byte[0];

                // =========================================
                // Writing new .tpl file
                string extractFileName = table.Rows[i].Cells[1].Value.ToString();
                DirectoryInfo folder = Directory.CreateDirectory("Extracted/" + Path.GetFileNameWithoutExtension(filepath));
                BinaryWriter bw = new BinaryWriter(File.Open($"Extracted/{folder.Name}/{extractFileName}", FileMode.Create));

                bw.Write(TPL.magic);
                bw.Write((uint)0x01);
                bw.Write(TPL.startOffset);
                bw.Write(TPL.unused1);
                bw.Write(TPL.width);
                bw.Write(TPL.height);
                bw.Write(TPL.bitDepth);
                bw.Write(TPL.interlace);
                bw.Write(TPL.zPriority);
                bw.Write(TPL.mipmapCount);
                bw.Write(TPL.scale);
                bw.Write(TPL.unused2);
                bw.Write((uint)(0x00));
                bw.Write((uint)(0x00));
                bw.Write((uint)(0x00));
                bw.Write((uint)(0x00));
                bw.Write((uint)0x40);
                if (TPL.bitDepth == 8)
                {
                    bw.Write((TPL.width * TPL.height) / 2 + 0x40);
                }
                else if (TPL.bitDepth == 9)
                {
                    bw.Write((TPL.width * TPL.height) + 0x40);
                }
                bw.Write(TPL.unused3);
                bw.Write(TPL.config1);
                bw.Write(TPL.config2);
                bw.Write(TPL.config3);
                bw.Write(TPL.unused4);
                bw.Write(TPL.unused5);
                bw.Write(TPL.endTag);
                bw.Write(TPL.pixels);
                bw.Write(TPL.palette);
                bw.Close();
                Console.WriteLine("Arquivo " + i);
                Console.WriteLine(TPL.mipmapOffset1.ToString("X"));
                Console.WriteLine(TPL.mipmapOffset2.ToString("X"));
                Console.WriteLine("");
                // Extracts mipmap if option is checked
                if (includeMipmapToolStripMenuItem.Checked)
                {
                    BinaryReader br2 = new BinaryReader(File.Open(filepath, FileMode.Open));
                    for (int count = 0; count < TPL.mipmapCount; count++)
                    {
                        if (count == 0)
                            br2.BaseStream.Position = TPL.mipmapOffset1;
                        else
                            br2.BaseStream.Position = TPL.mipmapOffset2;

                        MipMap.width = br2.ReadUInt16();
                        MipMap.height = br2.ReadUInt16();
                        MipMap.bitDepth = br2.ReadUInt16();
                        MipMap.interlace = br2.ReadUInt16();
                        MipMap.baseResolution = br2.ReadUInt16();
                        MipMap.mipmapCount = br2.ReadUInt16();
                        MipMap.multipliedResolution = br2.ReadUInt16();
                        MipMap.unused2 = br2.ReadUInt16();

                        MipMap.mipmapOffset1 = br2.ReadUInt32();
                        MipMap.mipmapOffset2 = br2.ReadUInt32();
                        MipMap.unknown1 = br2.ReadUInt32();
                        MipMap.unknown2 = br2.ReadUInt32();

                        MipMap.pixelsOffset = br2.ReadUInt32();
                        MipMap.paletteOffset = br2.ReadUInt32();
                        MipMap.unused3 = br2.ReadByte();
                        MipMap.config1 = br2.ReadByte();
                        MipMap.config2 = br2.ReadByte();
                        MipMap.config3 = br2.ReadUInt16();
                        MipMap.unused4 = br2.ReadByte();
                        MipMap.unused5 = br2.ReadByte();
                        MipMap.endTag = br2.ReadByte();

                        br2.BaseStream.Position = MipMap.pixelsOffset;
                        if (MipMap.bitDepth == 8)
                        {
                            mipmapPixels = br2.ReadBytes((int)((MipMap.width * MipMap.height) / 2));
                        }
                        else if (MipMap.bitDepth == 9)
                        {
                            mipmapPixels = br2.ReadBytes((int)(MipMap.width * MipMap.height));
                        }

                        // =============================
                        // CREATING FILE
                        // =============================
                        BinaryWriter bwMipMap = new BinaryWriter(File.Open($"Extracted/{folder.Name}/{i}_{count}.tpl", FileMode.Create));
                        bwMipMap.Write(TPL.magic);
                        bwMipMap.Write((uint)0x01);
                        bwMipMap.Write(TPL.startOffset);
                        bwMipMap.Write(TPL.unused1);

                        bwMipMap.Write(MipMap.width);
                        bwMipMap.Write(MipMap.height);
                        bwMipMap.Write(MipMap.bitDepth);
                        bwMipMap.Write(MipMap.interlace);
                        bwMipMap.Write(MipMap.baseResolution);
                        bwMipMap.Write((short)0x00); // Nulls mipmap count for viewing on GGS
                        bwMipMap.Write(MipMap.multipliedResolution);
                        bwMipMap.Write(MipMap.unused2);

                        bwMipMap.Write(MipMap.mipmapOffset1);
                        bwMipMap.Write(MipMap.mipmapOffset2);
                        bwMipMap.Write(MipMap.unknown1);
                        bwMipMap.Write(MipMap.unknown2);

                        bwMipMap.Write((uint)0x40);
                        if (MipMap.bitDepth == 8)
                        {
                            bwMipMap.Write((MipMap.width * MipMap.height) / 2 + 0x40);
                        }
                        else if (MipMap.bitDepth == 9)
                        {
                            bwMipMap.Write((MipMap.width * MipMap.height) + 0x40);
                        }
                        bwMipMap.Write(MipMap.unused3);
                        bwMipMap.Write(MipMap.config1);
                        bwMipMap.Write(MipMap.config2);
                        bwMipMap.Write(MipMap.config3);
                        bwMipMap.Write(MipMap.unused4);
                        bwMipMap.Write(MipMap.unused5);
                        bwMipMap.Write(MipMap.endTag);

                        bwMipMap.Write(mipmapPixels);
                        bwMipMap.Write(TPL.palette);

                        bwMipMap.Close();
                    }
                    br2.Close();
                }
            }
            UpdateStatusText(TPL.tplCount.ToString() + " textures extracted to " + $"'Extracted/{Path.GetFileNameWithoutExtension(filepath)}'");
            br.Close();
        }
        private void AddNewTextures()
        {
            EnsureBackup();
            if (filepath == "") return;

            // --- Primeiro, carrega tudo do TPL original ---
            byte[] topPart;
            byte[] bottomPart;
            uint tplCountOriginal;
            int totalMipmaps = 0;

            using (BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                TPL.magic = br.ReadUInt32();
                TPL.tplCount = tplCountOriginal = br.ReadUInt32();
                TPL.startOffset = br.ReadUInt32();
                TPL.unused1 = br.ReadUInt32();
                int chunk = 0;

                for (int i = 0; i < TPL.tplCount; i++)
                {
                    br.BaseStream.Position = 0x1A + chunk;
                    ushort mipmapCount = br.ReadUInt16();
                    if (mipmapCount > 0)
                        totalMipmaps += mipmapCount;
                    chunk += 0x30;
                }

                br.BaseStream.Position = 0;
                topPart = br.ReadBytes((int)(0x10 + (0x30 * (TPL.tplCount + totalMipmaps))));

                bottomPart = new byte[0];

                if (TPL.tplCount > 0)
                {
                    br.BaseStream.Position = 0x30;
                    uint pixelsStart = br.ReadUInt32();

                    br.BaseStream.Position = pixelsStart;
                    bottomPart = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                }
            } // <- aqui o arquivo é fechado!

            // --- Agora, seleciona os novos arquivos para adicionar ---
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "TPL/Images (*.tpl;*.png;*.jpg;*.jpeg;*.bmp)|*.tpl;*.png;*.jpg;*.jpeg;*.bmp";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() != DialogResult.OK) return;

            List<byte[]> headers = new List<byte[]>();
            List<byte[]> pixels = new List<byte[]>();
            List<byte[]> palettes = new List<byte[]>();

            foreach (var file in dialog.FileNames)
            {
                string ext = Path.GetExtension(file).ToLower();

                if (ext == ".tpl")
                {
                    AddTplFile(file, headers, pixels, palettes);
                }
                else
                {
                    AddImageFile(file, headers, pixels, palettes);
                }
            }

            // --- Agora, reabre para escrever o novo TPL ---
            using (BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(topPart);

                // Atualiza contador de texturas
                bw.BaseStream.Position = 0x04;
                bw.Write((uint)(tplCountOriginal + headers.Count)); // corrigido!

                bw.BaseStream.Position = bw.BaseStream.Length;

                // Headers
                foreach (var h in headers)
                    bw.Write(h);

                // Parte de dados anterior
                bw.Write(bottomPart);

                // Novos pixels e paletas
                for (int i = 0; i < pixels.Count; i++)
                {
                    bw.Write(pixels[i]);
                    bw.Write(palettes[i]);
                }
            }

            UpdateAllOffsets(filepath);
            FillTable();
        }

        private void AddTplFile(string path, List<byte[]> headers, List<byte[]> pixels, List<byte[]> palettes)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                br.BaseStream.Position = 0x00;
                uint magic = br.ReadUInt32();
                uint tplCount = br.ReadUInt32();
                uint startOffset = br.ReadUInt32();
                uint unused1 = br.ReadUInt32();

                for (int texture = 0; texture < tplCount; texture++)
                {
                    // Lê o header
                    br.BaseStream.Position = 0x10 + (texture * 0x30);
                    byte[] header = br.ReadBytes(0x30);

                    // Lê valores importantes do header
                    ushort width = BitConverter.ToUInt16(header, 0x00);
                    ushort height = BitConverter.ToUInt16(header, 0x02);
                    ushort bitDepth = BitConverter.ToUInt16(header, 0x04);
                    uint pixelsOffset = BitConverter.ToUInt32(header, 0x20);
                    uint paletteOffset = BitConverter.ToUInt32(header, 0x24);

                    // Adiciona o header
                    headers.Add(header);

                    // Lê os pixels
                    br.BaseStream.Position = pixelsOffset;
                    byte[] pixelData;

                    if (bitDepth == 0x8) // 4bpp
                        pixelData = br.ReadBytes((width * height) / 2);
                    else if (bitDepth == 0x9) // 8bpp
                        pixelData = br.ReadBytes(width * height);
                    else
                        throw new NotSupportedException($"Unsupported bit depth {bitDepth:X}");

                    pixels.Add(pixelData);

                    // Lê a paleta
                    br.BaseStream.Position = paletteOffset;
                    byte[] paletteData;

                    if (bitDepth == 0x8)
                        paletteData = br.ReadBytes(0x80);
                    else if (bitDepth == 0x9)
                        paletteData = br.ReadBytes(0x400);
                    else
                        paletteData = new byte[0];

                    palettes.Add(paletteData);
                }
            }
        }
        private void AddImageFile(string path, List<byte[]> headers, List<byte[]> pixels, List<byte[]> palettes)
        {
            int bitsPerPixel = ImageHelper.GetImageBitDepth(path);
            using (Bitmap bitmap = new Bitmap(path))
            {
                int width = bitmap.Width;
                int height = bitmap.Height;

                // Pega a profundidade de bits da imagem

                // Decide a quantidade de cores com base nos bits
                int colorCount = (bitsPerPixel <= 4) ? 16 : 256;

                // Quantiza a imagem para o número correto de cores
                IColorQuantizer colorQuantizer = new OctreeQuantizer();
                Bitmap quantized = (Bitmap)ImageBuffer.QuantizeImage(bitmap, colorQuantizer, colorCount, 4);

                // --- Resto do processamento (igual ao que já montamos) ---
                Color[] paletteColors = quantized.Palette.Entries.Take(colorCount).ToArray();
                byte[] paletteData = new byte[colorCount == 16 ? 0x80 : 0x400];

                for (int i = 0; i < paletteColors.Length; i++)
                {
                    Color color = paletteColors[i];
                    paletteData[i * 4 + 0] = color.R;
                    paletteData[i * 4 + 1] = color.G;
                    paletteData[i * 4 + 2] = color.B;
                    paletteData[i * 4 + 3] = (byte)(color.A * 0x80 / 0xFF);
                }

                if (colorCount == 16)
                {
                    Array.Copy(paletteData, 0, paletteData, 0x40, 0x20);
                }

                else if (colorCount == 256)
                {
                    paletteData = new byte[0x400]; // 256 cores * 4 bytes cada

                    int dstOffset = 0;

                    for (int i = 0; i < paletteColors.Length; i++) // 32 cores por chunk
                    {
                        Color color = paletteColors[i];

                        paletteData[dstOffset++] = color.R;
                        paletteData[dstOffset++] = color.G;
                        paletteData[dstOffset++] = color.B;
                        paletteData[dstOffset++] = (byte)(color.A * 0x80 / 0xFF); // Ajuste do Alpha (0x80 = 100%)
                    }

                }

                List<byte> pixelData = new List<byte>();

                if (colorCount == 16)
                {
                    pixelData = new List<byte>();
                    for (int y = quantized.Height - 1; y >= 0; y--) // bottom-up
                    {
                        for (int x = 0; x < quantized.Width; x += 2)
                        {
                            // Pega 2 pixels de uma vez
                            Color pixel1Color = quantized.GetPixel(x, y);
                            byte pixel1Index = TplHelper.FindPaletteIndex(pixel1Color, paletteColors);

                            Color pixel2Color = Color.Black;
                            byte pixel2Index = 0;

                            if (x + 1 < quantized.Width)
                            {
                                pixel2Color = quantized.GetPixel(x + 1, y);
                                pixel2Index = TplHelper.FindPaletteIndex(pixel2Color, paletteColors);
                            }

                            byte packed = (byte)((pixel2Index << 4) | (pixel1Index & 0x0F)); // SWAPPED!
                            pixelData.Add(packed);
                        }
                    }

                }
                else
                {
                    pixelData = new List<byte>();

                    int blockWidth = 16;

                    for (int y = quantized.Height - 2; y >= 0; y -= 2)
                    {
                        for (int x = 0; x < quantized.Width; x += blockWidth)
                        {
                            for (int inner = 0; inner < blockWidth; inner++)
                            {
                                int px = x + inner;

                                if (px < quantized.Width && (y + 1) < quantized.Height)
                                {
                                    Color colorTop = quantized.GetPixel(px, y + 1);
                                    byte idxTop = TplHelper.FindPaletteIndex(colorTop, paletteColors);
                                    pixelData.Add(idxTop);
                                }

                                if (px < quantized.Width && y >= 0)
                                {
                                    Color colorBottom = quantized.GetPixel(px, y);
                                    byte idxBottom = TplHelper.FindPaletteIndex(colorBottom, paletteColors);
                                    pixelData.Add(idxBottom);
                                }
                            }
                        }
                    }

                }

                // Header correto
                byte[] header = new byte[0x30];
                using (MemoryStream ms = new MemoryStream(header))
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((ushort)width);
                    bw.Write((ushort)height);
                    bw.Write((ushort)(colorCount == 16 ? 0x8 : 0x9)); // bitDepth
                    bw.Write((ushort)0x2); // interlace
                    bw.Write((ushort)0x100); // zPriority
                    bw.Write((ushort)0x0); // mipmapCount
                    bw.Write((ushort)((width * height) / (colorCount == 16 ? 32 : 16))); // scale
                    bw.Write((ushort)0); // unused2

                    bw.Write((uint)0); // mipmapOffset1
                    bw.Write((uint)0); // mipmapOffset2
                    bw.Write((uint)0); // unk1
                    bw.Write((uint)0); // unk2

                    bw.Write((uint)0); // pixelsOffset placeholder
                    bw.Write((uint)0); // paletteOffset placeholder

                    bw.Write((byte)0); // unused3

                    bw.Write((byte)(width > 128 ? 0x00 : 0x80)); // config1
                    if (width > 128)
                        bw.Write((byte)(0x40 + (width / 16)));
                    else
                        bw.Write((byte)0x40);

                    ushort config3 = 1229;
                    for (int m = 0; m < 8; m++)
                    {
                        if (width != Math.Pow(2, 3 + m)) config3 += 4;
                        else break;
                    }
                    for (int m = 0; m < 8; m++)
                    {
                        if (height != Math.Pow(2, 3 + m)) config3 += 0x40;
                        else break;
                    }
                    bw.Write(config3);

                    bw.Write((byte)0); // unused4
                    bw.Write((byte)0); // unused5
                    bw.Write((byte)0x40); // endTag
                }

                headers.Add(header);
                pixels.Add(pixelData.ToArray());
                palettes.Add(paletteData);
            }
        }

        private void CompileFromFolder()
        {
            // Folder Dialog
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
                folderDialog.Title = "Select folder containing indexed PNG files (0.png, 1.png, 2.png, ...)";
            if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok) return;

            if (folderDialog.FileName != "")
            {
                // Array of .tpl files names
                List<string> tplFiles = Directory.GetFiles(folderDialog.FileName, "*.tpl", SearchOption.TopDirectoryOnly).ToList<string>();

                // Creates file with the name of the folder
                Directory.CreateDirectory("Compiled");
                BinaryWriter bw = new BinaryWriter(File.Open("Compiled/" + new DirectoryInfo(folderDialog.FileName).Name + ".tpl", FileMode.Create));

                // Write main header
                bw.Write((uint)4096);
                bw.Write((uint)0x00);
                bw.Write((uint)0x10);
                bw.Write((uint)0x00);
                uint totalTextures = 0x00;

                // Acumulators
                List<uint> lengthEachFilePixels = new List<uint>();
                List<uint> lengthEachFilePalette = new List<uint>();

                // Loop through all .tpl files in directory [HEADERS]
                for (int i = 0; i < tplFiles.Count; i++)
                {
                    BinaryReader br = new BinaryReader(File.Open(tplFiles[i], FileMode.Open));

                    br.BaseStream.Position = 0x04;
                    byte tplCount = br.ReadByte();

                    // Get headers
                    for (int texture = 0; texture < tplCount; texture++)
                    {
                        br.BaseStream.Position = 0x10 + (0x30 * texture);
                        byte[] header = br.ReadBytes(0x30);
                        bw.Write(header);
                        totalTextures++;
                    }
                    br.Close();
                }

                // --------------------
                // DEPOIS FAZER SUPORTE A MIPMAPS, USANDO UNDERLINE PARA ENCONTRÁ-LOS
                // --------------------

                // Loop through all .tpl files in directory [MIPMAPS]
                for (int i = 0; i < tplFiles.Count; i++)
                {
                    if (Path.GetFileNameWithoutExtension(tplFiles[i]).EndsWith("_0") ||
                        Path.GetFileNameWithoutExtension(tplFiles[i]).EndsWith("_1"))
                    {
                        //BinaryReader br = new BinaryReader(File.Open(tplFiles[i], FileMode.Open));

                        //br.BaseStream.Position = 0x04;
                        //byte tplCount = br.ReadByte();

                        //// Get headers
                        //for (int texture = 0; texture < tplCount; texture++)
                        //{
                        //    br.BaseStream.Position = 0x10 + (0x30 * texture);
                        //    byte[] header = br.ReadBytes(0x30);
                        //    bw.Write(header);
                        //    totalTextures++;
                        //}
                        //br.Close();
                    }
                }

                // Loop through all .tpl files in directory [PIXELS AND PALETTES]
                for (int i = 0; i < tplFiles.Count; i++)
                {
                    BinaryReader br = new BinaryReader(File.Open(tplFiles[i], FileMode.Open));

                    br.BaseStream.Position = 0x04;
                    byte tplCount = br.ReadByte();
                    byte[] pixels = new byte[0];
                    byte[] palette = new byte[0];

                    // Get pixels and palettes
                    for (int texture = 0; texture < tplCount; texture++)
                    {
                        br.BaseStream.Position = 0x10 + (0x30 * texture);
                        ushort width = br.ReadUInt16();
                        ushort height = br.ReadUInt16();
                        ushort bitDepth = br.ReadUInt16();

                        // Get pixels and palette
                        br.BaseStream.Position = 0x30 + (0x30 * texture);
                        br.BaseStream.Position = br.ReadUInt32();

                        if (bitDepth == 0x08)
                        {
                            pixels = br.ReadBytes((width * height) / 2);
                            palette = br.ReadBytes(0x80);
                        }
                        else
                        {
                            pixels = br.ReadBytes(width * height);
                            palette = br.ReadBytes(0x400);
                        }

                        bw.Write(pixels);
                        bw.Write(palette);

                        lengthEachFilePixels.Add((uint)(pixels.Length));
                        lengthEachFilePalette.Add((uint)(palette.Length));
                    }
                    br.Close();
                }

                // -------------------
                // OFFSETS UPDATES
                // -------------------

                // Update texture count
                bw.BaseStream.Position = 0x04;
                bw.Write(totalTextures);
                uint tempAcumulatorPixels = 0x00;
                uint tempAcumulatorPalette = 0x00;

                for (int i = 0; i < totalTextures; i++)
                {
                    bw.BaseStream.Position = 0x30 + (0x30 * i);

                    // Pixels offset
                    bw.Write(tempAcumulatorPixels + tempAcumulatorPalette + (0x30 * totalTextures) + 0x10);

                    // Acumulator
                    tempAcumulatorPixels += lengthEachFilePixels[i];

                    // Palette offset
                    bw.Write(tempAcumulatorPixels + tempAcumulatorPalette + (0x30 * totalTextures) + 0x10);
                    tempAcumulatorPalette += lengthEachFilePalette[i];
                }

                // Removes mipmap data from headers
                for (int m = 0; m < totalTextures; m++)
                {
                    bw.BaseStream.Position = 0x1A + (0x30 * m);
                    bw.Write((ushort)0x00);

                    bw.BaseStream.Position = 0x20 + (0x30 * m);
                    bw.Write((long)0x00);
                    bw.Write((long)0x00);
                }
                bw.Close();
                UpdateStatusText($"TPL compiled successfully: Compiled/{new DirectoryInfo(folderDialog.FileName).Name}.tpl");
            }
        }
        private void ReadTexture(string tplFile, int textureIndex)
        {
            TPL = tplReader.ReadTexture(tplFile, textureIndex);
        }
        private void ReadTexture(BinaryReader br, int textureIndex)
        {
            TPL = tplReader.ReadTexture(br, textureIndex);
        }
        private void WriteTexture(int rowNumber)
        {
            // =========================================
            // Writing new .tpl file
            string extractFileName = table.Rows[rowNumber].Cells[1].Value.ToString();
            DirectoryInfo folder = Directory.CreateDirectory("Extracted/" + Path.GetFileNameWithoutExtension(filepath));
            BinaryWriter bw = new BinaryWriter(File.Open($"Extracted/{folder.Name}/{extractFileName}", FileMode.Create));

            bw.Write(TPL.magic);
            bw.Write((uint)0x01);
            bw.Write(TPL.startOffset);
            bw.Write(TPL.unused1);
            bw.Write(TPL.width);
            bw.Write(TPL.height);
            bw.Write(TPL.bitDepth);
            bw.Write(TPL.interlace);
            bw.Write(TPL.zPriority);
            bw.Write(TPL.mipmapCount);
            bw.Write(TPL.scale);
            bw.Write(TPL.unused2);
            bw.Write((uint)(0x00));
            bw.Write((uint)(0x00));
            bw.Write((uint)(0x00));
            bw.Write((uint)(0x00));
            bw.Write((uint)0x40);
            if (TPL.bitDepth == 8)
            {
                bw.Write((TPL.width * TPL.height) / 2 + 0x40);
            }
            else if (TPL.bitDepth == 9)
            {
                bw.Write((TPL.width * TPL.height) + 0x40);
            }
            bw.Write(TPL.unused3);
            bw.Write(TPL.config1);
            bw.Write(TPL.config2);
            bw.Write(TPL.config3);
            bw.Write(TPL.unused4);
            bw.Write(TPL.unused5);
            bw.Write(TPL.endTag);
            bw.Write(TPL.pixels);
            bw.Write(TPL.palette);
            bw.Close();
        }
        private int GetTotalMipmapCount()
        {
            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            TPL.magic = br.ReadUInt32();
            TPL.tplCount = br.ReadUInt32();
            TPL.startOffset = br.ReadUInt32();
            TPL.unused1 = br.ReadUInt32();

            int mipmapCount = 0;
            for (int i = 0; i < TPL.tplCount; i++)
            {
                br.BaseStream.Position = 0x1A + (0x30 * i);
                mipmapCount += br.ReadInt16();
            }
            br.Close();

            return mipmapCount;
        }
        private bool VerifyMipmapPositions()
        {
            bool mipmapsAreWrong = false;
            int mipmapHeadersCount = GetTotalMipmapCount();

            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            TPL.magic = br.ReadUInt32();
            TPL.tplCount = br.ReadUInt32();
            br.BaseStream.Position = 0x04 + (0x30 * TPL.tplCount);
            TPL.paletteOffset = br.ReadUInt32(); // Get last palette offset

            // Verify if all mipmaps are after last palette offset
            for (int i = 0; i < mipmapHeadersCount; i++)
            {
                br.BaseStream.Position = 0x30 + (0x30 * (TPL.tplCount + i));
                Console.WriteLine("Reading offset: 0x" + br.BaseStream.Position.ToString("X"));
                MipMap.pixelsOffset = br.ReadUInt32();
                if (MipMap.pixelsOffset < TPL.paletteOffset)
                {
                    mipmapsAreWrong = true;
                    break;
                }
            }

            br.Close();
            return mipmapsAreWrong;
        }
        private void UpdateStatusText(string text)
        {
            lblStatusText.Text = text;
            statusStrip1.Invalidate();
            statusStrip1.Refresh();
        }
        private void UpdateTextureSelectionStatus(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= table.Rows.Count) return;
            DataGridViewRow row = table.Rows[rowIndex];
            string width = row.Cells[2].Value?.ToString() ?? "?";
            string height = row.Cells[3].Value?.ToString() ?? "?";
            string depth = row.Cells[4].Value?.ToString() ?? "?";
            UpdateStatusText($"Texture {rowIndex} selected  •  {width}×{height}  •  {depth}");
        }
        private void CreateTable()
        {
            table.Columns.Clear();

            // Table config
            DataGridViewImageColumn thumbnail = new DataGridViewImageColumn();
            thumbnail.Name = "thumbnail";
            thumbnail.HeaderText = "Thumbnail";
            table.Columns.Insert(0, thumbnail);

            table.Columns.Add("title", "Title"); // 1
            table.Columns[1].ReadOnly = true;
            table.Columns.Add("width", "Width"); // 2
            table.Columns.Add("height", "Height"); // 3

            // Metadata columns are display-only. Changing these values without converting the
            // underlying pixel/CLUT data can make the TPL header inconsistent with its payload.
            table.Columns.Add("bitDepth", "Bit Depth"); // 4
            table.Columns[4].ReadOnly = true;

            table.Columns.Add("interlace", "Interlace"); // 5
            table.Columns[5].ReadOnly = true;

            table.Columns.Add("baseResolution", "Z-Priority"); // 6
            table.Columns.Add("mipmapCount", "Mipmaps"); // 7
            table.Columns.Add("multResolution", "Scale"); // 8
            table.Columns.Add("config1", "Config 1"); // 9
            table.Columns.Add("config2", "Config 2"); // 10
            table.Columns.Add("config3", "Config 3"); // 11

            table.Columns[6].Visible = false;
            table.Columns[8].Visible = false;
            table.Columns[9].Visible = false;
            table.Columns[10].Visible = false;
            table.Columns[11].Visible = false;

            FormatTable();
        }
        private void FormatTable()
        {
            for (int i = 0; i < table.ColumnCount; i++)
            {
                table.Columns[i].Width = 60;
            }
            table.Columns[5].Width = 100;
        }
        private void FillTable()
        {
            // First check if there are mipmaps, if true rebuild the file to move all mipmaps to the end of the file
            if (GetTotalMipmapCount() > 0 && VerifyMipmapPositions())
            {
                DialogResult result = MessageBox.Show("This .tpl file contains mipmaps in a unsupported sequence," +
                    "in order to open it you must rearrange the file.\n" +
                    "Rearrange it now?", "Mipmaps: Unsupported sequence detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    Rearrange();
                }
                else return;
            }

            // Create table columns
            CreateTable();

            uint texturesTotal;

            // Abre o arquivo uma vez e mantém o fluxo até terminar tudo
            using (BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open)))
            {
                br.BaseStream.Position = 0x04;
                texturesTotal = br.ReadUInt32();

                // Status bar
                UpdateStatusText($"Loading {texturesTotal} textures, please wait...");
                progressBar.Maximum = (int)texturesTotal;
                progressBar.Value = 0;

                for (int i = 0; i < texturesTotal; i++)
                {
                    progressBar.Value++;
                    statusStrip1.Invalidate();
                    statusStrip1.Refresh();

                    ReadTexture(br, i);

                    Bitmap bmp = textureDecoder.Decode(TPL, br);

                    string GetBitDepth()
                    {
                        for (int x = 0; x < TPLClass.BitDepthDict.Values.ToArray()[0].Length; x++)
                        {
                            if (TPL.bitDepth == TPLClass.BitDepthDict.Values.ToArray()[0][x])
                                return TPLClass.BitDepthDict.Keys.ToArray()[0][x];
                        }
                        return "";
                    }

                    string GetInterlace()
                    {
                        for (int x = 0; x < TPLClass.InterlaceDict.Values.ToArray()[0].Length; x++)
                        {
                            if (TPL.interlace == TPLClass.InterlaceDict.Values.ToArray()[0][x])
                                return TPLClass.InterlaceDict.Keys.ToArray()[0][x];
                        }
                        return "";
                    }

                    // Adiciona a linha na tabela
                    var index = table.Rows.Add();
                    table.Rows[index].Cells[0].Value = bmp;
                    table.Rows[index].Cells[1].Value = $"{i}.tpl";
                    table.Rows[index].Cells[2].Value = TPL.width;
                    table.Rows[index].Cells[3].Value = TPL.height;
                    table.Rows[index].Cells[4].Value = GetBitDepth();
                    table.Rows[index].Cells[5].Value = GetInterlace();
                    table.Rows[index].Cells[6].Value = TPL.zPriority;
                    table.Rows[index].Cells[7].Value = TPL.mipmapCount;
                    table.Rows[index].Cells[8].Value = TPL.scale;
                    table.Rows[index].Cells[9].Value = TPL.config1;
                    table.Rows[index].Cells[10].Value = TPL.config2;
                    table.Rows[index].Cells[11].Value = TPL.config3;

                    // Define colunas como somente leitura
                    table.Rows[index].Cells[1].ReadOnly = true;
                    table.Rows[index].Cells[2].ReadOnly = true;
                    table.Rows[index].Cells[3].ReadOnly = true;
                    table.Rows[index].Cells[4].ReadOnly = true;
                    table.Rows[index].Cells[5].ReadOnly = true;
                    table.Rows[index].Cells[7].ReadOnly = true;
                    table.Rows[index].Cells[9].ReadOnly = true;
                    table.Rows[index].Cells[10].ReadOnly = true;
                    table.Rows[index].Cells[11].ReadOnly = true;
                }
            }
            UpdateStatusText(texturesTotal + " textures loaded successfully");
        }
        private void RefreshTable()
        {
            if (filepath != "")
            {
                try
                {
                    table.Rows.Clear();
                    table.Columns.Clear();
                    FillTable();
                }
                catch (Exception ex)
                {
                    UpdateStatusText("Could not refresh texture table: " + ex.Message);
                }
            }
        }
        private void ShowThumbnails()
        {
            // Temporary workaround
            ConverterBMP converterBMP = new ConverterBMP();
            converterBMP.TPLtoBMP(filepath, "BMP");
        }
        private void RemoveAll()
        {
            EnsureBackup();
            try
            {
                DialogResult confirmBoxResult = MessageBox.Show("Are you sure? This action cannot be undone.", "Question",
                     MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (confirmBoxResult == DialogResult.OK)
                {
                    BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
                    bw.Write((uint)4096);
                    bw.Write((uint)0x00);
                    bw.Write((uint)0x10);
                    bw.Write((uint)0x00);
                    bw.Close();

                    table.Rows.Clear();
                    UpdateStatusText("All textures removed successfully");
                    progressBar.Value = progressBar.Maximum;
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                UpdateStatusText("Could not remove textures: " + ex.Message);
            }
        }
        private void RemoveAllMipmaps()
        {
            EnsureBackup();
            if (GetTotalMipmapCount() > 0)
            {
                try
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(filepath));
                    TPL.magic = br.ReadUInt32();
                    TPL.tplCount = br.ReadUInt32();

                    // Get first texture pixels offset
                    br.BaseStream.Position = 0x30;
                    TPL.pixelsOffset = br.ReadUInt32();

                    // Get top part
                    br.BaseStream.Position = 0;
                    byte[] topPart = br.ReadBytes((int)(0x10 + (0x30 * TPL.tplCount)));

                    // Get first mipmap pixels offset
                    br.BaseStream.Position += 0x20;
                    MipMap.pixelsOffset = br.ReadUInt32();

                    // Get bottom part
                    br.BaseStream.Position = TPL.pixelsOffset;
                    byte[] bottomPart = br.ReadBytes((int)(MipMap.pixelsOffset - br.BaseStream.Position));
                    br.Close();

                    BinaryWriter bw = new BinaryWriter(File.Create(filepath));
                    bw.Write(topPart);
                    bw.Write(bottomPart);

                    // Remove all mipmap values from textures headers
                    for (int i = 0; i < TPL.tplCount; i++)
                    {
                        bw.BaseStream.Position = 0x1A + (0x30 * i);
                        bw.Write((ushort)0x00);
                        bw.BaseStream.Position = 0x20 + (0x30 * i);
                        bw.Write((long)0x00);
                        bw.Write((long)0x00);
                    }
                    bw.Close();
                    Console.WriteLine("All mipmaps removed");
                    UpdateAllOffsets(filepath);
                    RefreshTable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else return;
        }
        private void Duplicate()
        {
            // Checks if prompt is disabled
            if (!disableDuplicatePromptToolStripMenuItem.Checked)
            {
                DialogResult result = MessageBox.Show("This option will not duplicate mipmaps and will overwrite the file, duplicate anyway?"
                    , "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result != DialogResult.OK)
                {
                    return;
                }
            }
            // If user clicks on OK
            ReadTexture(filepath, selectedRowIndexGlobal);

            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            byte[] topPart = br.ReadBytes((int)(0x10 + (0x30 * TPL.tplCount)));
            byte[] bottomPart = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            br.Close();

            // Overwrite .tpl and insert new duplicated texture
            BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
            bw.Write(topPart);
            bw.Write(TPL.header);
            bw.Write(bottomPart);
            bw.Write(TPL.pixels);
            bw.Write(TPL.palette);

            // Update texture count
            bw.BaseStream.Position = 0x04;
            bw.Write(TPL.tplCount + 1);
            bw.Close();

            UpdateStatusText($"Texture {selectedRowIndexGlobal} duplicated");
            progressBar.Value = progressBar.Maximum;
            UpdateAllOffsets(filepath);
            FillTable();
        }
        private void Replace(string tplFile, string replacerTpl = "", int dialogTextureIndex = 0, bool isTemp = false, bool isBatch = false, BatchColorDepthMode batchMode = BatchColorDepthMode.Preserve)
        {
            EnsureBackup();
            if (String.IsNullOrWhiteSpace(tplFile) || selectedRowIndexGlobal < 0) return;

            ReadTexture(tplFile, selectedRowIndexGlobal);

            string selectedPath = replacerTpl;
            if (!isTemp && !isBatch && String.IsNullOrWhiteSpace(selectedPath))
            {
                using (OpenFileDialog fileDialog = new OpenFileDialog())
                {
                    fileDialog.Filter = "Image Files (*.png;*.bmp;*.tga)|*.png;*.bmp;*.tga|RE4 PS2 TPL Files (*.tpl)|*.tpl";
                    if (fileDialog.ShowDialog() != DialogResult.OK) return;
                    selectedPath = fileDialog.FileName;
                }
            }

            if (String.IsNullOrWhiteSpace(selectedPath)) return;

            string extension = Path.GetExtension(selectedPath).ToLowerInvariant();
            if (extension == ".png" || extension == ".bmp" || extension == ".tga" || extension == ".jpg" || extension == ".jpeg")
            {
                using (Bitmap image = LoadBitmapForImport(selectedPath))
                {
                    // Interactive replace may ask which target palette size to use for true-color images.
                    // Batch replace must never ask per texture: preserve the color depth of the destination.
                    int colorCount;
                    if (isBatch)
                    {
                        switch (batchMode)
                        {
                            case BatchColorDepthMode.Force4Bit:
                                colorCount = 16;
                                break;
                            case BatchColorDepthMode.Force8Bit:
                                colorCount = 256;
                                break;
                            default:
                                colorCount = TPL.bitDepth == 0x08 ? 16 : 256;
                                break;
                        }
                    }
                    else
                    {
                        colorCount = GetImportColorCount(image);
                    }

                    TPLDefinition.TPL destination = tplReader.ReadTexture(tplFile, selectedRowIndexGlobal);
                    TPLDefinition.TPL replacement = textureEncoder.EncodeImage(image, colorCount, destination.interlace);
                    tplWriter.ReplaceTexture(tplFile, selectedRowIndexGlobal, replacement);
                }
            }
            else if (extension == ".tpl")
            {
                uint tplCount = tplReader.ReadTextureCount(selectedPath);
                if (tplCount > 1 && !isBatch)
                {
                    using (DialogGetIndex dialogGetIndex = new DialogGetIndex())
                    {
                        dialogGetIndex.lblTplCount.Text = "of " + tplCount;
                        if (dialogGetIndex.ShowDialog() != DialogResult.OK) return;
                        dialogTextureIndex = dialogGetIndex.GetIndex();
                    }
                }
                tplWriter.ReplaceTexture(tplFile, selectedRowIndexGlobal, selectedPath, dialogTextureIndex);
            }
            else
            {
                MessageBox.Show("Unsupported replacement file format.", "Replace", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Mipmaps share the parent CLUT. After the main palette changes, their indices must
            // be regenerated against that same CLUT or distant rendering may show wrong colors.
            bool hasMipmaps = TPL.mipmapCount > 0;
            if (isBatch && hasMipmaps)
            {
                mipmapService.Regenerate(tplFile, selectedRowIndexGlobal);
            }
            else if (!isBatch && hasMipmaps)
            {
                DialogResult result = MessageBox.Show("Do you want to update mipmaps with this texture as well?\n" +
                    "They will be resized and mapped to the texture's shared CLUT.", "Mipmaps detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) ReplaceMipmaps();
            }

            UpdateStatusText("Texture replaced successfully");
            if (!isBatch) RefreshTableAndKeepSelection(selectedRowIndexGlobal);
        }

        private void OpenMipmapEditor()
        {
            if (String.IsNullOrWhiteSpace(filepath) || table.Rows.Count == 0 || selectedRowIndexGlobal < 0) return;
            try
            {
                using (DialogMipmapEditor editor = new DialogMipmapEditor(filepath, selectedRowIndexGlobal, tplReader, mipmapService, EnsureBackup))
                {
                    editor.ShowDialog(this);
                    if (editor.Modified)
                    {
                        RefreshTableAndKeepSelection(selectedRowIndexGlobal);
                        UpdateStatusText(String.IsNullOrWhiteSpace(editor.LastAction) ? "Texture/mipmaps updated" : editor.LastAction);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mipmap Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void table_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= table.Rows.Count) return;
            selectedRowIndexGlobal = e.RowIndex;
            OpenMipmapEditor();
        }

        private Bitmap LoadBitmapForImport(string path)
        {
            if (Path.GetExtension(path).Equals(".tga", StringComparison.OrdinalIgnoreCase))
            {
                TGASharpLib.TGA tga = new TGASharpLib.TGA(path);
                return new Bitmap(tga.ToBitmap());
            }
            using (Image source = Image.FromFile(path)) return new Bitmap(source);
        }

        private int GetImportColorCount(Bitmap image)
        {
            if (image.PixelFormat == PixelFormat.Format4bppIndexed) return 16;
            if (image.PixelFormat == PixelFormat.Format8bppIndexed) return 256;

            using (DialogGetColor dialogGetColor = new DialogGetColor())
            {
                dialogGetColor.ShowDialog();
                return dialogGetColor.ColorCount == 16 ? 16 : 256;
            }
        }

        private void ReplaceCurrentPreviewAtColorDepth(int colorCount)
        {
            EnsureBackup();
            if (String.IsNullOrWhiteSpace(filepath) || table.Rows.Count == 0 || selectedRowIndexGlobal < 0) return;
            Image preview = table.Rows[selectedRowIndexGlobal].Cells[0].Value as Image;
            if (preview == null) return;

            TPLDefinition.TPL destination = tplReader.ReadTexture(filepath, selectedRowIndexGlobal);
            TPLDefinition.TPL replacement = textureEncoder.EncodeImage(preview, colorCount, destination.interlace);
            tplWriter.ReplaceTexture(filepath, selectedRowIndexGlobal, replacement);
            if (destination.mipmapCount > 0) mipmapService.Regenerate(filepath, selectedRowIndexGlobal);
            UpdateStatusText(colorCount == 256 ? "Color depth increased to 256 colors" : "Color depth decreased to 16 colors");
            RefreshTableAndKeepSelection(selectedRowIndexGlobal);
        }

        private void RefreshTableAndKeepSelection(int index)
        {
            RefreshTable();
            if (table.Rows.Count == 0) return;
            int safeIndex = Math.Max(0, Math.Min(index, table.Rows.Count - 1));
            selectedRowIndexGlobal = safeIndex;
            table.ClearSelection();
            table.Rows[safeIndex].Selected = true;
            LoadEditorImage(table.Rows[safeIndex].Cells[0].Value as Image, true);
        }

        private void ReplaceMipmaps()
        {
            EnsureBackup();
            if (String.IsNullOrWhiteSpace(filepath) || selectedRowIndexGlobal < 0) return;
            mipmapService.Regenerate(filepath, selectedRowIndexGlobal);
            UpdateStatusText("Mipmaps regenerated using the parent texture CLUT");
        }
        private void Remove()
        {
            EnsureBackup();
            ReadTexture(filepath, selectedRowIndexGlobal);

            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));

            // From the beginning to the start of the header
            byte[] part1 = br.ReadBytes(0x10 + (0x30 * selectedRowIndexGlobal));
            br.BaseStream.Position += 0x30;

            // After the header to the start of the pixels chunk
            byte[] part2 = br.ReadBytes((int)(TPL.pixelsOffset - br.BaseStream.Position));
            br.BaseStream.Position += TPL.pixels.Length;

            // After the pixels chunk to the start of the palette chunk
            byte[] part3 = br.ReadBytes((int)(TPL.paletteOffset - br.BaseStream.Position));
            br.BaseStream.Position += TPL.palette.Length;

            // After the palette chunk to the end of the file
            byte[] part4 = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
            br.Close();

            // Overwrite .tpl and remove texture
            BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
            bw.Write(part1);
            bw.Write(part2);
            bw.Write(part3);
            bw.Write(part4);

            // Update texture count
            bw.BaseStream.Position = 0x04;
            bw.Write(TPL.tplCount - 1);
            bw.Close();

            UpdateStatusText("Texture removed successfully");
            UpdateAllOffsets(filepath);
            FillTable();
        }
        private void RemoveMipmaps()
        {
            EnsureBackup();
            if (String.IsNullOrWhiteSpace(filepath) || selectedRowIndexGlobal < 0) return;
            mipmapService.RemoveMipmaps(filepath, selectedRowIndexGlobal);
            UpdateStatusText("Mipmaps removed successfully");
            RefreshTableAndKeepSelection(selectedRowIndexGlobal);
        }
        private void Rearrange()
        {




            /* This method rearranges the file, getting all mipmaps pixels and moving them to the end of the file
             * the order remains the same. Can take some time on big .tpl files.
             */

            UpdateStatusText("Rearranging file, please wait...");

            int mipmapHeadersCount = GetTotalMipmapCount();
            byte[] topPart; // Top part of the file
            byte[] bottomPart; // Bottom part of the file
            byte[] pixels; // Mipmap pixels
            int pixelsLengthAcumulator = 0; // Length of every pixels

            Console.WriteLine("Total Mipmaps = " + mipmapHeadersCount);
            Console.WriteLine("");

            for (int i = 0; i < mipmapHeadersCount; i++)
            {
                Console.WriteLine("Rearranging mipmap " + i);
                BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
                br.BaseStream.Position = 0x00;
                TPL.magic = br.ReadUInt32();
                TPL.tplCount = br.ReadUInt32();

                br.BaseStream.Position = 0x10 + (0x30 * (TPL.tplCount + i));
                MipMap.width = br.ReadUInt16();
                MipMap.height = br.ReadUInt16();
                MipMap.bitDepth = br.ReadUInt16();
                br.BaseStream.Position = 0x30 + (0x30 * (TPL.tplCount + i));
                MipMap.pixelsOffset = br.ReadUInt32();

                // Get top and bottom parts
                br.BaseStream.Position = 0;
                topPart = br.ReadBytes((int)MipMap.pixelsOffset - pixelsLengthAcumulator);
                switch (MipMap.bitDepth)
                {
                    case 8:
                        br.BaseStream.Position += MipMap.width * MipMap.height / 2;
                        break;
                    case 9:
                        br.BaseStream.Position += MipMap.width * MipMap.height;
                        break;
                    default:
                        break;
                }
                bottomPart = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));

                // Get pixels byte array
                br.BaseStream.Position = MipMap.pixelsOffset - pixelsLengthAcumulator;
                if (MipMap.bitDepth == 8)
                {
                    pixels = br.ReadBytes(MipMap.width * MipMap.height / 2);
                    pixelsLengthAcumulator += pixels.Length;
                }
                else
                {
                    pixels = br.ReadBytes(MipMap.width * MipMap.height);
                    pixelsLengthAcumulator += pixels.Length;
                }
                br.Close();

                // Overwrites the file to rearrange
                BinaryWriter bw = new BinaryWriter(File.Create(filepath));
                bw.Write(topPart);
                bw.Write(bottomPart);
                bw.Write(pixels);
                bw.Close();
            }
            UpdateAllOffsets(filepath);
            UpdateStatusText("Rearrange done, loading textures...");
        }
        private void ExtendView()
        {
            // Shows a simple window with the image and its specifications
            Form form = new Form();

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(texturePreview.Image.Width, texturePreview.Image.Height);
            pictureBox.Image = texturePreview.Image;

            StatusBar statusBar = new StatusBar();
            statusBar.Panels.Add("Width: " + texturePreview.Image.Width + "px");
            statusBar.Panels.Add("Height: " + texturePreview.Image.Height + "px");
            statusBar.ShowPanels = true;

            form.Text = table.Rows[selectedRowIndexGlobal].Cells[1].Value.ToString();
            form.ShowIcon = false;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Controls.Add(pictureBox);
            form.Controls.Add(statusBar);
            form.Size = new Size(texturePreview.Image.Width + 25, texturePreview.Image.Height + 65);
            form.Show();
        }
        public Bitmap GetResizeImage(Bitmap bitmap, int newWidth, int newHeight)
        {
            var newSize = new Size(newWidth, newHeight);
            return new Bitmap(bitmap, newSize);
        }
        // Menu buttons events 
        private void btnCreateNewFile_Click(object sender, EventArgs e)
        {
            CreateEmptyTPL();
        }
        private void createEmptyTPLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateEmptyTPL();
        }
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            dialog.Filter = "RE4 PS2 TPL Files (*.tpl)|*.tpl";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            filepath = dialog.FileName;
            AddRecentFile(filepath);
            backupCreatedForCurrentFile = false;
            btnOpenFile.Dispose();
            btnCreateNewFile.Dispose();
            lblDropHere.Dispose();
            if (filepath != "")
            {
                this.Text = AppTitle + " - " + Path.GetFileName(filepath);
                UpdateStatusText("Opening " + Path.GetFileName(filepath) + "...");
                FillTable();
            }
        }
        private void openTPLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialog.Filter = "RE4 PS2 TPL Files (*.tpl)|*.tpl";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            filepath = dialog.FileName;
            AddRecentFile(filepath);
            backupCreatedForCurrentFile = false;
            btnCreateNewFile.Dispose();
            btnOpenFile.Dispose();
            lblDropHere.Dispose();
            if (filepath != "")
            {
                this.Text = AppTitle + " - " + Path.GetFileName(filepath);
                UpdateStatusText("Opening " + Path.GetFileName(filepath) + "...");
                FillTable();
            }
        }
        private void refreshTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateStatusText("Refreshing texture table...");
            RefreshTable();
        }
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                try
                {
                    BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Open));
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        bw.BaseStream.Position = 0x10 + (0x30 * i);
                        bw.Write((ushort)table.Rows[i].Cells[2].Value);
                        bw.Write((ushort)table.Rows[i].Cells[3].Value);

                        if (table.Rows[i].Cells[4].Value.ToString() == "32-bit")
                        {
                            bw.Write((ushort)0x06);
                        }
                        else if (table.Rows[i].Cells[4].Value.ToString() == "8-bit")
                        {
                            bw.Write((ushort)0x09);
                        }
                        else
                        {
                            bw.Write((ushort)0x08);
                        }

                        switch (table.Rows[i].Cells[5].Value.ToString())
                        {
                            case "BGRA":
                                bw.Write((ushort)0x00);
                                break;
                            case "BGRA Inverted":
                                bw.Write((ushort)0x01);
                                break;
                            case "PS2":
                                bw.Write((ushort)0x02);
                                break;
                            case "PS2 Inverted":
                                bw.Write((ushort)0x03);
                                break;
                            default:
                                bw.Write((ushort)0x00);
                                break;
                        }

                        bw.Write((ushort)table.Rows[i].Cells[6].Value); // Z-priority
                        bw.Write((ushort)table.Rows[i].Cells[7].Value); // Mipmap count
                        bw.Write((ushort)table.Rows[i].Cells[8].Value); // Scale
                        bw.BaseStream.Position = 0x39 + (0x30 * i);
                        bw.Write((byte)table.Rows[i].Cells[9].Value); // Render Config 1
                        bw.Write((byte)table.Rows[i].Cells[10].Value); // Render Config 2
                        bw.Write((ushort)table.Rows[i].Cells[11].Value); // Render Config 3
                    }
                    bw.Close();
                    UpdateStatusText("File changes saved successfully!");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }

            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Credits credits = new Credits();
            credits.Show();
        }
        private void stopBackgroundTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();
            stopBackgroundTaskToolStripMenuItem.Visible = false;
            UpdateStatusText("Background tasks killed");
        }
        // Menu: Texture
        private void btnExtract_Click(object sender, EventArgs e)
        {
            UpdateStatusText("Extracting textures...");
            ExtractTPL();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            UpdateStatusText("Adding textures...");
            AddNewTextures();
        }
        private void showHiddenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                // Show hidden parameters toggle
                if (showHiddenToolStripMenuItem.Checked)
                {
                    for (int column = 0; column < table.ColumnCount; column++)
                    {
                        table.Columns[column].Visible = true;
                    }
                }
                else
                {
                    table.Columns[6].Visible = false;
                    table.Columns[8].Visible = false;
                    table.Columns[9].Visible = false;
                    table.Columns[10].Visible = false;
                    table.Columns[11].Visible = false;
                }
                UpdateStatusText(showHiddenToolStripMenuItem.Checked ? "Advanced texture columns shown" : "Advanced texture columns hidden");
            }
        }
        private void extractSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in table.SelectedRows)
            {
                try
                {
                    ReadTexture(filepath, row.Index);
                    WriteTexture(row.Index);
                    UpdateStatusText("Texture(s) extracted");
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }
        private void removeAllMipmapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllMipmaps();
        }
        private void convertAllToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath)) return;

            try
            {
                ConverterBMP converterBMP = new ConverterBMP();
                converterBMP.TPLtoBMP(filepath, "PNG");
                UpdateStatusText("All textures exported to PNG successfully.");
            }
            catch (Exception ex)
            {
                // ConverterBMP continues past individual texture failures and reports them here,
                // instead of silently stopping at the first problematic entry.
                MessageBox.Show(ex.Message, "Export all to PNG", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatusText("PNG export completed with one or more errors.");
            }
        }
        private void increaseAllTo256ColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureBackup();
            if (String.IsNullOrWhiteSpace(filepath)) return;
            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                if (table.Rows[i].Cells[4].Value.ToString() == "8-bit") continue;
                Image image = table.Rows[i].Cells[0].Value as Image;
                if (image == null) continue;
                TPLDefinition.TPL destination = tplReader.ReadTexture(filepath, i);
                TPLDefinition.TPL replacement = textureEncoder.EncodeImage(image, 256, destination.interlace);
                tplWriter.ReplaceTexture(filepath, i, replacement);
                if (destination.mipmapCount > 0) mipmapService.Regenerate(filepath, i);
            }
            UpdateStatusText("All eligible textures converted to 256 colors");
            RefreshTableAndKeepSelection(selectedRowIndexGlobal);
        }
        private void decreaseAllTo16ColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureBackup();
            if (String.IsNullOrWhiteSpace(filepath)) return;
            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                if (table.Rows[i].Cells[4].Value.ToString() == "4-bit") continue;
                Image image = table.Rows[i].Cells[0].Value as Image;
                if (image == null) continue;
                TPLDefinition.TPL destination = tplReader.ReadTexture(filepath, i);
                TPLDefinition.TPL replacement = textureEncoder.EncodeImage(image, 16, destination.interlace);
                tplWriter.ReplaceTexture(filepath, i, replacement);
                if (destination.mipmapCount > 0) mipmapService.Regenerate(filepath, i);
            }
            UpdateStatusText("All eligible textures converted to 16 colors");
            RefreshTableAndKeepSelection(selectedRowIndexGlobal);
        }
        // Menu: Tools
        private void btnCompileFromFolder_Click(object sender, EventArgs e)
        {
            UpdateStatusText("Compiling TPL from folder...");
            CompileFromFolder();
        }
        private void convertAndImportBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BMP Files (*.bmp)|*.bmp";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            ConverterBMP converterBMP = new ConverterBMP();
            converterBMP.BMPtoTPL(openFileDialog.FileNames);
        }
        // Table events
        private void table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LoadEditorImage(table.Rows[e.RowIndex].Cells[0].Value as Image, true);
                texturePreview.Invalidate();
                UpdateTextureSelectionStatus(e.RowIndex);
            }
        }
        private void table_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;

            selectedRowIndexGlobal = e.RowIndex;

            if (e.Button == MouseButtons.Right)
            {
                // Right-click now behaves like a normal selection before opening the context menu.
                table.ClearSelection();
                table.Rows[e.RowIndex].Selected = true;
                if (e.ColumnIndex >= 0) table.CurrentCell = table.Rows[e.RowIndex].Cells[e.ColumnIndex];
                LoadEditorImage(table.Rows[e.RowIndex].Cells[0].Value as Image, true);
                texturePreview.Invalidate();
                UpdateTextureSelectionStatus(e.RowIndex);
                ctxMenuTable.Show(Cursor.Position);
            }
        }
        private void table_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedRowIndexGlobal = e.RowIndex;
                LoadEditorImage(table.Rows[e.RowIndex].Cells[0].Value as Image, true);
                texturePreview.Invalidate();
                UpdateTextureSelectionStatus(e.RowIndex);
            }
        }
        // Table context menu
        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Get texture name from selected row, and its index
                string extractFileName = table.Rows[selectedRowIndexGlobal].Cells[1].Value.ToString();
                ReadTexture(filepath, selectedRowIndexGlobal);

                // Writing new .tpl file
                string folderName = Path.GetFileNameWithoutExtension(filepath);
                Directory.CreateDirectory("Extracted/" + folderName);
                BinaryWriter bw = new BinaryWriter(File.Open($"Extracted/{folderName}/{extractFileName}", FileMode.Create));

                bw.Write(TPL.magic);
                bw.Write((uint)0x01);
                bw.Write(TPL.startOffset);
                bw.Write(TPL.unused1);
                bw.Write(TPL.width);
                bw.Write(TPL.height);
                bw.Write(TPL.bitDepth);
                bw.Write(TPL.interlace);
                bw.Write(TPL.zPriority);
                bw.Write((short)0x00); // Nulls mipmap count for viewing on GGS
                bw.Write(TPL.scale);
                bw.Write(TPL.unused2);
                bw.Write((uint)(0x00));
                bw.Write((uint)(0x00));
                bw.Write((uint)(0x00));
                bw.Write((uint)(0x00));
                bw.Write((uint)0x40);
                if (TPL.bitDepth == 8)
                {
                    bw.Write((TPL.width * TPL.height) / 2 + 0x40);
                }
                else if (TPL.bitDepth == 9)
                {
                    bw.Write((TPL.width * TPL.height) + 0x40);
                }
                bw.Write(TPL.unused3);
                bw.Write(TPL.config1);
                bw.Write(TPL.config2);
                bw.Write(TPL.config3);
                bw.Write(TPL.unused4);
                bw.Write(TPL.unused5);
                bw.Write(TPL.endTag);
                bw.Write(TPL.pixels);
                bw.Write(TPL.palette);
                bw.Close();

                UpdateStatusText("Texture extracted successfully");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Replace(filepath);
        }
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Duplicate();
        }
        private void pNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderName = Path.GetFileNameWithoutExtension(filepath);
            Image a = new Bitmap(texturePreview.Image);
            a.Save($"Converted/{folderName}/{table.Rows[selectedRowIndexGlobal].Cells[1].Value}.png", ImageFormat.Png);
            UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
        }
        private void bMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderName = Path.GetFileNameWithoutExtension(filepath);
            int colorCount = 256;
            Image image = new Bitmap(texturePreview.Image);

            if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() == "4-bit")
            {
                colorCount = 16;
            }

            IColorQuantizer colorQuantizer = new OctreeQuantizer();
            Image target256 = ImageBuffer.QuantizeImage(image, colorQuantizer, colorCount, 4);
            target256.Save($"Converted/{folderName}/{table.Rows[selectedRowIndexGlobal].Cells[1].Value}.bmp", ImageFormat.Bmp);
            UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
        }
        private void tGAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string folderName = Path.GetFileNameWithoutExtension(filepath);
            var tga = TGASharpLib.TGA.FromBitmap(new Bitmap(texturePreview.Image));
            tga.Save($"Converted/{folderName}/{table.Rows[selectedRowIndexGlobal].Cells[1].Value}.tga");
            UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(table.Rows[selectedRowIndexGlobal].Cells[7].Value.ToString()) > 0)
                {
                    MessageBox.Show("You must remove mipmaps first, before removing this texture.", "Error: mipmaps present",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    Remove();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void removeMipmapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveMipmaps();
        }
        private void increaseColorDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (table.Rows.Count == 0) return;
            if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() != "8-bit")
                ReplaceCurrentPreviewAtColorDepth(256);
            else
                MessageBox.Show("Texture is already at maximum colors allowed (256).", "Cannot increase bit depth", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void decreaseColorDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (table.Rows.Count == 0) return;
            if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() != "4-bit")
                ReplaceCurrentPreviewAtColorDepth(16);
            else
                MessageBox.Show("Texture is already at minimum colors allowed (16).", "Cannot decrease bit depth", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        // Menu: Help
        private void usabilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Steps to use this tool:\n" +
                "1 - Open your .tpl file using the menu or create one from scratch;\n\n" +
                "2 - Choose the option you want by right-clicking a row in the table." +
                " You can add new textures, extract all, convert, replace, remove and etc...;\n\n" +
                "3 - If you want to create a new .tpl using every .tpl inside a folder," +
                "just click on 'Compile from folder...' from Tools menu;\n\n" +
                "4 - If you use the editor on the right side, click on Apply Changes to rebuild the .tpl.\n\n\n" +
                "Tips:\nYou can add multiple textures at once;\n" +
                "After extracting, textures names ending with underscore _ means it's a mipmap.", "Help");
        }
        private void problemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Table not showing all textures:\n" +
                "Reopen the .tpl to quick fix, or click on 'Refresh table' in the File menu.\n\n" +
                "Cannot replace texture:\n" +
                "Images above 8-bit (like 32-bit, 24-bit and 16-bit) are not fully supported. " +
                "Always try to use images with indexed palette.\n\n" +
                "Texture is not applied correctly in the character/weapon/model:\n" +
                "PS2 has a limited hardware, so keep in mind to use resolution lower than 1024px. " +
                "Textures with 'Inverted' interlace are always horizontal oriented (90º clockwise).\n\n" +
                "Texture is flickering in-game:\n" +
                "In the Options menu, activate 'Show hidden' then put 16384 on Scale field.\n\n" +
                "Texture is disappearing/overlapping in some camera angles:\n" +
                "In the Options menu, activate 'Show hidden' then put 4096 on Z-Priority field.", "Help"
                , MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // v1.1.3: no fixed temporary application directory is created or cleaned up.
            Console.WriteLine();
        }
        // Editor
        private void trackBrightness_ValueChanged(object sender, EventArgs e) { SyncNumericFromTrack(trackBrightness, spinBrightness); }
        private void trackContrast_ValueChanged(object sender, EventArgs e) { SyncNumericFromTrack(trackContrast, spinContrast); }
        private void trackSaturation_ValueChanged(object sender, EventArgs e) { SyncNumericFromTrack(trackSaturation, spinSaturation); }
        private void trackHue_ValueChanged(object sender, EventArgs e) { SyncNumericFromTrack(trackHue, spinHue); }
        private void trackSharpen_ValueChanged(object sender, EventArgs e) { SyncNumericFromTrack(trackSharpen, spinSharpen); }
        private void trackPixelate_ValueChanged(object sender, EventArgs e) { SyncNumericFromTrack(trackPixelate, spinPixelate); }

        private void spinBrightness_ValueChanged(object sender, EventArgs e) { SyncTrackFromNumeric(trackBrightness, spinBrightness); RebuildEditorPreview(); }
        private void spinContrast_ValueChanged(object sender, EventArgs e) { SyncTrackFromNumeric(trackContrast, spinContrast); RebuildEditorPreview(); }
        private void spinSaturation_ValueChanged(object sender, EventArgs e) { SyncTrackFromNumeric(trackSaturation, spinSaturation); RebuildEditorPreview(); }
        private void spinSharpen_ValueChanged(object sender, EventArgs e) { SyncTrackFromNumeric(trackSharpen, spinSharpen); RebuildEditorPreview(); }
        private void spinPixelate_ValueChanged(object sender, EventArgs e) { SyncTrackFromNumeric(trackPixelate, spinPixelate); RebuildEditorPreview(); }
        private void spinHue_ValueChanged(object sender, EventArgs e) { SyncTrackFromNumeric(trackHue, spinHue); RebuildEditorPreview(); }

        private void SyncTrackFromNumeric(TrackBar track, NumericUpDown numeric)
        {
            if (suppressEditorEvents || track == null) return;
            int value = Math.Max(track.Minimum, Math.Min(track.Maximum, Decimal.ToInt32(numeric.Value)));
            if (track.Value != value)
            {
                suppressEditorEvents = true;
                track.Value = value;
                suppressEditorEvents = false;
            }
        }

        private void SyncNumericFromTrack(TrackBar track, NumericUpDown numeric)
        {
            if (suppressEditorEvents) return;
            decimal value = Math.Max((decimal)numeric.Minimum, Math.Min((decimal)numeric.Maximum, track.Value));
            if (numeric.Value != value)
            {
                suppressEditorEvents = true;
                numeric.Value = value;
                suppressEditorEvents = false;
            }
            RebuildEditorPreview();
        }

        private void LoadEditorImage(Image image, bool resetAdjustments)
        {
            if (image == null) return;
            if (editorBaseImage != null) editorBaseImage.Dispose();
            editorBaseImage = new Bitmap(image);
            if (resetAdjustments) ResetAdjustmentValues();
            editorHasChanges = false;
            RenderEditorPreview(false);
        }

        private void ResetAdjustmentValues()
        {
            suppressEditorEvents = true;
            spinBrightness.Value = 0;
            spinContrast.Value = 0;
            spinSaturation.Value = 0;
            spinSharpen.Value = 0;
            spinPixelate.Value = 0;
            spinHue.Value = 0;
            foreach (TrackBar track in new[] { trackBrightness, trackContrast, trackSaturation, trackSharpen, trackPixelate, trackHue })
                if (track != null) track.Value = 0;
            suppressEditorEvents = false;
        }

        private Bitmap RunImageProcessor(Bitmap source, Action<ImageFactory> operation)
        {
            using (ImageFactory factory = new ImageFactory())
            {
                factory.Load(new Bitmap(source));
                operation(factory);
                return new Bitmap(factory.Image);
            }
        }

        private void RebuildEditorPreview()
        {
            if (suppressEditorEvents || editorBaseImage == null) return;
            RenderEditorPreview(true);
        }

        private void RenderEditorPreview(bool markChanged)
        {
            if (editorBaseImage == null) return;
            Bitmap rendered = new Bitmap(editorBaseImage);
            try
            {
                if (spinBrightness.Value != 0) { Bitmap next = RunImageProcessor(rendered, f => f.Brightness((int)spinBrightness.Value)); rendered.Dispose(); rendered = next; }
                if (spinContrast.Value != 0) { Bitmap next = RunImageProcessor(rendered, f => f.Contrast((int)spinContrast.Value)); rendered.Dispose(); rendered = next; }
                if (spinSaturation.Value != 0) { Bitmap next = RunImageProcessor(rendered, f => f.Saturation((int)spinSaturation.Value)); rendered.Dispose(); rendered = next; }
                if (spinHue.Value != 0) { Bitmap next = RunImageProcessor(rendered, f => f.Hue((int)spinHue.Value)); rendered.Dispose(); rendered = next; }
                if (spinSharpen.Value > 0) { Bitmap next = RunImageProcessor(rendered, f => f.GaussianSharpen((int)spinSharpen.Value)); rendered.Dispose(); rendered = next; }
                if (spinPixelate.Value > 0) { Bitmap next = RunImageProcessor(rendered, f => f.Pixelate((int)spinPixelate.Value)); rendered.Dispose(); rendered = next; }

                Image old = texturePreview.Image;
                texturePreview.Image = rendered;
                if (old != null && !ReferenceEquals(old, editorBaseImage) && !IsTableThumbnail(old)) old.Dispose();
                if (markChanged) editorHasChanges = true;
                UpdateEditorChangesLabel();
            }
            catch
            {
                rendered.Dispose();
                throw;
            }
        }

        private bool IsTableThumbnail(Image image)
        {
            foreach (DataGridViewRow row in table.Rows)
                if (ReferenceEquals(row.Cells[0].Value, image)) return true;
            return false;
        }

        private void UpdateEditorChangesLabel()
        {
            if (editorChangesLabel == null) return;
            editorChangesLabel.Text = editorHasChanges ? "● Unsaved changes" : "No pending changes";
            button1.Text = editorHasChanges ? "✓ Apply *" : "✓ Apply";
            editorChangesLabel.ForeColor = editorHasChanges ? Color.Gainsboro : Color.Gray;
        }

        private void TransformEditorBase(Action<Bitmap> transform)
        {
            if (editorBaseImage == null) return;
            Bitmap next = new Bitmap(editorBaseImage);
            transform(next);
            editorBaseImage.Dispose();
            editorBaseImage = next;
            editorHasChanges = true;
            RenderEditorPreview(false);
        }

        private void btnRotate_Click_1(object sender, EventArgs e) { TransformEditorBase(b => b.RotateFlip(RotateFlipType.Rotate90FlipNone)); }
        private void btnFlipX_Click(object sender, EventArgs e) { TransformEditorBase(b => b.RotateFlip(RotateFlipType.RotateNoneFlipX)); }
        private void btnFlipY_Click(object sender, EventArgs e) { TransformEditorBase(b => b.RotateFlip(RotateFlipType.RotateNoneFlipY)); }

        private void btnResize_Click(object sender, EventArgs e)
        {
            if (editorBaseImage == null) return;

            using (DialogResizeTexture dialogResize = new DialogResizeTexture(editorBaseImage))
            {
                if (dialogResize.ShowDialog(this) != DialogResult.OK)
                    return;

                Bitmap next = ResizeEditorImage(editorBaseImage, dialogResize.TargetWidth, dialogResize.TargetHeight, dialogResize.Resampling);
                editorBaseImage.Dispose();
                editorBaseImage = next;
                editorHasChanges = true;
                RenderEditorPreview(false);
                UpdateStatusText($"Texture resized to {dialogResize.TargetWidth}×{dialogResize.TargetHeight} • {dialogResize.Resampling}");
            }
        }

        private Bitmap ResizeEditorImage(Bitmap source, int width, int height, ResizeResampling resampling)
        {
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                graphics.InterpolationMode = resampling == ResizeResampling.NearestNeighbor
                    ? System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor
                    : resampling == ResizeResampling.Bilinear
                        ? System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear
                        : System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(source, new Rectangle(0, 0, width, height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel);
            }
            return result;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (filepath != "" && table.Rows.Count > 0)
            {
                LoadEditorImage(table.Rows[selectedRowIndexGlobal].Cells[0].Value as Image, true);
                UpdateStatusText("Texture editor reset to original preview");
            }
        }

        private void btnApplyChanges_Click_1(object sender, EventArgs e)
        {
            if (texturePreview.Image == null) return;

            if (texturePreview.Image.Width.ToString() != table.Rows[selectedRowIndexGlobal].Cells[2].Value.ToString() ||
                texturePreview.Image.Height.ToString() != table.Rows[selectedRowIndexGlobal].Cells[3].Value.ToString())
            {
                DialogResult result = MessageBox.Show("Image dimensions are different from original image, do you want to insert it anyway?",
                    "Different dimension detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result != DialogResult.OK) return;
            }

            EnsureBackup();
            int colorCount = table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() == "8-bit" ? 256 : 16;
            TPLDefinition.TPL destination = tplReader.ReadTexture(filepath, selectedRowIndexGlobal);
            TPLDefinition.TPL replacement = textureEncoder.EncodeImage(texturePreview.Image, colorCount, destination.interlace);
            tplWriter.ReplaceTexture(filepath, selectedRowIndexGlobal, replacement);
            if (destination.mipmapCount > 0) mipmapService.Regenerate(filepath, selectedRowIndexGlobal);
            editorHasChanges = false;
            UpdateStatusText("Texture changes applied successfully");
            RefreshTableAndKeepSelection(selectedRowIndexGlobal);
        }

        // Texture preview
        private void texturePreview_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && texturePreview.Image != null)
            {
                ctxPreviewImage.Show(Cursor.Position);
            }
        }
        private void swapTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.bmp;*.tga)|*.png;*.bmp;*.tga";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (openFileDialog.FileName != "")
            {
                // If it's TGA, then convert to bitmap
                if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".tga")
                {
                    TGASharpLib.TGA tga = new TGASharpLib.TGA(openFileDialog.FileName);
                    LoadEditorImage(new Bitmap(tga.ToBitmap()), true);
                    return;
                }

                using (Bitmap imported = new Bitmap(openFileDialog.FileName)) LoadEditorImage(imported, true);
                openFileDialog.Dispose();
            }
        }

        private void pNGToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string folderName = Path.GetFileNameWithoutExtension(filepath);
            Image a = new Bitmap(texturePreview.Image);
            a.Save($"Converted/{folderName}/{table.Rows[selectedRowIndexGlobal].Cells[1].Value}.png", ImageFormat.Png);
            UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
        }
        private void bMPToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string folderName = Path.GetFileNameWithoutExtension(filepath);
            int colorCount = 256;
            Image image = new Bitmap(texturePreview.Image);

            if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() == "4-bit")
            {
                colorCount = 16;
            }

            IColorQuantizer colorQuantizer = new OctreeQuantizer();
            Image target256 = ImageBuffer.QuantizeImage(image, colorQuantizer, colorCount, 4);
            target256.Save($"Converted/{folderName}/{table.Rows[selectedRowIndexGlobal].Cells[1].Value}.bmp", ImageFormat.Bmp);
            UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
        }
        private void tGAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string folderName = Path.GetFileNameWithoutExtension(filepath);
            var tga = TGASharpLib.TGA.FromBitmap(new Bitmap(texturePreview.Image));
            tga.Save($"Converted/{folderName}/{table.Rows[selectedRowIndexGlobal].Cells[1].Value}.tga");
            UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
        }

        private void extendViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtendView();
        }
        private void animatedViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Shows a simple window with the image and its specifications
            Form form = new Form();

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(texturePreview.Image.Width, texturePreview.Image.Height);
            pictureBox.Image = texturePreview.Image;

            StatusBar statusBar = new StatusBar();
            statusBar.Panels.Add("Width: " + texturePreview.Image.Width + "px");
            statusBar.Panels.Add("Height: " + texturePreview.Image.Height + "px");
            statusBar.ShowPanels = true;

            form.Text = table.Rows[selectedRowIndexGlobal].Cells[1].Value.ToString();
            form.ShowIcon = false;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Controls.Add(pictureBox);
            form.Controls.Add(statusBar);
            form.Size = new Size(texturePreview.Image.Width + 25, texturePreview.Image.Height + 65);
            form.Show();

            // Timer to cycle through all textures
            timer = new System.Timers.Timer(100);
            timer.AutoReset = true;
            int count = 0;
            UpdateStatusText("Timer running in background...");
            timer.Elapsed += async (source, eventArgs) =>
            {
                if (count == table.Rows.Count)
                {
                    count = 0;
                }
                pictureBox.Image = (Bitmap)table.Rows[count].Cells[0].Value;
                count++;
                await Task.Delay(50);
            };
            timer.Start();
            stopBackgroundTaskToolStripMenuItem.Visible = true;
        }

        private void convertAllToBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(filepath)) return;
            try
            {
                ConverterBMP converterBMP = new ConverterBMP();
                converterBMP.TPLtoBMP(filepath, "BMP");
                UpdateStatusText($"Textures converted at folder 'Converted/{Path.GetFileNameWithoutExtension(filepath)}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Convert to BMP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void convertAllToTGAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                string folderName = Path.GetFileNameWithoutExtension(filepath);
                UpdateStatusText("Exporting all textures to TGA...");
                foreach (DataGridViewRow row in table.Rows)
                {
                    var tga = TGASharpLib.TGA.FromBitmap((Bitmap)table.Rows[row.Index].Cells[0].Value);
                    tga.Save($"Converted/{folderName}/{table.Rows[row.Index].Cells[1].Value}.tga");
                }
                UpdateStatusText($"All textures exported to TGA at 'Converted/{folderName}'");
            }
        }

        private void btnFilterComic_Click(object sender, EventArgs e)
        {
            if (editorBaseImage == null) return;
            Bitmap next = RunImageProcessor(editorBaseImage, f => f.Filter(MatrixFilters.Comic));
            editorBaseImage.Dispose();
            editorBaseImage = next;
            editorHasChanges = true;
            RenderEditorPreview(false);
        }

        private void btnFilterGray_Click(object sender, EventArgs e)
        {
            if (editorBaseImage == null) return;
            Bitmap next = RunImageProcessor(editorBaseImage, f => f.Filter(MatrixFilters.GreyScale));
            editorBaseImage.Dispose();
            editorBaseImage = next;
            editorHasChanges = true;
            RenderEditorPreview(false);
        }

        private void btnFilterColorful_Click(object sender, EventArgs e)
        {
            if (editorBaseImage == null) return;
            Bitmap next = RunImageProcessor(editorBaseImage, f => f.Filter(MatrixFilters.HiSatch));
            editorBaseImage.Dispose();
            editorBaseImage = next;
            editorHasChanges = true;
            RenderEditorPreview(false);
        }

        private void btnFilterInvert_Click(object sender, EventArgs e)
        {
            if (editorBaseImage == null) return;
            Bitmap next = RunImageProcessor(editorBaseImage, f => f.Filter(MatrixFilters.Invert));
            editorBaseImage.Dispose();
            editorBaseImage = next;
            editorHasChanges = true;
            RenderEditorPreview(false);
        }

        private void btnLayerAddMask_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.png;*.bmp;*.tga)|*.png;*.bmp;*.tga";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                if (dialog.FileName != "")
                {
                    DialogResult result = DialogResult.OK;
                    Image image = Image.FromFile(dialog.FileName);
                    // Verifies if selected image has a different aspect ration from the one in the preview
                    if (editorBaseImage == null || image.Width != editorBaseImage.Width || image.Height != editorBaseImage.Height)
                    {
                        result = MessageBox.Show("The selected image has a different size from original," +
                            " the results may not be good.\nProceed anyway?", "Different resolution detected",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    }
                    image.Dispose();

                    if (result == DialogResult.OK)
                    {
                        ImageLayer imageLayer = new ImageLayer();
                        imageLayer.Image = Image.FromFile(dialog.FileName);

                        Bitmap next;
                        using (ImageFactory imageFactory = new ImageFactory())
                        {
                            imageFactory.Load(new Bitmap(editorBaseImage));
                            imageFactory.Mask(imageLayer);
                            next = new Bitmap(imageFactory.Image);
                        }
                        editorBaseImage.Dispose();
                        editorBaseImage = next;
                        editorHasChanges = true;
                        RenderEditorPreview(false);
                        imageLayer.Dispose();
                    }
                }
            }
        }

        private void btnLayerOverlay_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.png;*.bmp;*.tga)|*.png;*.bmp;*.tga";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                if (dialog.FileName != "")
                {
                    DialogResult result = DialogResult.OK;
                    Image image = Image.FromFile(dialog.FileName);
                    // Verifies if selected image has a different aspect ration from the one in the preview
                    if (editorBaseImage == null || image.Width != editorBaseImage.Width || image.Height != editorBaseImage.Height)
                    {
                        result = MessageBox.Show("The selected image has a different size from original," +
                            " the results may not be good.\nProceed anyway?", "Different resolution detected",
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    }
                    image.Dispose();

                    if (result == DialogResult.OK)
                    {
                        ImageLayer imageLayer = new ImageLayer();
                        imageLayer.Image = Image.FromFile(dialog.FileName);

                        Bitmap next;
                        using (ImageFactory imageFactory = new ImageFactory())
                        {
                            imageFactory.Load(new Bitmap(editorBaseImage));
                            imageFactory.Overlay(imageLayer);
                            next = new Bitmap(imageFactory.Image);
                        }
                        editorBaseImage.Dispose();
                        editorBaseImage = next;
                        editorHasChanges = true;
                        RenderEditorPreview(false);
                        imageLayer.Dispose();
                    }
                }
            }
        }

        // Drag and drop

        private void table_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void table_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (Path.GetExtension(fileList[0]).ToLower() == ".tpl")
            {
                filepath = fileList[0];
                btnOpenFile.Dispose();
                btnCreateNewFile.Dispose();
                lblDropHere.Dispose();
                this.Text = AppTitle + " - " + Path.GetFileName(filepath);
                RefreshTable();
            }
            else
            {
                MessageBox.Show($"Invalid {Path.GetExtension(fileList[0])} file format, only .tpl texture files are supported.",
                    "Invalid format", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void extractTPLFromSMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RE4 PS2 SMD Files (*.SMD)|*.SMD";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (openFileDialog.FileNames.Length > 0 && openFileDialog.FileNames[0] != "")
            {
                string filename = "";
                try
                {
                    for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                    {
                        BinaryReader br = new BinaryReader(File.OpenRead(openFileDialog.FileNames[i]));
                        br.BaseStream.Position = 0x08;
                        uint smdTextureOffset = br.ReadUInt32();
                        br.BaseStream.Position = smdTextureOffset + 0x10;
                        byte[] tpl = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                        br.Close();

                        // Create folder
                        if (!Directory.Exists("SMD Textures"))
                        {
                            Directory.CreateDirectory("SMD Textures");
                        }
                        filename = Path.GetFileNameWithoutExtension(openFileDialog.FileNames[i]);

                        BinaryWriter bw = new BinaryWriter(File.Create("SMD Textures/" + filename + ".tpl"));
                        bw.Write(tpl);
                        bw.Close();
                        UpdateStatusText($"TPL extracted from SMD at directory 'SMD Textures/{filename}.tpl'");
                    }
                    DialogResult result = MessageBox.Show("Do you want to open the extracted TPL?", "Question", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        filepath = "SMD Textures/" + filename + ".tpl";
                        btnOpenFile.Dispose();
                        btnCreateNewFile.Dispose();
                        lblDropHere.Dispose();
                        this.Text = AppTitle + " - " + Path.GetFileName(filepath);
                        FillTable();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void importTPLToSMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RE4 PS2 SMD Files (*.SMD)|*.SMD";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            if (openFileDialog.FileName != "")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to inject this TPL into the selected SMD?",
                    "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        BinaryReader br = new BinaryReader(File.OpenRead(openFileDialog.FileName));
                        br.BaseStream.Position = 0x08;
                        uint smdTextureOffset = br.ReadUInt32();

                        // Get top part
                        br.BaseStream.Position = 0x00;
                        byte[] topPart = br.ReadBytes((int)(smdTextureOffset + 0x10));
                        br.Close();

                        // Get bottom part
                        BinaryReader br2 = new BinaryReader(File.OpenRead(filepath));
                        byte[] bottomPart = br2.ReadBytes((int)br2.BaseStream.Length);
                        br2.Close();

                        BinaryWriter bw = new BinaryWriter(File.Create(openFileDialog.FileName));
                        bw.Write(topPart);
                        bw.Write(bottomPart);
                        bw.Close();

                        UpdateStatusText("TPL inserted into SMD successfully");
                        MessageBox.Show("TPL inserted into SMD successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void extractTPLFromEFFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RE4 PS2 EFF Files (*.EFF)|*.EFF";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            byte[] textures;
            using (BinaryReader br = new BinaryReader(File.OpenRead(openFileDialog.FileName)))
            {
                br.BaseStream.Position = 0x18;
                uint effTexturesStartOffset = br.ReadUInt32();
                uint effTexturesEndOffset = br.ReadUInt32();
                if (effTexturesEndOffset < effTexturesStartOffset || effTexturesEndOffset > br.BaseStream.Length)
                    throw new InvalidDataException("Invalid EFF texture chunk offsets.");

                br.BaseStream.Position = effTexturesStartOffset;
                textures = br.ReadBytes((int)(effTexturesEndOffset - effTexturesStartOffset));
            }

            Directory.CreateDirectory("EFF Textures");
            string filename = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

            // HEFF is an embedded header/index. Read it directly from memory instead of creating an intermediate HEFF file.
            using (MemoryStream memory = new MemoryStream(textures, false))
            using (BinaryReader br2 = new BinaryReader(memory))
            {
                if (memory.Length < 4) throw new InvalidDataException("EFF texture header is truncated.");
                uint fileCount = br2.ReadUInt32();

                long offsetTableEnd = 4L + (4L * fileCount);
                if (offsetTableEnd > memory.Length)
                    throw new InvalidDataException("EFF texture offset table is truncated.");

                for (int i = 0; i < fileCount; i++)
                {
                    long offsetEntry = 4L + (4L * i);
                    memory.Position = offsetEntry;
                    uint startOffset = br2.ReadUInt32();

                    uint actualEnd;
                    if (i < fileCount - 1)
                    {
                        actualEnd = br2.ReadUInt32();
                    }
                    else
                    {
                        actualEnd = (uint)memory.Length;
                    }

                    if (startOffset < offsetTableEnd || startOffset > actualEnd || actualEnd > memory.Length)
                        throw new InvalidDataException("Invalid embedded TPL range in EFF file.");

                    memory.Position = startOffset;
                    byte[] tpl = br2.ReadBytes((int)(actualEnd - startOffset));
                    File.WriteAllBytes(Path.Combine("EFF Textures", filename + "_" + i + ".tpl"), tpl);
                }
            }

            UpdateStatusText("EFF textures extracted successfully");
        }

        private void pNGToTPLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "PNG Files (*.png)|*.png";
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                using (Bitmap image = new Bitmap(openFileDialog.FileName))
                {
                    int colorCount = GetImportColorCount(image);
                    TPLDefinition.TPL texture = textureEncoder.EncodeImage(image, colorCount);
                    Directory.CreateDirectory("Converted");
                    string output = Path.Combine("Converted", Path.GetFileNameWithoutExtension(openFileDialog.FileName) + ".tpl");
                    tplWriter.WriteSingleTexture(output, texture);
                    UpdateStatusText("PNG converted to TPL: " + output);
                }
            }
        }


        private void batchReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(filepath)) return;

            BatchColorDepthMode batchMode;
            using (DialogBatchReplaceMode modeDialog = new DialogBatchReplaceMode())
            {
                if (modeDialog.ShowDialog(this) != DialogResult.OK)
                {
                    UpdateStatusText("Batch replace cancelled");
                    return;
                }
                batchMode = modeDialog.SelectedMode;
            }

            using (CommonOpenFileDialog folderDialog = new CommonOpenFileDialog())
            {
                folderDialog.IsFolderPicker = true;
                if (folderDialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    UpdateStatusText("Batch replace cancelled");
                    return;
                }

                var files = Directory.GetFiles(folderDialog.FileName, "*.png")
                    .Where(f =>
                    {
                        int parsedIndex;
                        return Int32.TryParse(Path.GetFileNameWithoutExtension(f), out parsedIndex) && parsedIndex >= 0;
                    })
                    .ToArray();
                int replaced = 0;
                int replaced4Bit = 0;
                int replaced8Bit = 0;
                var failures = new List<string>();
                EnsureBackup();

                string modeLabel = GetBatchModeLabel(batchMode);
                UpdateStatusText($"Batch replace in progress • PNG indexed filenames • Mode: {modeLabel}...");

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string candidate = files.FirstOrDefault(f =>
                        String.Equals(Path.GetFileNameWithoutExtension(f), i.ToString(), StringComparison.OrdinalIgnoreCase));
                    if (candidate == null) continue;

                    try
                    {
                        selectedRowIndexGlobal = i;
                        Replace(filepath, candidate, 0, false, true, batchMode);
                        replaced++;

                        string extension = Path.GetExtension(candidate).ToLowerInvariant();
                        if (extension != ".tpl")
                        {
                            int resultingColorCount;
                            if (batchMode == BatchColorDepthMode.Force4Bit) resultingColorCount = 16;
                            else if (batchMode == BatchColorDepthMode.Force8Bit) resultingColorCount = 256;
                            else
                            {
                                string targetDepth = Convert.ToString(table.Rows[i].Cells[4].Value) ?? "";
                                resultingColorCount = targetDepth == "4-bit" ? 16 : 256;
                            }

                            if (resultingColorCount == 16) replaced4Bit++;
                            else replaced8Bit++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failures.Add(i + ": " + ex.Message);
                    }
                }

                RefreshTableAndKeepSelection(selectedRowIndexGlobal);
                UpdateStatusText($"Batch replace completed • {replaced} replaced • Mode: {modeLabel}" +
                    (replaced4Bit + replaced8Bit > 0 ? $" • {replaced4Bit} 4-bit / {replaced8Bit} 8-bit" : "") +
                    (failures.Count > 0 ? $" • {failures.Count} failed" : ""));

                if (failures.Count > 0)
                {
                    MessageBox.Show("Some textures could not be replaced:\n\n" + String.Join("\n", failures.Take(12)),
                        "Batch Replace", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static string GetBatchModeLabel(BatchColorDepthMode mode)
        {
            switch (mode)
            {
                case BatchColorDepthMode.Force4Bit:
                    return "Force 4-bit";
                case BatchColorDepthMode.Force8Bit:
                    return "Force 8-bit";
                default:
                    return "Preserve TPL color depth";
            }
        }

        private string RecentFilesPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RE4_PS2_TPL_Manager", "recent.txt");
        private void InitializeRecentFilesMenu()
        {
            recentFilesMenu = new ToolStripMenuItem("Recent files");
            fileToolStripMenuItem.DropDownItems.Insert(Math.Min(3, fileToolStripMenuItem.DropDownItems.Count), recentFilesMenu);
            DarkTheme.ApplyToToolStripItem(recentFilesMenu);
            LoadRecentFiles(); RefreshRecentFilesMenu();
        }
        private void LoadRecentFiles()
        {
            try { if (File.Exists(RecentFilesPath)) recentFiles.AddRange(File.ReadAllLines(RecentFilesPath).Where(x => !String.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).Take(10)); } catch { }
        }
        private void SaveRecentFiles()
        {
            try { Directory.CreateDirectory(Path.GetDirectoryName(RecentFilesPath)); File.WriteAllLines(RecentFilesPath, recentFiles.Take(10)); } catch { }
        }
        private void AddRecentFile(string path)
        {
            if (String.IsNullOrWhiteSpace(path)) return;
            recentFiles.RemoveAll(x => String.Equals(x, path, StringComparison.OrdinalIgnoreCase)); recentFiles.Insert(0, path);
            if (recentFiles.Count > 10) recentFiles.RemoveRange(10, recentFiles.Count - 10); SaveRecentFiles(); RefreshRecentFilesMenu();
        }
        private void RefreshRecentFilesMenu()
        {
            if (recentFilesMenu == null) return; recentFilesMenu.DropDownItems.Clear();
            foreach (string path in recentFiles.ToArray())
            {
                var item = new ToolStripMenuItem(Path.GetFileName(path)) { ToolTipText = path, Tag = path };
                item.Click += (o, a) => OpenRecentFile(Convert.ToString(((ToolStripMenuItem)o).Tag));
                recentFilesMenu.DropDownItems.Add(item);
                DarkTheme.ApplyToToolStripItem(item);
            }
            if (recentFilesMenu.DropDownItems.Count == 0)
            {
                var emptyItem = new ToolStripMenuItem("(empty)") { Enabled = false };
                recentFilesMenu.DropDownItems.Add(emptyItem);
                DarkTheme.ApplyToToolStripItem(emptyItem);
            }
            DarkTheme.ApplyToToolStripItem(recentFilesMenu);
        }
        private void OpenRecentFile(string path)
        {
            if (!File.Exists(path)) { recentFiles.RemoveAll(x => String.Equals(x,path,StringComparison.OrdinalIgnoreCase)); SaveRecentFiles(); RefreshRecentFilesMenu(); MessageBox.Show("This recent file no longer exists.", "Recent files", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            filepath=path; backupCreatedForCurrentFile=false; AddRecentFile(path); this.Text=AppTitle+" - "+Path.GetFileName(filepath);
            btnOpenFile.Dispose(); btnCreateNewFile.Dispose(); lblDropHere.Dispose(); UpdateStatusText("Opening "+Path.GetFileName(filepath)+"..."); FillTable();
        }
        private void EnsureBackup()
        {
            if (backupCreatedForCurrentFile || String.IsNullOrWhiteSpace(filepath) || !File.Exists(filepath)) return;
            try
            {
                string backup = filepath + ".bak";
                File.Copy(filepath, backup, true); backupCreatedForCurrentFile = true;
                UpdateStatusText("Backup created: " + Path.GetFileName(backup));
            }
            catch (Exception ex) { throw new IOException("Could not create the automatic backup before modifying the TPL. " + ex.Message, ex); }
        }
    }
}
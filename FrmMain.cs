using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using Microsoft.WindowsAPICodePack.Dialogs;
using RE4_PS2_TPL_Manager.Dialog;
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
        // Structs
        TPLDefinition.TPL TPL;
        MipMap MipMap;
        TPLDefinition TPLClass = new TPLDefinition();

        // Global
        OpenFileDialog dialog = new OpenFileDialog();
        string filepath = "";
        int selectedRowIndexGlobal = 0;

        public FrmMain()
        {
            InitializeComponent();
            ColorSlider.ColorSlider slider = new ColorSlider.ColorSlider();
            slider.Location = new Point(50, 50);
            slider.Maximum = 100;
            slider.Minimum = -100;

            // Directory used for temporary creating and deleting files
            string path = ".temp";
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }
        public FrmMain(string tplFile)
        {
            InitializeComponent();
            filepath = tplFile;
            try
            {
                FillTable();
                btnOpenFile.Dispose();
                btnCreateNewFile.Dispose();
                this.Text = "RE4 PS2 TPL Manager - " + Path.GetFileName(filepath);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void CreateEmptyTPL()
        {
            BinaryWriter bw = new BinaryWriter(File.Open("temp.tpl", FileMode.Create));
            bw.Write((uint)4096);
            bw.Write((uint)0x00);
            bw.Write((uint)0x10);
            bw.Write((uint)0x00);
            bw.Close();

            filepath = "temp.tpl";
            btnCreateNewFile.Dispose();
            btnOpenFile.Dispose();
            if (filepath != "")
            {
                this.Text = "RE4 PS2 TPL Manager - " + Path.GetFileName(filepath);
                FillTable();
            }
        }
        private void UpdateAllOffsets(string tplFilename)
        {
            BinaryReader br = new BinaryReader(File.Open(tplFilename, FileMode.Open));
            TPL.magic = br.ReadUInt32();
            TPL.tplCount = br.ReadUInt32();
            TPL.startOffset = br.ReadUInt32();
            TPL.unused1 = br.ReadUInt32();
            int chunk = 0x00;
            int totalMipmaps = 0x00;

            Console.WriteLine("Updating " + TPL.tplCount + " texture offsets");

            // Iterates through each header to get mipmap quantity
            for (int i = 0; i < TPL.tplCount; i++)
            {
                br.BaseStream.Position = 0x1A + chunk;
                ushort mipmapCount = br.ReadUInt16();

                if (mipmapCount > 0)
                {
                    totalMipmaps += mipmapCount;
                }
                chunk += 0x30;
            }
            br.Close();

            uint tempAcumulatorPixels = 0x00;
            uint tempAcumulatorPalette = 0x00;
            uint tempAcumulatorMipMapHeader = 0x00;
            uint tempAcumulatorMipMapPixels = 0x00;

            for (int i = 0; i < TPL.tplCount + totalMipmaps; i++)
            {
                BinaryReader br2 = new BinaryReader(File.Open(tplFilename, FileMode.Open));
                br2.BaseStream.Position = 0x10 + (0x30 * i);
                TPL.width = br2.ReadUInt16();
                TPL.height = br2.ReadUInt16();
                TPL.bitDepth = br2.ReadUInt16();
                TPL.interlace = br2.ReadUInt16();
                TPL.zPriority = br2.ReadUInt16();
                TPL.mipmapCount = br2.ReadUInt16();

                br2.BaseStream.Position = 0x30 + (0x30 * i);
                TPL.pixelsOffset = br2.ReadUInt32();
                TPL.paletteOffset = br2.ReadUInt32();

                br2.BaseStream.Position = 0x20 + (0x30 * i);
                TPL.mipmapOffset1 = br2.ReadUInt32();
                TPL.mipmapOffset2 = br2.ReadUInt32();

                // Get mipmap 1 parameters
                br2.BaseStream.Position = TPL.mipmapOffset1;
                ushort mipmapWidth1 = br2.ReadUInt16();
                ushort mipmapHeight1 = br2.ReadUInt16();
                ushort mipmapBitDepth1 = br2.ReadUInt16();
                br2.BaseStream.Position = TPL.mipmapOffset1 + 0x20;
                uint mipmapPixelsOffset1 = br2.ReadUInt32();

                // Get mipmap 2 parameters
                br2.BaseStream.Position = TPL.mipmapOffset2;
                ushort mipmapWidth2 = br2.ReadUInt16();
                ushort mipmapHeight2 = br2.ReadUInt16();
                ushort mipmapBitDepth2 = br2.ReadUInt16();
                br2.BaseStream.Position = TPL.mipmapOffset2 + 0x20;
                uint mipmapPixelsOffset2 = br2.ReadUInt32();
                br2.Close();

                BinaryWriter bw = new BinaryWriter(File.Open(tplFilename, FileMode.Open));

                // Updates mipmap headers offsets
                if (TPL.mipmapCount > 0 && i < TPL.tplCount)
                {
                    bw.BaseStream.Position = 0x20 + (0x30 * i);
                    for (int h = 0; h < TPL.mipmapCount; h++)
                    {
                        bw.Write((uint)((0x30 * TPL.tplCount) + 0x10 + tempAcumulatorMipMapHeader));
                        tempAcumulatorMipMapHeader += 0x30;
                    }
                }

                bw.BaseStream.Position = 0x30 + (0x30 * i);
                // Pixels offset

                bw.Write((uint)(tempAcumulatorPixels + tempAcumulatorPalette + tempAcumulatorMipMapPixels +
                    (0x30 * (TPL.tplCount + totalMipmaps)) + 0x10));

                // Acumulator pixels
                if (TPL.bitDepth == 8)
                    tempAcumulatorPixels += (uint)((TPL.width * TPL.height) / 2);
                else
                    tempAcumulatorPixels += (uint)(TPL.width * TPL.height);

                // Palette offset (only updates on textures, not on mipmaps)
                if (!(i >= TPL.tplCount))
                {
                    bw.Write((uint)(tempAcumulatorPixels + tempAcumulatorPalette + tempAcumulatorMipMapPixels +
                        (0x30 * (TPL.tplCount + totalMipmaps)) + 0x10));
                }

                // Acumulator palette
                if (TPL.bitDepth == 8)
                    tempAcumulatorPalette += 0x80;
                else
                    tempAcumulatorPalette += 0x400;

                bw.Close();
            }

            // -----------------------------
            // MIPMAPS
            // Used to update all mipmap offsets
            int offsetAcumulator = 0;
            for (int i = 0; i < totalMipmaps; i++)
            {
                // Getting last texture metadata
                BinaryReader brMip = new BinaryReader(File.Open(filepath, FileMode.Open));
                brMip.BaseStream.Position = 0x14 + (0x30 * (TPL.tplCount - 1));
                TPL.bitDepth = brMip.ReadUInt16();
                brMip.BaseStream.Position = 0x34 + (0x30 * (TPL.tplCount - 1));
                TPL.paletteOffset = brMip.ReadUInt32();

                // Get mipmap metadata
                brMip.BaseStream.Position = 0x10 + (0x30 * (TPL.tplCount + i));
                MipMap.width = brMip.ReadUInt16();
                MipMap.height = brMip.ReadUInt16();
                MipMap.bitDepth = brMip.ReadUInt16();
                brMip.Close();

                // Update offsets
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(filepath));
                bw.BaseStream.Position = 0x30 + (0x30 * (TPL.tplCount + i));
                switch (TPL.bitDepth)
                {
                    case 8:
                        TPL.paletteOffset += 0x80;
                        bw.Write(TPL.paletteOffset + offsetAcumulator);
                        offsetAcumulator += MipMap.width * MipMap.height / 2;
                        break;
                    case 9:
                        TPL.paletteOffset += 0x400;
                        bw.Write(TPL.paletteOffset + offsetAcumulator);
                        offsetAcumulator += MipMap.width * MipMap.height;
                        break;
                    default:
                        break;
                }
                bw.Close();
            }
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
            if (filepath != "")
            {
                // -----------------
                // SPLITS LOADED FILE INTO TWO PARTS (TOP & BOTTOM)
                // -----------------
                BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));

                TPL.magic = br.ReadUInt32();
                TPL.tplCount = br.ReadUInt32();
                TPL.startOffset = br.ReadUInt32();
                TPL.unused1 = br.ReadUInt32();
                int chunk = 0x00;
                int totalMipmaps = 0x00;

                // Iterates through each header to get mipmap quantity
                for (int i = 0; i < TPL.tplCount; i++)
                {
                    br.BaseStream.Position = 0x1A + chunk;
                    ushort mipmapCount = br.ReadUInt16();

                    if (mipmapCount > 0)
                    {
                        totalMipmaps += mipmapCount;
                    }
                    chunk += 0x30;
                }

                // Get top part of file (headers)
                br.BaseStream.Position = 0;
                byte[] topPart = br.ReadBytes((int)(0x10 + (0x30 * (TPL.tplCount + totalMipmaps))));
                byte[] bottomPart = new byte[0];

                if (TPL.tplCount > 0)
                {
                    // Get bottom part of file (data)
                    br.BaseStream.Position = 0x30;
                    uint pixelsStart = br.ReadUInt32();

                    br.BaseStream.Position = pixelsStart;
                    bottomPart = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                }

                br.Close();

                // -----------------
                // GETS SELECTED NEW TPL FILES
                // -----------------
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "RE4 PS2 TPL Files (*.tpl)|*.tpl";
                dialog.Multiselect = true;
                dialog.ShowDialog();

                List<byte[]> headers = new List<byte[]>();
                List<byte[]> pixels = new List<byte[]>();
                List<byte[]> palettes = new List<byte[]>();
                List<byte[]> mipmapHeaders = new List<byte[]>();
                List<byte[]> mipmapPixels = new List<byte[]>();

                // Iterates through each selected file
                for (int tpl = 0; tpl < dialog.FileNames.Length; tpl++)
                {
                    BinaryReader brTpl = new BinaryReader(File.Open(dialog.FileNames[tpl], FileMode.Open));
                    TPL.magic = brTpl.ReadUInt32();
                    TPL.tplCount = brTpl.ReadUInt32();
                    TPL.startOffset = brTpl.ReadUInt32();
                    TPL.unused1 = brTpl.ReadUInt32();
                    chunk = 0x00;
                    totalMipmaps = 0x00;

                    // Iterates through each texture inside .tpl
                    for (int texture = 0; texture < TPL.tplCount; texture++)
                    {
                        // Returns to header position on each iteration
                        brTpl.BaseStream.Position = 0x10 + (0x30 * texture);
                        TPL.width = brTpl.ReadUInt16();
                        TPL.height = brTpl.ReadUInt16();
                        TPL.bitDepth = brTpl.ReadUInt16();
                        TPL.interlace = brTpl.ReadUInt16();
                        TPL.zPriority = brTpl.ReadUInt16();
                        TPL.mipmapCount = brTpl.ReadUInt16();
                        TPL.scale = brTpl.ReadUInt16();
                        TPL.unused2 = brTpl.ReadUInt16();

                        TPL.mipmapOffset1 = brTpl.ReadUInt32();
                        TPL.mipmapOffset2 = brTpl.ReadUInt32();
                        TPL.unknown1 = brTpl.ReadUInt32();
                        TPL.unknown2 = brTpl.ReadUInt32();

                        TPL.pixelsOffset = brTpl.ReadUInt32();
                        TPL.paletteOffset = brTpl.ReadUInt32();

                        Console.WriteLine("Arquivo: " + tpl);
                        // Iterates through each header to get mipmap quantity (only 1 time to get all)
                        if (texture == 0)
                        {
                            for (int i = 0; i < TPL.tplCount; i++)
                            {
                                brTpl.BaseStream.Position = 0x1A + chunk;
                                ushort mipmapCount = brTpl.ReadUInt16();
                                if (mipmapCount > 0)
                                {
                                    totalMipmaps += mipmapCount;
                                }
                                chunk += 0x30;
                                Console.WriteLine("Mipmaps: " + totalMipmaps);
                            }
                        }

                        // Get all texture headers
                        brTpl.BaseStream.Position = 0x10 + (0x30 * texture);
                        headers.Add(brTpl.ReadBytes(0x30));

                        // Get all mipmap headers
                        if (totalMipmaps > 0 && texture == TPL.tplCount - 1)
                        {
                            Console.WriteLine("Adding mipmap headers...");
                            mipmapHeaders.Add(brTpl.ReadBytes((int)(0x30 * totalMipmaps)));
                        }

                        // Get all pixels [textures]
                        brTpl.BaseStream.Position = TPL.pixelsOffset;
                        if (TPL.bitDepth == 8)
                        {
                            pixels.Add(brTpl.ReadBytes((TPL.width * TPL.height) / 2));
                        }
                        else
                        {
                            pixels.Add(brTpl.ReadBytes(TPL.width * TPL.height));
                        }

                        // Get all palettes [textures]
                        brTpl.BaseStream.Position = TPL.paletteOffset;
                        if (TPL.bitDepth == 8)
                        {
                            palettes.Add(brTpl.ReadBytes(0x80));
                        }
                        else
                        {
                            palettes.Add(brTpl.ReadBytes(0x400));
                        }

                        // Get all pixels [mipmaps]
                        for (int i = 0; i < TPL.mipmapCount; i++)
                        {
                            if (i == 0)
                            {
                                brTpl.BaseStream.Position = TPL.mipmapOffset1;
                                MipMap.width = brTpl.ReadUInt16();
                                MipMap.height = brTpl.ReadUInt16();
                                brTpl.BaseStream.Position = TPL.mipmapOffset1 + 0x20;
                                brTpl.BaseStream.Position = brTpl.ReadUInt32();

                                if (TPL.bitDepth == 8)
                                {
                                    mipmapPixels.Add(brTpl.ReadBytes((MipMap.width * MipMap.height) / 2));
                                }
                                else
                                {
                                    mipmapPixels.Add(brTpl.ReadBytes(MipMap.width * MipMap.height));
                                }
                            }
                            else
                            {
                                brTpl.BaseStream.Position = TPL.mipmapOffset2;
                                MipMap.width = brTpl.ReadUInt16();
                                MipMap.height = brTpl.ReadUInt16();
                                brTpl.BaseStream.Position = TPL.mipmapOffset2 + 0x20;
                                brTpl.BaseStream.Position = brTpl.ReadUInt32();

                                if (TPL.bitDepth == 8)
                                {
                                    mipmapPixels.Add(brTpl.ReadBytes((MipMap.width * MipMap.height) / 2));
                                }
                                else
                                {
                                    mipmapPixels.Add(brTpl.ReadBytes(MipMap.width * MipMap.height));
                                }
                            }
                        }
                    }
                    brTpl.Close();
                }

                // -----------------
                // WRITES NEW MERGED TPL
                // -----------------
                BinaryWriter bw = new BinaryWriter(File.Open(filepath, FileMode.Create));
                bw.Write(topPart);

                // Updates texture count
                bw.BaseStream.Position = 0x04;
                bw.Write(topPart[4] + headers.Count);
                bw.BaseStream.Position = bw.BaseStream.Length;

                // Write headers
                for (int header = 0; header < headers.Count; header++)
                {
                    bw.Write(headers[header]);
                }

                // Write mipmap headers
                if (includeMipmapToolStripMenuItem.Checked && totalMipmaps > 0)
                {
                    bw.Write(mipmapHeaders[0]);
                }

                bw.Write(bottomPart);

                // Write pixels and palettes
                for (int i = 0; i < pixels.Count; i++)
                {
                    bw.Write(pixels[i]);
                    bw.Write(palettes[i]);
                }

                // Write mipmap pixels
                if (includeMipmapToolStripMenuItem.Checked && totalMipmaps > 0)
                {
                    for (int i = 0; i < mipmapPixels.Count; i++)
                    {
                        bw.Write(mipmapPixels[i]);
                    }
                }

                bw.Close();
                UpdateAllOffsets(filepath);
                FillTable();
            }
        }
        private void CompileFromFolder()
        {
            // Folder Dialog
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            folderDialog.ShowDialog();

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
        }
        private void ReadTexture(string tplFile, int textureIndex)
        {
            BinaryReader br = new BinaryReader(File.Open(tplFile, FileMode.Open));
            TPL.magic = br.ReadUInt32();
            TPL.tplCount = br.ReadUInt32();
            TPL.startOffset = br.ReadUInt32();
            TPL.unused1 = br.ReadUInt32();

            // Get every parameter
            br.BaseStream.Position = 0x10 + (0x30 * textureIndex);
            TPL.width = br.ReadUInt16();
            TPL.height = br.ReadUInt16();
            TPL.bitDepth = br.ReadUInt16();
            TPL.interlace = br.ReadUInt16();
            TPL.zPriority = br.ReadUInt16();
            TPL.mipmapCount = br.ReadUInt16();
            TPL.scale = br.ReadUInt16();
            TPL.unused2 = br.ReadUInt16();

            TPL.mipmapOffset1 = br.ReadUInt32();
            TPL.mipmapOffset2 = br.ReadUInt32();
            TPL.unknown1 = br.ReadUInt32();
            TPL.unknown2 = br.ReadUInt32();

            TPL.pixelsOffset = br.ReadUInt32();
            TPL.paletteOffset = br.ReadUInt32();
            TPL.unused3 = br.ReadByte();
            TPL.config1 = br.ReadByte();
            TPL.config2 = br.ReadByte();
            TPL.config3 = br.ReadUInt16();
            TPL.unused4 = br.ReadByte();
            TPL.unused5 = br.ReadByte();
            TPL.endTag = br.ReadByte();

            // Get whole header into a byte array
            br.BaseStream.Position = 0x10 + (0x30 * textureIndex);
            TPL.header = br.ReadBytes(0x30);

            // Get pixels indices and palette
            br.BaseStream.Position = TPL.pixelsOffset;
            if (TPL.bitDepth == 8)
            {
                TPL.pixels = br.ReadBytes((TPL.width * TPL.height) / 2);
                br.BaseStream.Position = TPL.paletteOffset;
                TPL.palette = br.ReadBytes(0x80);
            }
            else if (TPL.bitDepth == 9)
            {
                TPL.pixels = br.ReadBytes(TPL.width * TPL.height);
                br.BaseStream.Position = TPL.paletteOffset;
                TPL.palette = br.ReadBytes(0x400);
            }
            else
            {
                TPL.pixels = br.ReadBytes(TPL.width * TPL.height);
            }

            // Get mipmap headers and pixels
            if (TPL.mipmapCount > 0)
            {
                // Get header 1
                br.BaseStream.Position = TPL.mipmapOffset1;
                TPL.mipmapHeader1 = br.ReadBytes(0x30);

                // Get pixels 1
                br.BaseStream.Position = TPL.mipmapOffset1;
                MipMap.width = br.ReadUInt16();
                MipMap.height = br.ReadUInt16();
                MipMap.bitDepth = br.ReadUInt16();

                br.BaseStream.Position = TPL.mipmapOffset1 + 0x20;
                MipMap.pixelsOffset = br.ReadUInt32();

                br.BaseStream.Position = MipMap.pixelsOffset;
                if (MipMap.bitDepth == 8)
                {
                    TPL.mipmapPixels1 = br.ReadBytes((MipMap.width * MipMap.height) / 2);
                }
                else if (MipMap.bitDepth == 9 || MipMap.bitDepth == 6)
                {
                    TPL.mipmapPixels1 = br.ReadBytes(MipMap.width * MipMap.height);
                }

                // Get header 2
                br.BaseStream.Position = TPL.mipmapOffset2;
                TPL.mipmapHeader2 = br.ReadBytes(0x30);

                // Get pixels 2
                br.BaseStream.Position = TPL.mipmapOffset2;
                MipMap.width = br.ReadUInt16();
                MipMap.height = br.ReadUInt16();
                MipMap.bitDepth = br.ReadUInt16();

                br.BaseStream.Position = TPL.mipmapOffset2 + 0x20;
                MipMap.pixelsOffset = br.ReadUInt32();

                br.BaseStream.Position = MipMap.pixelsOffset;
                if (MipMap.bitDepth == 8)
                {
                    TPL.mipmapPixels2 = br.ReadBytes((MipMap.width * MipMap.height) / 2);
                }
                else if (MipMap.bitDepth == 9 || MipMap.bitDepth == 6)
                {
                    TPL.mipmapPixels2 = br.ReadBytes(MipMap.width * MipMap.height);
                }

            }
            br.Close();
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
        private void CreateTable()
        {
            table.Columns.Clear();

            // Table config
            DataGridViewImageColumn thumbnail = new DataGridViewImageColumn();
            thumbnail.Name = "thumbnail";
            thumbnail.HeaderText = "Thumbnail";
            table.Columns.Insert(0, thumbnail);

            table.Columns.Add("title", "Title"); // 1
            table.Columns.Add("width", "Width"); // 2
            table.Columns.Add("height", "Height"); // 3

            DataGridViewComboBoxColumn bitDepth = new DataGridViewComboBoxColumn();
            bitDepth.Name = "bitDepth";
            bitDepth.HeaderText = "Bit Depth";
            table.Columns.Insert(4, bitDepth);

            DataGridViewComboBoxColumn interlace = new DataGridViewComboBoxColumn();
            interlace.Name = "interlace";
            interlace.HeaderText = "Interlace";
            table.Columns.Insert(5, interlace);

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
                DialogResult result = MessageBox.Show("This .tpl file contains mipmaps, in order to open it you must rebuild the file.\n" +
                    "Rebuild it now?", "Mipmaps detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    Rebuild();
                }
                else return;
            }

            // Create table columns
            CreateTable();
            ShowThumbnails();

            BinaryReader br = new BinaryReader(File.Open(filepath, FileMode.Open));
            string folderName = Path.GetFileNameWithoutExtension(filepath);

            br.BaseStream.Position = 0x04;
            uint texturesTotal = br.ReadUInt32();
            br.Close();

            // Status bar
            UpdateStatusText($"Loading {texturesTotal} textures, please wait...");
            progressBar.Maximum = (int)texturesTotal;
            progressBar.Value = 0;

            for (int i = 0; i < texturesTotal; i++)
            {
                progressBar.Value++;
                statusStrip1.Invalidate();
                statusStrip1.Refresh();

                // Returns to header position on each iteration
                ReadTexture(filepath, i);

                // Generating image thumbnail
                Image thumbnail;
                if (TPL.bitDepth == 8)
                {
                    thumbnail = Image.FromFile(".temp/" + folderName + "/" + i + "_16.bmp");
                }
                else
                {
                    thumbnail = Image.FromFile(".temp/" + folderName + "/" + i + "_256.bmp");
                }

                // Create combobox of bit depth and interlace modes
                DataGridViewComboBoxCell cbBitDepth = new DataGridViewComboBoxCell();
                cbBitDepth.Items.AddRange(TPLClass.BitDepthDict.Keys.ToArray()[0]);
                DataGridViewComboBoxCell cbInterlace = new DataGridViewComboBoxCell();
                cbInterlace.Items.AddRange(TPLClass.InterlaceDict.Keys.ToArray()[0]);

                // Bit depth selected value
                string GetBitDepth()
                {
                    string temp = "";
                    for (int x = 0; x < TPLClass.BitDepthDict.Values.ToArray()[0].Length; x++)
                    {
                        if (TPL.bitDepth == TPLClass.BitDepthDict.Values.ToArray()[0][x])
                        {
                            temp = TPLClass.BitDepthDict.Keys.ToArray()[0][x];
                        }
                    }
                    return temp;
                }

                // Interlace selected value
                string GetInterlace()
                {
                    string temp = "";
                    for (int x = 0; x < TPLClass.InterlaceDict.Values.ToArray()[0].Length; x++)
                    {
                        if (TPL.interlace == TPLClass.InterlaceDict.Values.ToArray()[0][x])
                        {
                            temp = TPLClass.InterlaceDict.Keys.ToArray()[0][x];
                        }
                    }
                    return temp;
                }

                // Adding rows
                var index = table.Rows.Add();
                table.Rows[index].Cells[0].Value = new Bitmap(thumbnail);
                table.Rows[index].Cells[1].Value = $"{i}.tpl";
                table.Rows[index].Cells[2].Value = TPL.width;
                table.Rows[index].Cells[3].Value = TPL.height;
                table.Rows[index].Cells[4] = cbBitDepth;
                table.Rows[index].Cells[4].Value = GetBitDepth();
                table.Rows[index].Cells[5] = cbInterlace;
                table.Rows[index].Cells[5].Value = GetInterlace();
                table.Rows[index].Cells[6].Value = TPL.zPriority;
                table.Rows[index].Cells[7].Value = TPL.mipmapCount;
                table.Rows[index].Cells[8].Value = TPL.scale;
                table.Rows[index].Cells[9].Value = TPL.config1;
                table.Rows[index].Cells[10].Value = TPL.config2;
                table.Rows[index].Cells[11].Value = TPL.config3;
                // Disabling edit
                table.Rows[index].Cells[2].ReadOnly = true;
                table.Rows[index].Cells[3].ReadOnly = true;
                table.Rows[index].Cells[7].ReadOnly = true;
                table.Rows[index].Cells[9].ReadOnly = true;
                table.Rows[index].Cells[10].ReadOnly = true;
                table.Rows[index].Cells[11].ReadOnly = true;
                thumbnail.Dispose();
            }
            lblStatusText.Text = texturesTotal + " textures loaded successfully";
        }
        private void RefreshTable()
        {
            if (filepath != "")
            {
                try
                {
                    table.Rows.Clear();
                    table.Columns.Clear();
                    try
                    {
                        //Directory.Delete(".temp", true);
                        //Directory.CreateDirectory(".temp");
                    }
                    catch (Exception)
                    {
                    }
                    FillTable();
                }
                catch (Exception)
                {
                    // Optional error handling to be added
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
                    lblStatusText.Text = "All textures removed successfully";
                    progressBar.Value = progressBar.Maximum;
                }
                else
                {
                    return;
                }

            }
            catch (Exception)
            {
            }
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
        private void Replace(string tplFile, string replacerTpl = "", int dialogTextureIndex = 0, bool isTemp = false)
        {
            ReadTexture(tplFile, selectedRowIndexGlobal);

            BinaryReader br = new BinaryReader(File.Open(tplFile, FileMode.Open));

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

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "RE4 PS2 TPL Files (*.tpl)|*.tpl|Image Files (*.png;*.bmp;*.tga)|*.png;*.bmp;*.tga";

            if (!isTemp)
            {
                fileDialog.ShowDialog();
                replacerTpl = fileDialog.FileName;
            }

            // Verifies which format imported file is 
            if (Path.GetExtension(fileDialog.FileName).ToLower() == ".png" || Path.GetExtension(fileDialog.FileName).ToLower() == ".bmp")
            {
                var image = new Bitmap(fileDialog.FileName);
                string bitsPerPixel = image.PixelFormat.ToString();
                int colorCount;

                // If selected image has a bit depth greater than 8-bit, show this dialog
                if (bitsPerPixel != "Format4bppIndexed" && bitsPerPixel != "Format8bppIndexed")
                {
                    DialogGetColor dialogGetColor = new DialogGetColor();
                    dialogGetColor.ShowDialog();
                    colorCount = dialogGetColor.ColorCount;
                }
                else if (bitsPerPixel == "Format4bppIndexed")
                {
                    colorCount = 16;
                }
                else
                {
                    colorCount = 256;
                }

                // Create temporary .bmp for converting it directly to .tpl
                IColorQuantizer colorQuantizer = new OctreeQuantizer();
                Image target = ImageBuffer.QuantizeImage(new Bitmap(fileDialog.FileName), colorQuantizer, colorCount, 4);
                target.Save($".temp/temp_{colorCount}.bmp", ImageFormat.Bmp);
                target.Dispose();

                ConverterBMP converterBMP = new ConverterBMP();
                converterBMP.BMPtoTPL(new string[] { $".temp/temp_{colorCount}.bmp" }, true);
                replacerTpl = $".temp/0_{colorCount}.tpl";
            }

            if (replacerTpl != "")
            {
                // Check if tpl contains multiple textures, then opens a dialog for selecting index
                BinaryReader brTemp = new BinaryReader(File.Open(replacerTpl, FileMode.Open));
                brTemp.BaseStream.Position += 4;
                uint tplCount = brTemp.ReadUInt32();
                brTemp.Close();

                if (tplCount > 1)
                {
                    DialogGetIndex dialogGetIndex = new DialogGetIndex();
                    dialogGetIndex.lblTplCount.Text = "of " + tplCount.ToString();
                    dialogGetIndex.ShowDialog();
                    dialogTextureIndex = dialogGetIndex.GetIndex();
                }

                BinaryReader br2 = new BinaryReader(File.Open(replacerTpl, FileMode.Open));
                br2.BaseStream.Position = 0x10 + (0x30 * dialogTextureIndex);
                ushort width = br2.ReadUInt16();
                ushort height = br2.ReadUInt16();
                ushort bitDepth = br2.ReadUInt16();

                br2.BaseStream.Position = 0x30 + (0x30 * dialogTextureIndex);
                uint pixelsOffset = br2.ReadUInt32();
                uint paletteOffset = br2.ReadUInt32();

                // Get header
                br2.BaseStream.Position = 0x10 + (0x30 * dialogTextureIndex);
                byte[] header = br2.ReadBytes(0x30);
                byte[] pixels = null;
                byte[] palette = null;

                // Get pixels indices
                br2.BaseStream.Position = pixelsOffset;
                if (bitDepth == 8)
                {
                    pixels = br2.ReadBytes(width * height / 2);
                }
                else if (bitDepth == 9 || bitDepth == 6)
                {
                    pixels = br2.ReadBytes(width * height);
                }
                Console.WriteLine("Width: " + width.ToString());
                Console.WriteLine("Height: " + height.ToString());
                Console.WriteLine("Pixels: " + pixels.ToString());
                // Get palette
                br2.BaseStream.Position = paletteOffset;
                if (bitDepth == 8)
                {
                    palette = br2.ReadBytes(width * height / 2);
                }
                else if (bitDepth == 9 || bitDepth == 6)
                {
                    palette = br2.ReadBytes(width * height);
                }
                br2.Close();

                // Overwrite .tpl and replace texture
                BinaryWriter bw = new BinaryWriter(File.Open(tplFile, FileMode.Create));
                bw.Write(part1);
                bw.Write(header);
                bw.Write(part2);
                bw.Write(pixels);
                bw.Write(part3);
                bw.Write(palette);
                bw.Write(part4);
                bw.Close();

                // Ask the user to also replace mipmaps
                if (table.Rows[selectedRowIndexGlobal].Cells[7].Value.ToString() == "2")
                {
                    DialogResult result = MessageBox.Show("Do you want to update mipmaps with this texture as well?\n" +
                        "(it will be resized automatically).", "Mipmaps detected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ReplaceMipmaps();
                    }
                    else return;
                }
                UpdateStatusText("Texture replaced successfully");
                UpdateAllOffsets(tplFile);
                RefreshTable();
            }
        }
        private void ReplaceMipmaps()
        {
            // ============
            // This method needs optimization/refactoring
            // ============

            ReadTexture(filepath, selectedRowIndexGlobal);
            uint MipMapHeaderOffset = 0;

            for (int mip = 0; mip < 2; mip++)
            {
                // Read mipmap 1 header
                BinaryReader br = new BinaryReader(File.OpenRead(filepath));
                switch (mip)
                {
                    case 0:
                        MipMapHeaderOffset = TPL.mipmapOffset1;
                        break;
                    case 1:
                        MipMapHeaderOffset = TPL.mipmapOffset2;
                        break;
                    default:
                        break;
                }
                br.BaseStream.Position = MipMapHeaderOffset;
                MipMap.width = br.ReadUInt16();
                MipMap.height = br.ReadUInt16();
                MipMap.bitDepth = br.ReadUInt16();
                MipMap.interlace = br.ReadUInt16();
                br.BaseStream.Position = MipMapHeaderOffset + 0x20;
                MipMap.pixelsOffset = br.ReadUInt32();
                br.Close();
                Console.WriteLine(MipMap.width);
                Console.WriteLine(MipMap.height);
                // Generates smaller version of the image in the preview window
                IColorQuantizer colorQuantizer = new OctreeQuantizer();
                Image a = new Bitmap(texturePreview.Image, MipMap.width, MipMap.height);
                Image target = ImageBuffer.QuantizeImage(a, colorQuantizer, 16, 4);
                target.Save($".temp/temp_{16}.bmp", ImageFormat.Bmp);
                target.Dispose();

                // Get pixels from new generated image
                BinaryReader brBmp = new BinaryReader(File.OpenRead($".temp/temp_{16}.bmp"));
                brBmp.BaseStream.Position = 0x0A;
                brBmp.BaseStream.Position = brBmp.ReadUInt32();
                byte[] bmpPixels = brBmp.ReadBytes(MipMap.width * MipMap.height / 2);
                brBmp.Close();

                // Overwrite mipmap pixels
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(filepath));
                bw.BaseStream.Position = MipMap.pixelsOffset;
                bw.Write(bmpPixels);
                bw.Close();
            }
        }
        private void Remove()
        {
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
        private void Rebuild()
        {
            /* This method rebuilds the file, getting all mipmaps pixels and moving them to the end of the file
             * the order remains the same. Can take some time on big .tpl files.
             */

            UpdateStatusText("Rebuilding file, please wait...");

            int mipmapHeadersCount = GetTotalMipmapCount();
            byte[] topPart; // Top part of the file
            byte[] bottomPart; // Bottom part of the file
            byte[] pixels; // Mipmap pixels
            int pixelsLengthAcumulator = 0; // Length of every pixels

            Console.WriteLine("Total Mipmaps = " + mipmapHeadersCount);
            Console.WriteLine("");

            for (int i = 0; i < mipmapHeadersCount; i++)
            {
                Console.WriteLine("Rebuilding mipmap " + i);
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

                Console.WriteLine("Width: " + MipMap.width + " | Height: " + MipMap.height + " | Bit: " + MipMap.bitDepth);

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

                // Overwrites the file to rebuild
                BinaryWriter bw = new BinaryWriter(File.Create(filepath));
                bw.Write(topPart);
                bw.Write(bottomPart);
                bw.Write(pixels);
                bw.Close();
                Console.WriteLine("");
            }
            UpdateAllOffsets(filepath);


            UpdateStatusText("Rebuild done, loading textures...");
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
            dialog.ShowDialog();
            filepath = dialog.FileName;
            btnOpenFile.Dispose();
            btnCreateNewFile.Dispose();
            if (filepath != "")
            {
                this.Text = "RE4 PS2 TPL Manager - " + Path.GetFileName(filepath);
                FillTable();
            }
        }
        private void openTPLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialog.Filter = "RE4 PS2 TPL Files (*.tpl)|*.tpl";
            dialog.ShowDialog();
            filepath = dialog.FileName;
            btnCreateNewFile.Dispose();
            btnOpenFile.Dispose();
            if (filepath != "")
            {
                this.Text = "RE4 PS2 TPL Manager - " + Path.GetFileName(filepath);
                FillTable();
            }
        }
        private void refreshTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
        // Menu: Texture
        private void btnExtract_Click(object sender, EventArgs e)
        {
            ExtractTPL();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
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
        private void convertAllToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ConverterBMP converterBMP = new ConverterBMP();
                converterBMP.TPLtoBMP(filepath, "PNG");
            }
            catch (Exception)
            {
            }
        }
        private void increaseAllTo256ColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    selectedRowIndexGlobal = i;
                    if (table.Rows[i].Cells[4].Value.ToString() != "8-bit")
                    {
                        string folderName = Path.GetFileNameWithoutExtension(filepath);

                        ConverterBMP converterBMP = new ConverterBMP();
                        converterBMP.BMPtoTPL(new string[] { $".temp/{folderName}/{i}_256.bmp" }, true);
                        Replace(filepath, $".temp/0_256.tpl", 0, true);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        private void decreaseAllTo16ColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    selectedRowIndexGlobal = i;
                    if (table.Rows[i].Cells[4].Value.ToString() != "4-bit")
                    {
                        string folderName = Path.GetFileNameWithoutExtension(filepath);

                        ConverterBMP converterBMP = new ConverterBMP();
                        converterBMP.BMPtoTPL(new string[] { $".temp/{folderName}/{i}_16.bmp" }, true);
                        Replace(filepath, $".temp/0_16.tpl", 0, true);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        // Menu: Tools
        private void btnCompileFromFolder_Click(object sender, EventArgs e)
        {
            CompileFromFolder();
        }
        private void convertAndImportBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BMP Files (*.bmp)|*.bmp";
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();

            ConverterBMP converterBMP = new ConverterBMP();
            converterBMP.BMPtoTPL(openFileDialog.FileNames);
        }
        private void fixBrokenTPLexperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TPL Files (*.tpl)|*.tpl";
            openFileDialog.ShowDialog();
            UpdateAllOffsets(openFileDialog.FileName);
            openFileDialog.Dispose();
        }
        // Table events
        private void table_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                texturePreview.Image = (Bitmap)(table.Rows[e.RowIndex].Cells[0].Value);
            }
        }
        private void table_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ctxMenuTable.Show(Cursor.Position);
                texturePreview.Image = (Bitmap)(table.Rows[e.RowIndex].Cells[0].Value);
            }
            if (e.RowIndex >= 0)
            {
                selectedRowIndexGlobal = e.RowIndex;
            }
        }
        private void table_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedRowIndexGlobal = e.RowIndex;
                texturePreview.Image = (Bitmap)(table.Rows[e.RowIndex].Cells[0].Value);
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
        private void increaseColorDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() != "8-bit")
            {
                string folderName = Path.GetFileNameWithoutExtension(filepath);

                ConverterBMP converterBMP = new ConverterBMP();
                converterBMP.BMPtoTPL(new string[] { $".temp/{folderName}/{selectedRowIndexGlobal}_256.bmp" }, true);
                Replace(filepath, $".temp/0_256.tpl", 0, true);
            }
            else
            {
                MessageBox.Show("Texture is already at maximum colors allowed (256).", "Cannot increase bit depth",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void decreaseColorDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() != "4-bit")
            {
                string folderName = Path.GetFileNameWithoutExtension(filepath);

                ConverterBMP converterBMP = new ConverterBMP();
                converterBMP.BMPtoTPL(new string[] { $".temp/{folderName}/{selectedRowIndexGlobal}_16.bmp" }, true);
                Replace(filepath, $".temp/0_16.tpl", 0, true);
            }
            else
            {
                MessageBox.Show("Texture is already at minimum colors allowed (16).", "Cannot decrease bit depth",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
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
            Directory.Delete(".temp", true);
            Console.WriteLine();
        }
        // Editor 
        private void spinBrightness_ValueChanged(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Brightness((int)spinBrightness.Value);
                texturePreview.Image = imageFactory.Image;
            }
        }
        private void spinContrast_ValueChanged(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Contrast((int)spinContrast.Value);
                texturePreview.Image = imageFactory.Image;
            }
        }
        private void spinSaturation_ValueChanged(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Saturation((int)spinSaturation.Value);
                texturePreview.Image = imageFactory.Image;
            }
        }
        private void spinSharpen_ValueChanged(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.GaussianSharpen((int)spinSharpen.Value);
                texturePreview.Image = imageFactory.Image;
            }
        }
        private void spinPixelate_ValueChanged(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Pixelate((int)spinPixelate.Value);
                texturePreview.Image = imageFactory.Image;
            }
        }

        private void spinHue_ValueChanged(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Hue((int)spinHue.Value);
                texturePreview.Image = imageFactory.Image;
            }
        }

        private void btnRotate_Click_1(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                Image image = new Bitmap(texturePreview.Image);
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                texturePreview.Image = image;
            }
        }
        private void btnFlipX_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                Image image = new Bitmap(texturePreview.Image);
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                texturePreview.Image = image;
            }
        }
        private void btnFlipY_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                Image image = new Bitmap(texturePreview.Image);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                texturePreview.Image = image;
            }
        }
        private void btnUpscale_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                texturePreview.Image = GetResizeImage((Bitmap)texturePreview.Image, texturePreview.Image.Width * 2, texturePreview.Image.Height * 2);
            }
        }
        private void btnDownscale_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                texturePreview.Image = GetResizeImage((Bitmap)texturePreview.Image, texturePreview.Image.Width / 2, texturePreview.Image.Height / 2);
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null && filepath != "")
            {
                texturePreview.Image = (Bitmap)table.Rows[selectedRowIndexGlobal].Cells[0].Value;
            }
        }

        private void btnApplyChanges_Click_1(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                // Verifies if imported texture is wider/smaller than original
                if (texturePreview.Image.Width.ToString() != table.Rows[selectedRowIndexGlobal].Cells[2].Value.ToString() ||
                    texturePreview.Image.Height.ToString() != table.Rows[selectedRowIndexGlobal].Cells[3].Value.ToString())
                {
                    DialogResult result = MessageBox.Show("Image dimensions are different from original image, do you want to insert it anyway?",
                          "Different dimension detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        table.Rows[selectedRowIndexGlobal].Cells[0].Value = texturePreview.Image;
                    }
                }
                else
                {
                    table.Rows[selectedRowIndexGlobal].Cells[0].Value = texturePreview.Image;
                }

                // Creating image then converting it to .tpl and adding to the main file
                string folderName = Path.GetFileNameWithoutExtension(filepath);
                IColorQuantizer colorQuantizer = new OctreeQuantizer();
                Image image = new Bitmap(texturePreview.Image);

                if (table.Rows[selectedRowIndexGlobal].Cells[4].Value.ToString() == "8-bit")
                {
                    Image target256 = ImageBuffer.QuantizeImage(image, colorQuantizer, 256, 4);
                    image.Dispose();
                    target256.Save(".temp/" + folderName + "/" + selectedRowIndexGlobal + "_256.bmp", ImageFormat.Bmp);
                    target256.Dispose();

                    ConverterBMP converterBMP = new ConverterBMP();
                    converterBMP.BMPtoTPL(new string[] { ".temp/" + folderName + "/" + selectedRowIndexGlobal + "_256.bmp" }, true);

                    Replace(filepath, ".temp/0_256.tpl", 0, true);
                }
                else
                {
                    Image target16 = ImageBuffer.QuantizeImage(image, colorQuantizer, 16, 4);
                    image.Dispose();
                    target16.Save(".temp/" + folderName + "/" + selectedRowIndexGlobal + "_16.bmp", ImageFormat.Bmp);
                    target16.Dispose();

                    ConverterBMP converterBMP = new ConverterBMP();
                    converterBMP.BMPtoTPL(new string[] { ".temp/" + folderName + "/" + selectedRowIndexGlobal + "_16.bmp" }, true);

                    Replace(filepath, ".temp/0_16.tpl", 0, true);
                }
            }
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
            openFileDialog.ShowDialog();

            // If it's TGA, then convert to bitmap
            if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".tga")
            {
                TGASharpLib.TGA tga = new TGASharpLib.TGA(new Bitmap(openFileDialog.FileName));
                texturePreview.Image = new Bitmap(tga.ToBitmap());
                texturePreview.Dispose();
                return;
            }

            texturePreview.Image = new Bitmap(openFileDialog.FileName);
            openFileDialog.Dispose();
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

        private void convertAllToBMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                try
                {
                    string sourcePath = ".temp/" + Path.GetFileNameWithoutExtension(filepath);
                    string targetPath = "Converted/" + Path.GetFileNameWithoutExtension(filepath);
                    string fileName = string.Empty;
                    string destFile = string.Empty;

                    if (Directory.Exists(sourcePath))
                    {
                        string[] files = Directory.GetFiles(sourcePath);

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            fileName = Path.GetFileName(s);
                            destFile = Path.Combine(targetPath, fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Source path does not exist.");
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void convertAllToTGAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filepath != "")
            {
                string folderName = Path.GetFileNameWithoutExtension(filepath);
                foreach (DataGridViewRow row in table.Rows)
                {
                    var tga = TGASharpLib.TGA.FromBitmap((Bitmap)table.Rows[row.Index].Cells[0].Value);
                    tga.Save($"Converted/{folderName}/{table.Rows[row.Index].Cells[1].Value}.tga");
                    UpdateStatusText($"Texture converted at folder 'Converted/{folderName}'");
                }
            }
        }

        private void btnFilterComic_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Filter(MatrixFilters.Comic);
                texturePreview.Image = imageFactory.Image;
            }
        }

        private void btnFilterGray_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Filter(MatrixFilters.GreyScale);
                texturePreview.Image = imageFactory.Image;
            }
        }

        private void btnFilterColorful_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Filter(MatrixFilters.HiSatch);
                texturePreview.Image = imageFactory.Image;
            }
        }

        private void btnFilterInvert_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(texturePreview.Image);
                imageFactory.Filter(MatrixFilters.Invert);
                texturePreview.Image = imageFactory.Image;
            }
        }

        private void btnLayerAddMask_Click(object sender, EventArgs e)
        {
            if (texturePreview.Image != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.png;*.bmp;*.tga)|*.png;*.bmp;*.tga";
                dialog.ShowDialog();

                if (dialog.FileName != "")
                {
                    DialogResult result = DialogResult.OK;
                    Image image = Image.FromFile(dialog.FileName);
                    // Verifies if selected image has a different aspect ration from the one in the preview
                    if (image.Width != texturePreview.Image.Width || image.Height != texturePreview.Image.Height)
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

                        ImageFactory imageFactory = new ImageFactory();
                        imageFactory.Load(texturePreview.Image);
                        imageFactory.Mask(imageLayer);
                        texturePreview.Image = imageFactory.Image;
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
                dialog.ShowDialog();

                if (dialog.FileName != "")
                {
                    DialogResult result = DialogResult.OK;
                    Image image = Image.FromFile(dialog.FileName);
                    // Verifies if selected image has a different aspect ration from the one in the preview
                    if (image.Width != texturePreview.Image.Width || image.Height != texturePreview.Image.Height)
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

                        ImageFactory imageFactory = new ImageFactory();
                        imageFactory.Load(texturePreview.Image);
                        imageFactory.Overlay(imageLayer);
                        texturePreview.Image = imageFactory.Image;
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
                RefreshTable();
            }
            else
            {
                MessageBox.Show($"Invalid {Path.GetExtension(fileList[0])} file format, only .tpl texture files are supported.",
                    "Invalid format", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            // ---->>> Criar forma de fechar o loop depois de fechar o form
            System.Timers.Timer timer = new System.Timers.Timer(100);
            timer.AutoReset = true;
            int count = 0;
            timer.Elapsed += async (source, eventArgs) =>
            {
                Console.WriteLine("Executando timer");
                if (count == table.Rows.Count)
                {
                    count = 0;
                }
                pictureBox.Image = (Bitmap)table.Rows[count].Cells[0].Value;
                count++;
                await Task.Delay(50);
            };
            timer.Start();
        }
    }
}
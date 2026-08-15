using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RE4_PS2_TPL_Manager
{
    public struct BMP
    {
        public ushort magic;
        public uint filesize;
        public uint unused;
        public uint pixelsOffset;

        public uint infoHeader;
        public uint width;
        public uint height;
        public ushort planes; // always 01
        public ushort bitDepth; // 1-2-4-8-16-24 bits per pixel
        public uint compression; // 0 = uncompressed | 1 = 8bit RLE | 2 = 4bit RLE
        public uint imageSize; // after compressing, if uncompressed use 0
        public uint horResolution; // horizontal resolution: Pixels/meter
        public uint verResolution; // vertical resolution: Pixels/meter
        public uint colors; // colors quantity
        public uint colorsImportant;

    }

    public class LegacyTplHeader
    {
        public uint magic;
        public uint tplCount;
        public uint startOffset;
        public uint unused1;

        public ushort width;
        public ushort height;
        public ushort bitDepth;
        public ushort interlace;
        public ushort zPriority;
        public ushort mipmapCount;
        public ushort scale;
        public ushort unused2;

        public uint mipmapOffset1;
        public uint mipmapOffset2;
        public uint unk1;
        public uint unk2;

        public uint pixelsOffset;
        public uint paletteOffset;
        public byte unused3;
        public byte config1;
        public byte config2;
        public ushort config3;
        public byte unused4;
        public byte unused5;
        public byte endTag;
    }

    public class ConverterBMP
    {
        public void TPLtoBMP(string tplFile, string fileType, int bitDepth = 256)
        {
            if (String.IsNullOrWhiteSpace(tplFile) || !File.Exists(tplFile))
                throw new FileNotFoundException("TPL file was not found.", tplFile);

            string normalizedType = (fileType ?? String.Empty).Trim().ToUpperInvariant();
            if (normalizedType != "BMP" && normalizedType != "PNG")
                throw new ArgumentException("Supported export formats are BMP and PNG.", nameof(fileType));

            string folderName = Path.GetFileNameWithoutExtension(tplFile);
            string outputFolder = Path.Combine("Converted", folderName);
            Directory.CreateDirectory(outputFolder);

            var reader = new Core.Services.TplReader();
            var decoder = new Core.Services.TextureDecoder();
            uint textureCount = reader.ReadTextureCount(tplFile);

            var failedTextures = new System.Collections.Generic.List<string>();

            using (FileStream stream = File.Open(tplFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(stream))
            {
                for (int textureIndex = 0; textureIndex < textureCount; textureIndex++)
                {
                    try
                    {
                        TPLDefinition.TPL texture = reader.ReadTexture(br, textureIndex);
                        using (Bitmap bitmap = decoder.Decode(texture, br))
                        {
                            string extension = normalizedType == "BMP" ? ".bmp" : ".png";
                            string output = Path.Combine(outputFolder, textureIndex + extension);

                            if (normalizedType == "BMP")
                            {
                                bitmap.Save(output, ImageFormat.Bmp);
                            }
                            else
                            {
                                // PNG natively supports full-color pixels and alpha. Re-quantizing the
                                // decoded texture here was unnecessary, could alter colors/alpha and,
                                // more importantly, allowed one quantization failure to abort Export All.
                                bitmap.Save(output, ImageFormat.Png);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Keep exporting the remaining entries. A single unusual/corrupt texture must
                        // never make the last textures silently disappear from an Export All operation.
                        failedTextures.Add(textureIndex + ": " + ex.Message);
                    }
                }
            }

            if (failedTextures.Count > 0)
            {
                throw new InvalidOperationException(
                    "Export finished, but " + failedTextures.Count + " texture(s) could not be exported.\n\n" +
                    String.Join("\n", failedTextures.Take(12)) +
                    (failedTextures.Count > 12 ? "\n..." : String.Empty));
            }
        }

        public void BMPtoTPL(string[] tplFile, bool isTemp = false)
        {
            if (tplFile == null || tplFile.Length == 0) return;

            var reader = new Core.Services.TplReader();
            var writer = new Core.Services.TplWriter(reader);
            var encoder = new Core.Services.TextureEncoder();
            Directory.CreateDirectory("Converted");

            for (int i = 0; i < tplFile.Length; i++)
            {
                if (String.IsNullOrWhiteSpace(tplFile[i]) || !File.Exists(tplFile[i])) continue;
                try
                {
                    TPLDefinition.TPL texture = encoder.EncodeIndexedBmp(tplFile[i]);
                    string output;
                    if (isTemp)
                    {
                        // Compatibility path for old callers: use the operating system temp directory,
                        // never a fixed temporary folder beside the application.
                        string tempFolder = Path.Combine(Path.GetTempPath(), "RE4_PS2_TPL_Manager");
                        Directory.CreateDirectory(tempFolder);
                        output = Path.Combine(tempFolder, texture.bitDepth == 0x08 ? "0_16.tpl" : "0_256.tpl");
                    }
                    else
                    {
                        output = Path.Combine("Converted", Path.GetFileNameWithoutExtension(tplFile[i]) + ".tpl");
                    }
                    writer.WriteSingleTexture(output, texture);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "BMP to TPL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public byte SwapNibbles(byte x)
        {
            return (byte)((x & 0x0F) << 4 | (x & 0xF0) >> 4);
        }
    }
}

using System.Drawing;
using System.IO;

namespace RE4_PS2_TPL_Util
{
    public class ConverterTGA
    {
        public void TPLtoTGA(string tplFile)
        {
            // Load tpl file
            BinaryReader br = new BinaryReader(File.Open(tplFile, FileMode.Open));

            // Gets tpl count
            br.BaseStream.Position = 0x04;
            byte tplCount = br.ReadByte();
            System.Console.WriteLine("Found " + tplCount + " textures");

            // Iterates through each texture inside .tpl file
            for (int tplNumber = 0; tplNumber < tplCount; tplNumber++)
            {
                System.Console.WriteLine("Converting texture " + tplNumber);

                // Adds 0x30 on each iteration to get next texture values, as each header has 0x30 length
                int chunkSize = 0x30 * tplNumber;

                // Gets basic tpl parameters
                br.BaseStream.Position = 0x10 + chunkSize;
                ushort width = br.ReadUInt16();
                ushort height = br.ReadUInt16();
                byte bitDepth = br.ReadByte();

                // Gets offsets where data starts
                br.BaseStream.Position = 0x30 + chunkSize;
                uint pixelsOffset = br.ReadUInt32();
                uint palleteOffset = br.ReadUInt32();
                uint totalPixels = palleteOffset - pixelsOffset;

                // Creates a .tga header with an array of 0x12 length
                ushort[] tgaHeader = new ushort[] { 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, width, height, 0x20 };

                // Creates .tga file based on tpl count
                BinaryWriter bw;
                if (tplCount > 1)
                {
                    // Remove .tpl extension and creates .tga
                    string tplFileName = tplFile.Substring(0, tplFile.Length - 4);
                    tplFileName = Path.GetFileName(tplFileName);

                    Directory.CreateDirectory($"Converted/{tplFileName}");
                    bw = new BinaryWriter(File.Open($"Converted/{tplFileName}" + "/" + tplFileName + "_" + tplNumber + ".tga", FileMode.Create));
                }
                else
                {
                    // Remove .tpl extension and creates .tga
                    string tplFileName = tplFile.Substring(0, tplFile.Length - 4);
                    tplFileName = Path.GetFileName(tplFileName);

                    Directory.CreateDirectory($"Converted/{tplFileName}");
                    bw = new BinaryWriter(File.Open($"Converted/{tplFileName}" + "/" + tplFileName + ".tga", FileMode.Create));
                }

                // Writes every byte of tgaHeader array in the .tga file
                for (int value = 0; value < tgaHeader.Length; value++)
                {
                    bw.Write((ushort)tgaHeader[value]);
                }

                // Verifies bit depth
                if (bitDepth == 8)
                {
                    // Loop through each pixel
                    for (int x = 0; x < totalPixels; x++)
                    {
                        // LOWER 4 BITS
                        br.BaseStream.Position = pixelsOffset + x;
                        int nibbleLow = br.ReadByte() >> 4;
                        byte padding = 0x20;

                        // Check if it will need padding (used in 4-bit .tpl files)
                        if (nibbleLow < 8)
                        { br.BaseStream.Position = palleteOffset + (nibbleLow * 4); }
                        else
                        { br.BaseStream.Position = palleteOffset + padding + (nibbleLow * 4); }

                        byte R1 = br.ReadByte(); // R
                        byte G1 = br.ReadByte(); // G
                        byte B1 = br.ReadByte(); // B
                        byte A1 = br.ReadByte(); // A

                        bw.Write(B1); // B
                        bw.Write(G1); // G
                        bw.Write(R1); // R
                        bw.Write((byte)((A1 * 0xFF) / 0x80)); // A

                        // HIGHER 4 BITS
                        br.BaseStream.Position = pixelsOffset + x;
                        int nibbleHigh = br.ReadByte() & 0x0F;

                        // Check if it will need padding (used in 4-bit .tpl files)
                        if (nibbleHigh < 8)
                        { br.BaseStream.Position = palleteOffset + (nibbleHigh * 4); }
                        else
                        { br.BaseStream.Position = palleteOffset + padding + (nibbleHigh * 4); }

                        // Creates temporary bytes for channel swap (RGBA to BGRA)
                        byte R2 = br.ReadByte(); // R
                        byte G2 = br.ReadByte(); // G
                        byte B2 = br.ReadByte(); // B
                        byte A2 = br.ReadByte();  // A

                        bw.Write(B2); // B
                        bw.Write(G2); // G
                        bw.Write(R2); // R
                        bw.Write((byte)((A2 * 0xFF) / 0x80)); // A
                    }
                }
                else if (bitDepth == 9)
                {
                    // Loop through each pixel
                    for (int x = 0; x < totalPixels; x++)
                    {
                        br.BaseStream.Position = pixelsOffset + x;
                        int colorIndex = br.ReadByte();
                        br.BaseStream.Position = palleteOffset + (colorIndex * 4);

                        // Creates temporary bytes for channel swap (RGBA to BGRA)
                        byte R = br.ReadByte(); // R
                        byte G = br.ReadByte(); // G
                        byte B = br.ReadByte(); // B
                        byte A = br.ReadByte(); // A

                        bw.Write(B); // B
                        bw.Write(G); // G
                        bw.Write(R); // R
                        bw.Write((byte)((A * 0xFF) / 0x80));    // A
                    }
                }
                bw.Close();
                br.Close();
            }
        }
    }

    public class GenerateView
    {
        public Bitmap Generate(string tplFile)
        {
            // Load tpl file
            BinaryReader br = new BinaryReader(File.Open(tplFile, FileMode.Open));

            // Gets tpl count
            br.BaseStream.Position = 0x04;
            byte tplCount = br.ReadByte();
            System.Console.WriteLine("Found " + tplCount + " textures");

            // Adds 0x30 on each iteration to get next texture values, as each header has 0x30 length
            int chunkSize = 0x30;

            // Gets basic tpl parameters
            br.BaseStream.Position = 0x10 + chunkSize;
            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            ushort bitDepth = br.ReadUInt16();
            ushort interlace = br.ReadUInt16();

            // Gets offsets where data starts
            br.BaseStream.Position = 0x30 + chunkSize;
            uint pixelsOffset = br.ReadUInt32();
            uint paletteOffset = br.ReadUInt32();
            uint totalPixels = paletteOffset - pixelsOffset;

            Bitmap bitmap = new Bitmap(width, height);
            if (bitDepth == 0x8)
            {
                int indicesbytesCount = (height * width) / 2;
                br.BaseStream.Position = pixelsOffset;

                byte[] indices = br.ReadBytes(indicesbytesCount);
                br.BaseStream.Position = paletteOffset;

                byte[] palette = br.ReadBytes(0x80);
                Color[] colors = new Color[16];

                int cont = 0;
                for (int ic = 0; ic < 16; ic++)
                {
                    Color c = Color.FromArgb(
                        (byte)(palette[cont + 3] * 0xFF / 0x80),
                        palette[cont + 0],
                        palette[cont + 1],
                        palette[cont + 2]
                        );
                    colors[ic] = c;
                    cont += 4;
                    if (ic == 7)
                    {
                        cont = 0x40;
                    }


                    int Xcont = 0;
                    int Ycont = 0;
                    for (int IN = 0; IN < indices.Length; IN++)
                    {
                        int nibbleLow = indices[IN] >> 4;
                        int nibbleHigh = indices[IN] & 0x0F;

                        bitmap.SetPixel(Xcont + 1, Ycont, colors[nibbleLow]);
                        bitmap.SetPixel(Xcont, Ycont, colors[nibbleHigh]);

                        Xcont += 2;
                        if (Xcont >= width)
                        {
                            Xcont = 0;
                            Ycont += 1;
                        }
                    }

                    if (interlace == 0x1)
                    {
                        bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }

                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    //bitmap.Save(info.FullName + "_" + i + "_" + bitDepth.ToString("X4") + "_"+ interlacing.ToString("X4") + ".png", System.Drawing.Imaging.ImageFormat.Png);

                    //bitmap.Save(tplNumber + ".png", System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            br.Close();
            return bitmap;
        }
    }
}

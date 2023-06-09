﻿using SimplePaletteQuantizer.Helpers;
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.Octree;
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

    public struct TPL
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
            BinaryReader br = new BinaryReader(File.Open(tplFile, FileMode.Open));

            // Gets tpl count
            br.BaseStream.Position = 0x04;
            byte tplCount = br.ReadByte();
            string folderName = Path.GetFileNameWithoutExtension(tplFile);
            Directory.CreateDirectory(".temp/" + folderName);
            Directory.CreateDirectory("Converted/" + folderName);

            // Iterates through each texture inside .tpl file
            for (int tplNumber = 0; tplNumber < tplCount; tplNumber++)
            {
                // Adds 0x30 on each iteration to get next texture values, as each header has 0x30 length
                int chunkSize = 0x30 * tplNumber;
                TPL tpl;
                br.BaseStream.Position = 0x10 + chunkSize;
                tpl.width = br.ReadUInt16();
                tpl.height = br.ReadUInt16();
                tpl.bitDepth = br.ReadUInt16();
                tpl.interlace = br.ReadUInt16();
                tpl.zPriority = br.ReadUInt16();
                tpl.mipmapCount = br.ReadUInt16();
                tpl.scale = br.ReadUInt16();
                tpl.unused2 = br.ReadUInt16();
                tpl.mipmapOffset1 = br.ReadUInt32();
                tpl.mipmapOffset2 = br.ReadUInt32();
                tpl.unk1 = br.ReadUInt32();
                tpl.unk2 = br.ReadUInt32();
                tpl.pixelsOffset = br.ReadUInt32();
                tpl.paletteOffset = br.ReadUInt32();
                tpl.unused3 = br.ReadByte();
                tpl.config1 = br.ReadByte();
                tpl.config2 = br.ReadByte();
                tpl.config3 = br.ReadUInt16();

                // Checking interlace
                if (tpl.bitDepth == 0x8 && (tpl.interlace == 0x0 || tpl.interlace == 0x1))
                {
                    br.BaseStream.Position = tpl.pixelsOffset;
                    byte[] indices = br.ReadBytes((tpl.height * tpl.width) / 2);

                    br.BaseStream.Position = tpl.paletteOffset;
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
                    }

                    Bitmap bitmap = new Bitmap(tpl.width, tpl.height);

                    int Xcont = 0;
                    int Ycont = 0;
                    for (int IN = 0; IN < indices.Length; IN++)
                    {
                        int nibbleLow = indices[IN] >> 4;
                        int nibbleHigh = indices[IN] & 0x0F;

                        bitmap.SetPixel(Xcont + 1, Ycont, colors[nibbleLow]);
                        bitmap.SetPixel(Xcont, Ycont, colors[nibbleHigh]);

                        Xcont += 2;
                        if (Xcont >= tpl.width)
                        {
                            Xcont = 0;
                            Ycont += 1;
                        }
                    }

                    if (tpl.interlace == 0x1)
                    {
                        //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    if (fileType == "BMP")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target256 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target256.Save(".temp/" + folderName + "/" + tplNumber + "_256.bmp", ImageFormat.Bmp);

                            Image target16 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 16, 4);
                            target16.Save(".temp/" + folderName + "/" + tplNumber + "_16.bmp", ImageFormat.Bmp);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (fileType == "PNG")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, bitDepth, 4);
                            target.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (tpl.bitDepth == 0x8 && (tpl.interlace == 0x2 || tpl.interlace == 0x3))
                {
                    br.BaseStream.Position = tpl.pixelsOffset;
                    byte[] indices = br.ReadBytes((int)((tpl.height * tpl.width) / 2));

                    br.BaseStream.Position = tpl.paletteOffset;
                    byte[] paletteA = br.ReadBytes(0x80);

                    Color[] colors = new Color[16];

                    int cont = 0;
                    for (int ic = 0; ic < 16; ic++)
                    {
                        Color c = Color.FromArgb(
                            (byte)(paletteA[cont + 3] * 0xFF / 0x80),
                            paletteA[cont + 0],
                            paletteA[cont + 1],
                            paletteA[cont + 2]
                            );
                        colors[ic] = c;
                        cont += 4;
                        if (ic == 7)
                        {
                            cont = 0x40;
                        }
                    }

                    Bitmap bitmap = new Bitmap(tpl.height, tpl.width);

                    // Notas
                    // a cada 32 bytes é uma nova sequencia
                    // parede de 16 bytes, sendo 32 pixel
                    // sendo 2 linhas

                    int Xcont = 0;
                    int Ycont = 0;

                    int altCont = 0;
                    int niveis = 0;

                    int nivelMax = (tpl.width / 16);

                    bool flipEmX = false;

                    for (int IN = 0; IN < indices.Length; IN += 32)
                    {

                        Deinterlace4bit(ref bitmap, Xcont, Ycont, ref colors, ref indices, IN, flipEmX);

                        Ycont += 16;
                        niveis += 1;

                        if (niveis == nivelMax)
                        {
                            niveis = 0;
                            altCont += 1;
                            if (altCont % 2 == 0)
                            {
                                altCont += 2;

                                if (flipEmX)
                                {
                                    flipEmX = false;
                                }
                                else
                                {
                                    flipEmX = true;
                                }
                            }
                            Ycont = altCont;

                            if (altCont == 16)
                            {
                                flipEmX = false;
                                Ycont = 0;
                                altCont = 0;
                                niveis = 0;
                                Xcont += 32;
                            }
                        }
                    }

                    // correção da imagem baseado em gambiarra
                    if (tpl.width > 128 || tpl.height > 128)
                    {
                        Bitmap bitmapFix = new Bitmap(tpl.width, tpl.height);

                        int blockAmounts = (tpl.width / 128) * (tpl.height / 128);
                        int copyX = 0;
                        int copyY = 0;
                        int setX = 0;
                        int setY = 0;

                        for (int iB = 0; iB < blockAmounts; iB++)
                        {
                            for (int y = 0; y < 128; y++)
                            {
                                for (int x = 0; x < 128; x++)
                                {
                                    Color colorGet = bitmap.GetPixel(x + copyX, y + copyY);
                                    bitmapFix.SetPixel(x + setX, y + setY, colorGet);
                                }
                            }

                            setX += 128;
                            if (setX >= tpl.width)
                            {
                                setX = 0;
                                setY += 128;
                            }

                            copyY += 128;
                            if (copyY >= tpl.width)
                            {
                                copyY = 0;
                                copyX += 128;
                            }
                        }
                        bitmap = bitmapFix;
                    }

                    if (tpl.interlace == 0x3)
                    {
                        //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }

                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    if (fileType == "BMP")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target256 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target256.Save(".temp/" + folderName + "/" + tplNumber + "_256.bmp", ImageFormat.Bmp);
                            Image target16 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 16, 4);
                            target16.Save(".temp/" + folderName + "/" + tplNumber + "_16.bmp", ImageFormat.Bmp);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (fileType == "PNG")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                            // bitmap.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (tpl.bitDepth == 0x9 && (tpl.interlace == 0x0 || tpl.interlace == 0x1))
                {
                    br.BaseStream.Position = tpl.pixelsOffset;
                    byte[] indices = br.ReadBytes(tpl.height * tpl.width);

                    br.BaseStream.Position = tpl.paletteOffset;
                    byte[] palette = br.ReadBytes(0x400);

                    Color[] colors = new Color[256];

                    int index = 0;
                    int swap = 1;
                    for (int ic = 0; ic < 256; ic++)
                    {

                        if (ic % 8 == 0 && ic != 0)
                        {
                            if (swap == 1)
                            {
                                index += 8;
                            }
                            else if (swap == 2)
                            {
                                index -= 16;
                            }
                            else if (swap == 3)
                            {
                                index += 8;
                            }
                            else
                            {
                                swap = 0;
                            }
                            swap++;

                        }

                        int cont = index * 4;

                        Color c = Color.FromArgb(
                          (byte)(palette[cont + 3] * 0xFF / 0x80),
                          palette[cont + 0],
                          palette[cont + 1],
                          palette[cont + 2]
                          );

                        colors[ic] = c;
                        index++;
                    }

                    Bitmap bitmap = new Bitmap(tpl.width, tpl.height);

                    int Xcont = 0;
                    int Ycont = 0;
                    for (int IN = 0; IN < indices.Length; IN++)
                    {
                        bitmap.SetPixel(Xcont, Ycont, colors[indices[IN]]);

                        Xcont += 1;
                        if (Xcont >= tpl.width)
                        {
                            Xcont = 0;
                            Ycont += 1;
                        }
                    }

                    if (tpl.interlace == 0x1)
                    {
                        //bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    if (fileType == "BMP")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target256 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target256.Save(".temp/" + folderName + "/" + tplNumber + "_256.bmp", ImageFormat.Bmp);
                            Image target16 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 16, 4);
                            target16.Save(".temp/" + folderName + "/" + tplNumber + "_16.bmp", ImageFormat.Bmp);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (fileType == "PNG")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                            // bitmap.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (tpl.bitDepth == 0x9 && (tpl.interlace == 0x2 || tpl.interlace == 0x3))
                {
                    int indicesbytesCount = (tpl.height * tpl.width);

                    br.BaseStream.Position = tpl.pixelsOffset;

                    byte[] indices = br.ReadBytes(indicesbytesCount);

                    br.BaseStream.Position = tpl.paletteOffset;

                    byte[] palette = br.ReadBytes(256 * 4);

                    Color[] colors = new Color[256];

                    int index = 0;
                    int swap = 1;
                    for (int ic = 0; ic < 256; ic++)
                    {

                        if (ic % 8 == 0 && ic != 0)
                        {
                            if (swap == 1)
                            {
                                index += 8;
                            }
                            else if (swap == 2)
                            {
                                index -= 16;
                            }
                            else if (swap == 3)
                            {
                                index += 8;
                            }
                            else
                            {
                                swap = 0;
                            }
                            swap++;

                        }

                        int cont = index * 4;

                        Color c = Color.FromArgb(
                          (byte)(palette[cont + 3] * 0xFF / 0x80),
                          palette[cont + 0],
                          palette[cont + 1],
                          palette[cont + 2]
                          );

                        colors[ic] = c;

                        index++;

                    }

                    Bitmap bitmap = new Bitmap(tpl.width, tpl.height);

                    int Xcont = 0;
                    int Ycont = 0;
                    bool flipEmX = false;
                    for (int IN = 0; IN < indices.Length; IN += 32)
                    {
                        Deinterlace8bit(ref bitmap, Xcont, Ycont, ref colors, ref indices, IN, flipEmX);

                        Xcont += 16;

                        if (Xcont >= tpl.width)
                        {
                            Xcont = 0;
                            Ycont += 1;
                            if (Ycont % 2 == 0)
                            {
                                Ycont += 2;

                                if (flipEmX)
                                {
                                    flipEmX = false;
                                }
                                else
                                {
                                    flipEmX = true;
                                }
                            }
                        }
                    }

                    if (tpl.interlace == 0x3)
                    {
                        // bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    if (fileType == "BMP")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target256 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target256.Save(".temp/" + folderName + "/" + tplNumber + "_256.bmp", ImageFormat.Bmp);
                            Image target16 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 16, 4);
                            target16.Save(".temp/" + folderName + "/" + tplNumber + "_16.bmp", ImageFormat.Bmp);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (fileType == "PNG")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                            // bitmap.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (tpl.bitDepth == 0x6 && (tpl.interlace == 0x0 || tpl.interlace == 0x1))
                {
                    int bytesCount = (tpl.height * tpl.width) * 4;

                    br.BaseStream.Position = tpl.pixelsOffset;

                    byte[] ColorBytes = br.ReadBytes(bytesCount);

                    Bitmap bitmap = new Bitmap(tpl.width, tpl.height);

                    int Xcont = 0;
                    int Ycont = 0;
                    int lenght = tpl.height * tpl.width;
                    int cont = 0;
                    for (int IN = 0; IN < lenght; IN++)
                    {
                        Color c = Color.FromArgb(
                           ColorBytes[cont + 3],
                           ColorBytes[cont + 0],
                           ColorBytes[cont + 1],
                           ColorBytes[cont + 2]
                           );
                        cont += 4;

                        bitmap.SetPixel(Xcont, Ycont, c);

                        Xcont++;
                        if (Xcont >= tpl.width)
                        {
                            Xcont = 0;
                            Ycont += 1;
                        }
                    }

                    if (tpl.interlace == 0x1)
                    {
                        // bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    if (fileType == "BMP")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target256 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target256.Save(".temp/" + folderName + "/" + tplNumber + "_256.bmp", ImageFormat.Bmp);
                            Image target16 = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 16, 4);
                            target16.Save(".temp/" + folderName + "/" + tplNumber + "_16.bmp", ImageFormat.Bmp);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else if (fileType == "PNG")
                    {
                        try
                        {
                            IColorQuantizer colorQuantizer = new OctreeQuantizer();
                            Image target = ImageBuffer.QuantizeImage(bitmap, colorQuantizer, 256, 4);
                            target.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                            // bitmap.Save("Converted/" + folderName + "/" + tplNumber + ".png", ImageFormat.Png);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Texture not supported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Huge thanks to JADERLINK for creating these functions
                void Deinterlace4bit(ref Bitmap bitmap, int Xcont, int Ycont, ref Color[] colors, ref byte[] indices, int IN, bool flipEmX)
                {
                    int[][] idxs = new int[0x20][]; // indice de cor;
                    for (int i = 0; i < 0x20; i++)
                    {
                        int nibbleLow = indices[IN + i] >> 4;
                        int nibbleHigh = indices[IN + i] & 0x0F;

                        int[] two = new int[2];

                        two[0] = nibbleLow;
                        two[1] = nibbleHigh;

                        idxs[i] = two;
                    }
                    if (flipEmX == false)
                    {
                        bitmap.SetPixel(Xcont + 0x4, Ycont + 2, colors[idxs[0x0][0]]);
                        bitmap.SetPixel(Xcont + 0x0, Ycont + 0, colors[idxs[0x0][1]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 2, colors[idxs[0x1][0]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 0, colors[idxs[0x1][1]]);
                        bitmap.SetPixel(Xcont + 0x14, Ycont + 2, colors[idxs[0x2][0]]);
                        bitmap.SetPixel(Xcont + 0x10, Ycont + 0, colors[idxs[0x2][1]]);
                        bitmap.SetPixel(Xcont + 0x1C, Ycont + 2, colors[idxs[0x3][0]]);
                        bitmap.SetPixel(Xcont + 0x18, Ycont + 0, colors[idxs[0x3][1]]);

                        bitmap.SetPixel(Xcont + 0x5, Ycont + 2, colors[idxs[0x4][0]]);
                        bitmap.SetPixel(Xcont + 0x1, Ycont + 0, colors[idxs[0x4][1]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 2, colors[idxs[0x5][0]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 0, colors[idxs[0x5][1]]);
                        bitmap.SetPixel(Xcont + 0x15, Ycont + 2, colors[idxs[0x6][0]]);
                        bitmap.SetPixel(Xcont + 0x11, Ycont + 0, colors[idxs[0x6][1]]);
                        bitmap.SetPixel(Xcont + 0x1D, Ycont + 2, colors[idxs[0x7][0]]);
                        bitmap.SetPixel(Xcont + 0x19, Ycont + 0, colors[idxs[0x7][1]]);

                        //acima feito // arquivo idm_012_mod3B_A2

                        bitmap.SetPixel(Xcont + 0x6, Ycont + 2, colors[idxs[0x8][0]]);
                        bitmap.SetPixel(Xcont + 0x2, Ycont + 0, colors[idxs[0x8][1]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 2, colors[idxs[0x9][0]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 0, colors[idxs[0x9][1]]);
                        bitmap.SetPixel(Xcont + 0x16, Ycont + 2, colors[idxs[0xA][0]]);
                        bitmap.SetPixel(Xcont + 0x12, Ycont + 0, colors[idxs[0xA][1]]);
                        bitmap.SetPixel(Xcont + 0x1E, Ycont + 2, colors[idxs[0xB][0]]);
                        bitmap.SetPixel(Xcont + 0x1A, Ycont + 0, colors[idxs[0xB][1]]);

                        bitmap.SetPixel(Xcont + 0x7, Ycont + 2, colors[idxs[0xC][0]]);
                        bitmap.SetPixel(Xcont + 0x3, Ycont + 0, colors[idxs[0xC][1]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 2, colors[idxs[0xD][0]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 0, colors[idxs[0xD][1]]);
                        bitmap.SetPixel(Xcont + 0x17, Ycont + 2, colors[idxs[0xE][0]]);
                        bitmap.SetPixel(Xcont + 0x13, Ycont + 0, colors[idxs[0xE][1]]);
                        bitmap.SetPixel(Xcont + 0x1F, Ycont + 2, colors[idxs[0xF][0]]);
                        bitmap.SetPixel(Xcont + 0x1B, Ycont + 0, colors[idxs[0xF][1]]);

                        //acima feito //arquivo idm_012_mod3B_A3

                        bitmap.SetPixel(Xcont + 0x0, Ycont + 2, colors[idxs[0x10][0]]);
                        bitmap.SetPixel(Xcont + 0x4, Ycont + 0, colors[idxs[0x10][1]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 2, colors[idxs[0x11][0]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 0, colors[idxs[0x11][1]]);
                        bitmap.SetPixel(Xcont + 0x10, Ycont + 2, colors[idxs[0x12][0]]);
                        bitmap.SetPixel(Xcont + 0x14, Ycont + 0, colors[idxs[0x12][1]]);
                        bitmap.SetPixel(Xcont + 0x18, Ycont + 2, colors[idxs[0x13][0]]);
                        bitmap.SetPixel(Xcont + 0x1C, Ycont + 0, colors[idxs[0x13][1]]);

                        bitmap.SetPixel(Xcont + 0x1, Ycont + 2, colors[idxs[0x14][0]]);
                        bitmap.SetPixel(Xcont + 0x5, Ycont + 0, colors[idxs[0x14][1]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 2, colors[idxs[0x15][0]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 0, colors[idxs[0x15][1]]);
                        bitmap.SetPixel(Xcont + 0x11, Ycont + 2, colors[idxs[0x16][0]]);
                        bitmap.SetPixel(Xcont + 0x15, Ycont + 0, colors[idxs[0x16][1]]);
                        bitmap.SetPixel(Xcont + 0x19, Ycont + 2, colors[idxs[0x17][0]]);
                        bitmap.SetPixel(Xcont + 0x1D, Ycont + 0, colors[idxs[0x17][1]]);

                        // acima feito  //arquivo idm_012_mod3B_A4

                        //----

                        bitmap.SetPixel(Xcont + 0x2, Ycont + 2, colors[idxs[0x18][0]]);
                        bitmap.SetPixel(Xcont + 0x6, Ycont + 0, colors[idxs[0x18][1]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 2, colors[idxs[0x19][0]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 0, colors[idxs[0x19][1]]);
                        bitmap.SetPixel(Xcont + 0x12, Ycont + 2, colors[idxs[0x1A][0]]);
                        bitmap.SetPixel(Xcont + 0x16, Ycont + 0, colors[idxs[0x1A][1]]);
                        bitmap.SetPixel(Xcont + 0x1A, Ycont + 2, colors[idxs[0x1B][0]]);
                        bitmap.SetPixel(Xcont + 0x1E, Ycont + 0, colors[idxs[0x1B][1]]);

                        bitmap.SetPixel(Xcont + 0x3, Ycont + 2, colors[idxs[0x1C][0]]);
                        bitmap.SetPixel(Xcont + 0x7, Ycont + 0, colors[idxs[0x1C][1]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 2, colors[idxs[0x1D][0]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 0, colors[idxs[0x1D][1]]);
                        bitmap.SetPixel(Xcont + 0x13, Ycont + 2, colors[idxs[0x1E][0]]);
                        bitmap.SetPixel(Xcont + 0x17, Ycont + 0, colors[idxs[0x1E][1]]);
                        bitmap.SetPixel(Xcont + 0x1B, Ycont + 2, colors[idxs[0x1F][0]]);
                        bitmap.SetPixel(Xcont + 0x1F, Ycont + 0, colors[idxs[0x1F][1]]);
                    }
                    else
                    {
                        // versão flip


                        bitmap.SetPixel(Xcont + 0x0, Ycont + 2, colors[idxs[0x0][0]]);
                        bitmap.SetPixel(Xcont + 0x4, Ycont + 0, colors[idxs[0x0][1]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 2, colors[idxs[0x1][0]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 0, colors[idxs[0x1][1]]);
                        bitmap.SetPixel(Xcont + 0x10, Ycont + 2, colors[idxs[0x2][0]]);
                        bitmap.SetPixel(Xcont + 0x14, Ycont + 0, colors[idxs[0x2][1]]);
                        bitmap.SetPixel(Xcont + 0x18, Ycont + 2, colors[idxs[0x3][0]]);
                        bitmap.SetPixel(Xcont + 0x1C, Ycont + 0, colors[idxs[0x3][1]]);

                        bitmap.SetPixel(Xcont + 0x1, Ycont + 2, colors[idxs[0x4][0]]);
                        bitmap.SetPixel(Xcont + 0x5, Ycont + 0, colors[idxs[0x4][1]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 2, colors[idxs[0x5][0]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 0, colors[idxs[0x5][1]]);
                        bitmap.SetPixel(Xcont + 0x11, Ycont + 2, colors[idxs[0x6][0]]);
                        bitmap.SetPixel(Xcont + 0x15, Ycont + 0, colors[idxs[0x6][1]]);
                        bitmap.SetPixel(Xcont + 0x19, Ycont + 2, colors[idxs[0x7][0]]);
                        bitmap.SetPixel(Xcont + 0x1D, Ycont + 0, colors[idxs[0x7][1]]);


                        bitmap.SetPixel(Xcont + 0x2, Ycont + 2, colors[idxs[0x8][0]]);
                        bitmap.SetPixel(Xcont + 0x6, Ycont + 0, colors[idxs[0x8][1]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 2, colors[idxs[0x9][0]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 0, colors[idxs[0x9][1]]);
                        bitmap.SetPixel(Xcont + 0x12, Ycont + 2, colors[idxs[0xA][0]]);
                        bitmap.SetPixel(Xcont + 0x16, Ycont + 0, colors[idxs[0xA][1]]);
                        bitmap.SetPixel(Xcont + 0x1A, Ycont + 2, colors[idxs[0xB][0]]);
                        bitmap.SetPixel(Xcont + 0x1E, Ycont + 0, colors[idxs[0xB][1]]);


                        bitmap.SetPixel(Xcont + 0x3, Ycont + 2, colors[idxs[0xC][0]]);
                        bitmap.SetPixel(Xcont + 0x7, Ycont + 0, colors[idxs[0xC][1]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 2, colors[idxs[0xD][0]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 0, colors[idxs[0xD][1]]);
                        bitmap.SetPixel(Xcont + 0x13, Ycont + 2, colors[idxs[0xE][0]]);
                        bitmap.SetPixel(Xcont + 0x17, Ycont + 0, colors[idxs[0xE][1]]);
                        bitmap.SetPixel(Xcont + 0x1B, Ycont + 2, colors[idxs[0xF][0]]);
                        bitmap.SetPixel(Xcont + 0x1F, Ycont + 0, colors[idxs[0xF][1]]);

                        //-------


                        bitmap.SetPixel(Xcont + 0x4, Ycont + 2, colors[idxs[0x10][0]]);
                        bitmap.SetPixel(Xcont + 0x0, Ycont + 0, colors[idxs[0x10][1]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 2, colors[idxs[0x11][0]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 0, colors[idxs[0x11][1]]);
                        bitmap.SetPixel(Xcont + 0x14, Ycont + 2, colors[idxs[0x12][0]]);
                        bitmap.SetPixel(Xcont + 0x10, Ycont + 0, colors[idxs[0x12][1]]);
                        bitmap.SetPixel(Xcont + 0x1C, Ycont + 2, colors[idxs[0x13][0]]);
                        bitmap.SetPixel(Xcont + 0x18, Ycont + 0, colors[idxs[0x13][1]]);


                        bitmap.SetPixel(Xcont + 0x5, Ycont + 2, colors[idxs[0x14][0]]);
                        bitmap.SetPixel(Xcont + 0x1, Ycont + 0, colors[idxs[0x14][1]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 2, colors[idxs[0x15][0]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 0, colors[idxs[0x15][1]]);
                        bitmap.SetPixel(Xcont + 0x15, Ycont + 2, colors[idxs[0x16][0]]);
                        bitmap.SetPixel(Xcont + 0x11, Ycont + 0, colors[idxs[0x16][1]]);
                        bitmap.SetPixel(Xcont + 0x1D, Ycont + 2, colors[idxs[0x17][0]]);
                        bitmap.SetPixel(Xcont + 0x19, Ycont + 0, colors[idxs[0x17][1]]);


                        bitmap.SetPixel(Xcont + 0x6, Ycont + 2, colors[idxs[0x18][0]]);
                        bitmap.SetPixel(Xcont + 0x2, Ycont + 0, colors[idxs[0x18][1]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 2, colors[idxs[0x19][0]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 0, colors[idxs[0x19][1]]);
                        bitmap.SetPixel(Xcont + 0x16, Ycont + 2, colors[idxs[0x1A][0]]);
                        bitmap.SetPixel(Xcont + 0x12, Ycont + 0, colors[idxs[0x1A][1]]);
                        bitmap.SetPixel(Xcont + 0x1E, Ycont + 2, colors[idxs[0x1B][0]]);
                        bitmap.SetPixel(Xcont + 0x1A, Ycont + 0, colors[idxs[0x1B][1]]);

                        bitmap.SetPixel(Xcont + 0x7, Ycont + 2, colors[idxs[0x1C][0]]);
                        bitmap.SetPixel(Xcont + 0x3, Ycont + 0, colors[idxs[0x1C][1]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 2, colors[idxs[0x1D][0]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 0, colors[idxs[0x1D][1]]);
                        bitmap.SetPixel(Xcont + 0x17, Ycont + 2, colors[idxs[0x1E][0]]);
                        bitmap.SetPixel(Xcont + 0x13, Ycont + 0, colors[idxs[0x1E][1]]);
                        bitmap.SetPixel(Xcont + 0x1F, Ycont + 2, colors[idxs[0x1F][0]]);
                        bitmap.SetPixel(Xcont + 0x1B, Ycont + 0, colors[idxs[0x1F][1]]);

                    }
                }
                void Deinterlace8bit(ref Bitmap bitmap, int Xcont, int Ycont, ref Color[] colors, ref byte[] indices, int IN, bool flipEmX)
                {
                    if (flipEmX == false)
                    {
                        bitmap.SetPixel(Xcont + 0x0, Ycont + 0, colors[indices[IN + 0x0]]);
                        bitmap.SetPixel(Xcont + 0x4, Ycont + 2, colors[indices[IN + 0x1]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 0, colors[indices[IN + 0x2]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 2, colors[indices[IN + 0x3]]);

                        bitmap.SetPixel(Xcont + 0x1, Ycont + 0, colors[indices[IN + 0x4]]);
                        bitmap.SetPixel(Xcont + 0x5, Ycont + 2, colors[indices[IN + 0x5]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 0, colors[indices[IN + 0x6]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 2, colors[indices[IN + 0x7]]);

                        bitmap.SetPixel(Xcont + 0x2, Ycont + 0, colors[indices[IN + 0x8]]);
                        bitmap.SetPixel(Xcont + 0x6, Ycont + 2, colors[indices[IN + 0x9]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 0, colors[indices[IN + 0xA]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 2, colors[indices[IN + 0xB]]);

                        bitmap.SetPixel(Xcont + 0x3, Ycont + 0, colors[indices[IN + 0xC]]);
                        bitmap.SetPixel(Xcont + 0x7, Ycont + 2, colors[indices[IN + 0xD]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 0, colors[indices[IN + 0xE]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 2, colors[indices[IN + 0xF]]);

                        bitmap.SetPixel(Xcont + 0x4, Ycont + 0, colors[indices[IN + 0x10]]);
                        bitmap.SetPixel(Xcont + 0x0, Ycont + 2, colors[indices[IN + 0x11]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 0, colors[indices[IN + 0x12]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 2, colors[indices[IN + 0x13]]);

                        bitmap.SetPixel(Xcont + 0x5, Ycont + 0, colors[indices[IN + 0x14]]);
                        bitmap.SetPixel(Xcont + 0x1, Ycont + 2, colors[indices[IN + 0x15]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 0, colors[indices[IN + 0x16]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 2, colors[indices[IN + 0x17]]);

                        bitmap.SetPixel(Xcont + 0x6, Ycont + 0, colors[indices[IN + 0x18]]);
                        bitmap.SetPixel(Xcont + 0x2, Ycont + 2, colors[indices[IN + 0x19]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 0, colors[indices[IN + 0x1A]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 2, colors[indices[IN + 0x1B]]);

                        bitmap.SetPixel(Xcont + 0x7, Ycont + 0, colors[indices[IN + 0x1C]]);
                        bitmap.SetPixel(Xcont + 0x3, Ycont + 2, colors[indices[IN + 0x1D]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 0, colors[indices[IN + 0x1E]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 2, colors[indices[IN + 0x1F]]);
                    }
                    else
                    {
                        bitmap.SetPixel(Xcont + 0x4, Ycont + 0, colors[indices[IN + 0x0]]);
                        bitmap.SetPixel(Xcont + 0x0, Ycont + 2, colors[indices[IN + 0x1]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 0, colors[indices[IN + 0x2]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 2, colors[indices[IN + 0x3]]);

                        bitmap.SetPixel(Xcont + 0x5, Ycont + 0, colors[indices[IN + 0x4]]);
                        bitmap.SetPixel(Xcont + 0x1, Ycont + 2, colors[indices[IN + 0x5]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 0, colors[indices[IN + 0x6]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 2, colors[indices[IN + 0x7]]);

                        bitmap.SetPixel(Xcont + 0x6, Ycont + 0, colors[indices[IN + 0x8]]);
                        bitmap.SetPixel(Xcont + 0x2, Ycont + 2, colors[indices[IN + 0x9]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 0, colors[indices[IN + 0xA]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 2, colors[indices[IN + 0xB]]);

                        bitmap.SetPixel(Xcont + 0x7, Ycont + 0, colors[indices[IN + 0xC]]);
                        bitmap.SetPixel(Xcont + 0x3, Ycont + 2, colors[indices[IN + 0xD]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 0, colors[indices[IN + 0xE]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 2, colors[indices[IN + 0xF]]);

                        bitmap.SetPixel(Xcont + 0x0, Ycont + 0, colors[indices[IN + 0x10]]);
                        bitmap.SetPixel(Xcont + 0x4, Ycont + 2, colors[indices[IN + 0x11]]);
                        bitmap.SetPixel(Xcont + 0x8, Ycont + 0, colors[indices[IN + 0x12]]);
                        bitmap.SetPixel(Xcont + 0xC, Ycont + 2, colors[indices[IN + 0x13]]);

                        bitmap.SetPixel(Xcont + 0x1, Ycont + 0, colors[indices[IN + 0x14]]);
                        bitmap.SetPixel(Xcont + 0x5, Ycont + 2, colors[indices[IN + 0x15]]);
                        bitmap.SetPixel(Xcont + 0x9, Ycont + 0, colors[indices[IN + 0x16]]);
                        bitmap.SetPixel(Xcont + 0xD, Ycont + 2, colors[indices[IN + 0x17]]);

                        bitmap.SetPixel(Xcont + 0x2, Ycont + 0, colors[indices[IN + 0x18]]);
                        bitmap.SetPixel(Xcont + 0x6, Ycont + 2, colors[indices[IN + 0x19]]);
                        bitmap.SetPixel(Xcont + 0xA, Ycont + 0, colors[indices[IN + 0x1A]]);
                        bitmap.SetPixel(Xcont + 0xE, Ycont + 2, colors[indices[IN + 0x1B]]);

                        bitmap.SetPixel(Xcont + 0x3, Ycont + 0, colors[indices[IN + 0x1C]]);
                        bitmap.SetPixel(Xcont + 0x7, Ycont + 2, colors[indices[IN + 0x1D]]);
                        bitmap.SetPixel(Xcont + 0xB, Ycont + 0, colors[indices[IN + 0x1E]]);
                        bitmap.SetPixel(Xcont + 0xF, Ycont + 2, colors[indices[IN + 0x1F]]);
                    }
                }
            }
            br.Close();
        }

        public void BMPtoTPL(string[] tplFile, bool isTemp = false)
        {
            FrmMain frmMain = new FrmMain();

            for (int i = 0; i < tplFile.Length; i++)
            {
                BinaryReader br = new BinaryReader(File.Open(tplFile[i], FileMode.Open));
                TPL tpl;

                tpl.magic = 0x00001000;
                tpl.tplCount = 1;
                tpl.startOffset = 0x10;
                tpl.unused1 = 0;

                br.BaseStream.Position = 0x12;
                tpl.width = (ushort)br.ReadUInt32();
                tpl.height = (ushort)br.ReadUInt32();

                br.BaseStream.Position = 0x1C;
                tpl.bitDepth = br.ReadUInt16();
                if (tpl.bitDepth == 4) { tpl.bitDepth = 8; }
                else if (tpl.bitDepth == 8) { tpl.bitDepth = 9; }
                else
                {
                    MessageBox.Show($"Bit depth {tpl.bitDepth}-bit not supported, images must be 4-bit (16 colors) or 8-bit (256 colors)",
                    "Bit Depth Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); br.Close(); continue;
                }

                tpl.interlace = 0;
                tpl.zPriority = 256;
                tpl.mipmapCount = frmMain.mipmapCount;
                if (tpl.width >= 128)
                {
                    if (tpl.bitDepth == 9)
                    {
                        tpl.scale = (ushort)(tpl.width * 16);
                    }
                    else
                    {
                        tpl.scale = (ushort)(tpl.width * 8);
                    }
                }
                else
                {
                    if (tpl.bitDepth == 9)
                    {
                        tpl.scale = (ushort)(tpl.width * 8);
                    }
                    else
                    {
                        tpl.scale = (ushort)(tpl.width * 4);
                    }
                }
                tpl.unused2 = 0;

                tpl.mipmapOffset1 = frmMain.mipmapOffset1;
                tpl.mipmapOffset2 = frmMain.mipmapOffset2;
                tpl.unk1 = frmMain.mipmapUnk1;
                tpl.unk2 = frmMain.mipmapUnk2;

                tpl.pixelsOffset = 0x40;
                tpl.paletteOffset = 0;
                tpl.unused3 = 0;

                // Render config parameters
                if (tpl.width > 128)
                {
                    tpl.config1 = 0x00; // This byte is 00 if width is > 128px
                }
                else
                {
                    tpl.config1 = 0x80; // This byte is 80 if width is < 256px
                }

                // Getting bit depth for tpl config
                if (tpl.bitDepth == 8)
                {
                    // Render config 2 is splitted in two nibbles,
                    // first nibble is related to bit depth
                    // second nibble is related to width
                    if (tpl.config1 == 00)
                    {
                        tpl.config2 = (byte)(0x40 + BitConverter.GetBytes(tpl.width)[1]);
                    }
                    else
                    {
                        tpl.config2 = 0x40;
                    }
                    tpl.paletteOffset = (uint)(tpl.width * tpl.height / 2 + 0x40);
                }
                else if (tpl.bitDepth == 9)
                {
                    if (tpl.config1 == 00)
                    {
                        tpl.config2 = (byte)(0x30 + BitConverter.GetBytes(tpl.width)[1]);
                    }
                    else
                    {
                        tpl.config2 = 0x30;
                    }
                    tpl.paletteOffset = (uint)(tpl.width * tpl.height + 0x40);
                }
                else
                {
                    tpl.config2 = 0x00;
                    tpl.paletteOffset = 0x00;
                }

                // The last render config increases accordingly to width & height parameters
                // currently only working with power of 2 type resolutions, eg. 8,16,32,64,128,256,512,1024
                // Documentation in portuguese: https://curt.link/g2NpZ2
                ushort defaultValue = 1229;
                for (int m = 0; m < 8; m++)
                {
                    if (tpl.width != Math.Pow(2, 3 + m))
                    {
                        defaultValue += 4;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int m = 0; m < 8; m++)
                {
                    if (tpl.height != Math.Pow(2, 3 + m))
                    {
                        defaultValue += 0x40;
                    }
                    else
                    {
                        break;
                    }
                }
                tpl.config3 = defaultValue; // This byte controls the way the texture is applied to the model
                tpl.unused4 = 0;
                tpl.unused5 = 0;
                tpl.endTag = 0x40;

                // Get pixels bytes
                br.BaseStream.Position = 0x0A;
                uint bmpPixelsOffset = br.ReadUInt32();
                br.BaseStream.Position = bmpPixelsOffset;
                byte[] bmpPixels = br.ReadBytes(tpl.width * tpl.height);

                // Get palette bytes
                br.BaseStream.Position = 0x2E;
                uint bmpPaletteCount = br.ReadUInt32();
                br.BaseStream.Position = 0x36;
                byte[] bmpPalette = br.ReadBytes((int)bmpPaletteCount * 4);

                //-------------------------------
                // WRITING DATA
                //-------------------------------
                string directory = $"Converted/{Path.GetFileNameWithoutExtension(tplFile[i])}.tpl";
                if (isTemp)
                {
                    if (tpl.bitDepth == 8) { directory = $".temp/0_16.tpl"; }
                    else if (tpl.bitDepth == 9) { directory = $".temp/0_256.tpl"; }
                    else { directory = $".temp/0.tpl"; }

                }
                BinaryWriter bw = new BinaryWriter(File.Open(directory, FileMode.Create));

                Console.WriteLine("Writing TPL header");
                bw.Write(tpl.magic);
                bw.Write(tpl.tplCount);
                bw.Write(tpl.startOffset);
                bw.Write(tpl.unused1);
                bw.Write(tpl.width);
                bw.Write(tpl.height);
                bw.Write(tpl.bitDepth);
                bw.Write(tpl.interlace);
                bw.Write(tpl.zPriority);
                bw.BaseStream.Position += 2;
                // bw.Write(tpl.mipmapCount);
                bw.Write(tpl.scale);
                bw.Write(tpl.unused2);
                bw.BaseStream.Position += 8;
                // bw.Write(tpl.mipmapOffset1);
                // bw.Write(tpl.mipmapOffset2);
                bw.Write(tpl.unk1);
                bw.Write(tpl.unk2);
                bw.Write(tpl.pixelsOffset);
                bw.Write(tpl.paletteOffset);
                bw.Write(tpl.unused3);
                bw.Write(tpl.config1);
                bw.Write(tpl.config2);
                bw.Write(tpl.config3);
                bw.Write(tpl.unused4);
                bw.Write(tpl.unused5);
                bw.Write(tpl.endTag);

                Console.WriteLine("Writing TPL pixels");
                // Writing pixels
                bw.Write(bmpPixels);

                Console.WriteLine("Writing TPL palette");
                // Writing palette
                if (tpl.bitDepth == 8)
                {
                    Console.WriteLine("4 bit texture");
                    for (int p = 0; p < bmpPalette.Length; p += 4)
                    {
                        if (p == 0x20)
                        {
                            for (int padding = 0; padding < 0x08; padding++)
                            {
                                bw.Write((uint)0);
                            }
                        }
                        bw.Write(bmpPalette[p + 2]);
                        bw.Write(bmpPalette[p + 1]);
                        bw.Write(bmpPalette[p]);
                        bw.Write(bmpPalette[p + 3]);
                    }
                    for (int padding = 0; padding < 0x08; padding++)
                    {
                        bw.Write((uint)0);
                    }
                }
                else if (tpl.bitDepth == 9)
                {
                    // Get palette bytes
                    br.BaseStream.Position = 0x36;
                    for (int j = 0; j < 0x8; j++)
                    {
                        byte[] b = br.ReadBytes(0x80);
                        byte[] chunk1 = b.Skip(0x00).Take(0x20).ToArray();
                        byte[] chunk2 = b.Skip(0x40).Take(0x20).ToArray();
                        byte[] chunk3 = b.Skip(0x20).Take(0x20).ToArray();
                        byte[] chunk4 = b.Skip(0x60).Take(0x20).ToArray();

                        for (int p = 0; p < chunk1.Length; p += 4)
                        {
                            bw.Write(chunk1[p + 2]);
                            bw.Write(chunk1[p + 1]);
                            bw.Write(chunk1[p]);
                            bw.Write(chunk1[p + 3]);
                        }
                        for (int p = 0; p < chunk2.Length; p += 4)
                        {
                            bw.Write(chunk2[p + 2]);
                            bw.Write(chunk2[p + 1]);
                            bw.Write(chunk2[p]);
                            bw.Write(chunk2[p + 3]);
                        }
                        for (int p = 0; p < chunk3.Length; p += 4)
                        {
                            bw.Write(chunk3[p + 2]);
                            bw.Write(chunk3[p + 1]);
                            bw.Write(chunk3[p]);
                            bw.Write(chunk3[p + 3]);
                        }
                        for (int p = 0; p < chunk4.Length; p += 4)
                        {
                            bw.Write(chunk4[p + 2]);
                            bw.Write(chunk4[p + 1]);
                            bw.Write(chunk4[p]);
                            bw.Write(chunk4[p + 3]);
                        }
                    }
                }
                br.Close();
                bw.Close();
            }
            //MessageBox.Show("Files converted successfully, check on the root of folder 'Converted'.",
            //    $"Success on converting {tplFile.Length} textures",
            //    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

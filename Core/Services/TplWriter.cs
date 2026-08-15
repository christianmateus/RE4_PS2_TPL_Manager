using System;
using System.IO;
using TplModel = RE4_PS2_TPL_Manager.TPLDefinition.TPL;

namespace RE4_PS2_TPL_Manager.Core.Services
{
    /// <summary>
    /// Binary write/rebuild operations for TPL files. Keeps file surgery out of the WinForms layer.
    /// </summary>
    public sealed class TplWriter
    {
        private readonly TplReader reader;

        public TplWriter(TplReader reader)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public void CreateEmpty(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                writer.Write((uint)4096);
                writer.Write((uint)0);
                writer.Write((uint)0x10);
                writer.Write((uint)0);
            }
        }

        /// <summary>
        /// Rebuilds header offsets after operations that changed payload sizes.
        /// Preserves the legacy file layout while fixing 32-bit payload sizing and avoiding
        /// repeated open/close cycles for every header.
        /// </summary>
        public void UpdateAllOffsets(string path)
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            using (BinaryReader br = new BinaryReader(stream, System.Text.Encoding.Default, true))
            using (BinaryWriter bw = new BinaryWriter(stream, System.Text.Encoding.Default, true))
            {
                if (stream.Length < 0x10)
                    throw new InvalidDataException("TPL file is smaller than the global header.");

                stream.Position = 4;
                uint textureCount = br.ReadUInt32();
                int totalMipmaps = 0;

                for (int i = 0; i < textureCount; i++)
                {
                    stream.Position = 0x1A + (0x30L * i);
                    ushort mipCount = br.ReadUInt16();
                    totalMipmaps += mipCount;
                }

                int headerCount = checked((int)textureCount + totalMipmaps);
                uint headerEnd = checked((uint)(0x10 + 0x30 * headerCount));
                uint dataCursor = headerEnd;
                uint mipHeaderCursor = checked((uint)(0x10 + 0x30 * textureCount));

                // Main texture headers: pixels immediately followed by palette.
                for (int i = 0; i < textureCount; i++)
                {
                    stream.Position = 0x10 + (0x30L * i);
                    ushort width = br.ReadUInt16();
                    ushort height = br.ReadUInt16();
                    ushort bitDepth = br.ReadUInt16();
                    stream.Position += 4; // interlace + zPriority
                    ushort mipCount = br.ReadUInt16();

                    // A main texture header only stores two mipmap-header offsets. Clear stale
                    // values first, then write the supported offsets while still advancing the
                    // cursor for every declared mipmap header.
                    stream.Position = 0x20 + (0x30L * i);
                    bw.Write((uint)0);
                    bw.Write((uint)0);
                    stream.Position = 0x20 + (0x30L * i);
                    for (int m = 0; m < mipCount; m++)
                    {
                        if (m < 2)
                            bw.Write(mipHeaderCursor);
                        mipHeaderCursor += 0x30;
                    }

                    stream.Position = 0x30 + (0x30L * i);
                    bw.Write(dataCursor);
                    dataCursor = checked(dataCursor + (uint)TplReader.GetPixelDataLength(width, height, bitDepth));

                    int paletteLength = TplReader.GetPaletteLength(bitDepth);
                    if (paletteLength > 0)
                    {
                        bw.Write(dataCursor);
                        dataCursor = checked(dataCursor + (uint)paletteLength);
                    }
                    else
                    {
                        // 32-bit textures do not use a CLUT.
                        bw.Write((uint)0);
                    }
                }

                // Mipmap headers follow the main texture headers. Mipmaps share the parent CLUT;
                // therefore only their pixel offset needs to be advanced.
                for (int i = 0; i < totalMipmaps; i++)
                {
                    int headerIndex = checked((int)textureCount + i);
                    stream.Position = 0x10 + (0x30L * headerIndex);
                    ushort width = br.ReadUInt16();
                    ushort height = br.ReadUInt16();
                    ushort bitDepth = br.ReadUInt16();

                    stream.Position = 0x30 + (0x30L * headerIndex);
                    bw.Write(dataCursor);
                    dataCursor = checked(dataCursor + (uint)TplReader.GetPixelDataLength(width, height, bitDepth));
                }

                bw.Flush();
            }
        }

        /// <summary>
        /// Replaces one texture payload/header with another TPL texture and rebuilds offsets.
        /// Existing mipmap linkage on the target texture is preserved.
        /// </summary>
        public void ReplaceTexture(string targetPath, int targetIndex, string replacementPath, int replacementIndex)
        {
            TplModel replacement = reader.ReadTexture(replacementPath, replacementIndex);
            ReplaceTexture(targetPath, targetIndex, replacement);
        }

        /// <summary>
        /// Replaces one texture directly from an in-memory encoded TPL texture.
        /// This is used by image import/color-depth workflows to avoid temporary files.
        /// </summary>
        public void ReplaceTexture(string targetPath, int targetIndex, TplModel replacement)
        {
            TplModel target = reader.ReadTexture(targetPath, targetIndex);
            if (replacement.header == null || replacement.header.Length != 0x30)
                throw new InvalidDataException("Replacement texture header must contain exactly 0x30 bytes.");

            byte[] replacementHeader = (byte[])replacement.header.Clone();
            PreserveMipMapMetadata(target.header, replacementHeader);

            byte[] fileBytes = File.ReadAllBytes(targetPath);
            long headerOffset = 0x10L + (0x30L * targetIndex);
            long pixelsEnd = checked((long)target.pixelsOffset + target.pixels.Length);
            long paletteEnd = target.palette != null && target.palette.Length > 0
                ? checked((long)target.paletteOffset + target.palette.Length)
                : pixelsEnd;

            if (headerOffset < 0 || headerOffset + 0x30 > fileBytes.Length ||
                target.pixelsOffset > fileBytes.Length || pixelsEnd > fileBytes.Length || paletteEnd > fileBytes.Length)
                throw new InvalidDataException("Target texture contains invalid offsets and cannot be safely replaced.");

            byte[] part1 = Slice(fileBytes, 0, (int)headerOffset);
            byte[] part2 = Slice(fileBytes, (int)(headerOffset + 0x30), (int)(target.pixelsOffset - (headerOffset + 0x30)));
            byte[] part3;
            byte[] part4;

            if (target.palette != null && target.palette.Length > 0)
            {
                part3 = Slice(fileBytes, (int)pixelsEnd, (int)(target.paletteOffset - pixelsEnd));
                part4 = Slice(fileBytes, (int)paletteEnd, fileBytes.Length - (int)paletteEnd);
            }
            else
            {
                part3 = new byte[0];
                part4 = Slice(fileBytes, (int)pixelsEnd, fileBytes.Length - (int)pixelsEnd);
            }

            using (BinaryWriter bw = new BinaryWriter(File.Open(targetPath, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                bw.Write(part1);
                bw.Write(replacementHeader);
                bw.Write(part2);
                bw.Write(replacement.pixels ?? new byte[0]);
                bw.Write(part3);
                if (replacement.palette != null && replacement.palette.Length > 0)
                    bw.Write(replacement.palette);
                bw.Write(part4);
            }

            UpdateAllOffsets(targetPath);
        }

        public void WriteSingleTexture(string path, TplModel texture)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException("Output path is empty.", nameof(path));
            if (texture.header == null || texture.header.Length != 0x30)
                throw new InvalidDataException("Texture header must contain exactly 0x30 bytes.");

            string directory = Path.GetDirectoryName(path);
            if (!String.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);

            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                bw.Write((uint)0x1000);
                bw.Write((uint)1);
                bw.Write((uint)0x10);
                bw.Write((uint)0);
                bw.Write(texture.header);
                bw.Write(texture.pixels ?? new byte[0]);
                if (texture.palette != null && texture.palette.Length > 0) bw.Write(texture.palette);
            }
        }

        private static void PreserveMipMapMetadata(byte[] targetHeader, byte[] replacementHeader)
        {
            if (targetHeader == null || replacementHeader == null || targetHeader.Length < 0x30 || replacementHeader.Length < 0x30)
                return;

            // mipmapCount
            Buffer.BlockCopy(targetHeader, 0x0A, replacementHeader, 0x0A, 2);
            // mipmapOffset1/2 + unknown1/2
            Buffer.BlockCopy(targetHeader, 0x10, replacementHeader, 0x10, 0x10);
        }

        private static byte[] Slice(byte[] source, int offset, int length)
        {
            if (length < 0 || offset < 0 || offset + length > source.Length)
                throw new InvalidDataException("Invalid TPL section boundary while rebuilding file.");
            byte[] result = new byte[length];
            Buffer.BlockCopy(source, offset, result, 0, length);
            return result;
        }
    }
}

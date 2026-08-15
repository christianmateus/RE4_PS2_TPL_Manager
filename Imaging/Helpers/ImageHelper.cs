using System;
using System.IO;

namespace RE4_PS2_TPL_Manager.Helpers
{
    public static class ImageHelper
    {
        public static int GetBmpBitDepth(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                br.BaseStream.Position = 28; // posição do campo "BitsPerPixel"
                ushort bitsPerPixel = br.ReadUInt16();
                return bitsPerPixel;
            }
        }

        public static int GetPngBitDepth(string path)
        {
            using (BinaryReader br = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                br.BaseStream.Position = 8; // pular assinatura PNG (8 bytes)

                // Lê o primeiro chunk (que deve ser IHDR)
                uint ihdrLength = br.ReadUInt32();
                string chunkType = new string(br.ReadChars(4));

                if (chunkType != "IHDR")
                    throw new Exception("PNG inválido: IHDR esperado");

                br.BaseStream.Position += 8; // pula width e height (4 bytes cada)

                byte bitDepth = br.ReadByte(); // Aqui está o bit depth
                return bitDepth;
            }
        }

        public static int GetJpegBitDepth(string path)
        {
            return 8;
        }

        public static int GetImageBitDepth(string path)
        {
            string ext = Path.GetExtension(path).ToLower();

            switch (ext)
            {
                case ".bmp":
                    return GetBmpBitDepth(path);
                case ".png":
                    return GetPngBitDepth(path);
                case ".jpg":
                case ".jpeg":
                    return GetJpegBitDepth(path);
                default:
                    throw new NotSupportedException("Formato de imagem não suportado");
            }
        }

    }
}

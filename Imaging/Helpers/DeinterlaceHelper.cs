using System.Drawing;

namespace RE4_PS2_TPL_Manager.Helpers
{
    public static class DeinterlaceHelper
    {
        public static void Deinterlace4bit(ref Bitmap bitmap, int Xcont, int Ycont, ref Color[] colors, ref byte[] indices, int IN, bool flipEmX)
        {
            int[,] pattern = new int[32, 2]
            {
                        { 4, 0 }, { 12, 8 }, { 20, 16 }, { 28, 24 }, { 5, 1 }, { 13, 9 }, { 21, 17 }, { 29, 25 },
                        { 6, 2 }, { 14, 10 }, { 22, 18 }, { 30, 26 }, { 7, 3 }, { 15, 11 }, { 23, 19 }, { 31, 27 },
                        { 0, 4 }, { 8, 12 }, { 16, 20 }, { 24, 28 }, { 1, 5 }, { 9, 13 }, { 17, 21 }, { 25, 29 },
                        { 2, 6 }, { 10, 14 }, { 18, 22 }, { 26, 30 }, { 3, 7 }, { 11, 15 }, { 19, 23 }, { 27, 31 }
            };

            for (int i = 0; i < 32; i++)
            {
                int val = indices[IN + i];
                int nibble1 = val >> 4;
                int nibble2 = val & 0x0F;

                int x1 = flipEmX ? pattern[i, 1] : pattern[i, 0];
                int x2 = flipEmX ? pattern[i, 0] : pattern[i, 1];

                bitmap.SetPixel(Xcont + x1, Ycont + 2, colors[nibble1]);
                bitmap.SetPixel(Xcont + x2, Ycont + 0, colors[nibble2]);
            }
        }

        public static void Deinterlace8bit(ref Bitmap bitmap, int Xcont, int Ycont, ref Color[] colors, ref byte[] indices, int IN, bool flipEmX)
        {
            int[] xMapA = { 0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15, 4, 0, 12, 8, 5, 1, 13, 9, 6, 2, 14, 10, 7, 3, 15, 11 };
            int[] xMapB = { 4, 0, 12, 8, 5, 1, 13, 9, 6, 2, 14, 10, 7, 3, 15, 11, 0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15 };

            int[] xMap = flipEmX ? xMapB : xMapA;

            for (int i = 0; i < 32; i++)
            {
                int yOffset = (i % 2 == 0) ? 0 : 2;
                bitmap.SetPixel(Xcont + xMap[i], Ycont + yOffset, colors[indices[IN + i]]);
            }
        }
    }
}

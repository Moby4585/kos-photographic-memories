﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.API.Datastructures;
using System.Drawing;

namespace kosphotography
{
    class PhotoBitmap : IBitmap
    {
        int width = 255;
        int height = 255;

        public int Width => width;

        public int Height => height;

        public Bitmap bitmapGrayscale;

        public byte[] PixelsGrayscale;

        public int[] Pixels => GetBitmapAsInts();

        public PhotoBitmap()
        {
            bitmapGrayscale = new Bitmap(width, height);
            PixelsGrayscale = new byte[width * height];
        }

        public Color GetPixel(int x, int y)
        {
            Color col = bitmapGrayscale.GetPixel(Math.Min(x, bitmapGrayscale.Width - 1), Math.Min(y, bitmapGrayscale.Height - 1));
            /*int colRed = (int)(((float)col.R) * 0.393f + ((float)col.G) * 0.769f + ((float)col.B) * 0.189f);
            int colGreen = (int)(((float)col.R) * 0.349f + ((float)col.G) * 0.686f + ((float)col.B) * 0.168f);
            int colBlue = (int)(((float)col.R) * 0.272f + ((float)col.G) * 0.534f + ((float)col.B) * 0.131f);*/

            int color = GameMath.LerpRgbaColor(((float)col.R) / 255f, Color.FromArgb(35, 31, 26).ToArgb(), Color.FromArgb(254, 225, 181).ToArgb());

            //return Color.FromArgb(Math.Min(colRed, 255), Math.Min(colGreen, 255), Math.Min(colBlue, 255));
            return Color.FromArgb(color);
        }

        public Color GetPixelRel(float x, float y)
        {
            return GetPixel((int)((float)width * x), (int)((float)height * x));
        }

        public int[] GetPixelsTransformed(int rot = 0, int alpha = 100)
        {
            return GetBitmapAsInts();
        }

        int[] GetBitmapAsInts()
        {
            List<int> pixels = new List<int>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels.Add(GetPixel(x, y).ToArgb());
                    //pixels.Add(bitmapGrayscale.GetPixel(x, y).ToArgb());
                }
            }
            return pixels.ToArray();
        }

        public void setBitmap(Bitmap bmp)
        {
            width = bmp.Width;
            height = bmp.Height;

            int edge = Math.Min(width, height);
            int longEdge = Math.Max(width, height);
            bool isHeightEdge = height <= width;
            int deadshift = (longEdge - edge) / 2;

            List<byte> pixelsByte = new List<byte>();
            if (isHeightEdge)
            {
                width = height;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < height; x++)
                    {
                        pixelsByte.Add((byte)(bmp.GetPixel(x + deadshift, y).B * (byte)2));
                    }
                }
            }

            PixelsGrayscale = pixelsByte.ToArray();

            bitmapGrayscale = BitmapUtil.GrayscaleBitmapFromPixels(PixelsGrayscale, width, height);
        }
    }
}

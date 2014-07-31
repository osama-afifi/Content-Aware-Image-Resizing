using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
///Algorithms Project
///Content Aware Resize
///

namespace ContentAwareResize
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct MyPixel
    {
        public byte red, green, blue;
    }
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static MyPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            MyPixel[,] Buffer = new MyPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[0];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[2];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(MyPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(MyPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Calculate energy between the given two pixels
        /// </summary>
        /// <param name="Pixel1">First pixel color</param>
        /// <param name="Pixel2">Second pixel color</param>
        /// <returns>Energy between the 2 pixels</returns>
        public static int CalculatePixelsEnergy(MyPixel Pixel1, MyPixel Pixel2)
        {
            int Energy = Math.Abs(Pixel1.red - Pixel2.red) + Math.Abs(Pixel1.green - Pixel2.green) + Math.Abs(Pixel1.blue - Pixel2.blue);
            return Energy;
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(MyPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[2] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }

        // To Bitmap
        // Create Image:
        //==============
       public static Bitmap toBitmap(MyPixel[,] ImageMatrix)
        {
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[0] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[2] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            return ImageBMP;
        }


        /// <summary>
        /// Normal resize of an image
        /// </summary>
        /// <param name="ImageMatrix">2D array of image values</param>
        /// <param name="NewWidth">desired width</param>
        /// <param name="NewHeight">desired height</param>
        /// <returns>Resized image</returns>
        public static MyPixel[,] NormalResize(MyPixel[,] ImageMatrix, int NewWidth, int NewHeight)
        {
            int i = 0, j = 0;
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            double WidthRatio = (double)(Width) / (double)(NewWidth);
            double HeightRatio = (double)(Height) / (double)(NewHeight);

            int OldWidth = Width;
            int OldHeight = Height;

            MyPixel P1, P2, P3, P4;

            MyPixel Y1, Y2, X = new MyPixel();

            MyPixel[,] Data = new MyPixel[NewHeight, NewWidth];

            Width = NewWidth;
            Height = NewHeight;

            int floor_x, ceil_x;
            int floor_y, ceil_y;

            double x, y;
            double fraction_x, one_minus_x;
            double fraction_y, one_minus_y;

            for (j = 0; j < NewHeight; j++)
                for (i = 0; i < NewWidth; i++)
                {
                    x = (double)(i) * WidthRatio;
                    y = (double)(j) * HeightRatio;

                    floor_x = (int)(x);
                    ceil_x = floor_x + 1;
                    if (ceil_x >= OldWidth) ceil_x = floor_x;

                    floor_y = (int)(y);
                    ceil_y = floor_y + 1;
                    if (ceil_y >= OldHeight) ceil_y = floor_y;

                    fraction_x = x - floor_x;
                    one_minus_x = 1.0 - fraction_x;

                    fraction_y = y - floor_y;
                    one_minus_y = 1.0 - fraction_y;

                    P1 = ImageMatrix[floor_y, floor_x];
                    P2 = ImageMatrix[ceil_y, floor_x];
                    P3 = ImageMatrix[floor_y, ceil_x];
                    P4 = ImageMatrix[ceil_y, ceil_x];

                    Y1.red = (byte)(one_minus_y * P1.red + fraction_y * P2.red);
                    Y1.green = (byte)(one_minus_y * P1.green + fraction_y * P2.green);
                    Y1.blue = (byte)(one_minus_y * P1.blue + fraction_y * P2.blue);

                    Y2.red = (byte)(one_minus_y * P3.red + fraction_y * P4.red);
                    Y2.green = (byte)(one_minus_y * P3.green + fraction_y * P4.green);
                    Y2.blue = (byte)(one_minus_y * P3.blue + fraction_y * P4.blue);

                    X.red = (byte)(one_minus_x * Y1.red + fraction_x * Y2.red);
                    X.green = (byte)(one_minus_x * Y1.green + fraction_x * Y2.green);
                    X.blue = (byte)(one_minus_x * Y1.blue + fraction_x * Y2.blue);

                    Data[j, i] = X;
                }

            return Data;
        }
    }
}

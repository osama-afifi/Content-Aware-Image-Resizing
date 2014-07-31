using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentAwareResize
{
   
    class coord
    {
        public int row;
        public int col;
        public coord(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    class ContentAwareResize
    {
        //int newHeight;
        //int newWidth;
        public int CurHeight;
        public int CurWidth;
        int[,] dp;
        int[,] path;
        MyPixel[,] sampleImage;

        public ContentAwareResize(MyPixel[,] sampleImage)
        {
            this.sampleImage = sampleImage;
            CurHeight = CurWidth = 0;
        }

        private bool validMove(int i, int j)
        {
            return false;
        }

        private void reCreatePath(int finish, out List<coord> seamPixels)
        {
            seamPixels = new List<coord>();
            int cur = finish;
            for(int i=CurHeight-1; i>=0 ; i--)
            {
                seamPixels.Add(new coord(i, cur));
                cur = path[i, cur];
            }
            seamPixels.Reverse();
        }

        private int verSeamCarve()
        {

            for (int j = 0; j < CurWidth; j++)
                dp[0,j]=0;
            for (int j = 0; j < CurHeight; j++)
                dp[j, 0] = dp[j,CurWidth - 1] = (int)1e9;

            for (int i = 1; i < CurHeight; i++)
                for (int j = 1; j < CurWidth - 1; j++)
                {
                    int sol1 = dp[i - 1, j - 1] + ImageOperations.CalculatePixelsEnergy(sampleImage[i, j - 1], sampleImage[i, j + 1]) + ImageOperations.CalculatePixelsEnergy(sampleImage[i, j - 1], sampleImage[i - 1, j]);
                    int sol2 = dp[i - 1, j] + ImageOperations.CalculatePixelsEnergy(sampleImage[i, j - 1], sampleImage[i, j + 1]);
                    int sol3 = dp[i - 1, j + 1] + ImageOperations.CalculatePixelsEnergy(sampleImage[i, j - 1], sampleImage[i, j + 1]) + ImageOperations.CalculatePixelsEnergy(sampleImage[i, j + 1], sampleImage[i - 1, j]);
                    dp[i, j] = Math.Min(sol1, Math.Min(sol2, sol3));
                    if (dp[i, j] == sol1)
                        path[i, j] = j - 1;
                    else if (dp[i, j] == sol2)
                        path[i, j] = j;
                    else
                        path[i, j] = j + 1;
                }

          int miniIndex = -1;
          int mini = (int)1e9;
          for (int j = 1; j < CurWidth - 1; j++)
          {
              if (dp[CurHeight - 1, j] < mini)
              {
                  mini = dp[CurHeight - 1, j];
                  miniIndex = j;
              }
          }
            return miniIndex;
        }

        void bindVertical(ref List<coord> seam)
        {
            foreach (coord p in seam)
            {
                for (int i = p.col + 1; i < CurWidth; i++)
                {
                    sampleImage[p.row, i - 1] = sampleImage[p.row, i];
                }
            }
            CurWidth--;
        }

        private void reShiftImage()
        {
            MyPixel[,] tempImage = new MyPixel[CurHeight, CurWidth];
            for (int i = 0; i < CurHeight; i++)
                for (int j = 0; j < CurWidth; j++)
                    tempImage[i, j] = sampleImage[i, j];
            sampleImage = tempImage;            
        }

        public MyPixel[,] seamCarve(int newHeight, int newWidth)
        {
            CurHeight = sampleImage.GetLength(0);
            CurWidth = sampleImage.GetLength(1);
            dp = new int[CurHeight, CurWidth];
            path = new int[CurHeight, CurWidth];


            while (newWidth < CurWidth)
            {
                int colStart = verSeamCarve();
                List<coord> seam;
                reCreatePath(colStart, out seam);
                bindVertical(ref seam);
            }

            reShiftImage();
            return sampleImage;
        }

    }
}

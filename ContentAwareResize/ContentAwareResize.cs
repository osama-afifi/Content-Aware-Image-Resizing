using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentAwareResize
{
    #region Helper Coord Class
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

    #endregion


    class ContentAwareResize
    {
        #region Class members

        public int CurHeight;
        public int CurWidth;

        int[,] dp;
        int[,] path;
        MyPixel[,] sampleImage;

        #endregion

        public ContentAwareResize(MyPixel[,] sampleImage)
        {
            this.sampleImage = sampleImage;
            CurHeight = CurWidth = 0;
        }

        private void reShiftImage()
        {
            MyPixel[,] tempImage = new MyPixel[CurHeight, CurWidth];
            for (int i = 0; i < CurHeight; i++)
                for (int j = 0; j < CurWidth; j++)
                    tempImage[i, j] = sampleImage[i, j];
            sampleImage = tempImage;
        }


        #region Vertical Seam Carving

        private void reCreateVerPath(int finish, out List<coord> seamPixels)
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

        #endregion

        #region Horizontal Seam Carving

        private void reCreateHorPath(int finish, out List<coord> seamPixels)
        {
            seamPixels = new List<coord>();
            int cur = finish;
            for (int i = CurWidth - 1; i >= 0; i--)
            {
                seamPixels.Add(new coord(cur, i));
                cur = path[cur, i];
            }
            seamPixels.Reverse();
        }

        private int horSeamCarve()
        {

            for (int j = 0; j < CurHeight; j++)
                dp[j, 0] = 0;

            for (int j = 0; j < CurWidth; j++)
                dp[0, j] = dp[CurHeight - 1, j] = (int)1e9;

            for (int i = 1; i < CurWidth; i++)
                for (int j = 1; j < CurHeight - 1; j++)
                {
                    int sol1 = dp[j - 1, i - 1] + ImageOperations.CalculatePixelsEnergy(sampleImage[j - 1, i], sampleImage[j + 1, i]) + ImageOperations.CalculatePixelsEnergy(sampleImage[j - 1, i], sampleImage[j, i - 1]);
                    int sol2 = dp[j, i - 1] + ImageOperations.CalculatePixelsEnergy(sampleImage[j - 1, i], sampleImage[j + 1, i]);
                    int sol3 = dp[j + 1, i - 1] + ImageOperations.CalculatePixelsEnergy(sampleImage[j - 1, i], sampleImage[j + 1, i]) + ImageOperations.CalculatePixelsEnergy(sampleImage[j + 1, i], sampleImage[j, i - 1]);
                    dp[j, i] = Math.Min(sol1, Math.Min(sol2, sol3));
                    if (dp[j, i] == sol1)
                        path[j, i] = j - 1;
                    else if (dp[j, i] == sol2)
                        path[j, i] = j;
                    else
                        path[j, i] = j + 1;
                }

            int miniIndex = -1;
            int mini = (int)1e9;
            for (int j = 1; j < CurHeight - 1; j++)
            {
                if (dp[j, CurWidth - 1] < mini)
                {
                    mini = dp[j, CurWidth - 1];
                    miniIndex = j;
                }
            }
            return miniIndex;
        }

        void bindHorizontal(ref List<coord> seam)
        {
            foreach (coord p in seam)
            {
                for (int i = p.row + 1; i < CurHeight; i++)
                {
                    sampleImage[i - 1, p.col] = sampleImage[i, p.col];
                }
            }
            CurHeight--;
        }

        #endregion
    

        // Main Algorithm Function Call
        public MyPixel[,] seamCarve(int newHeight, int newWidth)
        {
            CurHeight = sampleImage.GetLength(0);
            CurWidth = sampleImage.GetLength(1);
            dp = new int[CurHeight, CurWidth];
            path = new int[CurHeight, CurWidth];

            // Vertical Seam Reduce
            while (newWidth < CurWidth)
            {
                int colStart = verSeamCarve();
                List<coord> seam;
                reCreateVerPath(colStart, out seam);
                bindVertical(ref seam);
            }

            //Horizontal Seam Reduce
            while (newHeight < CurHeight)
            {
                int rowStart = horSeamCarve();
                List<coord> seam;
                reCreateHorPath(rowStart, out seam);
                bindHorizontal(ref seam);
            }

            reShiftImage();
            return sampleImage;
        }

    }
}

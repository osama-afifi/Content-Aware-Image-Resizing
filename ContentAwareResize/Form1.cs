using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace ContentAwareResize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MyPixel[,] ImageMatrix;
        MyPixel[,] contentAwareImage;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();

        }

        private void btnNormalRersize_Click(object sender, EventArgs e)
        {
            int newWidth = int.Parse(txtWidth.Text);
            int newHeight = int.Parse(txtHeight.Text);
            MyPixel[,] resizedImage = ImageOperations.NormalResize(ImageMatrix, newWidth, newHeight);

            ImageOperations.DisplayImage(resizedImage, pictureBox2);
        }

        private void ContentAwareButton_Click(object sender, EventArgs e)
        {
            int newWidth = int.Parse(txtWidth.Text);
            int newHeight = int.Parse(txtHeight.Text);
            ContentAwareResize CA = new ContentAwareResize(ImageMatrix);
            contentAwareImage = CA.seamCarve(newHeight, newWidth);
            //contentAwareImage =  ImageOperations.NormalResize(contentAwareImage, newWidth, newHeight);

            ImageOperations.DisplayImage(contentAwareImage, pictureBox2);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }
                pictureBox2.Image.Save(sfd.FileName, format);
            }
        }

        
        
    }
}
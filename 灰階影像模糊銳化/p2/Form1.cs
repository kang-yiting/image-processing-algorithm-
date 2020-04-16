using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging;
namespace p2
{
    public partial class Form1 : Form
    {
        private int[,] arr;
        private Bitmap source;

        public sealed class HSI
        {
            public double Hue;
            public double Saturation;
            public double Intensity;

            public HSI() { }
            public HSI(double hue, double saturation, double intensity)
            {
                this.Hue = hue;
                this.Saturation = saturation;
                this.Intensity = intensity;
            }
        }

        public sealed class RGB
        {
            public int Red;
            public int Green;
            public int Blue;
            public RGB() { }
            public RGB(int red, int green, int blue)
            {
                this.Red = red;
                this.Green = green;
                this.Blue = blue;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) 
            {

                source = new Bitmap(openFileDialog1.FileName);
                if (source.PixelFormat != PixelFormat.Format8bppIndexed) 
                {
                    MessageBox.Show("Invalid pixel format!");
                    source.Dispose();
                    return;
                }
                LoadFromBitmap(source);
            }
        }

        private void LoadFromBitmap(Bitmap source) 
        {
            pictureBox1.Image = source;
            arr = new int[pictureBox1.Image.Height, pictureBox1.Image.Width];
            for (int i = 0; i < source.Height; i++) 
            {
                for (int j = 0; j < source.Width; j++) 
                {
                    arr[i, j] = source.GetPixel(j, i).R;
                }
            }
        
        }

        private void enhancernentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int[] GrayLevel = new int[256];

            int height = arr.GetLength(0);
            int width = arr.GetLength(1);
            int[,] result = new int[height, width];
            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                    int inx = arr[i, j];
                    GrayLevel[inx]++;
                }
            }
            double[] Transform = new double[256];
            for (int i = 0; i < 256; i++) 
            {
                Transform[i] = (double)GrayLevel[i] / (double)(height * width);
            }
            for (int i = 1; i < 256; i++) 
            {
                Transform[i] = Transform[i] + Transform[i - 1];
            }
            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                    int inx = arr[i, j];
                    result[i, j] = (int)(Transform[inx] * (double)255);
                }
            }
            progressBar1.Minimum = 0;
            progressBar1.Maximum= height * width;
            progressBar1.Value = 0;

            Bitmap resultBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                    int g = result[i, j];
                    resultBmp.SetPixel(j, i, Color.FromArgb(g, g, g));
                    progressBar1.Increment(1);
                }
            
            }
            pictureBox2.Image = resultBmp;
            progressBar1.Value = 0;
        }

        private void blurringToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int height = arr.GetLength(0), width = arr.GetLength(1);
            int[,] result = new int[height, width];

            progressBar1.Minimum = 0;
            progressBar1.Maximum = height * width;
            progressBar1.Value = 0;

            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                    int Sum = 0, count = 0;
                    for (int x = -1; x <= 1; x++) 
                    {
                        for (int y = -1; y <= 1; y++) 
                        {
                            if (i + x >= 0 && i + x < height && j + y >= 0 && j + y < width) 
                            {
                                Sum += arr[i + x, j + y];
                                count++;
                            }
                        
                        }
                    }
                    result[i, j] = Sum / count;
                    progressBar1.Increment(1);
                }
            
            }
            Bitmap resultBmp = new Bitmap(height, width, PixelFormat.Format24bppRgb);
            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                resultBmp.SetPixel(j,i,Color.FromArgb(result[i,j],result[i,j],result[i,j]));
                }
            }
            pictureBox2.Image=resultBmp;
            progressBar1.Value=0;
        }

        private void sharpeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int width = arr.GetLength(0);
            int height = arr.GetLength(1);
            int[,] result = new int[height, width];

            progressBar1.Minimum = 0;
            progressBar1.Maximum = height * width;
            progressBar1.Value = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int Sum = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) 
                            {
                                continue;
                            }
                            if (i + x >= 0 && i + x < height && j + y >= 0 && j + y < width) 
                            {
                                Sum += (-1 * arr[i + x, j + y]);
                            }

                        }
                    }
                    Sum += (8 * arr[i, j]);
                    result[i, j] = (Sum >= 0) ? Sum : 0;
                    progressBar1.Increment(1);
                }
            }

            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                    result[i, j] = (arr[i, j] + result[i, j] > 255) ? 255 : arr[i, j] + result[i, j];
                }
            }
            Bitmap resultBmp = new Bitmap(height, width, PixelFormat.Format24bppRgb);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    resultBmp.SetPixel(j, i, Color.FromArgb(result[i, j], result[i, j], result[i, j]));
                }
            }
            pictureBox2.Image = resultBmp;
            progressBar1.Value = 0;
        }

        private void grayLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap source = (Bitmap)pictureBox1.Image;
            int[,] R = new int[source.Height, source.Width];
            int[,] G = new int[source.Height, source.Width];
            int[,] B = new int[source.Height, source.Width];
            progressBar1.Minimum = 0;
            progressBar1.Maximum = (source.Width * source.Height);
            progressBar1.Value = 0;
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    int a = (source.GetPixel(j, i).R + source.GetPixel(j, i).G + source.GetPixel(j, i).B) / 3;
                    R[i, j] = a;
                    G[i, j] = a;
                    B[i, j] = a;
                }
            }
            Bitmap result = new Bitmap(source.Width, source.Height);
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    result.SetPixel(j, i, Color.FromArgb(R[i, j], G[i, j], B[i, j]));
                    progressBar1.Increment(1);
                }
            }
            progressBar1.Value = 0;
            pictureBox2.Image = result;
        }

        private void openRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap source = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = source;
            }
        }

        public HSI to_HSI(RGB rgb)
        {
            HSI hsi = new HSI();

            double r = (rgb.Red / 255.0);
            double g = (rgb.Green / 255.0);
            double b = (rgb.Blue / 255.0);

            double theta = Math.Acos(0.5 * ((r - g) + (r - b)) / Math.Sqrt((r - g) * (r - g) + (r - b) * (g - b))) / (2 * Math.PI);

            hsi.Hue = (b <= g) ? theta : (1 - theta);

            hsi.Saturation = 1 - 3 * Math.Min(Math.Min(r, g), b) / (r + g + b);

            hsi.Intensity = (r + g + b) / 3;

            return hsi;
        }

        public RGB to_RGB(HSI hsi)
        {
            RGB rgb = new RGB();
            double r, g, b;

            double h = hsi.Hue;
            double s = hsi.Saturation;
            double i = hsi.Intensity;

            h = h * 2 * Math.PI;

            if (h >= 0 && h < 2 * Math.PI / 3)
            {
                b = i * (1 - s);
                r = i * (1 + s * Math.Cos(h) / Math.Cos(Math.PI / 3 - h));
                g = 3 * i - (r + b);
            }
            else if (h >= 2 * Math.PI / 3 && h < 4 * Math.PI / 3)
            {
                r = i * (1 - s);
                g = i * (1 + s * Math.Cos(h - 2 * Math.PI / 3) / Math.Cos(Math.PI - h));
                b = 3 * i - (r + g);
            }
            else //if (h >= 4 * Math.PI / 3 && h <= 2 * Math.PI)
            {
                g = i * (1 - s);
                b = i * (1 + s * Math.Cos(h - 4 * Math.PI / 3) / Math.Cos(5 * Math.PI / 3 - h));
                r = 3 * i - (g + b);
            }

            r *= 255;
            g *= 255;
            b *= 255;

            if (r > 255)
                r = 255;
            else if (r < 0)
                r = 0;

            if (g > 255)
                g = 255;
            else if (g < 0)
                g = 0;

            if (b > 255)
                b = 255;
            else if (b < 0)
                b = 0;
            rgb.Red = Convert.ToInt32(r);
            rgb.Blue = Convert.ToInt32(b);
            rgb.Green = Convert.ToInt32(g);
            return rgb;
        }

        private void histogramEqualizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap source = (Bitmap)pictureBox1.Image;

            int[] graylevel = new int[256];

            HSI[,] hsi = new HSI[source.Height, source.Width];


            progressBar1.Minimum = 0;
            progressBar1.Maximum = (source.Width * source.Height*4);
            progressBar1.Value = 0;

            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    RGB rgb = new RGB(source.GetPixel(j, i).R, source.GetPixel(j, i).G, source.GetPixel(j, i).B);
                    hsi[i, j] = to_HSI(rgb);
                    progressBar1.Increment(1);
                }
            }


            ////
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    int inx = Convert.ToInt32(hsi[i, j].Intensity * 255);
                    graylevel[inx]++;
                    progressBar1.Increment(1);
                }
            }
            double[] Transform = new double[256];
            for (int i = 0; i < 256; i++)
            {
                Transform[i] = (double)graylevel[i] / (double)(source.Height * source.Width);
            }
            for (int i = 1; i < 256; i++)
            {
                Transform[i] = Transform[i] + Transform[i - 1];
            }
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    int inx = Convert.ToInt32(hsi[i, j].Intensity * 255);
                    hsi[i, j].Intensity = Transform[inx];
                    progressBar1.Increment(1);
                }
            }

            ////
            Bitmap result = new Bitmap(source.Width, source.Height);
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    RGB rgb = new RGB();
                    rgb = to_RGB(hsi[i, j]);
                    result.SetPixel(j, i, Color.FromArgb(rgb.Red, rgb.Green, rgb.Blue));
                    progressBar1.Increment(1);
                }
            }
            progressBar1.Value = 0;
            pictureBox2.Image = result;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}

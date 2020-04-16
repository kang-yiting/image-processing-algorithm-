using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace image
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
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
        public HSI to_HSI(RGB rgb) 
        {
            HSI hsi = new HSI();
            if (rgb.Red == 255 && rgb.Green == 255 && rgb.Blue == 255) 
            {
                
                hsi.Hue = 0;
                hsi.Saturation = 0;
                hsi.Intensity = 1;
            }
            else if (rgb.Red == 0 && rgb.Green == 0 && rgb.Blue == 0)
            {
                
                hsi.Hue = 0;
                hsi.Saturation = 0;
                hsi.Intensity = 0;
            }
            else if (rgb.Red == rgb.Blue && rgb.Red == rgb.Green) 
            {
                hsi.Hue = 0;
                hsi.Saturation = 0;
                hsi.Intensity = rgb.Green/255.0;
            }
            else
            {


                double r = (rgb.Red / 255.0);
                double g = (rgb.Green / 255.0);
                double b = (rgb.Blue / 255.0);

                double theta = Math.Acos(0.5 * ((r - g) + (r - b)) / Math.Sqrt((r - g) * (r - g) + (r - b) * (g - b))) / (2 * Math.PI);

                hsi.Hue = (b <= g) ? theta : (1 - theta);

                hsi.Saturation = 1 - 3 * Math.Min(Math.Min(r, g), b) / (r + g + b);

                hsi.Intensity = (r + g + b) / 3;


            }
            return hsi;
        }
        public RGB to_RGB(HSI hsi) 
        {
            RGB rgb=new RGB();
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

        private void btnopen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap source = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = source;
            }
        }

        private void btntran_Click(object sender, EventArgs e)
        {
            Bitmap source = (Bitmap)pictureBox1.Image;

            int[] graylevel = new int[256];

            HSI[,] hsi = new HSI[source.Height, source.Width];


            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    RGB rgb = new RGB(source.GetPixel(j, i).R, source.GetPixel(j, i).G, source.GetPixel(j, i).B);
                    hsi[i,j] = to_HSI(rgb);
                }
            }


            ////
            for (int i = 0; i < source.Height; i++) 
            {
                for (int j = 0; j < source.Width; j++) 
                {
                    int inx = Convert.ToInt32(hsi[i, j].Intensity * 255.0);
                    graylevel[inx]++;
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
            for(int i=0;i<source.Height;i++)
            {
                for (int j = 0; j < source.Width; j++) 
                {
                    int inx = Convert.ToInt32(hsi[i, j].Intensity * 255.0);
                    hsi[i, j].Intensity = Transform[inx];
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
                }
            }
            pictureBox2.Image = result;


        }

    }
}

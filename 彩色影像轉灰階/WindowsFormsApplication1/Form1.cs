using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) 
            {
                Bitmap source = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = source;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap source = (Bitmap)pictureBox1.Image;
            int[,] R = new int[source.Height, source.Width];
            int[,] G = new int[source.Height, source.Width];
            int[,] B = new int[source.Height, source.Width];
            for (int i = 0; i < source.Height; i++)
            {
                for (int j = 0; j < source.Width; j++)
                {
                    int a = (source.GetPixel(j, i).R + source.GetPixel(j, i).G + source.GetPixel(j, i).B)/3;
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
                }
            }
            pictureBox2.Image = result;
        }
    }
}

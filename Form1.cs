using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace logotst
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Bitmap bmp = new Bitmap(289, 199);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.DarkCyan);
                }
            }
            pictureBox1.Image = bmp;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap logo = (Bitmap)Image.FromFile(openFileDialog1.FileName);
                    if (logo.Width > 288)
                    {
                        MessageBox.Show("Image Width isnt allow to exceed 288 pixels!");
                        return;
                    }
                    if (logo.Height > 72)
                    {
                        MessageBox.Show("Image Height isnt allow to exceed 72 pixels!");
                        return;
                    }
                    int startx = (288 - logo.Width) / 2;
                    int starty = (72 - logo.Height) / 2;
                    Bitmap bmp = new Bitmap(pictureBox1.Image);
                    for (int y = 7 * 18; y < 199; y++)
                    {
                        for (int x = 0; x < 289; x++)
                        {
                            bmp.SetPixel(x, y, Color.DarkCyan);
                        }
                    }
                    for (int y = 0 ; y < logo.Height ; y++)
                    {
                        for (int x = 0 ; x < logo.Width ; x++)
                        {
                            Color test = logo.GetPixel(x, y);
                            if (test.B > 150 && test.G > 150 && test.R > 150)
                                bmp.SetPixel(startx + x, 7*18+starty + y, Color.DarkCyan);
                            else if (test.B < 150 && test.G < 150 && test.R < 150)
                                bmp.SetPixel(startx + x, 7 * 18 + starty + y, Color.Black);
                            else
                                bmp.SetPixel(startx + x, 7 * 18 + starty + y, Color.White);
                        }
                    }
                    pictureBox1.Image = bmp;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.Filter = "MCM files (*.mcm)|*.mcm";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap bmp = new Bitmap(289, 199);
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            bmp.SetPixel(x, y, Color.DarkCyan);
                        }
                    }
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        StreamReader sr = new StreamReader(myStream);
                        if (sr.ReadLine() != "MAX7456")
                        {
                            MessageBox.Show("Image Height isnt allow to exceed 72 pixels!");
                            return;
                        }
                        int charx = 8, chary = 0;
                        byte[] gdata = new byte[256];
                        string line = "";
                        for (int g = 0; g < 256; g++)
                        {
                            gdata = new byte[256];
                            for (int i = 0; i < 64; i++)
                            {
                                line = sr.ReadLine();
                                for (int c = 0; c < 4; c++)
                                {
                                    gdata[(i * 4) + c] |= (byte)((line[c * 2] == '1' ? 1 : 0) << 1);
                                    gdata[(i * 4) + c] |= (byte)((line[(c * 2) + 1] == '1' ? 1 : 0));
                                }
                            }
                            //draw pixels;
                            for (int y = 0; y < 18; y++)
                            {
                                for (int x = 0; x < 12; x++)
                                {
                                    if (gdata[x + y*12] == 2)
                                        bmp.SetPixel(charx * 12 + x, chary * 18 + y, Color.White);
                                    else if (gdata[x + y * 12] == 0)
                                        bmp.SetPixel(charx * 12 + x, chary * 18 + y, Color.Black);
                                    else
                                        bmp.SetPixel(charx * 12 + x, chary * 18 + y, Color.DarkCyan);
                                }
                            }
                            charx++;
                            if (charx==24)
                            {
                                chary++;
                                charx = 0;
                            }
                        }
                        sr.Close();
                        pictureBox1.Image = bmp;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.Filter = "MCM files (*.mcm)|*.mcm";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    Bitmap bmp = new Bitmap(pictureBox1.Image);
                    using (StreamWriter sw = new StreamWriter(myStream))
                    {
                        sw.Write("MAX7456\n");
                        int charx=8, chary=0, i =0;
                        while (chary < 11)
                        {
                            for (int y = 0; y < 18; y++)
                            {
                                for (int x = 0; x < 12; x++)
                                {
                                    Color pixel = bmp.GetPixel(charx * 12 + x, chary * 18 + y);
                                    if (pixel.R == 255 && pixel.G==255)
                                    {
                                        sw.Write("10"); //white
                                    }
                                    else if (pixel.R == 0 && pixel.G == 0)
                                    {
                                        sw.Write("00"); //black
                                    }
                                    else
                                    {
                                        sw.Write("01"); //transparent
                                    }
                                    i++;
                                    if (i== 4)
                                    {
                                        i = 0;
                                        sw.Write("\n");
                                    }
                                }
                            }
                            for (int x=0;x<40;x++)
                            {
                                sw.Write("01");
                                i++;
                                if (i == 4)
                                {
                                    i = 0;
                                    sw.Write("\n");
                                }
                            }
                            charx++;
                            if (charx==24)
                            {
                                charx = 0;
                                chary++;
                            }
                        }
                        sw.Close();
                    }
                    myStream.Close();
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

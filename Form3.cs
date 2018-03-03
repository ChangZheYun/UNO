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

namespace uno
{
    public partial class Form3 : Form
    {
        private int index = 2;
        private string sticker_photo;
        public Form3()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.SetBounds(Screen.PrimaryScreen.Bounds.Width / 2 - 150, Screen.PrimaryScreen.Bounds.Height / 2 - 170, 350, 370);
            this.FormBorderStyle = FormBorderStyle.None;
        }


        private void Form3_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("../../Resources/people/people1.jpg");
            sticker_photo = "../../Resources/people/people1.jpg";
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            textBox1.Text = "輸入名稱";
            textBox1.ForeColor = Color.FromName("Gray");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("../../Resources/people/people"+index.ToString()+".jpg");
            sticker_photo = "../../Resources/people/people" + index.ToString() + ".jpg";
            index++;
            if (index > 9)
            {
                index = 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.Filter = "JPG圖片|*.JPG|JPEG圖片|*.JPEG|PNG圖片|*.PNG|GIF圖片|*.GIF"; //名稱|*.檔案名
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true; //讓每次開起的路徑固定

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        sticker_photo = openFileDialog1.FileName;
                        pictureBox1.Image = Image.FromFile(sticker_photo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("路徑錯誤. Original error: " + ex.Message);
                }
            }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text ="";
            textBox1.ForeColor = Color.FromName("Black");
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                if (Encoding.GetEncoding("Big5").GetByteCount(textBox1.Text) == 0) //將字元轉成Byte型態計算，避免中文字算1字元
                {
                    MessageBox.Show("名稱長度不得為0", "警告");
                }
                else if (Encoding.GetEncoding("Big5").GetByteCount(textBox1.Text) > 10)
                {
                    MessageBox.Show("名稱長度不得超過10", "警告");
                }
                else
                {
                    this.Close();
                }
            }
        }

        public string get_photo()
        {
            return sticker_photo;
        }

        public string get_name()
        {
            return textBox1.Text;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uno
{
    public partial class Form1 : Form
    {
        private static int order = 0;  //一開始都是玩家先
        private int[] update_screen = new int[4] { 7, 7, 7, 7 };
        private static int direction = 1; //1逆時針 ， 2順時針
        private int previous_direction = 1;
        private int previous_order = 0;
        private PictureBox[] photo = new PictureBox[4];
        private Label[] name = new Label[4];
        Player play = new Player();
        Computer AI = new Computer();

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.SetBounds(Screen.PrimaryScreen.Bounds.Width/2-600, Screen.PrimaryScreen.Bounds.Height / 2-380, 1200, 750);
            this.FormBorderStyle = FormBorderStyle.None;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            set_log_in();            //登錄
            Game_Start();
        }

        public int set_order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
                if (order > 3)
                {
                    order = 0;
                }
                if (order < 0)
                {
                    order = 3;
                }
            }
        }

        public int set_direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        private void Game_Start()
        {
            pictureBox1.Left = 850;
            pictureBox1.Top = 270;
            pictureBox2.Left = 515;
            pictureBox2.Top = 415;
            pictureBox3.Left = 438;
            pictureBox3.Top = 490;
            pictureBox4.Left = -500;
            pictureBox5.Left = -500;
            pictureBox6.Left = -500;

            Card.arrange();
            for (int i = 0; i < 110; i++)
            {
                this.Controls.Add(Card.pattern[i]);
                Card.pattern[i].BringToFront();
            }
            timer1.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e) //抽牌堆
        {
            if (set_order == 0) 
            {
                play.pump_card();
                check_pattern(); //檢查是不是還能出牌
            }
        }

        private void check_pattern()
        {
            int num;
            for (num = 0; num < Card.hand_count()[0].Count; num++)
            {
                if (Card.pattern[num + 1].Top < 540)
                {
                    break;
                }
            }
            if (num == Card.hand_count()[0].Count)
            {
                next();
            }
        }

        public void next()
        {
            play.reset_status();
            if (set_direction == 1)  //換下一位玩家
            {
                set_order++;
            }
            else
            {
                set_order--;
            }

            if (set_order != 0) 
            {
                for (int i = 0; i < 50; i++)
                {
                    Random delay = new Random();
                    System.Threading.Thread.Sleep(delay.Next(1,50));
                    Application.DoEvents();
                }

                AI.put(1);
            }
            else
            {
                Card.smart_select();
                Card.set(1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)  //判斷當有人出牌/加牌時，呼叫更新檯面
        {
            if (set_direction != previous_direction && set_direction == 2)
            {
                pictureBox2.Image = Image.FromFile("../../Resources/119.png");
                previous_direction = 2;
            }
            else if (set_direction != previous_direction && set_direction == 1)
            {
                pictureBox2.Image = Image.FromFile("../../Resources/118.png");
                previous_direction = 1;
            }

            if (set_order != previous_order)
            {
                switch(set_order)
                {
                    case 0:
                        pictureBox3.Left = 438;
                        pictureBox3.Top = 490;
                        pictureBox4.Left = -500;
                        pictureBox5.Left = -500;
                        pictureBox6.Left = -500;
                        break;
                    case 1:
                        pictureBox5.Left = 960;
                        pictureBox5.Top = 120;
                        pictureBox3.Left = -500;
                        pictureBox4.Left = -500;
                        pictureBox6.Left = -500;
                        break;
                    case 2:
                        pictureBox4.Left = 280;
                        pictureBox4.Top = 25;
                        pictureBox3.Left = -500;
                        pictureBox5.Left = -500;
                        pictureBox6.Left = -500;
                        break;
                    case 3:
                        pictureBox6.Left = 0;
                        pictureBox6.Top = 225;
                        pictureBox3.Left = -500;
                        pictureBox4.Left = -500;
                        pictureBox5.Left = -500;
                        break;
                }
                previous_order = set_order;
            }


            for (int i = 0; i < 4; i++)
            {
                if (Card.hand_count()[i].Count == 0)
                {
                    timer1.Stop();
                    var result = MessageBox.Show(name[i].Text + "獲勝", "獲勝");
                    if (result==DialogResult.OK)
                    {
                        if(DialogResult.OK==MessageBox.Show("是否再進行一場遊戲?" , "提示",MessageBoxButtons.OKCancel))
                        {
                            開新遊戲ToolStripMenuItem_Click(sender, e);
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                }
            }
        }

        private void set_log_in()
        {
            Form3 log_in = new Form3();
            log_in.ShowDialog();
            log_in.Close();
            string sticker_photo = log_in.get_photo();

            for (int i = 0; i < 4; i++)
            {
                photo[i] = new PictureBox();
                photo[i].Size = new Size(100, 100);
                photo[i].SizeMode = PictureBoxSizeMode.StretchImage;
                photo[i].BringToFront();
                this.Controls.Add(photo[i]);
                name[i] = new Label();
                name[i].BackColor = Color.Transparent;
                name[i].Font = new Font("微軟正黑體", 12);
                name[i].ForeColor = Color.White;
                this.Controls.Add(name[i]);
            }
            photo[0].Image = Image.FromFile(sticker_photo);
            photo[0].Location = new Point(230, 580);
            photo[0].Name = sticker_photo;
            for (int i = 1; i < 4; i++)
            {
                while (true)
                {
                    Random number = new Random();
                    string file_name = "../../Resources/people/people" + number.Next(1, 10).ToString() + ".jpg";

                    int j;
                    for (j = 0; j < i; j++)
                    {
                        if (file_name == photo[j].Name) 
                            break;
                    }
                    if (j == i)
                    {
                        photo[i].Image = Image.FromFile(file_name);
                        photo[i].Name = file_name;
                        break;
                    }
                }
            }
            photo[1].Location = new Point(1050, 580);
            photo[2].Location = new Point(850, 45);
            photo[3].Location = new Point(40, 45);

            name[0].Text = log_in.get_name();
            if (Encoding.GetEncoding("Big5").GetByteCount(name[0].Text) != name[0].Text.Length)  //名字有中文(讓文字可置中)
            {
                name[0].Left = 275 - (Encoding.GetEncoding("Big5").GetByteCount(name[0].Text) / 2 * 7);
            }
            else   //名字都是英文
            {
                name[0].Left = 275 - (Encoding.GetEncoding("Big5").GetByteCount(name[0].Text) / 2 * 9);
            }
            name[0].Top = 685;
            name[1].Text = "Watson";
            name[1].Location = new Point(1070,685);
            name[2].Text = "Cohen";
            name[2].Location = new Point(870, 150);
            name[3].Text = "Quinn";
            name[3].Location = new Point(60, 150);
        }


        private void 開新遊戲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            for (int i = 0; i < 110; i++)
            {
                Card.pattern[i].Left = -500;
            }
            order = 0;  
            update_screen = new int[4] { 7, 7, 7, 7 };
            direction = 1;
            previous_direction = 1;
            previous_order = 0;
            pictureBox2.Image = Image.FromFile("../../Resources/118.png");
            Card.card_clear();
            play.player_clear();
            play = new Player();
            AI = new Computer();

            Game_Start(); //有bug
        }

        private void 重新登錄ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void 結束遊戲ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override CreateParams CreateParams
        {
            get
            {

                CreateParams cp = base.CreateParams;

                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED  

                if (this.IsXpOr2003 == true)
                {
                    cp.ExStyle |= 0x00080000;  // Turn on WS_EX_LAYERED
                    this.Opacity = 1;
                }

                return cp;

            }

        }  //防止閃爍

        private Boolean IsXpOr2003
        {
            get
            {
                OperatingSystem os = Environment.OSVersion;
                Version vs = os.Version;

                if (os.Platform == PlatformID.Win32NT)
                    if ((vs.Major == 5) && (vs.Minor != 0))
                        return true;
                    else
                        return false;
                else
                    return false;
            }
        }
    }
}


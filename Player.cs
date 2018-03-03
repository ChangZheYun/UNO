using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;

namespace uno
{
    class Player
    {
        private static bool check_pump=false, check_motion=false;   //玩家有無抽牌，有無出牌

        public void reset_status()
        {
            check_pump = false;
            check_motion = false;
        }

        public bool motion_status
        {
            get
            {
                return check_motion;
            }
            set
            {
                check_motion = value;
            }
        }

        public void pump_card()
        {
            if (!check_pump && !check_motion)
            {
                check_pump = true;
                Card.send(2);
            }
        }

        public void picturebox_click(object sender,EventArgs e)
        {
            PictureBox temp = (PictureBox)sender;
            Form1 player_order = new Form1();

            if (!check_motion && temp.Top < 540 && player_order.set_order == 0)   
            {
                check_motion = true;

                temp.Top = 280;
                temp.Left = 550;
            //    temp.BringToFront();

                int image_name = Convert.ToInt32(temp.Name);  //將檔案名稱轉為int
                Card.change_card(image_name);  //0為使用者(player0)，後為檔案名稱
                player_order.next();
            }
        }

        public void player_clear()
        {
            check_pump = false;
            check_motion = false;
        }
    }
}

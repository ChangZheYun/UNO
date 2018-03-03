using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace uno
{
    static class Card
    {
        private static List<int>[] brand = new List<int>[3];         //0:沒抽過的，1:出過的，2:手牌
        private static List<int>[] hand_card = new List<int>[4];     //4人的手牌資訊
        private static List<int>[] possible = new List<int>[18];      //存取AI可以用的牌
        public static  PictureBox[] pattern = new PictureBox[110];   //圖案
        private static bool[] have_pump = new bool[4];               //是否有抽過牌(抽過後只能出抽的那張)
        private static Random random_number = new Random();
        private static Player user = new Player();
        private static Form1 present = new Form1();

        static Card()
        {
            //new list 避免null(要有括號)
            begin_set();
        }

        public static List<int>[] hand_count()
        {
            return hand_card;
        }

    /*    public static bool[] pump_status()
        {
            return have_pump;
        }*/

        public static List<int>[] possible_card
        {
            get
            {
                return possible;
            }
            set
            {
                possible = value;
            }
        }

        public static void begin_set()
        {
            for (int i = 0; i < 3; i++)
            {
                brand[i] = new List<int>();
            }
            for (int i = 0; i < 4; i++)
            {
                hand_card[i] = new List<int>();
                have_pump[i] = new bool();
                have_pump[i] = false;
            }
            for (int i = 0; i < 18; i++)
            {
                possible[i] = new List<int>();
            }
            for (int i = 0; i < 110; i++)
            {
                pattern[i] = new PictureBox();
                pattern[i].Left = -500;
            }
        }

        public static void arrange()  //洗牌
        {
            //新增0~107的數字(不是手牌且不是場面第一張牌)進沒抽過的牌堆
            for (int i = 0; i < 108; i++) 
            {
                int j;
                for (j = 0; j < brand[2].Count; j++) 
                {
                    if (brand[2][j] == i || brand[1][brand[1].Count() - 1] == i) 
                    {
                        break;
                    }
                }
                if (j == brand[2].Count ||  brand[2].Count == 0)   //當手牌量為0(初始)時，仍須發牌
                {
                    brand[0].Add(i);
                }
            }

            //依序走訪取亂數，和亂數後的value交換
            for (int i = 0; i < brand[0].Count; i++) 
            {
                int number = random_number.Next(0, brand[0].Count);
                int temp = brand[0][i];
                brand[0][i] = brand[0][number];
                brand[0][number] = temp;
            }

            if (brand[2].Count == 0) //初始
            {
                begin();
            }
        }

        public static void begin()   //一開始的第一張牌
        {
            //第一張牌不為黑色牌
            int check_first_card;
            for (check_first_card = 0; check_first_card < brand[0].Count; check_first_card++) 
            {
                if (brand[0][check_first_card] < 99)
                {
                    break;
                }
            }
            brand[1].Add(brand[0][check_first_card]);
            brand[0].RemoveAt(check_first_card);
            send(1);
        }

        public static void send(int status)  //發牌
        {
            if (status == 1)  //初始
            {
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        hand_card[i].Add(brand[0][0]); //取牌庫第一張
                        brand[2].Add(brand[0][0]);
                        brand[0].RemoveAt(0);
                    }
                    hand_card[i].Sort();               //照順序整理牌
                }
            }
            else if (status == 2)  //抽一張牌
            {
                have_pump[present.set_order] = true;
                check_card();  //檢查牌庫的牌是否還足夠
                hand_card[present.set_order].Add(brand[0][0]);
                brand[2].Add(brand[0][0]);
                brand[0].RemoveAt(0);
                hand_card[present.set_order].Sort();
            }
            set(1);
        }

        public static void change_card(int image_name)
        {
            int self = present.set_order;  //自己
            int next;                      //要加牌的對象

            if (present.set_direction == 1)            //逆時針
            {
                next = present.set_order + 1;
                if (next > 3)
                {
                    next = 0;
                }
            }
            else                           //順時針
            {
                next = present.set_order - 1;
                if (next < 0) 
                {
                    next = 3;
                }
            }

            brand[1].Add(image_name);
            for (int i = 0; i < brand[2].Count; i++) //出牌後，刪除brand[2]內這張牌的資訊
            {
                if (brand[2][i] == image_name)
                {
                    brand[2].RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < hand_card[self].Count; i++) //刪除玩家的這張手牌資訊
            {
                if (hand_card[self][i] == image_name)
                {
                    hand_card[self].RemoveAt(i);
                    hand_card[self].Sort();

                    if (image_name % 25 == 19 || image_name % 25 == 20) //加兩張牌
                    {
                        for (int repeat = 0; repeat < 2; repeat++)
                        {
                            hand_card[next].Add(brand[0][0]);
                            brand[2].Add(brand[0][0]);
                            brand[0].RemoveAt(0);
                        }
                        hand_card[next].Sort();
                        if (present.set_direction == 1)  //跳過下一位玩家
                        {
                            present.set_order++;
                        }
                        else
                        {
                            present.set_order--;
                        }
                    }
                    else if (image_name % 25 == 21 || image_name % 25 == 22) //迴轉
                    {
                        if (present.set_direction == 1)
                        {
                            present.set_direction = 2;
                        }
                        else
                        {
                            present.set_direction = 1;
                        }
                    }
                    else if (image_name % 25 == 23 || image_name % 25 == 24) //禁止
                    {
                        if (present.set_direction == 1)
                        {
                            present.set_order++;
                        }
                        else
                        {
                            present.set_order--;
                        }
                    }
                    else if (image_name > 99 && self == 0)        //換顏色牌
                    {
                        Form2 color = new Form2();
                        if (hand_card[0].Count - 1 != 0)
                        {
                            color.ShowDialog();         //開啟選擇顏色模式
                            color.Close();
                        }
                        String choose = color.Button_Status();
                        if (image_name < 104)  //無加4
                        {
                            switch (choose)
                            {
                                case "button1":
                                    brand[1].Add(110); //紅
                                    break;
                                case "button2":
                                    brand[1].Add(111); //黃
                                    break;
                                case "button3":
                                    brand[1].Add(109); //綠
                                    break;
                                default:
                                    brand[1].Add(108); //藍
                                    break;
                            }
                        }
                        else                   //有加4
                        {
                            switch (choose)
                            {
                                case "button1":
                                    brand[1].Add(114); //紅
                                    break;
                                case "button2":
                                    brand[1].Add(115); //黃
                                    break;
                                case "button3":
                                    brand[1].Add(113); //綠
                                    break;
                                default:
                                    brand[1].Add(112); //藍
                                    break;
                            }
                            for (int repeat = 0; repeat < 4; repeat++) //抽四張牌
                            {
                                hand_card[next].Add(brand[0][0]);
                                brand[2].Add(brand[0][0]);
                                brand[0].RemoveAt(0);
                            }
                            hand_card[next].Sort();

                            if (present.set_direction == 1)  //跳過下一位玩家
                            {
                                present.set_order++;
                            }
                            else
                            {
                                present.set_order--;
                            }
                        }
                   //     color.Close();
                    }
                    else if (image_name > 99)  //AI選顏色
                    {
                        Random num = new Random();
                        int[] each_color = new int[4];
                        int max_color = num.Next(0,4);  //最多的顏色

                        for (int j = 0; j < hand_card[present.set_order].Count; j++)
                        {
                            if (hand_card[present.set_order][j] < 100)
                            {
                                each_color[hand_card[present.set_order][j] / 25]++;
                            }
                        }
                        int max_number = each_color[max_color];//最多的顏色數量
                        for (int j = 0; j < 4; j++)
                        {
                            if (each_color[j] > max_number)
                            {
                                max_number = each_color[j];
                                max_color = j;
                            }
                        }

                        if (image_name < 104)
                        {
                            switch (max_color)
                            {
                                case 0:
                                    brand[1].Add(108); //藍
                                    break;
                                case 1:
                                    brand[1].Add(109); //綠
                                    break;
                                case 2:
                                    brand[1].Add(110); //紅
                                    break;
                                default:
                                    brand[1].Add(111); //黃
                                    break;
                            }
                        }
                        else
                        {
                            switch (max_color)
                            {
                                case 0:
                                    brand[1].Add(112); //藍
                                    break;
                                case 1:
                                    brand[1].Add(113); //綠
                                    break;
                                case 2:
                                    brand[1].Add(114); //紅
                                    break;
                                default:
                                    brand[1].Add(115); //黃
                                    break;
                            }
                            for (int repeat = 0; repeat < 4; repeat++) //抽四張牌
                            {
                                hand_card[next].Add(brand[0][0]);
                                brand[2].Add(brand[0][0]);
                                brand[0].RemoveAt(0);
                            }
                            hand_card[next].Sort();

                            if (present.set_direction == 1)  //跳過下一位玩家
                            {
                                present.set_order++;
                            }
                            else
                            {
                                present.set_order--;
                            }
                        }
                    }
                    break;
                }
            }
            set(2);
        }

        public static int set(int have_choose)  //設定牌組(顯示於畫面)
        {
            delete();
            //中間那張牌
            pattern[0].SizeMode = PictureBoxSizeMode.StretchImage;
            pattern[0].BackColor = Color.Transparent;
            pattern[0].BorderStyle = BorderStyle.FixedSingle;
            pattern[0].Size = new Size(110, 160);
            pattern[0].Location = new Point(550, 270);
            pattern[0].Image = Image.FromFile("../../Resources/" + brand[1][brand[1].Count-1].ToString() + ".png");
            pattern[0].Name = brand[1][brand[1].Count - 1].ToString();

            int picture_number = 1;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < hand_card[i].Count; j++)
                {
                    pattern[picture_number].SizeMode = PictureBoxSizeMode.StretchImage;
                    pattern[picture_number].BackColor = Color.Transparent;
                    pattern[picture_number].BorderStyle = BorderStyle.FixedSingle;
                    pattern[picture_number].Name = hand_card[i][j].ToString();
                    pattern[picture_number].BringToFront();

                    if (i == 0)
                    {
                        int width = 680 - (((hand_card[0].Count-1) * 32 + 100) / 2);
                        if (width < 340)
                        {
                            width = 340;
                        }
                        pattern[picture_number].Size = new Size(100, 150);
                        pattern[picture_number].Location = new Point(width + j * 32, 540);
                        pattern[picture_number].Image = Image.FromFile("../../Resources/" + hand_card[i][j].ToString() + ".png");
                        pattern[picture_number].Click += new EventHandler(user.picturebox_click);  //將使用者的牌加入事件中
                    }
                    else if (i == 1)
                    {
                        int width = 215 + (((hand_card[1].Count - 1) * 32 + 100) / 2);
                        if (width > 470)
                        {
                            width = 470;
                        }
                        pattern[picture_number].Size = new Size(150, 100);
                        pattern[picture_number].Location = new Point(1010, width - j * 32);
                        pattern[picture_number].Image = Image.FromFile("../../Resources/116.png");
                    //    pattern[picture_number].Image = Image.FromFile("../../Resources/" + hand_card[i][j].ToString() + ".png");
                        pattern[picture_number].Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        pattern[picture_number].Refresh();
                    }
                    else if (i == 2)
                    {
                        int width = 520 - (((hand_card[2].Count - 1) * 32 + 100) / 2);
                        if (width < 200) 
                        {
                            width = 200;
                        }
                        pattern[picture_number].Size = new Size(100, 150);
                        pattern[picture_number].Location = new Point(width + j * 32, 45);
                        pattern[picture_number].Image = Image.FromFile("../../Resources/116.png");
                  //      pattern[picture_number].Image = Image.FromFile("../../Resources/" + hand_card[i][j].ToString() + ".png");
                        pattern[picture_number].Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        pattern[picture_number].Refresh();
                    }
                    else if (i == 3)
                    {
                        int width =  440 - (((hand_card[3].Count - 1) * 32 + 100) / 2);
                        if (width < 180)
                        {
                            width = 180;
                        }
                        pattern[picture_number].Size = new Size(150, 100);
                        pattern[picture_number].Location = new Point(20, width + j * 32);
                        pattern[picture_number].Image = Image.FromFile("../../Resources/116.png");
                   //     pattern[picture_number].Image = Image.FromFile("../../Resources/" + hand_card[i][j].ToString() + ".png");
                        pattern[picture_number].Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        pattern[picture_number].Refresh();
                    }
                    picture_number++;
                }
            }
            if (have_choose == 1)
            {
                smart_select();
            }
            for (int j = picture_number; j < 110; j++)
            {
                pattern[j].Left = -500;
            }
            return picture_number; //回傳所有牌的數量
        }

        public static void smart_select()  //智慧選牌
        {
            int order = present.set_order;

            if (order == 0) //避免輪到下一位仍出現選牌，若抽過牌，只能出抽的那張
            {
                for (int j = 0; j < hand_card[0].Count; j++)
                {
                    if (hand_card[0][j] > 99 && (!have_pump[0] || (have_pump[0] && brand[2][brand[2].Count - 1] == hand_card[0][j])))
                    {
                        pattern[j + 1].Top -= 20;
                    }
                    else if (brand[1][brand[1].Count - 1] > 107)
                    {
                        int value = 0;
                        if (brand[1][brand[1].Count - 1] < 112)
                        {
                            value = (brand[1][brand[1].Count - 1] - 108) * 25;
                        }
                        else if (brand[1][brand[1].Count - 1] < 116)
                        {
                            value = (brand[1][brand[1].Count - 1] - 112) * 25;
                        }
                        for (int k = value; k < value + 25; k++)
                        {
                            if (hand_card[0][j] == k && (!have_pump[0] || (have_pump[0] && hand_card[0][j] == brand[2][brand[2].Count - 1])))
                            {
                                pattern[j + 1].Top -= 20;
                                break;
                            }
                        }
                    }
                    else if ((brand[1][brand[1].Count - 1] / 25 == hand_card[0][j] / 25       //同色
                        || (brand[1][brand[1].Count - 1] % 25 == hand_card[0][j] % 25)        //同數字
                        || (brand[1][brand[1].Count - 1] % 25 % 2 == 0 && brand[1][brand[1].Count - 1] % 25 == (hand_card[0][j] + 1) % 25 && brand[1][brand[1].Count - 1] % 25 != 0)
                        || (brand[1][brand[1].Count - 1] % 25 % 2 == 1 && brand[1][brand[1].Count - 1] % 25 == (hand_card[0][j] - 1) % 25)) //同數字  
                        && (!have_pump[0] || (have_pump[0] && hand_card[0][j] == brand[2][brand[2].Count - 1])))
                    {
                        pattern[j + 1].Top -= 20;    //0是場面上的牌
                    }
                }
            }
            else if (order > 0)
            {
                for (int j = 0; j < hand_card[order].Count; j++)
                {
                    /*   bool check = false;   //讓possible內不要重複
                       for (int k = 0; k < possible[hand_card[order][j] / 25].Count; k++)
                       {
                           if (hand_card[order][j] == possible[hand_card[order][j] / 25][k])
                           {
                               check = true;
                               break;
                           }
                       }*/

                    /*    if (!check)
                        {*/
                    if (hand_card[order][j] > 99 && (!have_pump[order] || (have_pump[order] && brand[2][brand[2].Count - 1] == hand_card[order][j])))    //若為功能牌，則不該判其他顏色
                    {
                        if (hand_card[order][j] > 104)
                        {
                            possible[17].Add(hand_card[order][j]); //黑+4
                        }
                        else
                        {
                            possible[16].Add(hand_card[order][j]); //黑換色
                        }
                    }
                    else if (brand[1][brand[1].Count - 1] > 107)
                    {
                        int value = 0;
                        if (brand[1][brand[1].Count - 1] > 107 && brand[1][brand[1].Count - 1] < 112) 
                        {
                            value = (brand[1][brand[1].Count - 1] - 108) * 25;
                        }
                        else if (brand[1][brand[1].Count - 1] > 111 && brand[1][brand[1].Count - 1] < 116)
                        {
                            value = (brand[1][brand[1].Count - 1] - 112) * 25;
                        }

                        for (int k = value; k < value + 25; k++)
                        {
                            if (hand_card[order][j] == k && (!have_pump[order] || (have_pump[order] && hand_card[order][j] == brand[2][brand[2].Count - 1])))
                            {
                                switch (hand_card[order][j] % 25)
                                {
                                    case 19: //+2
                                    case 20:
                                        possible[hand_card[order][j] / 25 * 4 + 1].Add(hand_card[order][j]);
                                        break;
                                    case 21: //迴轉
                                    case 22:
                                        possible[hand_card[order][j] / 25 * 4 + 2].Add(hand_card[order][j]);
                                        break;
                                    case 23: //禁止
                                    case 24:
                                        possible[hand_card[order][j] / 25 * 4 + 3].Add(hand_card[order][j]);
                                        break;
                                    default:
                                        possible[hand_card[order][j] / 25 * 4].Add(hand_card[order][j]);
                                        break;
                                }
                                break;
                            }
                        }
                    }
                    else if ((brand[1][brand[1].Count - 1] / 25 == hand_card[order][j] / 25       //同色
                        || (brand[1][brand[1].Count - 1] % 25 == hand_card[order][j] % 25)
                        || (brand[1][brand[1].Count - 1] % 25 % 2 == 0 && brand[1][brand[1].Count - 1] % 25 == (hand_card[order][j] + 1) % 25 && brand[1][brand[1].Count - 1] % 25 != 0)
                        || (brand[1][brand[1].Count - 1] % 25 % 2 == 1 && brand[1][brand[1].Count - 1] % 25 == (hand_card[order][j] - 1) % 25))
                        && (!have_pump[order] || (have_pump[order] && hand_card[order][j] == brand[2][brand[2].Count - 1])))  //同數字
                    {
                        switch (hand_card[order][j] % 25)
                        {
                            case 19: //+2
                            case 20:
                                possible[hand_card[order][j] / 25 * 4 + 1].Add(hand_card[order][j]);
                                break;
                            case 21: //迴轉
                            case 22:
                                possible[hand_card[order][j] / 25 * 4 + 2].Add(hand_card[order][j]);
                                break;
                            case 23: //禁止
                            case 24:
                                possible[hand_card[order][j] / 25 * 4 + 3].Add(hand_card[order][j]);
                                break;
                            default:
                                possible[hand_card[order][j] / 25 * 4].Add(hand_card[order][j]);
                                break;
                        }
                        //   pattern[hand_card[0].Count + j + 1].Left -= 20;    //0是場面上的牌
                    }
                //     }
                }
            }
            for (int j = 0; j < 4; j++)
            {
                have_pump[j] = false; //還原
            }
        }

        public static void delete()
        {
            for (int i = 0; i < 110; i++)
            {
                pattern[i].Click -= new EventHandler(user.picturebox_click);
            }
        }

        public static void check_card()
        {
            if (brand[0].Count < 10)
            {
                for (int i = 0; i < brand[1].Count - 1; i++)  //場面上第一張牌(brand[1]最後一張)不回收
                {
               //     brand[0].Add(brand[1][i]);
                    brand[1].Remove(i);
                }
                arrange();
            }
        }

        public static void card_clear()
        {
            brand = new List<int>[3];         //0:沒抽過的，1:出過的，2:手牌
            hand_card = new List<int>[4];     //4人的手牌資訊
            possible = new List<int>[18];      //存取AI可以用的牌
            pattern = new PictureBox[110];   //圖案
            have_pump = new bool[4];               //是否有抽過牌(抽過後只能出抽的那張)
            random_number = new Random();
            user = new Player();
            present = new Form1();
            begin_set();
        }
    }
}

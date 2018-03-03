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
    class Computer
    {
        private List<int>[] select = new List<int>[18];
        private int sum = 0;

        public void put(int frequency)
        {
            sum = 0;
            Form1 test = new Form1();
            int a=test.set_order;

            if (test.set_order != 0)
            {
                Card.smart_select(); //選出能出牌給possible
                for (int i = 0; i < 18; i++)
                {
                    select[i] = new List<int>();
                    select[i] = Card.possible_card[i];  //將可以出的牌給select
                    sum += select[i].Count;
                }

                if (frequency == 2 && sum == 0)
                {
                    next_people();
                }
                else
                {
                    choose(frequency);
                }
            }
        }

        public void choose(int frequency)
        {
            if (sum == 0)      //若都沒牌，則抽牌
            {
                Card.send(2);
                put(2);         //抽牌後再判定一次
            }
            else
            {
                Form1 set = new Form1();
                int next_player;
                int color_sum = 0, feature_sum = 0;

                if (set.set_direction == 1)
                {
                    next_player = set.set_order + 1;
                    if (next_player > 3)
                    {
                        next_player = 0;
                    }
                }
                else
                {
                    next_player = set.set_order - 1;
                    if (next_player < 0)
                    {
                        next_player = 3;
                    }
                }

                Random num = new Random();
                for (int i = 0; i < 16; i++)
                {
                    if (i % 4 > 0)
                    {
                        feature_sum += select[i].Count;
                    }
                    else
                    {
                        color_sum += select[i].Count;
                    }
                }

                if (Card.hand_count()[next_player].Count < 4 && (feature_sum != 0 || select[17].Count != 0))  //下一位玩家剩三張且自己有功能牌
                {
                    while (true)
                    {
                        int number = num.Next(0, 16);
                        if (number % 4 > 0 && select[number].Count != 0)  //有功能牌就出
                        {
                            Card.change_card(select[number][0]);
                            break;
                        }
                        else if (select[17].Count != 0)
                        {
                            Card.change_card(select[17][0]);
                            break;
                        }
                    }
                }
                else if (color_sum == 0 && feature_sum == 0)  //只剩黑色牌
                {
                    if (select[16].Count != 0)
                    {
                        Card.change_card(select[16][0]);
                    }
                    else  
                    {
                        Card.change_card(select[17][0]);
                    }
                }
                else if (feature_sum - color_sum == feature_sum)  //只剩有顏色的功能牌(或許有黑牌)
                {
                    List<int> i = new List<int>();
                    for (int j = 0; j < 16; j++)
                    {
                        if (j % 4 > 0 && select[j].Count != 0)  //有功能牌就出
                        {
                            i.Add(j);
                        }
                    }
                    int number = num.Next(0, i.Count);
                    Card.change_card(select[i[number]][0]);
                }
                else if (color_sum != 0) 
                {
                    //此處為混雜數字牌+顏色功能牌+黑色牌，只出數字牌(不出功能牌)

                    int[] species_color = new int[4];
                    int[] species_number = new int[4];
                    int species = 0;
                    for (int i = 0; i < 16; i+=4)
                    {
                        if (select[i].Count != 0)
                        {
                            species_color[i / 4]++;
                            species++;
                        }
                    }
                    if (species == 1)  //只有一種顏色牌能出
                    {
                        int row;
                        for (row = 0; row < 4; row++)
                        {
                            if (species_color[row] != 0)
                            {
                                break;
                            }
                        }
                        Card.change_card(select[row * 4][num.Next(0, select[row * 4].Count)]);
                    }
                    else               //有多種顏色的牌能出，出最多數量的
                    {
                        int a = Card.hand_count()[set.set_order].Count;

                        for (int i = 0; i < Card.hand_count()[set.set_order].Count; i++)
                        {
                            if (Card.hand_count()[set.set_order][i] < 100 && species_color[Card.hand_count()[set.set_order][i] / 25] != 0 ) 
                            {
                                species_number[Card.hand_count()[set.set_order][i] / 25]++; //手牌中能出顏色的數量
                            }
                        }

                        int max_number = 0, max_color = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            if (species_number[i] > max_number)
                            {
                                max_number = species_number[i];
                                max_color = i;
                            }
                        }
                        Card.change_card(select[max_color * 4][num.Next(0, select[max_color * 4].Count)]);
                    }
                }
             /*   int i;
                for (i = 0; i<18; i++)
                {
                    if (select[i].Count != 0)
                        break;
                }
                Card.change_card(select[i][0]);*/
            }

            if (sum != 0) 
            {
                next_people();
            }
            //  set.set_order++;//只有第一次的判定才可以加order
        }

        public void next_people()
        {
            Form1 set = new Form1();

            for (int k = 0; k < 18; k++)
            {
                Card.possible_card[k].Clear();
                select[k].Clear();
            }
            sum = 0;
            set.next();
        }
    }
}

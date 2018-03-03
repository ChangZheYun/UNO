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
    public partial class Form2 : Form
    {
        Button[] option = new Button[4];
        String name="";

        public Form2()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.SetBounds(Screen.PrimaryScreen.Bounds.Width / 2 - 160, Screen.PrimaryScreen.Bounds.Height / 2 - 180, 320, 360);

            option = new Button[4] { button1, button2, button3, button4 };
            for (int i = 0; i < 4; i++)
            {
                option[i].Click += new EventHandler(Button_Click);
            }
        }

        public void Button_Click(object sender, EventArgs e)
        {
            Button temp = (Button)sender;
            name = temp.Name.ToString();
            this.Close();  //只要一選擇後就關閉
        }

        public String Button_Status()
        {
            return name;
        }
    }
}

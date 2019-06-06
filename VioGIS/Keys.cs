using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VioGIS
{
    public partial class Keys : Form
    {
        public TextBox lblM;
        public Keys()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            StringBuilder kValue = new StringBuilder();
            kValue.Length = 0;
            kValue.Append("");
            if (e.Modifiers != 0)
            {
                if (e.Control)
                    kValue.Append("Ctrl+ ");
                if (e.Alt)
                    kValue.Append("Alt+ ");
                if (e.Shift)
                    kValue.Append("Shift+ ");
            }
            if ((e.KeyValue >= 33 && e.KeyValue <= 40) || (e.KeyValue >= 65 && e.KeyValue <= 90) || (e.KeyValue >= 112 && e.KeyValue <= 123))//小键盘+a~z+f1~f12
            {
                kValue.Append(e.KeyCode);
            }
            else if (e.KeyValue >= 48 && e.KeyValue <= 57)//0~9
            {
                kValue.Append(e.KeyCode.ToString().Substring(1));
            }
            this.ActiveControl.Text = "";
            this.ActiveControl.Text = kValue.ToString();
        }
        private void keyUp(object sender, KeyEventArgs e)
        {
            string str = this.ActiveControl.Text.TrimEnd();
            int len = str.Length;
            if (len >= 1 && str.Substring(str.Length - 1) == "+")
            {
                this.ActiveControl.Text = "";
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown(sender, e);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(sender, e);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown(sender, e);
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            keyUp(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblM = new TextBox();
            lblM.Text = textBox1.Text;
            this.Close();
        }
    }
}

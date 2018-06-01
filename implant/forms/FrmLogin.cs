using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace implant
{
    public partial class FrmLogowanie : Form
    {
        public FrmLogowanie()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            startup();
        }

        void startup()
        {
            label2.Text = "";
            //Setting.get();
        }

        void login()
        {
            if (checkPassword())
            {
                Dispose();
            }
            else
            {
                textBox1.Clear();
                label2.Text = "Podane hasło jest nieprawidłowe!";
            }
        }

        bool checkPassword()
        {
            string password = textBox1.Text;
            bool valid = XMLService.checkPassword(password);

            return valid;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                login();
            }
        }
    }
}

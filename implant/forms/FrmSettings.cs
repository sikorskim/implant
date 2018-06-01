using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace implant
{
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();
            startup();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        void startup()
        {
            comboBox1.DataSource = RSAservice.getPossibleKeyLengths();
            //comboBox3.DataSource = MyKey.getKeyNames();
            numericUpDown2.Value = 32;

            textBox4.Enabled = false;
            textBox5.Enabled = false;
            //comboBox3.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            loadSettings();
        }

        void loadSettings()
        {
            setControls();
        }

        void saveSettings()
        {
            getControls();
            Setting.set();
            Dispose();
        }

        void setControls()
        {
            textBox5.Text = Setting.aesIv;
            textBox4.Text = Setting.salt;
            numericUpDown1.Value = (decimal)Setting.genPassLength;
            comboBox1.SelectedIndex = comboBox1.FindStringExact(Setting.rsaKeyLength.ToString());
            //checkBox8.Checked = (bool)Setting.autoSignMsg;

            //if ((bool)Setting.autoSignMsg)
            //{
            //    string keyName = Setting.autoSignName;
            // //   comboBox3.SelectedIndex = comboBox3.FindStringExact(keyName);
            // //   comboBox3.Enabled = true;
            //}
            //else
            //{
            //    comboBox3.SelectedIndex = -1;
            //}
        }

        void getControls()
        {
            Setting.aesIv = textBox5.Text;
            Setting.salt=textBox4.Text;
            Setting.genPassLength = (int)numericUpDown1.Value;
            Setting.rsaKeyLength = Int32.Parse(comboBox1.SelectedValue.ToString());

            //if (checkBox8.Checked && comboBox3.SelectedIndex!=-1)
            //{
            //    Setting.autoSignName = comboBox3.SelectedValue.ToString();
            //    Setting.autoSignMsg = true;
            //}
            //else
            //{
            //    Setting.autoSignName = string.Empty;
            //    Setting.autoSignMsg = false;
            //}
        }

        void unlockIvTxtBox()
        {
            if (checkBox2.Checked)
            {
                textBox5.Enabled = true;
                button1.Enabled = true;
                MessageBox.Show("Pamiętaj o przekazaniu parametru odbiorcy wiadomości, gdyż bez niego odszyfrowanie wiadomości będzie niemożliwe!", "Ostrzeżenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                textBox5.Enabled = false;
                button1.Enabled = false;
            }
        }

        void unlockSaltTxtBox()
        {
            if (checkBox1.Checked)
            {
                textBox4.Enabled = true;
                button2.Enabled = true;
                MessageBox.Show("Pamiętaj o przekazaniu parametru odbiorcy wiadomości, gdyż bez niego odszyfrowanie wiadomości będzie niemożliwe!", "Ostrzeżenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                textBox4.Enabled = false;
                button2.Enabled = false;
            }
        }

        //void unlockKeyNameComBox()
        //{
        //    if (checkBox8.Checked)
        //    {
        //        comboBox3.Enabled = true;
        //    }
        //    else
        //    {
        //        comboBox3.Enabled = false;
        //    }
        //}

        void generateString(int txtBox)
        {
            if (txtBox == 0)
            {
                textBox5.Text = PasswordGenerator.generate(16);
            }
            else if (txtBox == 1)
            {
                int length = (int)numericUpDown2.Value;
                textBox4.Text = PasswordGenerator.generate(length);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dispose();
        }
        
        private void FrmSettings_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            
            unlockIvTxtBox();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            unlockSaltTxtBox();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            //unlockKeyNameComBox();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            generateString(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            generateString(1);
        }
    }
}

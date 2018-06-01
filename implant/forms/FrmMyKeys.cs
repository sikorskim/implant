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
    public partial class FrmMyKeys : Form
    {
        public FrmMyKeys()
        {
            InitializeComponent();
            startup();
        }

        void startup()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            textBox3.ReadOnly = true;
            textBox3.ScrollBars = ScrollBars.Vertical;
            getKeyLength();
        }

        void getKeyLength()
        {
            //Setting.get();
            comboBox1.DataSource=RSAservice.getPossibleKeyLengths();
            comboBox1.SelectedIndex = comboBox1.FindStringExact(Setting.rsaKeyLength.ToString());
        }

        void generateKeys()
        {
            UseWaitCursor = true;
            string keyLength = comboBox1.SelectedValue.ToString();
            textBox3.Text=RSAservice.generateKeyPair(keyLength);
            UseWaitCursor = false;
        }

        void save()
        {
            MyKey.name = textBox1.Text;
            MyKey.privPubKey = textBox3.Text;
            MyKey.note = textBox2.Text;

            if (checkName())
            {
                MyKey.add();
                refreshDgv();
                Dispose();
            }
            else
            {
                MessageBox.Show("Nazwa "+MyKey.name+" już istnieje! Wybierz inną!", "Ostrzeżenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        bool checkName()
        {
            bool unique = MyKey.checkName(MyKey.name);
            return unique;
        }

        void refreshDgv()
        {
            FrmKeysManager frmKeysManager = (FrmKeysManager)this.Owner;
            frmKeysManager.refreshDgv();
        }

        void clear()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            generateKeys();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            save();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            clear();
        }
    }
}

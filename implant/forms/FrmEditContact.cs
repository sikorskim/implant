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
    public partial class FrmEditContact : Form
    {
        public FrmEditContact()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
        }

        void clear()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        void addContact()
        {
            bool validKey = checkRSAkey();
            bool validName = checkName();

            if (validKey && validName)
            {
                TrustedContact.name = textBox1.Text;
                TrustedContact.pubKey = textBox2.Text;
                TrustedContact.note = textBox3.Text;

                TrustedContact.add();

                refreshDgv();
                Dispose();
            }
        }

        bool checkRSAkey()
        {
            string key = textBox2.Text;
            bool valid = XMLService.checkRSAkey(key);

            if (!valid)
            {
                MessageBox.Show("Podany klucz jest nieprawidłowy!" + Environment.NewLine + "Dozwolone jest używanie wyłącznie kluczy wygenerowanych przez ninejszy program.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return valid;
        }

        bool checkName()
        {
            bool valid = true;
            string name = textBox1.Text;

            if ( name== string.Empty || name.Length <= 1)
            {
                valid = false;
                MessageBox.Show("Nazwa kontaktu jest wymgana!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!TrustedContact.checkName(name))
            {
                valid = false;
                MessageBox.Show("Nazwa " + MyKey.name + " już istnieje! Wybierz inną!", "Ostrzeżenie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return valid;
        }

        void refreshDgv()
        {
            FrmKeysManager frmKeysManager = (FrmKeysManager)this.Owner;
            frmKeysManager.refreshDgv();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addContact();
        }
    }
}

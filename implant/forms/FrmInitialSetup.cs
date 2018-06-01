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
    public partial class FrmInitialSetup : Form
    {
        public FrmInitialSetup()
        {
            InitializeComponent();
            startup();
        }

        int tabPageNumber = 0;

        void startup()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            hideTabNames();
            setTitleLabels();
            button7.Enabled = false;
            button6.Enabled = false;
            getKeyLength();
            label12.Text = "";
            label13.Text = "";
            label14.Text = "";
        }

        void getKeyLength()
        {
            comboBox1.DataSource = RSAservice.getPossibleKeyLengths();
            comboBox1.SelectedIndex = comboBox1.FindStringExact(Setting.rsaKeyLength.ToString());
            comboBox1.SelectedIndex = 1;
        }

        void setTitleLabels()
        {
            label9.Text = "Ustalanie klucza zabezpieczającego dostęp do praogramu.";
            label10.Text = "Ustalanie wartości początkowej i soli dla algorytmu AES.";
            label11.Text = "Tworzenie pary kluczy RSA do podpisywania wiadomości.";
            label8.Text = "Konfiguracja zakończona!" + Environment.NewLine + "Pamiętaj o bezpiecznym przechowywaniu klucza zabezpieczjącego, " + Environment.NewLine + "gdyż jego utrata uniemożliwi dostęp do programu." + Environment.NewLine;
        }

        void hideTabNames()
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
        }

        void generatePassword()
        {
            int charCount = (int)numericUpDown1.Value;
            string password = PasswordGenerator.generate(charCount);
            textBox1.Text = password;
            textBox2.Text = password;
            Clipboard.SetText(password);
        }

        void getData()
        {
            Setting.userPassword = HashService.compute(textBox1.Text, "SHA512");
            Setting.aesIv = textBox3.Text;
            Setting.salt = textBox4.Text;
            Setting.autoSignMsg = false;
            Setting.autoSignName = string.Empty;
            Setting.genPassLength = 32;
            Setting.rsaKeyLength = 2048;
            MyKey.name = textBox7.Text;
            MyKey.privPubKey = textBox5.Text;
            MyKey.note = textBox6.Text;
        }

        void saveSettings()
        {
            getData();
            XMLService.initialSetup();
            Setting.set();
            MyKey.add();

            Dispose();
        }

        void checkPassword()
        {
            bool success = true;

            if (textBox1.Text != textBox2.Text)
            {
                success = false;
                label12.Text = "Hasła niezgodne!";
            }
            else if (textBox1.TextLength < 8)
            {
                success = false;
                label12.Text = "Hasła powinno składać się z co najmniej 8 znaków!";
            }
            else
            {
                label12.Text = "";
                success = true;
            }

            button6.Enabled = success;
        }

        void checkInitialValueAndSalt()
        {
            bool success = true;

            if (textBox3.TextLength != 16 && textBox4.TextLength < 8)
            {
                success = false;
                label13.Text = "Wartość początkowa musi składać się z 16 znaków!" + Environment.NewLine + "Jest to wymagane ze względu na długość bloku w algorytmie AES," + Environment.NewLine + "która ma długość 128 bitów (1 znak to 8 bitów => 16 znaków * 8 bitów = 128 bitów)." + Environment.NewLine + "Sól powinna składać się co najmniej z 8 znaków!" + Environment.NewLine + "Dłuższa sól podnosi bezpieczeństwo szyfrowania.";
            }
            else if (textBox3.TextLength != 16)
            {
                success = false;
                label13.Text = "Wartość początkowa musi składać się z 16 znaków!" + Environment.NewLine + "Jest to wymagane ze względu na długość bloku w algorytmie AES," + Environment.NewLine + "która ma długość 128 bitów (1 znak to 8 bitów => 16 znaków * 8 bitów = 128 bitów).";
            }
            else if (textBox4.TextLength < 8)
            {
                success = false;
                label13.Text = "Sól powinna składać się co najmniej z 8 znaków!" + Environment.NewLine + "Dłuższa sól podnosi bezpieczeństwo szyfrowania.";
            }
            else
            {
                label13.Text = "";
            }

            button6.Enabled = success;
        }

        void checkRSAKey()
        {
            bool success = true;

            if (textBox7.TextLength < 1)
            {
                success = false;
                label14.Text = "Nazwa klucza RSA powinna składać się z co najmniej 1 znaku.";
            }
            else if (textBox5.TextLength == 0)
            {
                success = false;
                label14.Text = "Nie wygenerowano pary kluczy RSA.";
            }
            else
            {
                label14.Text = "";
            }

            button6.Enabled = success;
        }

        void generateInitialValue()
        {
            textBox3.Text = PasswordGenerator.generate(16);
        }

        void generateSalt()
        {
            int saltLength = (int)numericUpDown2.Value;
            textBox4.Text = PasswordGenerator.generate(saltLength);
        }

        void next()
        {
            tabPageNumber += 1;
            tabControl1.SelectedIndex = tabPageNumber;

            if (tabPageNumber > 0)
            {
                button7.Enabled = true;
            }
            button6.Enabled = false;

            if (tabPageNumber == 1)
            {
                checkInitialValueAndSalt();
            }
            else if (tabPageNumber == 2)
            {
                checkRSAKey();
            }
            else if (tabPageNumber == 3)
            {
                button6.Text = "Zapisz";
                button6.Enabled = true;
            }
            else if (tabPageNumber == 4)
            {
                tabPageNumber = 3;
                saveSettings();
            }
        }

        void back()
        {
            tabPageNumber -= 1;
            tabControl1.SelectedIndex = tabPageNumber;

            if (tabPageNumber == 0)
            {
                checkPassword();
                button7.Enabled = false;
            }
            else if (tabPageNumber == 1)
            {
                checkInitialValueAndSalt();
            }
            else if (tabPageNumber == 2)
            {
                checkRSAKey();
            }


            if (tabPageNumber < 3)
            {
                button6.Text = "Dalej";
            }
        }

        void generateRSAKeys()
        {
            this.UseWaitCursor = true;
            string keyLength = comboBox1.SelectedValue.ToString();
            textBox5.Text = RSAservice.generateKeyPair(keyLength);
            this.UseWaitCursor = false;
        }

        void copyToClipboard()
        {
            Clipboard.SetText(textBox1.Text);
        }

        void showPassword()
        {
            if (checkBox1.Checked)
            {
                textBox1.UseSystemPasswordChar = false;
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox1.UseSystemPasswordChar = true;
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            generatePassword();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveSettings();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            generateInitialValue();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            generateSalt();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            next();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            back();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            generateRSAKeys();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            checkPassword();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            checkInitialValueAndSalt();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            checkInitialValueAndSalt();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            checkRSAKey();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            checkRSAKey();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            copyToClipboard();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            showPassword();
        }
    }
}

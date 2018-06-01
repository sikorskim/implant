using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace implant
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            startup();
        }

        string loadedFileExt = null;
        public string key;
        string msgSign;

        string tempDirectory = "temp";

        void startup()
        {
            checkPassword();
            Setting.get();
            visiblePassword();
            fileStatus(0);
            getHashAlgorithmsList();
            toolStripStatusLabel3.Text = string.Empty;
            saveEnabled(false);
            encryptDecryptEnabled(false);
        }

        void checkPassword()
        {
            bool exist = Setting.checkSettingsExist();


            if (exist)
            {

                FrmLogowanie frmLogowanie = new FrmLogowanie();
                frmLogowanie.ShowDialog();
            }
            else
            {
                FrmInitialSetup frmInitialSetup = new FrmInitialSetup();
                frmInitialSetup.ShowDialog();
            }

        }

        void saveEnabled(bool enabled)
        {
                saveToolStripMenuItem.Enabled = enabled;
            toolStripButton5.Enabled = enabled;
        }

        void encryptDecryptEnabled(bool enabled)
        {
            toolStripButton2.Enabled = enabled;
            toolStripButton3.Enabled = enabled;
            encryptToolStripMenuItem.Enabled = enabled;
            decryptToolStripMenuItem.Enabled = enabled;
            toolStripButton8.Enabled = enabled;
            toolStripButton9.Enabled = enabled;
        }

        void insertSampleText()
        {
            textBox1.Text = Sample.sampleText();
        }

        void getHashAlgorithmsList()
        {
            comboBox1.DataSource = HashService.getAlgorithms();
        }

        void clear()
        {
            textBox1.Clear();
            textBox2.Clear();
            FileService.deleteTemporaryFiles();
        }

        string getInput(TextBox textBox)
        {
            string input = textBox.Text;
            return input;
        }

        void printOutput(string output)
        {
            textBox1.Text = output;
        }

        AESservice getAESsettings()
        {
            string salt = Setting.salt;
            string initializationVector = Setting.aesIv;

            AESservice aesService = new AESservice(salt, initializationVector);

            return aesService;
        }

        bool checkKey()
        {
            bool provided = true;

            if (textBox2.TextLength == 0)
                {
                provided = false;
            }

            return provided;
        }

        void encrypt()
        {
            string plainText = getInput(textBox1);
            string password = getInput(textBox2);

            AESservice aesService = getAESsettings();

            CryptObject cryptObject = aesService.encrypt(plainText, password);

            string output = cryptObject.value;
            if (cryptObject.operationSuccess)
            {
                printOutput(output);
                autoSign();
            }
            else
            {
                MessageBox.Show(output, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void decrypt()
        {
            string ciphertext = getInput(textBox1);
            string password = getInput(textBox2);

            AESservice aesService = getAESsettings();

            CryptObject cryptObject = aesService.decrypt(ciphertext, password);
            string output = cryptObject.value;

            if (cryptObject.operationSuccess)
            {
                printOutput(output);
            }
            else
            {
                MessageBox.Show(output, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        string openOOXML()
        {
            FileService.deleteTemporaryFiles();

            string file = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Office Files|*.docx;*.xlsx;*.pptx";
            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                unzipFile(file);
                loadedFileExt = checkExtension(file);
                checkFile();
                saveEnabled(true);
            }
            else
            {
                MessageBox.Show("Nie wybrano pliku!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                file = null;
            }

            toolStripStatusLabel3.Text = file;

            return file;
        }

        void checkFile()
        {
            if (FileService.checkForImplants(loadedFileExt))
            {
                openTextFile();
                fileStatus(2);
            }
            else
            {
                fileStatus(1);
            }
        }

        string checkExtension(string file)
        {
            string ext = "*" + file.Substring(file.Length - 5, 5);

            if (ext == "*.docx")
            {
                toolStripStatusLabel9.Image = global::implant.Properties.Resources.ooo_writer;
            }
            else if (ext == "*.xlsx")
            {
                toolStripStatusLabel9.Image = global::implant.Properties.Resources.ooo_calc;
            }
            else if (ext == "*.pptx")
            {
                toolStripStatusLabel9.Image = global::implant.Properties.Resources.ooo_impress;
            }

            return ext;
        }

        void unzipFile(string file)
        {
            ZIPService.unzip(file);
        }

        void zipFile()
        {
            string destinationFile = null;
            string sourceDirectory = tempDirectory;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.ValidateNames = true;
            saveFileDialog.Filter = "Office Files|" + loadedFileExt;

            DialogResult dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                destinationFile = saveFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("Wystąpił błąd! Spróbuj ponownie.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ZIPService.zip(sourceDirectory, destinationFile);
        }

        /// <summary>
        /// File status:
        /// 0 = no file loaded;
        /// 1 = empty file loaded;
        /// 2 = file with message loaded;
        /// </summary>
        /// <param name="code"></param>
        void fileStatus(int code)
        {
            if (code == 0)
            {
                toolStripStatusLabel10.Image = global::implant.Properties.Resources.stock_stop;
                saveEnabled(false);
            }
            else if (code == 1)
            {
                toolStripStatusLabel10.Image = global::implant.Properties.Resources.mail_signed;
                saveEnabled(true);
            }
            else if (code == 2)
            {
                toolStripStatusLabel10.Image = global::implant.Properties.Resources.mail_signed_verified;
                saveEnabled(true);
            }
        }

        void openTextFile()
        {
            string ciphertext = XMLService.readImplant(loadedFileExt);
            msgSign = XMLService.readSign(loadedFileExt);
            textBox1.Text = ciphertext;
        }

        string generateRadomString(int length)
        {
            string password = PasswordGenerator.generate(length);
            return password;
        }

        void generatePassword()
        {
            int length = Setting.genPassLength;
            textBox2.Text = generateRadomString(length);
        }

        void visiblePassword()
        {
            if (checkBox3.Checked)
            {
                textBox2.UseSystemPasswordChar = false;
            }
            else
            {
                textBox2.UseSystemPasswordChar = true;
            }
        }

        void saveToOOXML()
        {
            string path = FileService.getImplantPath(loadedFileExt);
            string ciphertext = textBox1.Text;
            XMLService.writeRels(loadedFileExt);
            XMLService.writeContentTypes(loadedFileExt);
            XMLService.writeImplant(loadedFileExt, ciphertext, msgSign);
            zipFile();
            deleteFromTemp();
        }

        void signMessage()
        {
            using (FrmSelectKey frmSelectKey = new FrmSelectKey(0))
            {
                frmSelectKey.ShowDialog(this);
            }

            if (key != null)
            {
                string msg = textBox1.Text;
                msgSign = RSAservice.sign(key, msg);
            }

            key = null;
        }

        void verifySign()
        {
            using (FrmSelectKey frmSelectKey = new FrmSelectKey(1))
            {
                frmSelectKey.ShowDialog(this);
            }

            if (key != null)
            {
                string msg = textBox1.Text;

                if (RSAservice.verify(key, msg, msgSign))
                {
                    MessageBox.Show("Podpis prawidłowy!", "Weryfikacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Podpis nieprawidłowy!", "Weryfikacja", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void autoSign()
        {
            //int keyId = SQLservice.getAutoSignId();

            //if (keyId != 0)
            //{
            //    string msg = textBox1.Text;
            //    key = RSAservice.getPrivKey(keyId);
            //    msgSign = RSAservice.sign(key, msg);
            //}
        }

        string textCount()
        {
            string textLength = textBox1.TextLength.ToString();
            return textLength;
        }

        void deleteFromTemp()
        {
            FileService.deleteTemporaryFiles();
        }

        void openAboutBox()
        {
            AboutBox1 aboutBox1 = new AboutBox1();
            aboutBox1.ShowDialog();
        }

        void openLogViewer()
        {
            FrmLogViewer frmLogViewer = new FrmLogViewer();
            frmLogViewer.ShowDialog();
        }

        void openSettings()
        {
            FrmSettings frmSettings = new FrmSettings();
            frmSettings.ShowDialog();
        }

        void openCertificates()
        {
            FrmMyKeys frmCertificates = new FrmMyKeys();
            frmCertificates.ShowDialog();
        }

        void computeHash()
        {
            string algo = comboBox1.SelectedItem.ToString();
            string text = textBox1.Text;
            string hash = HashService.compute(text, algo);
            textBox4.Text = hash;
        }

        void copyHashToClipboard()
        {
            Clipboard.SetText(textBox4.Text);
        }

        void openKeysManager()
        {
            FrmKeysManager frmKeysManager = new FrmKeysManager();
            frmKeysManager.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            encrypt();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            decrypt();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openOOXML();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            zipFile();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            generatePassword();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            visiblePassword();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label6.Text = textCount();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            saveToOOXML();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            deleteFromTemp();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openOOXML();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToOOXML();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openAboutBox();
        }

        private void insertSampleTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insertSampleText();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            deleteFromTemp();
        }

        private void viewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLogViewer();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openSettings();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openOOXML();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            saveToOOXML();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            encrypt();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            decrypt();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            openSettings();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void certificateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCertificates();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            computeHash();
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void viewDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            MessageBox.Show(XMLService.readImplant("*.docx"));
        }

        private void manageRSAKeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openKeysManager();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            signMessage();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            verifySign();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            encryptDecryptEnabled(checkKey());
        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            encrypt();
        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            decrypt();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLogViewer();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            copyHashToClipboard();
        }
    }
}

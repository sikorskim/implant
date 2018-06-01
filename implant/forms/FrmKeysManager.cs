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
    public partial class FrmKeysManager : Form
    {
        public FrmKeysManager()
        {
            InitializeComponent();
            startup();
        }

        void startup()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            refreshDgv();
            dgvSettings(dataGridView1);
            dgvSettings(dataGridView2);
            button5.Visible = false;
        }

        void dgvSettings(DataGridView dgv)
        {
            dgv.ClearSelection();
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToResizeRows = false;

            //dgv.Columns[1].HeaderText = "Nazwa";
            //dgv.Columns[3].HeaderText = "Notatka";
        }

        void loadContacts()
        {
            dataGridView1.DataSource = TrustedContact.getData();
            //dataGridView1.Columns[1].Visible = false;
            //  dataGridView1.Columns[2].HeaderText = "Klucz publiczny";
        }

        void loadMyKeys()
        {
            dataGridView2.DataSource = MyKey.getData();
            //dataGridView2.Columns[2].HeaderText = "Para kluczy";
        }

        void openAddContact()
        {
            if (tabControl1.SelectedIndex == 0)
            {
                using (FrmEditContact frmEditContact = new FrmEditContact())
                {
                    frmEditContact.ShowDialog(this);
                }
            }
            else
            {
                using (FrmMyKeys frmMyKeys = new FrmMyKeys())
                {
                    frmMyKeys.ShowDialog(this);
                }
            }
        }

       public void refreshDgv()
        {
            loadContacts();
            loadMyKeys();
        }

        void getPublicKey()
        {
            try
            {
                string pubKey = dataGridView2[1, dataGridView2.CurrentRow.Index].Value.ToString();
                Clipboard.SetText(pubKey);
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
                MessageBox.Show("Nie zaznaczono klucza!", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void delete()
        {
            bool success = true;
            try
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    string name = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
                    TrustedContact.remove(name);
                }
                else
                {
                    string name = dataGridView2[0, dataGridView2.CurrentRow.Index].Value.ToString();
                    MyKey.remove(name);
                }
        }
            catch (Exception ex)
            {
                MessageBox.Show("Nie zaznaczono elementu do usunięcia!", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogService.add(ex.ToString());
            }

            if (!success)
            {
                MessageBox.Show("Wystąpił błąd!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            refreshDgv();
        }

        void copyKeyButton()
        {
            if (tabControl1.SelectedIndex == 0)
            {
                button5.Visible = false;
            }
            else
            {
                button5.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openAddContact(); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            refreshDgv();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            getPublicKey();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            copyKeyButton();
        }
    }
}

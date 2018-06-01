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
    public partial class FrmSelectKey : Form
    {
        public FrmSelectKey(int viewId)
        {
            InitializeComponent();
            this.viewId = viewId;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            startup();
        }

        int viewId;

        void startup()
        {
            load();
            dgvSettings(dataGridView1);
        }

        void dgvSettings(DataGridView dgv)
        {
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AllowUserToResizeRows = false;
            dgv.ClearSelection();

            dgv.Columns[1].Visible = false;
            dgv.Columns[2].Visible = false;
        }

        void load()
        {
            if (viewId == 0)
            {
                dataGridView1.DataSource = MyKey.getData();
            }
            else
            {
                dataGridView1.DataSource = TrustedContact.getData();
            }
        }

        void getKey()
        {
            try
            {
                string selectedKey = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
                FrmMain frmMain = (FrmMain)this.Owner;
                frmMain.key = selectedKey;
                Dispose();
            }
            catch (Exception ex)
            {
                LogService.add(ex.ToString());
                MessageBox.Show("Nie wybrano klucza!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getKey();
        }
    }
}

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
    public partial class FrmLogViewer : Form
    {
        public FrmLogViewer()
        {
            InitializeComponent();
            startup();
        }

        void startup()
        {
            dgvSettings();
            loadLog();
        }

        void dgvSettings()
        {
            dataGridView1.ClearSelection();
            dataGridView1.MultiSelect = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToResizeRows = false;
        }

        void loadLog()
        {
            dataGridView1.DataSource= LogService.getData();
        }

        void showDetails()
        {
            string id = dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString();
            string time = dataGridView1[1, dataGridView1.CurrentRow.Index].Value.ToString();
            string description = dataGridView1[2, dataGridView1.CurrentRow.Index].Value.ToString();

            textBox1.Text = id;
            textBox2.Text = time;
            textBox3.Text = description;
        }

        void clearLog()
        {
            //LogService.clear();
            loadLog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearLog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //showDetails();
        }
    }
}

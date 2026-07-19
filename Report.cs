using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HostelAllocationDatabaseSystem
{
    public partial class Report : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=localhost\SQLEXPRESS01;Initial Catalog=hosteldb;Integrated Security=True");

        private void LoadData(string query)
        {
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvReport.DataSource = dt;
        }
        public Report()
        {
            InitializeComponent();
        }

        private void Report_Load(object sender, EventArgs e)
        {
            dgvReport.DataSource = null;
            cmbReportType.SelectedIndex = -1;
            cmbReportType.Text = "Select Report Type";
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            if (cmbReportType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a report type first!");
                return;
            }

            if (cmbReportType.SelectedItem.ToString() == "Guest Report")
            {
                LoadData("SELECT * FROM guestab");
            }
            else if (cmbReportType.SelectedItem.ToString() == "Room Report")
            {
                LoadData("SELECT * FROM roomtab");
            }
            else if (cmbReportType.SelectedItem.ToString() == "Staff Report")
            {
                LoadData("SELECT * FROM stafftab");
            }
            else if (cmbReportType.SelectedItem.ToString() == "Booking Report")
            {
                LoadData("SELECT * FROM booktab");
            }
            else if (cmbReportType.SelectedItem.ToString() == "Payment Report")
            {
                LoadData("SELECT * FROM paytab");
            }
        }

        private void btnClearReport_Click(object sender, EventArgs e)
        {
            dgvReport.DataSource = null;
            cmbReportType.SelectedIndex = -1;
            cmbReportType.Text = "Select Report Type";
        }

        private void btnPrintReport_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Print functionality can be enhanced with ReportViewer.");
            }
        }

        private void dgvReport_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;

            if (rowIndex >= 0)
            {
                DataGridViewRow row = dgvReport.Rows[rowIndex];
                string firstColumnValue = row.Cells[0].Value.ToString();

                MessageBox.Show("Selected: " + firstColumnValue);
            }
        }
    }
}

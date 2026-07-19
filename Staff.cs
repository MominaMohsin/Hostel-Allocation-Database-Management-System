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
    public partial class Staff : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=localhost\SQLEXPRESS01;Initial Catalog=hosteldb;Integrated Security=True");
        public Staff()
        {
            InitializeComponent();
        }

        private void Staff_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        void LoadData()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM stafftab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }
        void ClearFields()
        {
            txtStaffID.Clear();
            txtStaffName.Clear();
            txtPhone.Clear();
            txtSalary.Clear();
            txtAddress.Clear();
            cmbRole.SelectedIndex = -1;
            cmbShift.SelectedIndex = -1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtStaffName.Text == "")
            {
                MessageBox.Show("Enter Staff Name");
                return;
            }

            if (txtStaffName.Text == "" || cmbRole.Text == "" || txtPhone.Text == "" || txtSalary.Text == "" ||cmbShift.Text == "" ||txtAddress.Text == "")
            {
                MessageBox.Show("Please Fill All Fields");
                return;
            }

            if (!txtPhone.Text.StartsWith("03") && !txtPhone.Text.StartsWith("+92"))
            {
                MessageBox.Show("Invalid Phone Number");
                txtPhone.Focus();
                return;
            }

            if (txtPhone.Text.Length != 11 && txtPhone.Text.Length != 13)
            {
                MessageBox.Show("Phone Number length invalid");
                txtPhone.Focus();
                return;
            }

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                "INSERT INTO stafftab (StaffName, Role, Phone, Salary, Shift, Address) VALUES (@name,@role,@phone,@salary,@shift,@address)", con);

                cmd.Parameters.AddWithValue("@name", txtStaffName.Text);

                cmd.Parameters.AddWithValue("@role", cmbRole.Text);

                cmd.Parameters.AddWithValue("@phone", txtPhone.Text);

                cmd.Parameters.AddWithValue("@salary", txtSalary.Text);

                cmd.Parameters.AddWithValue("@shift", cmbShift.Text);

                cmd.Parameters.AddWithValue("@address", txtAddress.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Staff Saved Successfully");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                con.Close();
            }

            LoadData();

            ClearFields();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
            "DELETE FROM stafftab WHERE StaffID=@id", con);

            cmd.Parameters.AddWithValue("@id", txtStaffID.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Staff Deleted Successfully");

            LoadData();
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
            "UPDATE stafftab SET StaffName=@name, Role=@role, Phone=@phone, Salary=@salary, Shift=@shift, Address=@address WHERE StaffID=@id", con);

            cmd.Parameters.AddWithValue("@id", txtStaffID.Text);
            cmd.Parameters.AddWithValue("@name", txtStaffName.Text);
            cmd.Parameters.AddWithValue("@role", cmbRole.Text);
            cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
            cmd.Parameters.AddWithValue("@salary", txtSalary.Text);
            cmd.Parameters.AddWithValue("@shift", cmbShift.Text);
            cmd.Parameters.AddWithValue("@address", txtAddress.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Staff Updated Successfully");

            LoadData();
            ClearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtStaffID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtStaffName.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                cmbRole.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtPhone.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtSalary.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                cmbShift.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                txtAddress.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
        }

        private void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRole.Text == "Receptionist")
                txtSalary.Text = "35000";
            else if (cmbRole.Text == "Manager")
                txtSalary.Text = "70000";
            else if (cmbRole.Text == "Cleaner")
                txtSalary.Text = "25000";
            else if (cmbRole.Text == "Security Guard")
                txtSalary.Text = "30000";
            else if (cmbRole.Text == "Accountant")
                txtSalary.Text = "50000";
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void txtSalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void txtStaffName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')

            {
                e.Handled = true;
            }
        }
    }
}

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
    public partial class Guest : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=localhost\SQLEXPRESS01;Database=hosteldb;Trusted_Connection=True;");
        public Guest()
        {
            InitializeComponent();
        }

        private void Guest_Load(object sender, EventArgs e)
        {
            LoadData();

            cmbGender.Items.Clear();
            cmbGender.Items.Add("Male");
            cmbGender.Items.Add("Female");

            cmbGender.SelectedIndex = -1;
        }

      
        void LoadData()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM guestab", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        
        void ClearFields()
        {
            txtGuestID.Clear();
            txtGuestName.Clear();
            txtCNIC.Clear();
            txtPhone.Clear();
            cmbGender.SelectedIndex = -1;
            txtAddress.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearFields();
            txtGuestName.Focus();
        }

    
        private void btnSave_Click(object sender, EventArgs e)
        {
            if ( txtGuestName.Text == "" ||
        txtCNIC.Text == "" ||
        txtPhone.Text == "" ||
        cmbGender.Text == "" ||
        txtAddress.Text == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            if (txtCNIC.Text.Length != 13)
            {
                MessageBox.Show("CNIC must be exactly 13 digits");
                txtCNIC.Focus();
                return;
            }

            foreach (char c in txtCNIC.Text)
            {
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("CNIC must contain digits only");
                    txtCNIC.Focus();
                    return;
                }
            }
            SqlCommand check = new SqlCommand("SELECT COUNT(*) FROM guestab WHERE CNIC=@cnic", con);
            check.Parameters.AddWithValue("@cnic", txtCNIC.Text);

            con.Open();
            int count = (int)check.ExecuteScalar();
            con.Close();

            if (count > 0)
            {
                MessageBox.Show("This CNIC already exists!");
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

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO guestab (GuestName, CNIC, Phone, Gender, Address) VALUES (@name,@cnic,@phone,@gender,@address)", con);

            cmd.Parameters.AddWithValue("@name", txtGuestName.Text);
            cmd.Parameters.AddWithValue("@cnic", txtCNIC.Text);
            cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
            cmd.Parameters.AddWithValue("@gender", cmbGender.Text);
            cmd.Parameters.AddWithValue("@address", txtAddress.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Guest Saved Successfully!");
            LoadData();
            ClearFields();
        }

        

        private void btnDelete_Click(object sender, EventArgs e)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM guestab WHERE GuestID=@id", con);
            cmd.Parameters.AddWithValue("@id", txtGuestID.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Guest Deleted Successfully!");
            LoadData();
            ClearFields();
        }

        private void txtGuestID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtGuestName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')

            {
                e.Handled = true;
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCNIC_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                txtGuestID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                txtGuestName.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtCNIC.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                txtPhone.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                cmbGender.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                txtAddress.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE guestab SET GuestName=@name, CNIC=@cnic, Phone=@phone, Gender=@gender, Address=@address WHERE GuestID=@id", con);

            cmd.Parameters.AddWithValue("@id", txtGuestID.Text);
            cmd.Parameters.AddWithValue("@name", txtGuestName.Text);
            cmd.Parameters.AddWithValue("@cnic", txtCNIC.Text);
            cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
            cmd.Parameters.AddWithValue("@gender", cmbGender.Text);
            cmd.Parameters.AddWithValue("@address", txtAddress.Text);

            cmd.ExecuteNonQuery();
            con.Close();

            MessageBox.Show("Guest Updated Successfully!");
            LoadData();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
    }
}




using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostelAllocationDatabaseSystem
{
    public partial class Room : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=localhost\SQLEXPRESS01;Initial Catalog=hosteldb;Integrated Security=True");
        public Room()
        {
            InitializeComponent();
        }
        private void Room_Load(object sender, EventArgs e)
        {
            LoadData();

            cmbRoomType.Items.Clear();
            cmbRoomType.Items.Add("Single");
            cmbRoomType.Items.Add("Double");
            cmbRoomType.Items.Add("Shared");

            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Available");
            cmbStatus.Items.Add("Booked");

            txtRoomID.Enabled = false;
            txtCharges.ReadOnly = true;
        }
        void LoadData()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM roomtab", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }
        void ClearFields()
        {
            txtRoomID.Clear();
            txtRoomNo.Clear();
            cmbRoomType.SelectedIndex = -1;
            txtCapacity.Clear();
            txtCharges.Clear();
            cmbStatus.SelectedIndex = -1;
        }

        private void cmbRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (cmbRoomType.Text == "Single")
            {
                txtCharges.Text = "5000";
                txtCapacity.Text = "1";
            }
            else if (cmbRoomType.Text == "Double")
            {
                txtCharges.Text = "8000";
                txtCapacity.Text = "2";
            }
            else if (cmbRoomType.Text == "Shared")
            {
                txtCharges.Text = "3000";
                txtCapacity.Text = "3";
            }
        }
        

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtRoomNo.Text == "" ||
       cmbRoomType.Text == "" ||
       txtCapacity.Text == "" ||
       txtCharges.Text == "" ||
       cmbStatus.Text == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO roomtab(RoomNo, RoomType, Capacity, Charges, Status) VALUES(@roomno,@type,@capacity,@charges,@status)", con);

            cmd.Parameters.AddWithValue("@roomno", txtRoomNo.Text);
            cmd.Parameters.AddWithValue("@type", cmbRoomType.Text);
            cmd.Parameters.AddWithValue("@capacity", txtCapacity.Text);
            cmd.Parameters.AddWithValue("@charges", txtCharges.Text);
            cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

            cmd.ExecuteNonQuery();

            con.Close();

            MessageBox.Show("Room Saved Successfully!");

            LoadData();
            ClearFields();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtRoomID.Text == "")
            {
                MessageBox.Show("Select a row first");
                return;
            }

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM roomtab WHERE RoomID=@id", con);

            cmd.Parameters.AddWithValue("@id", txtRoomID.Text);

            cmd.ExecuteNonQuery();

            con.Close();

            MessageBox.Show("Deleted Successfully!");

            LoadData();
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtRoomID.Text == "")
            {
                MessageBox.Show("Select a row first");
                return;
            }

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE roomtab SET RoomNo=@roomno, RoomType=@type, Capacity=@capacity, Charges=@charges, Status=@status WHERE RoomID=@id", con);

            cmd.Parameters.AddWithValue("@id", txtRoomID.Text);
            cmd.Parameters.AddWithValue("@roomno", txtRoomNo.Text);
            cmd.Parameters.AddWithValue("@type", cmbRoomType.Text);
            cmd.Parameters.AddWithValue("@capacity", txtCapacity.Text);
            cmd.Parameters.AddWithValue("@charges", txtCharges.Text);
            cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

            cmd.ExecuteNonQuery();

            con.Close();

            MessageBox.Show("Updated Successfully!");

            LoadData();
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                txtRoomID.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                txtRoomNo.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                cmbRoomType.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                txtCapacity.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                txtCharges.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                cmbStatus.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            }
        }

       
        private void txtRoomID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
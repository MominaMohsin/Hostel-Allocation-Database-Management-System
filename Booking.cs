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
    public partial class Booking : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=localhost\SQLEXPRESS01;Initial Catalog=hosteldb;Integrated Security=True");

        public Booking()
        {
            InitializeComponent();
        }

        private void Booking_Load(object sender, EventArgs e)
        {
            LoadGuests();
            LoadRooms();
            LoadStaff();
            LoadData();
        }
        void LoadGuests()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT GuestName FROM guestab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbGuest.DataSource = dt;
            cmbGuest.DisplayMember = "GuestName";
        }
        void LoadRooms()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT RoomNo FROM roomtab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbRoom.DataSource = dt;
            cmbRoom.DisplayMember = "RoomNo";
        }
        void LoadStaff()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT StaffName FROM stafftab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbStaff.DataSource = dt;
            cmbStaff.DisplayMember = "StaffName";
        }
        void LoadData()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT * FROM booktab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }
        void ClearFields()
        {
            txtBookingID.Clear();
            cmbGuest.SelectedIndex = -1;
            cmbRoom.SelectedIndex = -1;
            cmbStaff.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        bool ValidateFields()
        {
            if (cmbGuest.Text == "" ||
                cmbRoom.Text == "" ||
                cmbStaff.Text == "" ||
                cmbStatus.Text == "")
            {
                MessageBox.Show("Please fill all fields");
                return false;
            }

            if (dtCheckIn.Value >= dtCheckOut.Value)
            {
                MessageBox.Show("Check-Out must be after Check-In");
                return false;
            }

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                con.Open();

                SqlCommand roleCmd = new SqlCommand(
     "SELECT Role FROM stafftab WHERE StaffName=@staff", con);

                roleCmd.Parameters.AddWithValue("@staff", cmbStaff.Text);

                object roleObj = roleCmd.ExecuteScalar();

                if (roleObj == null)
                {
                    MessageBox.Show("Staff not found");
                    return;
                }

                string role = roleObj.ToString();

                if (role != "Receptionist")
                {
                    MessageBox.Show("Only Receptionist can handle bookings");
                    return;
                }

                SqlCommand checkRoom = new SqlCommand(
                "SELECT COUNT(*) FROM booktab WHERE RoomNo=@room AND BookingStatus='Booked'", con);

                checkRoom.Parameters.AddWithValue("@room", cmbRoom.Text);

                int count = (int)checkRoom.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("This room is already booked");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                "INSERT INTO booktab (GuestName, RoomNo, StaffName, CheckInDate, CheckOutDate, BookingStatus) " +
                "VALUES (@guest,@room,@staff,@in,@out,@status)", con);

                cmd.Parameters.AddWithValue("@guest", cmbGuest.Text);

                cmd.Parameters.AddWithValue("@room", cmbRoom.Text);

                cmd.Parameters.AddWithValue("@staff", cmbStaff.Text);

                cmd.Parameters.AddWithValue("@in", dtCheckIn.Value);

                cmd.Parameters.AddWithValue("@out", dtCheckOut.Value);

                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Booking Saved Successfully");
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                con.Open();

                SqlCommand roleCmd = new SqlCommand(
         "SELECT Role FROM stafftab WHERE StaffName=@staff", con);

                roleCmd.Parameters.AddWithValue("@staff", cmbStaff.Text);

                object roleObj = roleCmd.ExecuteScalar();

                if (roleObj == null)
                {
                    MessageBox.Show("Staff not found");
                    return;
                }

                string role = roleObj.ToString();

                if (role != "Receptionist")
                {
                    MessageBox.Show("Only Receptionist can handle bookings");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                "UPDATE booktab SET " +
                "GuestName=@guest, " +
                "RoomNo=@room, " +
                "StaffName=@staff, " +
                "CheckInDate=@in, " +
                "CheckOutDate=@out, " +
                "BookingStatus=@status " +
                "WHERE BookingID=@id", con);

                cmd.Parameters.AddWithValue("@id", txtBookingID.Text);

                cmd.Parameters.AddWithValue("@guest", cmbGuest.Text);

                cmd.Parameters.AddWithValue("@room", cmbRoom.Text);

                cmd.Parameters.AddWithValue("@staff", cmbStaff.Text);

                cmd.Parameters.AddWithValue("@in", dtCheckIn.Value);

                cmd.Parameters.AddWithValue("@out", dtCheckOut.Value);

                cmd.Parameters.AddWithValue("@status", cmbStatus.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Booking Updated Successfully");
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                "DELETE FROM booktab WHERE BookingID=@id", con);

                cmd.Parameters.AddWithValue("@id", txtBookingID.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Booking Deleted Successfully");
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtBookingID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                cmbGuest.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                cmbRoom.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                cmbStaff.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                dtCheckIn.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
                dtCheckOut.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[5].Value);
                cmbStatus.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
        }
    }
 
}

        
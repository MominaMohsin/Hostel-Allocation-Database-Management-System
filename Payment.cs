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
    public partial class Payment : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=localhost\SQLEXPRESS01;Initial Catalog=hosteldb;Integrated Security=True");

        public Payment()
        {
            InitializeComponent();
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            cmbBookingID.DataSource = null;
            cmbStaff.DataSource = null;

            LoadBookingIDs();
            LoadStaff();
            LoadData();
            GeneratePaymentID();

            cmbBookingID.SelectedIndex = -1;
            cmbStaff.SelectedIndex = -1;
        }
        void LoadBookingIDs()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT BookingID FROM booktab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbBookingID.DataSource = dt;
            cmbBookingID.DisplayMember = "BookingID";
            cmbBookingID.ValueMember = "BookingID";
        }
        void LoadStaff()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT StaffName FROM stafftab WHERE Role='Accountant'", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            cmbStaff.DataSource = dt;
            cmbStaff.DisplayMember = "StaffName";
            cmbStaff.ValueMember = "StaffName";
        }
        void LoadData()
        {
            SqlDataAdapter da = new SqlDataAdapter(
            "SELECT * FROM paytab", con);

            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt;
        }

        private void cmbBookingID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBookingID.SelectedValue == null)
                return;

            // ⭐ DATAROWVIEW FIX
            if (cmbBookingID.SelectedValue is DataRowView)
                return;

            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(
                "SELECT GuestName, RoomNo, CheckInDate, CheckOutDate " +
                "FROM booktab WHERE BookingID=@id", con);

                cmd.Parameters.AddWithValue("@id",
                Convert.ToInt32(cmbBookingID.SelectedValue));

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtGuestName.Text =
                    dr["GuestName"].ToString();

                    txtRoomNo.Text =
                    dr["RoomNo"].ToString();

                    dtCheckIn.Value =
                    Convert.ToDateTime(dr["CheckInDate"]);

                    dtCheckOut.Value =
                    Convert.ToDateTime(dr["CheckOutDate"]);
                }

                dr.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                con.Close();
            }

            CalculateDays();
            LoadRoomCharges();

        }
        void LoadRoomCharges()
        {
            SqlCommand cmd = new SqlCommand(
            "SELECT RoomType FROM roomtab WHERE RoomNo=@room", con);

            cmd.Parameters.AddWithValue("@room", txtRoomNo.Text);

            con.Open();
            object result = cmd.ExecuteScalar();
            con.Close();

            if (result == null) return;
            string type = result.ToString();

            if (type == "Single")
                txtRoomCharges.Text = "5000";
            else if (type == "Double")
                txtRoomCharges.Text = "8000";
            else if (type == "Shared")
                txtRoomCharges.Text = "3000";
        }
            
        void CalculateDays()
        {
            TimeSpan days = dtCheckOut.Value - dtCheckIn.Value;
            txtDays.Text = days.Days.ToString();

            CalculateBill();
        }
        void CalculateBill()
        {
            if (txtDays.Text != "" && txtRoomCharges.Text != "")
            {
                int days = Convert.ToInt32(txtDays.Text);
                int charges = Convert.ToInt32(txtRoomCharges.Text);
                int Total = days * charges;

                txtTotalBill.Text = Total.ToString();
            }
        }

       

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            if (txtPaidAmount.Text != "" && txtTotalBill.Text != "")
            {
                int total = Convert.ToInt32(txtTotalBill.Text);
                int paid = Convert.ToInt32(txtPaidAmount.Text);

                txtRemaining.Text = (total - paid).ToString();
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbBookingID.Text == "" || cmbStaff.Text == "" || txtPaidAmount.Text == "")
            {
                MessageBox.Show("Please fill all required fields");
                return;
            }

            try
            {
                con.Open();

                SqlCommand roleCmd = new SqlCommand(
                "SELECT Role FROM stafftab WHERE StaffName=@s", con);

                roleCmd.Parameters.AddWithValue("@s", cmbStaff.Text);

                string role = roleCmd.ExecuteScalar().ToString();

                if (role != "Accountant")
                {
                    MessageBox.Show("Only Accountant can receive payment");
                    return;
                }

                SqlCommand cmd = new SqlCommand(
                "INSERT INTO paytab VALUES " +
                "(@bid,@guest,@room,@in,@out,@charges,@total,@paid,@remain,@date,@staff)", con);

                cmd.Parameters.AddWithValue("@bid",Convert.ToInt32 (cmbBookingID.SelectedValue));
                cmd.Parameters.AddWithValue("@guest", txtGuestName.Text);
                cmd.Parameters.AddWithValue("@room", txtRoomNo.Text);
                cmd.Parameters.AddWithValue("@in", dtCheckIn.Value);
                cmd.Parameters.AddWithValue("@out", dtCheckOut.Value);
                cmd.Parameters.AddWithValue("@charges", txtRoomCharges.Text);
                cmd.Parameters.AddWithValue("@total", txtTotalBill.Text);
                cmd.Parameters.AddWithValue("@paid", txtPaidAmount.Text);
                cmd.Parameters.AddWithValue("@remain", txtRemaining.Text);
                cmd.Parameters.AddWithValue("@date", dtPaymentDate.Value);
                cmd.Parameters.AddWithValue("@staff", cmbStaff.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Payment Saved Successfully");
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
        void ClearFields()
        {
            txtPaymentID.Clear();

            cmbBookingID.SelectedIndex = -1;

            txtGuestName.Clear();

            txtRoomNo.Clear();

            txtDays.Clear();

            txtRoomCharges.Clear();

            txtTotalBill.Clear();

            txtPaidAmount.Clear();

            txtRemaining.Clear();

            cmbStaff.SelectedIndex = -1;

            GeneratePaymentID();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ClearFields();

            GeneratePaymentID();
        }
        void GeneratePaymentID()
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
            "SELECT ISNULL(MAX(PaymentID),0)+1 FROM paytab", con);

            txtPaymentID.Text = cmd.ExecuteScalar().ToString();

            con.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
            "DELETE FROM paytab WHERE PaymentID=@id", con);

            cmd.Parameters.AddWithValue("@id", txtPaymentID.Text);

            cmd.ExecuteNonQuery();

            con.Close();

            MessageBox.Show("Deleted Successfully");

            LoadData();
            ClearFields();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cmbBookingID.Text == "" || cmbStaff.Text == "")
            {
                MessageBox.Show("Please select required fields");
                return;
            }

            try
            {
                con.Open();

                // ================= ROLE CHECK =================

                SqlCommand roleCmd = new SqlCommand(
                "SELECT Role FROM stafftab WHERE StaffName=@s", con);

                roleCmd.Parameters.AddWithValue("@s", cmbStaff.Text);

                string role = roleCmd.ExecuteScalar().ToString();

                if (role != "Accountant")
                {
                    MessageBox.Show("Only Accountant can update payment");
                    return;
                }

                // ================= UPDATE QUERY =================

                SqlCommand cmd = new SqlCommand(
                "UPDATE paytab SET " +
                "BookingID=@bid, GuestName=@guest, RoomNo=@room, " +
                "CheckInDate=@in, CheckOutDate=@out, RoomCharges=@charges, " +
                "TotalBill=@total, PaidAmount=@paid, RemainingBalance=@remain, " +
                "PaymentDate=@date, Accountant=@acc " +
                "WHERE PaymentID=@id", con);

                cmd.Parameters.AddWithValue("@id", txtPaymentID.Text);
                cmd.Parameters.AddWithValue("@bid",Convert.ToInt32 (cmbBookingID.SelectedValue));
                cmd.Parameters.AddWithValue("@guest", txtGuestName.Text);
                cmd.Parameters.AddWithValue("@room", txtRoomNo.Text);
                cmd.Parameters.AddWithValue("@in", dtCheckIn.Value);
                cmd.Parameters.AddWithValue("@out", dtCheckOut.Value);
                cmd.Parameters.AddWithValue("@charges", txtRoomCharges.Text);
                cmd.Parameters.AddWithValue("@total", txtTotalBill.Text);
                cmd.Parameters.AddWithValue("@paid", txtPaidAmount.Text);
                cmd.Parameters.AddWithValue("@remain", txtRemaining.Text);
                cmd.Parameters.AddWithValue("@date", dtPaymentDate.Value);
                cmd.Parameters.AddWithValue("@acc", cmbStaff.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Payment Updated Successfully");
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtPaymentID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            cmbBookingID.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtGuestName.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtRoomNo.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

            dtCheckIn.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value);
            dtCheckOut.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[5].Value);

            txtRoomCharges.Text = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
            txtTotalBill.Text = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();
            txtPaidAmount.Text = dataGridView1.Rows[e.RowIndex].Cells[8].Value.ToString();
            txtRemaining.Text = dataGridView1.Rows[e.RowIndex].Cells[9].Value.ToString();

            dtPaymentDate.Value = Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[10].Value);

            cmbStaff.Text = dataGridView1.Rows[e.RowIndex].Cells[11].Value.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
    }
}

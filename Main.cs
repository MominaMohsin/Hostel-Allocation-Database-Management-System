using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostelAllocationDatabaseSystem
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void guestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Guest gt = new Guest();
            gt.Show();
        }

        private void roomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Room rm = new Room();
            rm.Show();
        }

        private void staffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Staff sf = new Staff();
            sf.Show();
        }

        private void bookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Booking bk = new Booking();
            bk.Show();
        }

        private void paymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Payment pt = new Payment();
            pt.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Report rf = new Report();
            rf.Show();
        }
    }
}

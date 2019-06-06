using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VioGIS
{
    public partial class AdmitBookmarkName : Form
    {
        public Form1 m_frmMaim;
        public AdmitBookmarkName(Form1 frm)
        {
            InitializeComponent();
            if (frm != null)
            {
                m_frmMaim = frm;
            }
        }
                
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (m_frmMaim != null || tbBookmarkName.Text == "")
            {
                m_frmMaim.CreatBookmark(tbBookmarkName.Text);
            }
            this.Close();
        }
    }
}

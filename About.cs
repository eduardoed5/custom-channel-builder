using System;
using System.Windows.Forms;

namespace CreatorChannelsXrmToolbox
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            DateTime _date = DateTime.Now;
            LblYear.Text = _date.Year.ToString();
        }

        private void LblLinkIcon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LblLinkIcon.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.flaticon.com/free-icons/process");
        }
    }
}

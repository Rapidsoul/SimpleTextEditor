using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LB5
{
    public partial class MyHelp : Form
    {
        public MyHelp()
        {
            InitializeComponent();
            linkLabel1.Text = "https://plus.google.com/116921188323374652255";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
    }
}

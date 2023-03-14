using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.Interface;

namespace TestApp
{
    public partial class Form1 : Form
    {
        internal FocusAdaptor m_fa;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {


            Chooser ch = new Chooser();
            ch.DeviceType = "Focuser";
            string device = ch.Choose();

            if (string.IsNullOrEmpty(device))
                return;


            m_fa = new FocusAdaptor(device, "Dummy Name");
            focusControl.Initialize(m_fa);
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_fa != null)
                m_fa.Dispose();
        }

    }
}

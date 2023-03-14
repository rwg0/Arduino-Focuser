using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASCOM.Simple.Arduino.Focuser
{
    public partial class CrashForm : Form
    {
        private Timer _timer;

        public CrashForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += TimerOnTick;
            _timer.Enabled = true;
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}

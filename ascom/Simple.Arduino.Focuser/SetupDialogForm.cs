using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO.Ports;

namespace ASCOM.Simple.Arduino.Focuser
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        public SetupDialogForm(string defaultPort, bool reversed, int position)
        {
            InitializeComponent();

            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }

            foreach (string s in comboBox1.Items)
            {
                if (s == defaultPort)
                {
                    comboBox1.SelectedItem = s;
                    break;
                }
            }

            if (comboBox1.SelectedIndex == -1 && comboBox1.Items.Count>0)
                comboBox1.SelectedIndex = 0;

            checkBox1.Checked = reversed;

            if (comboBox1.Items.Count == 0)
                cmdOK.Enabled = false;

            numericUpDown1.Value = position;
        }

        internal string GetSelectedPort()
        {
            return comboBox1.SelectedItem as String;
        }

        internal bool IsReversed()
        {
            return checkBox1.Checked;
        }


        private void cmdOK_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void BrowseToAscom(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Position = (int)numericUpDown1.Value;
        }

        public int Position { get; set; }
    }
}
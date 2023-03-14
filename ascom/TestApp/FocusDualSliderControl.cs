

using System;
using System.Windows.Forms;

namespace TestApp
{
    public partial class FocusDualSliderControl : UserControl
    {
        private IFocusAdaptor _adaptor;
        private int _iMaximum;
        private int _iMinimum;
        private int _value;

        public FocusDualSliderControl()
        {
            InitializeComponent();
            Enabled = false;
        }

        public void Initialize(IFocusAdaptor adapator)
        {
            adapator.Connected = true;
            _adaptor = adapator;
            SetRanges();
            SetValue(_adaptor.Current);
            Enabled = true;
            EnableControls(true);
            textBoxValue.Text = _value.ToString();
            adapator.MovingChanged += adapator_MovingChanged;
            adapator.PositionChanged += adapator_PositionChanged;
        }

        void adapator_PositionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<EventArgs>(adapator_PositionChanged), sender, e);
                return;
            }
            SetValue(_adaptor.Current);
            textBoxValue.Text = _value.ToString();
        }

        private void adapator_MovingChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<EventArgs>(adapator_MovingChanged), sender, e);
                return;
            }

            bool isMovingNow = _adaptor.IsMoving;

            EnableControls(!isMovingNow);
            if (!isMovingNow)
            {
                SetValue(_adaptor.Current);
                textBoxValue.Text = _value.ToString();
            }
        }

        private void EnableControls(bool p)
        {
            buttonLeftCoarse.Enabled = p;
            buttonLeftFine.Enabled = p;
            buttonRightCoarse.Enabled = p;
            buttonRightFine.Enabled = p;
            numericUpDownCoarse.Enabled = p;
            numericUpDownFine.Enabled = p;
            buttonStop.Enabled = !p;
        }


        private void SetValue(int value)
        {
            _value = value;
        }


        private void SetRanges()
        {
            _iMinimum = 0;
            _iMaximum = _adaptor.Maximum;

            numericUpDownCoarse.Maximum = Math.Min(numericUpDownCoarse.Maximum, _adaptor.MaxChange);
            numericUpDownFine.Maximum = Math.Min(numericUpDownFine.Maximum, _adaptor.MaxChange);
        }


        private void Adjust(int offset)
        {
            if (checkBoxReverse.Checked)
                offset = -offset;

            int focusValue = _value + offset;
            focusValue = Math.Max(focusValue, _iMinimum);
            focusValue = Math.Min(focusValue, _iMaximum);

            int realTarget = _adaptor.Move(focusValue);
            textBoxValue.Text = realTarget.ToString();
        }


        private void buttonStop_Click(object sender, EventArgs e)
        {
            _adaptor.Stop();
        }

        private void buttonLeftCoarse_Click(object sender, EventArgs e)
        {
            Adjust(-(int) numericUpDownCoarse.Value);
        }

        private void buttonRightCoarse_Click(object sender, EventArgs e)
        {
            Adjust((int) numericUpDownCoarse.Value);
        }

        private void buttonLeftFine_Click(object sender, EventArgs e)
        {
            Adjust(-(int) numericUpDownFine.Value);
        }

        private void buttonRightFine_Click(object sender, EventArgs e)
        {
            Adjust((int) numericUpDownFine.Value);
        }

        private void buttonSetup_Click(object sender, EventArgs e)
        {
            _adaptor.DoSetup();
        } 

        
    }
}
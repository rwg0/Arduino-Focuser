// This file is part of SharpCap, a C# Video Capture Application for Windows
//
//    Copyright (c) 2010-2012, Robin Glover
//    All rights reserved.
//
//    Redistribution and use in source and binary forms, with or without
//    modification, are permitted provided that the following conditions are met:
//        * Redistributions of source code must retain the above copyright
//          notice, this list of conditions and the following disclaimer.
//        * Redistributions in binary form must reproduce the above copyright
//          notice, this list of conditions and the following disclaimer in the
//          documentation and/or other materials provided with the distribution.
//        * Redistributors in source code or binary form may not impose additional
//          conditions to these terms of distribution 
//        * Redistributors in source code or binary form may not make this code or
//          products derived from it available in return for money or reward. This includes
//          but is not limited to : Payment for supply of software or sourcecode; payment
//          to unlock or activate a trial version of a product; payment to unlock extra 
//          features in a product; requesting voluntary donations in respect of a product
//
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//    DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
//    DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//    (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//    LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//    ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//    (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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
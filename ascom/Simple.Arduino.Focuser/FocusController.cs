using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace ASCOM.Simple.Arduino.Focuser
{
    class FocusController : IDisposable
    {
        const int maxMove = 1000;
        SerialPort m_port;
        Thread m_workerThread;
        ThreadControl m_control = new ThreadControl();
        private Exception _commsError;

        public FocusController(string portName)
        {

            m_port = new SerialPort(portName);
            m_port.BaudRate = 4800;
            m_port.Parity = Parity.None;
            m_port.DataBits = 8;
            m_port.Handshake = Handshake.None;
            m_port.StopBits = StopBits.One;
            m_port.ReadTimeout = 500;
            m_port.WriteTimeout = 500;

            m_port.RtsEnable = false;
            m_port.Open();

            try
            {
                // make sure we can talk to the port
                DoMove(1);
                DoMove(-1);
            }
            catch (Exception)
            {
                m_port.Close();
                throw;
            }

            m_workerThread = new Thread(new ThreadStart(ThreadProc));
            m_workerThread.IsBackground = true;
            m_workerThread.Start();

        }

        void ThreadProc()
        {
            try
            {
                while (!m_control.Destroyed)
                {
                    Thread.Sleep(1);
                    int command = m_control.Command;
                    if (command != 0)
                        ProcessCommand(command);
                    m_control.Halt = false;
                }
            }
            catch (Exception e)
            {
                _commsError = e;
                m_control.Moving = false;
            }

        }

        private void ProcessCommand(int command)
        {
            try
            {
                m_control.Moving = true;
                DoMove(command);
            }
            finally
            {
                m_control.Moving = false;
                m_control.Command = 0;
            }
        }

        private void DoMove(int stepSize)
        {
            m_port.RtsEnable = (stepSize < 0) ^ Reversed ;
            Thread.Sleep(20);
            for (int i = 0; i < Math.Abs(stepSize); i++)
            {
                if (m_control.Halt)
                    break;

 //               Debug.WriteLine("Moving focuser : " + ((stepSize < 0) ? "In" : "Out"));
                m_port.Write(new byte[1] {0x00}, 0, 1);
            }
            Debug.WriteLine("Move Complete");
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (m_port != null)
            {
                m_port.Close();
            }
            m_control.Destroyed = true;
        }

        #endregion

        internal bool IsMoving()
        {

            return m_control.Moving;
        }

        internal void Halt()
        {
            CheckForCommsError();

            if (IsMoving())
            {
                m_control.Halt = true;

                Debug.WriteLine("Halting");

                while (true)
                {
                    if (!IsMoving())
                        break;
                    Thread.Sleep(1);
                }
            }
        }

        private void CheckForCommsError()
        {
            if (_commsError != null)
                throw new Exception("Error talking to focuser", _commsError);
        }

        internal void MoveRelative(int val)
        {
            CheckForCommsError();

            if (Math.Abs(val) > maxMove)
                val = Math.Sign(val) * maxMove;

            Halt();

            m_control.Command = val;
        }

        internal int MaxMove()
        {
            return maxMove;
        }

        public bool Reversed { get; set; }
    }

}

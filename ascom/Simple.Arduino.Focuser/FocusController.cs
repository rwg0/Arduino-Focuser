using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ASCOM.Simple.Arduino.Focuser
{
    class FocusController : IDisposable, INotifyPropertyChanged
    {
        private readonly SerialPort m_port;
        private bool _isMoving;
        private string _pending = "";
        private readonly ConcurrentQueue<string> _readLines = new ConcurrentQueue<string>();

        public FocusController(string portName)
        {

            m_port = new SerialPort(portName);
            m_port.BaudRate = 9600;
            m_port.Parity = Parity.None;
            m_port.DataBits = 8;
            m_port.Handshake = Handshake.None;
            m_port.StopBits = StopBits.One;
            m_port.ReadTimeout = 500;
            m_port.WriteTimeout = 500;

            m_port.RtsEnable = false;
            m_port.DataReceived += _port_DataReceived;

            m_port.Open();

            SendRawCommand("I");
            string id = ReadTextTimeout(1000);

            if (id?.Trim() != "R Simple.Arduino.Focuser")
                throw new Exception($"Incorrect identification from focuser : {id}");

            while (_readLines.Count > 0)
                _readLines.TryDequeue(out var _);
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                try
                {
                    ReadAnyPortData();

                }
                catch (Exception exception)
                {
                    Trace.WriteLine(exception);
                }
            }
        }

        private void ReadAnyPortData()
        {
            _pending += m_port.ReadExisting();
            while (_pending.Contains("\n"))
            {
                var index = _pending.IndexOf("\n");
                var line = _pending.Substring(0, index).Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    _pending = _pending.TrimStart();
                    continue;
                }
                _pending = index == _pending.Length - 1 ? "" : _pending.Substring(index);
                if (IsMoving)
                {
                    if (ProcessPosition(line))
                    {
                        IsMoving = false;
                    }
                }
                else
                    _readLines.Enqueue(line);
            }
        }

        public void Dispose()
        {
            m_port.DataReceived -= _port_DataReceived;
            m_port?.Close();
        }

        public bool Reversed { get; set; }

        public void Halt()
        {
            SendCommand("H", 5000);
            IsMoving = false;
        }

        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                if (value == _isMoving)
                    return;
                _isMoving = value;
                OnPropertyChanged(nameof(IsMoving));
            }
        }

        public int MaxMove()
        {
            return 100000;
        }


        public int Position { get; private set; }

        public void InitializePosition(int position)
        {
            SendCommand($"P {position}");
        }

        public void SetReverse(bool reverse)
        {
            SendCommand($"R {(reverse ? 1 : 0)}");
        }

        private void SendCommand(string s, int timeout = 1000)
        {
            SendRawCommand(s);
            ReadPosition(timeout);
        }

        private void SendCommandAsync(string s)
        {
            SendRawCommand(s);
        }

        private void SendRawCommand(string s)
        {
            m_port.Write($": {s} #");
        }

        private void ReadPosition(int timeout)
        {
            var text = ReadTextTimeout(timeout);

            if (string.IsNullOrWhiteSpace(text))
                throw new Exception("No reply from focuser");

            ProcessPosition(text);
        }
        // returns true if stationary
        private bool ProcessPosition(string text)
        {
            text = text.Trim();

            switch (text)
            {
                case "OK":
                    return true;
                case "ERR":
                    throw new Exception("Error reply from focuser");
                case string s when s.StartsWith("P ") && s.Length > 2:
                    var numval = s.Split(' ')[1].Trim();
                    if (int.TryParse(numval, out var position))
                    {
                        Position = position;
                        return true;
                    }

                    throw new Exception($"Could not understand position reply from focuser : {s}");


                case string s when s.StartsWith("M ") && s.Length > 2:
                    var numval2 = s.Split(' ')[1].Trim();
                    if (int.TryParse(numval2, out var position2))
                    {
                        Position = position2;
                    }

                    return false;

                default:
                    throw new Exception($"Unexpected reply from focuser : {text}");
            }
        }

        private string ReadTextTimeout(int timeout)
        {

            string text = null;
            SpinWait.SpinUntil(() => _readLines.TryDequeue(out text), timeout);

            return text;
        }

        public void MoveTo(int val)
        {
            IsMoving = true;
            SendCommandAsync($"M {val}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

}

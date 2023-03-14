// This file is part of SharpCap, a C# Video Capture Application for Windows
//
//    Copyright (c) 2010-2016, Robin Glover
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
using System.Runtime.InteropServices;
using System.Threading;
using ASCOM.Interface;
using SharpCap.Base;

namespace TestApp
{
    public interface IFocusAdaptor : IDisposable
    {
        int Maximum { get; }
        int Current { get; }
        bool IsMoving { get; }
        int MaxChange { get; }
        event EventHandler<EventArgs> MovingChanged;
        event EventHandler<EventArgs> PositionChanged;
        int Move(int target);
        void Stop();

        string GetName();

        void DoSetup();

        bool Connected { get; set; }
    }


    internal class FocusAdaptor : IFocusAdaptor
    {
        private IFocuser _focuser;
        private readonly object _lock = new object();
        private readonly string _name;
        private bool _absolute;
        private volatile bool _stopThread;
        private volatile int _iCurrent;
        private int _iTarget;
        private volatile bool _isMoving;
        private Thread _thMovementMonitor;
        private bool _alreadySetup;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public FocusAdaptor(string id, string name)
        {
            Tracer.TraceStart(LogLevel.Debug);
            _name = name;

            Tracer.Trace(LogLevel.Info, "Creating ASCOM Focuser of type {0} ('{1}')", id, name);
            object oFocuser = Activator.CreateInstance(Type.GetTypeFromProgID(id, true));
            IFocuser focuser = oFocuser as IFocuser ?? new FocuserDispatchProxy(oFocuser);
          
            _focuser = focuser;

            Tracer.TraceEnd(LogLevel.Debug);
        }

        #region IFocusAdaptor Members

        public event EventHandler<EventArgs> MovingChanged;
        public event EventHandler<EventArgs> PositionChanged;


        public int Maximum { get; private set; }

        public int Current
        {
            get { return _iCurrent; }
        }

        public int Move(int target)
        {
            try
            {

                int offset = Math.Abs(_iCurrent - target);
                offset = Math.Min(offset, MaxChange);

                int realTarget = _iCurrent + offset*(target < _iCurrent ? -1 : 1);
                realTarget = Math.Max(0, realTarget);
                realTarget = Math.Min(Maximum, realTarget);

                _iTarget = realTarget;

                lock (_lock)
                {
                    _focuser.Move(_absolute ? realTarget : realTarget - _iCurrent);
                    _isMoving = true;
                    FireMovingChangedEvent();
                }

                return realTarget;
            }
            catch (Exception e)
            {
                Tracer.TraceException("Moving Focuser", e);
                throw;
            }

        }

        public void Stop()
        {
            lock (_lock)
            {
                _focuser.Halt();
                _isMoving = false;
                FireMovingChangedEvent();
            }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
        }

        public void Dispose()
        {
            ShutdownMonitorThread();

            if (_focuser == null)
                return;

            try
            {
                _focuser.Link = false;
            }
            catch (Exception e)
            {
                Tracer.TraceException("Trying to unlink from focuser", e);
            }

            if (Marshal.IsComObject(_focuser))
                Marshal.ReleaseComObject(_focuser);
            else if (_focuser is IDisposable)
                (_focuser as IDisposable).Dispose();
            _focuser = null;
        }

        private void ShutdownMonitorThread()
        {
            if (_thMovementMonitor != null)
            {
                _stopThread = true; // shutdown monitor thread
                _thMovementMonitor.Join();
                _thMovementMonitor = null;
                _stopThread = false;
            }
        }

        public int MaxChange { get; private set; }

        public string GetName()
        {
            return _name;
        }

        public void DoSetup()
        {
            _focuser.SetupDialog();
        }

        public bool Connected
        {
            get { return _focuser.Link; }
            set
            {
                if (_focuser.Link != value)
                {
                    try
                    {
                        _focuser.Link = value;

                        if (value && !_focuser.Link)
                            throw new InvalidOperationException("Focuser not connected");

                        if (value)
                        {
                            Setup();
                        }
                        else
                        {
                            ShutdownMonitorThread();
                        }
                    }
                    catch (Exception e)
                    {
                        Tracer.TraceException(value ? "Connecting Focuser " : "Disconnecting Focuser", e);
                        throw;
                        //MessageBox.Show("Focuser not connected because:\r\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        private void MonitorMovement()
        {
            try
            {

                while (!_stopThread)
                {
                    lock (_lock)
                    {
                        bool old = _isMoving;
                        var focuser = _focuser;
                        _isMoving = focuser.IsMoving;
                        if (old != _isMoving)
                        {
                            FireMovingChangedEvent();
                        }


                        int iNewPos = _absolute ? focuser.Position : _iTarget;
                        if (iNewPos != _iCurrent)
                        {
                            _iCurrent = iNewPos;
                            if (PositionChanged != null)
                                PositionChanged.Invoke(this, new EventArgs());
                        }

                    }



                    Thread.Sleep(500);
                }
            }
            catch (Exception)
            {
            }
        }


        private void Setup()
        {
            if (!_alreadySetup)
            {
                _alreadySetup = true;
                MaxChange = _focuser.MaxIncrement;
                Maximum = _focuser.MaxStep;
                if (_focuser.Absolute)
                {
                    _absolute = true;
                    _iCurrent = _focuser.Position;
                }
                else
                {
                    if (Maximum == 0)
                        Maximum = 5000;

                    Maximum = Math.Max(Maximum, MaxChange*10);
                    _iTarget = _iCurrent = Maximum/2;
                }
            }

            if (_thMovementMonitor == null)
            {
                _thMovementMonitor = new Thread(MonitorMovement) {IsBackground = true, Name = "Monitor Focuser Movement"};
                _thMovementMonitor.Start();
            }

        }

        private void FireMovingChangedEvent()
        {
            if (MovingChanged != null)
                MovingChanged.Invoke(this, new EventArgs());
        }
    }
}
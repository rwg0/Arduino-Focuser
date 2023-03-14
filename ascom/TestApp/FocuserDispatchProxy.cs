using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ASCOM.Interface;

namespace TestApp
{
    internal class FocuserDispatchProxy : IFocuser, IDisposable
    {
        private object _impl;

        public FocuserDispatchProxy(object oDispObj)
        {
            _impl = oDispObj;
        }

        #region IFocuser Members

        public bool Absolute
        {
            get { return GetProperty<bool>("Absolute"); }
        }

        public void Halt()
        {
            CallMethod("Halt");
        }

        public bool IsMoving
        {
            get { return GetProperty<bool>("IsMoving"); }
        }

        public bool Link
        {
            get { return GetProperty<bool>("Link"); }

            set { SetProperty("Link", value); }
        }


        public int MaxIncrement
        {
            get { return GetProperty<int>("MaxIncrement"); }
        }

        public int MaxStep
        {
            get { return GetProperty<int>("MaxStep"); }
        }

        public void Move(int val)
        {
            CallMethod("Move", val);
        }

        public int Position
        {
            get { return GetProperty<int>("Position"); }
        }

        public void SetupDialog()
        {
            CallMethod("SetupDialog");
        }

        public double StepSize
        {
            get { return GetProperty<double>("StepSize"); }
        }

        public bool TempComp
        {
            get { return GetProperty<bool>("TempComp"); }
            set { SetProperty("TempComp", value); }
        }

        public bool TempCompAvailable
        {
            get { return GetProperty<bool>("TempCompAvailable"); }
        }

        public double Temperature
        {
            get { return GetProperty<double>("Temperature"); }
        }

        #endregion

        private T GetProperty<T>(string name)
        {
            try
            {
                object result = _impl.GetType().InvokeMember(name, BindingFlags.GetProperty, null, _impl, new object[0]);
                return (T) result;
            }
            catch (Exception e)
            {
                Tracer.TraceException("Getting IDispatch property : " + name, e);
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        private void SetProperty<T>(string name, T value)
        {
            try
            {
                _impl.GetType().InvokeMember(name, BindingFlags.SetProperty, null, _impl, new object[] {value});
            }
            catch (Exception e)
            {
                Tracer.TraceException("Setting IDispatch property : " + name, e);
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        private void CallMethod(string name, params object[] oParams)
        {
            try
            {
                _impl.GetType().InvokeMember(name, BindingFlags.InvokeMethod, null, _impl, oParams);
            }
            catch (Exception e)
            {
                Tracer.TraceException("Calling IDispatch method : " + name, e);
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        public void Dispose()
        {
            if (_impl != null && Marshal.IsComObject(_impl))
            {
                Marshal.ReleaseComObject(_impl);
            }
            _impl = null;
        }
    }
}
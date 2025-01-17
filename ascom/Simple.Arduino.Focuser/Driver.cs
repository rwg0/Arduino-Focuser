//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Focuser driver for Simple.Arduino.Focuser
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM Focuser interface version: 1.0
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	1.0.0	Initial edit, from ASCOM Focuser Driver template
// --------------------------------------------------------------------------------
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ASCOM.DeviceInterface;

using System.Diagnostics;
using System.Reflection;
using ASCOM.Utilities;

namespace ASCOM.Simple.Arduino.Focuser
{
    //
    // Your driver's ID is ASCOM.Simple.Arduino.Focuser
    //
    // The Guid attribute sets the CLSID for ASCOM.Simple.Arduino.Focuser
    // The ClassInterface/None addribute prevents an empty interface called
    // _Focuser from being created and used as the [default] interface
    //
    [ComVisible(true)]
    [Guid("BAAFB8F6-D39B-476B-9519-32FBBB9A1A17")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Focuser :  IFocuserV2
    {
        //
        // Driver ID and descriptive string that shows in the Chooser
        //
        private static string s_csDriverID = "ASCOM.Simple.Arduino.Focuser.Focuser";
        private static string s_csDriverDescription = "Simple Arduino Focuser";


        FocusController _controller;
        Profile _profile;
        private bool _reversed;
        private int _speed;

        //
        // Constructor - Must be public for COM registration!
        //
        public Focuser()
        {
            _profile = new Profile();
            _profile.DeviceType = "Focuser";
            SetFlags();
        }

        #region ASCOM Registration
        //
        // Register or unregister driver for ASCOM. This is harmless if already
        // registered or unregistered. 
        //
        private static void RegUnregASCOM(bool bRegister)
        {
            Profile P = new Profile();
            P.DeviceType = "Focuser";					//  Requires Helper 5.0.3 or later
            if (bRegister)
                P.Register(s_csDriverID, s_csDriverDescription);
            else
                P.Unregister(s_csDriverID);
            try // In case Helper becomes native .NET
            {
                P.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
            //P = null;
        }

        [ComRegisterFunction]
        public static void RegisterASCOM(Type t)
        {
            RegUnregASCOM(true);
        }

        [ComUnregisterFunction]
        public static void UnregisterASCOM(Type t)
        {
            RegUnregASCOM(false);
        }
        #endregion


        #region IFocuser Members

        public ArrayList SupportedActions { get; } = new ArrayList();

        public bool Absolute => true;

        public void Dispose()
        {
            CleanupController();
        }

        private void CleanupController()
        {
            if (_controller != null)
            {
                if (_controller.Position != 0)
                    SavePosition(_controller.Position);
                _controller.PropertyChanged -= ControllerOnPropertyChanged;
                _controller?.Dispose();
                _controller = null;
            }
        }

        public void Halt()
        {
            if (!Link)
                throw new InvalidOperationException("Focuser link not activated");
            _controller.Halt();
        }

        public bool IsMoving => _controller.IsMoving;

        public bool Link
        {
            get => (_controller != null);
            set
            {
                if (_controller != null)
                {
                    SetValue("LastPos", _controller.Position.ToString());
                    CleanupController();
                }
                if (value)
                {
                    BuildController();
                }
                else
                {
                    _controller = null;
                }
            }
        }

        private int GetSavedPosition()
        {
            return GetValue("LastPos", 25000);

        }

        private void BuildController()
        {
            SetFlags();
            if (string.IsNullOrEmpty(GetPort()))
            {
                SetupDialog();
                CleanupController();
            }
            try
            {
                _controller = new FocusController(GetPort());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                SetupDialog();
                CleanupController();
                _controller = new FocusController(GetPort());
            }
            _controller.PropertyChanged += ControllerOnPropertyChanged;
            _controller.InitializePosition(GetSavedPosition());
            _controller.SetSpeed(_speed);
            _controller.SetReverse(_reversed);
        }

        private void ControllerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FocusController.IsMoving))
            {
                if (_controller != null && !_controller.IsMoving)
                {
                    SavePosition(_controller.Position);
                }
            }
        }

        private void SavePosition(int position)
        {
            SetValue("LastPos", position.ToString());
        }

        private string GetPort()
        {
            return GetValue("port");
        }

        private void SetPort(string portName)
        {
            SetValue("port", portName);
        }

        public int MaxIncrement => _controller.MaxMove();

        public int MaxStep => 100000;

        public void Move(int val)
        {
            if (!Link)
                throw new InvalidOperationException("Focuser link not activated");

            _controller.MoveTo(val);
        }

        public bool Connected
        {
            get => Link;
            set => Link = value;
        }

        public string Description => "ASCOM driver for Simple Arduino stepper motor focuser";
        public string DriverInfo { get; private set; }
        public string DriverVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public short InterfaceVersion => 2;
        public string Name => s_csDriverDescription;

        public int Position
        {
            get
            {
                if (!Link)
                    throw new InvalidOperationException("Focuser link not activated");
                return _controller.Position;
            }
        }

        public void SetupDialog()
        {
            var initialPosition = _controller?.Position ?? GetSavedPosition();
            SetFlags();
            SetupDialogForm sf = new SetupDialogForm(GetPort(), _reversed, initialPosition, _speed);
            if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetPort(sf.GetSelectedPort());
                SetValue("reverse", sf.IsReversed().ToString());
                SetValue("speed", sf.Speed.ToString());
                if (_controller != null)
                {
                    _controller.InitializePosition(sf.Position);
                    CleanupController();
                    BuildController();
                }
                else
                {
                    SavePosition(sf.Position);
                }
            }
        }

        public string Action(string ActionName, string ActionParameters)
        {
            throw new System.NotImplementedException();
        }

        public void CommandBlind(string Command, bool Raw = false)
        {
            throw new System.NotImplementedException();
        }

        public bool CommandBool(string Command, bool Raw = false)
        {
            throw new System.NotImplementedException();
        }

        public string CommandString(string Command, bool Raw = false)
        {
            throw new System.NotImplementedException();
        }

        private void SetValue(string key, string value)
        {
            _profile.WriteValue(s_csDriverID, key, value ?? "", "");
        }

        private string GetValue(string key, string defaultVal = "")
        {
            return _profile.GetValue(s_csDriverID, key, defaultVal);
        }

        private int GetValue(string key, int defaultVal)
        {
            var valText = GetValue(key);
            if (string.IsNullOrWhiteSpace(valText))
                return defaultVal;

            if (int.TryParse(valText, out var value))
                return value;

            return defaultVal;
        }

        private void SetFlags()
        {
            _reversed = GetValue("reverse") == "True";
            _speed = GetValue("speed", 200);
        }



        public double StepSize => 1;

        public bool TempCompAvailable => false;

        #endregion


        //
        // PUBLIC COM INTERFACE IFocuser IMPLEMENTATION
        //

        #region IFocuser Members


        public bool TempComp
        {
            get => false;
            set => throw new PropertyNotImplementedException("TempComp", true);
        }

        public double Temperature => throw
            // TODO Replace this with your implementation
            new PropertyNotImplementedException("Temperature", false);

        #endregion
    }
}

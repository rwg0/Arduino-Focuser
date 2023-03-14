using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;


namespace ASCOM.Simple.Arduino.Focuser
{
    [RunInstaller(true)]
    public partial class COMInstaller : Installer
    {
        public COMInstaller()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            RegistrationServices regSrv = new RegistrationServices();
            regSrv.RegisterAssembly(Assembly.GetExecutingAssembly(), AssemblyRegistrationFlags.SetCodeBase);

            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                var rt = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                rt = rt.Replace("Framework\\", "Framework64\\");
                var regasm = Path.Combine(rt, "RegAsm.exe");
                var cmdLine = string.Format("/codebase \"{1}\"", regasm, Assembly.GetExecutingAssembly().Location);
                Process.Start(regasm, cmdLine).WaitForExit(5000);
            }

        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.UnregisterAssembly(Assembly.GetExecutingAssembly());

            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                var rt = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                rt = rt.Replace("Framework\\", "Framework64\\");
                var regasm = Path.Combine(rt, "RegAsm.exe");
                var cmdLine = string.Format("/unregister \"{1}\"", regasm, Assembly.GetExecutingAssembly().Location);
                Process.Start(regasm, cmdLine).WaitForExit(5000);
            }

        }

    }
}

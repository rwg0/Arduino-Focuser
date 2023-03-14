// This file is part of SharpCap, a C# Video Capture Application for Windows
//
//    Copyright (c) 2010-2011, Robin Glover
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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace SharpCap.Base
{
    public class TraceEventArgs : EventArgs
    {
        public string Message;
    }

    public enum LogLevel
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error
    }

    

    public class Tracer
    {
        public static void TraceStart(LogLevel level)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            MethodBase mb = sf.GetMethod();
            TraceImpl(level, MakeMethodName(mb) + " :: Started");
        }

        public static void TraceEnd(LogLevel level)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            MethodBase mb = sf.GetMethod();
            TraceImpl(level, MakeMethodName(mb) + " :: Ended");
        }

        public static void TraceEnd(LogLevel level, string msg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            MethodBase mb = sf.GetMethod();
            TraceImpl(level, MakeMethodName(mb) + " :: Ended (" + msg + ")");
        }

        public static void Trace(LogLevel level, string format, params object[] args)
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat(format, args);
            Trace(level, bld.ToString());
        }




        public static void Trace(LogLevel level, string msg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            MethodBase mb = sf.GetMethod();
            msg = MakeMethodName(mb) + " :: " + msg;
            TraceImpl(level, msg);
        }

        public static event EventHandler<TraceEventArgs> OnTrace;
        private static StringBuilder m_tracerBld = new StringBuilder();

        public static void TraceImpl(LogLevel level, string msg)
        {
            if (level < LogLevel.Debug)
                return;

            msg = level.ToString() + ":\t" + DateTime.Now.TimeOfDay.ToString() + " " + msg;
            System.Diagnostics.Trace.WriteLine(msg);

            if (OnTrace != null)
            {
                if (m_tracerBld.Length > 0)
                {
                    OnTrace(null, new TraceEventArgs() { Message = m_tracerBld.ToString() });
                    m_tracerBld.Length = 0;
                }
                OnTrace(null, new TraceEventArgs() { Message = msg });
            }
            else
            {
                lock (m_tracerBld)
                {
                    m_tracerBld.AppendLine(msg);
                }
            }
        }


        public static void Assert(bool p, string msg)
        {
            if (p)
                return;

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            MethodBase mb = sf.GetMethod();
            msg = MakeMethodName(mb) + " :: Assert Failed : " + msg;
            TraceImpl(LogLevel.Warning, msg);
        }

        private static string MakeMethodName(MethodBase mb)
        {
            return mb.ReflectedType.FullName + "." + mb.Name + "(" + BuildParameters(mb.GetParameters()) + ")";
        }

        private static string BuildParameters(ParameterInfo[] parameterInfo)
        {
            StringBuilder bld = new StringBuilder();
            foreach (ParameterInfo pi in parameterInfo)
            {
                bld.Append(pi.ParameterType.Name + " " + pi.Name);
                bld.Append(", ");

            }
            if (bld.Length>2)
                bld.Remove(bld.Length - 2, 2);
            return bld.ToString();
        }


        public static void LogIfFailed(int hr, int frameOffset = 0)
        {
            if (hr == 0)
                return;

            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1 + frameOffset);
            MethodBase mb = sf.GetMethod();
            if (hr < 0)
            {
                string msg = MakeMethodName(mb) + " :: Failure HR from COM method : 0x" + hr.ToString("X8"); ;
                TraceImpl(LogLevel.Warning, msg);
            }
            else
            {
                string msg = MakeMethodName(mb) + " :: non S_OK HR from COM method : 0x" + hr.ToString("X8"); ;
                TraceImpl(LogLevel.Warning, msg);
            }
        }

        public static void TraceException(string action, Exception e, string additionalInfo = null)
        {
            Trace(LogLevel.Error, "Exception from {3} : {0} \r\nStack Trace:{1}\r\nExtra Info:{2}", e.Message, e.StackTrace, additionalInfo, action);
            if (e.InnerException != null)
                Trace(LogLevel.Error, "Inner Exception : " + e.InnerException.ToString());
        }

        public static void TraceWithStackTrace(LogLevel level, string msg)
        {
            StackTrace st = new StackTrace(1);
            Tracer.Trace(level, msg + "\n" + st.ToString());
        }

        public static void Assert(bool p)
        {
            if (!p)
                TraceWithStackTrace(LogLevel.Warning, "Assertion failed");
        }
    }
}

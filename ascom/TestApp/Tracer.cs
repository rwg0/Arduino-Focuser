
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace TestApp
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

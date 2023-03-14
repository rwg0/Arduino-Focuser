using System;

namespace ASCOM.Simple.Arduino.Focuser
{
    class ThreadControl
    {
        volatile bool destroyed = false;
        volatile bool isMoving = false;
        volatile int command = 0;
        volatile bool halt = false;
        object m_lock = new Object();

        public bool Halt
        {
            get
            {
                lock (m_lock)
                {
                    return halt;
                }
            }
            set
            {
                lock (m_lock)
                {
                    halt = value;
                }
            }
        }

        public bool Moving
        {
            get
            {
                lock (m_lock)
                {
                    return isMoving;
                }
            }
            set
            {
                lock (m_lock)
                {
                    isMoving = value;
                }
            }
        }

        public bool Destroyed
        {
            get
            {
                lock (m_lock)
                {
                    return destroyed;
                }
            }
            set
            {
                lock (m_lock)
                {
                    destroyed = value;
                }
            }
        }

        public int Command
        {
            get
            {
                lock (m_lock)
                {
                    return command;
                }
            }
            set
            {
                lock (m_lock)
                {
                    command = value;
                }
            }
        }


    }
}
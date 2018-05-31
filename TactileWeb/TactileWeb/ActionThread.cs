using System;
using System.Diagnostics;
using System.Threading;


namespace TactileWeb
{
    public delegate void Callback();


    /// <summary>Start and Stop Thread</summary>
    public class ActionThread
    {
        protected Thread                _thread;        // Thread for testing
        protected ManualResetEvent      _mre;           // Thread.Sleep
        protected Callback              _callback;      // Event
        protected ThreadPriority        _priority;
        protected string                _name;          // A name to see in debugging
        protected ActionThreadState     _state;

        /// <summary>Creates a new Thread</summary>
        public ActionThread(Callback callback, string name) : this (callback, name, ThreadPriority.Normal)
        { 
        }


        /// <summary>Creates a new Thread</summary>
        public ActionThread(Callback callback, string name, ThreadPriority priority)
        { 
            _callback   = callback;
            _name       = name;
            _priority   = priority;

            _mre        = new ManualResetEvent(false);
            _state      = ActionThreadState.Stopped;
        }






        /// <summary>Starts the thread</summary>
        public void Start()
        { 
            if (_state != ActionThreadState.Stopped)    return;

            _state = ActionThreadState.Starting;
            ThreadPool.QueueUserWorkItem( new WaitCallback( Thread_Event ) );   // --> Thread_Event
        }

        /// <summary>The thread calls this</summary>
        protected void Thread_Event(object o)
        { 
            _state = ActionThreadState.Running;

            _thread             = Thread.CurrentThread;
            _thread.Priority    = _priority;
            _thread.Name        = _name;

            _callback.Invoke();     // --> User Callback
        }






        /// <summary>Stops the thread</summary>
        public void Stop()
        { 
            if ( _thread != null )
            { 
                _state = ActionThreadState.AbortRequested;
                _thread.Abort();    // --> Exception
            }

            _state = ActionThreadState.Stopped;
        }

        /// <summary>Wait and sleep until the next callback</summary>
        public void Sleep (int milliseconds)
        { 
            // Thread.Sleep is not working in aborts!!!
            if ( milliseconds < 0 ) milliseconds = 0;


            if ( _thread != null )
            { 
                _mre.WaitOne(milliseconds);
            }
        }

        /// <summary>Returns the state of the thread</summary>
        public ActionThreadState State { get { return _state; } } 


        /// <summary>Starts or stops the thread</summary>
        public bool     Enable
        {
            get 
            {
                if ( _thread == null )
                {
                    return false;
                }
                else
                {
                    return ( _thread.ThreadState == System.Threading.ThreadState.Running );
                }
            }
            set
            {
                if (value == true)
                { 
                    Start();
                }
                else
                {
                    Stop();
                }

            }
        }


        /// <summary>Information of the object</summary>
        public override string ToString()
        {
            return String.Format("Name={0} State={1} Priority={3}", _name, _state, _priority);
        }

    }
}
using System;
using System.Threading;

namespace FALibrary.Utility
{
    public class TimeCriticalWork : IDisposable
    {
        private class ThreadWrapper
        {
            private static object _syncRoot = new Object();
            private ManualResetEvent _idleStatusEvent = new ManualResetEvent(false);
            private ManualResetEvent _allDone = new ManualResetEvent(false);
            private Action _method = null;
            private bool _result = false;
            Thread _thread;

            public ThreadWrapper()
            {
                _thread = new Thread(
                    delegate(object obj)
                    {
                        while (true)
                        {
                            while (IsIdle())
                            {
                                _idleStatusEvent.Set();
                                _idleStatusEvent.Reset();                                
                                if (_stop) break;
                            }

                            try
                            {
                                if (_method != null)
                                {
                                    _method();
                                    _result = true;
                                }
                                else
                                    _result = false;
                            }
                            catch
                            {
                                _result = false;
                            }

                            _allDone.Set();

                            SetIdle(true);

                            if (_stop) break;
                        }
                    });

                _thread.Priority = ThreadPriority.Highest;
                _thread.Start();
            }

            public bool Execute(Action method, int milliseconds)
            {
                if (_idleStatusEvent.WaitOne(milliseconds) == false)
                    return false;

                if (IsIdle() == false)
                    return false;

                _method = method;
                _allDone.Reset();
                SetIdle(false);

                if (_allDone.WaitOne(milliseconds) == false)
                    return false;

                return _result;
            }

            private bool _stop = false;

            public void Stop()
            {
                _stop = true;
            }

            private bool _isIdle = true;

            public bool IsIdle()
            {
                lock (_syncRoot)
                {
                    return _isIdle;
                }
            }

            private void SetIdle(bool flag)
            {
                lock (_syncRoot)
                {
                    _isIdle = flag;
                }
            }
        }        

        private ThreadWrapper tw = new ThreadWrapper();

        public TimeCriticalWork()
        {
        }

        public bool Execute(Action method, int milliseconds)
        {
            if (tw.IsIdle() == false)
            {
                tw.Stop();
                tw = new ThreadWrapper();
            }

            return tw.Execute(method, milliseconds);
        }

        public void Dispose()
        {
            if (tw != null)
                tw.Stop();
        }
    }    
}

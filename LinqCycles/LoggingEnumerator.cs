using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqCycles
{
    public class LoggingEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        private readonly Action<string> _log;

        public LoggingEnumerator(IEnumerator<T> enumerator, Action<string> log)
        {
            _enumerator = enumerator;
            _log = log;
        }

        public void Dispose()
        {
            _log("Dispose");
            _enumerator.Dispose();
        }

        public bool MoveNext()
        {
            _log("MoveNext");
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _log("Reset");
            _enumerator.Reset();
        }

        public T Current
        {
            get
            {
                _log("get_Current");
                return _enumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
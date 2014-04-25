using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqCycles
{
    public class LoggingEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;
        private readonly Action<string> _log;

        public LoggingEnumerable(IEnumerable<T> enumerable, Action<string> log)
        {
            _enumerable = enumerable;
            _log = log;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LoggingEnumerator<T>(_enumerable.GetEnumerator(), _log);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
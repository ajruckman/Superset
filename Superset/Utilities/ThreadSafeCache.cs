using System;
using System.Diagnostics.CodeAnalysis;

namespace Superset.Utilities
{
    public class ThreadSafeCache<T> where T : class
    {
        public delegate T ValueGetter();

        private readonly object _lock = new();

        private T?   _value;
        private bool _hasSet;

        public bool HasValue
        {
            get
            {
                lock (_lock)
                {
                    return _hasSet;
                }
            }
        }

        public T Get()
        {
            lock (_lock)
            {
                if (!_hasSet)
                    throw new InvalidOperationException("Call to ThreadSafeCache.Get() with unset value.");
                return _value!;
            }
        }

        public bool TryGet([NotNullWhen(returnValue: true)] out T? result)
        {
            lock (_lock)
            {
                if (_hasSet)
                {
                    result = _value!;
                    return true;
                }

                result = null;
                return false;
            }
        }

        public T Set(T newValue)
        {
            lock (_lock)
            {
                _hasSet = true;
                return _value = newValue;
            }
        }

        public T SetIf(ValueGetter valueGetter)
        {
            lock (_lock)
            {
                _hasSet = true;
                return _value ??= valueGetter.Invoke();
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _hasSet = false;
                _value  = null;
            }
        }
    }
}
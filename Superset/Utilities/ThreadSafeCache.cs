using System;

namespace Superset.Utilities
{
    public class ThreadSafeCache<T> where T : class?
    {
        private readonly object _lock  = new object();
        private          T      _value = default!;
        private          bool   _hasSet;

        public bool HasValue
        {
            get
            {
                lock (_lock)
                {
                    return _value != null || _hasSet;
                }
            }
        }

        public T Get()
        {
            lock (_lock)
            {
                if (_value == null)
                    throw new InvalidOperationException("Call to ThreadSafeCache.Get() with unset value.");
                return _value;
            }
        }

        public T Set(T newValue, bool final = false)
        {
            lock (_lock)
            {
                if (final) _hasSet = true;
                return _value = newValue;
            }
        }

        public delegate T ValueGetter();

        public T SetIf(ValueGetter valueGetter, bool final = false)
        {
            lock (_lock)
            {
                if (final) _hasSet = true;
                return _value ??= valueGetter.Invoke();
            }
        }

        public void Invalidate()
        {
            lock (_lock)
            {
                _hasSet = false;
                _value  = default!;
            }
        }
    }
}
using System.Timers;

namespace Superset.Utilities
{
    public class Debouncer<T>
    {
        public delegate void OnElapsed(T newValue);

        private readonly Timer     _debouncer;
        private readonly OnElapsed _onElapsed;
        private readonly object    _valueLock = new object();

        public Debouncer(OnElapsed onElapsed, T initialValue, int milliseconds = 200)
        {
            _onElapsed = onElapsed;
            _debouncer = new Timer(milliseconds);
            _debouncer.Stop();
            _debouncer.AutoReset = false;
            _debouncer.Elapsed += (_, __) =>
            {
                lock (_valueLock)
                {
                    _onElapsed.Invoke(Value);
                }
            };
        }

        public T Value { get; private set; }

        public void Reset(T newValue)
        {
            _debouncer.Stop();
            lock (_valueLock)
            {
                Value = newValue;
            }

            _debouncer.Start();
        }
    }
}
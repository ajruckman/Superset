using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Superset.Utilities
{
    public class FaultAwareRateLimiter<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Dictionary<string, Dictionary<TIdentifier, Timeout>> _calls =
            new Dictionary<string, Dictionary<TIdentifier, Timeout>>();

        private readonly uint   _baseTimeoutSeconds;
        private readonly uint   _maxTimeoutSeconds;
        private readonly double _multiplier;

        public string DefaultMaxTimeoutNotice =>
            "Reached max possible timeout of FaultAwareRateLimiter for data with unique ID.";

        public FaultAwareRateLimiter
        (
            uint   baseTimeoutSeconds = 1,
            uint   maxTimeoutSeconds  = 1800,
            double multiplier         = 1.25
        )
        {
            _baseTimeoutSeconds = baseTimeoutSeconds;
            _maxTimeoutSeconds  = maxTimeoutSeconds;
            _multiplier         = multiplier;
        }

        public event Action<(string CallerID, TIdentifier UniqueID, Exception Fault)>? OnMaxTimeoutReached;

        public void Try
        (
            TIdentifier               uniqueID,
            Action                    action,
            Action                    onSuccess,
            Action<Exception>         onFailure,
            [CallerMemberName] string sourceName = "",
            [CallerFilePath]   string sourceFile = "",
            [CallerLineNumber] int    sourceLine = 0
        )
        {
            string caller = $"{sourceName}@{sourceFile}:{sourceLine}";

            long now = DateTime.Now.Ticks;

            if (GetCall(caller, uniqueID, out Timeout timeout))
            {
                if (now >= timeout.Next)
                {
                    try
                    {
                        action.Invoke();
                        HandleSuccess(caller, uniqueID);
                        onSuccess.Invoke();
                    }
                    catch (Exception e)
                    {
                        HandleFailure(caller, uniqueID, now, timeout, e);
                        onFailure.Invoke(e);
                    }
                }
                else
                {
                    Console.WriteLine($"Timed out attempt to call: " + caller);
                }
            }
            else
            {
                try
                {
                    action.Invoke();
                    Console.WriteLine("Initial action called successfully: " + caller);
                    onSuccess.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Initial action failed: " + caller);
                    AddCall(caller, uniqueID, now, _baseTimeoutSeconds, e.Message);
                    onFailure.Invoke(e);
                }
            }
        }

        public void Try<TResult>
        (
            TIdentifier               uniqueID,
            Func<TResult>             action,
            Action<TResult>           onSuccess,
            Action<Exception>         onFailure,
            [CallerMemberName] string sourceName = "",
            [CallerFilePath]   string sourceFile = "",
            [CallerLineNumber] int    sourceLine = 0
        )
        {
            string caller = $"{sourceName}@{sourceFile}:{sourceLine}";

            long now = DateTime.Now.Ticks;

            if (GetCall(caller, uniqueID, out Timeout timeout))
            {
                if (now >= timeout.Next)
                {
                    try
                    {
                        TResult result = action.Invoke();
                        HandleSuccess(caller, uniqueID);
                        onSuccess.Invoke(result);
                    }
                    catch (Exception e)
                    {
                        HandleFailure(caller, uniqueID, now, timeout, e);
                        onFailure.Invoke(e);
                    }
                }
                else
                {
                    Console.WriteLine($"Timed out attempt to call: " + caller);
                }
            }
            else
            {
                try
                {
                    TResult result = action.Invoke();
                    Console.WriteLine("Initial action called successfully: " + caller);
                    onSuccess.Invoke(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Initial action failed: " + caller);
                    AddCall(caller, uniqueID, now, _baseTimeoutSeconds, e.Message);
                    onFailure.Invoke(e);
                }
            }
        }

        public TOutput Try<TResult, TOutput>
            (
                TIdentifier               uniqueID,
                Func<TResult>             action,
                Func<TResult, TOutput>    onSuccess,
                Func<Exception, TOutput>  onFailure,
                [CallerMemberName] string sourceName = "",
                [CallerFilePath]   string sourceFile = "",
                [CallerLineNumber] int    sourceLine = 0
            )
            // where TOutput : struct
        {
            string caller = $"{sourceName}@{sourceFile}:{sourceLine}";

            long now = DateTime.Now.Ticks;

            if (GetCall(caller, uniqueID, out Timeout timeout))
            {
                if (now >= timeout.Next)
                {
                    try
                    {
                        TResult result = action.Invoke();
                        HandleSuccess(caller, uniqueID);
                        return onSuccess.Invoke(result);
                    }
                    catch (Exception e)
                    {
                        HandleFailure(caller, uniqueID, now, timeout, e);
                        return onFailure.Invoke(e);
                    }
                }
                else
                {
                    Console.WriteLine($"Timed out attempt to call: " + caller);
                    return default!;
                }
            }
            else
            {
                try
                {
                    TResult result = action.Invoke();
                    Console.WriteLine("Initial action called successfully: " + caller);
                    return onSuccess.Invoke(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Initial action failed: " + caller);
                    AddCall(caller, uniqueID, now, _baseTimeoutSeconds, e.Message);
                    return onFailure.Invoke(e);
                }
            }
        }

        private uint GetTimeoutSeconds
        (
            string      caller,
            TIdentifier uniqueID,
            Timeout     t,
            Exception   e
        )
        {
            var seconds = (uint) (t.Seconds * _multiplier);

            if (seconds > _maxTimeoutSeconds)
            {
                OnMaxTimeoutReached?.Invoke((caller, uniqueID, e));
                // Common.Logger.Error
                // (
                //     "ExceptionRateLimiter call hit the timeout ceiling for an exception.",
                //     meta: new Fields
                //     {
                //         {"UniqueID", uniqueID},
                //         {"ID", id},
                //         {"Message", m},
                //     }
                // );
                seconds = _maxTimeoutSeconds;
            }
            else if (seconds == t.Seconds)
            {
                seconds += 1;
            }

            return seconds;
        }

        private bool GetCall(string caller, TIdentifier uniqueID, [MaybeNullWhen(false)] out Timeout t)
        {
            if (_calls.TryGetValue(caller, out Dictionary<TIdentifier, Timeout>? timeouts))
            {
                if (timeouts.TryGetValue(uniqueID, out Timeout timeout))
                {
                    t = timeout;
                    return true;
                }
            }

            t = default;
            return false;
        }

        private void AddCall(string caller, TIdentifier uniqueID, long now, uint seconds, string m)
        {
            if (!_calls.TryGetValue(caller, out Dictionary<TIdentifier, Timeout>? timeouts))
            {
                timeouts       = new Dictionary<TIdentifier, Timeout>();
                _calls[caller] = timeouts;
            }

            timeouts[uniqueID] = new Timeout
            (
                seconds,
                now + (TimeSpan.TicksPerSecond * seconds),
                m
            );
        }

        private void RemoveCall(string caller, TIdentifier uniqueID)
        {
            if (_calls.TryGetValue(caller, out Dictionary<TIdentifier, Timeout>? timeouts))
            {
                timeouts.Remove(uniqueID);

                if (timeouts.Count == 0)
                {
                    _calls.Remove(caller);
                }
            }
        }

        private void HandleSuccess(string caller, TIdentifier uniqueID)
        {
            Console.WriteLine($"Action called successfully: {caller} for {uniqueID}");
            RemoveCall(caller, uniqueID);
        }

        private void HandleFailure
        (
            string      caller,
            TIdentifier uniqueID,
            long        now,
            Timeout     t,
            Exception   e
        )
        {
            Console.WriteLine($"Action failed: {caller} for {uniqueID}");

            if (t.Exception == e.Message)
            {
                uint seconds = GetTimeoutSeconds(caller, uniqueID, t, e);
                Console.WriteLine($"New timeout: {seconds}");

                AddCall(caller, uniqueID, now, seconds, e.Message);
            }
            else
            {
                AddCall(caller, uniqueID, now, _baseTimeoutSeconds, e.Message);
            }
        }

        private readonly struct Timeout
        {
            internal readonly uint   Seconds;
            internal readonly long   Next;
            internal readonly string Exception;

            public Timeout(uint seconds, long next, string exception)
            {
                Seconds   = seconds;
                Next      = next;
                Exception = exception;
            }
        }
    }
}
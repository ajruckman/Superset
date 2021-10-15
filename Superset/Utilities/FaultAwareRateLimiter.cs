using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Superset.Utilities
{
    public interface IFaultableResult { }

    // ReSharper disable once UnusedTypeParameter
    public interface IFaultableResult<TResult> { }

    public class FaultableResultSuccess<TResult> : IFaultableResult<TResult>
    {
        public FaultableResultSuccess(TResult value)
        {
            Value = value;
        }

        public TResult Value { get; }
    }

    public class FaultableResultSuccess : IFaultableResult { }

    public class FaultableResultFailure : IFaultableResult
    {
        public FaultableResultFailure(Exception exception, TimeSpan? minTimeoutSeconds)
        {
            Exception         = exception;
            MinTimeoutSeconds = minTimeoutSeconds;
        }

        public Exception Exception         { get; }
        public TimeSpan? MinTimeoutSeconds { get; }
    }

    public class FaultableResultFailure<TResult> : IFaultableResult<TResult>
    {
        public FaultableResultFailure(Exception exception, TimeSpan? minTimeoutSeconds)
        {
            Exception         = exception;
            MinTimeoutSeconds = minTimeoutSeconds;
        }

        public Exception Exception         { get; }
        public TimeSpan? MinTimeoutSeconds { get; }
    }

    // public class FaultableResult<TResult>
    // {
    //     private readonly bool _succeeded;
    //
    //     private FaultableResult(TResult value)
    //     {
    //         _succeeded = true;
    //         Value      = value;
    //     }
    //
    //     private FaultableResult(Exception e, TimeSpan retryAfter)
    //     {
    //         _succeeded = false;
    //         Exception  = e;
    //         RetryAfter = retryAfter;
    //     }
    //
    //     public TResult?   Value      { get; }
    //     public Exception? Exception  { get; }
    //     public TimeSpan?  RetryAfter { get; }
    //
    //     public bool Succeeded([MaybeNullWhen(false)] out TResult value)
    //     {
    //         if (_succeeded)
    //         {
    //             value = Value!;
    //             return true;
    //         }
    //
    //         value = default;
    //         return false;
    //     }
    //
    //     public FaultableResult<TResult> Success(TResult   value)                  => new(value);
    //     public FaultableResult<TResult> Failure(Exception e, TimeSpan retryAfter) => new(e, retryAfter);
    // }

    public class FaultAwareRateLimiter<TIdentifier>
        where TIdentifier : IEquatable<TIdentifier>
    {
        private readonly Dictionary<string, Dictionary<TIdentifier, Timeout>> _calls = new();

        private readonly TimeSpan _baseTimeoutSeconds;
        private readonly TimeSpan _maxTimeoutSeconds;
        private readonly double   _multiplier;

        public FaultAwareRateLimiter
        (
            TimeSpan? baseTimeoutSeconds = null,
            TimeSpan? maxTimeoutSeconds  = null,
            double    multiplier         = 1.25
        )
        {
            _baseTimeoutSeconds = baseTimeoutSeconds ?? TimeSpan.FromSeconds(1);
            _maxTimeoutSeconds  = maxTimeoutSeconds  ?? TimeSpan.FromSeconds(1800);
            _multiplier         = multiplier;
        }

        public string DefaultMaxTimeoutNotice =>
            "Reached max possible timeout of FaultAwareRateLimiter for data with unique ID.";

        public event Action<(string CallerID, TIdentifier UniqueID, Exception? Fault)>? OnMaxTimeoutReached;

        public void Try
        (
            TIdentifier               uniqueID,
            Func<IFaultableResult>    action,
            Action                    onSuccess,
            Action<Exception?>        onFailure,
            [CallerMemberName] string sourceName = "",
            [CallerFilePath]   string sourceFile = "",
            [CallerLineNumber] int    sourceLine = 0
        )
        {
            string caller = $"{sourceName}@{sourceFile}:{sourceLine}";

            DateTime now = DateTime.Now;

            if (GetCall(caller, uniqueID, out Timeout lastTimeout))
            {
                if (now >= lastTimeout.Next)
                {
                    IFaultableResult result = action.Invoke();

                    switch (result)
                    {
                        case FaultableResultSuccess:
                            HandleSuccess(caller, uniqueID);
                            onSuccess.Invoke();
                            break;

                        case FaultableResultFailure failure:
                            HandleFailure(caller, uniqueID, now, lastTimeout, failure.Exception);
                            onFailure.Invoke(failure.Exception);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Debug.WriteLine($"Timed out attempt to call: " + caller);
                }
            }
            else
            {
                IFaultableResult result = action.Invoke();

                switch (result)
                {
                    case FaultableResultSuccess:
                        Debug.WriteLine("Initial action called successfully: " + caller);
                        onSuccess.Invoke();
                        break;

                    case FaultableResultFailure failure:
                        Debug.WriteLine("Initial action failed: " + caller);
                        AddCall(caller, uniqueID, now, failure.MinTimeoutSeconds ?? _baseTimeoutSeconds);
                        onFailure.Invoke(failure.Exception);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Try<TResult>
        (
            TIdentifier                     uniqueID,
            Func<IFaultableResult<TResult>> action,
            Action<TResult>                 onSuccess,
            Action<Exception?>              onFailure,
            [CallerMemberName] string       sourceName = "",
            [CallerFilePath]   string       sourceFile = "",
            [CallerLineNumber] int          sourceLine = 0
        )
        {
            string   caller = $"{sourceName}@{sourceFile}:{sourceLine}";
            DateTime now    = DateTime.Now;

            if (GetCall(caller, uniqueID, out Timeout lastTimeout))
            {
                if (now >= lastTimeout.Next)
                {
                    IFaultableResult<TResult> result = action.Invoke();

                    switch (result)
                    {
                        case FaultableResultSuccess<TResult> success:
                            HandleSuccess(caller, uniqueID);
                            onSuccess.Invoke(success.Value);
                            break;

                        case FaultableResultFailure<TResult> failure:
                            HandleFailure(caller, uniqueID, now, lastTimeout, failure.Exception);
                            onFailure.Invoke(failure.Exception);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Debug.WriteLine($"Timed out attempt to call: " + caller);
                }
            }
            else
            {
                IFaultableResult<TResult> result = action.Invoke();

                switch (result)
                {
                    case FaultableResultSuccess<TResult> success:
                        Debug.WriteLine("Initial action called successfully: " + caller);
                        onSuccess.Invoke(success.Value);
                        break;

                    case FaultableResultFailure<TResult> failure:
                        Debug.WriteLine("Initial action failed: " + caller);
                        AddCall(caller, uniqueID, now, failure.MinTimeoutSeconds ?? _baseTimeoutSeconds);
                        onFailure.Invoke(failure.Exception);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public TOutput Try<TResult, TOutput>
        (
            TIdentifier                     uniqueID,
            Func<IFaultableResult<TResult>> action,
            Func<TResult, TOutput>          onSuccess,
            Func<Exception?, TOutput>       onFailure,
            [CallerMemberName] string       sourceName = "",
            [CallerFilePath]   string       sourceFile = "",
            [CallerLineNumber] int          sourceLine = 0
        )
        {
            string   caller = $"{sourceName}@{sourceFile}:{sourceLine}";
            DateTime now    = DateTime.Now;

            if (GetCall(caller, uniqueID, out Timeout timeout))
            {
                if (now >= timeout.Next)
                {
                    IFaultableResult<TResult> result = action.Invoke();

                    switch (result)
                    {
                        case FaultableResultSuccess<TResult> success:
                            HandleSuccess(caller, uniqueID);
                            return onSuccess.Invoke(success.Value);

                        case FaultableResultFailure<TResult> failure:
                            HandleFailure(caller, uniqueID, now, timeout, failure.Exception);
                            return onFailure.Invoke(failure.Exception);

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Debug.WriteLine($"Timed out attempt to call: " + caller);
                    return default!;
                }
            }
            else
            {
                IFaultableResult<TResult> result = action.Invoke();
                switch (result)
                {
                    case FaultableResultSuccess<TResult> success:
                        Debug.WriteLine("Initial action called successfully: " + caller);
                        return onSuccess.Invoke(success.Value);

                    case FaultableResultFailure<TResult> failure:
                        Debug.WriteLine("Initial action failed: " + caller);
                        AddCall(caller, uniqueID, now, failure.MinTimeoutSeconds ?? _baseTimeoutSeconds);

                        return onFailure.Invoke(failure.Exception);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private TimeSpan GetNextTimeout
        (
            string      caller,
            TIdentifier uniqueID,
            Timeout     timeout,
            Exception?  e
        )
        {
            TimeSpan duration = timeout.Duration * _multiplier;

            if (duration > _maxTimeoutSeconds)
            {
                OnMaxTimeoutReached?.Invoke((caller, uniqueID, e));
                duration = _maxTimeoutSeconds;
            }
            else if (duration == timeout.Duration)
            {
                duration += TimeSpan.FromSeconds(1);
            }

            return duration;
        }

        // ReSharper disable once RedundantNullableFlowAttribute
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

        private void AddCall(string caller, TIdentifier uniqueID, DateTime now, TimeSpan timeout)
        {
            if (!_calls.TryGetValue(caller, out Dictionary<TIdentifier, Timeout>? timeouts))
            {
                timeouts       = new Dictionary<TIdentifier, Timeout>();
                _calls[caller] = timeouts;
            }

            timeouts[uniqueID] = new Timeout
            (
                timeout,
                now + timeout
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
            Debug.WriteLine($"Action called successfully: {caller} for {uniqueID}");
            RemoveCall(caller, uniqueID);
        }

        private void HandleFailure
        (
            string      caller,
            TIdentifier uniqueID,
            DateTime    now,
            Timeout     lastTimeout,
            Exception?  e
        )
        {
            Debug.WriteLine($"Action failed: {caller} for {uniqueID}");

            TimeSpan seconds = GetNextTimeout(caller, uniqueID, lastTimeout, e);
            Debug.WriteLine($"New timeout: {seconds}");

            AddCall(caller, uniqueID, now, seconds);
        }

        private readonly struct Timeout
        {
            internal readonly TimeSpan Duration;
            internal readonly DateTime Next;

            public Timeout(TimeSpan duration, DateTime next)
            {
                Duration = duration;
                Next     = next;
            }
        }
    }
}
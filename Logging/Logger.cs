using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Superset.Common;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Superset.Logging
{
    public sealed class Logger
    {
        private readonly List<Message> _history = new List<Message>();
        private          string        _status;

        public UpdateTrigger OnAny     { get; } = new UpdateTrigger();
        public UpdateTrigger OnDebug   { get; } = new UpdateTrigger();
        public UpdateTrigger OnInfo    { get; } = new UpdateTrigger();
        public UpdateTrigger OnWarning { get; } = new UpdateTrigger();
        public UpdateTrigger OnError   { get; } = new UpdateTrigger();

        public UpdateTrigger OnStatusChange { get; } = new UpdateTrigger();

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnStatusChange?.Trigger();
            }
        }

        public Message Debug(string text, Fields meta = null)
        {
            Message message = new Message
            {
                Time  = DateTime.Now,
                Level = MessageLevel.DEBG,
                Text  = text,
                Meta  = meta
            };

            _history.Add(message);
            message.Print(false);
            OnDebug?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public Message Info(string text, Fields meta = null)
        {
            Message message = new Message
            {
                Time  = DateTime.Now,
                Level = MessageLevel.INFO,
                Text  = text,
                Meta  = meta
            };

            _history.Add(message);
            message.Print(false);
            OnInfo?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public Message Warning(string text, Exception e = null, Fields meta = null, bool printStacktrace = false)
        {
            Message message = new Message
            {
                Time      = DateTime.Now,
                Level     = MessageLevel.WARN,
                Text      = text,
                Meta      = meta,
                Exception = e
            };

            _history.Add(message);
            message.Print(printStacktrace);
            OnWarning?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public Message Error(string text, Exception e = null, Fields meta = null, bool printStacktrace = false)
        {
            Message message = new Message
            {
                Time      = DateTime.Now,
                Level     = MessageLevel.ERRR,
                Text      = text,
                Meta      = meta,
                Exception = e
            };

            _history.Add(message);
            message.Print(printStacktrace);
            OnError?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public List<Message> Events()
        {
            return _history;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Fields : OrderedDictionary { }

    public sealed class Message
    {
        public DateTime     Time      { get; set; }
        public MessageLevel Level     { get; set; }
        public string       Text      { get; set; }
        public Fields       Meta      { get; set; }
        public Exception    Exception { get; set; }

        private const string Delimiter       = "     ";
        private const string MultilinePrefix = "                      ";

        public override string ToString()
        {
            string result = Text;

            if (Meta != null && Meta.Count > 0)
                result += Delimiter + FormatMeta(Meta);

            return result;
        }

        public string ToString(bool printStacktrace)
        {
            string result = Text;

            if (Meta != null && Meta.Count > 0)
                result += Delimiter + FormatMeta(Meta);

            if (Exception != null && printStacktrace)
            {
                result += "\n" + MultilinePrefix + "Stacktrace:";

                using StringReader reader = new StringReader(Exception.ToString());

                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    result += "\n" + MultilinePrefix + line;
            }

            return result;
        }

        public void Print(bool printStacktrace)
        {
            string formatted = $"{FormatLevel(Level)} [{DateTime.Now:HH:mm:ss.fff}] {ToString(printStacktrace)}";

            Console.WriteLine(formatted);
            Debug.WriteLine(formatted);
        }

        private static string FormatLevel(MessageLevel level)
        {
            return $"[{level,-4}]";
        }

        private static string FormatMeta(Fields meta)
        {
            return string.Join(' ', meta.Keys.OfType<string>().Select(v => $"[{v}: {meta[v]}]"));
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum MessageLevel
    {
        Undefined,
        DEBG,
        INFO,
        WARN,
        ERRR,
        STAT
    }
}
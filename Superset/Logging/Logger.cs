#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Superset.Web.State;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Superset.Logging
{
    public sealed class Logger
    {
        private readonly List<Message> _history = new List<Message>();
        private          string?       _status;

        public UpdateTrigger OnAny     { get; } = new UpdateTrigger();
        public UpdateTrigger OnDebug   { get; } = new UpdateTrigger();
        public UpdateTrigger OnInfo    { get; } = new UpdateTrigger();
        public UpdateTrigger OnWarning { get; } = new UpdateTrigger();
        public UpdateTrigger OnError   { get; } = new UpdateTrigger();

        public UpdateTrigger OnStatusChange { get; } = new UpdateTrigger();

        public readonly bool   PrintSourceInfo;
        public readonly string ProjectRoot;

        public Logger(bool printSourceInfo = false, string projectRoot = "")
        {
            PrintSourceInfo = printSourceInfo;
            ProjectRoot     = projectRoot;
        }

        public string? Status
        {
            get => _status;
            set
            {
                _status = value;
                OnStatusChange?.Trigger();
            }
        }

        public Message Debug(
            string                    text,
            Fields?                   meta       = null,
            [CallerMemberName] string sourceName = "",
            [CallerFilePath]   string sourceFile = "",
            [CallerLineNumber] int    sourceLine = 0
        )
        {
            Message message = BuildMessage(MessageLevel.DEBG, text, null, meta, sourceName, sourceFile, sourceLine);

            message.Print(false, PrintSourceInfo);
            OnDebug?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public Message Info(
            string                    text,
            Fields?                   meta       = null,
            [CallerMemberName] string sourceName = "",
            [CallerFilePath]   string sourceFile = "",
            [CallerLineNumber] int    sourceLine = 0
        )
        {
            Message message = BuildMessage(MessageLevel.INFO, text, null, meta, sourceName, sourceFile, sourceLine);

            message.Print(false, PrintSourceInfo);
            OnInfo?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public Message Warning(
            string                    text,
            Exception?                e               = null,
            Fields?                   meta            = null,
            bool                      printStacktrace = false,
            [CallerMemberName] string sourceName      = "",
            [CallerFilePath]   string sourceFile      = "",
            [CallerLineNumber] int    sourceLine      = 0
        )
        {
            Message message = BuildMessage(MessageLevel.WARN, text, e, meta, sourceName, sourceFile, sourceLine);

            message.Print(printStacktrace, PrintSourceInfo);
            OnWarning?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        public Message Error(
            string                    text,
            Exception?                e               = null,
            Fields?                   meta            = null,
            bool                      printStacktrace = false,
            [CallerMemberName] string sourceName      = "",
            [CallerFilePath]   string sourceFile      = "",
            [CallerLineNumber] int    sourceLine      = 0
        )
        {
            Message message = BuildMessage(MessageLevel.ERRR, text, e, meta, sourceName, sourceFile, sourceLine);

            message.Print(printStacktrace, PrintSourceInfo);
            OnError?.Trigger();
            OnAny?.Trigger();

            return message;
        }

        private Message BuildMessage(
            MessageLevel level,
            string       text,
            Exception?   e          = null,
            Fields?      meta       = null,
            string       sourceName = "",
            string       sourceFile = "",
            int          sourceLine = 0
        )
        {
            if (PrintSourceInfo && ProjectRoot != "")
                if (sourceFile.Contains(ProjectRoot))
                {
                    int index = sourceFile.IndexOf(ProjectRoot, StringComparison.Ordinal);
                    sourceFile = sourceFile.Substring(index, sourceFile.Length - index);
                }

            Message message = new Message(
                level: level,
                text: text,
                time: DateTime.Now,
                meta: meta,
                exception: e,
                memberName: sourceName,
                sourceFilePath: sourceFile,
                sourceLineNumber: sourceLine
            );
            _history.Add(message);
            return message;
        }

        public List<Message> Events()
        {
            return _history;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Fields : OrderedDictionary
    {
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

        public Fields() { }

        public Fields(object source)
        {
            Type t = source.GetType();

            foreach (FieldInfo member in t.GetFields(BindingFlags))
                Add(member.Name, member.GetValue(source));

            foreach (PropertyInfo member in t.GetProperties(BindingFlags))
                Add(member.Name, member.GetValue(source));
        }
    }

    public sealed class Message
    {
        private const string Delimiter                  = "     ";
        private const string MultilinePrefix            = "                      ";
        private const int    MinAbsoluteMetaLeftPadding = 125;

        private static readonly int MinMetaLeftPadding = MinAbsoluteMetaLeftPadding - Delimiter.Length - 22;

        public DateTime     Time      { get; set; }
        public MessageLevel Level     { get; set; }
        public string       Text      { get; set; }
        public Fields?      Meta      { get; set; }
        public Exception?   Exception { get; set; }

        public string? MemberName       { get; set; }
        public string? SourceFilePath   { get; set; }
        public int?    SourceLineNumber { get; set; }

        public Message(
            MessageLevel level,
            string       text,
            DateTime?    time             = null,
            Fields?      meta             = null,
            Exception?   exception        = null,
            string?      memberName       = null,
            string?      sourceFilePath   = null,
            int?         sourceLineNumber = null
        )
        {
            Level            = level;
            Text             = text;
            Time             = time ?? DateTime.Now;
            Meta             = meta;
            Exception        = exception;
            MemberName       = memberName;
            SourceFilePath   = sourceFilePath;
            SourceLineNumber = sourceLineNumber;
        }

        public override string ToString()
        {
            string result = Text;

            if (Meta != null && Meta.Count > 0)
                result += Delimiter + FormatMeta(Meta);

            return result;
        }

        public string Format(bool printStacktrace, bool printSource = true, bool padded = true)
        {
            var result = "";

            if (printSource)
                result += $"[{SourceFilePath}:{SourceLineNumber} > {MemberName}] ";

            result += Text;

            if (Meta != null && Meta.Count > 0)
            {
                if (result.Length < MinMetaLeftPadding && padded)
                    result += new string(' ', MinMetaLeftPadding - result.Length);

                result += Delimiter + FormatMeta(Meta);
            }

            if (Exception != null && printStacktrace)
            {
                result += "\n" + MultilinePrefix + "Stacktrace:";

                using StringReader reader = new StringReader(Exception.ToString());

                for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    result += "\n" + MultilinePrefix + line;
            }

            return result;
        }

        public void Print(bool printStacktrace, bool printSource = true, bool padded = true)
        {
            string formatted =
                $"{FormatLevel(Level)} [{DateTime.Now:HH:mm:ss.fff}] {Format(printStacktrace, printSource, padded)}";

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
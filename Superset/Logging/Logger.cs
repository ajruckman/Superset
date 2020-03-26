#nullable enable

using System;
using System.Collections.Generic;
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

        public event Action<Message> OnAny;
        public event Action<Message> OnDebug;
        public event Action<Message> OnInfo;
        public event Action<Message> OnWarning;
        public event Action<Message> OnError;
        public event Action<Message> OnStatistic;

        public event Action OnStatusChange;

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
                OnStatusChange?.Invoke();
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
            return message;
        }

        public Message Error(
            string                    text,
            Exception?                e               = null,
            Fields?                   meta            = null,
            bool                      printStacktrace = false,
            bool                      @throw          = false,
            [CallerMemberName] string sourceName      = "",
            [CallerFilePath]   string sourceFile      = "",
            [CallerLineNumber] int    sourceLine      = 0
        )
        {
            e ??= new Exception(text);
            
            Message message = BuildMessage(MessageLevel.ERRR, text, e, meta, sourceName, sourceFile, sourceLine);
            message.Print(printStacktrace, PrintSourceInfo);

            if (@throw)
                throw e;
            
            return message;
        }

        public Message Statistic(
            string                    text,
            Fields?                   meta       = null,
            [CallerMemberName] string sourceName = "",
            [CallerFilePath]   string sourceFile = "",
            [CallerLineNumber] int    sourceLine = 0
        )
        {
            Message message = BuildMessage(MessageLevel.STAT, text, null, meta, sourceName, sourceFile, sourceLine);
            message.Print(false, PrintSourceInfo);
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

            switch (level)
            {
                case MessageLevel.Undefined:
                    break;
                case MessageLevel.DEBG:
                    OnDebug?.Invoke(message);
                    break;
                case MessageLevel.INFO:
                    OnInfo?.Invoke(message);
                    break;
                case MessageLevel.WARN:
                    OnWarning?.Invoke(message);
                    break;
                case MessageLevel.ERRR:
                    OnError?.Invoke(message);
                    break;
                case MessageLevel.STAT:
                    OnStatistic?.Invoke(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            OnAny?.Invoke(message);

            return message;
        }

        public List<Message> Events()
        {
            return _history;
        }
    }
}
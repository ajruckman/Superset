using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Superset.Logging
{
    public sealed class Message
    {
        private const           string Delimiter          = "     ";
        private const           string MultilinePrefix    = "                      ";
        private static readonly int    MinMetaLeftPadding = 175 - Delimiter.Length;

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

        public string Format(bool printStacktrace, bool printSource = true, bool padded = true, int? minMetaLeftPadding = null)
        {
            var result = "";

            if (printSource)
                result += $"[{SourceFilePath}:{SourceLineNumber} > {MemberName}] ";

            result += Text;

            if (Meta != null && Meta.Count > 0)
            {
                int leftPadding = minMetaLeftPadding != null
                    ? minMetaLeftPadding.Value - Delimiter.Length
                    : MinMetaLeftPadding;
                if (result.Length < leftPadding && padded)
                    result += new string(' ', leftPadding - result.Length);

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

        public void Print(bool printStacktrace, bool printSource = true, bool padded = true, int? minMetaLeftPadding = null)
        {
            string formatted =
                $"{FormatLevel(Level)} [{DateTime.Now:HH:mm:ss.fff}] {Format(printStacktrace, printSource, padded, minMetaLeftPadding)}";

            Console.WriteLine(formatted);
            Debug.WriteLine(formatted);
        }

        private static string FormatLevel(MessageLevel level)
        {
            return $"[{level,-4}]";
        }

        private static string FormatMeta(Fields meta)
        {
            return string.Join(' ', meta.Keys.OfType<string>().Select(v => $"[{v}: {(meta[v] is Fields ? "[...]" : meta[v])}]"));
        }
    }
}
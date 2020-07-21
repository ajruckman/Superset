using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBePrivate.Global

namespace Superset.Logging
{
    public static class Log
    {
        /// <summary>
        /// A default Logger instance that can be used internally or externally.
        /// </summary>
        public static Logger Logger { get; } = new Logger();

        public static bool LogUpdates      { get; set; }
        public static bool LogValueChanges { get; set; }

        public static void Update([CallerMemberName] string location = "")
        {
            if (!LogUpdates) return;

            Logger.Debug("Update", new Fields
            {
                {"location", location},
            });
        }

        public static void ValueChange(string path, object value)
        {
            if (!LogValueChanges) return;

            if (value is IEnumerable enumerable && !(value is string))
            {
                List<string> res = new List<string>();
                foreach (object? x in enumerable)
                    res.Add(x?.ToString() ?? "<null>");

                value = string.Join(", ", res);
            }

            Logger.Debug($"{path} -> {value}");
        }
    }
}
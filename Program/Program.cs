using System;
using Superset.Logging;

namespace Program
{
    internal static class Program
    {
        private static void Main()
        {
            Logger logger = new Logger(printSourceInfo: true, projectRoot: "Superset");

            for (var i = 0; i < 130; i++)
            {
                logger.Info(new string('-', i), new Fields {{"a", 1}});
            }

            SplunkExporter splunkExporter = new SplunkExporter(
            );

            Message msg = logger.Warning("Test exception", new Exception("asdf"), printStacktrace: true);

            splunkExporter.Send(msg);
        }
    }
}

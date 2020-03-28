using Superset.Logging;

namespace Program
{
    internal static class Program
    {
        private static void Main()
        {
            Logger logger = new Logger(printSourceInfo: true, projectRoot: "Superset", minMetaLeftPadding: 0);

            // for (int i = 0; i < 10000; i++)
            // {
                // logger.Info(new string('-', 25), new Fields {{"a", 1}});
            // }

            for (var i = 0; i < 130; i++)
            {
                logger.Info(new string('-', i), new Fields {{"a", 1}});
            }

            // SplunkExporter splunkExporter = new SplunkExporter(
            // );

            // Message msg = logger.Warning("Test exception", new Exception("asdf"), printStacktrace: true);

            // splunkExporter.Send(msg);
        }
    }
}

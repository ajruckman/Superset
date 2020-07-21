using System.Collections.Generic;
using Superset.Logging;
using Superset.Web;

namespace Program
{
    internal static class Program
    {
        private static void Main()
        {
            // var x = new ResourceManifests2();
            var                  scripts     = ResourceSets.Listeners.AllScripts();
            var stylesheets = ResourceSets.Listeners.AllStylesheets();

            // Logger logger = new Logger(printSourceInfo: true, projectRoot: "Superset", minMetaLeftPadding: 0);

            // for (int i = 0; i < 10000; i++)
            // {
            // logger.Info(new string('-', 25), new Fields {{"a", 1}});
            // }

            // for (var i = 0; i < 130; i++)
            // {
            //     logger.Info(new string('-', i), new Fields {{"a", 1}});
            // }

            // SplunkExporter splunkExporter = new SplunkExporter(
            // );

            // Message msg = logger.Warning("Test exception", new Exception("asdf"), printStacktrace: true);

            // splunkExporter.Send(msg);
        }
    }
}

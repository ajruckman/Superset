using System;
using Superset.Logging;

namespace Program
{
    internal static class Program
    {
        private static void Main()
        {
            Logger logger = new Logger(printSourceInfo:true, projectRoot:"Superset");

            for (var i = 0; i < 130; i++)
            {
                logger.Info(new string('-', i), new Fields {{"a", 1}});
            }

            logger.Warning("Test exception", new Exception("asdf"), printStacktrace:true);
        }
    }
}

// [INFO] [11:31:57.397] -                                                                                                   [a: 1]
// [INFO] [11:36:36.186] -                                                                              [a: 1]
                       
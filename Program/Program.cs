using Superset.Logging;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = new Logger();

            for (int i = 0; i < 130; i++)
            {
                logger.Info(new string('-', i), new Fields {{"a", 1}});
            }
        }
    }
}

// [INFO] [11:31:57.397] -                                                                                                   [a: 1]
// [INFO] [11:36:36.186] -                                                                              [a: 1]
                       
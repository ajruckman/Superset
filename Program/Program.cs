﻿using System;
using System.Collections.Generic;
using Superset.Logging;
using Superset.Web;

namespace Program
{
    internal static class Program
    {
        private static void Main()
        {
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();

            var c = (a, b);
            var d = (b, a);
            var e = (c, d);

            Console.WriteLine(a.GetHashCode());
            Console.WriteLine(b.GetHashCode());
            Console.WriteLine(c.GetHashCode());
            Console.WriteLine(d.GetHashCode());
            Console.WriteLine(e.GetHashCode());
                        Console.WriteLine(c.GetHashCode());
            Console.WriteLine(d.GetHashCode());
            Console.WriteLine(e.GetHashCode());
            
            Hash(a);
            Hash(b);
            Hash(c);
            Hash(e);
            Hash(d);

            Console.WriteLine(c == c);
            Console.WriteLine(c == d);

            HashSet<object> s = new HashSet<object>();
            s.Add(a);
            s.Add(b);
            s.Add(c);

            Console.WriteLine(s.Count);
            
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

        private static void Hash<T>(T x)
            where T : IEquatable<T>
        {
            Console.WriteLine(x.GetHashCode());
        }
            
    }
}

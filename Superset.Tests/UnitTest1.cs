using System;
using Superset.Logging;
using Xunit;

namespace Superset.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Logger logger = new Logger();
            logger.Info("-", new Fields {{"a", 1}});
        }
    }
}
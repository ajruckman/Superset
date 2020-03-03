using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Superset.Web.Resources.Tests
{
    public class ResourceManifestTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ResourceManifestTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ResourceManifest_Stylesheets_Expands()
        {
            ResourceManifest resourceManifest = new ResourceManifest(
                nameof(ResourceManifestTests),
                stylesheets: new[] {"/{{var1}}/{{var2}}/{{not_var}"}
            );

            string res = resourceManifest.Stylesheets(new Dictionary<string, string>
            {
                {"var1", "val1"},
                {"var2", "val2"},
                {"var3", "val3"}
            });

            const string expected = @"<link rel='stylesheet' type='text/css' href='/_content/ResourceManifestTests//val1/val2/{{not_var}' />
<link rel='stylesheet' type='text/css' href='css/local.css' />
";

            Assert.Equal(res, expected);
        }

        [Fact]
        public void ResourceManifest_Scripts_Expands()
        {
            ResourceManifest resourceManifest = new ResourceManifest(
                nameof(ResourceManifestTests),
                scripts: new[] {"/{{var1}}/{{var2}}/{{not_var}"}
            );
            
            string res = resourceManifest.Scripts(new Dictionary<string, string>
            {
                {"var1", "val1"},
                {"var2", "val2"},
                {"var3", "val3"}
            });

            const string expected = @"<script src='/_content/ResourceManifestTests//val1/val2/{{not_var}'></script>
";
            Assert.Equal(res, expected);
        }
    }
}
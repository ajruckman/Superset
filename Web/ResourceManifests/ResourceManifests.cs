using Superset.Web.Resources;

namespace Superset.Web.ResourceManifests
{
    public static class ResourceManifests
    {
        public static ResourceManifest SaveAsFile =
            new ResourceManifest(nameof(Superset), scripts: new[] {"js/SaveAsFile.js"});
    }
}
using Superset.Web.Resources;

namespace Superset.Web.ResourceManifests
{
    public static class ResourceManifests
    {
        public static ResourceManifest SaveAsFile =
            new ResourceManifest(nameof(Superset), scripts: new[] {"js/SaveAsFile.js"});

        public static ResourceManifest LocalCSS =
            new ResourceManifest(nameof(Superset), stylesheetsExternal: new[] {"/css/Local.css"});
    }
}
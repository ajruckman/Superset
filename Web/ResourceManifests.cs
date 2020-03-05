using Superset.Web.Resources;

// ReSharper disable once CheckNamespace
namespace Superset.Web
{
    public static class ResourceManifests
    {
        public static ResourceManifest SaveAsFile =
            new ResourceManifest(nameof(Superset), new[] {"js/SaveAsFile.js"});

        public static ResourceManifest LocalCSS =
            new ResourceManifest(nameof(Superset), stylesheetsExternal: new[] {"/css/Local.css"});
    }
}
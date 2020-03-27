using Superset.Web.Resources;

// ReSharper disable once CheckNamespace
namespace Superset.Web
{
    public static class ResourceManifests
    {
        public static ResourceManifest SaveAsFile =
            new ResourceManifest(nameof(Superset), new[] {"js/SaveAsFile.js"});

        public static ResourceManifest FocusElement =
            new ResourceManifest(nameof(Superset), new[] {"js/FocusElement.js"});

        public static ResourceManifest Listeners =
            new ResourceManifest(nameof(Superset), new[] {"js/Utilities.js", "js/ClickListener.js", "js/KeyListener.js"});

        public static ResourceManifest LocalCSS =
            new ResourceManifest(nameof(Superset), stylesheetsExternal: new[] {"/css/Local.css"});

        public static ResourceManifest LocalStyledCSS =
            new ResourceManifest(nameof(Superset),
                stylesheetsExternal: new[] {"/css/Local_Style.{{ThemeVariant}}.css"});
    }
}
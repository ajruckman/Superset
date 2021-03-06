﻿using Superset.Web.Resources;

namespace Superset.Web
{
    public static class ResourceManifests
    {
        public static ResourceManifest SaveAsFile =
            new ResourceManifest(nameof(Superset), new[] {"js/SaveAsFile.js"});

        public static ResourceManifest FocusElement =
            new ResourceManifest(nameof(Superset), new[] {"js/FocusElement.js"});

        public static ResourceManifest Listeners =
            new ResourceManifest(nameof(Superset),
                new[] {"js/Utilities.js", "js/ClickListener.js", "js/KeyListener.js"});

        public static ResourceManifest LocalCSS =
            new ResourceManifest(nameof(Superset), stylesheetsExternal: new[] {"/css/Local.css"});

        public static ResourceManifest LocalStyledCSS =
            new ResourceManifest(nameof(Superset),
                stylesheetsExternal: new[] {"/css/Local_Style.{{ThemeVariant}}.css"});

        public static ResourceManifest Tooltip =
            new ResourceManifest(nameof(Superset),
                scripts: new[]
                {
                    "vendor/popper.min.js",           // https://unpkg.com/@popperjs/core@2
                    "vendor/tippy-bundle.umd.min.js", // https://unpkg.com/tippy.js@6
                    "js/Tooltip.js",
                });
    }

    public static class ResourceSets
    {
        private static ResourceSet Utilities =
            new ResourceSet(nameof(Superset), nameof(Utilities),
                new[] {"js/Utilities.js"});

        //

        public static ResourceSet SaveAsFile =
            new ResourceSet(nameof(Superset), nameof(SaveAsFile),
                new[] {"js/SaveAsFile.js"});

        public static ResourceSet FocusElement =
            new ResourceSet(nameof(Superset), nameof(FocusElement),
                new[] {"js/FocusElement.js"});

        public static ResourceSet ClickListener =
            new ResourceSet(nameof(Superset), nameof(ClickListener),
                new[] {"js/ClickListener.js"},
                dependencies: new[] {Utilities});

        public static ResourceSet KeyListener =
            new ResourceSet(nameof(Superset), nameof(KeyListener),
                new[] {"js/KeyListener.js"},
                dependencies: new[] {Utilities});

        public static ResourceSet Listeners =
            new ResourceSet(nameof(Superset), nameof(Listeners),
                dependencies: new[] {Utilities, ClickListener, KeyListener});

        public static ResourceSet LocalCSS =
            new ResourceSet(nameof(Superset), nameof(LocalCSS),
                stylesheetsExternal: new[] {"/css/Local.css"});

        public static ResourceSet LocalStyledCSS =
            new ResourceSet(nameof(Superset), nameof(LocalStyledCSS),
                stylesheetsExternal: new[] {"/css/Local.Style.{{ThemeVariant}}.css"});

        public static ResourceSet Tooltip =
            new ResourceSet(nameof(Superset), nameof(Tooltip),
                scripts: new[]
                {
                    "vendor/popper.min.js",           // https://unpkg.com/@popperjs/core@2
                    "vendor/tippy-bundle.umd.min.js", // https://unpkg.com/tippy.js@6
                    "js/Tooltip.js",
                });
    }
}
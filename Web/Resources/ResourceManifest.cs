#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Superset.Web.Resources
{
    public sealed partial class ResourceManifest
    {
        private readonly List<string> _scripts;
        private readonly List<string> _stylesheets;

        public ResourceManifest(
            string               assembly,
            IEnumerable<string>? scripts             = null,
            IEnumerable<string>? stylesheets         = null,
            IEnumerable<string>? scriptsExternal     = null,
            IEnumerable<string>? stylesheetsExternal = null
        )
        {
            Assembly = assembly;
            string rootPath = $"/_content/{assembly}/";

            _stylesheets = new List<string>();
            _scripts     = new List<string>();

            if (scripts != null)
                foreach (string script in scripts)
                    _scripts.Add(rootPath + script);

            if (stylesheets != null)
                foreach (string stylesheet in stylesheets)
                    _stylesheets.Add(rootPath + stylesheet);

            if (scriptsExternal != null)
                foreach (string script in scriptsExternal)
                    _scripts.Add(script);

            if (stylesheetsExternal != null)
                foreach (string stylesheet in stylesheetsExternal)
                    _stylesheets.Add(stylesheet);
        }

        public string Assembly { get; }

        public RenderFragment Stylesheets(Dictionary<string, string>? variables = null)
        {
            void Fragment(RenderTreeBuilder b)
            {
                var i = 0;
                foreach (string stylesheet in _stylesheets ?? new List<string>())
                    b.AddContent(++i, Stylesheet(Expand(stylesheet, variables)));
            }

            return Fragment;
        }

        public RenderFragment Scripts(Dictionary<string, string>? variables = null)
        {
            void Fragment(RenderTreeBuilder b)
            {
                var i = 0;
                foreach (string script in _scripts ?? new List<string>())
                    b.AddContent(++i, Script(Expand(script, variables)));
            }

            return Fragment;
        }

        // https://stackoverflow.com/a/7957728/9911189
        private string Expand(string str, Dictionary<string, string>? variables)
        {
            if (variables != null)
                str = variables.Aggregate(str, (current, value) =>
                    current.Replace("{{" + value.Key + "}}", value.Value));

            return str;
        }
    }

    public sealed partial class ResourceManifest
    {
        private static RenderFragment Script(string src)
        {
            void Fragment(RenderTreeBuilder b)
            {
                b.OpenElement(0, "script");
                b.AddAttribute(1, "src", src);
                b.CloseElement();
            }

            return Fragment;
        }

        private static RenderFragment Stylesheet(string href)
        {
            void Fragment(RenderTreeBuilder b)
            {
                b.OpenElement(0, "link");
                b.AddAttribute(1, "rel",  "stylesheet");
                b.AddAttribute(2, "type", "text/css");
                b.AddAttribute(3, "href", href);
                b.CloseElement();
            }

            return Fragment;
        }

        public static RenderFragment Render
        (
            IEnumerable<ResourceManifest> manifests,
            bool                          stylesheets,
            bool                          scripts,
            Dictionary<string, string>?   variables
        )
        {
            void Fragment(RenderTreeBuilder builder)
            {
                var i = 0;
                foreach (ResourceManifest manifest in manifests ?? new List<ResourceManifest>())
                {
                    if (stylesheets)
                        builder.AddContent(++i, manifest.Stylesheets(variables));
                    if (scripts)
                        builder.AddContent(++i, manifest.Scripts(variables));
                }
            }

            return Fragment;
        }

        //

        public static RenderFragment Render
        (
            IEnumerable<ResourceManifest> manifests,
            Dictionary<string, string>?   variables = null
        )
        {
            return Render(manifests, true, true, variables);
        }

        public static RenderFragment Render(
            ResourceManifest            manifest,
            Dictionary<string, string>? variables = null
        )
        {
            return Render(new[] {manifest}, variables);
        }

        //

        public static RenderFragment RenderScripts
        (
            IEnumerable<ResourceManifest> manifests,
            Dictionary<string, string>?   variables = null
        )
        {
            return Render(manifests, false, true, variables);
        }

        public static RenderFragment RenderScripts(
            ResourceManifest            manifest,
            Dictionary<string, string>? variables = null
        )
        {
            return RenderScripts(new[] {manifest}, variables);
        }

        //

        public static RenderFragment RenderStylesheets
        (
            IEnumerable<ResourceManifest> manifests,
            Dictionary<string, string>?   variables = null
        )
        {
            return Render(manifests, true, false, variables);
        }

        public static RenderFragment RenderStylesheets(
            ResourceManifest            manifest,
            Dictionary<string, string>? variables = null
        )
        {
            return RenderStylesheets(new[] {manifest}, variables);
        }
    }
}
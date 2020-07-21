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

        public List<string> Stylesheets(Dictionary<string, string>? variables = null)
        {
            return ((_stylesheets ?? new List<string>()).Select(stylesheet => Expand(stylesheet, variables))).ToList();

            // void Fragment(RenderTreeBuilder b)
            // {
            //     var i = 0;
            //     foreach (string stylesheet in _stylesheets ?? new List<string>())
            //         b.AddContent(++i, Stylesheet(Expand(stylesheet, variables)));
            // }
            //
            // return Fragment;
        }

        public List<string> Scripts(Dictionary<string, string>? variables = null)
        {
            return ((_scripts ?? new List<string>()).Select(script => Expand(script, variables))).ToList();

            // void Fragment(RenderTreeBuilder b)
            // {
            //     var i = 0;
            //     foreach (string script in _scripts ?? new List<string>())
            //         b.AddContent(++i, Script(Expand(script, variables)));
            // }
            //
            // return Fragment;
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

                HashSet<string> seenStylesheets = new HashSet<string>();
                HashSet<string> seenScripts     = new HashSet<string>();

                foreach (ResourceManifest manifest in manifests ?? new List<ResourceManifest>())
                {
                    builder.OpenElement(++i, "section");
                    builder.AddAttribute(++i, "hidden", "hidden");
                    builder.AddContent(++i, $"Assembly: {manifest.Assembly}");
                    builder.CloseElement();

                    if (stylesheets)
                        foreach (string stylesheet in manifest.Stylesheets(variables))
                        {
                            if (seenStylesheets.Contains(stylesheet)) continue;
                            
                            builder.AddContent(++i, Stylesheet(stylesheet));
                            seenStylesheets.Add(stylesheet);
                        }

                    if (scripts)
                        foreach (string script in manifest.Scripts(variables))
                        {
                            if (seenScripts.Contains(script)) continue;

                            builder.AddContent(++i, Script(script));
                            seenScripts.Add(script);
                        }
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
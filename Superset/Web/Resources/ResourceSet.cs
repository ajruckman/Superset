using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Superset.Logging;
using Superset.Utilities;

namespace Superset.Web.Resources
{
    public class ResourceSet
    {
        public readonly string Assembly;
        public readonly string ID;
        public readonly string CompositeID;

        private readonly HashSet<string>                 _scripts;
        private readonly HashSet<string>                 _stylesheets;
        private readonly Dictionary<string, ResourceSet> _dependencies;

        public ResourceSet
        (
            string                    assembly,
            string                    id,
            IEnumerable<string>?      scripts             = null,
            IEnumerable<string>?      stylesheets         = null,
            IEnumerable<string>?      scriptsExternal     = null,
            IEnumerable<string>?      stylesheetsExternal = null,
            IEnumerable<ResourceSet>? dependencies        = null
        )
        {
            Assembly    = assembly;
            ID          = id;
            CompositeID = $"{Assembly}+{ID}";

            _scripts      = new HashSet<string>();
            _stylesheets  = new HashSet<string>();
            _dependencies = new Dictionary<string, ResourceSet>();

            string rootPath = $"/_content/{assembly}/";

            if (scripts != null)
            {
                foreach (string script in scripts)
                {
                    if (_scripts.Contains(script))
                        Log.Logger.Warning(
                            $"Re-registering script '{script}' in manifest with ID '{CompositeID}'.");
                    _scripts.Add(rootPath + script);
                }
            }

            if (stylesheets != null)
            {
                foreach (string stylesheet in stylesheets)
                {
                    if (_stylesheets.Contains(stylesheet))
                        Log.Logger.Warning(
                            $"Re-registering script '{stylesheet}' in manifest with ID '{CompositeID}'.");
                    _stylesheets.Add(rootPath + stylesheet);
                }
            }

            if (scriptsExternal != null)
            {
                foreach (string script in scriptsExternal)
                {
                    if (_scripts.Contains(script))
                        Log.Logger.Warning(
                            $"Re-registering external script '{script}' in manifest with ID '{CompositeID}'.");
                    _scripts.Add(script);
                }
            }

            if (stylesheetsExternal != null)
            {
                foreach (string stylesheet in stylesheetsExternal)
                {
                    if (_stylesheets.Contains(stylesheet))
                        Log.Logger.Warning(
                            $"Re-registering external stylesheet '{stylesheet}' in manifest with ID '{CompositeID}'.");
                    _stylesheets.Add(stylesheet);
                }
            }

            if (dependencies != null)
            {
                foreach (ResourceSet dependency in dependencies)
                {
                    if (_dependencies.ContainsKey(dependency.CompositeID))
                        Log.Logger.Warning(
                            $"Re-registering dependency manifest '{dependency.CompositeID}' in manifest with ID '{CompositeID}'.");
                    _dependencies[dependency.CompositeID] = dependency;
                }
            }
        }

        //

        private readonly ThreadSafeCache<HashSet<string>> _allScripts = new ThreadSafeCache<HashSet<string>>();

        public HashSet<string> AllScripts()
        {
            return _allScripts.SetIf(() =>
            {
                HashSet<string> result = new HashSet<string>();

                foreach (string script in _scripts)
                    result.Add(script);

                foreach (var (_, manifest) in _dependencies)
                {
                    foreach (var script in manifest.AllScripts())
                    {
                        result.Add(script);
                    }
                }

                return result;
            });
        }

        private readonly ThreadSafeCache<HashSet<string>> _allStylesheets = new ThreadSafeCache<HashSet<string>>();

        public HashSet<string> AllStylesheets()
        {
            return _allStylesheets.SetIf(() =>
            {
                HashSet<string> result = new HashSet<string>();

                foreach (string stylesheet in _stylesheets)
                    result.Add(stylesheet);

                foreach (var (_, manifest) in _dependencies)
                {
                    foreach (var stylesheet in manifest.AllStylesheets())
                    {
                        result.Add(stylesheet);
                    }
                }

                return result;
            });
        }

        //

        private static RenderFragment Script(string src) => b =>
        {
            b.OpenElement(0, "script");
            b.AddAttribute(1, "src", src);
            b.CloseElement();
        };

        private static RenderFragment Stylesheet(string href) => b =>
        {
            b.OpenElement(0, "link");
            b.AddAttribute(1, "rel",  "stylesheet");
            b.AddAttribute(2, "type", "text/css");
            b.AddAttribute(3, "href", href);
            b.CloseElement();
        };
        
        // https://stackoverflow.com/a/7957728/9911189
        private string Expand(string str, Dictionary<string, string>? variables)
        {
            if (variables != null)
                str = variables.Aggregate(str, (current, value) =>
                    current.Replace("{{" + value.Key + "}}", value.Value));

            return str;
        }

        public RenderFragment Render
        (
            bool                        stylesheets = true,
            bool                        scripts     = true,
            Dictionary<string, string>? variables   = null
        )
        {
            void Fragment(RenderTreeBuilder builder)
            {
                var i = 0;

                builder.OpenElement(++i, "section");
                builder.AddAttribute(++i, "hidden", "hidden");
                builder.AddContent(++i, $"ID: {CompositeID}");
                builder.CloseElement();

                if (scripts)
                    foreach (string script in AllScripts())
                        builder.AddContent(++i, Script(Expand(script, variables)));

                if (stylesheets)
                    foreach (string stylesheet in AllStylesheets())
                        builder.AddContent(++i, Stylesheet(Expand(stylesheet, variables)));
            }

            return Fragment;
        }
    }
}
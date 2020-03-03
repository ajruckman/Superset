#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Html;

// ReSharper disable MemberCanBeInternal
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Superset.Web.Resources
{
    public sealed class ResourceManifest
    {
        private readonly List<string> _scripts;
        private readonly List<string> _stylesheets;

        public ResourceManifest(
            string              assembly,
            IEnumerable<string> scripts             = null,
            IEnumerable<string> stylesheets         = null,
            IEnumerable<string> scriptsExternal     = null,
            IEnumerable<string> stylesheetsExternal = null
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

            _stylesheets.Add("css/local.css");
        }

        public string Assembly { get; }

        public string Stylesheets(Dictionary<string, string>? variables = null)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string stylesheet in _stylesheets)
                builder.AppendLine($"<link rel='stylesheet' type='text/css' href='{Expand(stylesheet, variables)}' />");

            return builder.ToString();
        }

        public string Scripts(Dictionary<string, string>? variables = null)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string script in _scripts)
                builder.AppendLine($"<script src='{Expand(script, variables)}'></script>");

            return builder.ToString();
        }

        // https://stackoverflow.com/a/7957728/9911189
        private string Expand(string str, Dictionary<string, string>? variables)
        {
            if (variables != null)
                str = variables.Aggregate(str, (current, value) =>
                    current.Replace("{{" + value.Key + "}}", value.Value));

            return str;
        }

        public static IHtmlContent Render(
            ResourceManifest            manifest,
            Dictionary<string, string>? variables = null
        )
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(manifest.Stylesheets(variables));
            result.AppendLine(manifest.Scripts(variables));

            return new HtmlString(result.ToString());
        }

        public static IHtmlContent RenderSet
        (
            IEnumerable<ResourceManifest> manifests,
            Dictionary<string, string>?   variables = null
        )
        {
            StringBuilder result = new StringBuilder();
            foreach (ResourceManifest manifest in manifests)
            {
                result.AppendLine($"<!-- Assembly: {manifest.Assembly} -->");
                result.AppendLine(manifest.Stylesheets(variables));
                result.AppendLine(manifest.Scripts(variables));
            }

            return new HtmlString(result.ToString());
        }
    }
}
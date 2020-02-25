using System.Collections.Generic;
using System.Text;

namespace Superset.Web.Resources
{
    public sealed class ResourceManifest
    {
        private readonly List<string> _scripts;
        private readonly List<string> _stylesheets;

        public ResourceManifest(
            string              assembly,
            IEnumerable<string> scripts,
            IEnumerable<string> stylesheets,
            IEnumerable<string> scriptsExternal,
            IEnumerable<string> stylesheetsExternal
        )
        {
            string rootPath = $"/_content/{assembly}/";

            _stylesheets = new List<string>();
            _scripts     = new List<string>();

            foreach (string script in scripts)
                _scripts.Add(rootPath + script);

            foreach (string stylesheet in stylesheets)
                _stylesheets.Add(rootPath + stylesheet);

            foreach (string script in scriptsExternal)
                _scripts.Add(script);

            foreach (string stylesheet in stylesheetsExternal)
                _stylesheets.Add(stylesheet);

            _stylesheets.Add("css/local.css");
        }

        public string Stylesheets()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string stylesheet in _stylesheets)
                builder.AppendLine($"<link href='{stylesheet}' rel='stylesheet' />");

            return builder.ToString();
        }

        public string Scripts()
        {
            StringBuilder builder = new StringBuilder();

            foreach (string script in _scripts)
                builder.AppendLine($"<script src='{script}'></script>");

            return builder.ToString();
        }
    }
}
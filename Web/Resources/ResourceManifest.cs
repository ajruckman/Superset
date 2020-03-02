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
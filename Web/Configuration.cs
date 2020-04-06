using System.Collections.Generic;
using Superset.Web.Resources;

namespace Web
{
    public static class Configuration
    {
        public static List<ResourceManifest> Resources;

        static Configuration()
        {
            Resources = new List<ResourceManifest>
            {
                Superset.Web.ResourceManifests.Listeners,
                Superset.Web.ResourceManifests.Listeners,
                Superset.Web.ResourceManifests.FocusElement,
                Superset.Web.ResourceManifests.LocalCSS,
                Superset.Web.ResourceManifests.FocusElement,
                Superset.Web.ResourceManifests.SaveAsFile,
                Superset.Web.ResourceManifests.LocalCSS,
                Superset.Web.ResourceManifests.LocalCSS,
            };
        }
    }
}
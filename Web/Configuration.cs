using System.Collections.Generic;
using Superset.Web.Resources;

namespace Web
{
    public static class Configuration
    {
        // public static List<ResourceManifest> Resources;

        public static ResourceSet Resources;

        static Configuration()
        {
            Resources = new ResourceSet(
                nameof(Web), nameof(Resources),
                dependencies: new []
                {
                    Superset.Web.ResourceSets.Listeners,
                    Superset.Web.ResourceSets.FocusElement,
                    Superset.Web.ResourceSets.LocalCSS,
                    Superset.Web.ResourceSets.FocusElement,
                    Superset.Web.ResourceSets.SaveAsFile,
                    Superset.Web.ResourceSets.LocalCSS,
                    Superset.Web.ResourceSets.LocalCSS,
                    Superset.Web.ResourceSets.LocalStyledCSS,
                    Superset.Web.ResourceSets.Tooltip,
                }
            );
            
            // Resources = new List<ResourceManifest>
            // {
            // Superset.Web.ResourceManifests.Listeners,
            // Superset.Web.ResourceManifests.FocusElement,
            // Superset.Web.ResourceManifests.LocalCSS,
            // Superset.Web.ResourceManifests.FocusElement,
            // Superset.Web.ResourceManifests.SaveAsFile,
            // Superset.Web.ResourceManifests.LocalCSS,
            // Superset.Web.ResourceManifests.LocalCSS,
            // Superset.Web.ResourceManifests.Tooltip,
            // };
        }
    }
}
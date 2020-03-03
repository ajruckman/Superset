using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Superset.Web.Resources;

namespace Superset.Web.Utilities
{
    public static partial class Utilities
    {
        // https://stackoverflow.com/a/53822526/9911189
        public static async Task SaveAsFile(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "Superset_SaveAsFile",
                filename,
                Convert.ToBase64String(data)
            );
        }

        public static ResourceManifest SaveAsFileManifest =
            new ResourceManifest(nameof(Superset), scripts: new[] {"js/SaveAsFile.js"});
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Superset.Web.Utilities
{
    public static partial class Utilities
    {
        // https://stackoverflow.com/a/53822526/9911189
        public static async Task SaveAsFile(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data)
            );
        }
    }
}
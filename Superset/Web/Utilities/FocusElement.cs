using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Superset.Web.Utilities
{
    public static partial class Utilities
    {
        public static async Task FocusElement(IJSRuntime js, ElementReference element)
        {
            await js.InvokeAsync<object>(
                "Superset_FocusElement",
                element
            );
        }
    }
}
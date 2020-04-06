using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Superset.Web.JSInterop;

namespace Superset.Web.Markup
{
    public class Tooltip : IJSTask
    {
        public Tooltip(ElementReference element, string content)
        {
            Element = element;
            Content = content;
        }

        private ElementReference Element { get; set; }
        private string           Content { get; set; }

        public async Task Execute(IJSRuntime runtime)
        {
            await runtime.InvokeAsync<object>(
                "window.Superset_InitTooltip",
                Element,
                Content
            );
        }
    }
}
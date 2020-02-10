using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Superset.Common
{
    public class TriggerWrapper : ComponentBase
    {
        private readonly object _keyLock = new object();
        private          bool   _canRender;
        private          string _key;

        [Parameter] public RenderFragment  ChildContent { get; set; }
        [Parameter] public UpdateTrigger   Trigger      { get; set; }
        [Parameter] public UpdateTrigger[] Triggers     { get; set; }
        [Parameter] public bool            Protected    { get; set; }

        private void NewKey()
        {
            lock (_keyLock)
            {
                _key = DateTime.Now.Ticks.ToString();
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "section");
            builder.AddAttribute(1, "key",   _key);
            builder.AddAttribute(2, "style", "display: contents;");
            builder.AddContent(2, ChildContent);
        }

        protected override void OnInitialized()
        {
            NewKey();

            if (Trigger != null)
            {
                Trigger.OnUpdate += Refresh;
                Trigger.OnReDiff += () =>
                {
                    NewKey();
                    Refresh();
                };
            }

            if (Triggers == null) return;

            foreach (UpdateTrigger trigger in Triggers)
            {
                trigger.OnUpdate += Refresh;
                trigger.OnReDiff += () =>
                {
                    NewKey();
                    Refresh();
                };
            }
        }

        private void Refresh()
        {
            _canRender = true;
            InvokeAsync(StateHasChanged);
            _canRender = false;
        }

        protected override bool ShouldRender() => !Protected || _canRender;
    }
}
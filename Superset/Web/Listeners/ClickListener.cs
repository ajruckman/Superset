using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Superset.Web.Listeners
{
    public sealed class ClickListener
    {
        private readonly ElementReference _element;

        public ClickListener(ElementReference element)
        {
            _element = element;
        }

        public ValueTask<object> Initialize(IJSRuntime runtime)
        {
            return runtime.InvokeAsync<object>(
                "window.Superset_AddClickCallback",
                _element,
                DotNetObjectReference.Create(this)
            );
        }

        [JSInvokable]
        public void OuterClick(int button, string targetID, int x, int y, bool shift, bool control)
        {
            OnOuterClick?.Invoke(new ClickArgs(button, x, y, shift, control, targetID));
        }

        [JSInvokable]
        public void Click(int button, string targetID, int x, int y, bool shift, bool control)
        {
            OnClick?.Invoke(new ClickArgs(button, x, y, shift, control, targetID));
        }

        [JSInvokable]
        public void InnerClick(int button, string targetID, int x, int y, bool shift, bool control)
        {
            OnInnerClick?.Invoke(new ClickArgs(button, x, y, shift, control, targetID));
        }

        public event Action<ClickArgs> OnOuterClick;
        public event Action<ClickArgs> OnClick;
        public event Action<ClickArgs> OnInnerClick;

        public struct ClickArgs
        {
            internal ClickArgs(int button, int x, int y, bool shift, bool control, string targetID)
            {
                Button   = button;
                X        = x;
                Y        = y;
                Shift    = shift;
                Control  = control;
                TargetID = targetID;
            }

            public readonly int    Button;
            public readonly string TargetID;
            public readonly int    X;
            public readonly int    Y;
            public readonly bool   Shift;
            public readonly bool   Control;
        }
    }
}
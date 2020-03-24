using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Superset.Web.Listeners
{
    public sealed class ClickListener
    {
        private readonly string _targetID;

        public ClickListener(string targetID)
        {
            _targetID = targetID;
        }

        public ValueTask<object> Initialize(IJSRuntime runtime)
        {
            return runtime.InvokeAsync<object>(
                "window.clickCallback",
                _targetID,
                DotNetObjectReference.Create(this)
            );
        }

        [JSInvokable]
        public void OuterClick(int button, int x, int y, bool shift, bool control, string targetID)
        {
            OnOuterClick?.Invoke(new ClickArgs(button, x, y, shift, control, targetID));
        }

        [JSInvokable]
        public void Click(int button, int x, int y, bool shift, bool control, string targetID)
        {
            OnClick?.Invoke(new ClickArgs(button, x, y, shift, control, targetID));
        }

        [JSInvokable]
        public void InnerClick(int button, int x, int y, bool shift, bool control, string targetID)
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
            public readonly int    X;
            public readonly int    Y;
            public readonly bool   Shift;
            public readonly bool   Control;
            public readonly string TargetID;
        }
    }
}
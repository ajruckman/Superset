using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Superset.Web.JSInterop;

namespace Superset.Web.Listeners
{
    public sealed class KeyListener : IJSTask
    {
        private readonly ElementReference _element;

        public KeyListener(ElementReference element)
        {
            _element = element;
        }

        public async Task Execute(IJSRuntime runtime)
        {
            await runtime.InvokeAsync<object>(
                "window.Superset_AddKeyCallback",
                _element,
                DotNetObjectReference.Create(this)
            );
        }

        [JSInvokable]
        public void OuterKeyUp(string key, string targetID, bool shift, bool control)
        {
            OnOuterKeyUp?.Invoke(new KeyArgs(key, targetID, shift, control));
        }

        [JSInvokable]
        public void KeyUp(string key, string targetID, bool shift, bool control)
        {
            OnKeyUp?.Invoke(new KeyArgs(key, targetID, shift, control));
        }

        [JSInvokable]
        public void InnerKeyUp(string key, string targetID, bool shift, bool control)
        {
            OnInnerKeyUp?.Invoke(new KeyArgs(key, targetID, shift, control));
        }

        public event Action<KeyArgs> OnOuterKeyUp;
        public event Action<KeyArgs> OnKeyUp;
        public event Action<KeyArgs> OnInnerKeyUp;

        public struct KeyArgs
        {
            public KeyArgs(string key, string targetID, bool shift, bool control)
            {
                Key      = key;
                TargetID = targetID;
                Shift    = shift;
                Control  = control;
            }

            public readonly string Key;
            public readonly string TargetID;
            public readonly bool   Shift;
            public readonly bool   Control;
        }
    }
}
using System;
using Microsoft.AspNetCore.Components;
using Superset.Web.Listeners;

namespace Web.Pages
{
    public partial class Index
    {
        private ElementReference _inner1;
        private ElementReference _inner2;

        ClickListener _clickListener1;
        ClickListener _clickListener2;

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            _clickListener1 = new ClickListener(_inner1);
            _clickListener1.Initialize(JSRuntime);

            _clickListener2 = new ClickListener(_inner2);
            _clickListener2.Initialize(JSRuntime);

            _clickListener1.OnOuterClick += (args) =>
                Console.WriteLine($"Outer1 -> Outer {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener1.OnClick += (args) =>
                Console.WriteLine($"Outer1 -> Self  {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener1.OnInnerClick += (args) =>
                Console.WriteLine($"Outer1 -> Inner {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");

            _clickListener1.OnInnerClick += (args) => { Console.WriteLine(args.TargetID); };

            _clickListener2.OnOuterClick += (args) =>
                Console.WriteLine($"Outer2 -> Outer {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener2.OnClick += (args) =>
                Console.WriteLine($"Outer2 -> Self  {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener2.OnInnerClick += (args) =>
                Console.WriteLine($"Outer2 -> Inner {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");

            //

            KeyListener _keyListener1 = new KeyListener(_inner1);
            _keyListener1.Initialize(JSRuntime);

            _keyListener1.OnOuterKeyUp += (args) =>
                Console.WriteLine($"Outer1 -> Outer        {args.Key} {args.TargetID} {args.Shift} {args.Control}");
            _keyListener1.OnKeyUp += (args) =>
                Console.WriteLine($"Outer1 -> OnKeyUp      {args.Key} {args.TargetID} {args.Shift} {args.Control}");
            _keyListener1.OnInnerKeyUp += (args) =>
                Console.WriteLine($"Outer1 -> OnInnerKeyUp {args.Key} {args.TargetID} {args.Shift} {args.Control}");
        }
    }
}
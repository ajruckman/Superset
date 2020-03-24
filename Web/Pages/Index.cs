using System;
using Superset.Web.Listeners;

namespace Web.Pages
{
    public partial class Index
    {
        
        ClickListener _clickListener1;
        ClickListener _clickListener2;

        protected override void OnInitialized()
        {
            _clickListener1 = new ClickListener("Inner1");
            _clickListener1.Initialize(JSRuntime);
        
            _clickListener2 = new ClickListener("Inner2");
            _clickListener2.Initialize(JSRuntime);

            _clickListener1.OnOuterClick += (args) => Console.WriteLine($"Outer1 -> Outer {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener1.OnClick      += (args) => Console.WriteLine($"Outer1 -> Self  {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener1.OnInnerClick += (args) => Console.WriteLine($"Outer1 -> Inner {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");

            _clickListener1.OnInnerClick += (args) =>
            {
                Console.WriteLine(args.TargetID);
            };

            _clickListener2.OnOuterClick += (args) => Console.WriteLine($"Outer2 -> Outer {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener2.OnClick      += (args) => Console.WriteLine($"Outer2 -> Self  {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
            _clickListener2.OnInnerClick += (args) => Console.WriteLine($"Outer2 -> Inner {args.Button} {args.X} {args.Y} {args.Shift} {args.Control}");
        
        }
    }
}
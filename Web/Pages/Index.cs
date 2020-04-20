using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Superset.Web.Listeners;
using Superset.Web.Markup;
using Superset.Web.Utilities;
using Superset.Web.Validation;

namespace Web.Pages
{
    public partial class Index
    {
        private ElementReference _inner1;
        private ElementReference _inner2;
        private ElementReference _text1;

        ClickListener                              _clickListener1;
        ClickListener                              _clickListener2;
        private Validator<CustomValidationResults> _validator;

        private enum CustomValidationResults
        {
            Undefined,
            TooSmall,
            TooBig,
            Valid,
            Invalid
        }

        protected override void OnInitialized()
        {
            _validator = new Validator<CustomValidationResults>(() =>
            {
                Console.WriteLine("---");
                return new[]
                {
                    new Validation<CustomValidationResults>(CustomValidationResults.Invalid, "Not OK"),
                    new Validation<CustomValidationResults>(CustomValidationResults.TooBig,  "Warning"),
                    new Validation<CustomValidationResults>(CustomValidationResults.Valid,   "OK"),
                };
            });
            _validator.Register("field1",
                () =>
                {
                    return new[]
                    {
                        _field1value != ""
                            ? new Validation<CustomValidationResults>(CustomValidationResults.Valid,   "All OK")
                            : new Validation<CustomValidationResults>(CustomValidationResults.Invalid, "Required")
                    };
                });

            _validator.Validate();

            Console.WriteLine(_validator.AnyOfType(CustomValidationResults.TooSmall));
            Console.WriteLine(_validator.AnyOfType(CustomValidationResults.TooBig));
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            _clickListener1 = new ClickListener(_inner1);
            _clickListener1.Execute(JSRuntime);

            _clickListener2 = new ClickListener(_inner2);
            _clickListener2.Execute(JSRuntime);

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
            _keyListener1.Execute(JSRuntime);

            _keyListener1.OnOuterKeyUp += (args) =>
                Console.WriteLine($"Outer1 -> Outer        {args.Key} {args.TargetID} {args.Shift} {args.Control}");
            _keyListener1.OnKeyUp += (args) =>
                Console.WriteLine($"Outer1 -> OnKeyUp      {args.Key} {args.TargetID} {args.Shift} {args.Control}");
            _keyListener1.OnInnerKeyUp += (args) =>
                Console.WriteLine($"Outer1 -> OnInnerKeyUp {args.Key} {args.TargetID} {args.Shift} {args.Control}");

            //
            
            Tooltip tooltip = new Tooltip(_inner1, "Tooltip on inner 1");
            tooltip.Execute(JSRuntime);

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                // Utilities.FocusElement(JSRuntime, _text1);
            });
        }

        private string _field1value = "";

        private void UpdateField1(ChangeEventArgs args)
        {
            _field1value = args.Value?.ToString() ?? "";

            _validator.Validate();
        }
    }
}
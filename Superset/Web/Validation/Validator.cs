using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Superset.Web.State;

namespace Superset.Web.Validation
{
    public class Validator<T> where T : Enum
    {
        public delegate IEnumerable<Validation<T>> ResultGetter();

        private readonly ResultGetter? _overallValidationGetter;

        private readonly Dictionary<string, ResultGetter> _fields         = new Dictionary<string, ResultGetter>();
        private readonly UpdateTrigger                    _refreshTrigger = new UpdateTrigger();

        private IEnumerable<Validation<T>>? _cachedOverallResult =
            new List<Validation<T>>();

        private Dictionary<string, IEnumerable<Validation<T>>>? _cachedFieldResults =
            new Dictionary<string, IEnumerable<Validation<T>>>();

        public Validator(ResultGetter? overallValidationGetter = null)
        {
            _overallValidationGetter = overallValidationGetter;
        }

        public void Register(string field, ResultGetter resultGetter)
        {
            _fields.Add(field, resultGetter);
        }

        private void CacheResults()
        {
            _cachedOverallResult = _overallValidationGetter?.Invoke();

            _cachedFieldResults = new Dictionary<string, IEnumerable<Validation<T>>>();
            foreach ((string field, ResultGetter resultGetter) in _fields)
                _cachedFieldResults[field] = resultGetter.Invoke();
        }

        public void Validate()
        {
            CacheResults();
            _refreshTrigger.Trigger();
        }

        private RenderFragment RenderResults(IEnumerable<Validation<T>> results, bool overall)
        {
            int seq = -1;

            void Fragment(RenderTreeBuilder builder)
            {
                builder.OpenElement(++seq, "div");
                builder.AddAttribute(++seq, "class",
                    "ValidationContainer ValidationContainer--" + (!overall ? "Field" : "Overall"));

                builder.OpenComponent<TriggerWrapper>(++seq);
                builder.AddAttribute(++seq, "Trigger",   _refreshTrigger);
                builder.AddAttribute(++seq, "ChildContent", (RenderFragment) (builder2 =>
                {
                    foreach (Validation<T> result in results)
                    {
                        builder2.OpenElement(++seq, "div");
                        builder2.AddAttribute(++seq, "class", "Validation Validation--" + result.Result);

                        builder2.OpenElement(++seq, "div");
                        builder2.AddAttribute(++seq, "class", "ValidationResult ValidationResult--" + result.Result);
                        builder2.CloseElement();

                        builder2.OpenElement(++seq, "span");
                        builder2.AddAttribute(++seq, "class", "ValidationText");
                        builder2.AddContent(++seq, result.Message);
                        builder2.CloseElement();

                        builder2.CloseElement();
                    }
                }));
                builder.CloseComponent();

                builder.CloseElement();
            }

            return Fragment;
        }

        public RenderFragment RenderFieldResults(string field)
        {
            if (!_fields.ContainsKey(field))
                throw new ArgumentOutOfRangeException(nameof(field), $"Field '{field}' has not been registered.");

            if (_cachedFieldResults == null)
                throw new ArgumentNullException(nameof(_cachedFieldResults),
                    $"RenderFieldResults() was called, but no fields have been cached.");

            if (!_cachedFieldResults.ContainsKey(field))
                throw new ArgumentNullException(nameof(field), $"Field '{field}' has not been validated.");

            return RenderResults(_cachedFieldResults[field], false);
        }

        public RenderFragment RenderOverallResults()
        {
            if (_overallValidationGetter == null)
                throw new InvalidOperationException(
                    "RenderOverallResults() was called, but no overall result validator was passed to Validator constructor.");

            if (_cachedOverallResult == null)
                throw new ArgumentNullException(nameof(_cachedOverallResult),
                    "RenderOverallResults() was called, but the overall validation result has not been cached.");

            return RenderResults(_cachedOverallResult, true);
        }

        public bool AnyOfType(T type, bool includeOverall)
        {
            if (includeOverall && _overallValidationGetter == null)
            {
                throw new InvalidOperationException(
                    "AnyOfType() was called with includeOverall = true, but no overall result validator was passed to Validator constructor.");
            }
            
            if (includeOverall && _overallValidationGetter != null)
            {
                if (_cachedOverallResult == null)
                    throw new ArgumentNullException(nameof(_cachedOverallResult),
                        "AnyMatch() was called, but the overall validation result has not been cached.");

                foreach (var result in _cachedOverallResult)
                    if (Equals(result.Result, type))
                        return true;
            }

            if (_cachedFieldResults == null)
                throw new ArgumentNullException(nameof(_cachedFieldResults),
                    $"AnyOfType() was called, but no fields have been cached.");

            foreach ((string _, IEnumerable<Validation<T>> results) in _cachedFieldResults)
            foreach (var result in results)
                if (Equals(result.Result, type))
                    return true;

            return false;
        }
    }
}
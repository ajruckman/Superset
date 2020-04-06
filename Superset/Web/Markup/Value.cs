#nullable enable

using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

// ReSharper disable UnusedMember.Global

namespace Superset.Web.Markup
{
    public sealed class Value
    {
        private readonly ClassSet?       _classSet;
        private readonly StyleSet?       _styleSet;
        private readonly object?         _v;
        private readonly RenderFragment? _content;

        public Value(object v)
        {
            Debug.Assert(v != null, "Attempted to set null Superset.Web.Markup.Value value.");
            _v = v;
        }

        public Value(object v, ClassSet classSet)
        {
            Debug.Assert(v != null, "Attempted to set null Superset.Web.Markup.Value value.");
            _v        = v;
            _classSet = classSet;
        }

        public Value(object v, StyleSet styleSet)
        {
            Debug.Assert(v != null, "Attempted to set null Superset.Web.Markup.Value value.");
            _v        = v;
            _styleSet = styleSet;
        }

        public Value(object v, ClassSet classSet, StyleSet styleSet)
        {
            Debug.Assert(v != null, "Attempted to set null Superset.Web.Markup.Value value.");
            _v        = v;
            _classSet = classSet;
            _styleSet = styleSet;
        }

        public Value(RenderFragment content)
        {
            Debug.Assert(content != null, "Attempted to set null Superset.Web.Markup.Value value.");
            _content = content;
        }

        public RenderFragment Span
        {
            get
            {
                int seq = -1;

                void Fragment(RenderTreeBuilder builder)
                {
                    builder.OpenElement(++seq, "span");

                    ++seq;
                    if (_classSet != null)
                        builder.AddAttribute(seq, "class", _classSet.ToString());

                    ++seq;
                    if (_styleSet != null)
                        builder.AddAttribute(seq, "style", _styleSet.ToString());

                    if (_v != null)
                        builder.AddContent(++seq, _v);
                    else if (_content != null)
                        builder.AddContent(++seq, _content);
                    else
                        throw new Exception("Attempted to render Superset.Web.Markup.Value with no content.");

                    builder.CloseElement();
                }

                return Fragment;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Superset.Web.Markup
{
    public sealed class ClassSet
    {
        private readonly List<string> _classes;

        public ClassSet()
        {
            _classes = new List<string>();
        }

        public ClassSet(params string[] classes)
        {
            // Clone so that different classes are component-specific when same object is used in multiple components
            _classes = classes.ToList();
        }

        public ClassSet Add(params string[] classes)
        {
            _classes.AddRange(classes);
            return this;
        }

        public override string ToString()
        {
            return _classes != null ? string.Join(' ', _classes) : "";
        }

        public string[] Classes => _classes.ToArray();
    }
}
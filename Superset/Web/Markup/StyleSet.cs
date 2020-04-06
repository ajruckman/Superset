using System.Collections.Generic;

namespace Superset.Web.Markup
{
    public class StyleSet
    {
        private readonly Dictionary<string, string> _properties;

        public StyleSet(params (string, string)[] properties)
        {
            _properties = new Dictionary<string, string>();

            foreach ((string key, string value) in properties)
            {
                _properties.Add(key, value);
            }
        }

        public StyleSet Add(string key, string value)
        {
            _properties.Add(key, value);
            return this;
        }

        public override string ToString()
        {
            var result = "";

            foreach ((string key, string value) in _properties)
                result += $"{key}: {value}; ";

            if (result.EndsWith(" "))
                result = result.TrimEnd(' ');

            return result;
        }
    }
}
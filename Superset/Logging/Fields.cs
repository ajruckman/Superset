using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace Superset.Logging
{
    public class Fields : OrderedDictionary
    {
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

        public Fields() { }

        public Fields(object? source)
        {
            if (source == null) return;

            Type t = source.GetType();

            foreach (FieldInfo m in t.GetFields(BindingFlags))
                if (m.GetCustomAttribute(typeof(LoggerIgnore)) == null)
                    Add(m.Name, m.GetValue(source));

            foreach (PropertyInfo m in t.GetProperties(BindingFlags).Where(v => v.GetIndexParameters().Length == 0))
                if (m.GetCustomAttribute(typeof(LoggerIgnore)) == null)
                    Add(m.Name, m.GetValue(source));
        }

        public Fields(IDictionary<string, object>? dict)
        {
            if (dict == null) return;

            foreach ((string key, object value) in dict)
            {
                Add(key, value);
            }
        }

        public Fields(IReadOnlyDictionary<string, object>? dict)
        {
            if (dict == null) return;

            foreach ((string key, object value) in dict)
            {
                Add(key, value);
            }
        }

        public Fields(OrderedDictionary? dict)
        {
            if (dict == null) return;

            foreach (DictionaryEntry o in dict)
            {
                Add(o.Key, o.Value);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LoggerIgnore : Attribute { }
}
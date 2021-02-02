using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Superset.Logging
{
    public class Fields : OrderedDictionary
    {
        private const BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

        public Fields() { }

        public Fields(object source)
        {
            Type t = source.GetType();

            foreach (FieldInfo member in t.GetFields(BindingFlags))
                if (member.GetCustomAttribute(typeof(LoggerIgnore)) == null)
                    Add(member.Name, member.GetValue(source));

            foreach (PropertyInfo member in t.GetProperties(BindingFlags))
                if (member.GetCustomAttribute(typeof(LoggerIgnore)) == null)
                    Add(member.Name, member.GetValue(source));
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class LoggerIgnore : Attribute { }
}
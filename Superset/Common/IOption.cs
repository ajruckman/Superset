using System;

namespace Superset.Common
{
    public interface IOption<T>
        where T : IEquatable<T>
    {
        T      ID           { get; }
        string OptionText   { get; }
        string SelectedText { get; }
        bool   Selected     { get; }
        bool   Disabled     { get; }
        bool   Placeholder  { get; }
    }
}
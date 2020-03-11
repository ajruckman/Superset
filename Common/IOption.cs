#nullable enable

namespace Superset.Common
{
    public interface IOption
    {
        object? ID           { get; }
        string? Text         { get; }
        string? SelectedText { get; }
        bool    Selected     { get; }
        bool    Disabled     { get; }
        bool    Placeholder  { get; }
    }
}
using System.Diagnostics.CodeAnalysis;

namespace Superset.Logging
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum MessageLevel
    {
        Undefined,
        DEBG,
        INFO,
        WARN,
        ERRR,
        STAT
    }
}
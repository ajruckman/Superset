using System;

namespace Superset.Web.Validation
{
    public sealed class Validation<T> where T : Enum
    {
        public readonly T      Result;
        public readonly string Message;

        public Validation(T result, string message)
        {
            Result  = result;
            Message = message;
        }
    }
}
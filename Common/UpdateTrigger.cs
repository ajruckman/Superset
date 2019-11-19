using System;

// ReSharper disable MemberCanBeInternal

namespace Superset.Common
{
    public sealed class UpdateTrigger
    {
        public event Action OnUpdate;

        public void Trigger()
        {
            OnUpdate?.Invoke();
        }
    }
}
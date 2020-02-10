using System;

// ReSharper disable MemberCanBeInternal

namespace Superset.Common
{
    public sealed class UpdateTrigger
    {
        public event Action OnUpdate;
        public event Action OnReDiff;

        public void Trigger()
        {
            OnUpdate?.Invoke();
        }

        public void ReDiff()
        {
            OnReDiff?.Invoke();
        }
    }
}
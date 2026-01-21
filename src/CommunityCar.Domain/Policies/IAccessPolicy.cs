using System;

namespace CommunityCar.Domain.Policies;

public interface IAccessPolicy<T>
{
    bool CanAccess(Guid userId, T resource);
}

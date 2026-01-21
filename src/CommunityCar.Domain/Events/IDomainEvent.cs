using System;

namespace CommunityCar.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Management;

public class Ticket : AggregateRoot
{
    public string Subject { get; private set; }
    public string Description { get; private set; }
    public string Priority { get; private set; }
    public string Status { get; private set; }
    public Guid AssignedTo { get; private set; }

    public Ticket(string subject, string description, string priority, Guid assignedTo)
    {
        Subject = subject;
        Description = description;
        Priority = priority;
        Status = "Open";
        AssignedTo = assignedTo;
    }

    public void Close()
    {
        Status = "Closed";
        Audit(UpdatedBy);
    }
}

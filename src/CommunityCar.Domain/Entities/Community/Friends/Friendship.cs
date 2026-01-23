using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Community.Friends;

public enum FriendshipStatus
{
    Pending,
    Accepted,
    Blocked
}

public class Friendship : BaseEntity
{
    public Guid RequesterId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public FriendshipStatus Status { get; private set; }

    // Parameterless constructor for EF
    private Friendship() { }

    public Friendship(Guid requesterId, Guid receiverId)
    {
        RequesterId = requesterId;
        ReceiverId = receiverId;
        Status = FriendshipStatus.Pending;
    }

    public void Accept()
    {
        Status = FriendshipStatus.Accepted;
        Audit(nameof(Friendship));
    }

    public void Block()
    {
        Status = FriendshipStatus.Blocked;
        Audit(nameof(Friendship));
    }
}

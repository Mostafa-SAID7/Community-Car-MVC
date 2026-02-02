using System.ComponentModel.DataAnnotations;

namespace CommunityCar.Application.Features.Community.Friends.ViewModels;

/// <summary>
/// View model for displaying friend requests (both pending and sent)
/// </summary>
public class FriendRequestsVM
{
    /// <summary>
    /// Friend requests received by the current user (pending approval)
    /// </summary>
    [Display(Name = "Pending Requests")]
    public IEnumerable<FriendRequestVM> PendingRequests { get; set; } = new List<FriendRequestVM>();

    /// <summary>
    /// Friend requests sent by the current user (awaiting response)
    /// </summary>
    [Display(Name = "Sent Requests")]
    public IEnumerable<FriendRequestVM> SentRequests { get; set; } = new List<FriendRequestVM>();

    /// <summary>
    /// Total count of pending requests
    /// </summary>
    public int PendingCount => PendingRequests.Count();

    /// <summary>
    /// Total count of sent requests
    /// </summary>
    public int SentCount => SentRequests.Count();

    /// <summary>
    /// Whether there are any requests to display
    /// </summary>
    public bool HasRequests => PendingCount > 0 || SentCount > 0;

    /// <summary>
    /// Helper property to get the most recent pending requests
    /// </summary>
    public IEnumerable<FriendRequestVM> RecentPendingRequests => 
        PendingRequests.OrderByDescending(r => r.RequestDate).Take(5);

    /// <summary>
    /// Helper property to get the most recent sent requests
    /// </summary>
    public IEnumerable<FriendRequestVM> RecentSentRequests => 
        SentRequests.OrderByDescending(r => r.RequestDate).Take(5);
}
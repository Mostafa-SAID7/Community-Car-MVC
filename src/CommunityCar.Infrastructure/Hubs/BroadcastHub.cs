using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Common.Interfaces.Repositories;
using System.Collections.Concurrent;

namespace CommunityCar.Infrastructure.Hubs;

[Authorize]
public class BroadcastHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private static readonly ConcurrentDictionary<string, string> _userConnections = new();
    private static readonly ConcurrentDictionary<string, HashSet<string>> _groupConnections = new();

    public BroadcastHub(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            
            // Join user to their personal broadcast group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_broadcast_{userId}");
            
            // Auto-join user to their groups' broadcast channels
            await JoinUserGroups();
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = _currentUserService.UserId;
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections.TryRemove(userId, out _);
            
            // Remove from all group connections
            foreach (var group in _groupConnections.Values)
            {
                group.Remove(Context.ConnectionId);
            }
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific group's broadcast channel
    /// </summary>
    public async Task JoinGroupBroadcast(string groupId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // Verify user has access to this group
        var userGuid = Guid.Parse(userId);
        var groupGuid = Guid.Parse(groupId);
        
        var hasAccess = await _unitOfWork.Groups.UserHasAccessAsync(userGuid, groupGuid);
        if (!hasAccess)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"group_broadcast_{groupId}");
        
        if (!_groupConnections.ContainsKey(groupId))
        {
            _groupConnections[groupId] = new HashSet<string>();
        }
        _groupConnections[groupId].Add(Context.ConnectionId);

        await Clients.Caller.SendAsync("JoinedGroupBroadcast", groupId);
    }

    /// <summary>
    /// Leave a specific group's broadcast channel
    /// </summary>
    public async Task LeaveGroupBroadcast(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group_broadcast_{groupId}");
        
        if (_groupConnections.ContainsKey(groupId))
        {
            _groupConnections[groupId].Remove(Context.ConnectionId);
        }

        await Clients.Caller.SendAsync("LeftGroupBroadcast", groupId);
    }

    /// <summary>
    /// Broadcast a new post to group members
    /// </summary>
    public async Task BroadcastNewPost(string groupId, object postData)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // Verify user has permission to post in this group
        var userGuid = Guid.Parse(userId);
        var groupGuid = Guid.Parse(groupId);
        
        var canPost = await _unitOfWork.Groups.UserCanPostAsync(userGuid, groupGuid);
        if (!canPost)
            return;

        // Broadcast to all group members
        await Clients.Group($"group_broadcast_{groupId}")
            .SendAsync("NewPostBroadcast", postData);
    }

    /// <summary>
    /// Broadcast post interaction (like, comment, etc.)
    /// </summary>
    public async Task BroadcastPostInteraction(string postId, string interactionType, object interactionData)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // Get the post to determine which groups to broadcast to
        var postGuid = Guid.Parse(postId);
        var post = await _unitOfWork.Posts.GetByIdAsync(postGuid);
        
        if (post == null)
            return;

        // If post belongs to a group, broadcast to that group
        if (post.GroupId.HasValue)
        {
            await Clients.Group($"group_broadcast_{post.GroupId}")
                .SendAsync("PostInteractionBroadcast", new
                {
                    PostId = postId,
                    InteractionType = interactionType,
                    Data = interactionData,
                    UserId = userId
                });
        }
        else
        {
            // If it's a public post, broadcast to all connected users
            await Clients.All.SendAsync("PostInteractionBroadcast", new
            {
                PostId = postId,
                InteractionType = interactionType,
                Data = interactionData,
                UserId = userId
            });
        }
    }

    /// <summary>
    /// Broadcast news interaction (like, share, comment, etc.)
    /// </summary>
    public async Task BroadcastNewsInteraction(string newsId, string interactionType, bool newValue, int count)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // Broadcast to all users viewing this news article
        await Clients.Group($"News_{newsId}")
            .SendAsync("NewsInteractionUpdate", newsId, interactionType, newValue, count);
    }

    /// <summary>
    /// Join an event group for real-time updates
    /// </summary>
    public async Task JoinEventGroup(string eventId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        // Verify event exists and is accessible
        var eventGuid = Guid.Parse(eventId);
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventGuid);
        
        if (eventEntity == null)
            return;

        // Join the event group for real-time updates
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Event_{eventId}");
        await Clients.Caller.SendAsync("JoinedEventGroup", eventId);
    }

    /// <summary>
    /// Leave an event group
    /// </summary>
    public async Task LeaveEventGroup(string eventId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Event_{eventId}");
        await Clients.Caller.SendAsync("LeftEventGroup", eventId);
    }

    /// <summary>
    /// Join a news article group for real-time updates
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", groupName);
    }

    /// <summary>
    /// Leave a news article group
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("LeftGroup", groupName);
    }

    /// <summary>
    /// Request access to posts in a specific group with enhanced functionality
    /// </summary>
    public async Task AccessPosts(string groupId, string accessLevel = "read")
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("AccessDenied", "Authentication required");
            return;
        }

        var userGuid = Guid.Parse(userId);
        var groupGuid = Guid.Parse(groupId);

        try
        {
            // Check if user already has access
            var hasAccess = await _unitOfWork.Groups.UserHasAccessAsync(userGuid, groupGuid);
            
            if (hasAccess)
            {
                await Clients.Caller.SendAsync("PostAccessGranted", new
                {
                    GroupId = groupId,
                    AccessLevel = accessLevel,
                    Message = "Access granted - you're already a member",
                    Timestamp = DateTime.UtcNow,
                    Success = true
                });
                
                // Join the group broadcast channel
                await JoinGroupBroadcast(groupId);
                
                // Send initial posts for this group
                await SendGroupPosts(groupId, 1, 10);
            }
            else
            {
                // Check if group allows public access or requires approval
                var group = await _unitOfWork.Groups.GetByIdAsync(groupGuid);
                if (group == null)
                {
                    await Clients.Caller.SendAsync("PostAccessDenied", new
                    {
                        GroupId = groupId,
                        Reason = "Group not found",
                        Timestamp = DateTime.UtcNow,
                        Success = false
                    });
                    return;
                }

                if (group.Privacy == Domain.Enums.Community.GroupPrivacy.Public)
                {
                    // Auto-join public groups
                    await _unitOfWork.Groups.AddMemberAsync(groupGuid, userGuid);
                    await _unitOfWork.SaveChangesAsync();
                    
                    await Clients.Caller.SendAsync("PostAccessGranted", new
                    {
                        GroupId = groupId,
                        AccessLevel = accessLevel,
                        Message = "Welcome to the group!",
                        Timestamp = DateTime.UtcNow,
                        Success = true
                    });
                    
                    // Join the group broadcast channel
                    await JoinGroupBroadcast(groupId);
                    
                    // Broadcast new member to group
                    await Clients.Group($"group_broadcast_{groupId}")
                        .SendAsync("NewMemberJoined", new
                        {
                            GroupId = groupId,
                            UserId = userId,
                            Timestamp = DateTime.UtcNow
                        });
                    
                    // Send initial posts for this group
                    await SendGroupPosts(groupId, 1, 10);
                }
                else
                {
                    // Send join request for private groups
                    await _unitOfWork.Groups.CreateJoinRequestAsync(groupGuid, userGuid);
                    await _unitOfWork.SaveChangesAsync();
                    
                    await Clients.Caller.SendAsync("PostAccessRequested", new
                    {
                        GroupId = groupId,
                        Message = "Join request sent to group administrators",
                        Timestamp = DateTime.UtcNow,
                        Success = false,
                        RequiresApproval = true
                    });
                    
                    // Notify group admins
                    var admins = await _unitOfWork.Groups.GetAdminsAsync(groupGuid);
                    foreach (var admin in admins)
                    {
                        if (_userConnections.TryGetValue(admin.Id.ToString(), out var adminConnectionId))
                        {
                            await Clients.Client(adminConnectionId)
                                .SendAsync("NewJoinRequest", new
                                {
                                    GroupId = groupId,
                                    UserId = userId,
                                    RequestedAccessLevel = accessLevel,
                                    Timestamp = DateTime.UtcNow
                                });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("PostAccessError", new
            {
                GroupId = groupId,
                Error = "An error occurred while processing your request",
                Details = ex.Message,
                Timestamp = DateTime.UtcNow,
                Success = false
            });
        }
    }

    /// <summary>
    /// Legacy method for backward compatibility
    /// </summary>
    public async Task RequestGroupPostAccess(string groupId, string accessLevel = "read")
    {
        await AccessPosts(groupId, accessLevel);
    }

    /// <summary>
    /// Send group posts to the requesting user
    /// </summary>
    private async Task SendGroupPosts(string groupId, int page = 1, int pageSize = 10)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        var userGuid = Guid.Parse(userId);
        var groupGuid = Guid.Parse(groupId);

        try
        {
            var posts = await _unitOfWork.Posts.GetGroupPostsAsync(groupGuid, page, pageSize);
            
            await Clients.Caller.SendAsync("GroupPostsUpdate", new
            {
                GroupId = groupId,
                Posts = posts,
                Page = page,
                PageSize = pageSize,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("GroupPostsError", new
            {
                GroupId = groupId,
                Error = "Failed to load group posts",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Approve or deny a join request
    /// </summary>
    public async Task ProcessJoinRequest(string groupId, string requestUserId, bool approved, string reason = "")
    {
        var adminUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(adminUserId))
            return;

        var adminGuid = Guid.Parse(adminUserId);
        var groupGuid = Guid.Parse(groupId);
        var requestUserGuid = Guid.Parse(requestUserId);

        try
        {
            // Verify admin has permission to approve/deny requests
            var isAdmin = await _unitOfWork.Groups.IsUserAdminAsync(adminGuid, groupGuid);
            if (!isAdmin)
            {
                await Clients.Caller.SendAsync("ProcessJoinRequestDenied", "Insufficient permissions");
                return;
            }

            if (approved)
            {
                await _unitOfWork.Groups.ApproveJoinRequestAsync(groupGuid, requestUserGuid);
                await _unitOfWork.SaveChangesAsync();

                // Notify the user their request was approved
                if (_userConnections.TryGetValue(requestUserId, out var userConnectionId))
                {
                    await Clients.Client(userConnectionId)
                        .SendAsync("JoinRequestApproved", new
                        {
                            GroupId = groupId,
                            Message = "Your join request has been approved!",
                            Timestamp = DateTime.UtcNow
                        });
                }

                // Broadcast new member to group
                await Clients.Group($"group_broadcast_{groupId}")
                    .SendAsync("NewMemberJoined", new
                    {
                        GroupId = groupId,
                        UserId = requestUserId,
                        Timestamp = DateTime.UtcNow
                    });
            }
            else
            {
                await _unitOfWork.Groups.DenyJoinRequestAsync(groupGuid, requestUserGuid);
                await _unitOfWork.SaveChangesAsync();

                // Notify the user their request was denied
                if (_userConnections.TryGetValue(requestUserId, out var userConnectionId))
                {
                    await Clients.Client(userConnectionId)
                        .SendAsync("JoinRequestDenied", new
                        {
                            GroupId = groupId,
                            Reason = reason,
                            Timestamp = DateTime.UtcNow
                        });
                }
            }

            await Clients.Caller.SendAsync("JoinRequestProcessed", new
            {
                GroupId = groupId,
                UserId = requestUserId,
                Approved = approved,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ProcessJoinRequestError", "Failed to process join request");
        }
    }

    /// <summary>
    /// Get real-time post updates for accessible groups with enhanced filtering
    /// </summary>
    public async Task GetAccessiblePosts(int page = 1, int pageSize = 10, string? category = null, string? sortBy = "recent")
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("AccessDenied", "Authentication required");
            return;
        }

        var userGuid = Guid.Parse(userId);
        
        try
        {
            // Get posts from user's accessible groups with filtering
            var posts = await _unitOfWork.Posts.GetAccessiblePostsAsync(userGuid, page, pageSize, category, sortBy);
            
            await Clients.Caller.SendAsync("AccessiblePostsUpdate", new
            {
                Posts = posts,
                Page = page,
                PageSize = pageSize,
                Category = category,
                SortBy = sortBy,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("AccessiblePostsError", "Failed to load posts");
        }
    }

    /// <summary>
    /// Subscribe to real-time updates for specific post types
    /// </summary>
    public async Task SubscribeToPostUpdates(string[] postTypes, string[] groupIds = null)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        foreach (var postType in postTypes)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"posts_{postType}");
        }

        if (groupIds != null)
        {
            foreach (var groupId in groupIds)
            {
                // Verify access before subscribing
                var userGuid = Guid.Parse(userId);
                var groupGuid = Guid.Parse(groupId);
                var hasAccess = await _unitOfWork.Groups.UserHasAccessAsync(userGuid, groupGuid);
                
                if (hasAccess)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"group_posts_{groupId}");
                }
            }
        }

        await Clients.Caller.SendAsync("PostUpdatesSubscribed", new
        {
            PostTypes = postTypes,
            GroupIds = groupIds,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Unsubscribe from post updates
    /// </summary>
    public async Task UnsubscribeFromPostUpdates(string[] postTypes, string[] groupIds = null)
    {
        foreach (var postType in postTypes)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"posts_{postType}");
        }

        if (groupIds != null)
        {
            foreach (var groupId in groupIds)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group_posts_{groupId}");
            }
        }

        await Clients.Caller.SendAsync("PostUpdatesUnsubscribed", new
        {
            PostTypes = postTypes,
            GroupIds = groupIds,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get user's group memberships and access levels
    /// </summary>
    public async Task GetUserGroupAccess()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        var userGuid = Guid.Parse(userId);
        
        try
        {
            var userGroups = await _unitOfWork.Groups.GetUserGroupsWithAccessLevelAsync(userGuid);
            
            await Clients.Caller.SendAsync("UserGroupAccessUpdate", new
            {
                Groups = userGroups.Select(g => new
                {
                    GroupId = g.Id,
                    Name = g.Name,
                    AccessLevel = g.UserAccessLevel,
                    IsAdmin = g.IsUserAdmin,
                    CanPost = g.CanUserPost,
                    CanModerate = g.CanUserModerate
                }),
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("UserGroupAccessError", "Failed to load group access information");
        }
    }

    /// <summary>
    /// Auto-join user to their groups' broadcast channels
    /// </summary>
    private async Task JoinUserGroups()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return;

        var userGuid = Guid.Parse(userId);
        var userGroups = await _unitOfWork.Groups.GetUserGroupsAsync(userGuid);

        foreach (var group in userGroups)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"group_broadcast_{group.Id}");
            
            if (!_groupConnections.ContainsKey(group.Id.ToString()))
            {
                _groupConnections[group.Id.ToString()] = new HashSet<string>();
            }
            _groupConnections[group.Id.ToString()].Add(Context.ConnectionId);
        }
    }

    /// <summary>
    /// Check if user is online
    /// </summary>
    public static bool IsUserOnline(string userId)
    {
        return _userConnections.ContainsKey(userId);
    }

    /// <summary>
    /// Get user's connection ID
    /// </summary>
    public static string? GetUserConnectionId(string userId)
    {
        _userConnections.TryGetValue(userId, out var connectionId);
        return connectionId;
    }

    /// <summary>
    /// Get active connections count for a group
    /// </summary>
    public static int GetGroupConnectionsCount(string groupId)
    {
        return _groupConnections.TryGetValue(groupId, out var connections) ? connections.Count : 0;
    }
}
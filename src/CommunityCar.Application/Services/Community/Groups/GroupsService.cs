using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Common.Interfaces.Services.Community.Groups;
using CommunityCar.Application.Common.Interfaces.Services.Account.Core;
using CommunityCar.Application.Features.Community.Groups.ViewModels;
using CommunityCar.Domain.Entities.Community.Groups;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community.Groups;

public class GroupsService : IGroupsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GroupsService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<GroupVM?> GetGroupByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(id);
        return group == null ? null : _mapper.Map<GroupVM>(group);
    }

    public async Task<GroupVM?> GetGroupBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.Groups.GetBySlugAsync(slug, cancellationToken);
        return group == null ? null : _mapper.Map<GroupVM>(group);
    }

    public async Task<GroupsSearchVM> SearchGroupsAsync(GroupsSearchVM request, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.Groups.SearchAsync(
            request.SearchTerm,
            request.Privacy,
            request.Category,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);

        var summaryItems = _mapper.Map<IEnumerable<GroupSummaryVM>>(items);

        return new GroupsSearchVM
        {
            Items = summaryItems.ToList(),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<GroupVM> CreateGroupAsync(CreateGroupVM request, CancellationToken cancellationToken = default)
    {
        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        var group = new Group(
            request.Name,
            request.Description,
            request.Privacy,
            currentUserId,
            request.Category,
            request.Rules,
            request.RequiresApproval,
            request.Location);

        if (!string.IsNullOrEmpty(request.NameAr) || !string.IsNullOrEmpty(request.DescriptionAr))
        {
            group.UpdateArabicContent(request.NameAr, request.DescriptionAr, request.CategoryAr, request.RulesAr, request.LocationAr);
        }

        if (!string.IsNullOrWhiteSpace(request.CoverImageUrl))
        {
            group.UpdateCoverImage(request.CoverImageUrl);
        }

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            group.UpdateAvatar(request.AvatarUrl);
        }

        if (request.Tags?.Any() == true)
        {
            foreach (var tag in request.Tags)
            {
                group.AddTag(tag);
            }
        }

        await _unitOfWork.Groups.AddAsync(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GroupVM>(group);
    }

    public async Task<GroupVM> UpdateGroupAsync(Guid id, UpdateGroupVM request, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(id);
        if (group == null) throw new ArgumentException("Group not found");

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Check if user can edit (owner or admin)
        if (group.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only edit your own groups");
        }

        group.UpdateBasicInfo(request.Name, request.Description, request.Category);
        group.UpdatePrivacy(request.Privacy, request.RequiresApproval);
        group.UpdateLocation(request.Location);

        if (!string.IsNullOrEmpty(request.NameAr) || !string.IsNullOrEmpty(request.DescriptionAr))
        {
            group.UpdateArabicContent(request.NameAr, request.DescriptionAr, request.CategoryAr, request.RulesAr, request.LocationAr);
        }

        if (!string.IsNullOrWhiteSpace(request.Rules))
        {
            group.UpdateRules(request.Rules);
        }

        if (!string.IsNullOrWhiteSpace(request.CoverImageUrl))
        {
            group.UpdateCoverImage(request.CoverImageUrl);
        }

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            group.UpdateAvatar(request.AvatarUrl);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GroupVM>(group);
    }

    public async Task<bool> DeleteGroupAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(id);
        if (group == null) return false;

        var currentUserIdString = _currentUserService.UserId ?? throw new UnauthorizedAccessException("User must be authenticated");
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
            throw new UnauthorizedAccessException("Invalid user ID");

        // Check if user can delete (owner or admin)
        if (group.OwnerId != currentUserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own groups");
        }

        await _unitOfWork.Groups.DeleteAsync(group);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IEnumerable<GroupVM>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groups = await _unitOfWork.Groups.GetByOwnerAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<GroupVM>>(groups);
    }

    public async Task<IEnumerable<GroupVM>> GetPopularGroupsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var groups = await _unitOfWork.Groups.GetPopularGroupsAsync(count, cancellationToken);
        return _mapper.Map<IEnumerable<GroupVM>>(groups);
    }

    public async Task<IEnumerable<GroupVM>> GetRecentlyActiveGroupsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var groups = await _unitOfWork.Groups.GetRecentlyActiveAsync(count, cancellationToken);
        return _mapper.Map<IEnumerable<GroupVM>>(groups);
    }

    public async Task<GroupsStatsVM> GetGroupsStatsAsync(CancellationToken cancellationToken = default)
    {
        var allGroups = await _unitOfWork.Groups.GetAllAsync();
        var groupsList = allGroups.ToList();

        var popularGroups = await GetPopularGroupsAsync(5, cancellationToken);
        var recentlyActiveGroups = await GetRecentlyActiveGroupsAsync(5, cancellationToken);

        var stats = new GroupsStatsVM
        {
            TotalGroups = groupsList.Count,
            PublicGroups = groupsList.Count(g => g.Privacy == GroupPrivacy.Public),
            PrivateGroups = groupsList.Count(g => g.Privacy == GroupPrivacy.Private),
            TotalMembers = groupsList.Sum(g => g.MemberCount),
            PopularGroups = _mapper.Map<List<GroupSummaryVM>>(popularGroups),
            RecentlyActiveGroups = _mapper.Map<List<GroupSummaryVM>>(recentlyActiveGroups),
            GroupsByCategory = groupsList
                .Where(g => !string.IsNullOrEmpty(g.Category))
                .GroupBy(g => g.Category!)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return stats;
    }

    public async Task<bool> JoinGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
        if (group == null) return false;

        // TODO: Implement actual membership logic
        // For now, just increment member count
        group.IncrementMemberCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> LeaveGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
        if (group == null) return false;

        // TODO: Implement actual membership logic
        // For now, just decrement member count
        group.DecrementMemberCount();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
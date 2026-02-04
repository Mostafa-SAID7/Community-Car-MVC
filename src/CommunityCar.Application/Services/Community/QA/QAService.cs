using AutoMapper;
using CommunityCar.Application.Common.Interfaces.Services.Community;
using CommunityCar.Application.Common.Interfaces.Repositories;
using CommunityCar.Application.Features.Community.QA.ViewModels;
using CommunityCar.Domain.Entities.Community.QA;
using CommunityCar.Domain.Entities.Shared;
using CommunityCar.Domain.Enums.Shared;
using CommunityCar.Domain.Enums.Community;

namespace CommunityCar.Application.Services.Community.QA;

public class QAService : IQAService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QAService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QASearchVM> SearchQuestionsAsync(QASearchVM request)
    {
        var questions = await _unitOfWork.QA.GetAllAsync();
        var queryable = questions.AsQueryable();

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            queryable = queryable.Where(q => 
                q.Title.ToLowerInvariant().Contains(searchTerm) ||
                q.Body.ToLowerInvariant().Contains(searchTerm) ||
                (q.CarMake != null && q.CarMake.ToLowerInvariant().Contains(searchTerm)) ||
                (q.CarModel != null && q.CarModel.ToLowerInvariant().Contains(searchTerm)) ||
                q.Tags.Any(t => t.ToLowerInvariant().Contains(searchTerm))
            );
        }

        // Apply difficulty filter
        if (request.Difficulty.HasValue)
        {
            queryable = queryable.Where(q => q.Difficulty == request.Difficulty.Value);
        }

        // Apply status filters
        if (request.Status.HasValue)
        {
            queryable = request.Status.Value switch
            {
                QAStatus.Open => queryable.Where(q => !q.IsSolved && !q.IsLocked),
                QAStatus.Solved => queryable.Where(q => q.IsSolved),
                QAStatus.Unanswered => queryable.Where(q => q.AnswerCount == 0),
                QAStatus.Pinned => queryable.Where(q => q.IsPinned),
                QAStatus.Locked => queryable.Where(q => q.IsLocked),
                _ => queryable
            };
        }

        // Apply car filters
        if (!string.IsNullOrWhiteSpace(request.CarMake))
        {
            queryable = queryable.Where(q => q.CarMake != null && q.CarMake.ToLowerInvariant() == request.CarMake.ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(request.CarModel))
        {
            queryable = queryable.Where(q => q.CarModel != null && q.CarModel.ToLowerInvariant() == request.CarModel.ToLowerInvariant());
        }

        if (request.CarYear.HasValue)
        {
            queryable = queryable.Where(q => q.CarYear == request.CarYear.Value);
        }

        // Apply tag filters
        if (request.Tags.Any())
        {
            var lowerTags = request.Tags.Select(t => t.ToLowerInvariant()).ToList();
            queryable = queryable.Where(q => q.Tags.Any(t => lowerTags.Contains(t.ToLowerInvariant())));
        }

        // Apply boolean filters
        if (request.IsPinned.HasValue)
        {
            queryable = queryable.Where(q => q.IsPinned == request.IsPinned.Value);
        }

        if (request.IsLocked.HasValue)
        {
            queryable = queryable.Where(q => q.IsLocked == request.IsLocked.Value);
        }

        // Apply author filter
        if (request.AuthorId.HasValue)
        {
            queryable = queryable.Where(q => q.AuthorId == request.AuthorId.Value);
        }

        // Apply date filters
        if (request.CreatedAfter.HasValue)
        {
            queryable = queryable.Where(q => q.CreatedAt >= request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            queryable = queryable.Where(q => q.CreatedAt <= request.CreatedBefore.Value);
        }

        if (request.LastActivityAfter.HasValue)
        {
            queryable = queryable.Where(q => q.LastActivityAt >= request.LastActivityAfter.Value);
        }

        if (request.LastActivityBefore.HasValue)
        {
            queryable = queryable.Where(q => q.LastActivityAt <= request.LastActivityBefore.Value);
        }

        // Apply engagement filters
        if (request.MinVotes.HasValue)
        {
            queryable = queryable.Where(q => q.VoteScore >= request.MinVotes.Value);
        }

        if (request.MaxVotes.HasValue)
        {
            queryable = queryable.Where(q => q.VoteScore <= request.MaxVotes.Value);
        }

        if (request.MinAnswers.HasValue)
        {
            queryable = queryable.Where(q => q.AnswerCount >= request.MinAnswers.Value);
        }

        if (request.MaxAnswers.HasValue)
        {
            queryable = queryable.Where(q => q.AnswerCount <= request.MaxAnswers.Value);
        }

        if (request.MinViews.HasValue)
        {
            queryable = queryable.Where(q => q.ViewCount >= request.MinViews.Value);
        }

        if (request.MaxViews.HasValue)
        {
            queryable = queryable.Where(q => q.ViewCount <= request.MaxViews.Value);
        }

        // Get total count before pagination
        var totalCount = queryable.Count();

        // Apply sorting
        queryable = request.SortBy switch
        {
            QASortBy.Newest => queryable.OrderByDescending(q => q.CreatedAt),
            QASortBy.Oldest => queryable.OrderBy(q => q.CreatedAt),
            QASortBy.MostVotes => queryable.OrderByDescending(q => q.VoteScore),
            QASortBy.LeastVotes => queryable.OrderBy(q => q.VoteScore),
            QASortBy.MostAnswers => queryable.OrderByDescending(q => q.AnswerCount),
            QASortBy.LeastAnswers => queryable.OrderBy(q => q.AnswerCount),
            QASortBy.MostViews => queryable.OrderByDescending(q => q.ViewCount),
            QASortBy.LeastViews => queryable.OrderBy(q => q.ViewCount),
            QASortBy.RecentActivity => queryable.OrderByDescending(q => q.LastActivityAt),
            QASortBy.Relevance => !string.IsNullOrWhiteSpace(request.SearchTerm) 
                ? queryable.OrderByDescending(q => CalculateRelevanceScore(q, request.SearchTerm))
                : queryable.OrderByDescending(q => q.CreatedAt),
            _ => queryable.OrderByDescending(q => q.IsPinned).ThenByDescending(q => q.LastActivityAt)
        };

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var pagedQuestions = queryable.Skip(skip).Take(request.PageSize).ToList();

        // Map to ViewModels
        var questionVMs = new List<QuestionVM>();
        foreach (var question in pagedQuestions)
        {
            var vm = _mapper.Map<QuestionVM>(question);
            
            // Load additional data
            var answers = await _unitOfWork.QA.GetAnswersByQuestionIdAsync(question.Id);
            vm.AnswerCount = answers.Count();
            vm.VoteCount = await _unitOfWork.Votes.GetVoteCountAsync(question.Id, EntityType.Question);
            vm.VoteScore = question.VoteScore;
            vm.ViewCount = question.ViewCount;
            
            questionVMs.Add(vm);
        }

        // Calculate pagination info
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
        var pagination = new CommunityCar.Application.Features.Shared.ViewModels.PaginationVM
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages,
            StartItem = skip + 1,
            EndItem = Math.Min(skip + request.PageSize, totalCount)
        };

        // Get stats
        var stats = await GetQAStatsAsync();

        // Get available filters
        var availableTags = await GetPopularTagsAsync(50);
        var availableCarMakes = await GetAvailableCarMakesAsync();

        return new QASearchVM
        {
            Questions = questionVMs,
            Pagination = pagination,
            Stats = stats,
            AvailableTags = availableTags,
            AvailableCarMakes = availableCarMakes
        };
    }

    private static double CalculateRelevanceScore(Question question, string searchTerm)
    {
        var score = 0.0;
        var lowerSearchTerm = searchTerm.ToLowerInvariant();
        
        // Title matches get highest score
        if (question.Title.ToLowerInvariant().Contains(lowerSearchTerm))
            score += 10.0;
        
        // Body matches get medium score
        if (question.Body.ToLowerInvariant().Contains(lowerSearchTerm))
            score += 5.0;
        
        // Car make/model matches get medium score
        if (question.CarMake?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 7.0;
        
        if (question.CarModel?.ToLowerInvariant().Contains(lowerSearchTerm) == true)
            score += 7.0;
        
        // Tag matches get high score
        if (question.Tags.Any(t => t.ToLowerInvariant().Contains(lowerSearchTerm)))
            score += 8.0;
        
        // Boost score based on engagement
        score += question.VoteScore * 0.1;
        score += question.AnswerCount * 0.5;
        score += question.ViewCount * 0.01;
        
        // Boost pinned questions
        if (question.IsPinned)
            score += 5.0;
        
        return score;
    }

    public async Task<IEnumerable<QuestionVM>> GetAllQuestionsAsync()
    {
        var request = new QASearchVM
        {
            Page = 1,
            PageSize = 1000,
            SortBy = QASortBy.RecentActivity
        };
        
        var response = await SearchQuestionsAsync(request);
        return response.Questions;
    }

    public async Task<QuestionVM?> GetQuestionByIdAsync(Guid id)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(id);
        if (question == null) return null;

        var vm = _mapper.Map<QuestionVM>(question);
        var answers = await _unitOfWork.QA.GetAnswersByQuestionIdAsync(id);
        vm.AnswerCount = answers.Count();
        vm.VoteCount = await _unitOfWork.Votes.GetVoteCountAsync(id, EntityType.Question);
        vm.VoteScore = question.VoteScore;
        vm.ViewCount = question.ViewCount;
        
        return vm;
    }

    public async Task<QuestionVM?> GetQuestionBySlugAsync(string slug)
    {
        var question = await _unitOfWork.QA.GetQuestionBySlugAsync(slug);
        if (question == null) return null;

        var vm = _mapper.Map<QuestionVM>(question);
        var answers = await _unitOfWork.QA.GetAnswersByQuestionIdAsync(question.Id);
        vm.AnswerCount = answers.Count();
        vm.VoteCount = await _unitOfWork.Votes.GetVoteCountAsync(question.Id, EntityType.Question);
        vm.VoteScore = question.VoteScore;
        vm.ViewCount = question.ViewCount;
        
        return vm;
    }

    public async Task<IEnumerable<AnswerVM>> GetAnswersByQuestionIdAsync(Guid questionId)
    {
        var answers = await _unitOfWork.QA.GetAnswersByQuestionIdAsync(questionId);
        var vms = _mapper.Map<IEnumerable<AnswerVM>>(answers).ToList();
        
        foreach (var vm in vms)
        {
            vm.VoteCount = await _unitOfWork.Votes.GetVoteCountAsync(vm.Id, EntityType.Answer);
        }
        
        return vms;
    }

    public async Task<QuestionVM> CreateQuestionAsync(string title, string body, Guid authorId, string? titleAr = null, string? bodyAr = null, DifficultyLevel difficulty = DifficultyLevel.Beginner, string? carMake = null, string? carModel = null, int? carYear = null, string? carEngine = null, List<string>? tags = null)
    {
        var question = new Question(title, body, authorId, difficulty);
        
        if (!string.IsNullOrEmpty(titleAr) || !string.IsNullOrEmpty(bodyAr))
        {
            question.UpdateArabicContent(titleAr, bodyAr);
        }
        
        if (!string.IsNullOrWhiteSpace(carMake) || !string.IsNullOrWhiteSpace(carModel) || carYear.HasValue || !string.IsNullOrWhiteSpace(carEngine))
        {
            question.SetCarDetails(carMake, carModel, carYear, carEngine);
        }
        
        if (tags?.Any() == true)
        {
            foreach (var tag in tags)
            {
                question.AddTag(tag);
            }
        }
        
        await _unitOfWork.QA.AddAsync(question);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<QuestionVM>(question);
    }

    public async Task<AnswerVM> CreateAnswerAsync(Guid questionId, string body, Guid authorId, string? bodyAr = null)
    {
        var answer = new Answer(body, questionId, authorId);
        
        if (!string.IsNullOrEmpty(bodyAr))
        {
            answer.UpdateArabicContent(bodyAr);
        }
        await _unitOfWork.QA.AddAnswerAsync(answer);
        
        // Update question answer count and activity
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            var answers = await _unitOfWork.QA.GetAnswersByQuestionIdAsync(questionId);
            question.UpdateAnswerCount(answers.Count() + 1);
            question.UpdateActivity(authorId.ToString());
            await _unitOfWork.QA.UpdateAsync(question);
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<AnswerVM>(answer);
    }

    public async Task MarkAsSolvedAsync(Guid questionId, Guid acceptedAnswerId)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            question.MarkAsSolved(acceptedAnswerId);
            await _unitOfWork.QA.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task AcceptAnswerAsync(Guid answerId)
    {
        var answer = await _unitOfWork.QA.GetAnswerByIdAsync(answerId);
        if (answer != null)
        {
            answer.Accept();
            await _unitOfWork.QA.UpdateAnswerAsync(answer);
            
            // Also mark the question as solved
            var question = await _unitOfWork.QA.GetByIdAsync(answer.QuestionId);
            if (question != null)
            {
                question.MarkAsSolved(answerId);
                await _unitOfWork.QA.UpdateAsync(question);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task VoteAsync(Guid entityId, EntityType entityType, Guid userId, VoteType voteType)
    {
        var existingVote = await _unitOfWork.Votes.GetUserVoteAsync(entityId, entityType, userId);

        if (existingVote != null)
        {
            if (existingVote.Type == voteType)
            {
                // Toggle off if same vote type
                await _unitOfWork.Votes.DeleteAsync(existingVote);
            }
            else
            {
                // Change vote type
                existingVote.ChangeVote(voteType);
                await _unitOfWork.Votes.UpdateAsync(existingVote);
            }
        }
        else
        {
            // Create new vote
            var vote = new Vote(entityId, entityType, userId, voteType);
            await _unitOfWork.Votes.AddAsync(vote);
        }

        // Update vote score on the entity
        var voteScore = await _unitOfWork.Votes.GetVoteScoreAsync(entityId, entityType);
        
        if (entityType == EntityType.Question)
        {
            var question = await _unitOfWork.QA.GetByIdAsync(entityId);
            if (question != null)
            {
                question.UpdateVoteScore(voteScore);
                await _unitOfWork.QA.UpdateAsync(question);
            }
        }
        else if (entityType == EntityType.Answer)
        {
            var answer = await _unitOfWork.QA.GetAnswerByIdAsync(entityId);
            if (answer != null)
            {
                answer.UpdateVoteScore(voteScore);
                await _unitOfWork.QA.UpdateAnswerAsync(answer);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetVoteCountAsync(Guid entityId, EntityType entityType)
    {
        return await _unitOfWork.Votes.GetVoteCountAsync(entityId, entityType);
    }

    public async Task MarkHelpfulAsync(Guid answerId, Guid userId)
    {
        // Implementation for marking answer as helpful
        var answer = await _unitOfWork.QA.GetAnswerByIdAsync(answerId);
        if (answer != null)
        {
            answer.IncrementHelpfulCount();
            await _unitOfWork.QA.UpdateAnswerAsync(answer);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task BookmarkQuestionAsync(Guid questionId, Guid userId)
    {
        // Implementation for bookmarking
        var bookmark = new Bookmark(questionId, EntityType.Question, userId);
        await _unitOfWork.Bookmarks.AddAsync(bookmark);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnbookmarkQuestionAsync(Guid questionId, Guid userId)
    {
        // Implementation for removing bookmark
        var bookmark = await _unitOfWork.Bookmarks.GetUserBookmarkAsync(questionId, EntityType.Question, userId);
        if (bookmark != null)
        {
            await _unitOfWork.Bookmarks.DeleteAsync(bookmark);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task IncrementViewCountAsync(Guid questionId, Guid? userId = null, string? ipAddress = null, string? userAgent = null)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            question.IncrementViewCount();
            await _unitOfWork.QA.UpdateAsync(question);
            
            // Track view
            var view = new View(questionId, EntityType.Question, ipAddress ?? "", userAgent ?? "", userId);
            await _unitOfWork.Views.AddAsync(view);
            
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task PinQuestionAsync(Guid questionId, Guid moderatorId)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            question.Pin();
            await _unitOfWork.QA.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UnpinQuestionAsync(Guid questionId, Guid moderatorId)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            question.Unpin();
            await _unitOfWork.QA.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task LockQuestionAsync(Guid questionId, string reason, Guid moderatorId)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            question.Lock(reason, moderatorId);
            await _unitOfWork.QA.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UnlockQuestionAsync(Guid questionId, Guid moderatorId)
    {
        var question = await _unitOfWork.QA.GetByIdAsync(questionId);
        if (question != null)
        {
            question.Unlock();
            await _unitOfWork.QA.UpdateAsync(question);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task VerifyAnswerAsync(Guid answerId, Guid expertId, string? note = null)
    {
        var answer = await _unitOfWork.QA.GetAnswerByIdAsync(answerId);
        if (answer != null)
        {
            answer.VerifyByExpert(expertId, note);
            await _unitOfWork.QA.UpdateAnswerAsync(answer);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveAnswerVerificationAsync(Guid answerId, Guid expertId)
    {
        var answer = await _unitOfWork.QA.GetAnswerByIdAsync(answerId);
        if (answer != null)
        {
            answer.RemoveExpertVerification();
            await _unitOfWork.QA.UpdateAnswerAsync(answer);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetPopularTagsAsync(int count = 20)
    {
        var questions = await _unitOfWork.QA.GetAllAsync();
        var tagCounts = new Dictionary<string, int>();
        
        foreach (var question in questions)
        {
            foreach (var tag in question.Tags)
            {
                var lowerTag = tag.ToLowerInvariant();
                tagCounts[lowerTag] = tagCounts.GetValueOrDefault(lowerTag, 0) + 1;
            }
        }
        
        return tagCounts
            .OrderByDescending(kvp => kvp.Value)
            .Take(count)
            .Select(kvp => kvp.Key)
            .ToList();
    }

    public async Task<List<string>> GetAvailableCarMakesAsync()
    {
        var questions = await _unitOfWork.QA.GetAllAsync();
        return questions
            .Where(q => !string.IsNullOrWhiteSpace(q.CarMake))
            .Select(q => q.CarMake!)
            .Distinct()
            .OrderBy(make => make)
            .ToList();
    }

    public async Task<QASearchStats> GetQAStatsAsync()
    {
        var questions = await _unitOfWork.QA.GetAllAsync();
        var answers = new List<Answer>();
        
        foreach (var question in questions)
        {
            var questionAnswers = await _unitOfWork.QA.GetAnswersByQuestionIdAsync(question.Id);
            answers.AddRange(questionAnswers);
        }
        
        return new QASearchStats
        {
            TotalQuestions = questions.Count(),
            SolvedQuestions = questions.Count(q => q.IsSolved),
            OpenQuestions = questions.Count(q => !q.IsSolved && !q.IsLocked),
            UnansweredQuestions = questions.Count(q => q.AnswerCount == 0),
            PinnedQuestions = questions.Count(q => q.IsPinned),
            LockedQuestions = questions.Count(q => q.IsLocked),
            TotalAnswers = answers.Count,
            TotalVotes = questions.Sum(q => q.VoteScore) + answers.Sum(a => a.VoteScore),
            TotalViews = questions.Sum(q => q.ViewCount)
        };
    }
}
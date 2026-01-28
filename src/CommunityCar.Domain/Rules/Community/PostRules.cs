namespace CommunityCar.Domain.Rules.Community;

/// <summary>
/// Business rules for posts
/// </summary>
public static class PostRules
{
    /// <summary>
    /// Rule: Post title must not be empty
    /// </summary>
    public class PostTitleRequiredRule : IBusinessRule
    {
        private readonly string _title;

        public PostTitleRequiredRule(string title)
        {
            _title = title;
        }

        public string Message => "Post title is required.";

        public Task<bool> IsBrokenAsync()
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(_title));
        }
    }

    /// <summary>
    /// Rule: Post content must not be empty
    /// </summary>
    public class PostContentRequiredRule : IBusinessRule
    {
        private readonly string _content;

        public PostContentRequiredRule(string content)
        {
            _content = content;
        }

        public string Message => "Post content is required.";

        public Task<bool> IsBrokenAsync()
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(_content));
        }
    }

    /// <summary>
    /// Rule: Post content must not exceed maximum length
    /// </summary>
    public class PostContentLengthRule : IBusinessRule
    {
        private readonly string _content;
        private readonly int _maxLength;

        public PostContentLengthRule(string content, int maxLength = 10000)
        {
            _content = content;
            _maxLength = maxLength;
        }

        public string Message => $"Post content cannot exceed {_maxLength} characters.";

        public Task<bool> IsBrokenAsync()
        {
            return Task.FromResult(!string.IsNullOrEmpty(_content) && _content.Length > _maxLength);
        }
    }

    /// <summary>
    /// Rule: Only post author can edit the post
    /// </summary>
    public class OnlyAuthorCanEditRule : IBusinessRule
    {
        private readonly Guid _currentUserId;
        private readonly Guid _postAuthorId;

        public OnlyAuthorCanEditRule(Guid currentUserId, Guid postAuthorId)
        {
            _currentUserId = currentUserId;
            _postAuthorId = postAuthorId;
        }

        public string Message => "Only the post author can edit this post.";

        public Task<bool> IsBrokenAsync()
        {
            return Task.FromResult(_currentUserId != _postAuthorId);
        }
    }

    /// <summary>
    /// Rule: Cannot delete post with comments (unless admin)
    /// </summary>
    public class CannotDeletePostWithCommentsRule : IBusinessRule
    {
        private readonly int _commentCount;
        private readonly bool _isAdmin;

        public CannotDeletePostWithCommentsRule(int commentCount, bool isAdmin)
        {
            _commentCount = commentCount;
            _isAdmin = isAdmin;
        }

        public string Message => "Cannot delete post that has comments. Please contact an administrator.";

        public Task<bool> IsBrokenAsync()
        {
            return Task.FromResult(_commentCount > 0 && !_isAdmin);
        }
    }

    /// <summary>
    /// Rule: Post must have valid category
    /// </summary>
    public class ValidCategoryRule : IBusinessRule
    {
        private readonly Guid? _categoryId;
        private readonly Func<Guid, Task<bool>> _categoryExistsCheck;

        public ValidCategoryRule(Guid? categoryId, Func<Guid, Task<bool>> categoryExistsCheck)
        {
            _categoryId = categoryId;
            _categoryExistsCheck = categoryExistsCheck;
        }

        public string Message => "Post must have a valid category.";

        public async Task<bool> IsBrokenAsync()
        {
            if (!_categoryId.HasValue)
                return false; // Category is optional

            return !await _categoryExistsCheck(_categoryId.Value);
        }
    }
}
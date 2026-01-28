using CommunityCar.Domain.Rules;

namespace CommunityCar.Domain.Rules.Account;

public class ViewFrequencyLimitRule : IBusinessRule
{
    private readonly DateTime? _lastViewTime;
    private readonly int _minimumMinutesBetweenViews;

    public ViewFrequencyLimitRule(DateTime? lastViewTime, int minimumMinutesBetweenViews = 5)
    {
        _lastViewTime = lastViewTime;
        _minimumMinutesBetweenViews = minimumMinutesBetweenViews;
    }

    public string Message => $"Please wait at least {_minimumMinutesBetweenViews} minutes between profile views.";

    public Task<bool> IsBrokenAsync()
    {
        if (!_lastViewTime.HasValue)
            return Task.FromResult(false);

        var timeSinceLastView = DateTime.UtcNow - _lastViewTime.Value;
        return Task.FromResult(timeSinceLastView.TotalMinutes < _minimumMinutesBetweenViews);
    }
}
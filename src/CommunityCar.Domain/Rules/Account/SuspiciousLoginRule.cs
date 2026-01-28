using CommunityCar.Domain.Rules;
using CommunityCar.Domain.Enums.Account;

namespace CommunityCar.Domain.Rules.Account;

public class SuspiciousLoginRule : IBusinessRule
{
    private readonly string _currentIpAddress;
    private readonly string _currentUserAgent;
    private readonly string _lastKnownIpAddress;
    private readonly string _lastKnownUserAgent;
    private readonly DateTime? _lastSuccessfulLogin;
    private readonly bool _isFromNewLocation;
    private readonly bool _isFromNewDevice;

    public string Message { get; private set; } = string.Empty;

    public SuspiciousLoginRule(
        string currentIpAddress,
        string currentUserAgent,
        string lastKnownIpAddress,
        string lastKnownUserAgent,
        DateTime? lastSuccessfulLogin,
        bool isFromNewLocation,
        bool isFromNewDevice)
    {
        _currentIpAddress = currentIpAddress ?? string.Empty;
        _currentUserAgent = currentUserAgent ?? string.Empty;
        _lastKnownIpAddress = lastKnownIpAddress ?? string.Empty;
        _lastKnownUserAgent = lastKnownUserAgent ?? string.Empty;
        _lastSuccessfulLogin = lastSuccessfulLogin;
        _isFromNewLocation = isFromNewLocation;
        _isFromNewDevice = isFromNewDevice;
    }

    public Task<bool> IsBrokenAsync()
    {
        var riskLevel = GetRiskLevel();
        var isSuspicious = riskLevel >= LoginRiskLevel.High;

        if (isSuspicious)
        {
            var reasons = GetSuspiciousReasons();
            Message = $"Suspicious login detected: {string.Join(", ", reasons)}";
        }

        return Task.FromResult(isSuspicious);
    }

    public LoginRiskLevel GetRiskLevel()
    {
        int riskScore = 0;

        // New location adds risk
        if (_isFromNewLocation)
            riskScore += 2;

        // New device adds risk
        if (_isFromNewDevice)
            riskScore += 2;

        // Different IP address adds risk
        if (!string.IsNullOrEmpty(_lastKnownIpAddress) && 
            !_currentIpAddress.Equals(_lastKnownIpAddress, StringComparison.OrdinalIgnoreCase))
            riskScore += 1;

        // Different user agent adds risk
        if (!string.IsNullOrEmpty(_lastKnownUserAgent) && 
            !_currentUserAgent.Equals(_lastKnownUserAgent, StringComparison.OrdinalIgnoreCase))
            riskScore += 1;

        // Long time since last login adds risk
        if (_lastSuccessfulLogin.HasValue)
        {
            var timeSinceLastLogin = DateTime.UtcNow - _lastSuccessfulLogin.Value;
            if (timeSinceLastLogin > TimeSpan.FromDays(90))
                riskScore += 2;
            else if (timeSinceLastLogin > TimeSpan.FromDays(30))
                riskScore += 1;
        }
        else
        {
            // No previous login history is suspicious
            riskScore += 1;
        }

        return riskScore switch
        {
            0 => LoginRiskLevel.None,
            1 => LoginRiskLevel.Low,
            <= 3 => LoginRiskLevel.Medium,
            _ => LoginRiskLevel.High
        };
    }

    private List<string> GetSuspiciousReasons()
    {
        var reasons = new List<string>();

        if (_isFromNewLocation)
            reasons.Add("new location");

        if (_isFromNewDevice)
            reasons.Add("new device");

        if (!string.IsNullOrEmpty(_lastKnownIpAddress) && 
            !_currentIpAddress.Equals(_lastKnownIpAddress, StringComparison.OrdinalIgnoreCase))
            reasons.Add("different IP address");

        if (!string.IsNullOrEmpty(_lastKnownUserAgent) && 
            !_currentUserAgent.Equals(_lastKnownUserAgent, StringComparison.OrdinalIgnoreCase))
            reasons.Add("different browser/device");

        if (_lastSuccessfulLogin.HasValue)
        {
            var timeSinceLastLogin = DateTime.UtcNow - _lastSuccessfulLogin.Value;
            if (timeSinceLastLogin > TimeSpan.FromDays(90))
                reasons.Add("long time since last login");
        }
        else
        {
            reasons.Add("no previous login history");
        }

        return reasons;
    }
}
namespace CommunityCar.Application.Features.Dashboard.System.ViewModels;

public class EnvironmentVariableVM
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsSecret { get; set; }
    public bool IsSystem { get; set; }
    public string Category { get; set; } = string.Empty;
}
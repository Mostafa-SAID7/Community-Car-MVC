using CommunityCar.Domain.Base;
using System.Text.Json;

namespace CommunityCar.Domain.Entities.Dashboard.System;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DataType { get; set; } = "string"; // string, int, bool, decimal, json
    public bool IsReadOnly { get; set; }
    public bool IsSecure { get; set; }
    public string? ValidationRules { get; set; }
    public string? DefaultValue { get; set; }

    public void UpdateValue(string newValue, string? updatedBy = null)
    {
        if (IsReadOnly)
            throw new InvalidOperationException("Cannot update read-only setting");

        Value = newValue;
        Audit(updatedBy);
    }

    public T GetValue<T>()
    {
        return DataType.ToLower() switch
        {
            "int" => (T)(object)int.Parse(Value),
            "bool" => (T)(object)bool.Parse(Value),
            "decimal" => (T)(object)decimal.Parse(Value),
            "json" => JsonSerializer.Deserialize<T>(Value)!,
            _ => (T)(object)Value
        };
    }
}
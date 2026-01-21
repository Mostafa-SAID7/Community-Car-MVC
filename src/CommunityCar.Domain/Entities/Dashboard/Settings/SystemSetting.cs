using System;
using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.Entities.Dashboard.Settings;

public class SystemSetting : BaseEntity
{
    public string Key { get; private set; }
    public string Value { get; private set; }
    public string Description { get; private set; }
    public bool IsEncrypted { get; private set; }

    public SystemSetting(string key, string value, string description, bool isEncrypted = false)
    {
        Key = key;
        Value = value;
        Description = description;
        IsEncrypted = isEncrypted;
    }

    public void UpdateValue(string newValue)
    {
        Value = newValue;
        Audit(UpdatedBy);
    }
}

namespace CommunityCar.AI.Configuration;

public class AISettings
{
    public const string SectionName = "AI";

    public string DefaultProvider { get; set; } = "Gemini";
    public ProviderSettings Gemini { get; set; } = new();
    public ProviderSettings HuggingFace { get; set; } = new();
}

public class ProviderSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ModelUrl { get; set; } = string.Empty; // Optional for some providers
}

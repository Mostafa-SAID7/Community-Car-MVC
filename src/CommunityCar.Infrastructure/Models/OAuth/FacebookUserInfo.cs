using System.Text.Json.Serialization;

namespace CommunityCar.Infrastructure.Models.OAuth;

public class FacebookUserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("picture")]
    public FacebookPicture? Picture { get; set; }
}

public class FacebookPicture
{
    [JsonPropertyName("data")]
    public FacebookPictureData? Data { get; set; }
}

public class FacebookPictureData
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

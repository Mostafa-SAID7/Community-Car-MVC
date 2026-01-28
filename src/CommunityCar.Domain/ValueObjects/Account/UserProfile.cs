using CommunityCar.Domain.Base;

namespace CommunityCar.Domain.ValueObjects.Account;

public class UserProfile : ValueObject
{
    public string FullName { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Bio { get; private set; }
    public string? City { get; private set; }
    public string? Country { get; private set; }
    public string? BioAr { get; private set; }
    public string? CityAr { get; private set; }
    public string? CountryAr { get; private set; }
    public string? Website { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? CoverImageUrl { get; private set; }

    public UserProfile(
        string fullName,
        string? firstName = null,
        string? lastName = null,
        string? bio = null,
        string? city = null,
        string? country = null,
        string? bioAr = null,
        string? cityAr = null,
        string? countryAr = null,
        string? website = null,
        string? profilePictureUrl = null,
        string? coverImageUrl = null)
    {
        FullName = fullName ?? string.Empty;
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        City = city;
        Country = country;
        BioAr = bioAr;
        CityAr = cityAr;
        CountryAr = countryAr;
        Website = website;
        ProfilePictureUrl = profilePictureUrl;
        CoverImageUrl = coverImageUrl;
    }

    // Parameterless constructor for EF Core
    private UserProfile()
    {
        FullName = string.Empty;
        FirstName = null;
        LastName = null;
        Bio = null;
        City = null;
        Country = null;
        BioAr = null;
        CityAr = null;
        CountryAr = null;
        Website = null;
        ProfilePictureUrl = null;
        CoverImageUrl = null;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FullName;
        yield return FirstName ?? string.Empty;
        yield return LastName ?? string.Empty;
        yield return Bio ?? string.Empty;
        yield return City ?? string.Empty;
        yield return Country ?? string.Empty;
        yield return BioAr ?? string.Empty;
        yield return CityAr ?? string.Empty;
        yield return CountryAr ?? string.Empty;
        yield return Website ?? string.Empty;
        yield return ProfilePictureUrl ?? string.Empty;
        yield return CoverImageUrl ?? string.Empty;
    }

    public static UserProfile Empty => new(string.Empty);

    public UserProfile UpdateBasicInfo(string fullName, string? firstName, string? lastName)
    {
        return new UserProfile(
            fullName,
            firstName,
            lastName,
            Bio,
            City,
            Country,
            BioAr,
            CityAr,
            CountryAr,
            Website,
            ProfilePictureUrl,
            CoverImageUrl);
    }

    public UserProfile UpdateBio(string? bio, string? bioAr = null)
    {
        return new UserProfile(
            FullName,
            FirstName,
            LastName,
            bio,
            City,
            Country,
            bioAr ?? BioAr,
            CityAr,
            CountryAr,
            Website,
            ProfilePictureUrl,
            CoverImageUrl);
    }

    public UserProfile UpdateLocation(string? city, string? country, string? cityAr = null, string? countryAr = null)
    {
        return new UserProfile(
            FullName,
            FirstName,
            LastName,
            Bio,
            city,
            country,
            BioAr,
            cityAr ?? CityAr,
            countryAr ?? CountryAr,
            Website,
            ProfilePictureUrl,
            CoverImageUrl);
    }

    public UserProfile UpdateProfilePicture(string? profilePictureUrl)
    {
        return new UserProfile(
            FullName,
            FirstName,
            LastName,
            Bio,
            City,
            Country,
            BioAr,
            CityAr,
            CountryAr,
            Website,
            profilePictureUrl,
            CoverImageUrl);
    }

    public UserProfile UpdateCoverImage(string? coverImageUrl)
    {
        return new UserProfile(
            FullName,
            FirstName,
            LastName,
            Bio,
            City,
            Country,
            BioAr,
            CityAr,
            CountryAr,
            Website,
            ProfilePictureUrl,
            coverImageUrl);
    }
}
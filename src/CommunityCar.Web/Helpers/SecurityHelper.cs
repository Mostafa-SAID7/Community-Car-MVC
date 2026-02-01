using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunityCar.Web.Helpers;

/// <summary>
/// Helper class for security-related operations
/// </summary>
public static class SecurityHelper
{
    /// <summary>
    /// Generates a secure random token
    /// </summary>
    public static string GenerateSecureToken(int length = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    /// <summary>
    /// Generates a cryptographically secure random string
    /// </summary>
    public static string GenerateRandomString(int length = 16, bool includeSpecialChars = false)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        
        var characterSet = includeSpecialChars ? chars + specialChars : chars;
        
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        
        var result = new StringBuilder(length);
        foreach (var b in bytes)
        {
            result.Append(characterSet[b % characterSet.Length]);
        }
        
        return result.ToString();
    }

    /// <summary>
    /// Hashes a password using PBKDF2
    /// </summary>
    public static string HashPassword(string password, out string salt)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[32];
        rng.GetBytes(saltBytes);
        salt = Convert.ToBase64String(saltBytes);
        
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    public static bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000, HashAlgorithmName.SHA256);
        var testHash = pbkdf2.GetBytes(32);
        var hashBytes = Convert.FromBase64String(hash);
        
        return CryptographicOperations.FixedTimeEquals(testHash, hashBytes);
    }

    /// <summary>
    /// Sanitizes HTML input to prevent XSS attacks
    /// </summary>
    public static string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // Remove script tags
        input = Regex.Replace(input, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", "", RegexOptions.IgnoreCase);
        
        // Remove javascript: and vbscript: protocols
        input = Regex.Replace(input, @"javascript:", "", RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"vbscript:", "", RegexOptions.IgnoreCase);
        
        // Remove event handlers
        input = Regex.Replace(input, @"on\w+\s*=", "", RegexOptions.IgnoreCase);
        
        // Remove potentially dangerous tags
        var dangerousTags = new[] { "script", "object", "embed", "link", "style", "iframe", "frame", "frameset" };
        foreach (var tag in dangerousTags)
        {
            input = Regex.Replace(input, $@"<{tag}\b[^<]*(?:(?!<\/{tag}>)<[^<]*)*<\/{tag}>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, $@"<{tag}[^>]*>", "", RegexOptions.IgnoreCase);
        }
        
        return input;
    }

    /// <summary>
    /// Validates if a URL is safe for redirection
    /// </summary>
    public static bool IsSafeRedirectUrl(string url, string allowedHost)
    {
        if (string.IsNullOrEmpty(url)) return false;
        
        // Check for absolute URLs
        if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
        {
            return string.Equals(absoluteUri.Host, allowedHost, StringComparison.OrdinalIgnoreCase);
        }
        
        // Check for relative URLs
        if (Uri.TryCreate(url, UriKind.Relative, out _))
        {
            return !url.StartsWith("//") && 
                   !url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase) &&
                   !url.StartsWith("data:", StringComparison.OrdinalIgnoreCase);
        }
        
        return false;
    }

    /// <summary>
    /// Generates a CSRF token
    /// </summary>
    public static string GenerateCsrfToken()
    {
        return GenerateSecureToken(32);
    }

    /// <summary>
    /// Validates CSRF token
    /// </summary>
    public static bool ValidateCsrfToken(string token, string expectedToken)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(expectedToken))
            return false;
            
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(token),
            Encoding.UTF8.GetBytes(expectedToken)
        );
    }

    /// <summary>
    /// Encrypts sensitive data using AES
    /// </summary>
    public static string EncryptData(string plainText, string key)
    {
        using var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        
        swEncrypt.Write(plainText);
        swEncrypt.Close();
        
        var encrypted = msEncrypt.ToArray();
        var result = new byte[aes.IV.Length + encrypted.Length];
        Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
        Array.Copy(encrypted, 0, result, aes.IV.Length, encrypted.Length);
        
        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Decrypts data encrypted with EncryptData
    /// </summary>
    public static string DecryptData(string cipherText, string key)
    {
        var fullCipher = Convert.FromBase64String(cipherText);
        
        using var aes = Aes.Create();
        aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        
        var iv = new byte[aes.IV.Length];
        var cipher = new byte[fullCipher.Length - iv.Length];
        
        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);
        
        aes.IV = iv;
        
        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(cipher);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        return srDecrypt.ReadToEnd();
    }

    /// <summary>
    /// Generates a secure file name
    /// </summary>
    public static string GenerateSecureFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var randomPart = GenerateSecureToken(8);
        
        return $"{timestamp}_{randomPart}{extension}";
    }

    /// <summary>
    /// Validates file type based on content (magic numbers)
    /// </summary>
    public static bool IsValidFileType(byte[] fileBytes, string[] allowedTypes)
    {
        if (fileBytes == null || fileBytes.Length < 4) return false;
        
        var fileSignatures = new Dictionary<string, byte[][]>
        {
            {
                "jpg", new[]
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                }
            },
            {
                "png", new[]
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            },
            {
                "gif", new[]
                {
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }
                }
            },
            {
                "pdf", new[]
                {
                    new byte[] { 0x25, 0x50, 0x44, 0x46 }
                }
            }
        };
        
        foreach (var allowedType in allowedTypes)
        {
            if (fileSignatures.TryGetValue(allowedType.ToLower(), out var signatures))
            {
                foreach (var signature in signatures)
                {
                    if (fileBytes.Take(signature.Length).SequenceEqual(signature))
                        return true;
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// Masks sensitive information for logging
    /// </summary>
    public static string MaskSensitiveData(string input, int visibleChars = 4)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= visibleChars)
            return input;
            
        var masked = new string('*', input.Length - visibleChars);
        return masked + input.Substring(input.Length - visibleChars);
    }

    /// <summary>
    /// Validates IP address format
    /// </summary>
    public static bool IsValidIpAddress(string ipAddress)
    {
        return System.Net.IPAddress.TryParse(ipAddress, out _);
    }

    /// <summary>
    /// Checks if an IP address is in a private range
    /// </summary>
    public static bool IsPrivateIpAddress(string ipAddress)
    {
        if (!System.Net.IPAddress.TryParse(ipAddress, out var ip))
            return false;
            
        var bytes = ip.GetAddressBytes();
        
        // Check for private IP ranges
        return (bytes[0] == 10) ||
               (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
               (bytes[0] == 192 && bytes[1] == 168) ||
               (bytes[0] == 127); // localhost
    }

    /// <summary>
    /// Generates a secure API key
    /// </summary>
    public static string GenerateApiKey(string prefix = "cc")
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var randomPart = GenerateSecureToken(24);
        return $"{prefix}_{timestamp}_{randomPart}";
    }

    /// <summary>
    /// Validates password complexity
    /// </summary>
    public static bool IsPasswordComplex(string password, int minLength = 8)
    {
        if (string.IsNullOrEmpty(password) || password.Length < minLength)
            return false;
            
        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));
        
        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    /// <summary>
    /// Rate limiting helper - checks if action is allowed
    /// </summary>
    public static bool IsActionAllowed(string key, int maxAttempts, TimeSpan timeWindow, Dictionary<string, (int Count, DateTime LastAttempt)> cache)
    {
        var now = DateTime.UtcNow;
        
        if (cache.TryGetValue(key, out var entry))
        {
            if (now - entry.LastAttempt > timeWindow)
            {
                // Reset counter if time window has passed
                cache[key] = (1, now);
                return true;
            }
            
            if (entry.Count >= maxAttempts)
            {
                return false;
            }
            
            cache[key] = (entry.Count + 1, now);
            return true;
        }
        
        cache[key] = (1, now);
        return true;
    }
}
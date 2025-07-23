using System.Security.Cryptography;
using System.Text;

namespace CaseBattleBackend.Helpers;

public static class SpValidate
{
    public static bool CheckUser(Dictionary<string, object?> properties, string token)
    {
        if (!properties.Remove("hash", out var hashObj))
            throw new Exception("Missing 'hash' field");

        var receivedHash = hashObj?.ToString();

        var checkString = string.Join("\n", properties
            .Where(kv => kv.Value != null && kv.Key != "hash")
            .OrderBy(kv => kv.Key)
            .Select(kv => $"{kv.Key}={kv.Value?.ToString()}"));

        using var sha256 = SHA256.Create();
        var secret = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));

        using var hmac = new HMACSHA256(secret);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(checkString));
        var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return hex == receivedHash;
    }

    public static bool ValidateWebhook(string requestBody, string base64Hash, string token)
    {
        var requestData = Encoding.UTF8.GetBytes(requestBody);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(token));
        var hashBytes = hmac.ComputeHash(requestData);
        var computedHash = Convert.ToBase64String(hashBytes);

        return base64Hash == computedHash;
    }

    public static string GenerateToken(string id, string token)
    {
        var byteArray = Encoding.UTF8.GetBytes($"{id}:{token}");

        return Convert.ToBase64String(byteArray);
    }
}
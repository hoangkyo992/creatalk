using System.Security.Cryptography;
using System.Text;
using Cms.Application.Services.Abstractions;
using Cms.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Cms.Infrastructure.Services;

public class ZnsMessageSender : IMessageSender
{
    public Task<SendMessageResponse> SendAsync(MessageProvider provider, AttendeeMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool IsValidSignature(HttpRequest request, string oaSecret)
    {
        // 1. Read the raw request body (must be done carefully in ASP.NET Core)
        // EnableBuffering is necessary to read the body without affecting subsequent reads
        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, leaveOpen: true);
        var rawRequestBody = reader.ReadToEndAsync().Result;
        request.Body.Position = 0; // Reset the stream position for the controller action to read later

        // 2. Get headers
        if (!request.Headers.TryGetValue("X-ZEvent-Signature", out var signatureHeader) ||
            !request.Headers.TryGetValue("timestamp", out var timestampHeader) ||
            !request.Headers.TryGetValue("appid", out var appIdHeader))
        {
            return false;
        }

        var receivedSignature = signatureHeader.ToString();
        var timestamp = timestampHeader.ToString();
        var appId = appIdHeader.ToString();

        // 3. Reconstruct the signature base string: sha256(appId + data + timeStamp + OAsecretKey)
        // The 'data' here is the raw JSON string
        var signatureBase = $"{appId}{rawRequestBody}{timestamp}{oaSecret}";

        // 4. Compute the HMAC-SHA256 signature
        var computedSignature = CalculateHmacSha256(signatureBase, oaSecret);

        // 5. Compare the computed signature with the received signature (case-insensitive)
        return string.Equals(computedSignature, receivedSignature, StringComparison.OrdinalIgnoreCase);
    }

    private static string CalculateHmacSha256(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);

        // Zalo expects the signature in lowercase hex string format
        var builder = new StringBuilder();
        foreach (var t in hashBytes)
        {
            builder.Append(t.ToString("x2"));
        }
        return builder.ToString();
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend.Tests.Fixtures;

public class JwtTokenGenerator
{
    private const string TestSigningKey = "YourSuperSecretKeyThatIsAtLeast32CharsLong!!";
    private const string TestIssuer = "BuckeyeMarketplace";
    private const string TestAudience = "BuckeyeMarketplaceUser";

    /// <summary>
    /// Generates a valid JWT token for testing with the specified userId
    /// </summary>
    public static string GenerateToken(int userId, string username = "testuser", string role = "Customer")
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestSigningKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

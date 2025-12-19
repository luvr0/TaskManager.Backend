using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Core.Interfaces;
using TaskManager.Core.UserAggregate;

namespace TaskManager.Infrastructure.Services;

public sealed class TokenService : ITokenService
{
  private readonly string _jwtSecret;
  private readonly string _jwtIssuer;
  private readonly string _jwtAudience;
  private readonly int _jwtExpirationMinutes;

  public TokenService(IConfiguration configuration)
  {
    _jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    _jwtIssuer = configuration["Jwt:Issuer"] ?? "TaskManager";
    _jwtAudience = configuration["Jwt:Audience"] ?? "TaskManager";
    _jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
  }

  public string GenerateAccessToken(User user)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_jwtSecret);

    var claims = new List<Claim>
    {
      new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
      new(JwtRegisteredClaimNames.Email, user.Email.Value),
      new(JwtRegisteredClaimNames.Name, user.Name.Value),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
      Issuer = _jwtIssuer,
      Audience = _jwtAudience,
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  public RefreshToken GenerateRefreshToken()
  {
    var randomNumber = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    var token = Convert.ToBase64String(randomNumber);
    return RefreshToken.From(token);
  }

  public UserId? ValidateAccessToken(string token)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(_jwtSecret);

    try
    {
      tokenHandler.ValidateToken(token, new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = _jwtIssuer,
        ValidateAudience = true,
        ValidAudience = _jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      }, out SecurityToken validatedToken);

      var jwtToken = (JwtSecurityToken)validatedToken;
      var userIdClaim = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

      return UserId.From(int.Parse(userIdClaim));
    }
    catch
    {
      return null;
    }
  }
}

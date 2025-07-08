using LeaderboardApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LeaderboardApp.Security
{
    public class TokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly GhcacDbContext _context;

        public TokenService(IOptions<JwtSettings> jwtSettings, GhcacDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public string GenerateToken(Participant participant)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, participant.Participantid.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, participant.Firstname),
                new Claim(ClaimTypes.Email, participant.Email) // Adding email as a claim
            };

            return GenerateTokenFromClaims(claims);
        }

        public string GenerateToken(ClaimsPrincipal principal)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, principal.FindFirstValue(ClaimTypes.NameIdentifier)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, principal.Identity.Name),
                new Claim(ClaimTypes.Email, principal.FindFirstValue(ClaimTypes.Email)) // Adding email from principal claims
            };

            return GenerateTokenFromClaims(claims);
        }

        private string GenerateTokenFromClaims(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.Add(_jwtSettings.TokenLifetime),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(Participant participant)
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            participant.Refreshtoken = refreshToken;
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            return ValidateToken(token, validateLifetime: false);
        }

        public ClaimsPrincipal GetPrincipalFromValidToken(string token)
        {
            return ValidateToken(token, validateLifetime: true);
        }

        private ClaimsPrincipal ValidateToken(string token, bool validateLifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<ClaimsPrincipal> ValidateRefreshTokenAsync(string refreshToken, Guid participantId)
        {
            var participant = await _context.Participants.FindAsync(participantId);

            if (participant == null || participant.Refreshtoken != refreshToken)
            {
                return null;
            }

            return GetPrincipalFromValidToken(refreshToken);
        }
    }
}

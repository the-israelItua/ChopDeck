using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChopDeck.Interfaces;
using ChopDeck.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace ChopDeck.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IOptions<AppSettings> appSettings)
        {
            _jwtSettings = appSettings.Value.JWT;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        }

        public string CreateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.GivenName, user.Name)
    };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer =  _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };


            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
using TradeJournal.Data.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TradeJournal.Models;
namespace TradeJournal.Services.token
{
    public class JwtService : IJwtTokenService
    {
        private readonly string _jwtSecret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _jwtExpiryMinutes;

        // setup tokenu
        public JwtService(string jwtSecret, string issuer, string audience, int jwtExpiryMinutes)
        {
            _jwtSecret = jwtSecret;
            _issuer = issuer;
            _audience = audience;
            _jwtExpiryMinutes = jwtExpiryMinutes;
        }

        public TokenDTO GenerateToken(User user)
        {
            // okreslamy co zawiera
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Email, user.Auth.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.Aes128CbcHmacSha256); // ło kurwa ale potwór

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtExpiryMinutes),
                signingCredentials: credentials
            );

            return new TokenDTO
            {
                A_Token = new JwtSecurityTokenHandler().WriteToken(token),
                Ref_Token = GenerateRefreshToken()
            };

        }

        private string GenerateRefreshToken()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPRSTUVYZWX0123456789";
            return new string(Enumerable.Repeat(chars, 35).Select(n => n[new Random().Next(n.Length)]).ToArray());

        }
    }
}

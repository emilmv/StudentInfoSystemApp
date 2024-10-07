using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentInfoSystemApp.Application.Interfaces;
using StudentInfoSystemApp.Application.Settings;
using StudentInfoSystemApp.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentInfoSystemApp.Application.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(ApplicationUser user, List<string> userRoles)
        {
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)), 
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim("ID", user.Id),
            new Claim("Username", user.UserName),
            new Claim("Email", user.Email),
            new Claim("FullName", user.FullName),
        };

            claims.AddRange(userRoles.Select(r => new Claim("Roles", r)).ToList());

            var secToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(secToken);
        }
    }
}

using Authentications.Features.Authenticate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentications.Features.GenerateToken
{
    internal class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration _configuration;

        public GenerateToken(IConfiguration configuration)
        { 
            _configuration = configuration;
        }
        public string GenerateJwtToken(TokenRequestDto request)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, request.PartyId.ToString()),
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Name, request.Name),
                //new Claim(ClaimTypes.Role, request.Roles),
                new Claim("KycStatus", request.KycStatus.ToString())
            };
            claims.AddRange(request.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var keyString = _configuration["JwtSettings:SecretKey"]
                            ?? throw new InvalidOperationException("JWT Secret Key Not Found.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

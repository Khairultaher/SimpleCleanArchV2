﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WeatherForecast.Application.Constants;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.Interfaces.Security;

namespace WeatherForecast.Infrastructure.Security
{
    public class JwtTokenHelper : IJwtTokenHelper
    {
        public JwtTokenModel GetAccessToken(string username, IEnumerable<Claim> claims)
        {
            //var claims = new[]
            //{
            //    new Claim(JwtRegisteredClaimNames.Sub,username),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};

            //if (additionalClaims is object)
            //{
            //    var claimList = new List<Claim>(claims);
            //    claimList.AddRange(additionalClaims);
            //    claims = claimList.ToArray();
            //}

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConstants.JwtSettings.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = TimeSpan.FromMinutes(AppConstants.JwtSettings.TokenTimeoutMinutes);

            var jwt = new JwtSecurityToken(
                issuer: AppConstants.JwtSettings.Issuer,
                audience: AppConstants.JwtSettings.Audience,
                expires: DateTime.UtcNow.Add(expiration),
                claims: claims,
                signingCredentials: creds
            );

            var strJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new JwtTokenModel
            {
                AccessToken = strJwt,
                ExpiresAt = DateTime.UtcNow.Add(expiration),
                RefreshToken = GetRefreshToken()
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string signingKey)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public string GetRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}

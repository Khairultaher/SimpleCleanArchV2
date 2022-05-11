using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WeatherForecast.Application.Common.Constants;
using WeatherForecast.Application.Common.Interfaces;
using WeatherForecast.Application.Common.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System;

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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JwtSettings.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = TimeSpan.FromMinutes(Constants.JwtSettings.TokenTimeoutMinutes);

            var jwt = new JwtSecurityToken(
                issuer: Constants.JwtSettings.Issuer,
                audience: Constants.JwtSettings.Audience,
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

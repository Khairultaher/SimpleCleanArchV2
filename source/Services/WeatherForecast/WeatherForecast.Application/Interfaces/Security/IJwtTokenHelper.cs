using WeatherForecast.Application.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace WeatherForecast.Application.Interfaces.Security
{
    public interface IJwtTokenHelper
    {
        JwtTokenModel GetAccessToken(string username, IEnumerable<Claim> claims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string signingKey);
    }
}

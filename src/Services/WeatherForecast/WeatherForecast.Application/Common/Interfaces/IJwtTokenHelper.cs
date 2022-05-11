using WeatherForecast.Application.Common.Models;
using System.Security.Claims;
using System.Collections.Generic;

namespace WeatherForecast.Application.Common.Interfaces
{
    public interface IJwtTokenHelper
    {
        JwtTokenModel GetAccessToken(string username, IEnumerable<Claim> claims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, string signingKey);
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.API.ViewModels;
using WeatherForecast.Application.Constants;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.Interfaces.Security;
using WeatherForecast.Infrastructure.Identity;

namespace WeatherForecast.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "AUTH", IgnoreApi = false)]
    public class RefreshTokenController : BaseController
    {
        private readonly IJwtTokenHelper _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        public RefreshTokenController(IJwtTokenHelper tokenService, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh(JwtTokenModel tokenModel)
        {

            if (tokenModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenModel.AccessToken;
            string refreshToken = tokenModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken, AppConstants.JwtSettings.SigningKey);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = await _userManager.FindByNameAsync(username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");

            var token = _tokenService.GetAccessToken(user.UserName, principal.Claims);
            user.RefreshToken = token.RefreshToken;


            // update database
            var response = await _userManager.UpdateAsync(user);
            if (!response.Succeeded)
            {
                return BadRequest(response.Errors);
            }
            return Ok(new { token = token });

        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            AuthenticationModel authentication = new AuthenticationModel();
            var user = authentication.Users.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;

            // update database
            //_userContext.SaveChanges();

            return NoContent();
        }
    }
}

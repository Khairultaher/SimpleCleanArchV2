﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeatherForecast.API.ViewModels;
using WeatherForecast.Application.Constants;
using WeatherForecast.Application.Extensions;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.Interfaces.Security;
using WeatherForecast.Infrastructure.Identity;

namespace WeatherForecast.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "AUTH", IgnoreApi = false)]
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenHelper _jwtTokenHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(ILogger<AuthController> logger, IConfiguration configuration
            , IJwtTokenHelper jwtTokenHelper
            , UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _configuration = configuration;
            _jwtTokenHelper = jwtTokenHelper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Return success/fail status</returns>
        /// <remarks>
        /// **Sample request body:**
        ///
        ///     {
        ///        "userName": "admin",
        ///        "passWord":  "Qwe@1234"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="401">Failed/Unauthorized</response>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel vm)
        {
            try
            {
                await Task.Delay(500);

                // My application logic to validate the user
                var user = await _userManager.FindByNameAsync(vm.UserName);
                if (user is null)
                {
                    return BadRequest("User not found.");
                }
                var singin = await _signInManager.PasswordSignInAsync(user, vm.PassWord, false, false);
                if (!singin.Succeeded)
                {
                    return BadRequest("Invalid password.");
                }

                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id ?? "")); // NameIdentifier is the ID for an object
                claims.Add(new Claim(ClaimTypes.Name, vm.UserName ?? "")); //  Name is just that a name       

                var userRoles = await _userManager.GetRolesAsync(user);
                var userClaims = await _userManager.GetClaimsAsync(user);

                // Add roles as multiple claims
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Optionally add other app specific claims as needed
                foreach (var item in userClaims)
                {
                    claims.Add(new Claim(item.Type, item.Value));
                }

                // create a new token with token helper and add our claim
                JwtTokenModel token = null!;
                if (AppConstants.UseJwtToken)
                {

                    token = _jwtTokenHelper.GetAccessToken(vm.UserName ?? "", claims);

                    // this need to be saved in database
                    user.RefreshToken = token.RefreshToken;
                    user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(AppConstants.JwtSettings.RefreshTokenExpiryMinutes);
                    var res = await _userManager.UpdateAsync(user);
                }
                else
                {
                    // implement cookie based login
                    var identity = new ClaimsIdentity(claims, "AppCookies");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync("AppCookies", claimsPrincipal);
                }

                return Ok(new { UserName = user.UserName, roles = userRoles, claims = userClaims, token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetExceptions());
            }
        }

        [HttpPost]
        [Route("Logout")]
        //[Authorize]
        public async Task<IActionResult> Logout(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(0);

                var res = await _userManager.UpdateAsync(user);
                if (!res.Succeeded) return BadRequest("Unable to logout!");
                await HttpContext.SignOutAsync("AppCookies");
                return Ok(response);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.GetExceptions());
            }
        }
    }
}

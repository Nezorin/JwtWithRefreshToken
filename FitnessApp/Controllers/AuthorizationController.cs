using DataAccesLayer.Entities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceLayer;
using ServiceLayer.Contracts;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly JWTOptions _jwtOptions;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthorizationController(IOptions<JWTOptions> jwtOptions, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("Authorize")]
        [AllowAnonymous]
        public async Task<ActionResult> Authorize([FromBody] AuthorizeRequest authorizeRequest)
        {

            var user = await _userManager.FindByNameAsync(authorizeRequest.UserName);
            if (user is not null)
            {
                var isPassValid = await _userManager.CheckPasswordAsync(user, authorizeRequest.Password);

                if (isPassValid)
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, user.UserName)
                    };
                    string jwtToken = _tokenService.GenerateJwtToken(claims);
                    user.RefreshToken = _tokenService.GenerateRefreshToken();
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpiryTimeInDays);
                    await _userManager.UpdateAsync(user);
                    return Ok(new AuthenticatedResponse
                    {
                        Token = jwtToken,
                        RefreshToken = user.RefreshToken
                    });
                }
            }

            return Unauthorized();
        }
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegistrationRequest registrationRequest)
        {
            var user = new ApplicationUser { UserName = registrationRequest.UserName, Birthday = registrationRequest.Birthday };
            var result = await _userManager.CreateAsync(user, registrationRequest.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        [Route("refreshToken")]
        public async Task<IActionResult> RefreshToken(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var login = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = await _userManager.FindByNameAsync(login);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid client request");
            var newAccessToken = _tokenService.GenerateJwtToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User.Identity.Name;
            if (username is null)
                return BadRequest();
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return NotFound();
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}

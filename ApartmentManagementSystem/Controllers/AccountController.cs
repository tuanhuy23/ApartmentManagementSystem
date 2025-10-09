using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public AccountController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet]
        [Authorize(Policy = "Permissions.UserPermissions.Read")]
        public IActionResult Get()
        {
            return Ok("test");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var tokenResponse = _tokenService.LoginAsync(request);
            return Ok(tokenResponse);
        }
    }
}

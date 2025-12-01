using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Route("account/")]
    [ApiController]
    [ApiExceptionFilter]
    public class AccountController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        public AccountController(ITokenService tokenService, IAccountService accountService)
        {
            _tokenService = tokenService;
            _accountService = accountService;
        }

        [HttpGet("account-info")]
        [ProducesResponseType(typeof(ResponseData<AccountInfoResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var response = await _accountService.GetAccountInfo();
            return Ok(new ResponseData<AccountInfoResponseDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet("apartment-building")]
        [ProducesResponseType(typeof(ResponseData<ApartmentBuildingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApartment()
        {
            var response = await _accountService.GetCurrentApartment();
            return Ok(new ResponseData<ApartmentBuildingDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseData<TokenResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var tokenResponse = await _tokenService.LoginAsync(request);
            return Ok(new ResponseData<TokenResponseDto>(System.Net.HttpStatusCode.OK, tokenResponse, null, null));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseData<TokenResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var tokenResponse = await _tokenService.RefreshTokenAsync(request.RefreshToken);
            return Ok(new ResponseData<TokenResponseDto>(System.Net.HttpStatusCode.OK, tokenResponse, null, null));
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseData<TokenResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            await _tokenService.LogoutAsync(request.RefreshToken);
            return Ok(new ResponseData<TokenResponseDto>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ResponseData<ChangePasswordResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var response = await _tokenService.ChangePassword(request);
            return Ok(new ResponseData<ChangePasswordResponseDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }
    }
}

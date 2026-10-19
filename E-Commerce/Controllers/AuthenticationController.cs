using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationServices _authenticationServices;

        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            _authenticationServices = authenticationServices;
        }

        #region Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken ct = default)
        {
            var result = await _authenticationServices.RegisterAsync(registerDto, ct);
            if (!result.IsSuccess)
            {
                return ToProblem(result.Errors);
            }
            return ToActionResult(result);
        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, CancellationToken ct = default)
        {
            var result = await _authenticationServices.LoginAsync(loginDto, ct);
            if (!result.IsSuccess)
            {
                return ToProblem(result.Errors);
            }
            return ToActionResult(result);
        }
        #endregion

        #region CheckEmail
        [HttpGet("EmailExists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery]string email, CancellationToken ct)
         => ToActionResult(await _authenticationServices.CheckEmailAsync(email, ct));
        #endregion

        #region CurrentUser
        [HttpGet("CurrentUser")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser([FromQuery] string email, CancellationToken ct)
        => ToActionResult(await _authenticationServices.GetCurrentuser(email, ct));
        #endregion

        #region Address
        [HttpGet("Address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetUserAddress([FromQuery] string email, CancellationToken ct)
         => ToActionResult(await _authenticationServices.GetUserAddressAsync(email, ct));
        #endregion

        #region UpdateAddress
        [HttpPost("Address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress([FromQuery] string email, [FromBody] AddressDto addressDto ,CancellationToken ct)
         => ToActionResult(await _authenticationServices.UpdateUserAddressAsync(addressDto,email, ct));
        #endregion
    }
}

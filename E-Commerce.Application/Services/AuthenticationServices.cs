using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IIdentityServices _identityServices;
        private readonly ITokenServices tokenServices;

        public AuthenticationServices(IIdentityServices identityServices , ITokenServices tokenServices)
        {
         _identityServices = identityServices;
            this.tokenServices = tokenServices;
        }

        public async Task<Result<bool>> CheckEmailAsync(string email, CancellationToken ct = default)
            => await _identityServices.EmailExistAsync(email, ct);

        public async Task<Result<UserDto>> GetCurrentuser(string email, CancellationToken ct = default)
        {
           var user = await _identityServices.FindUserByEmailAsync(email, ct);

            if (!user.IsSuccess) return Result<UserDto>.Fail(user.Errors);

            var userResult = user.data;

            var role = await _identityServices.GetUserRolesAsync(userResult.Email);

            if(!role.IsSuccess) return Result<UserDto>.Fail(role.Errors);

            var token = tokenServices.CreateToken(userResult.Id,userResult.UserName,userResult.Email,role.data);
            return Result<UserDto>.Ok(new UserDto
            {
                DisplayName = userResult.DisplayName,
                Email = userResult.Email,
                Token = token
            });
        }

        public async Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default)
        {
            var result = await _identityServices.GetAddressByEmail(email, ct);

            if(!result.IsSuccess) return Result<AddressDto>.Fail(result.Errors);

            return Result<AddressDto>.Ok(result.data);
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            var userResult = await _identityServices.FindUserByEmailAsync(loginDto.Email, ct);
            if (!userResult.IsSuccess) return Result<UserDto>.Fail(Error.NotFound("user not found"));

            var isPasswordValid = await _identityServices.CheckPasswordAsync(loginDto.Email, loginDto.Password, ct);
            if(!isPasswordValid.IsSuccess) return Result<UserDto>.Fail(Error.InvalidCredentials("Invalid email or password"));

            var roleResult = await _identityServices.GetUserRolesAsync(userResult.data.Email, ct);
            var token = tokenServices.CreateToken(userResult.data.Id, userResult.data.UserName, userResult.data.Email, roleResult.data);
            return Result<UserDto>.Ok(new UserDto
            {
                DisplayName = userResult.data.DisplayName,
                Email = userResult.data.Email,
                Token = token
            });
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var result = await _identityServices.CreateUserAsync(registerDto, ct);

            if (!result.IsSuccess || result.data == null) return Result<UserDto>.Fail(result.Errors);

            var roleResult = await _identityServices.GetUserRolesAsync(result.data.Email, ct);
            var token = tokenServices.CreateToken(result.data.Id, result.data.UserName, result.data.Email, roleResult.data);
            return Result<UserDto>.Ok(new UserDto
            {
                DisplayName = result.data.DisplayName,
                Email = result.data.Email,
                Token = token
            });
        }

        public async Task<Result<AddressDto>> UpdateUserAddressAsync(AddressDto addressDto, string email, CancellationToken ct = default)
          => await _identityServices.UpdateAddressAsync(email, addressDto, ct);
    }
}

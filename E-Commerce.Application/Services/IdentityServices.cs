using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class IdentityServices : IIdentityServices
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityServices(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Result<IdentityUserResult>.Fail(Error.NotFound("User not found"));

            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.UserName, user.DisplayName, user.Email));

        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Result<bool>.Fail(Error.NotFound("User not found"));

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            return Result<bool>.Ok(isValid);
        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }

            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.UserName, user.DisplayName, user.Email));
        }

        public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Result<IEnumerable<string>>.Fail(Error.NotFound("User not found"));

            var roles = await _userManager.GetRolesAsync(user);
            return Result<IEnumerable<string>>.Ok(roles);
        }

        public async Task<Result<AddressDto>> GetAddressByEmail(string email, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user is null) return Result<AddressDto>.Fail(Error.NotFound("User not found"));
            if (user.Address is null) return Result<AddressDto>.Fail(Error.NotFound("Address not found"));

            return Result<AddressDto>.Ok(new AddressDto
            {
                Country =  user.Address.Country,
                City = user.Address.City,
                Street = user.Address.Street,
                FirstName = user.Address.FirstName,
                LastName = user.Address.LastName,
            });
        }

        public async Task<Result<AddressDto>> UpdateAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);
            if (user is null) return Result<AddressDto>.Fail(Error.NotFound("User not found"));

            if (user.Address is null)
            {
                user.Address = new Address()
                {
                    Country = addressDto.Country,
                    City = addressDto.City,
                    Street = addressDto.Street,
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName
                };
            }
            else
            {
                user.Address.Country = addressDto.Country;
                    user.Address.City = addressDto.City;
                user.Address.Street = addressDto.Street;
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
            }
            var result = await _userManager.UpdateAsync(user);

            if(!result.Succeeded) return Result<AddressDto>.Fail(Error.Failure("This address can not be updated"));

            return Result<AddressDto>.Ok(addressDto);
        }

        public async Task<Result<bool>> EmailExistAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return Result<bool>.Ok(user is not null);
        }
            
    }
}

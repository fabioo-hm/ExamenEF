using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Auth;
using static Api.Services.UserService;

namespace Api.Services;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterDto model);
    Task<DataUserDto> GetTokenAsync(LoginDto model, CancellationToken ct = default);
    Task<string> AddRoleAsync(AddRoleDto model);
    Task<DataUserDto> RefreshTokenAsync(string refreshToken);
    Task<List<UserListDto>> GetAllUsersAsync();
    Task<OperationResult> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<OperationResult> DeleteUserAsync(int id);

}
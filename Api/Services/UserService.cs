using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.DTOs.Auth;
using Api.Helpers;
using Application.Abstractions;
using Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class UserService : IUserService
{
    private readonly JWT _jwt;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<UserMember> _passwordHasher;
    public UserService(IOptions<JWT> jwt, IUnitOfWork unitOfWork, IPasswordHasher<UserMember> passwordHasher)
    {
        _jwt = jwt.Value;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }
    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        if (registerDto == null)
            throw new ArgumentNullException(nameof(registerDto));

        var usuarioExiste = _unitOfWork.UserMembers
            .Find(u => u.Username.ToLower() == registerDto.Username.ToLower())
            .FirstOrDefault();

        if (usuarioExiste != null)
            return $"El usuario {registerDto.Username} ya se encuentra registrado.";

        var usuario = new UserMember
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            Password = _passwordHasher.HashPassword(new UserMember(), registerDto.Password)
        };

        
        var roleName = string.IsNullOrWhiteSpace(registerDto.Role)
            ? UserAuthorization.rol_default.ToString()  
            : registerDto.Role.Trim();

        
        if (!Enum.GetNames(typeof(UserAuthorization.Roles)).Any(r => 
            r.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
        {
            
            roleName = UserAuthorization.rol_default.ToString();
        }

        
        var rol = _unitOfWork.Roles
            .Find(r => r.Name.ToLower() == roleName.ToLower())
            .FirstOrDefault();

        
        if (rol == null)
        {
            rol = new Rol 
            { 
                Name = roleName, 
                Description = $"{roleName} role" 
            };
            await _unitOfWork.Roles.AddAsync(rol);
            await _unitOfWork.SaveChanges();
        }

        try
        {
            
            usuario.UserMemberRols = new List<UserMemberRol>
            {
                new UserMemberRol { RolId = rol.Id }
            };

            await _unitOfWork.UserMembers.AddAsync(usuario);
            await _unitOfWork.SaveChanges();

            return $"✅ Usuario {registerDto.Username} registrado con el rol '{roleName}'.";
        }
        catch (Exception ex)
        {
            return $"Error al registrar usuario: {ex.Message}";
        }
    }
    public async Task<DataUserDto> GetTokenAsync(LoginDto model, CancellationToken ct = default)
    {
        var dto = new DataUserDto { IsAuthenticated = false };

        
        var username = model.Username?.Trim();
        var password = model.Password ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        
        var usuario = await _unitOfWork.UserMembers.GetByUserNameAsync(username, ct);
        if (usuario is null)
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        
        var verification = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            dto.Message = "Usuario o contraseña inválidos.";
            return dto;
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.Password = _passwordHasher.HashPassword(usuario, password);
            await _unitOfWork.UserMembers.UpdateAsync(usuario, ct);
        
        }

        
        var roles = usuario.UserMemberRols
            .Select(ur => ur.Rol.Name)
            .ToList();
        usuario.RefreshTokens ??= new List<RefreshToken>();

        
        await _unitOfWork.ExecuteInTransactionAsync(async _ =>
        {
            
            foreach (var t in usuario.RefreshTokens.Where(t => t.IsActive))
            {
                t.Revoked = DateTime.UtcNow;            
            }

            var refresh = CreateRefreshToken();
            usuario.RefreshTokens.Add(refresh);

            await _unitOfWork.SaveChanges(ct);
        }, ct);

        
        var jwt = CreateJwtToken(usuario);

        
        var currentRefresh = usuario.RefreshTokens.OrderByDescending(t => t.Created).First();

        dto.IsAuthenticated = true;
        dto.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
        dto.Email = usuario.Email;
        dto.UserName = usuario.Username;
        dto.Roles = roles;
        dto.RefreshToken = currentRefresh.Token;
        dto.RefreshTokenExpiration = DateTime.SpecifyKind(currentRefresh.Expires, DateTimeKind.Utc);

        return dto;
    }
    private JwtSecurityToken CreateJwtToken(UserMember usuario)
    {
        var roles = usuario.Rols;
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role.Name));
        }
        var claims = new[]
        {
                                new Claim(JwtRegisteredClaimNames.Sub, usuario.Username),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                                new Claim("uid", usuario.Id.ToString())
                        }
        .Union(roleClaims);
        if (string.IsNullOrEmpty(_jwt.Key))
        {
            throw new InvalidOperationException("JWT Key cannot be null or empty.");
        }
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                Expires = DateTime.UtcNow.AddDays(10),
                Created = DateTime.UtcNow
            };
        }
    }
    public async Task<string> AddRoleAsync(AddRoleDto model)
    {
        if (string.IsNullOrEmpty(model.Username))
            return "Username cannot be null or empty.";

        var user = await _unitOfWork.UserMembers.GetByUserNameAsync(model.Username);
        if (user == null)
            return $"User {model.Username} does not exist.";

        if (string.IsNullOrEmpty(model.Password))
            return $"Password cannot be null or empty.";

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
        if (result != PasswordVerificationResult.Success)
            return "Invalid credentials.";

        if (string.IsNullOrWhiteSpace(model.Role))
            return "Role name cannot be null or empty.";

        var roleName = model.Role.Trim();

        var existingRole = _unitOfWork.Roles
            .Find(u => EF.Functions.ILike(u.Name, roleName))
            .FirstOrDefault();

        if (existingRole == null)
        {
            var newRole = new Rol
            {
                Name = roleName,
                Description = $"{roleName} role"
            };
            await _unitOfWork.Roles.AddAsync(newRole);
            await _unitOfWork.SaveChanges();
            existingRole = newRole;
        }

        
        var existingUserRoles = await _unitOfWork.UserMemberRoles.GetByUserIdAsync(user.Id);

    
        if (existingUserRoles.Any())
        {
            _unitOfWork.UserMemberRoles.RemoveRange(existingUserRoles);
        }

    
        await _unitOfWork.UserMemberRoles.AddAsync(new UserMemberRol
        {
            UserMemberId = user.Id,
            RolId = existingRole.Id
        });

        
        await _unitOfWork.SaveChanges();

        return $"Rol {roleName} correctamente asignado a {model.Username}.";
    }

    public async Task<DataUserDto> RefreshTokenAsync(string refreshToken)
    {
        var dataUserDto = new DataUserDto();

        var usuario = await _unitOfWork.UserMembers
                        .GetByRefreshTokenAsync(refreshToken);

        if (usuario == null)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not assigned to any user.";
            return dataUserDto;
        }

        var refreshTokenBd = usuario.RefreshTokens.Single(x => x.Token == refreshToken);

        if (!refreshTokenBd.IsActive)
        {
            dataUserDto.IsAuthenticated = false;
            dataUserDto.Message = $"Token is not active.";
            return dataUserDto;
        }
        
        refreshTokenBd.Revoked = DateTime.UtcNow;
        
        var newRefreshToken = CreateRefreshToken();
        usuario.RefreshTokens.Add(newRefreshToken);
        await _unitOfWork.UserMembers.UpdateAsync(usuario);
        await _unitOfWork.SaveChanges();
        
        dataUserDto.IsAuthenticated = true;
        JwtSecurityToken jwtSecurityToken = CreateJwtToken(usuario);
        dataUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        dataUserDto.Email = usuario.Email;
        dataUserDto.UserName = usuario.Username;
        dataUserDto.Roles = usuario.Rols
                                        .Select(u => u.Name)
                                        .ToList();
        dataUserDto.RefreshToken = newRefreshToken.Token;
        dataUserDto.RefreshTokenExpiration = newRefreshToken.Expires;
        return dataUserDto;
    }
    
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    public async Task<List<UserListDto>> GetAllUsersAsync()
    {
        
        var usersQuery = _unitOfWork.UserMembers.GetAllWithRoles();

        var list = usersQuery
            .Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Roles = u.UserMemberRols != null
                    ? u.UserMemberRols
                        .Where(ur => ur.Rol != null)
                        .Select(ur => ur.Rol.Name)
                        .ToList()
                    : new List<string>()
            })
            .ToList();

        return await Task.FromResult(list);
    }


    
    public async Task<OperationResult> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = _unitOfWork.UserMembers
            .Find(u => u.Id == id)
            .FirstOrDefault();
            
        if (user == null)
            return new OperationResult { Success = false, Message = "Usuario no encontrado." };

        
        if (!string.IsNullOrWhiteSpace(dto.Username))
            user.Username = dto.Username.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Email))
            user.Email = dto.Email.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Password))
            user.Password = _passwordHasher.HashPassword(user, dto.Password);

        
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            var roleName = dto.Role.Trim();
            
            
            if (!Enum.GetNames(typeof(UserAuthorization.Roles)).Any(r => 
                r.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
            {
                roleName = UserAuthorization.rol_default.ToString();
            }
            
            
            var existingRole = _unitOfWork.Roles
                .Find(r => EF.Functions.ILike(r.Name, roleName))
                .FirstOrDefault();

        
            if (existingRole == null)
            {
                existingRole = new Rol { 
                    Name = roleName, 
                    Description = $"{roleName} role" 
                };
                await _unitOfWork.Roles.AddAsync(existingRole);
                await _unitOfWork.SaveChanges();
            }

            
            var existingUserRoles = await _unitOfWork.UserMemberRoles.GetByUserIdAsync(user.Id);

            
            if (existingUserRoles.Any())
            {
                _unitOfWork.UserMemberRoles.RemoveRange(existingUserRoles);
            }

            
            var newUserRole = new UserMemberRol
            {
                UserMemberId = user.Id,
                RolId = existingRole.Id
            };
            
            await _unitOfWork.UserMemberRoles.AddAsync(newUserRole);
            
            
            await _unitOfWork.SaveChanges();
        }

        
        await _unitOfWork.UserMembers.UpdateAsync(user);
        await _unitOfWork.SaveChanges();

        return new OperationResult { Success = true, Message = "Usuario actualizado correctamente." };
    }

        public async Task<OperationResult> DeleteUserAsync(int id)
    {
        var user = _unitOfWork.UserMembers.Find(u => u.Id == id).FirstOrDefault();
        if (user == null)
            return new OperationResult { Success = false, Message = "Usuario no encontrado." };

        var userRoles = await _unitOfWork.UserMemberRoles.GetByUserIdAsync(user.Id);
        if (userRoles.Any())
        {
            foreach (var ur in userRoles)
                _unitOfWork.UserMemberRoles.Remove(ur); 
        }

        await _unitOfWork.UserMembers.RemoveAsync(user);
        await _unitOfWork.SaveChanges();

        return new OperationResult { Success = true, Message = "Usuario eliminado correctamente." };
    }
}
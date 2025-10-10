using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Auth;

public class RefreshTokenRequestDto
{
    public string? RefreshToken { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DTOs.Auth;

public class UpdateUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; } // si quieres permitir cambiar password
    public string? Role { get; set; }     // role a asignar (reemplaza roles existentes)
}
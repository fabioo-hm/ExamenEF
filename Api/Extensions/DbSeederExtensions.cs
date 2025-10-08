using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Helpers;
using Domain.Entities.Auth;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class DbSeederExtensions
{
    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AutoTallerDbContext>();

        var existingNames = await db.Roles.Select(r => r.Name).ToListAsync();
        var targetNames = Enum.GetNames(typeof(UserAuthorization.Roles));

        var toAdd = targetNames
            .Except(existingNames, StringComparer.OrdinalIgnoreCase)
            .Select(n => new Rol
            {
                Name = n,
                Description = $"{n} role"
            })
            .ToList();

        if (toAdd.Count > 0)
        {
            db.Roles.AddRange(toAdd);
            await db.SaveChangesAsync();
        }
    }
}
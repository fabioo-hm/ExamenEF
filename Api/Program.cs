using Api.Extensions;
using Api.Helpers;
using Api.Helpers.Errors;
using Api.Services;
using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // 👈 Necesario para Swagger

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 🧩 Servicios
// ============================================================
builder.Services.AddControllers();
builder.Services.ConfigureCors();
builder.Services.AddCustomRateLimiter();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// ============================================================
// 🧩 Base de datos
// ============================================================
builder.Services.AddDbContext<AutoTallerDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("Postgres")!;
    options.UseNpgsql(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// ============================================================
// 🧩 Repositorios
// ============================================================
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IServiceOrderRepository, ServiceOrderRepository>();
builder.Services.AddScoped<ISparePartRepository, SparePartRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<UserService>();

// ============================================================
// 🧩 SWAGGER CONFIG
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AutoTaller API",
        Version = "v1",
        Description = "Documentación interactiva de la API del Taller Automotriz."
    });

    // 🔐 Configurar autenticación JWT (Bearer Token)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresa el token JWT con el prefijo **Bearer**. Ejemplo: `Bearer {token}`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// ============================================================
// 🏗️ Construcción de la app
// ============================================================
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
await app.SeedRolesAsync();

// ============================================================
// 🚀 Pipeline de la aplicación
// ============================================================
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    // ✅ Swagger habilitado solo en desarrollo / staging
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoTaller API v1");
        options.RoutePrefix = string.Empty; // Swagger en raíz → http://localhost:5000
    });
}

app.UseCors("CorsPolicy");
app.UseCors("CorsPolicyUrl");
app.UseCors("Dinamica");

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthentication(); // 👈 Importante: primero autenticación
app.UseAuthorization();  // 👈 Luego autorización

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Firebase
var credentialsPath = "encryptdecrypt.json"; // Archivo en la raíz del proyecto
var projectId = "encryptdecrypt-c040f"; // ProjectId real de Firebase
builder.Services.AddSingleton(new FirebaseRepository(credentialsPath, projectId));

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Encriptar y Desencriptar", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de su token JWT. Ejemplo: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Inyección de dependencias
builder.Services.AddScoped<IEncryptionRepository, EncryptionRepository>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

var key = "clave_super_secreta_123456789_abcdefg"; // 32 caracteres o más

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Lista de IPs públicas permitidas
var allowedIps = new[] { "187.155.101.200", "TU_OTRA_IP" }; // Agrega aquí todas las IPs públicas permitidas
app.Use(async (context, next) =>
{
    var remoteIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim()
                   ?? context.Connection.RemoteIpAddress?.ToString();

    // Log temporal para ver la IP real
    Console.WriteLine($"IP detectada: {remoteIp}");

    if (!allowedIps.Contains(remoteIp))
    {
        context.Response.StatusCode = 403; // Forbidden
        await context.Response.WriteAsync($"Acceso denegado: IP no permitida. Tu IP detectada es: {remoteIp}");
        return;
    }

    await next.Invoke();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run(); 
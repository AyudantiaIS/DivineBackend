using Backend.Src.Seeder;
using Backend.Src.Services;
using Backend.Src.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
//


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthServices>();

var var = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"🔗 Cadena de conexión: {var}");
// 1. Registrar DbContext

var dataBase =new NpgsqlDataSourceBuilder(var).Build();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(var));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:issuer"],
            ValidAudience = builder.Configuration["Jwt:audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                ctx.Token = ctx.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    }); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Cambia esto por la URL de tu frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// 2. Probar conexión ANTES de app.Run()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        
        await db.Database.OpenConnectionAsync();
        await db.Database.MigrateAsync();
        await Seeder.Seed(db);
        Console.WriteLine("✅ Migración aplicada!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error de conexión: {ex.Message}");
    }
}

app.Run();

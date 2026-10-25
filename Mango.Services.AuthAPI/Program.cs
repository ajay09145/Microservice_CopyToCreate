using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Mango.Services.AuthAPI.Service.Service;
using Mango.Services.AuthAPI.Services.IService;
using Mango.Services.AuthAPI.Services.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionstrig = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionstrig));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

//create the bridge b/w Identity and entity framework core with identity roles.
//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        // Tell Swagger UI to fetch the natively generated OpenAPI document
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}

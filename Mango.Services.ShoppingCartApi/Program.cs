using AutoMapper;
using Mango.Services.ShoppingCartApi;
using Mango.Services.ShoppingCartApi.Data;
using Mango.Services.ShoppingCartApi.Extensions;
using Mango.Services.ShoppingCartApi.Services.IService;
using Mango.Services.ShoppingCartApi.Services.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionstrig = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionstrig));

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddHttpClient("ProductAPI", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]));
builder.Services.AddHttpClient("CouponAPI", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponAPI"]));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
//add the authentication in swagger
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(option =>
//{
//    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Description = "Enter the Bearer Authorization string as follow: `Bearer Generated-JWT-Token`",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                 Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = JwtBearerDefaults.AuthenticationScheme
//                }
//            },new string[]{}
//        }
//    });
//});

//Authorize the application hare by this but now call the extension method

builder.AddApplicationAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        // Tell Swagger UI to fetch the natively generated OpenAPI document
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
        //options.SwaggerEndpoint("/swagger/openapi/swagger.json", "API v1");
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
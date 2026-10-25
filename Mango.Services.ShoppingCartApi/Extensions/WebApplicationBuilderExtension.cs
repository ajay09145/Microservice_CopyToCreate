using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.ShoppingCartApi.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        public static WebApplicationBuilder AddApplicationAuthentication(this WebApplicationBuilder builder)
        {
            var apiSettings = builder.Configuration.GetSection("ApiSettings");
            var secret = apiSettings.GetValue<string>("Secret");
            var issuer = apiSettings.GetValue<string>("Issuer");
            var audience = apiSettings.GetValue<string>("Audience");

            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    //ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                };
            });

            return builder;
        }

    }
}

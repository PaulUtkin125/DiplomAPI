using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;

namespace DiplomAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            })
            .AddJwtBearer(options =>
            {
                var jwtKey = builder.Configuration["JWT:Key"];
                var jwtIssuer = builder.Configuration["JWT:Issuer"];
                var jwtAudience = builder.Configuration["JWT:Audience"];
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            builder.Services.AddAuthentication();
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.Map("/LoginForm/Index/{id}", (string id) =>
            {
                var jwtKey = builder.Configuration["JWT:Key"];
                var jwtIssuer = builder.Configuration["JWT:Issuer"];
                var jwtAudience = builder.Configuration["JWT:Audience"];

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, id) };
                var jwt = new JwtSecurityToken(
                        issuer: jwtIssuer,
                        audience: jwtAudience,
                        claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(20))); // время действия 20 минуты
                        

                return new JwtSecurityTokenHandler().WriteToken(jwt);
            });

            app.MapControllers();

            app.Run();
        }
    }
}

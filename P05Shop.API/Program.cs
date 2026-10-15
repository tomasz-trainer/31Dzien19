
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using P05Shop.API.Models;
using P05Shop.API.Services;
using P06Shop.Shared.Services.AuthService;
using P06Shop.Shared.Services.CategoryService;
using P06Shop.Shared.Services.ProductService;
using P06Shop.Shared.Services.WeatherSeervice;

namespace P05Shop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            


            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IMeteoService, MeteoService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthService, AuthService>();


            // addScoped - oznacza, że w trakcie jednego requestu będzie istniała tylko jedna instancja klasy ProductService
            // addTransient - oznacza, że obiekt będzie tworzony za każdym razem, gdy odwolujemy się do niego
            // addSingleton - oznacza, że obiekt będzie tworzony tylko raz i będzie istniał tak długo, jak długo istnieje aplikacja


            string token = builder.Configuration.GetSection("AppSettings:Token").Value;
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                   // options.Authority = "https://localhost:5001"; // adres serwera autoryzacji
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(token))

                    };
                });


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });


            var app = builder.Build();

            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

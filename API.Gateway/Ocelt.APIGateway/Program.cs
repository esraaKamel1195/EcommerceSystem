using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Ocelt.APIGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            builder.Services.AddControllers();

            var authSchema = "EShoppingGatewayAuthSchema";
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.Authority = "https://host.docker.internal:9009"; // IdentityServer URL
                    options.Audience = "EShoppingGateway"; // API resource name defined in IdentityServer
                    options.RequireHttpsMetadata = false; // only for dev

                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidAudience = "EShoppingGateway",
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://host.docker.internal:9009",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero // Adjust as needed
                    };
                });

            builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json",
                optional: true, reloadOnChange: true
            );

            builder.Services.AddOcelot(builder.Configuration);
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            //builder.Services.AddSwaggerGen(options =>
            //{
            //    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            //    {
            //        Title = "Catalog.API",
            //        Version = "v1",
            //        Description = "Catalog Microservice",
            //        Contact = new Microsoft.OpenApi.Models.OpenApiContact
            //        {
            //            Name = "Esraa Kamel",
            //            Email = "esraa.kamel1811@gmail.com",
            //            Url = new Uri("https://example.com")
            //        }
            //    });
            //});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello ocelot"); });
            });

            await app.UseOcelot();

            await app.RunAsync();
        }
    }
}
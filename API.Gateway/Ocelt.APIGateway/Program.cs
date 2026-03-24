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

            builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json",
                optional: true, reloadOnChange: true
            );
            builder.Services.AddOcelot(builder.Configuration);
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

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
                app.MapOpenApi();
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
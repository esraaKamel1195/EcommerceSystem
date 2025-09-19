
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        // Replace the incorrect AddSwaggerGen usage with the correct overload that takes an options lambda.
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Catalog.API",
                Version = "v1",
                Description = "Catalog Microservice",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Esraa Kamel",
                    Email = "esraa.kamel1811@gmail.com",
                    Url = new Uri("https://example.com")
                }
            });
        });

        builder.Services.AddOpenApi();

        var app = builder.Build();

        //app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

using Basket.Application.Commands;
using Basket.Application.GRPCServices;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Discunt.Grpc.Protos;
using System.Reflection;

namespace Basket.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddAutoMapper(typeof(BasketMappingProfile).Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies
            (
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(CreateShoppingCartCommand))
            )
        );

        builder.Services.AddScoped<IBasketRepository, BasketRepository>();
        builder.Services.AddScoped<DiscountGrpcService>();
        builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
            cfg => cfg.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"])
        );

        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
        });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Basket API",
                Version = "v1",
                Description = "Basket Microservice",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Esraa Kamel",
                    Email = "esraa.kamel1811@gmail.com",
                    Url = new Uri("https://example.com")
                }
            });
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
        });

        builder.Services.AddOpenApi();

        builder.Services.AddCors();

        var app = builder.Build();

        //app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.UseCors();

        app.MapControllers();

        app.Run();
    }
}

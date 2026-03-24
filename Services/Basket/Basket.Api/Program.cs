using Basket.Application.Commands;
using Basket.Application.GRPCServices;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Common.Logging;
using Discount.Grpc.Protos;
using MassTransit;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Basket.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();

        // Add services to the container.
        builder.Host.UseSerilog(Logging.ConfigureLogger);

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

        builder.Services.AddMassTransit(config =>
            config.UsingRabbitMq((context, cfg) => {
                cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
            })
        );

        builder.Services.AddMassTransitHostedService();

        builder.Services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddEndpointsApiExplorer();

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

            options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Basket API",
                Version = "v2",
                Description = "Basket Microservice V2",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Esraa Kamel",
                    Email = "esraa.kamel1811@gmail.com",
                    Url = new Uri("https://example.com")
                }
            });

            options.DocInclusionPredicate((version, apiDescription) =>
            {
                if (apiDescription.TryGetMethodInfo(out var methodInfo))
                {
                    var versions = methodInfo.DeclaringType?
                        .GetCustomAttributes(true)
                        .OfType<Asp.Versioning.ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);
                    return versions != null && versions.Any(v => $"v{v.ToString()}" == version);
                }
                else
                {
                    return false;
                }
            });
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
        });

        builder.Services.AddOpenApi();

        builder.Services.AddCors(
            options =>
            {
                options.AddPolicy("*", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            }
            );

        var app = builder.Build();

        //app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.json", "Basket API V1");
                options.SwaggerEndpoint("v2/swagger.json", "Basket API V2");
                options.RoutePrefix = "swagger";
            });
        }

        app.UseAuthorization();
        app.UseCors("*");

        app.MapControllers();

        app.Run();
    }
}
using Common.Logging;
using Discount.API.Services;
using Discount.Application.Commands;
using Discount.Application.Mappers;
using Discount.Core.Repositories;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using Serilog;
using System.Reflection;

namespace Discount.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog(Logging.ConfigureLogger);

        // correct: gRPC SERVER registration only
        builder.Services.AddGrpc();
        builder.Services.AddControllers();

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

        builder.Services.AddAutoMapper(typeof(DiscountProfile).Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof(CreateDiscountCouponCommand))
        ));

        builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Discount API",
                Version = "v1",
                Description = "Discount Microservice - REST + gRPC"
            });
        });

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

        // correct: REMOVED: DiscountGrpcService (client) — belongs in Basket.API, not here
        // correct: REMOVED: AddGrpcClient — Discount.API is the SERVER, not a client of itself
        // correct: REMOVED: builder.WebHost.ConfigureKestrel — appsettings.json handles this already

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.Use((ctx, next) =>
            {
                if (ctx.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var prefix) &&
                    !string.IsNullOrEmpty(prefix))
                {
                    ctx.Request.PathBase = prefix.ToString(); // e.g., "/discount"
                }
                return next();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                // Use *relative* URLs so the /discount prefix is preserved by the browser
                c.SwaggerEndpoint("v1/swagger.json", "Discount API V1");
                c.RoutePrefix = "swagger";
            });
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount API V1");
            //});
        }

        app.MigrateDatabase<Program>();
        app.UseRouting();
        app.UseCors("*");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<DiscountService>();  // correct: gRPC server endpoint
            endpoints.MapControllers();                   // correct: REST endpoints
            endpoints.MapGet("/", () =>
                "Communication with gRPC endpoints must be made through a gRPC client.");
        });

        app.Run();
    }
}
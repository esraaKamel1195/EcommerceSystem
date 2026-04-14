using Basket.Application.GRPCServices;
using Common.Logging;
using Discount.API.Services;
using Discount.Application.Commands;
using Discount.Application.Mappers;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Reflection;

namespace Discount.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();
        builder.Host.UseSerilog(Logging.ConfigureLogger);

        // Add services to the container.

        builder.Services.AddGrpc();
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();

        builder.Services.AddAutoMapper(typeof(DiscountProfile).Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies
            (
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(CreateDiscountCouponCommand))
            )
        );
        builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
        builder.Services.AddScoped<DiscountGrpcService>();
        // Also ensure DiscountProtoServiceClient is registered, e.g.:
        builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
        {
            o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]);
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8002, o =>
            {
                o.Protocols = HttpProtocols.Http2;
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.MigrateDatabase<Program>();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<DiscountService>();  // gRPC service
            endpoints.MapGet("/", () =>
                "Communication with gRPC endpoints must be made through a gRPC client.");
        });
        app.Run();
    }
}
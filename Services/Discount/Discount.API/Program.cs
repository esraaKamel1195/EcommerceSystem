using Discount.API.Services;
using Discount.Application.Commands;
using Discount.Application.Mappers;
using Discount.Core.Repositories;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using System.Reflection;

namespace Discount.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddGrpc();
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddAutoMapper(typeof(DiscountProfile).Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies
            (
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(CreateDiscountCouponCommand))
            )
        );
        builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        //app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MigrateDatabase<Program>();
        app.UseRouting();
        app.MapGrpcService<DiscountService>();  // gRPC service
        app.MapGet("/", () =>
            "Communication with gRPC endpoints must be made through a gRPC client.");

        app.Run();
    }
}

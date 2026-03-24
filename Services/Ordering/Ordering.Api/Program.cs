using Common.Logging;
using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extentions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Serilog;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.AddServiceDefaults();

            builder.Host.UseSerilog(Logging.ConfigureLogger);
            // Add services to the container.
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
                    Title = "Ordering API",
                    Version = "v1",
                    Description = "This is API for Ordering microservice in ecommerce application",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Esraa",
                        Email = "Esraa@gmail.com",
                        Url = new Uri("https://yourwebsite.eg")
                    }
                });
            });

            builder.Services.AddApplicationServices();

            builder.Services.AddInfraServices(builder.Configuration);
            builder.Services.AddScoped<BasketOrderingConsumer>();
            builder.Services.AddScoped<BasketOrderingConsumerV2>();

            builder.Services.AddMassTransit(config => {
                config.AddConsumer<BasketOrderingConsumer>();

                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
                    cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, e =>
                    {
                        e.ConfigureConsumer<BasketOrderingConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueueV2, e =>
                    {
                        e.ConfigureConsumer<BasketOrderingConsumerV2>(context);
                    });
                });
            });

            builder.Services.AddMassTransitHostedService();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.MigrateDatabase<OrderContext>((context, services) =>
            {
                var logger = services.GetService<ILogger<OrderContextSeed>>();
                OrderContextSeed.SeedAsync(context, logger).Wait();
            });

            //app.MapDefaultEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

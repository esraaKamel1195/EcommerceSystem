using Basket.Application.Commands;
using Basket.Application.GRPCServices;
using Basket.Application.Mappers;
using Basket.Core.Repositories;
using Basket.Infrastructure.Repositories;
using Common.Logging;
using Discount.Grpc.Protos;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Basket.Api;

public class Program
{
    public static void Main(string[] args)
    {
        //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

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

        var userPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        builder.Services.AddControllers(config =>
        {
            config.Filters.Add(new AuthorizeFilter(userPolicy));
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://identityserver:9011/"; // IdentityServer URL
                options.RequireHttpsMetadata = false; // Set to true in production
                options.MetadataAddress = "http://identityserver:9011/.well-known/openid-configuration";

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://identityserver:9011/",
                    ValidateAudience = true,
                    ValidAudience = "Basket",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                };

                // add this to docker to host communication
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                //options.Audience = "catalog_api"; // API resource name defined in IdentityServer
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Error("Authentication failed: {Error}", context.Exception.Message);
                        Console.WriteLine($"Authentication failed");
                        Console.WriteLine($"Exception: {context.Exception}");
                        Console.WriteLine($"Authority: {options.Authority}");
                        return Task.CompletedTask;
                    }
                };
            });

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

            // Optional: add JWT bearer auth to Swagger UI
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
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
            options.Configuration = builder.Configuration["CacheSettings:ConnectionString"];
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
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.MapOpenApi();
            app.Use((ctx, next) =>
            {
                if (ctx.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var prefix) &&
                    !string.IsNullOrEmpty(prefix))
                {
                    ctx.Request.PathBase = prefix.ToString(); // e.g., "/basket"
                }
                return next();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Use *relative* URLs so the /basket prefix is preserved by the browser
                c.SwaggerEndpoint("v1/swagger.json", "Basket.API v1");   // no leading '/'
                c.SwaggerEndpoint("v2/swagger.json", "Basket.API v2");   // no leading '/'
                c.RoutePrefix = "swagger";
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("*");

        app.MapControllers();

        app.Run();
    }
}
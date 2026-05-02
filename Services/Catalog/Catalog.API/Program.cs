using Catalog.Application.GRPCServices;
using Catalog.Application.Mappers;
using Catalog.Application.Queries;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Context;
using Catalog.Infrastructure.Repositories;
using Common.Logging;
using Discount.Grpc.Protos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Catalog.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();

        // Add services to the container.
        builder.Host.UseSerilog(Logging.ConfigureLogger);

        builder.Services.AddControllers();
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
                    ValidAudience = "Catalog",
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
                    },

                    OnTokenValidated = context =>
                    {
                        Log.Information("Token validated successfully for {Subject}", context.Principal?.Identity?.Name);
                        Console.WriteLine($"Token validated successfully for {context.Principal?.Identity?.Name}");
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("CanRead", policy => policy.RequireClaim("scope", "catalogapi.read"));
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies
            (
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(GetProductByIdQuery))
            )
        );

        builder.Services.AddScoped<ICatalogContext, CatalogContext>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IBrandRepository, ProductRepository>();
        builder.Services.AddScoped<ITypeRepository, ProductRepository>();
        builder.Services.AddScoped<DiscountGrpcService>();
        builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(
            cfg => cfg.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"])
        );

        var userPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

        builder.Services.AddControllers(config =>
        {
            config.Filters.Add(new AuthorizeFilter(userPolicy));
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
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader()
            );
        });

        var app = builder.Build(); 

        //app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.Use((ctx, next) =>
            {
                if (ctx.Request.Headers.TryGetValue("X-Forwarded-Prefix", out var prefix) &&
                    !string.IsNullOrEmpty(prefix))
                {
                    ctx.Request.PathBase = prefix.ToString(); // e.g., "/catalog"
                }
                return next();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Use *relative* URLs so the /catalog prefix is preserved by the browser
                c.SwaggerEndpoint("v1/swagger.json", "Catalog API V1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
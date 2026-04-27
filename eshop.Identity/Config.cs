using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using System.Security.Cryptography;

namespace eshop.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("catalogapi"),
            new ApiScope("basketapi"),
            new ApiScope("catalogapi.read"),
            new ApiScope("catalogapi.write"),
            new ApiScope("eshoppinggateway"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("Catalog", "Catalog.API")
            {
                Scopes = { "catalogapi.read", "catalogapi.write" }
            },
            new ApiResource("Basket", "Basket.API")
            {
                Scopes = { "basketapi" }
            },
            new ApiResource("EShoppingGateway", "EShopping Gateway")
            {
                Scopes = { "eshoppinggateway", "basketapi" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "catalogclient",
                ClientName = "Catalog Client",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "catalogapi.read", "catalogapi.write" },
                RequirePkce = true,
                RequireClientSecret = true,
                //RedirectUris = { "https://localhost:44300/signin-oidc" },
                //FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                //PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },
            },
            new Client
            {
                ClientId = "BasketClient",
                ClientName = "Basket Client",
                ClientSecrets = { new Secret("49C1B8E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "basketapi" },
                RequirePkce = true,
                RequireClientSecret = true,
            },
            new Client
            {
                ClientName = "EShopping Gateway Client",
                ClientId = "EShppingGatewayClient",
                ClientSecrets = {new Secret("49C1A7B8-1C79-4A70-A3C6-A37998FB86B0".Sha256()) } ,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "eshoppinggateway", "basketapi" , "catalogapi.read" }
            },
            new Client
            {
                ClientId = "angular-client",
                ClientName = "Angular SPA",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,            // SPA
                RedirectUris = {
                    "http://localhost:4200/signin-callback",
                    "http://localhost:4200/assets/silent-callback.html"
                },
                PostLogoutRedirectUris = {
                     "http://localhost:4200/signout-callback"
                },
                AllowedCorsOrigins = {
                     "http://localhost:4200"
                },
                AllowedScopes = {
                     IdentityServerConstants.StandardScopes.OpenId,
                     IdentityServerConstants.StandardScopes.Profile,
                     "eshoppinggateway",
                     "basketapi",
                     "catalogapi.read"
                },
                AllowAccessTokensViaBrowser = true,
                AccessTokenLifetime = 3600,
                Enabled = true
            }
        };
}

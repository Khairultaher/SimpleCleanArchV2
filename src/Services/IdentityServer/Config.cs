// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("apiscope", "API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {              
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "client1",
                    ClientName = "MVC WEB API",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    //AllowedGrantTypes = GrantTypes.Implicit,
                    //AllowedGrantTypes = GrantTypes.Code,
                    //AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedGrantTypes = GrantTypes.Hybrid,

                    RequirePkce = false,
                    AllowRememberConsent = false,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "apiscope"
                    }
                },

                // machine to machine client
                new Client
                {
                    ClientId = "client2",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },                  
                    // scopes that client has access to
                    AllowedScopes = { "apiscope" }
                }
            };
    }
}
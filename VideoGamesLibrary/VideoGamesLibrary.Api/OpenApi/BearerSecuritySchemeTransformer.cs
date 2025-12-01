using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace VideoGamesLibrary.Api.OpenApi;

internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IAuthenticationSchemeProvider _schemeProvider;

    public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider schemeProvider)
    {
        _schemeProvider = schemeProvider;
    }

    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        // Vérifier qu'un schéma JWT Bearer est bien enregistré
        var schemes = await _schemeProvider.GetAllSchemesAsync();
        var hasBearer = schemes.Any(s =>
            string.Equals(s.Name, JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase));

        if (!hasBearer)
        {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        // 1. Déclarer le schéma de sécurité "Bearer"
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",          // Swagger UI saura générer "Authorization: Bearer <token>"
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Bearer token. Exemple : \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
        };

        // 2. L'appliquer comme requirement global à toutes les opérations
        document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
        document.SecurityRequirements.Add(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>() // pas de scopes
                }
            });
    }
}

// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Configuration;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Squidex.Hosting;
using Squidex.Web;

namespace Squidex.Areas.Api.Config.OpenApi;

public sealed class CommonProcessor : IDocumentProcessor
{
    private readonly string version;
    private readonly string logoColor = "#3f83df";
    private readonly string logoUrl;
    private readonly IConfiguration config;

    public CommonProcessor(ExposedValues exposedValues, IUrlGenerator urlGenerator, IConfiguration config)
    {
        this.config = config;
        logoUrl = urlGenerator.BuildUrl("images/logo-white.png", false);

        if (!exposedValues.TryGetValue("version", out version!) || version == null)
        {
            version = "1.0";
        }
    }

    public void Process(DocumentProcessorContext context)
    {
        context.Document.Info.Title = "Squidex API";
        context.Document.Info.Version = version;
        context.Document.Info.ExtensionData = new Dictionary<string, object?>
        {
            ["x-logo"] = new
            {
                url = logoUrl,
                backgroundStyle = string.Empty,
                backgroundColor = logoColor,
            },
        };

        context.Document.ExternalDocumentation = new OpenApiExternalDocumentation
        {
            Url = config["Api:DocumentationUrl"] ?? "https://docs.squidex.io",
        };
    }
}

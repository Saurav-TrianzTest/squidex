// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Amazon.SimpleSystemsManagement;
using Ganss.Xss;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Squidex.Areas.IdentityServer.Services;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
}

public sealed class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;
    private readonly ILogger<HtmlSanitizerService> _logger;

    public HtmlSanitizerService(
        IConfiguration config,
        ILogger<HtmlSanitizerService> logger)
    {
        _logger = logger;
        _sanitizer = new HtmlSanitizer();

        // Load allowed tags and attributes from configuration or AWS Secrets Manager.
        var allowedTags = config["HtmlSanitizer:AllowedTags"];
        if (!string.IsNullOrWhiteSpace(allowedTags))
        {
            foreach (var tag in allowedTags.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                _sanitizer.AllowedTags.Add(tag.Trim());
            }
        }

        var allowedAttributes = config["HtmlSanitizer:AllowedAttributes"];
        if (!string.IsNullOrWhiteSpace(allowedAttributes))
        {
            foreach (var attr in allowedAttributes.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                _sanitizer.AllowedAttributes.Add(attr.Trim());
            }
        }
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return html;
        }

        return _sanitizer.Sanitize(html);
    }
}

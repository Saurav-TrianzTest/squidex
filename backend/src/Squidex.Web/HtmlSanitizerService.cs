// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenigt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Ganss.XSS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Squidex.Web;

public interface IHtmlSanitizerService
{
    string Sanitize(string html);
}

public sealed class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer sanitizer;
    private readonly ILogger<HtmlSanitizerService> log;

    public HtmlSanitizerService(IOptions<HtmlSanitizerOptions> options,
        ILogger<HtmlSanitizerService> log)
    {
        this.log = log;

        sanitizer = new HtmlSanitizer
        {
            AllowDataAttributes = true,
        };

        if (options.Value.AllowedTags != null)
        {
            sanitizer.AllowedTags.Clear();
            foreach (var tag in options.Value.AllowedTags)
            {
                sanitizer.AllowedTags.Add(tag);
            }
        }

        if (options.Value.AllowedAttributes != null)
        {
            sanitizer.AllowedAttributes.Clear();
            foreach (var attr in options.Value.AllowedAttributes)
            {
                sanitizer.AllowedAttributes.Add(attr);
            }
        }
    }

    public string Sanitize(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return html;
        }

        return sanitizer.Sanitize(html);
    }
}

public sealed class HtmlSanitizerOptions
{
    public List<string>? AllowedTags { get; set; }

    public List<string>? AllowedAttributes { get; set; }
}

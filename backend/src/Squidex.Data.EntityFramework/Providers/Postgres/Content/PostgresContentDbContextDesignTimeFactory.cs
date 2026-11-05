// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Json.System;
using Squidex.Providers.Postgres.App;

namespace Squidex.Providers.Postgres.Content;

public sealed class PostgresContentDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PostgresContentDbContext>
{
    public PostgresContentDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
            ?? throw new InvalidOperationException("Database__ConnectionString environment variable is required for design-time configuration.");

        var builder = new DbContextOptionsBuilder<PostgresAppDbContext>()
            .UsePrefix(string.Empty)
            .UseNpgsql(connectionString, options =>
            {
                options.UseNetTopologySuite();
            });

        return new PostgresContentDbContext(builder.Options, new SystemJsonSerializer(JsonSerializerOptions.Default));
    }
}

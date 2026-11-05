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
using Squidex.Providers.MySql.App;

namespace Squidex.Providers.MySql.Content;

public sealed class MySqlContentDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MySqlContentDbContext>
{
    public MySqlContentDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
            ?? throw new InvalidOperationException("Database__ConnectionString environment variable is required for design-time configuration.");

        var builder = new DbContextOptionsBuilder<MySqlAppDbContext>()
            .UsePrefix(string.Empty)
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.UseNetTopologySuite();
            });

        return new MySqlContentDbContext(builder.Options, new SystemJsonSerializer(JsonSerializerOptions.Default));
    }
}

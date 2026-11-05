// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Squidex.Infrastructure.Json.System;

namespace Squidex.Providers.SqlServer.App;

public sealed class SqlServerAppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SqlServerAppDbContext>
{
    public SqlServerAppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("Database__ConnectionString")
            ?? throw new InvalidOperationException("Database__ConnectionString environment variable is required for design-time configuration.");

        var builder = new DbContextOptionsBuilder<SqlServerAppDbContext>()
            .UseSqlServer(connectionString, options =>
            {
                options.UseNetTopologySuite();
            });

        return new SqlServerAppDbContext(builder.Options, new SystemJsonSerializer(JsonSerializerOptions.Default));
    }
}
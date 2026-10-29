// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Text.Json;
using Amazon.SimpleSystemsManagement;
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
        var host = Environment.GetEnvironmentVariable("Database__Host") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("Database__Port") ?? "33060";
        var database = Environment.GetEnvironmentVariable("Database__Name") ?? "test";
        var user = Environment.GetEnvironmentVariable("Database__User") ?? "mysql";
        var secretName = Environment.GetEnvironmentVariable("Database__SecretName") ?? "/app/db/password";

        var password = GetSecretFromParameterStore(secretName);

        var ConnectionString = $"Server={host};Port={port};Database={database};User={user};Password={password}";

        var builder = new DbContextOptionsBuilder<MySqlAppDbContext>()
            .UsePrefix(string.Empty)
            .UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString), options =>
            {
                options.UseNetTopologySuite();
            });

        return new MySqlContentDbContext(builder.Options, new SystemJsonSerializer(JsonSerializerOptions.Default));
    }

    private static string GetSecretFromParameterStore(string secretName)
    {
        try
        {
            var ssm = new AmazonSimpleSystemsManagementClient();
            var response = ssm.GetParameterAsync(new Amazon.SimpleSystemsManagement.Model.GetParameterRequest
            {
                Name = secretName,
                WithDecryption = true
            }).GetAwaiter().GetResult();

            return response.Parameter?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}

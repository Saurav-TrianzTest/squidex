// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Concurrent;
using System.Text.Json;

namespace Squidex.Infrastructure;

public static class Singletons<T>
{
    private static readonly ConcurrentDictionary<string, T> Instances = new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase);

    // Redis-backed distributed singleton storage for cloud deployments.
    // Falls back to in-memory storage when Redis is not configured.
    private static readonly Lazy<ConnectionMultiplexer?> Redis = new Lazy<ConnectionMultiplexer?>(() =>
    {
        var redisConfig = Environment.GetEnvironmentVariable("AWS:ElastiCache:Endpoint");
        if (string.IsNullOrWhiteSpace(redisConfig))
        {
            return null;
        }

        try
        {
            return ConnectionMultiplexer.Connect(redisConfig);
        }
        catch
        {
            return null;
        }
    });

    public static T GetOrAdd(string key, Func<string, T> factory)
    {
        // Try Redis first for distributed singleton storage.
        if (Redis.Value != null)
        {
            try
            {
                var db = Redis.Value.GetDatabase();
                var cacheKey = $"squidex:singleton:{typeof(T).Name}:{key}";
                var cached = db.StringGet(cacheKey);

                if (!cached.HasValue)
                {
                    var instance = factory(key);
                    var json = JsonSerializer.Serialize(instance);
                    db.StringSet(cacheKey, json, TimeSpan.FromHours(1));
                    return instance;
                }

                return JsonSerializer.Deserialize<T>(cached)!;
            }
            catch
            {
                // Fall back to in-memory storage on Redis errors.
            }
        }

        return Instances.GetOrAdd(key, factory);
    }
}


using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Users.Infrastructure.Persistence;

namespace Tests.Integration.Users;

public abstract class UsersBaseIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient client;
    protected readonly AppDbContext dbContext;
    private static Respawner respawner = null!;
    private static string connectionString = null!;
    private readonly CustomWebApplicationFactory applicationFactory;
    public UsersBaseIntegrationTests(CustomWebApplicationFactory applicationFactory)
    {
        this.applicationFactory = applicationFactory;
        client = applicationFactory.CreateClient();
        var scope = applicationFactory.Services.CreateScope();
        dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
    public Task DisposeAsync() => Task.CompletedTask;

    public async Task InitializeAsync()
    {
        if (respawner == null)
        {
            connectionString = dbContext.Database.GetConnectionString()!;

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            respawner = await Respawner.CreateAsync(connection,new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[]{"public"},
                TablesToIgnore = new Table[]{"__EFMigrationsHistory"}
            });
        }

        await using var cleanupConnection = new NpgsqlConnection(connectionString);
        await cleanupConnection.OpenAsync();
        await respawner.ResetAsync(cleanupConnection);
    }
}
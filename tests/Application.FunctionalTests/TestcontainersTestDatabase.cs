using System.Data.Common;
using HrmBaharu.Application.Common.Interfaces;
using HrmBaharu.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace HrmBaharu.Application.FunctionalTests;

public class TestcontainersTestDatabase : ITestDatabase
{
    private readonly MsSqlContainer _container;
    private DbConnection _connection = null!;
    private string _connectionString = null!;
    private Respawner _respawner = null!;
    private readonly IUser _loggedInUser;

    public TestcontainersTestDatabase(IUser loggedInUser)
    {
        _container = new MsSqlBuilder()
            .WithAutoRemove(true)
            .Build();

        _loggedInUser = loggedInUser;
    }

    public async Task InitialiseAsync()
    {
        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();

        _connection = new SqlConnection(_connectionString);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        var context = new ApplicationDbContext(options, _loggedInUser);

        context.Database.Migrate();

        _respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
        });
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connectionString);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
    }
}

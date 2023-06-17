using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using othello_api.Contexts;
using Testcontainers.MsSql;
using Xunit;

namespace othello_api.Tests;

[CollectionDefinition("SqlContainerFixture")]
public class SqlContainerFixture : ICollectionFixture<SqlContainerFixture>, IAsyncLifetime
{
    private readonly IServiceCollection _services;



    public MsSqlContainer Container { get; }
    public WebApplicationFactory<Startup> Factory { get; }
    public IServiceProvider ServiceProvider { get; }
    public IHost Host { get; }

    public SqlContainerFixture()
    {
        Container = new MsSqlBuilder().Build();
        _services = new ServiceCollection();

        Factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_ => new DbContextOptionsBuilder<OthelloContext>()
                        .UseSqlServer(Container.GetConnectionString())
                        .Options);
                });
            });


        InitializeAsync().GetAwaiter().GetResult();

        Host = Factory.Services.GetService<IHost>();
        ServiceProvider = Factory.Services;


    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<OthelloContext>();
        optionsBuilder.UseSqlServer(Container.GetConnectionString());

        using (var context = new OthelloContext(optionsBuilder.Options))
        {
            await context.Database.MigrateAsync();
        }

    }

    public Task DisposeAsync()
    {
        Factory.Dispose();
        return Container.DisposeAsync().AsTask();
    }

}

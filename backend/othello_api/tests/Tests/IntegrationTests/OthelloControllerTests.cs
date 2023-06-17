using Microsoft.EntityFrameworkCore;
using othello_api.Contexts;
using othello_api.Controllers;
using othello_api.Services;
using Xunit;

namespace othello_api.Tests.IntegrationTests;

[Collection("SqlContainerFixture")]
public class OthelloControllerTests
{
    private readonly OthelloController _controller;
    private readonly OthelloService _service;

    public OthelloControllerTests(SqlContainerFixture fixture)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OthelloContext>();
        optionsBuilder.UseSqlServer(fixture.Container.GetConnectionString());

        var context = new OthelloContext(optionsBuilder.Options);

        _service = new OthelloService(context, null); // Pass null for IHubContext<GameHub> since we don't need it in tests
        _controller = new OthelloController(null, _service, null); // Pass null for ILogger<OthelloController> and IHubContext<GameHub>
    }

    [Fact]
    public async Task AddAndRetrieveGame()
    {
        // Arrange
        int timeLimitSeconds = 60;
        int timeIncrementSeconds = 5;

        // Act
        var createdGame = _service.NewGame(timeLimitSeconds, timeIncrementSeconds);
        var retrievedGame = await _service.GetGameByIdAsync(createdGame.Id);

        // Assert
        Assert.Equal(createdGame.Id, retrievedGame.Id);
        Assert.Equal(timeLimitSeconds, retrievedGame.TimeLimitSeconds);
        Assert.Equal(timeIncrementSeconds, retrievedGame.TimeIncrementSeconds);
    }


}



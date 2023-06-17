using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using othello_api.Contexts;
using othello_api.Controllers;
using othello_api.Services;
using Xunit;

namespace othello_api.Tests.IntegrationTests;

[Collection("SqlContainerFixture")]
public class GameHubTests
{
    private readonly GameHub _gameHub;
    private readonly Mock<OthelloService> _othelloServiceMock;
    private readonly Mock<HttpClient> _httpClientMock;
    private readonly Mock<IHubContext<GameHub>> _hubContextMock;

    private readonly OthelloController _controller;
    private readonly OthelloService _service;

    public GameHubTests(SqlContainerFixture fixture)
    {

        var optionsBuilder = new DbContextOptionsBuilder<OthelloContext>();
        optionsBuilder.UseSqlServer(fixture.Container.GetConnectionString());

        var context = new OthelloContext(optionsBuilder.Options);

        _service = new OthelloService(context, null); // Pass null for IHubContext<GameHub> since we don't need it in tests
        _controller = new OthelloController(null, _service, null); // Pass null for ILogger<OthelloController> and IHubContext<GameHub>



        /**/
        /**/
        /* _othelloServiceMock = new Mock<IOthelloService>(); */
        /* _hubContextMock = new Mock<IHubContext<GameHub>>(); */
        /* _httpClientMock = new Mock<HttpClient>(); */
        /**/
        /* // Mock the Clients property */
        /* var clientsMock = new Mock<IHubClients>(); */
        /* _hubContextMock.Setup(x => x.Clients).Returns(clientsMock.Object); */
        /**/
        /* // Mock the Groups property */
        /* var groupsMock = new Mock<IGroupManager>(); */
        /* _hubContextMock.Setup(x => x.Groups).Returns(groupsMock.Object); */
        /**/
        /* // Mock the Context property */
        /* var contextMock = new Mock<IHubCallerConnectionContext<dynamic>>(); */
        /* _gameHub = new GameHub(_httpClientMock.Object, _hubContextMock.Object, _othelloServiceMock.Object); */
        /* _gameHub.Context = contextMock.Object; */





        /* var optionsBuilder = new DbContextOptionsBuilder<OthelloContext>(); */
        /* optionsBuilder.UseSqlServer(fixture.Container.GetConnectionString()); */
        /**/
        /* var context = new OthelloContext(optionsBuilder.Options); */
        /**/
        /* _service = new OthelloService(context, null); // Pass null for IHubContext<GameHub> since we don't need it in tests */
        /* _controller = new OthelloController(null, _service, null); // Pass null for ILogger<OthelloController> and IHubContext<GameHub> */
        /**/
        /* _othelloServiceMock = new Mock<OthelloService>(context, It.IsAny<IHubContext<GameHub>>()); */
        /* _hubContextMock = new Mock<IHubContext<GameHub>>();; */
        /* _httpClientMock = new Mock<HttpClient>(); */
        /**/
        /* // Pass the mock IHubContext<GameHub> to the GameHub constructor */
        /* _gameHub = new GameHub(_httpClientMock.Object, _hubContextMock.Object, _othelloServiceMock.Object ); */

    }

    [Fact]
    public async Task JoinSession_WithValidInput_ShouldAddPlayerConnection()
    {
        int gameId = 1;
        Color playerColor = Color.BLACK;

        await _gameHub.JoinSession(1, gameId, playerColor);

        Assert.True(GameHub.sessionPlayerConnections.ContainsKey(gameId));
        Assert.Single(GameHub.sessionPlayerConnections[gameId]);
        Assert.Equal(playerColor, GameHub.sessionPlayerConnections[gameId][0].PlayerColor);
    }

    [Fact]
    public async Task JoinSession_WithTwoPlayers_ShouldStartGame()
    {
        int gameId = 1;
        Color playerColor1 = Color.BLACK;
        Color playerColor2 = Color.WHITE;

        await _gameHub.JoinSession(1, gameId, playerColor1);
        await _gameHub.JoinSession(1, gameId, playerColor2);

        _othelloServiceMock.Verify(service => service.ChangeGameStatusAsync(gameId, GameStatus.PLAYING), Times.Once);
    }

    [Fact]
    public async Task OnDisconnectedAsync_ShouldRemovePlayerConnection()
    {
        int gameId = 1;
        Color playerColor = Color.BLACK;

        await _gameHub.JoinSession(1, gameId, playerColor);
        Assert.Single(GameHub.sessionPlayerConnections[gameId]);

        string connectionId = GameHub.sessionPlayerConnections[gameId][0].ConnectionId;
        await _gameHub.OnDisconnectedAsync(null);

        Assert.Empty(GameHub.sessionPlayerConnections[gameId]);
    }

    /* [Fact] */
    /* public async Task Surrender_ShouldEndGame() */
    /* { */
    /*     int gameId = 1; */
    /*     Color playerColor1 = Color.BLACK; */
    /*     Color playerColor2 = Color.WHITE; */
    /**/
    /*     await _gameHub.JoinSession(gameId, playerColor1); */
    /*     await _gameHub.JoinSession(gameId, playerColor2); */
    /**/
    /*     string connectionId = GameHub.gamePlayerConnections[gameId].Find(connection => connection.PlayerColor == playerColor1).ConnectionId; */
    /*     _hubLifetimeManager.Connections[0].Context.ConnectionId = connectionId; */
    /**/
    /*     await _gameHub.Surrender(gameId); */
    /**/
    /*     _othelloServiceMock.Verify(service => service.SurrenderAsync(gameId, playerColor1), Times.Once); */
    /* } */
}



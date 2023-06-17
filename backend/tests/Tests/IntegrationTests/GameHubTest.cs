using System.Data.Common;
using Moq;
using Xunit;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using othello_api.Tests;



[Collection("SqlServerContainerCollection")]
public class GameHubTests
{
    /*       private readonly SqlContainerFixture _fixture; */
    /**/
    /*       public GameHubTests(SqlContainerFixture fixture) */
    /*       { */
    /*           _fixture = fixture; */
    /*       } */
    /**/
    /* [Fact] */
    /* public async Task TestJoinSession() */
    /* { */
    /*     // Arrange */
    /*     var gameId = 1; */
    /*     var playerColor = Color.BLACK; */
    /**/
    /*     var othelloServiceMock = new Mock<OthelloService>(); */
    /*     // Setup othelloServiceMock methods as needed */
    /**/
    /*     var hubContextMock = new Mock<IHubContext<GameHub>>(); */
    /*     // Setup hubContextMock methods as needed */
    /**/
    /*     var httpClient = new HttpClient(); */
    /*     var gameHub = new GameHub(httpClient, hubContextMock.Object, othelloServiceMock.Object); */
    /**/
    /*     // Act */
    /*     await gameHub.JoinSession(gameId, playerColor); */
    /**/
    /*     // Assert */
    /*     // Verify the expected behavior or method calls */
    /* } */
}

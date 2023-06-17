using Xunit;

namespace othello_api.Tests;

[CollectionDefinition("SqlServerContainerCollection")]
public class SqlServerContainerCollection : ICollectionFixture<SqlContainerFixture>
{
    // This class has no code, and is never created. It's just a place to apply
    // [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}
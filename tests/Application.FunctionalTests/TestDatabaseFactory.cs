using HrmBaharu.Application.Common.Interfaces;

namespace HrmBaharu.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync(IUser loggedInUser)
    {
        var database = new TestcontainersTestDatabase(loggedInUser);

        await database.InitialiseAsync();

        return database;
    }
}

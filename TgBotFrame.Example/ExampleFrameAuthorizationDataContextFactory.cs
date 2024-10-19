using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TgBotFrame.Example;

public class ExampleFrameAuthorizationDataContextFactory : IDesignTimeDbContextFactory<ExampleDataContext>
{
    public ExampleDataContext CreateDbContext(string[] args)
    {
        Directory.CreateDirectory("../data/sqlite");
        DbContextOptionsBuilder<ExampleDataContext> builder = new();
        string connectionString = args.Any()
            ? string.Join(' ', args)
            : "Data Source=../data/sqlite/example.sqlite;";
        Console.WriteLine(@"connectionString = " + connectionString);
        return new(builder
            .UseSqlite("Data Source=../data/sqlite/example.sqlite;")
            .Options);
    }
}
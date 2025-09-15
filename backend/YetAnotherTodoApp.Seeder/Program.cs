// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YetAnotherTodoApp.Core.Entities;
using YetAnotherTodoApp.Data.Context;

var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();

IConfigurationRoot config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = config.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

optionsBuilder.UseNpgsql(connectionString);

var rootCommand = new RootCommand();

var countOption = new Option<int>("--count")
{
    DefaultValueFactory = _ => 100,
    Description = "Number of todo tasks to create",
};

rootCommand.Add(countOption);

rootCommand.SetAction(
    async result =>
    {
        var faker = new Faker<TodoTask>()
            .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 6))
            .RuleFor(t => t.Description, f => f.Lorem.Sentence(5, 200))
            .RuleFor(t => t.CreatedAt, f => f.Date.Past(1, DateTime.UtcNow.AddMonths(-6)).ToUniversalTime())
            .RuleFor(t => t.DueDate, f => f.Date.Between(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow.AddMonths(6)).ToUniversalTime())
            .RuleFor(t => t.CompletedAt, f => f.Random.Bool(0.3f) ? f.Date.Recent(5).ToUniversalTime() : null);

        int count = result.GetRequiredValue(countOption);
        var tasks = faker.Generate(count);

        await using var context = new TodoContext(optionsBuilder.Options);
        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
);

await rootCommand.Parse(args).InvokeAsync();

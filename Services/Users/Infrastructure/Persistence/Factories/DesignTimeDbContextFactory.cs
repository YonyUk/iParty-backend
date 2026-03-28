using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Users.Domain.Rules;
using Users.Infrastructure.Configuration;
using Users.Infrastructure.Providers;

namespace Users.Infrastructure.Persistence.Factories;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json",optional:false,reloadOnChange:true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbOptions = Options.Create(new DatabaseConfigOptions
        {
            CommandTimeoutSeconds = 30,
            EnableSensitiveDataLogging = false
        });

        var domainOptions = Options.Create(new UserDomainRulesOptions
        {
            MinimumUserNameLength = configuration.GetSection("UsersDomain").GetValue<int>("MinimumUserNameLength"),
            MaximumUserNameLength = configuration.GetSection("UsersDomain").GetValue<int>("MaximumUserNameLength"),
            MinimumPasswordLength = configuration.GetSection("UsersDomain").GetValue<int>("MinimumPasswordLength"),
            MaximumPasswordLength = configuration.GetSection("UsersDomain").GetValue<int>("MaximumPasswordLength"),
        });

        IUserDomainRulesConfigProvider rulesProvider = new UserDomainRulesConfigProvider(
            domainOptions
        );

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString,npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(dbOptions.Value.CommandTimeoutSeconds);
        });

        if (dbOptions.Value.EnableSensitiveDataLogging)
            optionsBuilder.EnableSensitiveDataLogging();
        
        return new AppDbContext(optionsBuilder.Options,rulesProvider);
    }
}
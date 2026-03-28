using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Users.Domain.Aggregates;
using Users.Domain.Rules;
using Users.Infrastructure.Configuration;
using Users.Infrastructure.Persistence.Configuration;
using Users.Infrastructure.Providers;

namespace Users.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IUserDomainRulesConfigProvider userDomainRulesConfigProvider;
    public DbSet<User> Users { get; set; }
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IUserDomainRulesConfigProvider userDomainRulesConfigProvider
    )
    : base(options)
    {
        this.userDomainRulesConfigProvider = userDomainRulesConfigProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfig(userDomainRulesConfigProvider));
        base.OnModelCreating(modelBuilder);
    }
}
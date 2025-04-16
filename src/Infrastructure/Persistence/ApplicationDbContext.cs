using System.Reflection;
using HrmBaharu.Application.Common.Interfaces;
using HrmBaharu.Domain.Common;
using HrmBaharu.Domain.Entities;
using HrmBaharu.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HrmBaharu.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IUser _loggedInUser;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUser loggedInUser) : base(options)
    {
        _loggedInUser = loggedInUser;
        ChangeTracker.LazyLoadingEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Detached:
                    entry.State = EntityState.Modified;
                    break;
                case EntityState.Added:
                    entry.Entity.CreatedBy = _loggedInUser.Id;
                    entry.Entity.Created = DateTimeOffset.UtcNow;
                    entry.Entity.LastModifiedBy = _loggedInUser.Id;
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _loggedInUser.Id;
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

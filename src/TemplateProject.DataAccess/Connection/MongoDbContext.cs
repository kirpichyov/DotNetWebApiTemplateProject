using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.DataAccess.Connection;

public sealed class MongoDbContext : DbContext
{
    public DbSet<UserSpace> UserSpaces { get; init; }

    public static MongoDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<MongoDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);
    
    public MongoDbContext(DbContextOptions options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserSpace>(cfg =>
        {
            cfg.ToCollection(nameof(UserSpaces));
        });
    }
}
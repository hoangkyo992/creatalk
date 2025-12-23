using Cdn.Domain.Entities;
using Common.Domain;
using Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cdn.Infrastructure.Persistence;

public partial class AppDbContext(DbContextOptions<AppDbContext> options,
    IServiceProvider serviceProvider,
    ICurrentUser currentUser)
    : BaseDbContext(options, serviceProvider, currentUser), IAppContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // .
        }
    }

    public virtual DbSet<Domain.Entities.File> Files { get; set; }
    public virtual DbSet<Folder> Folders { get; set; }
    public virtual DbSet<FileExtension> FileExtensions { get; set; }
    public virtual DbSet<Album> Albums { get; set; }
    public virtual DbSet<AlbumFile> AlbumFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SequencesConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
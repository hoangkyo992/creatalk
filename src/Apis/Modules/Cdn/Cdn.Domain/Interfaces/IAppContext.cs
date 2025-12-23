using Cdn.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cdn.Domain.Interfaces;

public interface IAppContext : IDbContext, ISeqDbContext
{
    DbSet<Entities.File> Files { get; }
    DbSet<Folder> Folders { get; }
    DbSet<FileExtension> FileExtensions { get; }
    DbSet<Album> Albums { get; }
    DbSet<AlbumFile> AlbumFiles { get; }
}
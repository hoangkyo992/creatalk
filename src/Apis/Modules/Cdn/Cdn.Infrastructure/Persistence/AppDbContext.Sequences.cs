using Cdn.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cdn.Infrastructure.Persistence;

public class Sequence
{
    public Sequence()
    {
    }

    public string Name { get; set; }

    [ConcurrencyCheck]
    public int Value { get; set; }
}

public class SequencesConfiguration : IEntityTypeConfiguration<Sequence>
{
    public void Configure(EntityTypeBuilder<Sequence> builder)
    {
        builder.ToTable("Sequences", DbConstants.SchemaName);
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasKey(c => new { c.Name });
    }
}

public partial class AppDbContext
{
    private static readonly string NextValueSql = @$"INSERT INTO ""{DbConstants.SchemaName}"".""Sequences"" VALUES ('[[NAME]]', 1) ON CONFLICT (""Name"") DO UPDATE SET ""Value""= ""Sequences"".""Value"" + [[TOTAL]] RETURNING ""Value"";";

    public async Task<int> NextSeqValueAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = "Default";
        name = name.ToUpper();

        return await this.Database.ExecuteSqlRawAsync(NextValueSql.Replace("[[NAME]]", name).Replace("[[TOTAL]]", 1.ToString()));
    }

    public async Task<IEnumerable<int>> NextSeqValuesAsync(string name, int total)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(total, 1);
        if (string.IsNullOrWhiteSpace(name))
            name = "Default";
        name = name.ToUpper();
        var val = await this.Database.ExecuteSqlRawAsync(NextValueSql.Replace("[[NAME]]", name).Replace("[[TOTAL]]", total.ToString()));
        return Enumerable.Range(val - total + 1, total);
    }
}
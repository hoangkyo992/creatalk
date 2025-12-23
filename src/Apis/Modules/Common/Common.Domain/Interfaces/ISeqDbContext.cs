namespace Common.Domain.Interfaces;

public interface ISeqDbContext
{
    Task<IEnumerable<int>> NextSeqValuesAsync(string name, int total);

    Task<int> NextSeqValueAsync(string name);
}
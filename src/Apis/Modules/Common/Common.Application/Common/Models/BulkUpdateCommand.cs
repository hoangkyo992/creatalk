namespace Common.Application.Common.Models;

public record BulkUpdateCommand
{
    [JsonConverter(typeof(ZCodeCollectionJsonConverter<IEnumerable<long>>))]
    public IEnumerable<long> Ids { get; init; }

    public bool PartialUpdate { get; set; }
}
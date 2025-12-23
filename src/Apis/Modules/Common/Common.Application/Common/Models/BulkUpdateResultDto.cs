namespace Common.Application.Common.Models;

public record BulkUpdateResultDto
{
    public List<BulkUpdateResultItemDto> ResultSet { get; init; } = [];
}
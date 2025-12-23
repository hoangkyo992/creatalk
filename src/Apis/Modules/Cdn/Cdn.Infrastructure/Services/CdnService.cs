using Cdn.Application.Features.Files;
using Common.Application.Services;
using MediatR;

namespace Cdn.Infrastructure.Services;

public class CdnService(IMediator mediator) : ICdnService
{
    public async Task<FileItem?> GetFileAsync(long id, CancellationToken cancellationToken = default)
    {
        var apiResult = await mediator.Send(new GetItem.Request() { Id = id }, cancellationToken);
        if (apiResult.IsSuccess)
        {
            return new FileItem
            {
                Id = apiResult.Result.Id,
                Name = apiResult.Result.Name,
                Size = apiResult.Result.Size,
                Url = apiResult.Result.Url,
                PublicUrl = apiResult.Result.PublicUrl,
                Extension = apiResult.Result.Extension,
                FileTypeId = (int)apiResult.Result.FileTypeId,
                Properties = apiResult.Result.Properties,
                MineType = apiResult.Result.MineType,
                FolderId = apiResult.Result.FolderId,
                FolderName = apiResult.Result.FolderName,
            };
        }
        return null;
    }
}
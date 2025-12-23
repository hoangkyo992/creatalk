import HttpService from "@/plugins/httpService";
import type { ApiResult, DataSourceResultDto, DataSourceRequestDto, EmptyResultDto } from "@/contracts/Common";
import type { AlbumFileItemDto, AlbumItemDto, UpdateAlbumFileRequestDto, UpdateAlbumRequestDto } from "@/contracts/Albums";

class AlbumService extends HttpService {
  public async getList(query: DataSourceRequestDto): Promise<ApiResult<DataSourceResultDto<AlbumItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<AlbumItemDto>>>(`/api/albums`, { params: query });
    return result.data;
  }

  public async get(id: string): Promise<ApiResult<AlbumItemDto>> {
    const result = await this.cmsClient.get<ApiResult<AlbumItemDto>>(`/api/albums/${id}`);
    return result.data;
  }

  public async update(id: string, payload: UpdateAlbumRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/albums/${id}`, payload);
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/albums/${id}`, {
      data: {}
    });
    return result.data;
  }

  public async create(payload: UpdateAlbumRequestDto): Promise<EmptyResultDto> {
    const result = await this.cmsClient.post<EmptyResultDto>(`/api/albums`, payload);
    return result.data;
  }

  public async getFiles(id: string, query: DataSourceRequestDto): Promise<ApiResult<DataSourceResultDto<AlbumFileItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<AlbumFileItemDto>>>(`/api/albums/${id}/files`, { params: query });
    return result.data;
  }

  public async addFiles(id: string, fileIds: string[]): Promise<EmptyResultDto> {
    const result = await this.cmsClient.post<EmptyResultDto>(`/api/albums/${id}/files`, {
      fileIds: fileIds
    });
    return result.data;
  }

  public async removeFiles(id: string, fileIds: string[]): Promise<EmptyResultDto> {
    const result = await this.cmsClient.delete<EmptyResultDto>(`/api/albums/${id}/files`, {
      data: {
        fileIds: fileIds
      }
    });
    return result.data;
  }

  public async updateFile(id: string, fileId: string, payload: UpdateAlbumFileRequestDto): Promise<EmptyResultDto> {
    const result = await this.cmsClient.put<EmptyResultDto>(`/api/albums/${id}/files/${fileId}`, payload);
    return result.data;
  }

  public async updatePositions(id: string, fileIds: string[]): Promise<EmptyResultDto> {
    const result = await this.cmsClient.post<EmptyResultDto>(`/api/albums/${id}/files/positions`, {
      fileIds: fileIds
    });
    return result.data;
  }
}

export default AlbumService;

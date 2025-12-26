import type { FileItemDto, FileMoveRequestDto, FileRenameRequestDto, UploadImagesResponseDto } from "@/contracts/FileAndFolders";
import type { ApiResult, DataSourceRequestDto, DataSourceResultDto, EmptyResultDto } from "../contracts/Common";
import HttpService from "../plugins/httpService";

class FileService extends HttpService {
  public async getList(query: DataSourceRequestDto): Promise<ApiResult<DataSourceResultDto<FileItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<FileItemDto>>>(`/api/files`, { params: query });
    return result.data;
  }

  public async upload(folderId: string, files: File[]): Promise<ApiResult<UploadImagesResponseDto>> {
    const formData = new FormData();
    files.forEach((element) => {
      formData.append("files", element);
    });
    const result = await this.cmsClient.post<ApiResult<UploadImagesResponseDto>>(`/api/files/upload?folderId=${folderId}`, formData, {
      headers: {
        "Content-Type": "multipart/form-data"
      }
    });
    return result.data;
  }

  public async get(id: string): Promise<ApiResult<FileItemDto>> {
    const result = await this.cmsClient.get<ApiResult<FileItemDto>>(`/api/files/${id}`);
    return result.data;
  }

  public async move(id: string, payload: FileMoveRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/files/${id}/move`, payload);
    return result.data;
  }

  public async rename(id: string, payload: FileRenameRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/files/${id}/rename`, payload);
    return result.data;
  }

  public async moveToTrash(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/files/${id}/move-to-trash`, {});
    return result.data;
  }

  public async restoreFromTrash(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/files/${id}/restore-from-trash`, {});
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/files/${id}`, {
      data: {}
    });
    return result.data;
  }
}

export default FileService;

import type {
  FolderDetailItemDto,
  FolderItemItemDto,
  FolderMoveRequestDto,
  FolderRenameRequestDto,
  FoldersResponseDto,
  UpdateFolderRequestDto
} from "@/contracts/FileAndFolders";
import type { ApiResult, EmptyResultDto } from "../contracts/Common";
import HttpService from "../plugins/httpService";
import type { FileType } from "@/contracts/Enums";

class FolderService extends HttpService {
  public async getList(parentId?: string): Promise<ApiResult<FoldersResponseDto>> {
    const result = await this.cmsClient.get<ApiResult<FoldersResponseDto>>(`/api/folders`, {
      params: {
        parentId: parentId
      }
    });
    return result.data;
  }

  public async get(id: string, typeId?: FileType): Promise<ApiResult<FolderDetailItemDto>> {
    const result = await this.cmsClient.get<ApiResult<FolderDetailItemDto>>(`/api/folders/${id}`, {
      params: {
        typeId: typeId
      }
    });
    return result.data;
  }

  public async getInTrash(): Promise<ApiResult<FolderDetailItemDto>> {
    const result = await this.cmsClient.get<ApiResult<FolderDetailItemDto>>(`/api/folders/trash`);
    return result.data;
  }

  public async create(payload: UpdateFolderRequestDto): Promise<ApiResult<FolderItemItemDto>> {
    const result = await this.cmsClient.post<ApiResult<FolderItemItemDto>>(`/api/folders`, payload);
    return result.data;
  }

  public async update(id: string, payload: UpdateFolderRequestDto): Promise<ApiResult<FolderItemItemDto>> {
    const result = await this.cmsClient.put<ApiResult<FolderItemItemDto>>(`/api/folders/${id}`, payload);
    return result.data;
  }

  public async move(id: string, payload: FolderMoveRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/folders/${id}/move`, payload);
    return result.data;
  }

  public async moveToTrash(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/folders/${id}/move-to-trash`, {});
    return result.data;
  }

  public async restoreFromTrash(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/folders/${id}/restore-from-trash`, {});
    return result.data;
  }

  public async rename(id: string, payload: FolderRenameRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/folders/${id}/rename`, payload);
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/folders/${id}`, {
      data: {}
    });
    return result.data;
  }
}

export default FolderService;

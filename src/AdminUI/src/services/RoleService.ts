import HttpService from "@/plugins/httpService";
import type { ApiResult, DataSourceResultDto, DataSourceRequestDto, EmptyResultDto } from "@/contracts/Common";
import type { RoleFeatureResponseDto, RoleItemDto, UpdateRoleRequestDto, UpdateRoleFeatureRequestDto } from "@/contracts/Roles";

class RoleService extends HttpService {
  public async getList(query: DataSourceRequestDto): Promise<ApiResult<DataSourceResultDto<RoleItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<RoleItemDto>>>(`/api/roles`, { params: query });
    return result.data;
  }

  public async get(id: string): Promise<ApiResult<RoleItemDto>> {
    const result = await this.cmsClient.get<ApiResult<RoleItemDto>>(`/api/roles/${id}`);
    return result.data;
  }

  public async update(id: string, payload: UpdateRoleRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/roles/${id}`, payload);
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/roles/${id}`, {
      data: {}
    });
    return result.data;
  }

  public async create(payload: UpdateRoleRequestDto): Promise<EmptyResultDto> {
    const result = await this.cmsClient.post<EmptyResultDto>(`/api/roles`, payload);
    return result.data;
  }

  public async getFeatures(id: string): Promise<ApiResult<RoleFeatureResponseDto>> {
    const result = await this.cmsClient.get<ApiResult<RoleFeatureResponseDto>>(`/api/roles/${id}/features`);
    return result.data;
  }

  public async updateFeatures(id: string, payload: UpdateRoleFeatureRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.post<ApiResult<EmptyResultDto>>(`/api/roles/${id}/features`, payload);
    return result.data;
  }
}

export default RoleService;

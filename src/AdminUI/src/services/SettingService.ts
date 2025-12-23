import HttpService from "@/plugins/httpService";
import type { ApiResult, DataSourceResultDto, DataSourceRequestDto, EmptyResultDto } from "@/contracts/Common";
import type { SettingItemDto, UpdateSettingReqDto } from "@/contracts/Settings";

class SettingService extends HttpService {
  public async getList(query: DataSourceRequestDto): Promise<ApiResult<DataSourceResultDto<SettingItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<SettingItemDto>>>(`/api/settings`, { params: query });
    return result.data;
  }

  public async get(id: string): Promise<ApiResult<SettingItemDto>> {
    const result = await this.cmsClient.get<ApiResult<SettingItemDto>>(`/api/settings/${id}`);
    return result.data;
  }

  public async update(id: string, payload: UpdateSettingReqDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/settings/${id}`, payload);
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/settings/${id}`, {
      data: {}
    });
    return result.data;
  }

  public async create(payload: UpdateSettingReqDto): Promise<EmptyResultDto> {
    const result = await this.cmsClient.post<EmptyResultDto>(`/api/settings`, payload);
    return result.data;
  }

  public async getByKey(key: string): Promise<ApiResult<SettingItemDto>> {
    const result = await this.cmsClient.get<ApiResult<SettingItemDto>>(`/api/settings/key`, {
      params: {
        key: key
      }
    });
    return result.data;
  }
}

export default SettingService;

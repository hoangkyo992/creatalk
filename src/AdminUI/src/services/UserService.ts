import type { ApiResult, DataSourceResultDto, DataSourceRequestDto, EmptyResultDto } from "@/contracts/Common";
import type { UserItemDto, UserSessionLogItemDto, UpdateUserRequestDto } from "@/contracts/Users";
import HttpService from "@/plugins/httpService";

class UserService extends HttpService {
  public async getList(query: DataSourceRequestDto): Promise<ApiResult<DataSourceResultDto<UserItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<UserItemDto>>>(`/api/users`, { params: query });
    return result.data;
  }

  public async get(id: string): Promise<ApiResult<UserItemDto>> {
    const result = await this.cmsClient.get<ApiResult<UserItemDto>>(`/api/users/${id}`);
    return result.data;
  }

  public async update(id: string, payload: UpdateUserRequestDto): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/users/${id}`, payload);
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/users/${id}`, {
      data: {}
    });
    return result.data;
  }

  public async create(payload: UpdateUserRequestDto): Promise<EmptyResultDto> {
    const result = await this.cmsClient.post<EmptyResultDto>(`/api/users`, payload);
    return result.data;
  }

  public async resetPassword(userId: string, newPassword: string): Promise<EmptyResultDto> {
    const result = await this.cmsClient.put<EmptyResultDto>(`/api/users/${userId}/reset-password`, {
      newPassword: newPassword
    });
    return result.data;
  }

  public async getUserSessions(
    query: DataSourceRequestDto,
    startTime: Date,
    endTime: Date,
    filterTimeType: string
  ): Promise<ApiResult<DataSourceResultDto<UserSessionLogItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<UserSessionLogItemDto>>>(
      `/api/users/sessions?startTime=${startTime.toJSON()}&endTime=${endTime.toJSON()}&filterTimeType=${filterTimeType}`,
      { params: query }
    );
    return result.data;
  }
}

export default UserService;

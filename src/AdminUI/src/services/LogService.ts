import type { ApiResult, DataSourceResultDto, DataSourceRequestDto } from "../contracts/Common";
import type { ActivityLogItemDto } from "../contracts/Logs";
import HttpService from "../plugins/httpService";

class LogService extends HttpService {
  public async getActivities(query: DataSourceRequestDto, startTime: Date, endTime: Date): Promise<ApiResult<DataSourceResultDto<ActivityLogItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<ActivityLogItemDto>>>(
      `/api/logs/activities?startTime=${startTime.toJSON()}&endTime=${endTime.toJSON()}`,
      { params: query }
    );
    return result.data;
  }
}

export default LogService;

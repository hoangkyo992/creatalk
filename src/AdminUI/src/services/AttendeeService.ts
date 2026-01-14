import type { ApiResult, DataSourceResultDto, DataSourceRequestDto, EmptyResultDto } from "@/contracts/Common";
import type { AttendeeItemDto } from "@/contracts/Attendees";
import HttpService from "@/plugins/httpService";
import type { AttendeeStatus, MessageStatus } from "@/contracts/Enums";

class AttendeeService extends HttpService {
  public async getList(
    query: DataSourceRequestDto,
    keyword: string,
    startTime: Date | undefined,
    endTime: Date | undefined,
    messageStatusIds: MessageStatus[],
    providerCode: string,
    isSent: boolean,
    statusId: AttendeeStatus | undefined
  ): Promise<ApiResult<DataSourceResultDto<AttendeeItemDto>>> {
    const result = await this.cmsClient.get<ApiResult<DataSourceResultDto<AttendeeItemDto>>>(`/api/attendees`, {
      params: {
        ...query,
        query: keyword,
        startTime,
        endTime,
        messageStatusIds: messageStatusIds.join(","),
        providerCode,
        isSent,
        statusId
      }
    });
    return result.data;
  }

  public async get(id: string): Promise<ApiResult<AttendeeItemDto>> {
    const result = await this.cmsClient.get<ApiResult<AttendeeItemDto>>(`/api/attendees/${id}`);
    return result.data;
  }

  public async delete(id: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.delete<ApiResult<EmptyResultDto>>(`/api/attendees/${id}`, {
      data: {}
    });
    return result.data;
  }

  public async cancel(id: string, isCancelled: boolean): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.post<ApiResult<EmptyResultDto>>(`/api/attendees/${id}/cancel`, {
      isCancelled
    });
    return result.data;
  }

  public async updatePhoneNumber(id: string, phoneNumber: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.put<ApiResult<EmptyResultDto>>(`/api/attendees/${id}/phone-number`, {
      phoneNumber
    });
    return result.data;
  }

  public async resendMessage(id: string, providerCode: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.post<ApiResult<EmptyResultDto>>(`/api/attendees/${id}/messages/resend`, {
      providerCode
    });
    return result.data;
  }

  public async createMessages(providerCode: string): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.post<ApiResult<EmptyResultDto>>(`/api/attendees/messages`, {
      providerCode
    });
    return result.data;
  }
}

export default AttendeeService;

import type { ApiResult, EmptyResultDto } from "@/contracts/Common";
import type { LoginResponseDto, LoginRequestDto, PasswordChangeRequestDto, UserProfileItemDto } from "@/contracts/Auths";
import HttpService from "@/plugins/httpService";

class AuthService extends HttpService {
  public async signIn(payload: LoginRequestDto): Promise<ApiResult<LoginResponseDto>> {
    const result = await this.cmsClient.post<ApiResult<LoginResponseDto>>(`/api/auth/signIn`, payload);
    return result.data;
  }

  public async signOut(): Promise<ApiResult<EmptyResultDto>> {
    const result = await this.cmsClient.post<ApiResult<EmptyResultDto>>(`/api/auth/signOut`, {});
    return result.data;
  }

  public async getProfile(token: string): Promise<UserProfileItemDto> {
    const result = await this.cmsClient.get<UserProfileItemDto>(`/api/auth/me`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return result.data;
  }

  public async changePassword(request: PasswordChangeRequestDto): Promise<ApiResult<any>> {
    const result = await this.cmsClient.post<ApiResult<any>>(`/api/auth/change-password`, request);
    return result.data;
  }

  public async getUserAccesses(token: string): Promise<any> {
    const result = await this.cmsClient.get<any>(`/api/auth/accesses`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return result.data;
  }
}

export default AuthService;

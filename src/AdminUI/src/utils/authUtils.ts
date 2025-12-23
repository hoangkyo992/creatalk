import StorageKeys from "@/models/StorageKeys";
import localStorageUtils from "./localStorageUtils";
import type { UserProfileItemDto, LoginResponseDto } from "@/contracts/Auths";

class AuthUtils {
  getAuthentication(): LoginResponseDto {
    const authentication = localStorageUtils.getItem(StorageKeys.Authentication);
    return JSON.parse(authentication?.length > 0 ? authentication : "{}") as LoginResponseDto;
  }

  getProfile(): UserProfileItemDto {
    const profileData = localStorageUtils.getItem(StorageKeys.Profile);
    return JSON.parse(profileData?.length > 0 ? profileData : "{}") as UserProfileItemDto;
  }

  clearAuthentication(): void {
    localStorageUtils.removeItem(StorageKeys.Authentication);
    localStorageUtils.removeItem(StorageKeys.Profile);
  }
}

export default new AuthUtils();

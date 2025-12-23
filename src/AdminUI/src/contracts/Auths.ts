export interface LoginRequestDto {
  username: string;
  password: string;
}

export interface LoginResponseDto {
  access_token: string;
  expire_in: number;
  refresh_token: string;
  token_type: string;
}

export interface UserProfileItemDto {
  id?: string;
  username?: string;
  displayName?: string;
  email?: string;
  phone?: string;
}

export interface PasswordChangeRequestDto {
  password: string;
  newPassword: string;
  confirmPassword: string;
}

export interface AppFeatureDto {
  name: string;
  actions: string[];
}

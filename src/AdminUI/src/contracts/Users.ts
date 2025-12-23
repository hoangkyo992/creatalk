import type { UserStatus } from "./Enums";
import type { BaseDto } from "./Common";

export interface UserItemDto extends BaseDto {
  username: string;
  displayName: string;
  email: string;
  phone: string;
  statusId: UserStatus;
  statusCode: string;
  roles: string[];
}

export interface UpdateUserRequestDto {
  username: string;
  displayName: string;
  email: string;
  phone: string;
  password?: string;
  roleIds: string[];
  statusId: UserStatus;
  confirmPassword?: string;
}

export interface ResetPasswordRequestDto {
  newPassword: string;
}

export interface UserSessionLogItemDto extends BaseDto {
  startTime: Date;
  endTime?: Date;
  endBy: string;
  iP: string;
  platform: string;
  navigator: string;
  userId: string;
  username: string;
}

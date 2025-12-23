import type { BaseDto } from "./Common";

export interface SettingItemDto extends BaseDto {
  key: string;
  value: string;
}

export interface UpdateSettingReqDto {
  key: string;
  value: string;
}

import type { BaseDto } from "./Common";

export interface RoleItemDto extends BaseDto {
  name: string;
  description: string;
}

export interface UpdateRoleRequestDto {
  name: string;
  description: string;
}

export interface FeatureItemDto {
  name: string;
  action: string;
}

export interface AppFeatureItemDto {
  id: string;
  module: string;
  name: string;
  description: string;
  actions: AppFeatureActionItemDto[];
}

export interface AppFeatureActionItemDto {
  id: string;
  name: string;
  description: string;
}

export interface RoleFeatureResponseDto {
  features: FeatureItemDto[];
  appFeatures: AppFeatureItemDto[];
}

export interface UpdateRoleFeatureRequestDto {
  features: FeatureItemDto[];
}

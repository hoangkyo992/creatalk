import type { ValueComparison } from "./Enums";

export interface ApiResult<T> {
  isSuccess: boolean;
  result: T;
  statusCode: number;
  failureReason: string;
  data: object;
}

export interface DataSourceRequestDto {
  page: number;
  pageSize: number;
  sort?: string;
  filter?: string;
}

export interface DataSourceResultDto<T> {
  data: Array<T>;
  total: number;
  additionalData?: any;
}

/* eslint-disable @typescript-eslint/no-empty-object-type */
export interface EmptyResultDto {}

export interface CreateOrUpdateResultDto {
  id: string;
}

export interface BulkUpdateResultDto {
  resultSet: Array<BulkUpdateResultItemDto>;
}

export interface DownloadResultDto {
  fileName: string;
  content: any;
}

export interface BulkUpdateRequestDto {
  ids: Array<string>;
  partialUpdate: boolean;
}

export interface BulkUpdateResultItemDto {
  id: string;
  isSuccess: boolean;
  isUpdatable: boolean;
  statusCode: number;
  failureReason: string;
  data: object;
}

export interface ColumnOrder {
  k: string;
  d: string;
  i?: number;
}

export interface ColumnFilter {
  k: string;
  c: ValueComparison;
  v: any;
}

export interface BaseDto {
  id: string;
  createdBy: string;
  createdTime: Date;
  updatedBy?: string;
  updatedTime?: Date;
}

export interface IdDto {
  id: string;
}

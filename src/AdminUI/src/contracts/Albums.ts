import type { BaseDto } from "./Common";
import type { FileStatus, FileType } from "./Enums";

export interface AlbumItemDto extends BaseDto {
  name: string;
  description: string;
  numberOfItems: number;
  items: AlbumFileItemDto[];
}

export interface UpdateAlbumRequestDto {
  name: string;
  description: string;
}

export interface UpdateAlbumFileRequestDto {
  title: string;
  description: string;
  index: number;
}

export interface AlbumFileItemDto extends BaseDto {
  title: string;
  description: string;
  index: number;
  fileId: string;
  name: string;
  size: 0;
  statusId: FileStatus;
  url: string;
  fileTypeId: FileType;
  extension: string;
  mineType: string;
  properties: string;
}

import type { BaseDto } from "./Common";
import type { FileStatus, FileType, FolderStatus } from "./Enums";

export interface FolderItemDto extends BaseDto {
  name: string;
  parentId?: string;
  statusId: FolderStatus;
  statusCode: string;
}

export interface FoldersResponseDto {
  items: FolderItemDto[];
}

export interface FolderDetailItemDto extends FolderItemDto {
  items: FolderItemItemDto[];
}

export interface FolderItemItemDto extends BaseDto {
  size: number;
  isDirectory: boolean;
  name: string;
  url?: string;
  statusId: number;
  parentId?: string;
  fileTypeId: FileType;
  extension?: string;
  properties?: string;
  mineType?: string;
}

export interface UpdateFolderRequestDto {
  name: string;
  parentId?: string;
}

export interface FolderMoveRequestDto {
  parentId: string;
}

export interface FolderRenameRequestDto {
  name: string;
}

export interface FileItemDto extends BaseDto {
  name: string;
  url: string;
  content: string;
  folderId: string;
  folderName: string;
  statusId: FileStatus;
  statusCode: string;
  size: number;
  extension: string;
  publicUrl: string;
  mineType: string;
}

export interface FileMoveRequestDto {
  folderId: string;
}

export interface FileRenameRequestDto {
  name: string;
}

export interface UploadImagesResponseDto {
  uploadedFiles: FileItemDto[];
}

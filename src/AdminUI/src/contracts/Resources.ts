export interface ResourcesResponseDto {
  languages: LanguageItemDto[];
}

export interface LanguageItemDto {
  code: string;
  name: string;
  icon: string;
  resources: ResourceItemDto[];
}

export interface ResourceItemDto {
  code: string;
  name: string;
}

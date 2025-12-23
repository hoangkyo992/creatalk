export interface MediaSetting {
  imageSetting: ImageSetting;
}

export interface ImageSetting {
  cropExact: boolean;
  mediumImageSize: ImageSize;
  largeImageSize: ImageSize;
  thumbnailImageSize: ImageSize;
}

export interface ImageSize {
  maxWidth: number;
  maxHeight: number;
}

export interface GeneralSetting {
  siteTitle: string;
  tagline: string;
  siteIcon: string;
  siteLogo: string;
  gA4Id: string;
}

export interface SocialNetworkSetting {
  items: SocialNetworkItem[];
}

export interface SocialNetworkItem {
  name: string;
  link: string;
  enable: boolean;
}

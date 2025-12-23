class CdnUtils {
  public cdnUrl: string = import.meta.env.VITE_APP_CDN_URL;

  getImageUrl(imageId: string, size: string = "large") {
    return `${this.cdnUrl}/media/files/${imageId}?size=${size}`;
  }
}

export default new CdnUtils();

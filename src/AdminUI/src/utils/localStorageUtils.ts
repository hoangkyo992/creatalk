class LocalStorageUtils {
  public setItem(key: string, value: string): void {
    if (value) {
      localStorage.setItem(key, value);
    } else {
      localStorage.removeItem(key);
    }
  }

  public getItem(key: string): string {
    return localStorage.getItem(key) || "";
  }

  public removeItem(key: string): void {
    localStorage.removeItem(key);
  }
}

export default new LocalStorageUtils();

import type { DownloadResultDto } from "@/contracts/Common";

export function useAppDownloader() {
  const download = function (url, fileName) {
    const link = document.createElement("a");
    link.href = url;
    link.setAttribute("download", fileName);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };
  const downloadJson = function (response: DownloadResultDto) {
    const url = URL.createObjectURL(
      new Blob([response.content], {
        type: "application/json"
      })
    );
    download(url, response.fileName);
  };
  const downloadExcel = function (response: DownloadResultDto) {
    const binaryString = window.atob(response.content);
    const binaryLen = binaryString.length;
    const bytes = new Uint8Array(binaryLen);
    for (let i = 0; i < binaryLen; i++) {
      const ascii = binaryString.charCodeAt(i);
      bytes[i] = ascii;
    }
    const url = URL.createObjectURL(
      new Blob([bytes], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
      })
    );
    download(url, response.fileName);
  };

  return {
    downloadJson,
    downloadExcel
  };
}

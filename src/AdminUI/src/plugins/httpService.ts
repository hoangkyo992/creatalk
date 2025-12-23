import axios from "axios";
import type { AxiosInstance } from "axios";
import authUtils from "@/utils/authUtils";
import { emitter } from "@/plugins/pubsubService";

class HttpService {
  cmsClient: AxiosInstance;

  constructor() {
    const token = authUtils.getAuthentication()?.access_token;
    const authorization = `Bearer ${token}`;

    this.cmsClient = axios.create({
      baseURL: import.meta.env.MODE == "dev" ? "" : import.meta.env.VITE_API_BASE_URL,
      headers: {
        Authorization: authorization,
        "X-DateTime-Offset": new Date().getTimezoneOffset()
      }
    });

    this.cmsClient.interceptors.response.use(
      function (response) {
        return response;
      },
      function (error) {
        if (error.response.data?.failureReason?.length > 0) {
          emitter.emit("apiError", {
            error: error.response.data
          });
        }
        if (error?.response?.status === 401) {
          authUtils.clearAuthentication();
          window.location.href = `login`;
        }
        return Promise.reject(error.response);
      }
    );
  }
}

export default HttpService;

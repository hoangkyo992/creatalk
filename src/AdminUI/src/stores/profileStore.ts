import { defineStore } from "pinia";
import type { AppFeatureDto, UserProfileItemDto } from "@/contracts/Auths";
import AuthService from "@/services/AuthService";
import authUtils from "@/utils/authUtils";

const useProfileStore = defineStore("PROFILE_STORE", {
  state: () => ({
    isLoading: false,
    authService: new AuthService(),
    profile: {} as UserProfileItemDto,
    features: [] as Array<AppFeatureDto>
  }),
  getters: {
    displayName: (state) => state.profile.username,
    accesses: (state) => state.features
  },
  actions: {
    async getAccesses() {
      if (this.isLoading || this.features.length > 0) {
        return;
      }
      this.isLoading = true;
      const response = await this.authService.getUserAccesses(authUtils.getAuthentication().access_token);
      this.features = response.result.features;
      this.isLoading = false;
    },
    hasAccess(codes: string[]) {
      if (codes.length == 0) return true;
      return this.features.some((f) => {
        return codes.some((c) => c.toLowerCase() == f.name.toLowerCase());
      });
    },
    hasPermission(name: string, action: string) {
      const actions = action.split(",").map((x) => x.toUpperCase());
      return this.features.some((f) => {
        return f.name.toLowerCase() == name.toLowerCase() && f.actions.some((a) => actions.indexOf(a) > -1);
      });
    }
  }
});

export default useProfileStore;

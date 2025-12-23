import AppRoutes from "@/models/AppRoutes";
import type { NavigationGuardNext, RouteLocationNormalizedLoaded } from "vue-router";
import authUtils from "@/utils/authUtils";
import profilerStore from "@/stores/profileStore";
class AuthNavigationGuard {
  public async requireLogin(to: RouteLocationNormalizedLoaded, from: RouteLocationNormalizedLoaded, next: NavigationGuardNext): Promise<void> {
    if (to.meta.requiresAuth !== false) {
      const token = authUtils.getAuthentication()?.access_token;
      if (!token) {
        return next({ name: AppRoutes.Login, query: { returnUrl: to.fullPath } });
      }
      if (to.meta?.features) {
        const store = profilerStore();
        await store.getAccesses();
        const hasAccess = store.hasAccess(to.meta.features as any);
        if (!hasAccess) {
          return next({ name: AppRoutes.Forbidden, query: { returnUrl: from.fullPath } });
        }
      }
    }
    return next();
  }
}

export default new AuthNavigationGuard();

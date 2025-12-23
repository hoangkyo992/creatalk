import { createRouter, createWebHistory } from "vue-router";
import { setupLayouts } from "virtual:generated-layouts";
import generatedRoutes from "virtual:generated-pages";
import AuthNavigationGuard from "@/plugins/auth/AuthNavigationGuard";

const routes = setupLayouts(generatedRoutes);
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
});

router.beforeResolve(AuthNavigationGuard.requireLogin);

export default router;

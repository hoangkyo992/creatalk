<script setup lang="ts">
import { computed, watch, ref, onMounted } from "vue";
import { useLayout } from "@/composables/appLayout";
import { emitter } from "@/plugins/pubsubService";
import { useToast } from "primevue/usetoast";
import { useI18n } from "vue-i18n";
import authUtils from "@/utils/authUtils";
import AuthService from "@/services/AuthService";

const { layoutConfig, layoutState, isSidebarActive } = useLayout();
const $t = useI18n();
const outsideClickListener = ref();
const toast = useToast();
const authService = new AuthService();

watch(isSidebarActive, (newVal) => {
  if (newVal) {
    bindOutsideClickListener();
  } else {
    unbindOutsideClickListener();
  }
});

const containerClass = computed(() => {
  return {
    "layout-theme-light": layoutConfig.darkTheme.value === false,
    "layout-theme-dark": layoutConfig.darkTheme.value === true,
    "layout-overlay": layoutConfig.menuMode.value === "overlay",
    "layout-static": layoutConfig.menuMode.value === "static",
    "layout-static-inactive": layoutConfig.menuMode.value === "static",
    "layout-overlay-active": layoutState.overlayMenuActive.value,
    "layout-mobile-active": layoutState.staticMenuMobileActive.value,
    "p-input-filled": layoutConfig.inputStyle.value === "filled",
    "p-ripple-disabled": !layoutConfig.ripple.value
  };
});
const bindOutsideClickListener = () => {
  if (!outsideClickListener.value) {
    outsideClickListener.value = (event) => {
      if (isOutsideClicked(event)) {
        layoutState.overlayMenuActive.value = false;
        layoutState.staticMenuMobileActive.value = false;
        layoutState.menuHoverActive.value = false;
      }
    };
    document.addEventListener("click", outsideClickListener.value);
  }
};
const unbindOutsideClickListener = () => {
  if (outsideClickListener.value) {
    document.removeEventListener("click", outsideClickListener.value);
    outsideClickListener.value = null;
  }
};
const isOutsideClicked = (event) => {
  const sidebarEl = document.querySelector(".layout-sidebar");
  const topbarEl = document.querySelector(".layout-menu-button");

  return !(sidebarEl?.isSameNode(event.target) || sidebarEl?.contains(event.target) || topbarEl?.isSameNode(event.target) || topbarEl?.contains(event.target));
};

const isAuthenticated = computed(() => {
  const profileValue = authUtils.getProfile();
  if (!profileValue?.id) {
    return;
  }

  return true;
});

const onLogout = async () => {
  await authService.signOut();
  authUtils.clearAuthentication();
  window.location.href = `login`;
};

onMounted(async () => {
  emitter.on("apiError", (data: any) => {
    const msg = $t.t(`ServerErrors.${data.error.failureReason}`, data.error.data);
    toast.add({ severity: "error", summary: $t.t(`Common.ToastError`), detail: msg, life: 5000 });
  });
});
</script>

<template>
  <div class="layout-wrapper" :class="containerClass">
    <app-topbar :hide-menu-button="true" :is-authenticated="isAuthenticated" @on-logout="onLogout"></app-topbar>
    <div class="layout-main-container">
      <div class="layout-main p-fluid">
        <div class="p-fluid layout-content">
          <router-view></router-view>
        </div>
      </div>
    </div>
    <div class="layout-mask"></div>
  </div>
</template>

<style lang="scss" scoped></style>

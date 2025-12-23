<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useLayout } from "@/composables/appLayout";
import AppRoutes from "@/models/AppRoutes";
import authUtils from "@/utils/authUtils";
import { useRouter } from "vue-router";
import useProfileStore from "@/stores/profileStore";
import avatar from "@/assets/avatar/avatar_4.png";
import { Popover } from "primevue";

const profileStore = useProfileStore();
const { onMenuToggle } = useLayout();
const router = useRouter();
const outsideClickListener = ref();
const topbarMenuActive = ref(false);
const showPasswordChanged = ref(false);

const emit = defineEmits(["onLogout"]);
const props = defineProps({
  hideMenuButton: {
    type: Boolean,
    default: false
  },
  isAuthenticated: {
    type: Boolean,
    default: false
  }
});

const onTopBarMenuButton = () => {
  topbarMenuActive.value = !topbarMenuActive.value;
};
const topbarMenuClasses = computed(() => {
  return {
    "layout-topbar-menu-mobile-active": topbarMenuActive.value
  };
});

const bindOutsideClickListener = () => {
  if (!outsideClickListener.value) {
    outsideClickListener.value = (event) => {
      if (isOutsideClicked(event)) {
        topbarMenuActive.value = false;
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
  if (!topbarMenuActive.value) return;

  const sidebarEl = document.querySelector(".layout-topbar-menu") as any;
  const topbarEl = document.querySelector(".layout-topbar-menu-button") as any;

  return !(sidebarEl.isSameNode(event.target) || sidebarEl.contains(event.target) || topbarEl.isSameNode(event.target) || topbarEl.contains(event.target));
};

const onLogout = () => {
  emit("onLogout");
};

const userInfo = ref();
const showUserInfo = (event) => {
  userInfo.value.toggle(event);
};

const showLoginButton = computed(() => {
  return !props.isAuthenticated && router.currentRoute.value.name !== AppRoutes.Login;
});

const loadUserProfile = () => {
  const profileValue = authUtils.getProfile();
  if (!profileValue?.id) {
    return;
  }

  profileStore.$patch({ profile: profileValue });
};

const displayName = computed(() => {
  return profileStore.displayName;
});

onMounted(() => {
  loadUserProfile();
  bindOutsideClickListener();
});

onBeforeUnmount(() => {
  unbindOutsideClickListener();
});
</script>

<template>
  <div class="layout-topbar">
    <router-link to="/" class="layout-topbar-logo"> <img src="@/assets/vikiworld.svg" width="150" alt="logo" /> Vikiworld </router-link>
    <button v-if="isAuthenticated && !hideMenuButton" class="border-none bg-transparent layout-menu-button layout-topbar-button" @click="onMenuToggle()">
      <i class="pi pi-bars"></i>
    </button>
    <button class="border-none bg-transparent layout-topbar-menu-button layout-topbar-button" @click="onTopBarMenuButton()">
      <i class="pi pi-ellipsis-v"></i>
    </button>
    <div class="layout-topbar-menu" :class="topbarMenuClasses">
      <div class="flex flex-row flex-wrap w-full align-items-center">
        <LanguageSelection class="mr-6"></LanguageSelection>
        <div v-if="isAuthenticated" class="flex flex-row flex-wrap cursor-pointer" @click="showUserInfo">
          <img class="ml-3 mr-5" src="@/assets/avatar/avatar_4.png" height="30" />
          <div class="flex align-items-center justify-content-center font-semibold ml-5">
            {{ displayName }}
          </div>
          <Popover ref="userInfo" class="flex">
            <div class="flex flex-column w-full">
              <Button class="w-full flex align-items-center pl-6 pr-6 py-6 justify-content-start" severity="secondary" text>
                <Avatar :image="avatar" class="mr-2" shape="circle" size="large" />
                <div class="flex flex-column ml-4 align-items-start">
                  <span class="font-bold">{{ profileStore.profile.displayName }}</span>
                  <span class="text-sm">{{ profileStore.profile.email }}</span>
                </div>
              </Button>
              <Button text severity="secondary" class="justify-content-start" @click="showPasswordChanged = true">
                <i class="pi pi-key p-button-small" />
                <span class="ml-6">{{ $t("TopMenu.Actions.ChangePassword") }}</span>
              </Button>
              <Button text class="justify-content-start" severity="secondary" @click="onLogout">
                <i class="pi pi-sign-out" />
                <span class="ml-6">{{ $t("TopMenu.Actions.Logout") }}</span>
              </Button>
            </div>
          </Popover>
        </div>
        <Button v-if="showLoginButton" text @click="$router.push({ name: AppRoutes.Login })">{{ $t("LoginPage.LoginButton") }}</Button>
        <ChangePassword :show-password-changed="showPasswordChanged" @close="showPasswordChanged = false"></ChangePassword>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped></style>

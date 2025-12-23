<route lang="yaml">
meta:
  layout: empty
  requiresAuth: false
</route>

<template>
  <div class="surface-ground flex align-items-center justify-content-center min-w-screen overflow-x-hidden h-screen">
    <div class="flex flex-column align-items-center justify-content-center" style="width: 80vw; max-width: 26rem; min-width: 20rem">
      <img src="@/assets/vikiworld.svg" alt="logo" class="mb-8 w-8rem flex-shrink-0" />
      <div
        style="border-radius: 56px; padding: 0.3rem; background: linear-gradient(180deg, var(--p-primary-color) 10%, rgba(33, 150, 243, 0) 30%)"
        class="w-full"
      >
        <div class="w-full surface-card" style="border-radius: 53px">
          <div class="text-center h-10rem">
            <img src="@/assets/avatar/avatar_4.png" alt="Image" height="50" class="mb-3" />
            <div class="text-900 text-3xl font-medium mb-3">
              {{ $t("LoginPage.Welcome") }}
            </div>
            <span class="text-600 font-medium">{{ $t("LoginPage.Description") }}</span>
          </div>
          <div>
            <form @submit.prevent="onLogin">
              <div class="flex flex-column h-6rem mb-2">
                <label for="username" class="block text-900 font-medium mb-4">{{ $t("LoginPage.Username") }}</label>
                <InputText
                  id="username"
                  v-model="username"
                  type="text"
                  :placeholder="$t('LoginPage.Username')"
                  :invalid="v.username.$errors.length > 0"
                  :class="['w-full', 'mb-2']"
                  @focus="errorMessage = ''"
                />
                <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.username) }}</Message>
              </div>
              <div class="flex flex-column h-6rem mb-2">
                <label for="password" class="block text-900 font-medium mb-4">{{ $t("LoginPage.Password") }}</label>
                <Password
                  id="password"
                  v-model="password"
                  :feedback="false"
                  :placeholder="$t('LoginPage.Password')"
                  :toggle-mask="true"
                  :invalid="v.password.$errors.length > 0"
                  :class="['w-full', 'mb-2']"
                  input-class="w-full"
                  @focus="errorMessage = ''"
                ></Password>
                <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.password) }}</Message>
              </div>
              <div v-if="errorMessage" class="flex justify-content-center text-center w-full mb-8">
                <Message severity="error" variant="simple">{{ errorMessage }}</Message>
              </div>
              <div class="mt-5">
                <Button
                  :loading="isLoading"
                  :disabled="v.$errors.length > 0"
                  :label="$t('LoginPage.LoginButton')"
                  class="w-full p-3 h-3rem"
                  type="submit"
                ></Button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import localStorageUtils from "@/utils/localStorageUtils";
import AppRoutes from "@/models/AppRoutes";
import storageKeys from "@/models/StorageKeys";
import AuthService from "@/services/AuthService";
import type { LoginRequestDto } from "@/contracts/Auths";
import { computed, onMounted, ref } from "vue";
import { useVuelidate } from "@vuelidate/core";
import { required, helpers } from "@vuelidate/validators";
import { useRoute, useRouter } from "vue-router";
import useProfileStore from "@/stores/profileStore";
import { useAppValidation } from "@/composables/appValidation";
import authUtils from "@/utils/authUtils";
import { useI18n } from "vue-i18n";

const $t = useI18n().t;

const isLoading = ref<boolean>(false);
const username = ref("");
const password = ref("");
const errorMessage = ref("");
const authService = new AuthService();
const store = useProfileStore();
const router = useRouter();

const onLogin = async () => {
  v.value.$touch();

  if (v.value.$errors.length) {
    return;
  }

  isLoading.value = true;
  const requestDto: LoginRequestDto = {
    username: username.value,
    password: password.value
  };

  try {
    const loginResult = await authService.signIn(requestDto);
    if (!loginResult.isSuccess) {
      errorMessage.value = $t("LoginPage.IncorrectUsernameOrPassword");
      return;
    }
    localStorageUtils.setItem(storageKeys.Authentication, JSON.stringify(loginResult.result));
    await loadProfile();
    router.push({ name: AppRoutes.Home });
  } catch (error: any) {
    errorMessage.value = $t(`ServerErrors.${error?.data?.failureReason}`, error?.data?.data);
  } finally {
    isLoading.value = false;
  }
};
const loadProfile = async () => {
  const userProfile = await authService.getProfile(authUtils.getAuthentication()?.access_token);
  localStorageUtils.setItem(storageKeys.Profile, JSON.stringify(userProfile));
  store.profile.username = userProfile.username || "";
};
onMounted(() => {
  const authentication = localStorageUtils.getItem(storageKeys.Authentication);
  if (authentication) {
    const route = useRoute();
    const returnUrl = route.query["returnUrl"] as string;
    router.push(returnUrl ?? { name: AppRoutes.Home });
  }
});

const validator = useAppValidation();
const rules = computed(() => ({
  username: {
    required: helpers.withMessage($t("LoginPage.UsernameIsRequired"), required)
  },
  password: {
    required: helpers.withMessage($t("LoginPage.PasswordIsRequired"), required)
  }
}));
const v = useVuelidate(rules, { username, password });
</script>

<style scoped>
.surface-card {
  padding-left: 2rem;
  padding-right: 2rem;
  padding-top: 2rem;
  padding-bottom: 4rem;
}
</style>

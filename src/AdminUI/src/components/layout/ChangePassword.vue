<template>
  <Dialog
    :visible="props.showPasswordChanged"
    :style="{ width: '90vw', maxWidth: '480px', minWidth: '360px' }"
    modal
    :header="$t('ChangePassword.Form.Header')"
    class="overflow-hidden-content"
    @update:visible="close"
  >
    <div style="padding: 0.3rem; margin: auto">
      <div class="w-full surface-card pt-2">
        <div class="text-center mb-8">
          <img src="@/assets/avatar/avatar_4.png" alt="Image" height="50" class="mb-4" />
          <div class="text-900 text-3xl font-medium mb-3 overflow-hidden text-ellipsis">
            {{ $t("ChangePassword.Form.Title", [store.displayName]) }}
          </div>
          <span class="text-600 font-medium">{{ $t("ChangePassword.Form.Description") }}</span>
        </div>
        <div>
          <form @submit.prevent="changePassword">
            <div class="flex flex-column mb-6">
              <label for="password" class="block text-900 font-medium mb-4">{{ $t("ChangePassword.Form.Password") }}</label>
              <Password
                id="password"
                v-model="password"
                size="small"
                :placeholder="$t('ChangePassword.Form.Password')"
                :toggle-mask="true"
                :feedback="false"
                :invalid="v.password.$errors.length > 0"
                :class="['w-full', 'mb-1']"
                input-class="w-full p-inputtext-sm"
              ></Password>
              <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.password) }}</Message>
            </div>
            <div class="flex flex-column mb-6">
              <label for="newPassword" class="block text-900 font-medium mb-4">{{ $t("ChangePassword.Form.NewPassword") }}</label>
              <Password
                id="newPassword"
                v-model="newPassword"
                size="small"
                :placeholder="$t('ChangePassword.Form.NewPassword')"
                :toggle-mask="true"
                :invalid="v.newPassword.$errors.length > 0"
                :class="['w-full', 'mb-1']"
                input-class="w-full p-inputtext-sm"
              ></Password>
              <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.newPassword) }}</Message>
            </div>
            <div class="flex flex-column mb-6">
              <label for="confirmPassword" class="block text-900 font-medium mb-4">{{ $t("ChangePassword.Form.ConfirmPassword") }}</label>
              <Password
                id="confirmPassword"
                v-model="confirmPassword"
                size="small"
                :placeholder="$t('ChangePassword.Form.ConfirmPassword')"
                :toggle-mask="true"
                :invalid="v.confirmPassword.$errors.length > 0"
                :class="['w-full', 'mb-1']"
                input-class="w-full p-inputtext-sm"
              ></Password>
              <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.confirmPassword) }}</Message>
            </div>
            <div class="flex align-content-end justify-content-end flex-wrap mt-8">
              <div class="w-6 pr-4">
                <Button
                  :loading="isLoading"
                  :disabled="v.$errors.length > 0"
                  size="small"
                  :label="$t('ChangePassword.Form.Change')"
                  class="w-full"
                  type="submit"
                ></Button>
              </div>
              <div class="w-6 pl-4">
                <Button
                  severity="danger"
                  outlined:loading="isLoading"
                  outlined
                  size="small"
                  :label="$t('ChangePassword.Form.Cancel')"
                  class="w-full"
                  @click="close"
                ></Button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  </Dialog>
</template>

<script lang="ts" setup>
import AuthService from "@/services/AuthService";
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { computed, ref } from "vue";
import { useVuelidate } from "@vuelidate/core";
import { required, helpers, sameAs } from "@vuelidate/validators";
import useProfileStore from "@/stores/profileStore";
import type { PasswordChangeRequestDto } from "@/contracts/Auths";
import { useI18n } from "vue-i18n";

interface Props {
  showPasswordChanged: boolean;
}
const props = defineProps<Props>();
const $t = useI18n().t;
const store = useProfileStore();

const emits = defineEmits(["close"]);

const isLoading = ref(false);
const notifier = useAppNotification();
const password = ref("");
const newPassword = ref("");
const confirmPassword = ref("");
const authService = new AuthService();

const changePassword = async () => {
  v.value.$touch();

  if (v.value.$errors.length) {
    return;
  }

  isLoading.value = true;
  const requestDto: PasswordChangeRequestDto = {
    password: password.value,
    newPassword: newPassword.value,
    confirmPassword: confirmPassword.value
  };

  try {
    const changePassResult = await authService.changePassword(requestDto);
    if (!changePassResult.isSuccess) {
      notifier.error($t("ServerErrors.AUTH_INVALID_PASSWORD"));
      return;
    }
    close();
  } finally {
    isLoading.value = false;
  }
};
const close = () => {
  v.value.$reset();
  confirmPassword.value = "";
  password.value = "";
  newPassword.value = "";
  emits("close");
};

const validator = useAppValidation();
const rules = computed(() => ({
  password: {
    required: helpers.withMessage($t("ChangePassword.Validation.PasswordRequired"), required)
  },
  newPassword: {
    required: helpers.withMessage($t("ChangePassword.Validation.NewPasswordRequired"), required)
  },
  confirmPassword: {
    sameAs: helpers.withMessage($t("ChangePassword.Validation.ConfirmSameAsNewPassword"), sameAs(newPassword))
  }
}));
const v = useVuelidate(rules, { password, newPassword, confirmPassword });
</script>

<style scoped>
.surface-card {
  padding-left: 2rem;
  padding-right: 2rem;
  padding-top: 2rem;
  padding-bottom: 4rem;
}
</style>

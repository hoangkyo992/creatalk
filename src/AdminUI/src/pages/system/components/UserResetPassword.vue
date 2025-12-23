<template>
  <Dialog
    :visible="true"
    :style="{ width: '90vw', maxWidth: '480px', minWidth: '360px' }"
    :header="$t('UserResetPasswordDialog.Header', [username])"
    :modal="true"
    :closable="false"
  >
    <form v-focustrap @submit.prevent class="grid formgrid p-fluid">
      <!-- IMPORTANT - ADD THIS FIELD TO AVOID USERNAME AUTOFILL ON GOOGLE CHROME -->
      <div class="field mb-6 col-12 hidden">
        <label for="username" class="required">{{ $t("UserResetPasswordDialog.Password") }}</label>
        <InputText id="username" size="small"></InputText>
      </div>
      <!-- IMPORTANT - ADD THIS FIELD TO AVOID USERNAME AUTOFILL ON GOOGLE CHROME -->
      <div class="field mb-6 col-12">
        <label for="password" class="required">{{ $t("UserResetPasswordDialog.Password") }}</label>
        <Password
          id="password"
          v-model="item.password"
          :feedback="true"
          :toggle-mask="true"
          :invalid="v.item.password.$errors.length > 0"
          size="small"
          autofocus
          autocomplete="off"
          input-class="w-full p-inputtext-sm"
        ></Password>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.password) }}</Message>
      </div>
      <div class="field mb-6 col-12">
        <label for="confirmPassword" class="required">{{ $t("UserResetPasswordDialog.ConfirmPassword") }}</label>
        <Password
          id="confirmPassword"
          v-model="item.confirmPassword"
          :feedback="true"
          :toggle-mask="true"
          :invalid="v.item.confirmPassword.$errors.length > 0"
          autocomplete="off"
          size="small"
          input-class="w-full p-inputtext-sm"
        ></Password>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.confirmPassword) }}</Message>
      </div>
    </form>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="grantStore.hasPermission(`Administration.Users`, `ResetPassword`)"
        type="submit"
        icon="pi pi-save"
        :label="$t('Common.Actions.SaveChanges')"
        size="small"
        :loading="isLoading"
        :disabled="v.$errors.length > 0"
        @click="onUpdate"
      ></Button>
      <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="emits('close', false)"></Button>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import UserService from "@/services/UserService";
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { computed, onMounted, reactive, ref } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, minLength, required, sameAs } from "@vuelidate/validators";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";

interface Props {
  id?: string;
  username: string;
}
const props = defineProps<Props>();
const $t = useI18n().t;

const grantStore = useProfileStore();
const crudService = new UserService();
const isLoading = ref<boolean>(false);

const notifier = useAppNotification();
const emits = defineEmits(["close"]);
const onUpdate = async () => {
  v.value.$touch();
  if (v.value.$errors.length) {
    return;
  }

  try {
    isLoading.value = true;
    await crudService.resetPassword(props.id!, item.password);
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};

const item = reactive({
  password: "",
  confirmPassword: ""
});

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    password: {
      required: helpers.withMessage($t("UserResetPasswordDialog.Validations.PasswordIsRequired"), required),
      minLength: helpers.withMessage($t("Common.Validations.MinLength", [8]), minLength(8)),
      maxLength: validator.maxLength(50)
    },
    confirmPassword: {
      sameAs: helpers.withMessage($t("UserResetPasswordDialog.Validations.ConfirmPasswordMustBeSame"), sameAs(item.password))
    }
  }
}));
const v = useVuelidate(rules, { item });

onMounted(async () => {
  isLoading.value = true;
  if (props.id) {
    await crudService.get(props.id);
  }
  isLoading.value = false;
});
</script>

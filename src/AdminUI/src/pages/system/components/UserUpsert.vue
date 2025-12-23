<template>
  <Dialog :visible="true" :style="{ width: '90vw', maxWidth: '720px', minWidth: '360px' }" :header="$t('UsersPage.Header')" :modal="true" :closable="false">
    <form v-focustrap @submit.prevent class="grid formgrid p-fluid">
      <div class="field mb-6 col-12 md:col-6 sm:col-12">
        <label for="username" class="required">{{ $t("UsersPage.Username") }}</label>
        <InputText
          id="username"
          v-model="item.username"
          :disabled="(id?.length ?? 0) > 0"
          autocomplete="off"
          size="small"
          type="text"
          autofocus
          :invalid="v.item.username.$errors.length > 0"
        />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.username) }}</Message>
      </div>
      <div class="field mb-6 col-12 md:col-6 sm:col-12">
        <label for="displayName" class="required">{{ $t("UsersPage.DisplayName") }}</label>
        <InputText id="displayName" v-model="item.displayName" autocomplete="off" size="small" type="text" :invalid="v.item.displayName.$errors.length > 0" />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.displayName) }}</Message>
      </div>
      <div class="field mb-6 col-12 md:col-6 sm:col-12">
        <label for="email" class="required">{{ $t("UsersPage.Email") }}</label>
        <InputText id="email" v-model="item.email" autocomplete="off" type="email" size="small" :invalid="v.item.email.$errors.length > 0" />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.email) }}</Message>
      </div>
      <div class="field mb-6 col-12 md:col-6 sm:col-12">
        <label for="phone" class="required">{{ $t("UsersPage.Phone") }}</label>
        <InputText id="phone" v-model="item.phone" autocomplete="off" type="text" size="small" :invalid="v.item.phone.$errors.length > 0" />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.phone) }}</Message>
      </div>
      <div v-if="!id" class="field mb-6 col-12 md:col-6 sm:col-12">
        <label for="password" class="required">{{ $t("UsersPage.Password") }}</label>
        <Password
          id="password"
          v-model="item.password"
          :feedback="false"
          :toggle-mask="true"
          :invalid="v.item.password.$errors.length > 0"
          autocomplete="off"
          size="small"
          input-class="w-full p-inputtext-sm"
        ></Password>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.password) }}</Message>
      </div>
      <div v-if="!id" class="field mb-6 col-12 md:col-6 sm:col-12">
        <label for="confirmPassword" class="required">{{ $t("UsersPage.ConfirmPassword") }}</label>
        <Password
          id="password"
          v-model="item.confirmPassword"
          :feedback="false"
          :toggle-mask="true"
          :class="{
            'p-invalid': v.item.confirmPassword.$errors.length
          }"
          autocomplete="off"
          size="small"
          input-class="w-full p-inputtext-sm"
        ></Password>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.confirmPassword) }}</Message>
      </div>
      <div v-if="id" class="field mb-6 col-12 md:col-12 sm:col-12">
        <label for="statusId" class="required">{{ $t("UsersPage.StatusId") }}</label>
        <Select v-model="item.statusId" :options="userStatusOptions" option-label="text" option-value="value" class="p-select-sm"> </Select>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.statusId) }}</Message>
      </div>
      <div class="field mb-6 col-12 md:col-12 sm:col-12">
        <label for="roleIds" class="required">{{ $t("UsersPage.RoleId") }}</label>
        <MultiSelect
          v-model="item.roleIds"
          :options="roles"
          option-label="name"
          option-value="id"
          display="chip"
          size="small"
          :show-toggle-all="false"
          :placeholder="$t('Common.Messages.SelectPlaceholder')"
          show-clear
          class="p-select-sm"
        >
        </MultiSelect>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.roleIds) }}</Message>
      </div>
    </form>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="grantStore.hasPermission(`Administration.Users`, 'Create,Update')"
        type="submit"
        icon="pi pi-save"
        :label="$t('Common.Actions.SaveChanges')"
        size="small"
        :loading="isLoading"
        :disabled="v.$errors.length > 0"
        @click="onUpsert"
      ></Button>
      <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="emits('close', false)"></Button>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import UserService from "@/services/UserService";
import RoleService from "@/services/RoleService";
import { onMounted, reactive, ref, computed } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, minLength, required, sameAs, requiredIf } from "@vuelidate/validators";
import type { UpdateUserRequestDto } from "@/contracts/Users";
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { UserStatusOptions } from "@/contracts/Enums";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";
import type { RoleItemDto } from "@/contracts/Roles";

interface Props {
  id?: string;
}
const props = defineProps<Props>();
const $t = useI18n().t;

const grantStore = useProfileStore();
const crudService = new UserService();
const roleService = new RoleService();

const isLoading = ref<boolean>(false);
const roles = ref<RoleItemDto[]>([]);

const userStatusOptions = computed(() => {
  return UserStatusOptions.map((o) => {
    return {
      text: $t(o.text),
      value: o.value
    };
  });
});

const onUpsert = async () => {
  v.value.$touch();
  if (v.value.$errors.length) {
    return;
  }

  if (props.id) {
    await onUpdate();
    return;
  }
  await onCreate();
};

const notifier = useAppNotification();
const onCreate = async () => {
  try {
    isLoading.value = true;
    await crudService.create(item);
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};
const emits = defineEmits(["close"]);
const onUpdate = async () => {
  isLoading.value = true;
  try {
    await crudService.update(props.id!, item);
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};

const item = reactive<UpdateUserRequestDto>({
  statusId: "Active"
} as UpdateUserRequestDto);

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    username: {
      required: helpers.withMessage($t("UsersPage.Validations.UsernameIsRequired"), required),
      maxLength: validator.maxLength(50)
    },
    displayName: {
      required: helpers.withMessage($t("UsersPage.Validations.DisplayNameIsRequired"), required),
      maxLength: validator.maxLength(50)
    },
    email: {
      required: helpers.withMessage($t("UsersPage.Validations.EmailIsRequired"), required),
      maxLength: validator.maxLength(255)
    },
    statusId: {
      required: helpers.withMessage($t("UsersPage.Validations.StatusIdIsRequired"), required)
    },
    password: {
      requiredIf: helpers.withMessage(
        $t("UsersPage.Validations.PasswordIsRequired"),
        requiredIf(() => {
          return !props.id;
        })
      ),
      minLength: helpers.withMessage($t("Common.Validations.MinLength", [8]), minLength(8)),
      maxLength: validator.maxLength(50)
    },
    confirmPassword: {
      sameAs: helpers.withMessage($t("UsersPage.Validations.ConfirmPasswordMustBeSame"), sameAs(item.password))
    },
    roleIds: {},
    phone: {
      required: helpers.withMessage($t("UsersPage.Validations.PhoneIsRequired"), required),
      maxLength: validator.maxLength(20)
    }
  }
}));
const v = useVuelidate(rules, { item });

onMounted(async () => {
  if (props.id) {
    isLoading.value = true;
    const apiResponse = await crudService.get(props.id);
    Object.assign(item, {
      ...apiResponse.result
    });
    isLoading.value = false;
  }
  await getRoles();
});

const getRoles = async () => {
  isLoading.value = true;
  const apiResponse = await roleService.getList({ page: 0, pageSize: 0 });
  roles.value = apiResponse.result.data;
  isLoading.value = false;
};
</script>

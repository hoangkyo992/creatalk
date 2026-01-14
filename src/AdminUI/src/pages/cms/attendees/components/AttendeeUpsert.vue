<template>
  <Dialog :visible="true" :style="{ width: '90vw', maxWidth: '480px', minWidth: '360px' }" :header="$t('AttendeesPage.Header')" :modal="true" :closable="false">
    <form v-focustrap @submit.prevent class="grid formgrid p-fluid">
      <div class="field mb-6 col-12">
        <label for="name" class="required">{{ $t("AttendeesPage.PhoneNumber") }}</label>
        <InputText
          id="name"
          v-model="item.phoneNumber"
          autocomplete="off"
          size="small"
          type="text"
          autofocus
          :invalid="v.item.phoneNumber.$errors.length > 0"
        />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.phoneNumber) }}</Message>
      </div>
    </form>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="id && grantStore.hasPermission(`Cms.Attendees`, `Update`)"
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
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { computed, onMounted, reactive, ref } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, required } from "@vuelidate/validators";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";
import AttendeeService from "@/services/AttendeeService";
import type { AttendeeItemDto } from "@/contracts/Attendees";

interface Props {
  id?: string;
}
const props = defineProps<Props>();
const $t = useI18n().t;

const grantStore = useProfileStore();
const crudService = new AttendeeService();
const isLoading = ref<boolean>(false);

const onUpsert = async () => {
  v.value.$touch();
  if (v.value.$errors.length) {
    return;
  }

  if (props.id) {
    await onUpdate();
    return;
  }
};

const notifier = useAppNotification();

const emits = defineEmits(["close"]);
const onUpdate = async () => {
  isLoading.value = true;
  try {
    await crudService.updatePhoneNumber(props.id!, item.phoneNumber);
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};

const item = reactive<AttendeeItemDto>({} as AttendeeItemDto);

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    phoneNumber: {
      required: helpers.withMessage($t("AttendeesPage.Validations.PhoneNumberIsRequired"), required),
      maxLength: validator.maxLength(50)
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
});
</script>

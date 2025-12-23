<template>
  <Dialog :visible="true" :style="{ width: '90vw', maxWidth: '480px', minWidth: '360px' }" :header="$t('AlbumsPage.Header')" :modal="true" :closable="false">
    <form v-focustrap @submit.prevent class="grid formgrid p-fluid">
      <div class="field mb-6 col-12">
        <label for="name" class="required">{{ $t("AlbumsPage.Name") }}</label>
        <InputText id="name" v-model="item.name" autocomplete="off" size="small" type="text" :invalid="v.item.name.$errors.length > 0" />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.name) }}</Message>
      </div>
      <div class="field mb-6 col-12">
        <label for="description" class="required">{{ $t("AlbumsPage.Description") }}</label>
        <Textarea
          id="description"
          v-model="item.description"
          class="w-full max-w-full min-h-4rem mt-3"
          size="small"
          rows="2"
          cols="30"
          autocomplete="off"
          :invalid="v.item.description.$errors.length > 0"
          :class="{ 'p-invalid': v.item.description.$errors.length }"
        ></Textarea>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.description) }} </Message>
      </div>
    </form>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="(!id && grantStore.hasPermission(`Cdn.Albums`, `Create`)) || (id && grantStore.hasPermission(`Cdn.Albums`, `Update`))"
        type="button"
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
import AlbumService from "@/services/AlbumService";
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { computed, onMounted, reactive, ref } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, required } from "@vuelidate/validators";
import type { UpdateAlbumRequestDto } from "@/contracts/Albums";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";

interface Props {
  id?: string;
}
const props = defineProps<Props>();
const $t = useI18n().t;

const grantStore = useProfileStore();
const crudService = new AlbumService();
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

const item = reactive<UpdateAlbumRequestDto>({} as UpdateAlbumRequestDto);

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    name: {
      required: helpers.withMessage($t("AlbumsPage.Validations.NameIsRequired"), required),
      maxLength: validator.maxLength(255)
    },
    description: {
      required: helpers.withMessage($t("AlbumsPage.Validations.DescriptionIsRequired"), required),
      maxLength: validator.maxLength(255)
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

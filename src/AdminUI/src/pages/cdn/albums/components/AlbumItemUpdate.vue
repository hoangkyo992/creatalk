<template>
  <Dialog :visible="true" :style="{ width: '90vw', maxWidth: '720px', minWidth: '360px' }" :header="file.name" :modal="true" :closable="false">
    <form v-focustrap @submit.prevent class="grid formgrid p-fluid">
      <div class="field mb-6 col-12">
        <label for="title">{{ $t("AlbumsPage.Title") }}</label>
        <InputText id="title" v-model="item.title" autocomplete="off" size="small" type="text" :invalid="v.item.title.$errors.length > 0" />
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.title) }}</Message>
      </div>
      <div class="field mb-6 col-12">
        <label for="description">{{ $t("AlbumsPage.Description") }}</label>
        <m-tiny-editor
          id="description"
          :css="[`https://fonts.googleapis.com/css2?family=Montserrat:ital,wght@0,100..900;1,100..900&display=swap`]"
          class="lg"
          body-id=""
          body-style=""
          body-class=""
          :invalid="v.item.description.$errors.length > 0"
          :class="{ 'p-invalid': v.item.description.$errors.length }"
          v-model="item.description"
        ></m-tiny-editor>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.description) }} </Message>
      </div>
    </form>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="grantStore.hasPermission(`Cdn.Albums`, `Update`)"
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
import { computed, reactive, ref } from "vue";
import useVuelidate from "@vuelidate/core";
import type { AlbumFileItemDto, UpdateAlbumFileRequestDto } from "@/contracts/Albums";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";

interface Props {
  albumId: string;
  file: AlbumFileItemDto;
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

  await onUpdate();
};

const notifier = useAppNotification();
const emits = defineEmits(["close"]);
const onUpdate = async () => {
  isLoading.value = true;
  try {
    await crudService.updateFile(props.albumId, props.file.id, {
      ...item
    });
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};

const item = reactive<UpdateAlbumFileRequestDto>({
  title: props.file.title,
  description: props.file.description,
  index: props.file.index
});

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    title: {
      maxLength: validator.maxLength(255)
    },
    description: {
      maxLength: validator.maxLength(1024)
    }
  }
}));
const v = useVuelidate(rules, { item });
</script>

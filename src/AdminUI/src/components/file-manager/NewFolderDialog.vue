<template>
  <Card style="width: 50%; max-width: 360px; min-width: 250px" v-bind="$attrs">
    <template #title>
      {{ $t("FileManager.NewFolder") }}
    </template>
    <template #content>
      <form @submit.prevent v-focustrap class="grid formgrid p-fluid">
        <div class="field col-12">
          <InputText id="name" v-model="item.name" :invalid="v.item.name.$errors.length > 0" autocomplete="off" size="small" type="text" autofocus />
          <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.name) }}</Message>
        </div>
      </form>
    </template>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <div class="flex gap-4 justify-content-end">
        <Button
          type="button"
          icon="pi pi-save"
          :label="$t('Dialog.Button.Confirm')"
          size="small"
          :loading="isLoading"
          :disabled="v.$errors.length > 0"
          @click="onSubmit"
        ></Button>
        <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="emits('close', false)"></Button>
      </div>
    </template>
  </Card>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, required } from "@vuelidate/validators";
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import useFileManagerStore from "@/stores/fileManagerStore";
import type { FileRenameRequestDto, UpdateFolderRequestDto } from "@/contracts/FileAndFolders";

const $t = useI18n().t;
const fileManagerStore = useFileManagerStore();
const isLoading = ref<boolean>(false);

const onSubmit = async (e) => {
  e.preventDefault();

  v.value.$touch();

  if (v.value.$errors.length) {
    return;
  }

  isLoading.value = true;
  try {
    await fileManagerStore.operate("", item, "CREATE_FOLDER");
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};

const notifier = useAppNotification();
const emits = defineEmits(["close"]);

const item = reactive<UpdateFolderRequestDto>({
  name: ""
} as FileRenameRequestDto);

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    name: {
      required: helpers.withMessage($t("FileManager.Validations.FolderNameIsRequired"), required),
      maxLength: validator.maxLength(50)
    }
  }
}));
const v = useVuelidate(rules, { item });

onMounted(async () => {
  Object.assign(item, {
    name: "Untitled"
  });
});
</script>

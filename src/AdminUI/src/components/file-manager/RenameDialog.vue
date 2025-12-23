<template>
  <Card style="width: 50%; max-width: 360px; min-width: 250px" v-bind="$attrs">
    <template #title>
      {{ $t("FileManager.Rename") }}
    </template>
    <template #content>
      <form @submit.prevent v-focustrap class="grid formgrid p-fluid">
        <div class="field mb-6 col-12">
          <InputGroup>
            <InputText id="name" v-model="item.name" autocomplete="off" size="small" type="text" autofocus :invalid="v.item.name.$errors.length > 0" />
            <InputGroupAddon v-if="!props.dirItem?.isDirectory">
              <span>{{ extension }}</span>
            </InputGroupAddon>
          </InputGroup>
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
import { onMounted, reactive, ref, computed } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, required } from "@vuelidate/validators";
import { useAppValidation } from "@/composables/appValidation";
import { useAppNotification } from "@/composables/appNotification";
import { useI18n } from "vue-i18n";
import useFileManagerStore from "@/stores/fileManagerStore";
import type { FileRenameRequestDto, FolderItemItemDto } from "@/contracts/FileAndFolders";

interface Props {
  dirItem: FolderItemItemDto;
}
const props = defineProps<Props>();
const $t = useI18n().t;
const fileManagerStore = useFileManagerStore();
const isLoading = ref<boolean>(false);

const onSubmit = async (e) => {
  e.preventDefault();

  v.value.$touch();

  if (v.value.$errors.length) {
    return;
  }

  if (!v.value.$dirty) {
    emits("close", false);
    return;
  }

  if (`${item.name}${extension.value}` === props.dirItem.name) {
    emits("close", false);
    return;
  }

  isLoading.value = true;
  try {
    await fileManagerStore.operate(
      props.dirItem.id,
      {
        ...item,
        name: `${item.name}${extension.value}`
      },
      props.dirItem.isDirectory ? "RENAME_FOLDER" : "RENAME_FILE"
    );
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    emits("close", true);
  } finally {
    isLoading.value = false;
  }
};

const extension = ref(".png");
const notifier = useAppNotification();
const emits = defineEmits(["close"]);

const item = reactive<FileRenameRequestDto>({
  name: ""
} as FileRenameRequestDto);

const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    name: {
      required: helpers.withMessage(
        props.dirItem?.isDirectory ? $t("FileManager.Validations.FolderNameIsRequired") : $t("FileManager.Validations.FileNameIsRequired"),
        required
      ),
      maxLength: validator.maxLength(50)
    }
  }
}));
const v = useVuelidate(rules, { item });

onMounted(async () => {
  if (props.dirItem.isDirectory) {
    extension.value = "";
    Object.assign(item, {
      name: props.dirItem.name
    });
  } else {
    let paths = props.dirItem.name.split(".").reverse();
    if (paths.length > 1) {
      extension.value = `.${paths[0]}`;
      paths = paths.slice(1);
    }
    Object.assign(item, {
      name: paths.reverse().join(".")
    });
  }
});
</script>

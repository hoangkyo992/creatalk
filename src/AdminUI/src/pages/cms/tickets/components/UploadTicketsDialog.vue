<template>
  <Dialog
    :visible="true"
    :style="{ width: '90vw', maxWidth: '960px', minWidth: '360px' }"
    :header="$t('TicketsPage.ImportTickets')"
    :modal="true"
    :closable="false"
  >
    <form v-focustrap @submit.prevent class="grid formgrid p-fluid">
      <div class="field mb-6 col-12">
        <label for="zone" class="required">{{ $t("TicketsPage.Zone") }}</label>
        <Select v-model="zoneId" :options="zones" option-label="name" option-value="id" class="p-select-sm"> </Select>
        <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.zoneId) }}</Message>
      </div>
      <div class="field mb-6 col-12">
        <FileUpload
          ref="uploader"
          :custom-upload="true"
          @select="onSelectedFiles"
          :multiple="true"
          name="files[]"
          accept="application/pdf"
          :loading="isUploading"
          :max-file-size="256000"
          severity="danger"
        >
          <template #header="{ chooseCallback, clearCallback, files }">
            <div class="flex flex-wrap justify-content-between align-items-center flex-1 gap-4">
              <div class="flex gap-2">
                <Button @click="chooseCallback()" :disabled="isUploading" icon="pi pi-images" rounded variant="outlined" severity="secondary"></Button>
                <Button
                  @click="onReset(clearCallback)"
                  icon="pi pi-times"
                  rounded
                  variant="outlined"
                  severity="danger"
                  :disabled="!files || files.length === 0 || isUploading"
                ></Button>
              </div>
              <div>{{ $t("TicketsPage.FilesSelected", [files.length]) }}</div>
            </div>
          </template>
          <template #content="{ files, removeFileCallback }">
            <div class="flex flex-column gap-8 pt-4">
              <div v-if="files.length > 0">
                <div class="flex flex-wrap gap-4">
                  <div v-for="(file, index) of files" :key="file.name + file.type + file.size" class="w-full rounded-border flex align-items-center gap-4">
                    <div class="flex-1" style="word-break: break-all">{{ index + 1 }}. {{ file.name }}</div>
                    <div v-if="findStatus(file) == 'Failed'">
                      {{ failedFiles.find((x) => x.name == file.name && x.type == file.type && x.size == file.size)?.error }}
                    </div>
                    <Badge :value="findStatus(file)" class="w-auto" :severity="findStatusSeverity(file)" />
                    <div style="width: 2.5rem">
                      <Button
                        icon="pi pi-times"
                        v-if="findStatus(file) == 'Pending'"
                        @click="removeFileCallback(index)"
                        v-tooltip.top="$t('Common.Actions.Delete')"
                        variant="outlined"
                        size="small"
                        rounded
                        severity="danger"
                      ></Button>
                      <Button
                        icon="pi pi-loading"
                        v-if="findStatus(file) == 'Uploading'"
                        variant="outlined"
                        disabled
                        size="small"
                        loading
                        rounded
                        severity="primary"
                      ></Button>
                      <Button
                        icon="pi pi-check"
                        v-if="findStatus(file) == 'Uploaded'"
                        variant="outlined"
                        disabled
                        size="small"
                        rounded
                        severity="success"
                      ></Button>
                      <Button
                        icon="pi pi-exclamation-triangle"
                        v-if="findStatus(file) == 'Failed'"
                        variant="outlined"
                        disabled
                        size="small"
                        rounded
                        severity="danger"
                      ></Button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </template>
          <template #empty>
            <div class="flex align-items-center justify-content-center flex-column p-8">
              <i class="pi pi-cloud-upload" style="font-size: 40px"></i>
              <p class="mt-6 mb-0">Drag and drop files to here to upload.</p>
            </div>
          </template>
        </FileUpload>
      </div>
    </form>
    <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    <template #footer>
      <Button
        v-if="grantStore.hasPermission(`Cms.Tickets`, `Import`)"
        type="button"
        icon="pi pi-upload"
        :label="$t('Common.Actions.Upload')"
        size="small"
        :loading="isLoading || isUploading"
        :disabled="v.$errors.length > 0 || files.length == 0"
        @click="onUpload"
      ></Button>
      <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="onClose"></Button>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import { useAppValidation } from "@/composables/appValidation";
import { computed, onMounted, ref } from "vue";
import useVuelidate from "@vuelidate/core";
import { helpers, required } from "@vuelidate/validators";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";
import FileService from "@/services/FileService";
import FolderService from "@/services/FolderService";
import { FolderStatus } from "@/contracts/Enums";
import type { FolderItemDto } from "@/contracts/FileAndFolders";
import { useEventListener } from "@vueuse/core";
import { onBeforeRouteLeave } from "vue-router";

const $t = useI18n().t;

const grantStore = useProfileStore();
const crudService = new FileService();
const folderService = new FolderService();
const isLoading = ref<boolean>(false);

const onReset = (callback) => {
  uploadedFiles.value = [];
  failedFiles.value = [];
  uploadingFiles.value = [];
  callback();
};

const onUpload = async () => {
  v.value.$touch();
  if (v.value.$errors.length) {
    return;
  }
  isUploading.value = true;
  const chunkSize = 2;
  var pendingFiles = files.value.filter((x) => findStatus(x) == "Pending");
  for (let i = 0; i < pendingFiles.length; i += chunkSize) {
    const chunk = pendingFiles.slice(i, i + chunkSize);
    chunk.forEach((file) => {
      uploadingFiles.value.push(file);
    });
    const promises = chunk.map((x) => {
      return doUploadAsync(x);
    });
    await Promise.all(promises);
  }
  isUploading.value = false;
};

const doUploadAsync = async (file: File) => {
  uploadingFiles.value.push(file);
  try {
    const apiResponse = await crudService.upload(zoneId.value, [file]);
    if (apiResponse.isSuccess) {
      uploadedFiles.value.push(file);
    } else {
      failedFiles.value.push({
        name: file.name,
        type: file.type,
        size: file.size,
        error: apiResponse.failureReason
      });
    }
  } catch (error: any) {
    failedFiles.value.push({
      name: file.name,
      type: file.type,
      size: file.size,
      error: error?.data?.failureReason ?? error?.statusText
    });
  } finally {
    uploadingFiles.value = uploadingFiles.value.filter((x) => !(x.name == file.name && x.type == file.type && x.size == file.size));
  }
};

const files = ref([]);
const failedFiles = ref<any[]>([]);
const uploadedFiles = ref<any[]>([]);
const uploadingFiles = ref<any[]>([]);

const findStatus = (file: File) => {
  if (uploadedFiles.value.find((x) => x.name == file.name && x.type == file.type && x.size == file.size)) {
    return "Uploaded";
  } else if (failedFiles.value.find((x) => x.name == file.name && x.type == file.type && x.size == file.size)) {
    return "Failed";
  } else if (uploadingFiles.value.find((x) => x.name == file.name && x.type == file.type && x.size == file.size)) {
    return "Uploading";
  }
  return "Pending";
};
const findStatusSeverity = (file: File) => {
  const status = findStatus(file);
  switch (status) {
    case "Pending":
      return "warn";
    case "Failed":
      return "danger";
    case "Uploaded":
      return "primary";
    case "Uploading":
      return "success";
  }
};

const onSelectedFiles = (event) => {
  files.value = event.files;
};

const emits = defineEmits(["close"]);

const zoneId = ref("");
const validator = useAppValidation();
const rules = computed(() => ({
  zoneId: {
    required: helpers.withMessage($t("TicketsPage.Validations.ZoneIsRequired"), required)
  }
}));
const v = useVuelidate(rules, { zoneId });

const zones = ref<FolderItemDto[]>([]);
const getZones = async () => {
  try {
    isLoading.value = true;
    const apiResponse = await folderService.getList();
    zones.value = apiResponse.result.items.filter((x) => x.statusId == FolderStatus.Active && x.parentId != undefined);
    if (zones.value.length > 0) zoneId.value = zones.value[0].id;
  } finally {
    isLoading.value = false;
  }
};

onMounted(async () => {
  await getZones();
});

const onClose = () => {
  if (isUploading.value && !window.confirm($t("TicketsPage.Confirmations.SaveChangesBeforeExit"))) {
    return false;
  }

  emits("close", false);
};

const isUploading = ref(false);
// When the user refresh/leave the current tab
useEventListener(window, "beforeunload", (event) => {
  if (isUploading.value) event.preventDefault();
});
onBeforeRouteLeave(() => {
  if (isUploading.value && !window.confirm($t("TicketsPage.Confirmations.SaveChangesBeforeExit"))) {
    return false;
  }
});
</script>

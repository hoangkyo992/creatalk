<template>
  <div class="w-full flex align-items-center justify-content-between p-6">
    <span class="inline-flex items-center gap-2">
      <span class="font-semibold text-2xl">
        {{ name }}
      </span>
    </span>
    <div class="flex align-items-center gap-6">
      <Button severity="secondary" icon="pi pi-times" text rounded @click="props.closeCallback"></Button>
    </div>
  </div>
  <div class="page-content w-full h-full">
    <div class="page-progress-bar">
      <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
    </div>
    <DataView
      size="small"
      class="w-full h-full pt-3"
      data-key="key"
      paginator
      lazy
      scrollable
      scroll-height="flex"
      scroll-direction="both"
      striped-rows
      :rows="tabler.dataSourceRequest.value.pageSize"
      :rows-per-page-options="[9999]"
      :total-records="dataSourceResult.total"
      :value="dataSourceResult.data"
      @page="tabler.onPageChanged($event, fetchData)"
    >
      <template #empty>
        <div v-if="!isLoading" class="flex justify-content-center mt-8">
          <Button
            v-if="grantStore.hasPermission(`Cdn.Albums`, `Update`)"
            :label="$t('Common.Actions.SelectFromLibrary')"
            icon="pi pi-plus"
            class="w-auto border-round-3xl"
            severity="primary"
            @click="showSidebar = true"
          ></Button>
        </div>
        <div></div>
      </template>
      <template #paginatorstart>
        <Button
          type="button"
          size="small"
          icon="pi pi-refresh"
          v-tooltip.top="$t('Common.Actions.Refresh')"
          text
          :loading="isLoading"
          @click="fetchData"
        ></Button>
        <Button
          type="button"
          size="small"
          icon="pi pi-cog"
          severity="danger"
          v-tooltip.top="$t('Common.Actions.ResetConfig')"
          text
          @click="tabler.resetState('table-languages')"
        ></Button>
      </template>
      <template #paginatorend>
        <span>
          {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
        </span>
      </template>
      <template #list="slotProps">
        <div class="flex flex-column">
          <div v-for="(item, index) in slotProps.items" :key="index">
            <div class="flex flex-row align-items-start p-4 gap-4">
              <Image class="w-10rem shadow-2 block xl:block mx-auto border-round" :src="item.url" preview image-class="w-full" :alt="item.originalName" />
              <div class="flex flex-row justify-content-between align-items-center align-items-start flex-1 gap-4">
                <div class="flex flex-column align-items-center sm:align-items-start gap-5 pt-4">
                  <div class="font-bold" style="word-break: break-all">{{ item.title || item.name }}</div>
                  <div class="text-sm" style="word-break: break-all">{{ item.description || "-" }}</div>
                  <div class="flex align-items-center gap-3">
                    <span class="flex align-items-center gap-2">
                      <i class="pi pi-user mr-2"></i>
                      <span class="font-semibold">{{ item.createdBy }} ({{ formatDate.formatDateHM(item.createdTime) }})</span>
                    </span>
                  </div>
                  <div class="mt-6 flex gap-4">
                    <Button icon="pi pi-pencil" @click="viewItem(item)" severity="primary" outlined size="small"></Button>
                    <Button icon="pi pi-trash" @click="confirmDelete(item)" severity="danger" outlined size="small"></Button>
                    <Divider layout="vertical" class="w-auto" />
                    <Button
                      type="button"
                      icon="pi pi-arrow-up"
                      size="small"
                      @click="onUpdatePosition(item, 'Up')"
                      :disabled="index == 0"
                      outlined
                      rounded
                      v-tooltip.top="$t('Common.Actions.MoveUp')"
                      :severity="index == 0 ? 'secondary' : 'primary'"
                      class="w-auto"
                    ></Button>
                    <Button
                      type="button"
                      icon="pi pi-arrow-down"
                      size="small"
                      @click="onUpdatePosition(item, 'Down')"
                      :disabled="index == dataSourceResult.data.length - 1"
                      outlined
                      rounded
                      v-tooltip.top="$t('Common.Actions.MoveDown')"
                      :severity="index == dataSourceResult.data.length - 1 ? 'secondary' : 'primary'"
                      class="w-auto"
                    ></Button>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="flex justify-content-center my-8 py-8">
            <Button
              v-if="grantStore.hasPermission(`Cdn.Albums`, `Update`)"
              :label="$t('Common.Actions.SelectFromLibrary')"
              icon="pi pi-plus"
              class="w-auto border-round-3xl"
              severity="primary"
              @click="showSidebar = true"
            ></Button>
          </div>
        </div>
      </template>
    </DataView>
    <Drawer v-model:visible="showSidebar" position="left" class="h-full" style="width: 50vw; min-width: 320px">
      <template #container="{ closeCallback }">
        <FileManager
          class="h-full"
          size="small"
          :close-callback="closeCallback"
          :show-detail-panel="true"
          @on-select="
            async (evt) => {
              if (evt && evt.items.length > 0) {
                await onAddFilesAsync(evt.items.map((x) => x.id));
              }
            }
          "
        ></FileManager>
      </template>
    </Drawer>
    <AlbumItemUpdate v-if="showUpsert" :album-id="id" :file="selectedItem" @close="onClose"></AlbumItemUpdate>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref } from "vue";
import { useAppNotification } from "@/composables/appNotification";
import AlbumService from "@/services/AlbumService";
import { useAppDataTable } from "@/composables/appDataTable";
import type { DataSourceResultDto } from "@/contracts/Common";
import { useConfirm } from "primevue/useconfirm";
import { useI18n } from "vue-i18n";
import useProfileStore from "@/stores/profileStore";
import type { AlbumFileItemDto } from "@/contracts/Albums";
import formatDate from "@/plugins/dates/formatDate";

interface Props {
  id: string;
  name: string;
  closeCallback: () => void;
}

const props = defineProps<Props>();
const grantStore = useProfileStore();
const $t = useI18n().t;

const isLoading = ref(false);
const tabler = useAppDataTable(1, 9999);
const dataSourceResult = ref<DataSourceResultDto<AlbumFileItemDto>>({ total: 0, data: [] });
const crudService = new AlbumService();
const showSidebar = ref(false);

const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await crudService.getFiles(props.id, tabler.dataSourceRequest.value);
    dataSourceResult.value = apiResponse.result;
  } finally {
    isLoading.value = false;
  }
};

const onClose = async (reloadNeeded) => {
  showSidebar.value = false;
  showUpsert.value = false;
  selectedItem.value = undefined;
  if (reloadNeeded) {
    await fetchData();
  }
};

const updateIndex = (arr, fromIndex, toIndex) => {
  var element = arr[fromIndex];
  arr.splice(fromIndex, 1);
  arr.splice(toIndex, 0, element);
};

const onUpdatePosition = async (item: AlbumFileItemDto, direction: "Up" | "Down") => {
  try {
    isLoading.value = true;
    var files = [...dataSourceResult.value.data].sort((a, b) => a.index - b.index);
    const fileIds = files.map((x) => x.id);
    const index = fileIds.findIndex((c) => c == item.id);

    if (direction == "Up") {
      if (index < 0) return;
      updateIndex(fileIds, index, index - 1);
    } else {
      if (index >= fileIds.length - 1) return;
      updateIndex(fileIds, index, index + 1);
    }

    await crudService.updatePositions(props.id, fileIds);
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    await fetchData();
  } finally {
    isLoading.value = false;
  }
};

const confirm = useConfirm();
const notifier = useAppNotification();
const confirmDelete = (item) => {
  confirm.require({
    message: $t("AlbumsPage.Confirmations.RemoveFiles", [item.title]),
    header: $t("Dialog.Title.Confirm"),
    acceptLabel: $t("Dialog.Button.Accept"),
    rejectLabel: $t("Dialog.Button.Reject"),
    acceptClass: "p-button-danger p-button-sm w-auto",
    rejectClass: "p-button-outlined p-button-sm w-auto",
    icon: "pi pi-question-circle",
    accept: async () => {
      await onDelete(item.id);
      notifier.success($t("Dialog.Alert.DeleteSuccess"));
      await fetchData();
    }
  });
};

const onAddFilesAsync = async (files: string[]) => {
  try {
    isLoading.value = true;
    await crudService.addFiles(props.id, files);
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
    await fetchData();
  } finally {
    isLoading.value = false;
  }
};

const onDelete = async (id: string) => {
  try {
    isLoading.value = true;
    await crudService.removeFiles(props.id, [id]);
  } finally {
    isLoading.value = false;
  }
};

const showUpsert = ref(false);
const selectedItem = ref<AlbumFileItemDto>();

const viewItem = (item: AlbumFileItemDto) => {
  selectedItem.value = {
    ...item
  };
  showUpsert.value = true;
};

onMounted(async () => {
  try {
    await fetchData();
  } finally {
    isLoading.value = false;
  }
});
</script>

<style lang="scss" scoped>
.page-content {
  ::v-deep(.p-dataview) {
    .p-dataview-content {
      overflow: auto;
    }
  }
}
</style>

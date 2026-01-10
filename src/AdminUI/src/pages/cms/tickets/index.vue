<route lang="yaml">
meta:
  layout: default
  requiresAuth: true
  features: ["Cms.Tickets"]
</route>

<template>
  <div class="flex flex-column">
    <div class="page-toolbar mb-4">
      <Toolbar>
        <template #start>
          <AppBreadcrumb :home="breadcrumb.home" :model="breadcrumb.items" />
        </template>
        <template #end>
          <Button
            v-if="grantStore.hasPermission(`Cms.Tickets`, `Import`)"
            :label="$t('Common.Actions.Upload')"
            size="small"
            icon="pi pi-plus"
            class="mr-4 w-auto"
            severity="info"
            @click="
              () => {
                selectedItem = null;
                showUpload = true;
              }
            "
          ></Button>
        </template>
      </Toolbar>
    </div>
    <div class="page-content">
      <div class="page-progress-bar">
        <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
      </div>
      <DataTable
        v-model:filters="filters"
        class="w-full"
        size="small"
        data-key="id"
        paginator
        :rows-per-page-options="[10, 20, 50, 100]"
        :rows="tabler.dataSourceRequest.value.pageSize"
        :total-records="dataSourceResult.total"
        :value="dataSourceResult.data"
        lazy
        striped-rows
        resizable-columns
        column-resize-mode="fit"
        sort-mode="multiple"
        removable-sort
        state-storage="local"
        state-key="table-tickets"
        scrollable
        scroll-height="flex"
        scroll-direction="both"
        filter-display="row"
        @state-restore="tabler.onRestore($event)"
        @page="tabler.onPageChanged($event, fetchData)"
        @sort="tabler.onSort($event, fetchData)"
        @filter="tabler.onFilter($event, fetchData)"
      >
        <template #empty>
          <div v-if="!isLoading" class="text-center py-4">{{ $t("Common.Messages.NoData") }}</div>
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
            @click="tabler.resetState('table-tickets')"
          ></Button>
        </template>
        <template #paginatorend>
          <span>
            {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
          </span>
        </template>
        <Column field="actions" class="text-center non-resizer" style="width: 70px; max-width: 70px; min-width: 70px">
          <template #body="{ data }">
            <a
              :href="data.publicUrl"
              v-tooltip.top="$t('Common.Actions.ViewDetail')"
              target="_blank"
              class="p-button p-component p-button-icon-only p-button-primary p-button-rounded p-button-text"
            >
              <i class="pi pi-eye"></i>
            </a>
          </template>
        </Column>
        <Column field="name" :header="$t('TicketsPage.Name')" style="min-width: 250px" sortable>
          <template #filter="{ filterModel, filterCallback }">
            <InputText
              v-model="filterModel.value"
              fluid
              type="text"
              size="small"
              class="p-column-filter"
              @blur="filterCallback()"
              @keydown.enter="filterCallback()"
            />
          </template>
        </Column>
        <Column field="folderName" :header="$t('TicketsPage.Zone')" style="width: 200px" sortable>
          <template #filter="{ filterModel, filterCallback }">
            <InputText
              v-model="filterModel.value"
              fluid
              type="text"
              size="small"
              class="p-column-filter"
              @blur="filterCallback()"
              @keydown.enter="filterCallback()"
            />
          </template>
        </Column>
        <Column field="createdTime" :header="$t('Common.CreatedTime')" style="width: 150px" sortable>
          <template #body="{ data }">
            <span v-tooltip="`${formatDate.formatDate(data.createdTime)}`">
              {{ formatDate.formatDateHM(data.createdTime) }}
            </span>
          </template>
          <template #filter="{ filterModel, filterCallback }">
            <DatePicker
              v-model="filterModel.value"
              fluid
              input-class="w-full p-inputtext-sm"
              class="w-full"
              size="small"
              date-format="dd/mm/yy"
              @update:model-value="filterCallback"
              @hide="filterCallback"
            />
          </template>
        </Column>
        <Column field="createdBy" :header="$t('Common.CreatedBy')" style="width: 200px" sortable>
          <template #filter="{ filterModel, filterCallback }">
            <InputText
              v-model="filterModel.value"
              fluid
              type="text"
              size="small"
              class="p-column-filter"
              @blur="filterCallback()"
              @keydown.enter="filterCallback()"
            />
          </template>
        </Column>
      </DataTable>
    </div>
    <UploadTicketsDialog
      v-if="showUpload"
      @close="
        () => {
          showUpload = false;
        }
      "
      @uploaded="fetchData"
    ></UploadTicketsDialog>
  </div>
</template>

<script lang="ts" setup>
import { useAppDataTable } from "@/composables/appDataTable";
import type { DataSourceResultDto } from "@/contracts/Common";
import AppRoutes from "@/models/AppRoutes";
import useProfileStore from "@/stores/profileStore";
import formatDate from "@/plugins/dates/formatDate";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { FileItemDto } from "@/contracts/FileAndFolders";
import FileService from "@/services/FileService";

const $t = useI18n().t;
const breadcrumb = computed(() => {
  return {
    home: {
      icon: "pi pi-home",
      to: { name: AppRoutes.Home }
    },
    items: [
      {
        label: $t("Breadcrumb.Cms.ModuleName")
      },
      {
        label: $t("Breadcrumb.Cms.Tickets"),
        to: { name: AppRoutes.Cms_Tickets }
      }
    ]
  };
});

const filters = ref({
  name: { value: "", matchMode: "Contains" },
  folderName: { value: "", matchMode: "Contains" },
  createdBy: { value: "", matchMode: "Contains" },
  createdTime: { value: "", matchMode: "On" }
});

const grantStore = useProfileStore();
const crudService = new FileService();

const isLoading = ref(false);
const tabler = useAppDataTable();
const dataSourceResult = ref<DataSourceResultDto<FileItemDto>>({ total: 0, data: [] });
const selectedItem = ref<FileItemDto | null>(null);

const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await crudService.getList(tabler.dataSourceRequest.value);
    dataSourceResult.value = apiResponse.result;
  } finally {
    isLoading.value = false;
  }
};

const showUpload = ref(false);

onMounted(async () => {
  await fetchData();
});
</script>

<style scoped lang="scss"></style>

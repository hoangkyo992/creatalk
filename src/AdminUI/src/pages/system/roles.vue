<route lang="yaml">
meta:
  layout: default
  requiresAuth: true
  features: ["Administration.Roles"]
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
            v-if="grantStore.hasPermission(`Administration.Roles`, `Create`)"
            :label="$t('Common.Actions.CreateNew')"
            size="small"
            icon="pi pi-plus"
            class="mr-4 w-auto"
            severity="info"
            @click="showUpsert = true"
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
        state-key="table-roles"
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
            @click="tabler.resetState('table-roles')"
          ></Button>
        </template>
        <template #paginatorend>
          <span>
            {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
          </span>
        </template>
        <Column field="actions" class="text-center non-resizer" style="width: 100px; max-width: 100px; min-width: 100px">
          <template #body="{ data }">
            <Button
              type="button"
              rounded
              icon="pi pi-pencil"
              severity="primary"
              v-tooltip.top="$t('Common.Actions.ViewDetail')"
              text
              @click="viewItem(data)"
            ></Button>
            <Button
              v-if="grantStore.hasPermission(`Administration.Roles`, 'GrantAccess')"
              type="button"
              rounded
              icon="pi pi-cog"
              severity="warn"
              v-tooltip.top="$t('RolesPage.Actions.UpdatePermissions')"
              text
              @click="viewPermissions(data)"
            ></Button>
            <Button
              v-if="grantStore.hasPermission(`Administration.Roles`, `Delete`)"
              type="button"
              rounded
              icon="pi pi-trash"
              severity="danger"
              v-tooltip.top="$t('Common.Actions.Delete')"
              text
              @click="confirmDelete(data.id, data.name)"
            ></Button>
          </template>
        </Column>
        <Column field="name" :header="$t('RolesPage.Name')" style="width: 250px" sortable>
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
        <Column field="description" :header="$t('RolesPage.Description')" style="min-width: 200px" sortable>
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
    <RoleUpsert v-if="showUpsert" :id="selectedItem?.id" @close="onClose"></RoleUpsert>
    <RolePermissions v-if="showRoleAccesses" :id="selectedItem?.id" :name="selectedItem?.name" @close="onClose"></RolePermissions>
  </div>
</template>

<script lang="ts" setup>
import type { RoleItemDto } from "@/contracts/Roles";
import type { DataSourceResultDto } from "@/contracts/Common";
import { computed, onMounted, ref } from "vue";
import { useConfirm } from "primevue/useconfirm";
import RoleService from "@/services/RoleService";
import formatDate from "@/plugins/dates/formatDate";
import { useAppDataTable } from "@/composables/appDataTable";
import { useAppNotification } from "@/composables/appNotification";
import useProfileStore from "@/stores/profileStore";
import AppRoutes from "@/models/AppRoutes";
import { useI18n } from "vue-i18n";

const grantStore = useProfileStore();
const crudService = new RoleService();

const filters = ref({
  name: { value: "", matchMode: "Contains" },
  description: { value: "", matchMode: "Contains" },
  createdBy: { value: "", matchMode: "Contains" },
  createdTime: { value: "", matchMode: "On" }
});

const $t = useI18n().t;
const breadcrumb = computed(() => {
  return {
    home: {
      icon: "pi pi-home",
      to: { name: AppRoutes.Home }
    },
    items: [
      {
        label: $t("Breadcrumb.Administration.ModuleName")
      },
      {
        label: $t("Breadcrumb.Administration.Roles"),
        to: { name: AppRoutes.System_Roles }
      }
    ]
  };
});

const isLoading = ref(false);
const tabler = useAppDataTable();
const dataSourceResult = ref<DataSourceResultDto<RoleItemDto>>({ total: 0, data: [] });
const selectedItem = ref<RoleItemDto | null>(null);

const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await crudService.getList(tabler.dataSourceRequest.value);
    dataSourceResult.value = apiResponse.result;
  } finally {
    isLoading.value = false;
  }
};

const showUpsert = ref(false);
const onClose = async (reloadNeeded) => {
  showUpsert.value = false;
  showRoleAccesses.value = false;
  selectedItem.value = null;
  if (reloadNeeded) {
    await fetchData();
  }
};

const showRoleAccesses = ref(false);
const viewPermissions = (item: RoleItemDto) => {
  showRoleAccesses.value = true;
  selectedItem.value = item;
};

const confirm = useConfirm();
const notifier = useAppNotification();
const confirmDelete = (id: string, name: string) => {
  confirm.require({
    message: $t("RolesPage.Confirmations.Delete", [name]),
    header: $t("Dialog.Title.Confirm"),
    acceptLabel: $t("Dialog.Button.Accept"),
    rejectLabel: $t("Dialog.Button.Reject"),
    acceptClass: "p-button-danger p-button-sm w-auto",
    rejectClass: "p-button-outlined p-button-sm w-auto",
    icon: "pi pi-question-circle",
    accept: async () => {
      await onDelete(id);
      notifier.success($t("Dialog.Alert.UpdateSuccess"));
      await fetchData();
    }
  });
};
const viewItem = (item: RoleItemDto) => {
  selectedItem.value = item;
  showUpsert.value = true;
};
const onDelete = async (id: string) => {
  try {
    isLoading.value = true;
    await crudService.delete(id);
  } finally {
    isLoading.value = false;
  }
};

onMounted(async () => {
  await fetchData();
});
</script>

<style scoped lang="scss"></style>

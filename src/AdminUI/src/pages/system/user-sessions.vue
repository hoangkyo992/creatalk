<route lang="yaml">
meta:
  requiresAuth: true
  features: ["Administration.UserSessions"]
</route>

<template>
  <div class="flex flex-column">
    <div class="page-toolbar mb-4">
      <Toolbar>
        <template #start>
          <AppBreadcrumb :home="breadcrumb.home" :model="breadcrumb.items" />
        </template>
        <template #end>
          <DateRangePicker
            class="mr-4"
            :start="startTime"
            :end="endTime"
            :show-time="false"
            :filter-time-type="filterTimeType"
            :filter-time-type-options="filterTimeTypeOptions"
            :range="presetRange"
            @value-update="onFilterTimeUpdated"
            @on-changed="fetchData"
          ></DateRangePicker>
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
        column-resize-mode="expand"
        sort-mode="multiple"
        removable-sort
        state-storage="local"
        state-key="table-user-sessions"
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
            @click="tabler.resetState('table-user-sessions')"
          ></Button>
        </template>
        <template #paginatorend>
          <span>
            {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
          </span>
        </template>
        <Column field="actions" class="text-center non-resizer" style="width: 70px; max-width: 70px; min-width: 70px">
          <template #body="{ data }">
            <Button
              type="button"
              rounded
              icon="pi pi-info-circle"
              severity="primary"
              v-tooltip.top="$t('Common.Actions.ViewDetail')"
              text
              @click="viewDetail(data.id)"
            ></Button>
          </template>
        </Column>
        <Column field="username" :header="$t('UserSessionsPage.Username')" style="width: 200px" sortable>
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
        <Column field="startTime" :header="$t('UserSessionsPage.StartTime')" style="width: 150px" sortable>
          <template #body="{ data }">
            <span v-tooltip="`${formatDate.formatDate(data.startTime)}`">
              {{ formatDate.formatDateHM(data.startTime) }}
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
        <Column field="endTime" :header="$t('UserSessionsPage.EndTime')" style="width: 150px" sortable>
          <template #body="{ data }">
            <span v-tooltip="`${formatDate.formatDate(data.endTime)}`">
              {{ formatDate.formatDateHM(data.endTime) }}
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
        <Column field="ipAddress" :header="$t('UserSessionsPage.IpAddress')" style="width: 120px" sortable>
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
        <Column field="navigator" :header="$t('UserSessionsPage.Navigator')" style="min-width: 250px" sortable>
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
        <Column field="platform" hidden :header="$t('UserSessionsPage.Platform')" style="width: 120px" sortable>
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
  </div>
</template>

<script lang="ts" setup>
import type { UserSessionLogItemDto } from "@/contracts/Users";
import type { DataSourceResultDto } from "@/contracts/Common";
import { computed, onMounted, ref } from "vue";
import UserService from "@/services/UserService";
import formatDate from "@/plugins/dates/formatDate";
import { useAppDataTable } from "@/composables/appDataTable";
import localStorageUtils from "@/utils/localStorageUtils";
import AppRoutes from "@/models/AppRoutes";
import { useI18n } from "vue-i18n";

const crudService = new UserService();

const filters = ref({
  username: { value: "", matchMode: "Contains" },
  navigator: { value: "", matchMode: "Contains" },
  platform: { value: "", matchMode: "Contains" },
  ipAddress: { value: "", matchMode: "Contains" },
  endTime: { value: "", matchMode: "On" },
  startTime: { value: "", matchMode: "On" }
});

const filterTimeTypeOptions = ref(["CreatedTime"]);
const storeItem = useAppDataTable().getStoreItem("view-user-sessions", filterTimeTypeOptions.value);
const startTime = ref(storeItem.startTime);
const endTime = ref(storeItem.endTime);
const presetRange = ref(storeItem.presetRange);
const filterTimeType = ref(storeItem.filterTimeType);

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
        label: $t("Breadcrumb.Administration.UserSessions"),
        to: { name: AppRoutes.System_UserSessions }
      }
    ]
  };
});

const onFilterTimeUpdated = ($event) => {
  startTime.value = $event.start;
  endTime.value = $event.end;
  filterTimeType.value = $event.filterTimeType;
  presetRange.value = $event.presetRange;
  localStorageUtils.setItem(
    "view-user-sessions",
    JSON.stringify({
      presetRange: presetRange.value,
      filterTimeType: filterTimeType.value
    })
  );
};

const isLoading = ref(false);
const tabler = useAppDataTable();
const dataSourceResult = ref<DataSourceResultDto<UserSessionLogItemDto>>({ total: 0, data: [] });

const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await crudService.getUserSessions(tabler.dataSourceRequest.value, startTime.value, endTime.value, filterTimeType.value);
    dataSourceResult.value = apiResponse.result;
  } finally {
    isLoading.value = false;
  }
};

const viewDetail = (id: string) => {
  console.log(id);
};

onMounted(async () => {
  await fetchData();
});
</script>

<route lang="yaml">
meta:
  requiresAuth: true
  features: ["Administration.Activities"]
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
        state-key="table-activities"
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
            @click="tabler.resetState('table-activities')"
          ></Button>
        </template>
        <template #paginatorend>
          <span>
            {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
          </span>
        </template>
        <Column field="actions" class="text-center non-resizer" style="width: 3rem; max-width: 3rem; min-width: 3rem">
          <template #body="{ data }">
            <Button
              type="button"
              rounded
              icon="pi pi-info-circle"
              severity="primary"
              v-tooltip.top="$t('Common.Actions.ViewDetail')"
              text
              @click="viewDetail(data)"
            ></Button>
          </template>
        </Column>
        <Column field="time" :header="$t('LogActivitiesPage.Time')" style="width: 120px" sortable>
          <template #body="{ data }">
            <span v-tooltip="`${formatDate.formatDate(data.time)}`">
              {{ formatDate.formatDateHM(data.time) }}
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
        <Column field="username" :header="$t('LogActivitiesPage.Username')" style="width: 200px" sortable>
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
        <Column field="label" :header="$t('LogActivitiesPage.Label')" style="width: 200px" sortable>
          <template #body="{ data }">
            <Button
              type="button"
              outlined
              rounded
              size="small"
              class="w-auto p-unclickable"
              :severity="getSeverity(data)"
              :label="$t(`LogLabel.${data.label}`)"
            ></Button>
          </template>
          <template #filter="{ filterModel, filterCallback }">
            <MultiSelect
              v-model="filterModel.value"
              fluid
              :options="logLabelOptions"
              option-value="value"
              option-label="text"
              size="small"
              filter
              :filter-input-props="{
                class: 'p-inputtext-sm'
              }"
              :max-selected-labels="2"
              :selected-items-label="$t('Common.Messages.SelectedLabel', [logLabelOptions.length])"
              :placeholder="$t('Common.Messages.SelectPlaceholder')"
              class="w-full"
              style="max-width: 100%"
              @hide="filterCallback"
            />
          </template>
        </Column>
        <Column field="description" :header="$t('LogActivitiesPage.Description')" style="min-width: 300px" sortable>
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
        <Column field="requestId" :header="$t('LogActivitiesPage.RequestId')" hidden style="width: 120px" sortable>
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
    <ActivityLogDetail v-if="showDetail" :item="selectedItem" @close="showDetail = false"></ActivityLogDetail>
  </div>
</template>

<script lang="ts" setup>
import type { ActivityLogItemDto } from "@/contracts/Logs";
import type { DataSourceResultDto } from "@/contracts/Common";
import { computed, onMounted, ref } from "vue";
import formatDate from "@/plugins/dates/formatDate";
import { useAppDataTable } from "@/composables/appDataTable";
import localStorageUtils from "@/utils/localStorageUtils";
import { useI18n } from "vue-i18n";
import AppRoutes from "@/models/AppRoutes";
import { LogLabelOptions } from "@/contracts/Enums";
import LogService from "@/services/LogService";

const filterTimeTypeOptions = ["CreatedTime"];
const storeItem = useAppDataTable().getStoreItem("view-activities", filterTimeTypeOptions);
const startTime = ref(storeItem.startTime);
const endTime = ref(storeItem.endTime);
const presetRange = ref(storeItem.presetRange);
const filterTimeType = ref(storeItem.filterTimeType);

const filters = ref({
  username: { value: "", matchMode: "Contains" },
  ipAddress: { value: "", matchMode: "Contains" },
  label: { value: "", matchMode: "In" },
  source: { value: "", matchMode: "Contains" },
  description: { value: "", matchMode: "Contains" },
  action: { value: "", matchMode: "Contains" },
  methodName: { value: "", matchMode: "Contains" },
  requestId: { value: "", matchMode: "Contains" },
  time: { value: "", matchMode: "On" }
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
        label: $t("Breadcrumb.Administration.Activities"),
        to: { name: AppRoutes.System_Activities }
      }
    ]
  };
});

const logService = new LogService();
const isLoading = ref(false);
const tabler = useAppDataTable();
const dataSourceResult = ref<DataSourceResultDto<ActivityLogItemDto>>({ total: 0, data: [] });
const selectedItem = ref<ActivityLogItemDto>();
const showDetail = ref(false);

const logLabelOptions = computed(() => {
  return LogLabelOptions.map((o) => {
    return {
      text: $t(o.text),
      value: o.text.split(".").reverse()[0]
    };
  });
});
const onFilterTimeUpdated = ($event) => {
  startTime.value = $event.start;
  endTime.value = $event.end;
  filterTimeType.value = $event.filterTimeType;
  presetRange.value = $event.presetRange;
  localStorageUtils.setItem(
    "view-activities",
    JSON.stringify({
      presetRange: presetRange.value,
      filterTimeType: filterTimeType.value
    })
  );
};
const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await logService.getActivities(tabler.dataSourceRequest.value, startTime.value, endTime.value);
    dataSourceResult.value = apiResponse.result;
  } finally {
    isLoading.value = false;
  }
};

const viewDetail = (data) => {
  showDetail.value = true;
  selectedItem.value = data;
};

const getSeverity = (data) => {
  if (data.label?.startsWith("Create") || data.label?.startsWith("Add")) return "primary";
  else if (data.label?.startsWith("Update")) return "help";
  else if (
    data.label?.startsWith("Delete") ||
    data.label?.startsWith("Remove") ||
    data.label?.startsWith("Reset") ||
    data.label?.startsWith("Archive") ||
    data.label?.startsWith("Unpublish")
  )
    return "danger";
  else if (data.label?.startsWith("Import") || data.label?.startsWith("Publish")) return "success";
  return "secondary";
};

onMounted(async () => {
  await fetchData();
});
</script>

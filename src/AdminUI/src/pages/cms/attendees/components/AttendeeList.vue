<template>
  <DataTable
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
    state-key="table-attendees"
    scrollable
    scroll-height="flex"
    scroll-direction="both"
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
        @click="tabler.resetState('table-attendees')"
      ></Button>
    </template>
    <template #header>
      <div class="bg-purple-100">
        <div class="flex flex-wrap justify-content-end gap-4 my-4">
          <IconField>
            <InputIcon>
              <i class="pi pi-search" />
            </InputIcon>
            <InputText size="small" v-model="keyword" @keydown="fetchDataHandler" :placeholder="$t('Common.Messages.SearchPlaceholder')" />
          </IconField>
          <MultiSelect
            v-model="statusIds"
            fluid
            :options="messageStatusOptions"
            option-value="value"
            option-label="text"
            size="small"
            :show-toggle-all="false"
            :max-selected-labels="2"
            :selected-items-label="$t('Common.Messages.SelectedLabel', [messageStatusOptions.length])"
            :placeholder="$t('Common.Messages.SelectPlaceholder')"
            class="w-auto"
            style="min-width: 30%"
            @hide="fetchData"
            v-if="isSent"
          />
        </div>
      </div>
    </template>
    <template #paginatorend>
      <span>
        {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
      </span>
    </template>
    <Column field="actions" class="text-center non-resizer" style="width: 96px; max-width: 96px; min-width: 96px">
      <template #body="{ data }">
        <div class="flex flex-wrap text-center justify-content-center align-items-center">
          <a
            :href="data.ticketUrl"
            v-tooltip.top="$t('AttendeesPage.Actions.ViewTicket')"
            target="_blank"
            class="p-button p-component p-button-icon-only p-button-primary p-button-rounded p-button-text"
          >
            <i class="pi pi-eye"></i>
          </a>
          <Button
            type="button"
            rounded
            icon="pi pi-history"
            severity="help"
            v-tooltip.top="$t('AttendeesPage.Actions.ViewLogs')"
            text
            v-if="isSent && providerCode"
            @click="viewItem(data)"
          ></Button>
          <Button
            type="button"
            rounded
            icon="pi pi-ban"
            severity="danger"
            v-tooltip.top="$t('AttendeesPage.Actions.Cancel')"
            text
            v-if="statusId != AttendeeStatus.Cancelled && isSent == false && providerCode"
            @click="cancelItem(data, true)"
          ></Button>
          <Button
            type="button"
            rounded
            icon="pi pi-undo"
            severity="warn"
            v-tooltip.top="$t('AttendeesPage.Actions.Restore')"
            text
            v-if="statusId == AttendeeStatus.Cancelled"
            @click="cancelItem(data, false)"
          ></Button>
          <Button type="button" rounded icon="pi pi-pencil" severity="warn" v-tooltip.top="$t('Common.Actions.Update')" text @click="updateItem(data)"></Button>
          <Button
            type="button"
            rounded
            icon="pi pi-send"
            severity="success"
            v-tooltip.top="$t('AttendeesPage.Actions.Resend')"
            text
            v-if="statusId == AttendeeStatus.Default && isSent && providerCode"
            @click="resentMessage(data)"
          ></Button>
        </div>
      </template>
    </Column>
    <Column field="fullName" :header="$t('AttendeesPage.FullName')" style="min-width: 250px" sortable>
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
    <Column field="phoneNumber" :header="$t('AttendeesPage.PhoneNumber')" style="width: 200px" sortable>
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
    <Column field="ticketNumber" :header="$t('AttendeesPage.TicketNumber')" style="min-width: 250px" sortable>
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
    <Column field="ticketZone" :header="$t('AttendeesPage.TicketZone')" style="min-width: 100px" sortable>
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
    <Column :header="$t('AttendeesPage.MessageStatus')" style="width: auto" class="text-center">
      <template #body="{ data }">
        <Button
          v-if="findMessage(data)"
          type="button"
          rounded
          size="small"
          style="min-width: 120px"
          class="w-auto p-unclickable"
          :severity="componentUtils.getMessageStatusSeverity(findMessage(data)!.statusId)"
          :label="$t(`MessageStatus.${findMessage(data)!.statusId}`)"
        ></Button>
        <Button
          v-else
          type="button"
          style="min-width: 120px"
          rounded
          size="small"
          class="w-auto p-unclickable"
          severity="secondary"
          :label="$t(`MessageStatus.${defaultStatusId}`)"
        ></Button>
      </template>
    </Column>
  </DataTable>
  <AttendeeUpsert v-if="showUpsert" :id="selectedItem?.id" @close="onClose"></AttendeeUpsert>
  <AttendeeTimelineDialog v-if="showLog && selectedItem" :item="selectedItem" :message="findMessage(selectedItem)" @close="onClose"></AttendeeTimelineDialog>
</template>

<script lang="ts" setup>
import { useAppDataTable } from "@/composables/appDataTable";
import type { DataSourceResultDto } from "@/contracts/Common";
import formatDate from "@/plugins/dates/formatDate";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { AttendeeItemDto } from "@/contracts/Attendees";
import AttendeeService from "@/services/AttendeeService";
import { AttendeeStatus, MessageStatus, MessageStatusOptions } from "@/contracts/Enums";
import { debounce } from "lodash";
import componentUtils from "@/utils/componentUtils";
import { useConfirm } from "primevue";
import { useAppNotification } from "@/composables/appNotification";

interface Props {
  providerCode?: string;
  isSent?: boolean;
  statusId: AttendeeStatus | undefined;
}

const props = defineProps<Props>();
const $t = useI18n().t;

const defaultStatusId = computed(() => {
  return props.statusId == AttendeeStatus.Cancelled ? "Cancelled" : props.isSent == false ? "Waiting" : "New";
});

const statusIds = ref<MessageStatus[]>([]);
const keyword = ref("");

const messageStatusOptions = computed(() => {
  return MessageStatusOptions.map((o) => {
    return {
      text: $t(o.text),
      value: o.value
    };
  });
});

const crudService = new AttendeeService();

const isLoading = ref(false);
const tabler = useAppDataTable();
const dataSourceResult = ref<DataSourceResultDto<AttendeeItemDto>>({ total: 0, data: [] });
const selectedItem = ref<AttendeeItemDto | null>(null);

const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await crudService.getList(
      tabler.dataSourceRequest.value,
      keyword.value,
      undefined,
      undefined,
      statusIds.value,
      props.providerCode || "",
      props.isSent ?? false,
      props.statusId
    );
    dataSourceResult.value = apiResponse.result;
  } finally {
    isLoading.value = false;
  }
};

const fetchDataHandler = debounce(fetchData, 300);

const viewItem = (item: AttendeeItemDto) => {
  showLog.value = true;
  selectedItem.value = item;
};

const findMessage = (item: AttendeeItemDto) => {
  return item.messages.find((x) => x.providerCode == props.providerCode);
};

const showLog = ref(false);
const showUpsert = ref(false);
const onClose = () => {
  selectedItem.value = null;
  showUpsert.value = false;
  showLog.value = false;
};

const updateItem = (data) => {
  showUpsert.value = true;
  selectedItem.value = data;
};

const confirm = useConfirm();
const notifier = useAppNotification();

const cancelItem = (item: AttendeeItemDto, isCancelled: boolean) => {
  const msg = isCancelled ? "CancelAttendee" : "RestoreAttendee";
  confirm.require({
    message: $t(`AttendeesPage.Confirmations.${msg}`, [item.fullName, item.email, item.phoneNumber]),
    header: $t("Dialog.Title.Confirm"),
    acceptLabel: $t("Dialog.Button.Accept"),
    rejectLabel: $t("Dialog.Button.Reject"),
    acceptClass: "p-button-danger p-button-sm w-auto",
    rejectClass: "p-button-outlined p-button-sm w-auto",
    icon: "pi pi-question-circle",
    accept: async () => {
      const res = await onCancel(item, isCancelled);
      notifier.success(
        $t(`AttendeesPage.Alert..${msg}Success`, {
          ...res.result
        })
      );
      await fetchData();
    }
  });
};

const onCancel = async (item: AttendeeItemDto, isCancelled: boolean) => {
  try {
    isLoading.value = true;
    return await crudService.cancel(item.id, isCancelled);
  } finally {
    isLoading.value = false;
  }
};

const resentMessage = (item: AttendeeItemDto) => {
  confirm.require({
    message: $t(`AttendeesPage.Confirmations.ResendMessage`, [item.phoneNumber]),
    header: $t("Dialog.Title.Confirm"),
    acceptLabel: $t("Dialog.Button.Accept"),
    rejectLabel: $t("Dialog.Button.Reject"),
    acceptClass: "p-button-danger p-button-sm w-auto",
    rejectClass: "p-button-outlined p-button-sm w-auto",
    icon: "pi pi-question-circle",
    accept: async () => {
      await onResend(item);
      notifier.success($t(`Dialog.Alert.UpdateSuccess`));
      await fetchData();
    }
  });
};

const onResend = async (item: AttendeeItemDto) => {
  try {
    isLoading.value = true;
    return await crudService.resendMessage(item.id, props.providerCode ?? "");
  } finally {
    isLoading.value = false;
  }
};

onMounted(async () => {
  await fetchData();
});
</script>

<style scoped lang="scss"></style>

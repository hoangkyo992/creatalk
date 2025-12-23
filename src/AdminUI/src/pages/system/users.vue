<route lang="yaml">
meta:
  layout: default
  requiresAuth: true
  features: ["Administration.Users"]
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
            v-if="grantStore.hasPermission(`Administration.Users`, `Create`)"
            :label="$t('Common.Actions.CreateNew')"
            size="small"
            icon="pi pi-user-plus"
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
        v-model:expanded-rows="expandedRows"
        lazy
        striped-rows
        resizable-columns
        column-resize-mode="expand"
        sort-mode="multiple"
        removable-sort
        state-storage="local"
        state-key="table-users"
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
          <div v-if="!isLoading" class="text-center">{{ $t("Common.Messages.NoData") }}</div>
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
            @click="tabler.resetState('table-users')"
          ></Button>
        </template>
        <template #paginatorend>
          <span>
            {{ $t("Common.Messages.DisplayItems", [dataSourceResult.data.length, dataSourceResult.total]) }}
          </span>
        </template>
        <Column expander class="non-resizer" style="width: 2rem; max-width: 2rem; min-width: 2rem" />
        <Column field="actions" class="text-center non-resizer" style="width: 100px; max-width: 100px; min-width: 100px">
          <template #body="{ data }">
            <Button
              type="button"
              rounded
              icon="pi pi-pencil"
              severity="primary"
              v-tooltip.top="$t('Common.Actions.ViewDetail')"
              text
              @click="viewItem(data.id)"
            ></Button>
            <Button
              v-if="grantStore.hasPermission(`Administration.Users`, `ResetPassword`)"
              type="button"
              rounded
              icon="pi pi-refresh"
              severity="warn"
              v-tooltip.top="$t('UsersPage.Actions.ResetPassword')"
              text
              @click="resetUserPassword(data)"
            ></Button>
            <Button
              v-if="grantStore.hasPermission(`Administration.Users`, 'Delete')"
              type="button"
              rounded
              icon="pi pi-trash"
              severity="danger"
              v-tooltip.top="$t('Common.Actions.Delete')"
              text
              @click="confirmDelete(data.id, data.username)"
            ></Button>
          </template>
        </Column>
        <Column field="username" :header="$t('UsersPage.Username')" style="min-width: 200px" sortable>
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
        <Column field="displayName" :header="$t('UsersPage.DisplayName')" style="width: 200px" sortable>
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
        <Column field="statusId" :header="$t('UsersPage.StatusId')" style="width: 150px" sortable>
          <template #body="{ data }">
            <Button
              type="button"
              outlined
              rounded
              size="small"
              class="w-auto p-unclickable"
              :severity="componentUtils.getUserStatusSeverity(data.statusId)"
              :label="$t(data.statusCode)"
            ></Button>
          </template>
          <template #filter="{ filterModel, filterCallback }">
            <MultiSelect
              v-model="filterModel.value"
              fluid
              :options="userStatusOptions"
              option-value="value"
              option-label="text"
              size="small"
              :show-toggle-all="false"
              :max-selected-labels="2"
              :selected-items-label="$t('Common.Messages.SelectedLabel', [userStatusOptions.length])"
              :placeholder="$t('Common.Messages.SelectPlaceholder')"
              class="w-full"
              style="max-width: 100%"
              @hide="filterCallback"
            />
          </template>
        </Column>
        <Column field="email" :header="$t('UsersPage.Email')" style="width: 250px" sortable>
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
        <Column field="createdBy" :header="$t('Common.CreatedBy')" style="width: 150px" sortable>
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
        <template #expansion="slotProps">
          <div class="flex pl-8 justify-content-left align-items-center">
            <div class="mr-4 font-bold">{{ $t("UsersPage.RoleId", slotProps.data) }}:</div>
            <div class="flex">
              <Tag v-for="role in slotProps.data.roles.sort()" :key="role" severity="help" class="m-2 p-4" :value="role" rounded></Tag>
            </div>
          </div>
        </template>
      </DataTable>
    </div>
    <UserUpsert v-if="showUpsert" :id="selectedItemId" @close="onClose"></UserUpsert>
    <UserResetPassword v-if="showUserResetPassword" :id="selectedUser?.id" :username="selectedUser?.username" @close="onClose"></UserResetPassword>
  </div>
</template>

<script lang="ts" setup>
import type { UserItemDto } from "@/contracts/Users";
import type { DataSourceResultDto } from "@/contracts/Common";
import { onMounted, ref, computed } from "vue";
import { useConfirm } from "primevue/useconfirm";
import UserService from "@/services/UserService";
import formatDate from "@/plugins/dates/formatDate";
import componentUtils from "@/utils/componentUtils";
import { useAppDataTable } from "@/composables/appDataTable";
import { useAppNotification } from "@/composables/appNotification";
import { UserStatusOptions } from "@/contracts/Enums";
import useProfileStore from "@/stores/profileStore";
import { useI18n } from "vue-i18n";
import AppRoutes from "@/models/AppRoutes";

const grantStore = useProfileStore();
const crudService = new UserService();

const filters = ref({
  username: { value: "", matchMode: "Contains" },
  email: { value: "", matchMode: "Contains" },
  displayName: { value: "", matchMode: "Contains" },
  statusId: { value: "", matchMode: "In" },
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
        label: $t("Breadcrumb.Administration.Users"),
        to: { name: AppRoutes.System_Users }
      }
    ]
  };
});

const userStatusOptions = computed(() => {
  return UserStatusOptions.map((o) => {
    return {
      text: $t(o.text),
      value: o.value
    };
  });
});

const isLoading = ref(false);
const tabler = useAppDataTable();
const dataSourceResult = ref<DataSourceResultDto<UserItemDto>>({ total: 0, data: [] });
const selectedItemId = ref("");
const expandedRows = ref();

const fetchData = async () => {
  isLoading.value = true;
  try {
    const apiResponse = await crudService.getList(tabler.dataSourceRequest.value);
    dataSourceResult.value = apiResponse.result;
    expand(true);
  } finally {
    isLoading.value = false;
  }
};

const showUpsert = ref(false);
const onClose = async (reloadNeeded) => {
  showUpsert.value = false;
  selectedItemId.value = "";
  showUserResetPassword.value = false;
  selectedUser.value = null;
  if (reloadNeeded) {
    await fetchData();
  }
};

const confirm = useConfirm();
const notifier = useAppNotification();
const confirmDelete = (id: string, username: string) => {
  confirm.require({
    message: $t("UsersPage.Confirmations.Delete", [username]),
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
const viewItem = (id: string) => {
  selectedItemId.value = id;
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

const selectedUser = ref<UserItemDto | null>();
const showUserResetPassword = ref(false);
const resetUserPassword = (user: UserItemDto) => {
  selectedUser.value = user;
  showUserResetPassword.value = true;
};

const expand = (expanded) => {
  // eslint-disable-next-line no-constant-binary-expression
  expandedRows.value = expanded ? dataSourceResult.value.data.reduce((acc, p: any) => (acc[p.id] = true) && acc, {}) : null;
};

onMounted(async () => {
  await fetchData();
});
</script>

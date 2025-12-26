<route lang="yaml">
meta:
  layout: default
  requiresAuth: true
  features: ["Cms.Attendees"]
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
            v-if="grantStore.hasPermission(`Cms.Attendees`, `CreateMessages`)"
            :label="$t('AttendeesPage.Actions.CreateMessages')"
            size="small"
            icon="pi pi-send"
            class="mr-4 w-auto"
            severity="primary"
            @click="onCreateMessages"
          ></Button>
        </template>
      </Toolbar>
    </div>
    <div class="page-content">
      <div class="page-progress-bar">
        <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
      </div>
      <AttendeeList :key="key" provider-code="ZNS" :is-sent="false" :status-id="AttendeeStatus.Default"></AttendeeList>
    </div>
  </div>
</template>

<script lang="ts" setup>
import AppRoutes from "@/models/AppRoutes";
import useProfileStore from "@/stores/profileStore";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { AttendeeStatus } from "@/contracts/Enums";
import { useConfirm } from "primevue";
import { useAppNotification } from "@/composables/appNotification";
import AttendeeService from "@/services/AttendeeService";

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
        label: $t("Breadcrumb.Cms.Attendees"),
        to: { name: AppRoutes.Cms_Attendees }
      }
    ]
  };
});

const grantStore = useProfileStore();
const isLoading = ref(false);

const confirm = useConfirm();
const notifier = useAppNotification();

const onCreateMessages = () => {
  confirm.require({
    message: $t("AttendeesPage.Confirmations.CreateMessages"),
    header: $t("Dialog.Title.Confirm"),
    acceptLabel: $t("Dialog.Button.Accept"),
    rejectLabel: $t("Dialog.Button.Reject"),
    acceptClass: "p-button-danger p-button-sm w-auto",
    rejectClass: "p-button-outlined p-button-sm w-auto",
    icon: "pi pi-question-circle",
    accept: async () => {
      const res = await createMessages();
      notifier.success(
        $t("AttendeesPage.Alert.CreateMessagesSuccess", {
          ...res.result
        })
      );
      key.value = new Date().getTime();
    }
  });
};

const crudService = new AttendeeService();

const key = ref();
const createMessages = async () => {
  try {
    isLoading.value = true;
    return await crudService.createMessages("ZNS");
  } finally {
    isLoading.value = false;
  }
};
onMounted(() => {
  key.value = new Date().getTime();
});
</script>

<style scoped lang="scss"></style>

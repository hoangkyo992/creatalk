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
            v-if="grantStore.hasPermission(`Cms.Attendees`, `Import`)"
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
      <AttendeeList provider-code="ZNS" :is-sent="true" :status-id="AttendeeStatus.Default"></AttendeeList>
    </div>
  </div>
</template>

<script lang="ts" setup>
import AppRoutes from "@/models/AppRoutes";
import useProfileStore from "@/stores/profileStore";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import type { AttendeeItemDto } from "@/contracts/Attendees";
import { AttendeeStatus } from "@/contracts/Enums";

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
const selectedItem = ref<AttendeeItemDto | null>(null);
const showUpload = ref(false);
</script>

<style scoped lang="scss"></style>

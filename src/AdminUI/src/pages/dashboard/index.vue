<route lang="yaml">
meta:
  requiresAuth: true
  features: ["Dashboard"]
</route>

<template>
  <div class="flex flex-column">
    <div class="page-toolbar mb-4">
      <Toolbar>
        <template #start>
          <AppBreadcrumb :home="breadcrumb.home" :model="breadcrumb.items" />
        </template>
        <template #end />
      </Toolbar>
    </div>
    <div class="page-content"></div>
  </div>
</template>

<script lang="ts" setup>
import AppRoutes from "@/models/AppRoutes";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const $t = useI18n().t;
const breadcrumb = computed(() => {
  return {
    home: {
      icon: "pi pi-home",
      to: { name: AppRoutes.Home }
    },
    items: [
      {
        label: $t("Breadcrumb.Dashboard.ModuleName"),
        to: { name: AppRoutes.Dashboard }
      }
    ]
  };
});
</script>

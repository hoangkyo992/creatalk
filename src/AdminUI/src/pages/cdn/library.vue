<route lang="yaml">
meta:
  layout: default
  requiresAuth: true
  features: ["Cdn.Library"]
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
    <div class="page-content">
      <FileManager class="w-full h-full" size="small" :show-detail-panel="true" />
    </div>
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
        label: $t("Breadcrumb.Cdn.ModuleName")
      },
      {
        label: $t("Breadcrumb.Cdn.Library"),
        to: { name: AppRoutes.Cdn_Library }
      }
    ]
  };
});
</script>

<style scoped lang="scss"></style>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import AppRoutes from "@/models/AppRoutes";
import profileStore from "@/stores/profileStore";

const store = profileStore();
const features = ref<any[]>([]);

onMounted(async () => {
  await store.getAccesses();
  features.value = [
    {
      label: "Breadcrumb.Dashboard.ModuleName",
      separator: false,
      features: ["Dashboard"],
      items: [
        {
          label: "Breadcrumb.Dashboard.Dashboard",
          separator: false,
          icon: "pi pi-fw pi-home",
          to: { name: AppRoutes.Dashboard },
          features: ["Dashboard"]
        }
      ]
    },
    {
      label: "Breadcrumb.Cms.ModuleName",
      features: ["Cms.Tickets", "Cms.Attendees", "Cms.MessageProviders"],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Cms.Tickets",
          features: ["Cms.Tickets"],
          separator: false,
          icon: "pi pi-fw pi-ticket",
          to: { name: AppRoutes.Cms_Tickets }
        },
        {
          label: "Breadcrumb.Cms.Attendees",
          features: ["Cms.Attendees"],
          separator: false,
          icon: "pi pi-fw pi-user",
          items: [
            {
              label: "Breadcrumb.Cms.NewAttendees",
              features: ["Cms.Attendees"],
              separator: false,
              icon: "pi pi-fw pi-sparkles",
              to: { name: AppRoutes.Cms_New_Attendees }
            },
            {
              label: "Breadcrumb.Cms.SentAttendees",
              features: ["Cms.Attendees"],
              separator: false,
              icon: "pi pi-fw pi-send",
              to: { name: AppRoutes.Cms_Attendees }
            },
            {
              label: "Breadcrumb.Cms.CancelledAttendees",
              features: ["Cms.Attendees"],
              separator: false,
              icon: "pi pi-fw pi-times-circle",
              to: { name: AppRoutes.Cms_Cancelled_Attendees }
            }
          ]
        }
        // {
        //   label: "Breadcrumb.Cms.MessageProviders",
        //   features: ["Cms.MessageProviders"],
        //   separator: false,
        //   icon: "pi pi-fw pi-hashtag",
        //   to: { name: AppRoutes.Cms_MessageProviders }
        // }
      ]
    },
    {
      label: "Breadcrumb.Cdn.ModuleName",
      features: ["Cdn.Library", "Cdn.Albums"],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Cdn.Library",
          features: ["Cdn.Library"],
          separator: false,
          icon: "pi pi-fw pi-folder-open",
          to: { name: AppRoutes.Cdn_Library }
        },
        {
          label: "Breadcrumb.Cdn.Albums",
          features: ["Cdn.Albums"],
          separator: false,
          icon: "pi pi-fw pi-images",
          to: { name: AppRoutes.Cdn_Albums }
        }
      ]
    },
    {
      label: "Breadcrumb.Settings.ModuleName",
      features: ["Administration.Settings"],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Settings.Media",
          features: ["Administration.Settings"],
          separator: false,
          icon: "pi pi-fw pi-image",
          to: { name: AppRoutes.System_Settings_Media }
        },
        {
          label: "Breadcrumb.Settings.General",
          features: ["Administration.Settings"],
          separator: false,
          icon: "pi pi-fw pi-cog",
          to: { name: AppRoutes.System_Settings }
        },
        {
          label: "Breadcrumb.Settings.SocialNetworks",
          features: ["Administration.Settings"],
          separator: false,
          icon: "pi pi-fw pi-facebook",
          to: { name: AppRoutes.System_Settings_SocialNetwork }
        }
      ]
    },
    {
      label: "Breadcrumb.Administration.ModuleName",
      features: ["Administration.Users", "Administration.Roles", "Administration.UserSessions", "Administration.Activities"],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Administration.Users",
          features: ["Administration.Users"],
          separator: false,
          icon: "pi pi-fw pi-users",
          to: { name: AppRoutes.System_Users }
        },
        {
          label: "Breadcrumb.Administration.Roles",
          features: ["Administration.Roles"],
          separator: false,
          icon: "pi pi-fw pi-user-edit",
          to: { name: AppRoutes.System_Roles }
        },
        {
          label: "Breadcrumb.Administration.UserSessions",
          features: ["Administration.UserSessions"],
          separator: false,
          icon: "pi pi-fw pi-sign-in",
          to: { name: AppRoutes.System_UserSessions }
        },
        {
          label: "Breadcrumb.Administration.Activities",
          features: ["Administration.Activities"],
          separator: false,
          icon: "pi pi-fw pi-history",
          to: { name: AppRoutes.System_Activities }
        }
      ]
    }
  ];
});
</script>

<template>
  <ScrollPanel class="w-full h-full py-6">
    <ul class="layout-menu">
      <template v-for="(item, i) in features" :key="item">
        <app-menu-item v-if="!item.separator && store.hasAccess(item.features)" :item="item" :index="i"></app-menu-item>
        <li v-if="item.separator" class="menu-separator"></li>
      </template>
    </ul>
  </ScrollPanel>
</template>

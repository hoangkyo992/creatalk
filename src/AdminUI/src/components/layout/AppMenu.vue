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
      features: [
        "Cms.Pages",
        "Cms.Projects",
        "Cms.Products",
        "Cms.News",
        "Cms.Jobs",
        "Cms.Events",
        "Cms.Factories",
        "Cms.Sections",
        "Cms.Categories",
        "Cms.Languages",
        "Cms.Tags",
        "Cms.UIComponents",
        "Cms.Menus",
        "Cms.Attributes",
        "Cms.Templates"
      ],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Cms.Pages",
          features: ["Cms.Pages"],
          separator: false,
          icon: "pi pi-fw pi-external-link",
          to: { name: AppRoutes.Cms_Pages }
        },
        {
          label: "Breadcrumb.Cms.Posts.ModuleName",
          features: ["Cms.Projects", "Cms.Products", "Cms.News", "Cms.Jobs", "Cms.Events"],
          separator: false,
          icon: "pi pi-fw pi-pen-to-square",
          items: [
            {
              label: "Breadcrumb.Cms.Posts.Events",
              features: ["Cms.Events"],
              separator: false,
              icon: "",
              to: { name: AppRoutes.Cms_Events }
            },
            {
              label: "Breadcrumb.Cms.Posts.Jobs",
              features: ["Cms.Jobs"],
              separator: false,
              icon: "",
              to: { name: AppRoutes.Cms_Jobs }
            },
            {
              label: "Breadcrumb.Cms.Posts.News",
              features: ["Cms.News"],
              separator: false,
              icon: "",
              to: { name: AppRoutes.Cms_News }
            },
            {
              label: "Breadcrumb.Cms.Posts.Products",
              features: ["Cms.Products"],
              separator: false,
              icon: "",
              to: { name: AppRoutes.Cms_Products }
            },
            {
              label: "Breadcrumb.Cms.Posts.Projects",
              features: ["Cms.Projects"],
              separator: false,
              icon: "",
              to: { name: AppRoutes.Cms_Projects }
            },
            {
              label: "Breadcrumb.Cms.Posts.Factories",
              features: ["Cms.Factories"],
              separator: false,
              icon: "",
              to: { name: AppRoutes.Cms_Factories }
            }
          ]
        },
        {
          label: "Breadcrumb.Cms.Categories",
          features: ["Cms.Categories"],
          separator: false,
          icon: "pi pi-fw pi-sparkles",
          to: { name: AppRoutes.Cms_Categories }
        },
        {
          label: "Breadcrumb.Cms.Tags",
          features: ["Cms.Tags"],
          separator: false,
          icon: "pi pi-fw pi-hashtag",
          to: { name: AppRoutes.Cms_Tags }
        },
        {
          label: "Breadcrumb.Cms.Languages",
          features: ["Cms.Languages"],
          separator: false,
          icon: "pi pi-fw pi-language",
          to: { name: AppRoutes.Cms_Languages }
        },
        {
          label: "Breadcrumb.Cms.Attributes",
          features: ["Cms.Attributes"],
          separator: false,
          icon: "pi pi-fw pi-list",
          to: { name: AppRoutes.Cms_Attributes }
        },
        {
          label: "Breadcrumb.Cms.Templates",
          features: ["Cms.Templates"],
          separator: false,
          icon: "pi pi-fw pi-expand",
          to: { name: AppRoutes.Cms_Templates }
        },
        {
          label: "Breadcrumb.Cms.UIComponents",
          features: ["Cms.UIComponents"],
          separator: false,
          icon: "pi pi-fw pi pi-compass",
          to: { name: AppRoutes.Cms_UIComponents }
        },
        {
          label: "Breadcrumb.Cms.Posts.Sections",
          features: ["Cms.Sections"],
          separator: false,
          icon: "pi pi-fw pi-th-large",
          to: { name: AppRoutes.Cms_Sections }
        },
        {
          label: "Breadcrumb.Cms.Menus",
          features: ["Cms.Menus"],
          separator: false,
          icon: "pi pi-fw pi-bars",
          to: { name: AppRoutes.Cms_Menus }
        }
      ]
    },
    {
      label: "Breadcrumb.Rms.ModuleName",
      features: ["Rms.Candidates"],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Rms.Dashboard",
          features: ["Rms.Candidates"],
          separator: false,
          icon: "pi pi-fw pi-chart-bar",
          badge: 0,
          to: { name: AppRoutes.Rms }
        },
        {
          label: "Breadcrumb.Rms.Candidates",
          features: ["Rms.Candidates"],
          separator: false,
          icon: "pi pi-fw pi-check-square",
          badge: 0,
          to: { name: AppRoutes.Rms_Candidates }
        }
      ]
    },
    {
      label: "Breadcrumb.Crm.ModuleName",
      features: ["Crm.Contacts"],
      separator: false,
      items: [
        {
          label: "Breadcrumb.Crm.Dashboard",
          features: ["Crm.Contacts"],
          separator: false,
          icon: "pi pi-fw pi-chart-pie",
          badge: 0,
          to: { name: AppRoutes.Crm }
        },
        {
          label: "Breadcrumb.Crm.Contacts",
          features: ["Crm.Contacts"],
          separator: false,
          icon: "pi pi-fw pi-address-book",
          badge: 0,
          to: { name: AppRoutes.Crm_Contacts }
        }
      ]
    },
    {
      label: "Breadcrumb.Cdn.ModuleName",
      features: ["Cdn.Library"],
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

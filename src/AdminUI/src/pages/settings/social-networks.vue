<route lang="yaml">
meta:
  layout: default
  requiresAuth: true
  features: ["Administration.Settings"]
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
      <div class="page-progress-bar">
        <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
      </div>
      <form v-focustrap @submit.prevent class="flex flex-column gap-6 p-8" style="max-width: 720px">
        <div>
          <label class="font-bold text-2xl mb-4 block">{{ $t("SocialNetworkSettings.Title") }}</label>
          <small>
            {{ $t("SocialNetworkSettings.Description") }}
          </small>
        </div>
        <div v-for="data in item.items" :key="data.name">
          <div class="flex gap-6 align-items-center mb-4">
            <label class="font-bold block">{{ data.name }}</label>
            <ToggleSwitch v-model="data.enable" />
          </div>
          <InputText v-model="data.link" autocomplete="off" :disabled="!data.enable" size="small" type="text" fluid />
        </div>
        <div>
          <Button
            v-if="grantStore.hasPermission(`Administration.Settings`, `Update`)"
            class="mt-8"
            type="submit"
            icon="pi pi-save"
            :label="$t('Common.Actions.SaveChanges')"
            size="small"
            :loading="isLoading"
            @click="onSaveChanges"
          ></Button>
        </div>
      </form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useAppNotification } from "@/composables/appNotification";
import type { SettingItemDto } from "@/contracts/Settings";
import AppRoutes from "@/models/AppRoutes";
import SettingKeys from "@/models/SettingKeys";
import type { SocialNetworkSetting } from "@/models/SettingModels";
import SettingService from "@/services/SettingService";
import useProfileStore from "@/stores/profileStore";
import { computed, onMounted, reactive, ref } from "vue";
import { useI18n } from "vue-i18n";

const grantStore = useProfileStore();
const settingService = new SettingService();

const $t = useI18n().t;
const breadcrumb = computed(() => {
  return {
    home: {
      icon: "pi pi-home",
      to: { name: AppRoutes.Home }
    },
    items: [
      {
        label: $t("Breadcrumb.Settings.ModuleName")
      },
      {
        label: $t("Breadcrumb.Settings.SocialNetworks"),
        to: { name: AppRoutes.System_Settings_SocialNetwork }
      }
    ]
  };
});

const objSetting = ref<SettingItemDto>({} as SettingItemDto);

const onSaveChanges = async (event) => {
  event.preventDefault();

  isLoading.value = true;
  try {
    await settingService.update(objSetting.value.id, {
      ...objSetting.value,
      value: JSON.stringify(item)
    });
    notifier.success($t("Dialog.Alert.UpdateSuccess"));
  } finally {
    isLoading.value = false;
  }
};

const item = reactive<SocialNetworkSetting>({
  items: []
} as SocialNetworkSetting);

const notifier = useAppNotification();
const isLoading = ref<boolean>(false);
isLoading.value = true;
onMounted(async () => {
  isLoading.value = true;
  const apiResponse = await settingService.getByKey(SettingKeys.SocialNetworks);
  objSetting.value = apiResponse.result;
  if (!objSetting.value) {
    await settingService.create({
      key: SettingKeys.SocialNetworks,
      value: JSON.stringify(item)
    });
    const apiResponse = await settingService.getByKey(SettingKeys.SocialNetworks);
    objSetting.value = apiResponse.result;
  }
  Object.assign(item, {
    ...JSON.parse(apiResponse.result.value)
  });
  if (item.items.length == 0) {
    Object.assign(item, {
      items: [
        {
          name: "Facebook"
        },
        {
          name: "Zalo"
        },
        {
          name: "Whatsapp"
        }
      ]
    });
  }
  isLoading.value = false;
});
</script>

<style scoped lang="scss">
.min-w-24rem {
  min-width: 24rem;
}
</style>

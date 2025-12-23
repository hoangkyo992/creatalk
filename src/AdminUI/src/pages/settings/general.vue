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
    <div class="page-content overflow-auto">
      <div class="page-progress-bar">
        <ProgressBar v-if="isLoading" mode="indeterminate" style="height: 4px"></ProgressBar>
      </div>
      <form v-focustrap @submit.prevent class="flex flex-column gap-6 p-8" style="max-width: 720px">
        <div>
          <label class="font-bold text-2xl mb-4 block">{{ $t("GeneralSettings.WebsiteSetting.Title") }}</label>
          <small>
            {{ $t("GeneralSettings.WebsiteSetting.Description") }}
          </small>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("GeneralSettings.WebsiteSetting.SiteTitle") }}: </label>
            <InputText
              id="siteTitle"
              v-model="item.siteTitle"
              autocomplete="off"
              size="small"
              type="text"
              fluid
              :invalid="v.item.siteTitle.$errors.length > 0"
            />
            <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.siteTitle) }}</Message>
          </div>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("GeneralSettings.WebsiteSetting.Tagline") }}: </label>
            <InputText id="tagline" v-model="item.tagline" autocomplete="off" size="small" type="text" fluid :invalid="v.item.tagline.$errors.length > 0" />
            <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.tagline) }}</Message>
          </div>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("GeneralSettings.WebsiteSetting.GA4Id") }}: </label>
            <InputText id="gA4Id" v-model="item.gA4Id" autocomplete="off" size="small" type="text" fluid :invalid="v.item.gA4Id.$errors.length > 0" />
            <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.gA4Id) }}</Message>
          </div>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("GeneralSettings.WebsiteSetting.SiteLogo") }}: </label>
            <div class="flex">
              <ImageViewer
                :url="item.siteLogo"
                style="max-width: 100%; max-height: 100%; object-fit: cover"
                width="320px"
                height="auto"
                @on-select="
                  (e) => {
                    item.siteLogo = e.url;
                  }
                "
              >
              </ImageViewer>
              <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.siteLogo) }}</Message>
            </div>
          </div>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("GeneralSettings.WebsiteSetting.SiteIcon") }}: </label>
            <div class="flex">
              <ImageViewer
                :url="item.siteIcon"
                style="max-width: 100%; max-height: 100%; object-fit: cover"
                width="320px"
                height="auto"
                @on-select="
                  (e) => {
                    item.siteIcon = e.url;
                  }
                "
              >
              </ImageViewer>
              <Message severity="error" size="small" variant="simple">{{ validator.getErrorMessage(v.item.siteIcon) }}</Message>
            </div>
          </div>
        </div>
        <div style="margin-bottom: 2rem">
          <Button
            v-if="grantStore.hasPermission(`Administration.Settings`, `Update`)"
            class="mt-8"
            type="submit"
            icon="pi pi-save"
            :label="$t('Common.Actions.SaveChanges')"
            size="small"
            :loading="isLoading"
            :disabled="v.$errors.length > 0"
            @click="onSaveChanges"
          ></Button>
        </div>
      </form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useAppNotification } from "@/composables/appNotification";
import { useAppValidation } from "@/composables/appValidation";
import type { SettingItemDto } from "@/contracts/Settings";
import AppRoutes from "@/models/AppRoutes";
import SettingKeys from "@/models/SettingKeys";
import type { GeneralSetting } from "@/models/SettingModels";
import SettingService from "@/services/SettingService";
import useProfileStore from "@/stores/profileStore";
import useVuelidate from "@vuelidate/core";
import { helpers, required } from "@vuelidate/validators";
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
        label: $t("Breadcrumb.Settings.Media"),
        to: { name: AppRoutes.System_Settings }
      }
    ]
  };
});

const objSetting = ref<SettingItemDto>({} as SettingItemDto);

const onSaveChanges = async (event) => {
  event.preventDefault();

  v.value.$touch();
  if (v.value.$errors.length) {
    return;
  }

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

const item = reactive<GeneralSetting>({
  siteTitle: "",
  siteIcon: "",
  siteLogo: "",
  tagline: "",
  gA4Id: ""
} as GeneralSetting);

const notifier = useAppNotification();
const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    siteTitle: {
      required: helpers.withMessage($t("GeneralSettings.WebsiteSetting.Validations.SiteTitleIsRequired"), required)
    },
    tagline: {
      required: helpers.withMessage($t("GeneralSettings.WebsiteSetting.Validations.TaglineIsRequired"), required)
    },
    siteLogo: {
      required: helpers.withMessage($t("GeneralSettings.WebsiteSetting.Validations.SiteLogoIsRequired"), required)
    },
    siteIcon: {
      required: helpers.withMessage($t("GeneralSettings.WebsiteSetting.Validations.SiteIconIsRequired"), required)
    },
    gA4Id: {
      required: helpers.withMessage($t("GeneralSettings.WebsiteSetting.Validations.GA4IdIsRequired"), required)
    }
  }
}));
const v = useVuelidate(rules, { item });

const isLoading = ref<boolean>(false);
isLoading.value = true;
onMounted(async () => {
  isLoading.value = true;
  const apiResponse = await settingService.getByKey(SettingKeys.General);
  objSetting.value = apiResponse.result;
  if (!objSetting.value) {
    await settingService.create({
      key: SettingKeys.SocialNetworks,
      value: JSON.stringify(item)
    });
    const apiResponse = await settingService.getByKey(SettingKeys.General);
    objSetting.value = apiResponse.result;
  }
  Object.assign(item, {
    ...JSON.parse(apiResponse.result.value)
  });
  isLoading.value = false;
});
</script>

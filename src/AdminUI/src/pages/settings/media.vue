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
      <form v-focustrap @submit.prevent class="flex flex-column gap-6 p-8">
        <div>
          <label class="font-bold text-2xl mb-4 block">{{ $t("MediaSettings.ImageSizes") }}</label>
          <small>
            {{ $t("MediaSettings.ImageSizesDescription") }}
          </small>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("MediaSettings.ThumbnailSize") }}: </label>
            <div class="mb-6 w-24rem">
              <InputGroup>
                <InputNumber
                  v-model="item.imageSetting.thumbnailImageSize.maxWidth"
                  v-keyfilter.int
                  placeholder="w"
                  size="small"
                  class="w-3rem"
                  :invalid="v.item.imageSetting.thumbnailImageSize.maxWidth.$errors.length > 0"
                />
                <div class="flex w-2rem justify-content-center align-items-center">x</div>
                <InputNumber
                  v-model="item.imageSetting.thumbnailImageSize.maxHeight"
                  v-keyfilter.int
                  placeholder="h"
                  size="small"
                  class="w-3rem"
                  :invalid="v.item.imageSetting.thumbnailImageSize.maxHeight.$errors.length > 0"
                />
                <InputGroupAddon>
                  <span>px</span>
                </InputGroupAddon>
              </InputGroup>
            </div>
            <div class="flex justify-content-start align-items-center gap-4">
              <ToggleSwitch v-model="item.imageSetting.cropExact" />
              <span>
                {{ $t("MediaSettings.ThumbnailSizeCropExact") }}
              </span>
            </div>
          </div>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("MediaSettings.MediumSize") }}: </label>
            <div class="mb-6 w-24rem">
              <InputGroup>
                <InputNumber
                  v-model="item.imageSetting.mediumImageSize.maxWidth"
                  v-keyfilter.int
                  placeholder="w"
                  size="small"
                  class="w-3rem"
                  :invalid="v.item.imageSetting.mediumImageSize.maxWidth.$errors.length > 0"
                />
                <div class="flex w-2rem justify-content-center align-items-center">x</div>
                <InputNumber
                  v-model="item.imageSetting.mediumImageSize.maxHeight"
                  v-keyfilter.int
                  placeholder="h"
                  size="small"
                  class="w-3rem"
                  :invalid="v.item.imageSetting.mediumImageSize.maxHeight.$errors.length > 0"
                />
                <InputGroupAddon>
                  <span>px</span>
                </InputGroupAddon>
              </InputGroup>
            </div>
          </div>
          <div class="mt-8">
            <label class="font-bold mb-4 block"> {{ $t("MediaSettings.LargeSize") }}: </label>
            <div class="mb-6 w-24rem">
              <InputGroup>
                <InputNumber
                  v-model="item.imageSetting.largeImageSize.maxWidth"
                  v-keyfilter.int
                  placeholder="w"
                  size="small"
                  class="w-3rem"
                  :invalid="v.item.imageSetting.largeImageSize.maxWidth.$errors.length > 0"
                />
                <div class="flex w-2rem justify-content-center align-items-center">x</div>
                <InputNumber
                  v-model="item.imageSetting.largeImageSize.maxHeight"
                  v-keyfilter.int
                  placeholder="h"
                  size="small"
                  class="w-3rem"
                  :invalid="v.item.imageSetting.largeImageSize.maxHeight.$errors.length > 0"
                />
                <InputGroupAddon>
                  <span>px</span>
                </InputGroupAddon>
              </InputGroup>
            </div>
          </div>
        </div>
        <div>
          <Message
            v-for="msg in validator.getErrorMessages(v.item)"
            :key="msg"
            severity="error"
            size="small"
            icon="pi pi-times-circle"
            class="mb-2"
            variant="simple"
          >
            {{ msg }}
          </Message>
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
import type { MediaSetting } from "@/models/SettingModels";
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
        to: { name: AppRoutes.System_Settings_Media }
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

const item = reactive<MediaSetting>({
  imageSetting: {
    largeImageSize: {},
    thumbnailImageSize: {},
    mediumImageSize: {}
  }
} as MediaSetting);

const notifier = useAppNotification();
const validator = useAppValidation();
const rules = computed(() => ({
  item: {
    imageSetting: {
      thumbnailImageSize: {
        maxWidth: {
          required: helpers.withMessage($t("MediaSettings.Validations.ThumbnailWidthIsRequired"), required)
        },
        maxHeight: {
          required: helpers.withMessage($t("MediaSettings.Validations.ThumbnailHeightIsRequired"), required)
        }
      },
      mediumImageSize: {
        maxWidth: {
          required: helpers.withMessage($t("MediaSettings.Validations.MediumWidthIsRequired"), required)
        },
        maxHeight: {
          required: helpers.withMessage($t("MediaSettings.Validations.MediumHeightIsRequired"), required)
        }
      },
      largeImageSize: {
        maxWidth: {
          required: helpers.withMessage($t("MediaSettings.Validations.LargeWidthIsRequired"), required)
        },
        maxHeight: {
          required: helpers.withMessage($t("MediaSettings.Validations.LargeHeightIsRequired"), required)
        }
      }
    }
  }
}));
const v = useVuelidate(rules, { item });

const isLoading = ref<boolean>(false);
isLoading.value = true;
onMounted(async () => {
  isLoading.value = true;
  const apiResponse = await settingService.getByKey(SettingKeys.Media);
  objSetting.value = apiResponse.result;
  if (!objSetting.value) {
    await settingService.create({
      key: SettingKeys.SocialNetworks,
      value: JSON.stringify(item)
    });
    const apiResponse = await settingService.getByKey(SettingKeys.Media);
    objSetting.value = apiResponse.result;
  }
  Object.assign(item, {
    ...JSON.parse(apiResponse.result.value)
  });
  isLoading.value = false;
});
</script>

<style scoped lang="scss"></style>

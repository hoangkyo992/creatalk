<template>
  <Select v-model="locale" :options="cultures" option-label="name" option-value="code" class="language-selection w-6rem p-select-sm">
    <template #value>
      <div class="flex align-items-center">
        <img :src="getLanguageFlag" height="17" />
      </div>
    </template>
    <template #option="slotProps">
      <div class="flex align-items-center">
        <img :alt="slotProps.option.label" :src="slotProps.option.image" class="mr-5" style="width: 18px" />
        <div>{{ slotProps.option.name }}</div>
      </div>
    </template>
  </Select>
</template>

<script lang="ts" setup>
import { computed, watch, ref } from "vue";
import { useI18n } from "vue-i18n";
import localStorageUtils from "@/utils/localStorageUtils";
import StorageKeys from "@/models/StorageKeys";
import appLocalization from "@/plugins/localization";
import { usePrimeVue } from "primevue/config";
import vnFlag from "@/assets/flags/vn.svg";
import enFlag from "@/assets/flags/us.svg";

const { locale } = useI18n();
const primevue = usePrimeVue();

const currentLanguage = localStorageUtils.getItem(StorageKeys.Language);
if (currentLanguage) {
  locale.value = currentLanguage;
  appLocalization.setPrimevueLanguage(primevue, currentLanguage);
}

watch(locale, () => {
  localStorageUtils.setItem(StorageKeys.Language.toString(), locale.value);
  appLocalization.setPrimevueLanguage(primevue, locale.value);
});

const cultures = ref([
  { name: "Tiếng Việt", code: "vi-VN", image: vnFlag },
  { name: "English", code: "en-US", image: enFlag }
]);

const getLanguageFlag = computed(() => {
  return cultures.value.find((c) => c.code === (locale.value || import.meta.env.VITE_APP_DEFAULT_LANGUAGE))?.image;
});
</script>

<style scoped lang="scss">
.p-select .p-select-label {
  padding: 5px !important;
}
</style>

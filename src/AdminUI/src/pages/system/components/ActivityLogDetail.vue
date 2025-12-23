<template>
  <Dialog
    :visible="true"
    :style="{ width: '90vw', maxWidth: '1280px', minWidth: '360px' }"
    :header="$t('ActivityLogDetailDialog.Header')"
    :modal="true"
    :closable="false"
  >
    <div>
      <Panel v-for="entity in props.item.logEntities" :key="entity.id" toggleable class="p-panel-sm">
        <template #icons> </template>
        <template #header>
          <Tag class="m-2 p-4" :value="$t(`CRUD.${entity.crud}`)" :severity="getSeverity(entity)"></Tag>
          <span class="p-panel-title" data-pc-section="title"> {{ $t(`EntityName.${entity.entityName}`) }} - {{ entity.pk }} </span>
        </template>
        <div class="grid grid-nogutter surface-border pt-2">
          <div class="col-12 md:col-6 mt-6">
            <div class="p-5 mr-3 ml-3 border-1 surface-border border-round surface-card h-100pc">
              <div class="text-900 mb-4 flex">
                <span class="flex font-medium align-items-center justify-content-center ml-4">{{ $t("ActivityLogDetailDialog.OldValue") }}</span>
              </div>
              <div class="text-700 p-4">
                <div class="mb-3">
                  <vue-json-pretty :data="JSON.parse(entity.oldValue)" />
                </div>
              </div>
            </div>
          </div>
          <div class="col-12 md:col-6 mt-6">
            <div class="p-5 mr-3 ml-3 border-1 surface-border border-round surface-card h-100pc">
              <div class="text-900 mb-4 flex">
                <span class="flex font-medium align-items-center justify-content-center ml-4">{{ $t("ActivityLogDetailDialog.NewValue") }}</span>
              </div>
              <div class="text-700 p-4">
                <div class="mb-3">
                  <vue-json-pretty :data="JSON.parse(entity.newValue)" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </Panel>
    </div>
    <template #footer>
      <Button :label="$t('Dialog.Button.Close')" size="small" icon="pi pi-times" outlined severity="danger" @click="emits('close', false)"></Button>
    </template>
  </Dialog>
</template>

<script lang="ts" setup>
import type { ActivityLogItemDto } from "@/contracts/Logs";
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";

interface Props {
  item: ActivityLogItemDto;
}
const props = defineProps<Props>();
const getSeverity = (entity) => {
  switch (entity.crud) {
    case "C":
      return "primary";
    case "D":
      return "danger";
    default:
      return "warning";
  }
};
const emits = defineEmits(["close"]);
</script>

<style scoped lang="scss">
.vjs-tree {
  font-size: 0.875rem !important;
}
</style>

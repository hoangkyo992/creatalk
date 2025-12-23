<template>
  <div class="flex gap-4 align-items-center" v-bind="$attrs">
    <div class="italic" style="word-break: break-all">{{ src ?? "..." }}</div>
    <div class="flex gap-4">
      <Button type="button" icon="pi pi-pencil" severity="primary" size="small" text v-if="!viewOnly" @click="showSidebar = true"></Button>
      <Button type="button" icon="pi pi-trash" severity="danger" size="small" text v-if="!viewOnly && src && clearable" @click="emits('onClear')"></Button>
    </div>
  </div>
  <Drawer v-model:visible="showSidebar" v-if="!viewOnly" position="left" class="h-full" style="width: 50vw; min-width: 320px">
    <template #container="{ closeCallback }">
      <FileManager
        class="h-full"
        size="small"
        :close-callback="closeCallback"
        :show-detail-panel="true"
        @on-select="
          (evt) => {
            if (evt && evt.items.length > 0) {
              const file = evt.items[0];
              src = file.url;
              emits(`onSelect`, file);
            }
            closeCallback();
          }
        "
      ></FileManager>
    </template>
  </Drawer>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";

interface Props {
  url?: string;
  viewOnly?: boolean;
  clearable?: boolean;
}
const props = defineProps<Props>();
const emits = defineEmits(["onClear", "onSelect"]);

const showSidebar = ref(false);
const src = ref(props.url);

watch(props, () => {
  src.value = props.url;
});
</script>

<template>
  <div class="image-viewer">
    <Image v-bind="$attrs" :src="src" v-if="src" preview ref="imageViewer" />
    <img src="@/assets/default-image.jpg" v-bind="$attrs" v-else />
    <div class="image-viewer-backdrop">
      <div>
        <Button type="button" icon="pi pi-eye" severity="secondary" size="small" rounded v-if="src" @click="onPreview"></Button>
        <Button type="button" icon="pi pi-pencil" severity="primary" size="small" rounded v-if="!viewOnly" @click="showSidebar = true"></Button>
        <Button type="button" icon="pi pi-trash" severity="danger" size="small" rounded v-if="!viewOnly && src && clearable" @click="emits('onClear')"></Button>
      </div>
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
import { ref, useTemplateRef, watch } from "vue";

interface Props {
  url?: string;
  viewOnly?: boolean;
  clearable?: boolean;
}
const props = defineProps<Props>();
const emits = defineEmits(["onClear", "onSelect"]);
const imageViewer = useTemplateRef("imageViewer");
const onPreview = () => {
  (imageViewer.value as any).onImageClick();
};

const showSidebar = ref(false);
const src = ref(props.url);

watch(props, () => {
  src.value = props.url;
});
</script>
<style lang="scss" scoped>
.image-viewer {
  display: flex;
  position: relative;
  align-items: center;
  justify-content: center;

  &:hover {
    .image-viewer-backdrop {
      display: flex;
    }
  }
  .image-viewer-backdrop {
    position: absolute;
    display: none;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    justify-content: center;
    align-items: center;
    background-color: #00000044;

    > div {
      display: flex;
      gap: 8px;
      border-radius: 1rem;
      padding: 0.25rem;
    }
  }
}
</style>

<template>
  <Button v-bind="$attrs" type="button" rounded icon="pi pi-ellipsis-v" severity="secondary" @click="toggle"></Button>
  <Popover ref="op">
    <div class="flex flex-column py-6 min-w-12rem">
      <slot></slot>
    </div>
  </Popover>
</template>

<script lang="ts" setup>
import { ref } from "vue";

const op = ref();
const toggle = (event) => {
  op.value.toggle(event);
};
</script>
<style scoped lang="scss">
.min-w-12rem {
  min-width: 12rem;
}
</style>

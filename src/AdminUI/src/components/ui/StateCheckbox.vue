<template>
  <Checkbox :model-value="val" :disabled="disabled" :indeterminate="val === null" :checked="val === true" binary @update:model-value="handleChanged" />
</template>

<script setup lang="ts">
import { ref, watch } from "vue";

const props = defineProps({
  modelValue: {
    type: [Boolean, null],
    required: true
  },
  disabled: {
    type: Boolean,
    required: false,
    default: false
  }
});

const emit = defineEmits(["update:model-value"]);

const val = ref(props.modelValue);

const handleChanged = () => {
  if (val.value === false) val.value = null;
  else if (val.value === null) val.value = true;
  else val.value = false;
  emit("update:model-value", val.value);
};

watch(
  () => props.modelValue,
  (value) => {
    val.value = value;
  }
);
</script>

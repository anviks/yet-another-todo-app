<template>
  <v-checkbox
    :label="label"
    :model-value="modelValue"
    @update:model-value="updateValue"
    hide-details
    :indeterminate="modelValue === null"
  />
</template>

<script setup lang="ts">
const props = defineProps<{ modelValue: boolean | null | undefined, label?: string }>();
const emit = defineEmits<{ 'update:model-value': [value: boolean | null] }>();

const updateValue = () => {
  let newValue: boolean | null;

  if (props.modelValue === true) {
    newValue = false;
  } else if (props.modelValue === false) {
    newValue = null;
  } else { // null
    newValue = true;
  }

  emit('update:model-value', newValue);
};
</script>

<style scoped></style>

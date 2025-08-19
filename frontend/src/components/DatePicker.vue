<template>
  <div class="d-flex align-center ga-4">
    <!-- Date field -->
    <v-menu
      v-model="dateMenu"
      :close-on-content-click="false"
      transition="scale-transition"
      offset-y
      max-width="300px"
    >
      <template #activator="{ props }">
        <v-text-field
          v-model="dateFormatted"
          label="Date"
          readonly
          v-bind="props"
        />
      </template>
      <v-date-picker
        v-model="date"
        @update:model-value="emitDateTime()"
      />
    </v-menu>

    <!-- Time field -->
    <v-menu
      v-model="timeMenu"
      :close-on-content-click="false"
      transition="scale-transition"
      offset-y
      max-width="300px"
    >
      <template #activator="{ props }">
        <v-text-field
          v-model="time"
          label="Time"
          readonly
          v-bind="props"
        />
      </template>
      <v-time-picker
        v-model="time"
        format="24hr"
        @update:model-value="emitDateTime()"
      />
    </v-menu>
  </div>
</template>

<script setup lang="ts">
import moment, { type Moment } from 'moment';
import { ref, computed, watch, type PropType } from 'vue';

const props = defineProps({
  modelValue: {
    type: Object as PropType<Moment | null>,
    default: null,
  },
});
const emit = defineEmits<{ 'update:modelValue': [value: Moment] }>();

const date = ref<Date | null>(null);
const time = ref<string>('');

const dateMenu = ref(false);
const timeMenu = ref(false);

const dateFormatted = computed(() =>
  date.value ? moment(date.value).format('DD.MM.YYYY') : ''
);

const emitDateTime = () => {
  if (!date.value) return;

  const selectedDate = moment(date.value);

  if (time.value) {
    const [hours, minutes] = time.value.split(':').map(Number);
    selectedDate.add(hours, 'hours');
    selectedDate.add(minutes, 'minutes');
  }

  emit('update:modelValue', selectedDate);
};

// Initialize from incoming modelValue
watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      const d = val.toDate();
      date.value = d;
      time.value = d.toTimeString().slice(0, 5);
    }
  },
  { immediate: true }
);
</script>

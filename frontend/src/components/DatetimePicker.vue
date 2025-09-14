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
          :model-value="dateFormatted"
          :label="dateLabel"
          readonly
          v-bind="props"
          clearable
          @click:clear="dateValue = null"
        />
      </template>
      <v-date-picker v-model="dateValue" />
    </v-menu>

    <!-- Time field -->
    <v-menu
      v-if="type !== 'date'"
      v-model="timeMenu"
      :close-on-content-click="false"
      transition="scale-transition"
      offset-y
      max-width="300px"
    >
      <template #activator="{ props }">
        <v-text-field
          v-model="timeValue"
          :label="label === '' ? 'Time' : `${label} (time)`"
          readonly
          v-bind="props"
          :clearable="timeValue !== '00:00'"
          :disabled="!dateValue"
        />
      </template>
      <v-time-picker
        v-model="timeValue"
        format="24hr"
      />
    </v-menu>
  </div>
</template>

<script setup lang="ts">
import moment, { type Moment } from 'moment';
import { computed, ref } from 'vue';

const {
  modelValue = null,
  label = '',
  type = 'datetime',
} = defineProps<{
  modelValue?: Moment | null;
  label?: string;
  type?: 'datetime' | 'date';
}>();

const emit = defineEmits<{ 'update:modelValue': [value: Moment | null] }>();

const dateMenu = ref(false);
const timeMenu = ref(false);

const dateLabel = computed(() => {
  if (type === 'datetime') {
    return label === '' ? 'Date' : `${label} (date)`;
  } else {
    return label;
  }
});

const dateFormatted = computed(() =>
  dateValue.value ? moment(dateValue.value).format('DD.MM.YYYY') : ''
);

const dateValue = computed({
  get: () => modelValue?.toDate() ?? null,
  set: (newDate: Date | null) => {
    if (!newDate) {
      emit('update:modelValue', null);
      return;
    }

    const m = modelValue ? modelValue.clone() : moment(newDate);
    m.set({
      year: newDate.getFullYear(),
      month: newDate.getMonth(),
      date: newDate.getDate(),
    });
    emit('update:modelValue', m);
  },
});

const timeValue = computed({
  get: () => modelValue?.format('HH:mm') ?? '',
  set: (newTime: string | null) => {
    if (!modelValue) return;
    const [h, m] = newTime?.split(':').map(Number) ?? [0, 0];
    const updated = modelValue.clone().set({ hour: h, minute: m });
    emit('update:modelValue', updated);
  },
});
</script>

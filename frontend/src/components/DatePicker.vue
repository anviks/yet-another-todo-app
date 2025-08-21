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
          label="Date"
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
      v-model="timeMenu"
      :close-on-content-click="false"
      transition="scale-transition"
      offset-y
      max-width="300px"
    >
      <template #activator="{ props }">
        <v-text-field
          v-model="timeValue"
          label="Time"
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
import { ref, computed, watch, type PropType } from 'vue';

const props = defineProps({
  modelValue: {
    type: Object as PropType<Moment | null>,
    default: null,
  },
});

const emit = defineEmits<{ 'update:modelValue': [value: Moment | null] }>();

const dateMenu = ref(false);
const timeMenu = ref(false);

const dateFormatted = computed(() =>
  dateValue.value ? moment(dateValue.value).format('DD.MM.YYYY') : ''
);

const dateValue = computed({
  get: () => props.modelValue?.toDate() ?? null,
  set: (newDate: Date | null) => {
    if (!newDate) {
      emit('update:modelValue', null);
      return;
    }

    const m = props.modelValue ? props.modelValue.clone() : moment(newDate);
    m.set({
      year: newDate.getFullYear(),
      month: newDate.getMonth(),
      date: newDate.getDate(),
    });
    emit('update:modelValue', m);
  },
});

const timeValue = computed({
  get: () => props.modelValue?.format('HH:mm') ?? '',
  set: (newTime: string | null) => {
    if (!props.modelValue) return;
    const [h, m] = newTime?.split(':').map(Number) ?? [0, 0];
    const updated = props.modelValue.clone().set({ hour: h, minute: m });
    emit('update:modelValue', updated);
  },
});
</script>

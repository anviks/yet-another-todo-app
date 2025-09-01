<template>
  <h1 class="mb-8">{{ taskId ? 'Edit' : 'Create' }} a To-Do task</h1>
  <v-form
    ref="form"
    @submit.prevent="submitTask"
  >
    <div class="d-flex flex-column ga-2 mb-4">
      <v-text-field
        label="Title"
        v-model="task.title"
        :rules="rules.title"
        counter="100"
      />

      <v-textarea
        label="Description"
        v-model="task.description"
        :rules="rules.description"
        counter="2000"
      />

      <datetime-picker
        label="Due date"
        v-model="task.dueDate"
      />

      <v-input
        v-model="marker"
        :rules="rules.location"
        label="Location"
      >
        <template #default>
          <l-map
            style="height: 500px; width: 100%"
            :zoom="zoom"
            :center="center"
            @click="addMarker"
            @ready="onMapReady"
          >
            <l-tile-layer
              :url="url"
              :attribution="attribution"
            ></l-tile-layer>
            <l-marker
              v-if="marker"
              :lat-lng="marker"
            />
          </l-map>
        </template>
      </v-input>
    </div>

    <div class="d-flex justify-end">
      <v-btn
        variant="text"
        :to="{ name: 'home' }"
        class="mr-4"
      >
        Cancel
      </v-btn>
      <v-btn
        color="info"
        type="submit"
        :loading="isLoading"
      >
        Submit
      </v-btn>
    </div>
  </v-form>
</template>

<script setup lang="ts">
import { LMap, LMarker, LTileLayer } from '@vue-leaflet/vue-leaflet';
import type { LatLng } from 'leaflet-geosearch/dist/providers/provider.js';
import type { Moment } from 'moment';
import { onMounted, ref, useTemplateRef, watch } from 'vue';
import { DatetimePicker } from '../components';
import { createTask, getTask, updateTask } from '../api/todoTasks';
import { useRouter } from 'vue-router';
import type { Map as LeafletMap } from 'leaflet';
import { useToast } from 'vue-toastification';

const form = useTemplateRef('form');

const props = defineProps({
  taskId: {
    type: Number,
    default: null,
  },
});

interface TodoTaskForm {
  title: string;
  description: string;
  dueDate: Moment | null;
  latitude: number;
  longitude: number;
}

const task = ref<TodoTaskForm>({
  title: '',
  description: '',
  dueDate: null,
  latitude: 0.0,
  longitude: 0.0,
});

const mapObject = ref<LeafletMap>();
const router = useRouter();
const taskLoaded = ref(false);
const isLoading = ref(false);

const rules = {
  title: [
    (value: string) => !!value || 'Title is required',
    (value: string) =>
      value.length <= 100 || 'Title must be 100 characters or less',
  ],
  description: [
    (value: string) =>
      value.length <= 2000 || 'Description must be 2000 characters or less',
  ],
  location: [(value: LatLng) => !!value || 'Location is required'],
};

const marker = ref<LatLng>();

const url = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
const attribution =
  '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors';

const zoom = ref(1);
const center = ref<[number, number]>([0, 0]);

const addMarker = (event: { latlng: LatLng }) => {
  marker.value = event.latlng;
};

const onMapReady = (map: LeafletMap) => {
  mapObject.value = map;

  if (!props.taskId && navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(
      (position: GeolocationPosition) => {
        focusAt(position.coords.latitude, position.coords.longitude, 13);
      }
    );
  }
};

const focusAt = (latitude: number, longitude: number, zoom: number = 15) => {
  if (!mapObject.value) return;
  mapObject.value!.setView([latitude, longitude], zoom, { animate: true });
};

const toast = useToast();

const submitTask = async () => {
  const validationResult = await form.value?.validate();
  if (!validationResult?.valid) {
    return;
  }

  if (marker.value) {
    task.value.latitude = marker.value.lat;
    task.value.longitude = marker.value.lng;
  }

  isLoading.value = true;
  try {
    if (props.taskId) {
      await updateTask(props.taskId, task.value);
      toast.success('Task updated successfully');
    } else {
      await createTask(task.value);
      toast.success('Task created successfully');
    }
  } finally {
    isLoading.value = false;
  }

  await router.push({ name: 'home' });
};

onMounted(async () => {
  if (props.taskId) {
    task.value = await getTask(props.taskId);
    const { latitude, longitude } = task.value;
    marker.value = { lat: latitude, lng: longitude };
    taskLoaded.value = true;
  }
});

watch([mapObject, taskLoaded], ([mapValue, isTaskLoaded]) => {
  if (mapValue && isTaskLoaded) {
    focusAt(task.value.latitude, task.value.longitude, 13);
  }
});
</script>

<style scoped></style>

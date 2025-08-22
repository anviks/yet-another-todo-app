<template>
  <h1 class="mb-8">Create a To-Do task</h1>
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
import { ref, useTemplateRef } from 'vue';
import { DatetimePicker } from '../components';
import { createTask } from '../api/todoTasks';
import { useRouter } from 'vue-router';

const form = useTemplateRef('form');

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

const router = useRouter();

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

const onMapReady = () => {
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(
      (position: GeolocationPosition) => {
        center.value = [position.coords.latitude, position.coords.longitude];
        zoom.value = 13;
      }
    );
  }
};

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
  await createTask(task.value);
  isLoading.value = false;

  await router.push({ name: 'home' });
};
</script>

<style scoped></style>

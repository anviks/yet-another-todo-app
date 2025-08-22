<template>
  <h1 class="mb-8">Create a To-Do task</h1>
  <v-form >
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
      <date-picker
        label="Due date"
        v-model="task.dueDate"
      />
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
    </div>

    <div class="d-flex justify-end">
      <v-btn color="info">Submit</v-btn>
    </div>
  </v-form>
</template>

<script setup lang="ts">
import { LMap, LMarker, LTileLayer } from '@vue-leaflet/vue-leaflet';
import type { LatLng } from 'leaflet-geosearch/dist/providers/provider.js';
import type { Moment } from 'moment';
import { ref } from 'vue';
import { DatePicker } from '../components';

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
};

const marker = ref<LatLng>();

const url = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
const attribution =
  '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors';

const zoom = ref(1);
const center = ref([0, 0]);

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
</script>

<style scoped></style>

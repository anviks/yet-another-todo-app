<template>
  <v-form>
    <v-text-field
      label="Title"
      v-model="task.title"
    />
    <v-text-field
      label="Description"
      v-model="task.description"
    />
    
    <date-picker v-model="task.dueDate"></date-picker>

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
  </v-form>
</template>

<script setup lang="ts">
import { LMap, LMarker, LTileLayer } from '@vue-leaflet/vue-leaflet';
import type { LatLng } from 'leaflet-geosearch/dist/providers/provider.js';
import type { Moment } from 'moment';
import moment from 'moment';
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
  dueDate: moment(),
  latitude: 0.0,
  longitude: 0.0,
});

const marker = ref<LatLng>();

const url = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
const attribution = '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'

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

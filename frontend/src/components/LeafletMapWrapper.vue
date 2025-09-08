<template>
  <l-map
    style="height: 500px; width: 100%"
    :zoom="zoom"
    :center="center"
    @ready="onMapReady"
    @click="emit('click', $event)"
  >
    <l-tile-layer
      :url="url"
      :attribution="attribution"
    />

    <slot name="markers" />
  </l-map>
</template>

<script setup lang="ts">
import { LMap, LTileLayer } from '@vue-leaflet/vue-leaflet';
import type { LatLng, Map as LeafletMap } from 'leaflet';
import { ref } from 'vue';

const emit = defineEmits<{
  ready: [map: LeafletMap];
  click: [event: { latlng: LatLng }];
}>();

const url = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
const attribution =
  '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors';

const mapObject = ref<LeafletMap>();

const onMapReady = (map: LeafletMap) => {
  mapObject.value = map;
  emit('ready', map);
};

const zoom = ref(1);
const center = ref<[number, number]>([0, 0]);

const focusAt = (latitude: number, longitude: number, zoom: number = 15) => {
  if (!mapObject.value) return;
  mapObject.value.setView([latitude, longitude], zoom, { animate: true });
};

defineExpose({ focusAt, mapObject });
</script>

<style scoped></style>

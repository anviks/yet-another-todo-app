<template>
  <v-row>
    <v-col
      cols="12"
      sm="1"
    >
      <v-btn
        icon="mdi-plus"
        :to="{ name: 'add-task' }"
      ></v-btn>
    </v-col>
    <v-col
      cols="12"
      sm="5"
    >
      <div class="d-flex flex-column ga-3">
        <div
          v-for="task in tasks"
          :key="task.id"
          class="d-flex ga-2"
        >
          <v-checkbox
            hide-details
            :model-value="!!task.completedAt"
            @update:model-value="markCompleted(task.id, $event!)"
          ></v-checkbox>
          <v-card
            class="pa-3 w-100"
            @click="focusAt(task.latitude, task.longitude)"
          >
            <div class="d-flex justify-space-between align-center">
              <div class="d-flex flex-column">
                <h3 class="ml-1">{{ task.title }}</h3>
                <div class="d-flex ga-1">
                  <v-icon>mdi-map-marker</v-icon>
                  <span>
                    {{ task.latitude.toFixed(6) }},
                    {{ task.longitude.toFixed(6) }}
                  </span>
                </div>
              </div>
              <div class="d-flex ga-1">
                <v-btn
                  variant="flat"
                  icon="mdi-pencil"
                  @click.stop
                  :to="{ name: 'edit-task', params: { taskId: task.id } }"
                ></v-btn>
                <v-btn
                  variant="flat"
                  icon="mdi-trash-can"
                  @click.stop="console.log('hi')"
                ></v-btn>
              </div>
            </div>
          </v-card>
        </div>
      </div>
    </v-col>
    <v-col
      cols="12"
      sm="6"
    >
      <l-map
        style="height: 500px; width: 100%"
        :zoom="zoom"
        :center="center"
        @ready="onMapReady"
      >
        <l-tile-layer
          :url="url"
          :attribution="attribution"
        ></l-tile-layer>
        <l-marker
          v-for="(marker, i) in markers"
          :key="i"
          :lat-lng="marker"
          @click="focusAt(marker.lat, marker.lng)"
        />
      </l-map>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { completeTask, getTasks } from '../api/todoTasks.ts';
import type { TodoTask } from '../models.ts';
import type { LatLng } from 'leaflet-geosearch/dist/providers/provider.js';
import { LMap, LMarker, LTileLayer } from '@vue-leaflet/vue-leaflet';
import type { Map as LeafletMap } from 'leaflet';
import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr';

const tasks = ref<TodoTask[]>([]);
const mapObject = ref<LeafletMap>();

const zoom = ref(1);
const center = ref<[number, number]>([0, 0]);

const url = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
const attribution =
  '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors';

const markers = computed<LatLng[]>(() =>
  tasks.value.map((task) => ({
    lat: task.latitude,
    lng: task.longitude,
  }))
);

const onMapReady = (map: LeafletMap) => {
  mapObject.value = map;
};

const focusAt = (latitude: number, longitude: number) => {
  if (!mapObject.value) return;
  mapObject.value.setView([latitude, longitude], 15, { animate: true });
};

const markCompleted = async (taskId: number, completed: boolean) => {
  await completeTask(taskId, completed);
};

const loadTasks = async () => {
  tasks.value = await getTasks();
}

const connection = ref<HubConnection>();

const connectToHub = () => {
  connection.value = new HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_BACKEND_URL}/hubs/todo`)
    .build();
  
  connection.value.on('TasksUpdated', loadTasks);

  connection.value.start();
}

onMounted(async () => {
  await loadTasks();
  connectToHub();
});
</script>

<style scoped></style>

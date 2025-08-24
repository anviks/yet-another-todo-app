<template>
  <v-row>
    <v-col
      cols="12"
      sm="1"
    >
      <v-btn
        icon="mdi-plus"
        :to="{ name: 'add-task' }"
      />
    </v-col>

    <v-col
      cols="12"
      sm="5"
    >
      <transition-group
        name="tasks-list"
        tag="div"
        class="d-flex flex-column ga-3"
        @before-leave="beforeLeave"
      >
        <div
          v-for="(task, i) in tasks"
          :key="task.id"
          class="d-flex ga-2"
        >
          <v-checkbox
            hide-details
            :model-value="!!task.completedAt"
            @update:model-value="markCompleted(task.id, $event!)"
          />
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
                  variant="text"
                  icon="mdi-pencil"
                  @click.stop
                  :to="{ name: 'edit-task', params: { taskId: task.id } }"
                />
                <v-dialog
                  max-width="500"
                  absolute
                >
                  <template #activator="{ props }">
                    <v-btn
                      v-bind="props"
                      variant="text"
                      icon="mdi-trash-can"
                      @click.stop
                    />
                  </template>
                  <template #default="{ isActive }">
                    <v-card title="Are you sure you wish do delete this task?">
                      <v-card-actions>
                        <v-btn
                          variant="text"
                          @click="isActive.value = false"
                        >
                          Cancel
                        </v-btn>
                        <v-btn
                          variant="elevated"
                          color="error"
                          @click="confirmDelete(task.id)"
                        >
                          Delete
                        </v-btn>
                      </v-card-actions>
                    </v-card>
                  </template>
                </v-dialog>
              </div>
            </div>
          </v-card>
        </div>
      </transition-group>
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
import { completeTask, deleteTask, getTasks } from '../api/todoTasks.ts';
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

const confirmDelete = async (taskId: number) => {
  await deleteTask(taskId);
};

const loadTasks = async () => {
  tasks.value = await getTasks();
};

/* This is needed to fix a bug with transition-group and flexbox.
   When an element is removed from a flexbox container, its position
   is calculated incorrectly during the leave transition.
   By setting explicit width/height/left/top styles before the leave
   transition starts, we can work around this issue.

   Source: https://stackoverflow.com/a/59650481
*/
const beforeLeave = (el: Element) => {
  const div = el as HTMLDivElement;

  const { marginLeft, marginTop, width, height } = window.getComputedStyle(div);

  div.style.left = `${div.offsetLeft - parseFloat(marginLeft)}px`;
  div.style.top = `${div.offsetTop - parseFloat(marginTop)}px`;
  div.style.width = width;
  div.style.height = height;
};

const connection = ref<HubConnection>();

const connectToHub = () => {
  connection.value = new HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_BACKEND_URL}/hubs/todo`)
    .build();

  connection.value.on('TasksUpdated', loadTasks);

  connection.value.start();
};

onMounted(async () => {
  await loadTasks();
  connectToHub();
});
</script>

<style scoped>
.tasks-list-move,
.tasks-list-enter-active,
.tasks-list-leave-active {
  transition: all 0.5s ease;
}
.tasks-list-enter-from,
.tasks-list-leave-to {
  opacity: 0;
  transform: translateX(-50px);
}
.tasks-list-leave-active {
  position: absolute;
}
</style>

<template>
  <v-row>
    <v-col
      cols="12"
      sm="1"
      class="d-flex justify-center align-center"
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
        v-if="tasks.length > 0"
        name="tasks-list"
        tag="div"
        class="d-flex flex-column ga-3"
        @before-leave="beforeLeave"
      >
        <todo-task-card
          v-for="task in tasks"
          :key="task.id"
          :task="task"
          @task-clicked="focusAt(task.latitude, task.longitude)"
        ></todo-task-card>
      </transition-group>

      <div
        v-else
        class="text-center text-h3 ma-8 mt-15 text-grey"
      >
        No tasks to show
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
          v-for="task in tasks"
          :key="task.id + '-' + task.latitude + '-' + task.longitude"
          :lat-lng="[task.latitude, task.longitude]"
          @click="focusAt(task.latitude, task.longitude)"
        />
      </l-map>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr';
import { LMap, LMarker, LTileLayer } from '@vue-leaflet/vue-leaflet';
import type { Map as LeafletMap } from 'leaflet';
import { onMounted, onUnmounted, ref } from 'vue';
import { useToast } from 'vue-toastification';
import { convertTaskDates, getTasks } from '../api/todoTasks.ts';
import { TodoTaskCard } from '../components/index.ts';
import type { TodoTask } from '../models.ts';

const tasks = ref<TodoTask[]>([]);
const mapObject = ref<LeafletMap>();

const zoom = ref(1);
const center = ref<[number, number]>([0, 0]);

const url = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';
const attribution =
  '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors';

const onMapReady = (map: LeafletMap) => {
  mapObject.value = map;
};

const focusAt = (latitude: number, longitude: number) => {
  if (!mapObject.value) return;
  mapObject.value.setView([latitude, longitude], 15, { animate: true });
};

const loadTasks = async () => {
  tasks.value = await getTasks();
};

/* 
  This is needed to fix a bug with transition-group and flexbox.
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

const toast = useToast();

const connectToHub = () => {
  connection.value = new HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_BACKEND_URL}/hubs/todo`)
    .build();

  connection.value.on('TaskAdded', (task: TodoTask) => {
    tasks.value.push(convertTaskDates(task));
    toast.info(`Task "${task.title}" was added`);
  });

  connection.value.on('TaskUpdated', (task: TodoTask) => {
    const index = tasks.value.findIndex((t) => t.id === task.id);
    if (index !== -1) {
      tasks.value[index] = convertTaskDates(task);
      toast.info(`Task "${task.title}" was updated`);
    }
  });

  connection.value.on('TaskDeleted', (taskId: number) => {
    const index = tasks.value.findIndex((t) => t.id === taskId);
    if (index !== -1) {
      toast.info(`Task "${tasks.value[index].title}" was deleted`);
      tasks.value.splice(index, 1);
    }
  });

  connection.value.on('TaskCompletionUpdated', (task: TodoTask) => {
    const index = tasks.value.findIndex((t) => t.id === task.id);
    if (index !== -1) {
      tasks.value[index] = convertTaskDates(task);
      if (task.completedAt) {
        toast.info(`Task "${task.title}" was marked as completed`);
      } else {
        toast.info(`Task "${task.title}" was marked as not completed`);
      }
    }
  });

  connection.value.start();
};

onMounted(async () => {
  await loadTasks();
  connectToHub();
});

onUnmounted(async () => {
  await connection.value?.stop();
});
</script>

<style scoped lang="scss">
.tasks-list- {
  &move,
  &enter-active,
  &leave-active {
    transition: all 0.5s ease;
  }

  &enter-from,
  &leave-to {
    opacity: 0;
    transform: translateX(-50px);
  }

  &leave-active {
    position: absolute;
  }
}
</style>

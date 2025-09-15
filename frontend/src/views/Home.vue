<template>
  <v-row class="mt-1">
    <div class="d-flex w-100 ga-8 mx-5">
      <v-btn
        icon="mdi-plus"
        :to="{ name: 'add-task' }"
      />

      <v-expansion-panels>
        <v-expansion-panel title="Filter tasks">
          <template #text>
            <v-text-field
              v-model="tasksFilter.q"
              prepend-inner-icon="mdi-magnify"
              label="Search by description"
              hide-details
            />

            <tristate-checkbox
              v-model="tasksFilter.completed"
              :label="completionCheckboxLabel"
            />

            <datetime-picker
              v-model="tasksFilter.dueDate"
              type="date"
              label="Due on"
            />
          </template>
        </v-expansion-panel>
      </v-expansion-panels>
    </div>
  </v-row>

  <v-row class="my-8">
    <v-col>
      <div
        v-if="isLoading"
        class="d-flex justify-center align-center w-100"
      >
        <v-progress-circular indeterminate />
      </div>

      <template v-else>
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
            @task-deleted="onTaskDelete(task.id)"
            @task-completion="onTaskMarkCompletion(task.id, $event)"
          />
        </transition-group>

        <div
          v-else
          class="text-center text-h3 text-grey"
        >
          No tasks to show
        </div>
      </template>
    </v-col>
  </v-row>

  <v-row
    v-if="!isLoading && tasks.length"
    justify="center"
    class="mt-8 mb-1"
  >
    <div class="d-flex justify-center align-center ga-16 w-100">
      <v-select
        v-model="tasksFilter.pageSize"
        :items="[5, 10, 20]"
        label="Tasks per page"
        hide-details
        dense
        style="max-width: 150px"
      />

      <div class="d-flex align-center">
        <v-btn
          :disabled="tasksFilter.page <= 1"
          @click="
            tasksFilter.page--;
            loadTasks();
          "
        >
          &lt;&lt;
        </v-btn>

        <span class="mx-4 text-h6"> Page {{ tasksFilter.page }} </span>

        <v-btn
          :disabled="!hasNextPage"
          @click="
            tasksFilter.page++;
            loadTasks();
          "
        >
          &gt;&gt;
        </v-btn>
      </div>
    </div>
  </v-row>
</template>

<script setup lang="ts">
import { HubConnectionBuilder, type HubConnection } from '@microsoft/signalr';
import _ from 'lodash';
import { computed, onMounted, onUnmounted, reactive, ref, watch } from 'vue';
import { useToast } from 'vue-toastification';
import {
  convertTaskDates,
  deleteTask,
  getTasks,
  markTaskCompletion,
  type TodoTaskFilter,
} from '../api/todoTasks.ts';
import {
  DatetimePicker,
  TodoTaskCard,
  TristateCheckbox,
} from '../components/index.ts';
import type { TodoTask } from '../types.ts';

const tasksFilter = reactive<TodoTaskFilter>({
  completed: null,
  dueDate: null,
  q: null,
  page: 1,
  pageSize: 10,
});

const isLoading = ref(false);
const tasks = ref<TodoTask[]>([]);
const hasNextPage = ref(false);

const loadTasks = async () => {
  isLoading.value = true;

  // await new Promise((resolve) => setTimeout(resolve, 1000000));
  const paginatedTasks = await getTasks(tasksFilter);
  tasks.value = paginatedTasks.items;
  hasNextPage.value = paginatedTasks.hasNextPage;

  isLoading.value = false;
};

const loadTasksDebounced = _.debounce(loadTasks, 300);

watch(
  () => tasksFilter.q,
  () => {
    tasksFilter.page = 1;
    loadTasksDebounced();
  }
);

watch(
  [
    () => tasksFilter.completed,
    () => tasksFilter.dueDate,
    () => tasksFilter.pageSize,
  ],
  () => {
    tasksFilter.page = 1;
    loadTasks();
  }
);

const completionCheckboxLabel = computed(() => {
  if (tasksFilter.completed === true) {
    return 'Show only completed tasks';
  } else if (tasksFilter.completed === false) {
    return 'Show only uncompleted tasks';
  } else {
    return "Don't filter by completion";
  }
});

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

  connection.value.on('TaskCreated', (task: TodoTask) => {
    tasks.value.push(convertTaskDates(task));
    toast.info(`Task "${task.title}" was created`);
  });

  connection.value.on('TaskUpdated', (task: TodoTask) => {
    const index = tasks.value.findIndex((t) => t.id === task.id);
    if (index !== -1) {
      tasks.value[index] = convertTaskDates(task);
      toast.info(`Task "${task.title}" was updated`);
    }
  });

  connection.value.on('TaskDeleted', (taskId: number, connectionId: string) => {
    const index = tasks.value.findIndex((t) => t.id === taskId);
    if (index !== -1) {
      if (connectionId === connection.value?.connectionId) {
        toast.success(
          `Successfully deleted task "${tasks.value[index].title}"`
        );
      } else {
        toast.info(`Task "${tasks.value[index].title}" was deleted`);
      }
      tasks.value.splice(index, 1);
    }
  });

  connection.value.on(
    'TaskCompletionUpdated',
    (task: TodoTask, connectionId: string) => {
      const index = tasks.value.findIndex((t) => t.id === task.id);
      if (index !== -1) {
        tasks.value[index] = convertTaskDates(task);
        const completed = !!task.completedAt;
        const completion = completed ? 'completed' : 'not completed';

        if (connectionId === connection.value?.connectionId) {
          toast.success(
            `Successfully marked task "${task.title}" as ${completion}`
          );
        } else {
          toast.info(`Task "${task.title}" was marked as ${completion}`);
        }
      }
    }
  );

  connection.value.start();
};

const onTaskDelete = async (taskId: number) => {
  await deleteTask(taskId, connection.value?.connectionId ?? undefined);
};

const onTaskMarkCompletion = async (taskId: number, completed: boolean) => {
  await markTaskCompletion(
    taskId,
    completed,
    connection.value?.connectionId ?? undefined
  );
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

<template>
  <v-row>
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
  </v-row>

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
      sm="11"
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
          @task-deleted="onTaskDelete(task.id)"
          @task-completion="onTaskMarkCompletion(task.id, $event)"
        />
      </transition-group>

      <div
        v-else
        class="text-center text-h3 ma-8 mt-15 text-grey"
      >
        No tasks to show
      </div>
    </v-col>
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
import type { TodoTask } from '../models.ts';

const tasksFilter = reactive<TodoTaskFilter>({
  completed: null,
  dueDate: null,
  q: null,
});

const tasks = ref<TodoTask[]>([]);

const loadTasks = _.debounce(async () => {
  tasks.value = await getTasks(tasksFilter);
}, 300);

watch(tasksFilter, loadTasks, { deep: true });

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

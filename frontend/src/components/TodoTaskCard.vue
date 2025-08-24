<template>
  <div class="d-flex ga-2">
    <v-checkbox
      hide-details
      :model-value="!!task.completedAt"
      @update:model-value="markCompleted(task.id, $event!)"
    />

    <v-card
      class="pa-3 w-100 task-card"
      @click="emit('task-clicked')"
      :class="{ 'task-completed': !!task.completedAt }"
    >
      <v-fade-transition>
        <div
          v-if="!!task.completedAt"
          class="completed-task-overlay"
        ></div>
      </v-fade-transition>
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
              <v-card title="Are you sure you wish to delete this task?">
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
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { TodoTask } from '../models';
import { completeTask, deleteTask } from '../api/todoTasks';

defineProps({
  task: {
    type: Object as PropType<TodoTask>,
    required: true,
  },
});

const emit = defineEmits<{ 'task-clicked': [] }>();

const markCompleted = async (taskId: number, completed: boolean) => {
  await completeTask(taskId, completed);
};

const confirmDelete = async (taskId: number) => {
  await deleteTask(taskId);
};
</script>

<style scoped lang="scss">
.task-card {
  position: relative;
  overflow: visible;

  &.task-completed {
    opacity: 0.7;

    &::after {
      content: '';
      position: absolute;
      top: 50%;
      left: -6px;
      right: -6px;
      height: 2px;
      background-color: gray;
      transform: translateY(-50%);
      pointer-events: none;
    }
  }
}

.completed-task-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: inherit;
  pointer-events: none;
  background-color: rgba(0, 0, 0, 0.3);
}
</style>

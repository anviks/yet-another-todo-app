<template>
  <div class="d-flex ga-2">
    <v-checkbox
      hide-details
      :model-value="!!task.completedAt"
      @update:model-value="markCompleted(task.id, $event!)"
    />

    <v-card
      class="pa-3 w-100"
      @click="emit('task-clicked')"
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

<style scoped></style>

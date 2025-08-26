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
      <div
        v-if="!!task.completedAt"
        class="completed-task-overlay"
      ></div>

      <div class="d-flex justify-space-between align-center">
        <div class="d-flex flex-column">
          <h3 class="ml-1">{{ task.title }}</h3>

          <div
            v-if="task.dueDate"
            class="ml-2 task-due-date"
            :class="{ 'text-red': isOverdue }"
          >
            <v-icon
              small
              class="mr-1"
            >
              mdi-calendar-clock
            </v-icon>
            {{ task.dueDate.format('DD.MM.YYYY, HH:mm') }}
          </div>

          <div
            v-if="task.description"
            class="ml-2"
          >
            <div
              ref="textBlock"
              class="task-description"
              :class="{ 'text-preview-fade': !descriptionExpanded }"
              :style="{
                maxHeight: descriptionExpanded
                  ? textHeight + 'px'
                  : previewHeight + 'px',
              }"
            >
              {{ task.description }}
            </div>

            <v-btn
              variant="text"
              size="small"
              class="mt-1 px-0"
              @click.stop="descriptionExpanded = !descriptionExpanded"
            >
              {{ descriptionExpanded ? 'See less' : 'See more' }}
            </v-btn>
          </div>

          <div class="d-flex ga-1 mt-2">
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
import { computed, onMounted, onUnmounted, ref, type PropType } from 'vue';
import type { TodoTask } from '../models';
import { completeTask, deleteTask } from '../api/todoTasks';
import moment from 'moment';

const props = defineProps({
  task: {
    type: Object as PropType<TodoTask>,
    required: true,
  },
});

const emit = defineEmits<{ 'task-clicked': [] }>();

const descriptionExpanded = ref(false);
const textBlock = ref<HTMLElement | null>(null);
const previewHeight = ref(0);
const textHeight = ref(0);

const now = ref(moment());

let currentTimeTimeout: number | undefined;
let currentTimeInterval: number | undefined;

onMounted(async () => {
  if (textBlock.value) {
    textHeight.value = textBlock.value.scrollHeight;
    const lineHeight = parseFloat(getComputedStyle(textBlock.value).lineHeight);
    previewHeight.value = lineHeight * 2; // show 2 lines preview
  }

  // Calculate ms until the next full minute
  const delay = 60_000 - (Date.now() % 60_000);

  currentTimeTimeout = setTimeout(() => {
    now.value = moment();

    currentTimeInterval = setInterval(() => {
      now.value = moment();
    }, 60_000);
  }, delay);
});

onUnmounted(() => {
  clearTimeout(currentTimeTimeout);
  clearInterval(currentTimeInterval);
});

const isOverdue = computed(() => props.task.dueDate?.isBefore(now.value));

const markCompleted = async (taskId: number, completed: boolean) => {
  await completeTask(taskId, completed);
};

const confirmDelete = async (taskId: number) => {
  await deleteTask(taskId);
};
</script>

<style scoped lang="scss">
.task-card {
  --background-color: 255, 255, 255;

  position: relative;
  overflow: visible;
  word-break: break-word;

  &.task-completed {
    --background-color: 197, 197, 197;

    opacity: 0.7;
    transition: none;

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

.task-description {
  overflow: hidden;
  transition: max-height 0.3s ease;
}

.completed-task-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: inherit;
  pointer-events: none;
  background-color: rgba(var(--background-color), 1);
  z-index: -1;
}

.text-preview-fade {
  position: relative;

  &::after {
    content: '';
    display: block;
    position: absolute;
    bottom: 0;
    left: 0;
    width: 100%;
    height: 1.5em;
    background: linear-gradient(
      to bottom,
      rgba(var(--background-color), 0),
      rgba(var(--background-color), 1) 80%
    );
    pointer-events: none;
  }
}
</style>

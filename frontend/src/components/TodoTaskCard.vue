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
              ref="descriptionBlock"
              class="task-description"
              :class="{ 'text-preview-fade': shouldFadeDescription }"
              :style="{
                maxHeight: shouldFadeDescription
                  ? description.collapsedHeight + 'px'
                  : description.fullHeight + 'px',
              }"
            >
              {{ task.description }}
            </div>

            <v-btn
              v-if="description.previewEnabled"
              variant="text"
              size="small"
              class="mt-1 px-0"
              @click.stop="description.expanded = !description.expanded"
            >
              {{ description.expanded ? 'See less' : 'See more' }}
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
import moment from 'moment';
import {
  computed,
  nextTick,
  onMounted,
  onUnmounted,
  reactive,
  ref,
  useTemplateRef,
  watch,
  type PropType,
} from 'vue';
import { completeTask, deleteTask } from '../api/todoTasks';
import type { TodoTask } from '../models';
import _ from 'lodash';

const props = defineProps({
  task: {
    type: Object as PropType<TodoTask>,
    required: true,
  },
});

const emit = defineEmits<{ 'task-clicked': [] }>();

const description = reactive({
  element: useTemplateRef('descriptionBlock'),
  expanded: false,
  collapsedHeight: 0,
  fullHeight: 0,
  previewEnabled: false,
});

const shouldFadeDescription = computed(
  () => description.previewEnabled && !description.expanded
);

const recalculateDescriptionHeights = async () => {
  await nextTick();

  if (!description.element) return;

  description.fullHeight = description.element.scrollHeight;
  const lineHeight = parseFloat(
    getComputedStyle(description.element).lineHeight
  );
  description.collapsedHeight = lineHeight * 2; // show 2 lines of preview
  description.previewEnabled =
    description.fullHeight > description.collapsedHeight;
};

const throttledRecalc = _.throttle(recalculateDescriptionHeights, 50);

watch(() => props.task.description, recalculateDescriptionHeights, {
  immediate: true,
});

const currentTime = ref(moment());
const isOverdue = computed(() =>
  props.task.dueDate?.isBefore(currentTime.value)
);

let fullMinuteTimeout: number | undefined;
let minuteTicker: number | undefined;

onMounted(() => {
  // Calculate ms until the next full minute
  const delay = 60_000 - (Date.now() % 60_000);

  fullMinuteTimeout = setTimeout(() => {
    currentTime.value = moment();

    minuteTicker = setInterval(() => {
      currentTime.value = moment();
    }, 60_000);
  }, delay);

  window.addEventListener('resize', throttledRecalc);
});

onUnmounted(() => {
  clearTimeout(fullMinuteTimeout);
  clearInterval(minuteTicker);

  window.removeEventListener('resize', throttledRecalc);
});

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

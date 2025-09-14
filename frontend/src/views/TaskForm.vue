<template>
  <h1 class="mb-8">{{ taskId ? 'Edit' : 'Create' }} a To-Do task</h1>
  <v-form
    ref="form"
    @submit.prevent="submitTask"
  >
    <div class="d-flex flex-column ga-2 mb-4">
      <v-text-field
        label="Title"
        v-model="task.title"
        :rules="rules.title"
        counter="100"
      />

      <v-textarea
        label="Description"
        v-model="task.description"
        :rules="rules.description"
        counter="2000"
      />

      <datetime-picker
        label="Due date"
        v-model="task.dueDate"
      />
    </div>

    <div class="d-flex justify-end">
      <v-btn
        variant="text"
        :to="{ name: 'home' }"
        class="mr-4"
      >
        Cancel
      </v-btn>
      <v-btn
        color="info"
        type="submit"
        :loading="isLoading"
      >
        Submit
      </v-btn>
    </div>
  </v-form>
</template>

<script setup lang="ts">
import { onMounted, ref, useTemplateRef } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import { createTask, getTask, updateTask, type TodoTaskPayload } from '../api/todoTasks';
import { DatetimePicker } from '../components';

const form = useTemplateRef('form');

const props = defineProps({
  taskId: {
    type: Number,
    default: null,
  },
});

const task = ref<TodoTaskPayload>({
  title: '',
  description: '',
  dueDate: null,
});

const router = useRouter();
const taskLoaded = ref(false);
const isLoading = ref(false);

const rules = {
  title: [
    (value: string) => !!value || 'Title is required',
    (value: string) =>
      value.length <= 100 || 'Title must be 100 characters or less',
  ],
  description: [
    (value: string) =>
      value.length <= 2000 || 'Description must be 2000 characters or less',
  ],
};

const toast = useToast();

const submitTask = async () => {
  const validationResult = await form.value?.validate();
  if (!validationResult?.valid) {
    return;
  }

  isLoading.value = true;
  try {
    if (props.taskId) {
      await updateTask(props.taskId, task.value);
      toast.success(`Successfully updated task "${task.value.title}"`);
    } else {
      await createTask(task.value);
      toast.success(`Successfully created task "${task.value.title}"`);
    }
  } finally {
    isLoading.value = false;
  }

  await router.push({ name: 'home' });
};

onMounted(async () => {
  if (props.taskId) {
    task.value = await getTask(props.taskId);
    taskLoaded.value = true;
  }
});
</script>

<style scoped></style>

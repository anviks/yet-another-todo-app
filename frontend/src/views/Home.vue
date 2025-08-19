<template>
  <pre
    lang="json"
    v-if="tasks.length"
  >
    {{ JSON.stringify(tasks, null, 2) }}
  </pre>


</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { getTasks } from '../api/todoTasks.ts';


const tasks = ref([]);

onMounted(async () => {
  tasks.value = await getTasks();
});
</script>

<style scoped></style>

import { createRouter, createWebHistory } from 'vue-router';
import TaskForm from './views/TaskForm.vue';
import Home from './views/Home.vue';
import Layout from './layout/Layout.vue';

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '',
      component: Layout,
      children: [
        {
          path: '',
          component: Home,
        },
        {
          path: '/add-task',
          component: TaskForm,
        },
      ],
    },
  ],
});

export default router;

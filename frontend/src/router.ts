import { createRouter, createWebHistory, type RouteLocation } from 'vue-router';
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
          name: 'home',
          component: Home,
        },
        {
          path: '/add-task',
          name: 'add-task',
          component: TaskForm,
        },
        {
          path: '/edit-task/:taskId',
          name: 'edit-task',
          component: TaskForm,
          props: (route: RouteLocation) => ({
            taskId: Number(route.params.taskId) || null,
          }),
        },
      ],
    },
  ],
});

export default router;

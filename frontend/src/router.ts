import { createRouter, createWebHistory, type RouteLocation } from 'vue-router';
import TaskForm from './views/TaskForm.vue';
import Home from './views/Home.vue';
import Layout from './layout/Layout.vue';

const SITE_NAME = 'Yet Another To-Do App';

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
          meta: { title: 'Home' },
        },
        {
          path: '/add-task',
          name: 'add-task',
          component: TaskForm,
          meta: { title: 'Add a task' },
        },
        {
          path: '/edit-task/:taskId',
          name: 'edit-task',
          component: TaskForm,
          props: (route: RouteLocation) => ({
            taskId: Number(route.params.taskId) || null,
          }),
          meta: { title: 'Edit a task' },
        },
      ],
    },
  ],
});

router.afterEach((to) => {
  const pageTitle = to.meta.title as string | undefined;
  document.title = pageTitle ? `${SITE_NAME} | ${pageTitle}` : SITE_NAME;
});

export default router;

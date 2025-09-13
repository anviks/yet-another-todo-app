import moment from 'moment';
import type { TodoTask } from '../models.ts';
import apiRequest from './api.ts';

export function convertTaskDates(task: TodoTask): TodoTask {
  return {
    ...task,
    createdAt: moment(task.createdAt),
    dueDate: task.dueDate ? moment(task.dueDate) : null,
    completedAt: task.completedAt ? moment(task.completedAt) : null,
  };
}

export async function getTasks(): Promise<TodoTask[]> {
  const tasks = await apiRequest<TodoTask[]>({ method: 'GET', url: '/todo' });
  return tasks.map(convertTaskDates);
}

export async function getTask(taskId: number): Promise<TodoTask> {
  const task = await apiRequest<TodoTask>({
    method: 'GET',
    url: `/todo/${taskId}`,
  });
  return convertTaskDates(task);
}

export async function createTask(task: any) {
  return await apiRequest({ method: 'POST', url: '/todo', data: task });
}

export async function updateTask(taskId: number, task: any) {
  return await apiRequest({
    method: 'PUT',
    url: `/todo/${taskId}`,
    data: task,
  });
}

function withConnectionHeader(connectionId?: string) {
  return connectionId ? { 'X-Connection-Id': connectionId } : {};
}

export async function deleteTask(taskId: number, connectionId?: string) {
  return await apiRequest({
    method: 'DELETE',
    url: `/todo/${taskId}`,
    headers: { ...withConnectionHeader(connectionId) },
  });
}

export async function markTaskCompletion(
  taskId: number,
  completed: boolean = true,
  connectionId?: string
) {
  return await apiRequest({
    method: 'POST',
    url: `/todo/${taskId}/${completed ? '' : 'un'}complete`,
    headers: { ...withConnectionHeader(connectionId) },
  });
}

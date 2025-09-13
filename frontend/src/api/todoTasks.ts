import moment, { type Moment } from 'moment';
import type { TodoTask } from '../models.ts';
import apiRequest from './api.ts';

export interface TodoTaskPayload {
  title: string;
  description: string;
  dueDate: Moment | null;
  latitude: number;
  longitude: number;
}

export function convertTaskDates(task: TodoTask): TodoTask {
  return {
    ...task,
    createdAt: moment(task.createdAt),
    dueDate: task.dueDate ? moment(task.dueDate) : null,
    completedAt: task.completedAt ? moment(task.completedAt) : null,
  };
}

function withConnectionHeader(connectionId?: string) {
  return connectionId ? { 'X-Connection-Id': connectionId } : {};
}

export async function getTasks() {
  const tasks = await apiRequest<TodoTask[]>({ method: 'GET', url: '/todo' });
  return tasks.map(convertTaskDates);
}

export async function getTask(taskId: number) {
  const task = await apiRequest<TodoTask>({
    method: 'GET',
    url: `/todo/${taskId}`,
  });
  return convertTaskDates(task);
}

export async function createTask(taskPayload: TodoTaskPayload) {
  const task = await apiRequest<TodoTask>({ method: 'POST', url: '/todo', data: taskPayload });
  return convertTaskDates(task);
}

export async function updateTask(taskId: number, taskPayload: TodoTaskPayload) {
  const task = await apiRequest<TodoTask>({
    method: 'PUT',
    url: `/todo/${taskId}`,
    data: taskPayload,
  });
  return convertTaskDates(task);
}

export async function deleteTask(taskId: number, connectionId?: string) {
  return await apiRequest<void>({
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
  const task = await apiRequest<TodoTask>({
    method: 'POST',
    url: `/todo/${taskId}/${completed ? '' : 'un'}complete`,
    headers: { ...withConnectionHeader(connectionId) },
  });
  return convertTaskDates(task);
}

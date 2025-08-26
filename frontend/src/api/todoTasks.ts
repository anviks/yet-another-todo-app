import moment from 'moment';
import type { TodoTask } from '../models.ts';
import client from './client.ts';

export async function getTasks(): Promise<TodoTask[]> {
  const response = await client.get<TodoTask[]>('/todo');
  const tasks = response.data;
  tasks.forEach((task) => {
    if (task.dueDate) task.dueDate = moment(task.dueDate);
  });

  return tasks;
}

export async function getTask(taskId: number): Promise<TodoTask> {
  const response = await client.get<TodoTask>(`/todo/${taskId}`);
  const task = response.data;
  if (task.dueDate) task.dueDate = moment(task.dueDate);

  return task;
}

export async function createTask(task: any) {
  const response = await client.post('/todo', task);
  return response.data;
}

export async function updateTask(taskId: number, task: any) {
  const response = await client.put(`/todo/${taskId}`, task);
  return response.data;
}

export async function deleteTask(taskId: number) {
  const response = await client.delete(`/todo/${taskId}`);
  return response.data;
}

export async function completeTask(taskId: number, completed: boolean = true) {
  const response = await client.post(
    `/todo/${taskId}/${completed ? '' : 'un'}complete`
  );
  return response.data;
}

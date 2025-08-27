import moment from 'moment';
import type { TodoTask } from '../models.ts';
import client from './client.ts';

export function convertTaskDates(task: TodoTask): TodoTask {
  return {
    ...task,
    createdAt: moment(task.createdAt),
    dueDate: task.dueDate ? moment(task.dueDate) : null,
    completedAt: task.completedAt ? moment(task.completedAt) : null,
  };
}

export async function getTasks(): Promise<TodoTask[]> {
  const response = await client.get<TodoTask[]>('/todo');
  const tasks = response.data;

  return tasks.map(convertTaskDates);
}

export async function getTask(taskId: number): Promise<TodoTask> {
  const response = await client.get<TodoTask>(`/todo/${taskId}`);

  return convertTaskDates(response.data);
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

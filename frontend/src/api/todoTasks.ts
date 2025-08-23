import client from './client.ts';

export async function getTasks() {
  const response = await client.get('/todo');
  return response.data;
}

export async function getTask(taskId: number) {
  const response = await client.get(`/todo/${taskId}`);
  return response.data;
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
  const response = await client.post(`/todo/${taskId}/${completed ? '' : 'un'}complete`);
  return response.data;
}

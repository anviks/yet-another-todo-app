import client from './client.ts';

export async function getTasks() {
  const response = await client.get('/todo');
  return response.data;
}

export async function createTask(task: any) {
  const response = await client.post('/todo', task);
  return response.data;
}

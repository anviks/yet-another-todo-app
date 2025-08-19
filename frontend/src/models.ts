import { type Moment } from 'moment';

export interface TodoTask {
  id: number;
  title: string;
  description: string;
  dueDate: Moment | null;
  latitude: number;
  longitude: number;
  isCompleted: boolean;
  createdAt: Moment;
  completedAt: Moment | null;
}

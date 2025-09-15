import { type Moment } from 'moment';

export interface PaginatedResult<T> {
  items: T[];
  hasNextPage: boolean;
}

export interface TodoTask {
  id: number;
  title: string;
  description: string;
  dueDate: Moment | null;
  isCompleted: boolean;
  createdAt: Moment;
  completedAt: Moment | null;
}


export const TASK_STATUSES = ['Pending', 'InProgress', 'Completed'] as const
export type TaskStatus = (typeof TASK_STATUSES)[number]

export const TASK_PRIORITIES = ['Low', 'Medium', 'High'] as const
export type TaskPriority = (typeof TASK_PRIORITIES)[number]

export const TASK_SORTS = ['Newest', 'Priority', 'DueDate'] as const
export type TaskSort = (typeof TASK_SORTS)[number]

export type TaskStatusAction = 'start' | 'complete' | 'reopen'


export const STATUS_LABELS: Record<TaskStatus, string> = {
  Pending: 'Pendiente',
  InProgress: 'En progreso',
  Completed: 'Completada',
}

export const PRIORITY_LABELS: Record<TaskPriority, string> = {
  Low: 'Baja',
  Medium: 'Media',
  High: 'Alta',
}

export const SORT_LABELS: Record<TaskSort, string> = {
  Newest: 'Más recientes',
  Priority: 'Prioridad',
  DueDate: 'Fecha límite',
}

export interface TaskItem {
  id: string
  title: string
  description: string | null
  priority: TaskPriority
  status: TaskStatus
  dueDate: string | null       
  createdAt: string          
  completedAt: string | null
  isOverdue: boolean
}

export interface CreateTaskRequest {
  title: string
  description: string | null
  priority: TaskPriority
  dueDate: string | null
}

export type UpdateTaskRequest = CreateTaskRequest

export interface TaskFilter {
  status?: TaskStatus
  priority?: TaskPriority
  search?: string
  sort?: TaskSort
}
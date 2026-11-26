import type {
  CreateTaskRequest,
  TaskFilter,
  TaskItem,
  TaskStatusAction,
  UpdateTaskRequest,
} from '../types/task'

const BASE_URL = '/api/tasks'

interface ProblemDetails {
  title?: string
  detail?: string
  status?: number
}

export class ApiError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}


async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...init,
    headers: init?.body ? { 'Content-Type': 'application/json' } : undefined,
  })

  if (!response.ok) {
    const problem = (await response.json().catch(() => null)) as ProblemDetails | null
    throw new ApiError(response.status, problem?.detail ?? problem?.title ?? `Error HTTP ${response.status}`)
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

function toQueryString(filter: TaskFilter): string {
  const params = new URLSearchParams()

  if (filter.status) params.set('status', filter.status)
  if (filter.priority) params.set('priority', filter.priority)
  if (filter.search?.trim()) params.set('search', filter.search.trim())
  if (filter.sort) params.set('sort', filter.sort)

  const query = params.toString()
  return query ? `?${query}` : ''
}

const taskUrl = (id: string) => `${BASE_URL}/${encodeURIComponent(id)}`

export const tasksApi = {
  list: (filter: TaskFilter = {}, signal?: AbortSignal) =>
    request<TaskItem[]>(`${BASE_URL}${toQueryString(filter)}`, { signal }),

  getById: (id: string, signal?: AbortSignal) =>
    request<TaskItem>(taskUrl(id), { signal }),

  create: (task: CreateTaskRequest) =>
    request<TaskItem>(BASE_URL, { method: 'POST', body: JSON.stringify(task) }),

  update: (id: string, task: UpdateTaskRequest) =>
    request<TaskItem>(taskUrl(id), { method: 'PUT', body: JSON.stringify(task) }),

  changeStatus: (id: string, action: TaskStatusAction) =>
    request<TaskItem>(`${taskUrl(id)}/${action}`, { method: 'POST' }),

  remove: (id: string) =>
    request<void>(taskUrl(id), { method: 'DELETE' }),
}
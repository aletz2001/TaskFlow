import {
  PRIORITY_LABELS,
  STATUS_LABELS,
  type TaskItem,
  type TaskStatus,
  type TaskStatusAction,
} from '../types/task'
import { formatDate } from '../utils/format'

const ACTIONS_BY_STATUS: Record<TaskStatus, TaskStatusAction[]> = {
  Pending: ['start', 'complete'],
  InProgress: ['complete'],
  Completed: ['reopen'],
}

const ACTION_LABELS: Record<TaskStatusAction, string> = {
  start: 'Iniciar',
  complete: 'Completar',
  reopen: 'Reabrir',
}

interface TaskCardProps {
  task: TaskItem
  isBusy: boolean
  onChangeStatus: (id: string, action: TaskStatusAction) => void
  onDelete: (id: string) => void
}

export function TaskCard({ task, isBusy, onChangeStatus, onDelete }: TaskCardProps) {
  const isDone = task.status === 'Completed'

  return (
    <li className={`task-card${isDone ? ' task-card--done' : ''}`}>
      <div className="task-card__body">
        <h3 className="task-card__title">{task.title}</h3>
        {task.description && <p className="task-card__description">{task.description}</p>}

        <div className="task-card__meta">
          <span className={`badge badge--${task.priority.toLowerCase()}`}>
            {PRIORITY_LABELS[task.priority]}
          </span>
          <span className="badge">{STATUS_LABELS[task.status]}</span>
          {task.dueDate && (
            <span className={task.isOverdue ? 'due due--overdue' : 'due'}>
              Vence: {formatDate(task.dueDate)}
              {task.isOverdue && ' · Vencida'}
            </span>
          )}
        </div>
      </div>

      <div className="task-card__actions">
        {ACTIONS_BY_STATUS[task.status].map(action => (
          <button
            key={action}
            type="button"
            disabled={isBusy}
            onClick={() => onChangeStatus(task.id, action)}
          >
            {ACTION_LABELS[action]}
          </button>
        ))}
        <button
          type="button"
          className="button--danger"
          disabled={isBusy}
          onClick={() => onDelete(task.id)}
        >
          Eliminar
        </button>
      </div>
    </li>
  )
}
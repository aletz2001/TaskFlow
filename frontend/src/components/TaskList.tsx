import type { TaskItem, TaskStatusAction } from '../types/task'
import { TaskCard } from './TaskCard'

interface TaskListProps {
  tasks: TaskItem[]
  busyTaskId: string | null
  onChangeStatus: (id: string, action: TaskStatusAction) => void
  onDelete: (id: string) => void
}

export function TaskList({ tasks, busyTaskId, onChangeStatus, onDelete }: TaskListProps) {
  if (tasks.length === 0) {
    return <p className="muted">No hay tareas para mostrar.</p>
  }

  return (
    <ul className="task-list">
      {tasks.map(task => (
        <TaskCard
          key={task.id}
          task={task}
          isBusy={busyTaskId === task.id}
          onChangeStatus={onChangeStatus}
          onDelete={onDelete}
        />
      ))}
    </ul>
  )
}
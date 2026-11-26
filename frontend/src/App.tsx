import { useEffect, useState } from 'react'
import { tasksApi } from './api/tasksApi'
import type { TaskItem } from './types/task'

export default function App() {
  const [tasks, setTasks] = useState<TaskItem[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()

    tasksApi
      .list({}, controller.signal)
      .then(setTasks)
      .catch((err: unknown) => {
        if (!controller.signal.aborted) {
          setError(err instanceof Error ? err.message : 'Error desconocido')
        }
      })

    return () => controller.abort()   
  }, [])

  return (
    <main>
      <h1>TaskFlow</h1>
      {error && <p role="alert">Error: {error}</p>}
      <p>{tasks.length} tareas</p>
      <ul>
        {tasks.map(task => (
          <li key={task.id}>
            {task.title} — {task.status} — {task.priority}
          </li>
        ))}
      </ul>
    </main>
  )
}
import { useCallback, useEffect, useState } from 'react'
import { tasksApi } from '../api/tasksApi'
import type { TaskFilter, TaskItem } from '../types/task'

interface LoadResult {
  key: string           
  tasks: TaskItem[]
  error: string | null
}

export function useTasks(filter: TaskFilter) {
  const [reloadCount, setReloadCount] = useState(0)
  const [result, setResult] = useState<LoadResult | null>(null)

  const requestKey = `${JSON.stringify(filter)}#${reloadCount}`

  useEffect(() => {
    const controller = new AbortController()

    tasksApi
      .list(filter, controller.signal)
      .then(tasks => {
        if (!controller.signal.aborted) {
          setResult({ key: requestKey, tasks, error: null })
        }
      })
      .catch((err: unknown) => {
        if (controller.signal.aborted) return
        const message = err instanceof Error ? err.message : 'No se pudieron cargar las tareas.'
        setResult(previous => ({ key: requestKey, tasks: previous?.tasks ?? [], error: message }))
      })

    return () => controller.abort()
  }, [filter, requestKey])

  const reload = useCallback(() => setReloadCount(count => count + 1), [])

  return {
    tasks: result?.tasks ?? [],
    isLoading: result?.key !== requestKey,
    error: result?.key === requestKey ? result.error : null,
    reload,
  }
}
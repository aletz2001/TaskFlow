import { useCallback, useMemo, useState } from 'react'
import { tasksApi } from './api/tasksApi'
import { TaskFilters } from './components/TaskFilters'
import { TaskForm } from './components/TaskForm'
import { TaskList } from './components/TaskList'
import { useDebouncedValue } from './hooks/useDebouncedValue'
import { useTasks } from './hooks/useTasks'
import type { TaskFilter, TaskStatusAction } from './types/task'
import './App.css'

export default function App() {

  const [filters, setFilters] = useState<TaskFilter>({ sort: 'Newest' })
  const [searchText, setSearchText] = useState('')
  const [busyTaskId, setBusyTaskId] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)

  const debouncedSearch = useDebouncedValue(searchText, 400)


  const filter = useMemo<TaskFilter>(
    () => ({ ...filters, search: debouncedSearch.trim() || undefined }),
    [filters, debouncedSearch],
  )

  const { tasks, isLoading, error, reload } = useTasks(filter)


  const handleFiltersChange = useCallback((changes: Partial<TaskFilter>) => {
    setFilters(previous => ({ ...previous, ...changes }))   
  }, [])

  const runTaskAction = useCallback(
    async (taskId: string, action: () => Promise<unknown>) => {
      setBusyTaskId(taskId)
      setActionError(null)
      try {
        await action()
        reload()
      } catch (err) {
        setActionError(err instanceof Error ? err.message : 'La operación falló.')
      } finally {
        setBusyTaskId(null)
      }
    },
    [reload],
  )

  const handleChangeStatus = useCallback(
    (id: string, action: TaskStatusAction) => {
      void runTaskAction(id, () => tasksApi.changeStatus(id, action))
    },
    [runTaskAction],
  )

  const handleDelete = useCallback(
    (id: string) => {
      if (!window.confirm('¿Eliminar esta tarea? Esta acción no se puede deshacer.')) return
      void runTaskAction(id, () => tasksApi.remove(id))
    },
    [runTaskAction],
  )


  return (
    <main className="app">
      <header className="app__header">
        <h1>TaskFlow</h1>
        <p className="muted">Organiza tus tareas</p>
      </header>

      <section className="panel" aria-labelledby="new-task-heading">
        <h2 id="new-task-heading">Nueva tarea</h2>
        <TaskForm onCreated={reload} />
      </section>

      <section className="panel" aria-labelledby="tasks-heading" aria-busy={isLoading}>
        <h2 id="tasks-heading">Mis tareas</h2>

        <TaskFilters
          filters={filters}
          searchText={searchText}
          onFiltersChange={handleFiltersChange}
          onSearchTextChange={setSearchText}
        />

        {error && <p role="alert" className="alert">{error}</p>}
        {actionError && <p role="alert" className="alert">{actionError}</p>}

        {isLoading && tasks.length === 0 ? (
          <p className="muted">Cargando…</p>
        ) : (
          <TaskList
            tasks={tasks}
            busyTaskId={busyTaskId}
            onChangeStatus={handleChangeStatus}
            onDelete={handleDelete}
          />
        )}
      </section>
    </main>
  )
}
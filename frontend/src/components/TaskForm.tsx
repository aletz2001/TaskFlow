import { useState, type FormEvent } from 'react'
import { tasksApi } from '../api/tasksApi'
import { PRIORITY_LABELS, TASK_PRIORITIES, type TaskPriority } from '../types/task'

const TITLE_MAX_LENGTH = 100
const DESCRIPTION_MAX_LENGTH = 500

interface TaskFormProps {
  onCreated: () => void
}

export function TaskForm({ onCreated }: TaskFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [priority, setPriority] = useState<TaskPriority>('Medium')
  const [dueDate, setDueDate] = useState('')
  const [isSaving, setIsSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()   

    if (!title.trim()) {
      setError('El título es obligatorio.')
      return
    }

    setIsSaving(true)
    setError(null)

    try {
      await tasksApi.create({
        title: title.trim(),
        description: description.trim() || null,
        priority,
        dueDate: dueDate || null,
      })

      setTitle('')
      setDescription('')
      setPriority('Medium')
      setDueDate('')
      onCreated()   
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo crear la tarea.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <form className="task-form" onSubmit={handleSubmit} noValidate>
      <div className="field">
        <label htmlFor="task-title">Título</label>
        <input
          id="task-title"
          value={title}
          maxLength={TITLE_MAX_LENGTH}
          placeholder="¿Qué hay que hacer?"
          onChange={e => setTitle(e.target.value)}
        />
      </div>

      <div className="field">
        <label htmlFor="task-description">Descripción (opcional)</label>
        <textarea
          id="task-description"
          value={description}
          maxLength={DESCRIPTION_MAX_LENGTH}
          rows={2}
          onChange={e => setDescription(e.target.value)}
        />
      </div>

      <div className="field-row">
        <div className="field">
          <label htmlFor="task-priority">Prioridad</label>
          <select
            id="task-priority"
            value={priority}
            onChange={e => setPriority(TASK_PRIORITIES.find(p => p === e.target.value) ?? 'Medium')}
          >
            {TASK_PRIORITIES.map(p => (
              <option key={p} value={p}>{PRIORITY_LABELS[p]}</option>
            ))}
          </select>
        </div>

        <div className="field">
          <label htmlFor="task-due-date">Fecha límite (opcional)</label>
          <input
            id="task-due-date"
            type="date"
            value={dueDate}
            onChange={e => setDueDate(e.target.value)}
          />
        </div>
      </div>

      {error && <p role="alert" className="alert">{error}</p>}

      <button type="submit" disabled={isSaving}>
        {isSaving ? 'Guardando…' : 'Agregar tarea'}
      </button>
    </form>
  )
}
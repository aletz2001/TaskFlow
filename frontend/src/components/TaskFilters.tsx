import {
  PRIORITY_LABELS,
  SORT_LABELS,
  STATUS_LABELS,
  TASK_PRIORITIES,
  TASK_SORTS,
  TASK_STATUSES,
  type TaskFilter,
} from '../types/task'

interface TaskFiltersProps {
  filters: TaskFilter
  searchText: string
  onFiltersChange: (changes: Partial<TaskFilter>) => void
  onSearchTextChange: (text: string) => void
}

export function TaskFilters({ filters, searchText, onFiltersChange, onSearchTextChange }: TaskFiltersProps) {
  return (
    <div className="filters" role="search">
      <input
        type="search"
        aria-label="Buscar tareas"
        placeholder="Buscar…"
        value={searchText}
        onChange={e => onSearchTextChange(e.target.value)}
      />

      <select
        aria-label="Filtrar por estado"
        value={filters.status ?? ''}
        onChange={e => onFiltersChange({ status: TASK_STATUSES.find(s => s === e.target.value) })}
      >
        <option value="">Todos los estados</option>
        {TASK_STATUSES.map(s => (
          <option key={s} value={s}>{STATUS_LABELS[s]}</option>
        ))}
      </select>

      <select
        aria-label="Filtrar por prioridad"
        value={filters.priority ?? ''}
        onChange={e => onFiltersChange({ priority: TASK_PRIORITIES.find(p => p === e.target.value) })}
      >
        <option value="">Todas las prioridades</option>
        {TASK_PRIORITIES.map(p => (
          <option key={p} value={p}>{PRIORITY_LABELS[p]}</option>
        ))}
      </select>

      <select
        aria-label="Ordenar por"
        value={filters.sort ?? 'Newest'}
        onChange={e => onFiltersChange({ sort: TASK_SORTS.find(s => s === e.target.value) })}
      >
        {TASK_SORTS.map(s => (
          <option key={s} value={s}>{SORT_LABELS[s]}</option>
        ))}
      </select>
    </div>
  )
}
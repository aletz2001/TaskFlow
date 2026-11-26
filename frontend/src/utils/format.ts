const dateFormatter = new Intl.DateTimeFormat('es-CR', {
  day: 'numeric',
  month: 'short',
  year: 'numeric',
  timeZone: 'UTC',
})

export function formatDate(isoDate: string): string {
  return dateFormatter.format(new Date(isoDate))
}
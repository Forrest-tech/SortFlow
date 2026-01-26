import { useState, useEffect, useCallback } from 'react'

const MIN_COL_WIDTH = 60
const DEFAULT_WIDTH = 120

export function useColumnResize(columnCount: number, defaultPx: number = DEFAULT_WIDTH) {
  const [widths, setWidths] = useState<number[]>(() => Array(columnCount).fill(defaultPx))
  const [resizing, setResizing] = useState<{
    i: number
    startX: number
    startW: number
    startWNext: number
  } | null>(null)

  const startResize = useCallback(
    (i: number, e: React.MouseEvent) => {
      if (i >= columnCount - 1) return
      e.preventDefault()
      e.stopPropagation()
      setResizing({ i, startX: e.clientX, startW: widths[i], startWNext: widths[i + 1] })
    },
    [columnCount, widths]
  )

  useEffect(() => {
    if (!resizing) return
    const onMove = (e: MouseEvent) => {
      const delta = e.clientX - resizing.startX
      setWidths((prev) => {
        const next = [...prev]
        const newWi = Math.max(MIN_COL_WIDTH, resizing.startW + delta)
        const newWi1 = Math.max(MIN_COL_WIDTH, resizing.startWNext - delta)
        next[resizing.i] = newWi
        next[resizing.i + 1] = newWi1
        return next
      })
    }
    const onUp = () => setResizing(null)
    window.addEventListener('mousemove', onMove)
    window.addEventListener('mouseup', onUp)
    return () => {
      window.removeEventListener('mousemove', onMove)
      window.removeEventListener('mouseup', onUp)
    }
  }, [resizing])

  const colgroup = (
    <colgroup>
      {widths.map((w, i) => (
        <col key={i} style={{ width: w, minWidth: MIN_COL_WIDTH }} />
      ))}
    </colgroup>
  )

  return { colgroup, startResize, isResizing: !!resizing }
}

import { useState, useEffect, useCallback, useRef } from 'react'
import { getExceptions, getDashboardSummary } from '../api/client'
import type { ExceptionItem } from '../api/client'
import Layout from '../components/Layout'
import { useColumnResize } from '../hooks/useColumnResize'

export default function Exceptions() {
  const [list, setList] = useState<ExceptionItem[]>([])
  const [total, setTotal] = useState(0)
  const [page, setPage] = useState(1)
  const [pageSize, setPageSize] = useState(25)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [summary, setSummary] = useState<{ total: number; addr: number; inv: number; dam: number } | null>(null)
  const { colgroup, startResize } = useColumnResize(5, 100)
  const loadIdRef = useRef(0)
  const tableWrapRef = useRef<HTMLDivElement>(null)

  const load = useCallback(() => {
    const myId = ++loadIdRef.current
    setLoading(true)
    Promise.all([
      getExceptions({ page, pageSize, sortBy: 'Timestamp', sortDir: 'desc' }),
      getDashboardSummary({ windowMinutes: 60 }).catch(() => null)
    ])
      .then(([r, s]) => {
        if (myId !== loadIdRef.current) return
        setList(r.items)
        setTotal(r.totalCount)
        if (s) setSummary({
          total: s.exceptionsLastHour,
          addr: s.eventsByCategory?.AddressMismatch ?? 0,
          inv: s.eventsByCategory?.InvalidPostalCode ?? 0,
          dam: s.eventsByCategory?.DamagedLabel ?? 0
        })
      })
      .catch(e => { if (myId === loadIdRef.current) setError(e instanceof Error ? e.message : 'Failed') })
      .finally(() => { if (myId === loadIdRef.current) setLoading(false) })
  }, [page, pageSize])

  useEffect(() => { load() }, [load])

  const showCard = (!loading || list.length > 0) && !error

  return (
    <Layout title="Exceptions" subtitle="Recent sorting exceptions requiring attention.">
      {summary && (
        <>
          <p className="section-title">Exceptions in the last hour (hover row for details)</p>
          <div className="summary-bar">
          <span>TOTAL: {summary.total}</span>
          <span>ADDRESSMISMATCH: {summary.addr}</span>
          <span>DAMAGEDLABEL: {summary.dam}</span>
          <span>INVALIDPOSTALCODE: {summary.inv}</span>
          </div>
        </>
      )}
      {loading && list.length === 0 && <p className="loading">Loading…</p>}
      {error && <p className="error">{error}</p>}
      {showCard && (
        <div className="card card-fill border-gradient">
          <div className="table-wrap" ref={tableWrapRef}>
            <table className="table-resizable">
              {colgroup}
              <thead>
                <tr>
                  <th>Type<span className="th-resize-handle" onMouseDown={(e) => startResize(0, e)} /></th>
                  <th>Item<span className="th-resize-handle" onMouseDown={(e) => startResize(1, e)} /></th>
                  <th>Station<span className="th-resize-handle" onMouseDown={(e) => startResize(2, e)} /></th>
                  <th>Details<span className="th-resize-handle" onMouseDown={(e) => startResize(3, e)} /></th>
                  <th>Time (UTC)</th>
                </tr>
              </thead>
              <tbody>
                {list.length === 0 ? (
                  <tr><td colSpan={5}>{loading ? 'Loading…' : 'No exceptions'}</td></tr>
                ) : (
                  list.map(x => (
                    <tr key={x.id}>
                      <td><span className="badge err">{x.exceptionType}</span></td>
                      <td>{x.itemId}</td>
                      <td>{x.stationName}</td>
                      <td style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>{x.details}</td>
                      <td style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>{new Date(x.createdAtUtc).toISOString().replace('T', ' ').slice(0, 19)}</td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
          <div className="pagination">
            <button type="button" className="btn-ghost" disabled={page <= 1} onClick={() => { setPage(p => p - 1); tableWrapRef.current?.scrollTo(0, 0) }}>Prev</button>
            <span>Page {page} of {Math.max(1, Math.ceil(total / pageSize))} ({total} total)</span>
            <button type="button" className="btn-ghost" disabled={page >= Math.ceil(total / pageSize) || total === 0} onClick={() => { setPage(p => p + 1); tableWrapRef.current?.scrollTo(0, 0) }}>Next</button>
            <label style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              Per page
              <select value={pageSize} onChange={e => { const v = Number(e.target.value); setPageSize(v); setPage(1); tableWrapRef.current?.scrollTo(0, 0) }}>
                <option value={10}>10</option>
                <option value={25}>25</option>
                <option value={50}>50</option>
                <option value={100}>100</option>
                <option value={200}>200</option>
              </select>
            </label>
          </div>
        </div>
      )}
    </Layout>
  )
}

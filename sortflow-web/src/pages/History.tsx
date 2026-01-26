import { useState, useEffect, useCallback, useRef } from 'react'
import { getHistoryExportCsv, getEvents, getExceptions } from '../api/client'
import type { EventItem, ExceptionItem } from '../api/client'
import Layout from '../components/Layout'
import { useColumnResize } from '../hooks/useColumnResize'

type Tab = 'events' | 'exceptions'

export default function History() {
  const [tab, setTab] = useState<Tab>('events')
  const [groupBy, setGroupBy] = useState('day')
  const [from, setFrom] = useState(() => new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10))
  const [to, setTo] = useState(() => new Date().toISOString().slice(0, 10))
  const [exporting, setExporting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const [eventsList, setEventsList] = useState<EventItem[]>([])
  const [eventsTotal, setEventsTotal] = useState(0)
  const [eventsPage, setEventsPage] = useState(1)
  const [eventsLoading, setEventsLoading] = useState(false)

  const [exceptionsList, setExceptionsList] = useState<ExceptionItem[]>([])
  const [exceptionsTotal, setExceptionsTotal] = useState(0)
  const [exceptionsPage, setExceptionsPage] = useState(1)
  const [exceptionsLoading, setExceptionsLoading] = useState(false)
  const [pageSize, setPageSize] = useState(50)

  const { colgroup, startResize } = useColumnResize(tab === 'events' ? 6 : 5, 100)
  const loadIdRef = useRef(0)
  const tableWrapRef = useRef<HTMLDivElement>(null)

  const fromISO = from ? new Date(from).toISOString() : undefined
  const toISO = to ? new Date(to + 'T23:59:59').toISOString() : undefined

  const loadEvents = useCallback(() => {
    const myId = ++loadIdRef.current
    setEventsLoading(true)
    setError(null)
    getEvents({ timeFrom: fromISO, timeTo: toISO, page: eventsPage, pageSize, sortBy: 'Timestamp', sortDir: 'desc' })
      .then(r => { if (myId === loadIdRef.current) { setEventsList(r.items); setEventsTotal(r.totalCount) } })
      .catch(e => { if (myId === loadIdRef.current) setError(e instanceof Error ? e.message : 'Failed') })
      .finally(() => { if (myId === loadIdRef.current) setEventsLoading(false) })
  }, [fromISO, toISO, eventsPage])

  const loadExceptions = useCallback(() => {
    const myId = ++loadIdRef.current
    setExceptionsLoading(true)
    setError(null)
    getExceptions({ timeFrom: fromISO, timeTo: toISO, page: exceptionsPage, pageSize, sortBy: 'Timestamp', sortDir: 'desc' })
      .then(r => { if (myId === loadIdRef.current) { setExceptionsList(r.items); setExceptionsTotal(r.totalCount) } })
      .catch(e => { if (myId === loadIdRef.current) setError(e instanceof Error ? e.message : 'Failed') })
      .finally(() => { if (myId === loadIdRef.current) setExceptionsLoading(false) })
  }, [fromISO, toISO, exceptionsPage, pageSize])

  useEffect(() => { if (groupBy === 'day') setTo(from) }, [groupBy, from])
  useEffect(() => { setEventsPage(1); setExceptionsPage(1) }, [from, to])
  useEffect(() => { if (tab === 'events') loadEvents() }, [tab, loadEvents])
  useEffect(() => { if (tab === 'exceptions') loadExceptions() }, [tab, loadExceptions])

  async function handleExport() {
    setExporting(true)
    try {
      const blob = await getHistoryExportCsv(fromISO, toISO)
      const a = document.createElement('a')
      a.href = URL.createObjectURL(blob)
      a.download = `sortflow-${from || 'start'}-${to || 'end'}.csv`
      a.click()
      URL.revokeObjectURL(a.href)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Export failed')
    } finally {
      setExporting(false)
    }
  }

  const loading = tab === 'events' ? eventsLoading : exceptionsLoading
  const list = tab === 'events' ? eventsList : exceptionsList
  const total = tab === 'events' ? eventsTotal : exceptionsTotal
  const page = tab === 'events' ? eventsPage : exceptionsPage
  const setPage = tab === 'events' ? setEventsPage : setExceptionsPage
  const totalPages = Math.max(1, Math.ceil(total / pageSize))
  const showCard = (!loading || list.length > 0) && !error

  return (
    <Layout title="History" subtitle="Aggregated by day, week, or month. Export to CSV. Raw events and exceptions by date range.">
      <div className="filters">
        <select
          value={groupBy}
          onChange={e => {
            const v = e.target.value
            setGroupBy(v)
            if (v === 'day') setTo(from)
          }}
        >
          <option value="day">By day</option>
          <option value="week">By week</option>
          <option value="month">By month</option>
        </select>
        {groupBy === 'day' ? (
          <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            Date
            <input type="date" value={from} onChange={e => { const v = e.target.value; setFrom(v); setTo(v) }} />
          </label>
        ) : (
          <>
            <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              From
              <input type="date" value={from} onChange={e => setFrom(e.target.value)} />
            </label>
            <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              To
              <input type="date" value={to} onChange={e => setTo(e.target.value)} />
            </label>
          </>
        )}
        <button type="button" className="btn" onClick={() => { if (tab === 'events') loadEvents(); else loadExceptions() }}>Apply</button>
        <button type="button" className="btn btn-ghost" onClick={handleExport} disabled={exporting}>{exporting ? 'Exporting…' : 'Export CSV'}</button>
      </div>

      <div className="tabs">
        <button type="button" className={`tab-btn ${tab === 'events' ? 'active' : ''}`} onClick={() => { setTab('events'); setEventsPage(1) }}>Events</button>
        <button type="button" className={`tab-btn ${tab === 'exceptions' ? 'active' : ''}`} onClick={() => { setTab('exceptions'); setExceptionsPage(1) }}>Exceptions</button>
      </div>

      {loading && list.length === 0 && <p className="loading">Loading…</p>}
      {error && <p className="error">{error}</p>}
      {showCard && (
        <div className="card card-fill border-gradient">
          <div className="table-wrap" ref={tableWrapRef}>
            <table className="table-resizable">
              {colgroup}
              <thead>
                <tr>
                  {tab === 'events' ? (
                    <>
                      <th>Item<span className="th-resize-handle" onMouseDown={e => startResize(0, e)} /></th>
                      <th>Postal<span className="th-resize-handle" onMouseDown={e => startResize(1, e)} /></th>
                      <th>Station<span className="th-resize-handle" onMouseDown={e => startResize(2, e)} /></th>
                      <th>Zone<span className="th-resize-handle" onMouseDown={e => startResize(3, e)} /></th>
                      <th>Result<span className="th-resize-handle" onMouseDown={e => startResize(4, e)} /></th>
                      <th>Time (UTC)</th>
                    </>
                  ) : (
                    <>
                      <th>Type<span className="th-resize-handle" onMouseDown={e => startResize(0, e)} /></th>
                      <th>Item<span className="th-resize-handle" onMouseDown={e => startResize(1, e)} /></th>
                      <th>Station<span className="th-resize-handle" onMouseDown={e => startResize(2, e)} /></th>
                      <th>Details<span className="th-resize-handle" onMouseDown={e => startResize(3, e)} /></th>
                      <th>Time (UTC)</th>
                    </>
                  )}
                </tr>
              </thead>
              <tbody>
                {tab === 'events' ? (
                  list.length === 0 ? (
                    <tr><td colSpan={6}>{loading ? 'Loading…' : 'No events in this range.'}</td></tr>
                  ) : (
                    list.map(e => (
                      <tr key={e.id}>
                        <td>{e.itemId}</td>
                        <td><span className="code-tag">{e.postalCode}</span></td>
                        <td>{e.stationName}</td>
                        <td>{e.zoneName}</td>
                        <td>{e.isSuccessful ? <span className="badge ok">OK</span> : <span className="badge err">{e.exceptionType ?? 'Ex'}</span>}</td>
                        <td style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>{new Date(e.processedAtUtc).toISOString().replace('T', ' ').slice(0, 19)}</td>
                      </tr>
                    ))
                  )
                ) : (
                  list.length === 0 ? (
                    <tr><td colSpan={5}>{loading ? 'Loading…' : 'No exceptions in this range.'}</td></tr>
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
                  )
                )}
              </tbody>
            </table>
          </div>
          <div className="pagination">
            <button type="button" className="btn-ghost" disabled={page <= 1} onClick={() => { setPage(p => p - 1); tableWrapRef.current?.scrollTo(0, 0) }}>Prev</button>
            <span>Page {page} of {totalPages} ({total} total)</span>
            <button type="button" className="btn-ghost" disabled={page >= totalPages || total === 0} onClick={() => { setPage(p => p + 1); tableWrapRef.current?.scrollTo(0, 0) }}>Next</button>
            <label style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              Per page
              <select value={pageSize} onChange={e => { const v = Number(e.target.value); setPageSize(v); setEventsPage(1); setExceptionsPage(1); tableWrapRef.current?.scrollTo(0, 0) }}>
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

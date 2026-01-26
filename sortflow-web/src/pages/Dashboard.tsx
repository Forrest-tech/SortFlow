import { useState, useEffect, useCallback } from 'react'
import * as Hub from '@microsoft/signalr'
import { getDashboardSummary, getSignalRHubUrl, getTokenForSignalR } from '../api/client'
import type { DashboardSummary } from '../api/client'
import Layout from '../components/Layout'
import './Dashboard.css'

function BarRow({ label, value, max, type }: { label: string; value: number; max: number; type: 'success' | 'addr' | 'dam' | 'inv' }) {
  const pct = max > 0 ? (value / max) * 100 : 0
  return (
    <div className="dashboard-bar-row">
      <span className="dashboard-bar-label">{label}</span>
      <div className="dashboard-bar-track">
        <div className={`dashboard-bar-fill ${type}`} style={{ width: `${pct}%` }} />
      </div>
      <span className="dashboard-bar-value">{value}</span>
    </div>
  )
}

export default function Dashboard() {
  const [data, setData] = useState<DashboardSummary | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [live, setLive] = useState(false)
  const [windowMin, setWindowMin] = useState<number | ''>(60)

  const load = useCallback(async () => {
    try {
      const s = await getDashboardSummary({ windowMinutes: windowMin === '' ? 60 : windowMin })
      setData(s)
      setError(null)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load')
    } finally {
      setLoading(false)
    }
  }, [windowMin])

  useEffect(() => { load() }, [load])

  useEffect(() => {
    const token = getTokenForSignalR()
    if (!token) return
    const c = new Hub.HubConnectionBuilder()
      .withUrl(getSignalRHubUrl(), { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build()
    c.on('sortingEventReceived', () => { setLive(true); load() })
    c.on('dashboard:summaryUpdated', (s: DashboardSummary) => { setLive(true); setData(s) })
    c.on('events:newBatch', () => setLive(true))
    c.on('exceptions:newBatch', () => setLive(true))
    c.start().then(() => setLive(true)).catch(() => {})
    return () => { c.stop() }
  }, [load])

  let updated = ''
  try {
    updated = data?.generatedAtUtc ? new Date(data.generatedAtUtc).toLocaleString('en-GB', { day: 'numeric', month: 'Jan', year: 'numeric', hour: '2-digit', minute: '2-digit', second: '2-digit' }) : ''
  } catch { updated = '' }

  const cat = data?.eventsByCategory ?? {}
  const ok = cat['OK'] ?? data?.successfulEventsLastHour ?? 0
  const addr = cat['AddressMismatch'] ?? 0
  const dam = cat['DamagedLabel'] ?? 0
  const inv = cat['InvalidPostalCode'] ?? 0
  const eventsMax = Math.max(ok, addr, dam, inv, 1)
  const excMax = Math.max(addr, dam, inv, 1)

  return (
    <Layout
      title="Dashboard"
      subtitle={`Overview of sorting activity today. Data as of ${updated || '—'}${live ? ' — Updated just now' : ''}`}
      live={live}
    >
      <div className="dashboard-toolbar">
        <select className="select-dark" value={String(windowMin || 60)} onChange={e => setWindowMin(Number(e.target.value) || 60)}>
          <option value="60">Last hour</option>
          <option value="480">Today (8h)</option>
          <option value="1440">Last 24h</option>
        </select>
      </div>
      {loading && <p className="loading">Loading…</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && !data && (
        <p className="loading">Unable to load dashboard. Check that the API is running and you are signed in.</p>
      )}
      {data && (
        <>
          <div className="dashboard-grid-top">
            <div className="card border-gradient">
              <div className="card-label">ITEMS / MIN</div>
              <div className="card-value">{data.itemsPerMinute}</div>
            </div>
            <div className="card border-gradient">
              <div className="card-label">ITEMS / HOUR</div>
              <div className="card-value">{data.itemsPerHour}</div>
              <div className="card-meta">{windowMin || 60} min</div>
            </div>
            <div className="card border-gradient">
              <div className="card-label">TOTAL (TODAY)</div>
              <div className="card-value">{data.totalToday}</div>
            </div>
          </div>

          <div className="dashboard-grid-mid">
            <div className="dashboard-mid-left">
              <div className="card border-gradient">
                <div className="card-label">TOTAL EVENTS</div>
                <div className="card-value">{data.totalEventsLastHour}</div>
              </div>
              <div className="card border-gradient">
                <div className="card-label">SUCCESS RATE (PROCESS)</div>
                <div className="dashboard-circle-wrap">
                  <div className="dashboard-circle">
                    <svg className="dashboard-circle-svg" viewBox="0 0 36 36">
                      <path className="dashboard-circle-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" strokeWidth="2.5" />
                      <path
                        className="dashboard-circle-fill"
                        strokeDasharray={`${data.successRate} ${100 - data.successRate}`}
                        strokeDashoffset="0"
                        d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                        strokeWidth="2.5"
                      />
                    </svg>
                    <div className="dashboard-circle-text">
                      <span className="dashboard-circle-value">{data.successRate}%</span>
                      <span className="dashboard-circle-sub">{data.successfulEventsLastHour} / {data.totalEventsLastHour}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="card border-gradient">
              <div className="dashboard-chart-title">EVENTS BY CATEGORY</div>
              <div className="dashboard-bar-list">
                <BarRow label="Successful" value={ok} max={eventsMax} type="success" />
                <BarRow label="AddressMismatch" value={addr} max={eventsMax} type="addr" />
                <BarRow label="DamagedLabel" value={dam} max={eventsMax} type="dam" />
                <BarRow label="InvalidPostalCode" value={inv} max={eventsMax} type="inv" />
              </div>
            </div>
          </div>

          <div className="dashboard-grid-bot">
            <div className="card border-gradient">
              <div className="card-label">TOTAL EXCEPTIONS</div>
              <div className="card-value">{data.exceptionsLastHour}</div>
            </div>
            <div className="card border-gradient">
              <div className="dashboard-chart-title">BY CATEGORY</div>
              <div className="dashboard-bar-list">
                <BarRow label="AddressMismatch" value={addr} max={excMax} type="addr" />
                <BarRow label="DamagedLabel" value={dam} max={excMax} type="dam" />
                <BarRow label="InvalidPostalCode" value={inv} max={excMax} type="inv" />
              </div>
            </div>
          </div>
        </>
      )}
    </Layout>
  )
}

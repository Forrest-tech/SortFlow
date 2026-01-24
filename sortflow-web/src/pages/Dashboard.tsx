import { useState, useEffect, useCallback } from 'react'
import * as Hub from '@microsoft/signalr'
import { getDashboardSummary, getSignalRHubUrl, getTokenForSignalR } from '../api/client'
import type { DashboardSummary } from '../api/client'
import Nav from '../components/Nav'
import './Dashboard.css'

export default function Dashboard() {
  const [data, setData] = useState<DashboardSummary | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [live, setLive] = useState(false)

  const load = useCallback(async () => {
    try {
      const s = await getDashboardSummary()
      setData(s)
      setError(null)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    load()
  }, [load])

  useEffect(() => {
    const token = getTokenForSignalR()
    if (!token) return
    const c = new Hub.HubConnectionBuilder()
      .withUrl(getSignalRHubUrl(), { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build()
    c.on('sortingEventReceived', () => { setLive(true); load() })
    c.start().then(() => setLive(true)).catch(() => {})
    return () => { c.stop() }
  }, [load])

  return (
    <>
      <Nav />
      <div className="dashboard-header">
        <h1>Dashboard</h1>
        {live && <span className="live">Live</span>}
      </div>
      {loading && <p>Loading…</p>}
      {error && <p className="error">{error}</p>}
      {data && (
        <div className="grid">
          <div className="card">
            <div className="card-label">Total (1h)</div>
            <div className="card-value">{data.totalEventsLastHour}</div>
          </div>
          <div className="card">
            <div className="card-label">Successful (1h)</div>
            <div className="card-value">{data.successfulEventsLastHour}</div>
          </div>
          <div className="card">
            <div className="card-label">Exceptions (1h)</div>
            <div className="card-value badge err">{data.exceptionsLastHour}</div>
          </div>
          <div className="card">
            <div className="card-label">Items / min</div>
            <div className="card-value">{data.itemsPerMinute}</div>
          </div>
          <div className="card">
            <div className="card-label">Items / hour</div>
            <div className="card-value">{data.itemsPerHour}</div>
          </div>
        </div>
      )}
    </>
  )
}

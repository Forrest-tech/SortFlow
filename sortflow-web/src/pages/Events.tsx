import { useState, useEffect } from 'react'
import { getEvents } from '../api/client'
import type { EventItem } from '../api/client'
import Nav from '../components/Nav'
import './Events.css'

export default function Events() {
  const [list, setList] = useState<EventItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getEvents(80)
      .then(setList)
      .catch((e) => setError(e instanceof Error ? e.message : 'Failed to load'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <>
      <Nav />
      <h1>Events</h1>
      {loading && <p>Loading…</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && (
        <div className="card">
          <table>
            <thead>
              <tr>
                <th>Item</th>
                <th>Postal</th>
                <th>Station</th>
                <th>Zone</th>
                <th>Result</th>
                <th>Time (UTC)</th>
              </tr>
            </thead>
            <tbody>
              {list.length === 0 && (
                <tr><td colSpan={6}>No events yet. The background generator will create them.</td></tr>
              )}
              {list.map((e) => (
                <tr key={e.id}>
                  <td>{e.itemId}</td>
                  <td><code>{e.postalCode}</code></td>
                  <td>{e.stationName}</td>
                  <td>{e.zoneName}</td>
                  <td>
                    {e.isSuccessful
                      ? <span className="badge ok">OK</span>
                      : <span className="badge err">{e.exceptionType ?? 'Exception'}</span>
                    }
                  </td>
                  <td>{new Date(e.processedAtUtc).toISOString().replace('T', ' ').slice(0, 19)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}

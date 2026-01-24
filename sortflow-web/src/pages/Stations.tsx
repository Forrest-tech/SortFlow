import { useState, useEffect } from 'react'
import { getStations } from '../api/client'
import type { StationItem } from '../api/client'
import Nav from '../components/Nav'
import './Stations.css'

export default function Stations() {
  const [list, setList] = useState<StationItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getStations()
      .then(setList)
      .catch((e) => setError(e instanceof Error ? e.message : 'Failed to load'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <>
      <Nav />
      <h1>Stations</h1>
      {loading && <p>Loading…</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && (
        <div className="card">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Code</th>
                <th>Zone</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {list.length === 0 && (
                <tr><td colSpan={4}>No stations. Start the API to seed data.</td></tr>
              )}
              {list.map((s) => (
                <tr key={s.id}>
                  <td>{s.name}</td>
                  <td><code>{s.stationCode}</code></td>
                  <td>{s.zoneName}</td>
                  <td>{s.isActive ? <span className="badge ok">Active</span> : <span className="badge warn">Inactive</span>}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}

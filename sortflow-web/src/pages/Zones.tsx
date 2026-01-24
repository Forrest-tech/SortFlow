import { useState, useEffect } from 'react'
import { getZones } from '../api/client'
import type { ZoneItem } from '../api/client'
import Nav from '../components/Nav'
import './Zones.css'

export default function Zones() {
  const [list, setList] = useState<ZoneItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getZones()
      .then(setList)
      .catch((e) => setError(e instanceof Error ? e.message : 'Failed to load'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <>
      <Nav />
      <h1>Zones</h1>
      {loading && <p>Loading…</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && (
        <div className="card">
          <table>
            <thead>
              <tr>
                <th>Name</th>
                <th>Code</th>
                <th>Status</th>
                <th>Stations</th>
              </tr>
            </thead>
            <tbody>
              {list.length === 0 && (
                <tr><td colSpan={4}>No zones. Start the API to seed data.</td></tr>
              )}
              {list.map((z) => (
                <tr key={z.id}>
                  <td>{z.name}</td>
                  <td><code>{z.code}</code></td>
                  <td>{z.isActive ? <span className="badge ok">Active</span> : <span className="badge warn">Inactive</span>}</td>
                  <td>{z.stationCount}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}

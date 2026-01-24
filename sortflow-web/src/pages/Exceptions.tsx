import { useState, useEffect } from 'react'
import { getExceptions } from '../api/client'
import type { ExceptionItem } from '../api/client'
import Nav from '../components/Nav'
import './Exceptions.css'

export default function Exceptions() {
  const [list, setList] = useState<ExceptionItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getExceptions(50)
      .then(setList)
      .catch((e) => setError(e instanceof Error ? e.message : 'Failed to load'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <>
      <Nav />
      <h1>Exceptions</h1>
      {loading && <p>Loading…</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && (
        <div className="card">
          <table>
            <thead>
              <tr>
                <th>Type</th>
                <th>Item</th>
                <th>Station</th>
                <th>Details</th>
                <th>Time (UTC)</th>
              </tr>
            </thead>
            <tbody>
              {list.length === 0 && (
                <tr><td colSpan={5}>No exceptions</td></tr>
              )}
              {list.map((x) => (
                <tr key={x.id}>
                  <td><span className="badge err">{x.exceptionType}</span></td>
                  <td>{x.itemId}</td>
                  <td>{x.stationName}</td>
                  <td>{x.details}</td>
                  <td>{new Date(x.createdAtUtc).toISOString().replace('T', ' ').slice(0, 19)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}

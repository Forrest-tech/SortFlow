import { useState, useEffect, useCallback } from 'react'
import { getStations, getZones, createStation, updateStation, deleteStation } from '../api/client'
import type { StationItem, ZoneItem } from '../api/client'
import Layout from '../components/Layout'
import { useColumnResize } from '../hooks/useColumnResize'

export default function Stations() {
  const [list, setList] = useState<StationItem[]>([])
  const [zones, setZones] = useState<ZoneItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [edit, setEdit] = useState<StationItem | null>(null)
  const [form, setForm] = useState({ name: '', stationCode: '', isActive: true, zoneId: '' })
  const { colgroup, startResize } = useColumnResize(5, 100)

  const load = useCallback(() => {
    Promise.all([getStations(), getZones()])
      .then(([stations, z]) => { setList(stations); setZones(z); setForm(f => (f.zoneId ? f : { ...f, zoneId: z[0]?.id || '' })) })
      .catch(e => setError(e instanceof Error ? e.message : 'Failed'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  function clearForm() { setForm({ name: '', stationCode: '', isActive: true, zoneId: zones[0]?.id || '' }); setEdit(null) }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!form.zoneId) { setError('Select a zone'); return }
    setError(null)
    try {
      if (edit) await updateStation(edit.id, { name: form.name, stationCode: form.stationCode || undefined, isActive: form.isActive, zoneId: form.zoneId })
      else await createStation({ name: form.name, stationCode: form.stationCode || undefined, isActive: form.isActive, zoneId: form.zoneId })
      clearForm()
      load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed')
    }
  }

  async function handleDelete(s: StationItem) {
    if (!confirm(`Delete station "${s.name}"?`)) return
    setError(null)
    try {
      await deleteStation(s.id)
      load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed')
    }
  }

  return (
    <Layout title="Stations" subtitle="Sorting stations by zone. Rename, set code, Zone, Active/Offline, and add or delete stations.">
      <p className="section-title">Stations</p>
      <p className="section-desc">Rename, set code, Zone, Active/Offline, and add or delete stations.</p>
      <form onSubmit={handleSubmit} className="form-inline">
        <input placeholder="Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
        <input placeholder="Code" value={form.stationCode} onChange={e => setForm(f => ({ ...f, stationCode: e.target.value }))} />
        <select value={form.zoneId} onChange={e => setForm(f => ({ ...f, zoneId: e.target.value }))} required>
          <option value="">— Zone —</option>
          {zones.map(z => <option key={z.id} value={z.id}>{z.name}</option>)}
        </select>
        <label><input type="checkbox" checked={form.isActive} onChange={e => setForm(f => ({ ...f, isActive: e.target.checked }))} /> Active</label>
        <button type="submit" className="btn">{edit ? 'Update' : 'Add station'}</button>
        {edit && <button type="button" className="btn-ghost" onClick={clearForm}>Cancel</button>}
      </form>
      {loading && <p className="loading">Loading…</p>}
      {error && <p className="error">{error}</p>}
      {!loading && (
        <div className="card card-fill border-gradient">
          <div className="table-wrap">
            <table className="table-resizable">
              {colgroup}
              <thead>
                <tr>
                  <th>Name<span className="th-resize-handle" onMouseDown={(e) => startResize(0, e)} /></th>
                  <th>Code<span className="th-resize-handle" onMouseDown={(e) => startResize(1, e)} /></th>
                  <th>Zone<span className="th-resize-handle" onMouseDown={(e) => startResize(2, e)} /></th>
                  <th>Status<span className="th-resize-handle" onMouseDown={(e) => startResize(3, e)} /></th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {list.length === 0 && <tr><td colSpan={5}>No stations.</td></tr>}
                {list.map(s => (
                  <tr key={s.id}>
                    <td>{s.name}</td>
                    <td><span className="code-tag">{s.stationCode}</span></td>
                    <td>{s.zoneName}</td>
                    <td>{s.isActive ? <span className="badge ok">Active</span> : <span className="badge warn">Inactive</span>}</td>
                    <td>
                      <button type="button" className="btn-ghost" style={{ marginRight: '0.5rem' }} onClick={() => { setEdit(s); setForm({ name: s.name, stationCode: s.stationCode, isActive: s.isActive, zoneId: s.zoneId }) }}>Edit</button>
                      <button type="button" className="btn-icon danger" onClick={() => handleDelete(s)} title="Delete">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polyline points="3 6 5 6 21 6"/><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"/></svg>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </Layout>
  )
}

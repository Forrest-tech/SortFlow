import { useState, useEffect, useCallback } from 'react'
import { getZones, createZone, updateZone, deleteZone } from '../api/client'
import type { ZoneItem } from '../api/client'
import Layout from '../components/Layout'
import { useColumnResize } from '../hooks/useColumnResize'

export default function Zones() {
  const [list, setList] = useState<ZoneItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [edit, setEdit] = useState<ZoneItem | null>(null)
  const [form, setForm] = useState({ name: '', code: '', isActive: true })
  const { colgroup, startResize } = useColumnResize(5, 100)

  const load = useCallback(() => {
    getZones().then(setList).catch(e => setError(e instanceof Error ? e.message : 'Failed')).finally(() => setLoading(false))
  }, [])

  useEffect(() => { load() }, [load])

  function clearForm() { setForm({ name: '', code: '', isActive: true }); setEdit(null) }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    try {
      if (edit) await updateZone(edit.id, form)
      else await createZone(form)
      clearForm()
      load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed')
    }
  }

  async function handleDelete(z: ZoneItem) {
    if (!confirm(`Delete zone "${z.name}"? This will fail if it has stations.`)) return
    setError(null)
    try {
      await deleteZone(z.id)
      load()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed')
    }
  }

  return (
    <Layout title="Zones" subtitle="Sorting zones and their station counts. Rename, set code, Active/Offline, and add or delete zones.">
      <p className="section-title">Zones</p>
      <p className="section-desc">Rename, set code, Active/Offline, and add or delete zones.</p>
      <form onSubmit={handleSubmit} className="form-inline">
        <input placeholder="Name" value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))} required />
        <input placeholder="Code" value={form.code} onChange={e => setForm(f => ({ ...f, code: e.target.value }))} required />
        <label><input type="checkbox" checked={form.isActive} onChange={e => setForm(f => ({ ...f, isActive: e.target.checked }))} /> Active</label>
        <button type="submit" className="btn">{edit ? 'Update' : 'Add zone'}</button>
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
                  <th>Status<span className="th-resize-handle" onMouseDown={(e) => startResize(2, e)} /></th>
                  <th>Stations<span className="th-resize-handle" onMouseDown={(e) => startResize(3, e)} /></th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {list.length === 0 && <tr><td colSpan={5}>No zones.</td></tr>}
                {list.map(z => (
                  <tr key={z.id}>
                    <td>{z.name}</td>
                    <td><span className="code-tag">{z.code}</span></td>
                    <td>{z.isActive ? <span className="badge ok">Active</span> : <span className="badge warn">Inactive</span>}</td>
                    <td>{z.stationCount}</td>
                    <td>
                      <button type="button" className="btn-ghost" style={{ marginRight: '0.5rem' }} onClick={() => { setEdit(z); setForm({ name: z.name, code: z.code, isActive: z.isActive }) }}>Edit</button>
                      <button type="button" className="btn-icon danger" onClick={() => handleDelete(z)} title="Delete">
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

import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { getSettings, putSettings, generatorStart, generatorStop, generatorStatus } from '../api/client'
import type { AppSettings } from '../api/client'
import Layout from '../components/Layout'
import Toggle from '../components/Toggle'

const MODULE_KEYS = ['Dashboard', 'Events', 'Exceptions', 'Zones', 'Stations'] as const
const defaultModules = Object.fromEntries(MODULE_KEYS.map(k => [k, true])) as Record<string, boolean>

export default function Settings() {
  const [s, setS] = useState<AppSettings | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [genRunning, setGenRunning] = useState(false)
  const [genRate, setGenRate] = useState(1)
  const [userName, setUserName] = useState(() => localStorage.getItem('sortflow_userName') || 'Forrest')
  const [modules, setModules] = useState<Record<string, boolean>>(defaultModules)

  useEffect(() => {
    Promise.all([getSettings(), generatorStatus()])
      .then(([settings, status]) => {
        setS(settings || null)
        setGenRunning(status.isRunning)
        setGenRate(status.ratePerSecond)
        if (settings?.enableModules) {
          try {
            const m = JSON.parse(settings.enableModules)
            if (m && typeof m === 'object') setModules(() => ({ ...defaultModules, ...m }))
          } catch { /* ignore */ }
        }
      })
      .catch(e => setError(e instanceof Error ? e.message : 'Failed'))
      .finally(() => setLoading(false))
  }, [])

  async function handleSave() {
    if (!s) return
    setSaving(true)
    setError(null)
    try {
      localStorage.setItem('sortflow_userName', userName)
      const updated = await putSettings({ ...s, enableModules: JSON.stringify(modules) })
      setS(updated)
      const status = await generatorStatus()
      setGenRunning(status.isRunning)
      setGenRate(status.ratePerSecond)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Save failed')
    } finally {
      setSaving(false)
    }
  }

  async function handleGenStart() {
    try { await generatorStart(); setGenRunning(true) } catch (e) { setError(e instanceof Error ? e.message : 'Failed') }
  }
  async function handleGenStop() {
    try { await generatorStop(); setGenRunning(false) } catch (e) { setError(e instanceof Error ? e.message : 'Failed') }
  }

  if (loading) return <Layout title="Settings"><p className="loading">Loading…</p></Layout>
  if (!s) return <Layout title="Settings"><p className="error">Settings not found. Ensure API has run and DB is seeded.</p></Layout>

  return (
    <Layout title="Settings" subtitle="Configure your display name, which modules are visible, and zones & stations.">
      {error && <p className="error">{error}</p>}
      <section>
        <h2 className="section-title">Settings</h2>
        <p className="section-desc">Configure your display name, which modules are visible, and <Link to="/zones">zones</Link> & <Link to="/stations">stations</Link>.</p>
        <div className="card" style={{ maxWidth: 480 }}>
          <div className="form-row">
            <label>User name</label>
            <input type="text" value={userName} onChange={e => setUserName(e.target.value)} placeholder="Display name" />
          </div>
          <div className="form-row" style={{ marginTop: '1.25rem' }}>
            <label style={{ marginBottom: '0.5rem' }}>Modules</label>
            <p className="section-desc" style={{ marginTop: 0, marginBottom: '0.75rem' }}>Toggle which sections appear in the sidebar.</p>
            <div style={{ display: 'flex', flexWrap: 'wrap', gap: '1rem 1.5rem' }}>
              {MODULE_KEYS.map(k => (
                <Toggle key={k} checked={!!modules[k]} onChange={v => setModules(m => ({ ...m, [k]: v }))} label={k} />
              ))}
            </div>
          </div>
          <div className="form-row">
            <label>Generator rate (per second)</label>
            <input type="number" step="0.1" min="0.1" value={s.generatorRatePerSecond} onChange={e => setS({ ...s, generatorRatePerSecond: Number(e.target.value) })} />
          </div>
          <div className="form-row">
            <label>Address mismatch %</label>
            <input type="number" step="0.01" min="0" max="1" value={s.addressMismatchProbability} onChange={e => setS({ ...s, addressMismatchProbability: Number(e.target.value) })} />
          </div>
          <div className="form-row">
            <label>Invalid postal %</label>
            <input type="number" step="0.01" min="0" max="1" value={s.invalidPostalProbability} onChange={e => setS({ ...s, invalidPostalProbability: Number(e.target.value) })} />
          </div>
          <div className="form-row">
            <label>Damaged label %</label>
            <input type="number" step="0.01" min="0" max="1" value={s.damagedLabelProbability} onChange={e => setS({ ...s, damagedLabelProbability: Number(e.target.value) })} />
          </div>
          <div className="form-row">
            <label>Dashboard window (minutes)</label>
            <input type="number" min="1" value={s.dashboardWindowMinutes} onChange={e => setS({ ...s, dashboardWindowMinutes: Number(e.target.value) })} />
          </div>
          <button className="btn" onClick={handleSave} disabled={saving}>{saving ? 'Saving…' : 'Save'}</button>
        </div>
      </section>
      <section>
        <h2 className="section-title">Generator</h2>
        <p className="section-desc">Status: {genRunning ? 'Running' : 'Stopped'} · Rate: {genRate}/s</p>
        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button className="btn" onClick={handleGenStart} disabled={genRunning}>Start</button>
          <button className="btn btn-ghost" onClick={handleGenStop} disabled={!genRunning}>Stop</button>
        </div>
      </section>
    </Layout>
  )
}

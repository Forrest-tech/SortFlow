import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { login, loginDev, setToken } from '../api/client'
import './Login.css'

export default function Login() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()

  async function handleLogin() {
    setLoading(true)
    setError(null)
    try {
      const { token } = await login(username || 'dev', password || 'dev')
      setToken(token)
      navigate('/dashboard')
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Login failed')
    } finally {
      setLoading(false)
    }
  }

  async function handleDevToken() {
    setLoading(true)
    setError(null)
    try {
      const { token } = await loginDev()
      setToken(token)
      navigate('/dashboard')
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Login failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-logo" />
        <h1 className="login-title">SortFlow</h1>
        <p className="login-desc">Sign in to access the dashboard.</p>
        <div className="login-form">
          <input type="text" placeholder="Username" value={username} onChange={e => setUsername(e.target.value)} />
          <input type="password" placeholder="Password" value={password} onChange={e => setPassword(e.target.value)} />
          <button className="btn btn-login" onClick={handleLogin} disabled={loading}>
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </div>
        <p className="login-dev">
          <button type="button" className="btn-ghost btn-dev" onClick={handleDevToken} disabled={loading}>
            Get dev token & sign in
          </button>
        </p>
        {error && <p className="error">{error}</p>}
      </div>
    </div>
  )
}

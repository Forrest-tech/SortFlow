import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { login, setToken } from '../api/client'
import './Login.css'

export default function Login() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  async function handleLogin() {
    setLoading(true)
    setError(null)
    try {
      const { token } = await login()
      setToken(token)
      navigate('/dashboard')
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Login failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="login">
      <h1>SortFlow</h1>
      <p>Log in with the dev token to continue.</p>
      <button className="btn" onClick={handleLogin} disabled={loading}>
        {loading ? 'Signing in…' : 'Get dev token & sign in'}
      </button>
      {error && <p className="error">{error}</p>}
    </div>
  )
}

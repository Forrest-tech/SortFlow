import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { login, setToken } from '../api/client'
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
      // 默认使用输入的账密，若未输入则退回 admin / Admin123!
      const user = username || 'admin'
      const pass = password || 'Admin123!'
      const { token } = await login(user, pass)
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
      // Dev 按钮直接一键带入正确的 Seed 管理员凭据并登录
      const { token } = await login('admin', 'Admin123!')
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
          <input 
            type="text" 
            placeholder="Username" 
            value={username} 
            onChange={e => setUsername(e.target.value)} 
          />
          <input 
            type="password" 
            placeholder="Password" 
            value={password} 
            onChange={e => setPassword(e.target.value)} 
          />
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

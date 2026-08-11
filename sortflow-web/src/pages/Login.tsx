import { useState, FormEvent, MouseEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { login, setToken } from '../api/client'
import './Login.css'

export default function Login() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const navigate = useNavigate()

  // 处理常规登录表单提交
  async function handleLogin(e: FormEvent) {
    e.preventDefault() // 关键：阻止表单提交导致页面刷新
    setLoading(true)
    setError(null)
    try {
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

  // 处理快捷 Dev 登录按钮点击
  async function handleDevToken(e: MouseEvent) {
    e.preventDefault() // 关键：阻止页面刷新
    setLoading(true)
    setError(null)
    try {
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
        
        {/* 用 form 包裹并使用 onSubmit 处理 */}
        <form className="login-form" onSubmit={handleLogin}>
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
          <button type="submit" className="btn btn-login" disabled={loading}>
            {loading ? 'Signing in…' : 'Sign in'}
          </button>
        </form>

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

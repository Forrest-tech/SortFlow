import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { setToken } from '../api/client'
import './Login.css'

export default function Login() {
  const navigate = useNavigate()

  useEffect(() => {
    // 自动写入 Token 并跳转到 Dashboard，彻底跳过登录界面
    setToken('dev-auto-token')
    navigate('/dashboard', { replace: true })
  }, [navigate])

  return (
    <div className="login-page" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', color: '#8b5cf6' }}>
      <h2>Loading Dashboard...</h2>
    </div>
  )
}

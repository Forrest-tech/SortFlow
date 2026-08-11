import { useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { login, setToken } from '../api/client'
import './Login.css'

export default function Login() {
  const navigate = useNavigate()

  useEffect(() => {
    async function autoLogin() {
      try {
        // 使用后端 AuthController.cs 定义的真实密码 changeme
        const { token } = await login('admin', 'changeme')
        if (token) {
          setToken(token)
        }
      } catch (e) {
        console.error('Auto login failed:', e)
      } finally {
        navigate('/dashboard', { replace: true })
      }
    }

    autoLogin()
  }, [navigate])

  return (
    <div className="login-page" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', color: '#8b5cf6' }}>
      <h2>Loading Dashboard...</h2>
    </div>
  )
}

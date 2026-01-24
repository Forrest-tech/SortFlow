import { Routes, Route, Navigate } from 'react-router-dom'
import { isAuthenticated } from './api/client'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Events from './pages/Events'
import Exceptions from './pages/Exceptions'
import Zones from './pages/Zones'
import Stations from './pages/Stations'
import './App.css'

function RequireAuth({ children }: { children: React.ReactNode }) {
  if (!isAuthenticated()) return <Navigate to="/" replace />
  return <>{children}</>
}

function App() {
  return (
    <div className="app">
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/dashboard" element={<RequireAuth><Dashboard /></RequireAuth>} />
        <Route path="/events" element={<RequireAuth><Events /></RequireAuth>} />
        <Route path="/exceptions" element={<RequireAuth><Exceptions /></RequireAuth>} />
        <Route path="/zones" element={<RequireAuth><Zones /></RequireAuth>} />
        <Route path="/stations" element={<RequireAuth><Stations /></RequireAuth>} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  )
}

export default App

import { Link, useLocation } from 'react-router-dom'
import { clearToken } from '../api/client'

const links = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/events', label: 'Events' },
  { to: '/exceptions', label: 'Exceptions' },
  { to: '/zones', label: 'Zones' },
  { to: '/stations', label: 'Stations' },
]

export default function Nav() {
  const location = useLocation()

  function handleLogout() {
    clearToken()
    window.location.href = '/'
  }

  return (
    <nav className="nav">
      {links.map(({ to, label }) => (
        <Link key={to} to={to} className={location.pathname === to ? 'active' : ''}>
          {label}
        </Link>
      ))}
      <button type="button" className="btn btn-ghost" onClick={handleLogout}>
        Log out
      </button>
    </nav>
  )
}

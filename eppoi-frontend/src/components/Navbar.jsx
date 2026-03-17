import { useNavigate, useLocation } from 'react-router-dom';
import './Navbar.css';

export default function Navbar() {
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    navigate('/login');
  };

  const links = [
    { path: '/home', label: 'Home' },
    { path: '/events', label: 'Events' },
    { path: '/news', label: 'News' },
    { path: '/organizations', label: 'Organizations' },
  ];

  return (
    <nav className="navbar">
      <div className="navbar-brand" onClick={() => navigate('/home')}>
        Eppoi
      </div>
      <div className="navbar-links">
        {links.map((link) => (
          <span
            key={link.path}
            // Highlight the active link based on the current route
            className={`navbar-link ${location.pathname === link.path ? 'active' : ''}`}
            onClick={() => navigate(link.path)}
          >
            {link.label}
          </span>
        ))}
      </div>
      <button className="navbar-logout" onClick={handleLogout}>
        Logout
      </button>
    </nav>
  );
}

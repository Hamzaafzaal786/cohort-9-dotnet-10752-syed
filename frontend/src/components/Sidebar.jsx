import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { 
  FaHome, 
  FaTasks, 
  FaUser, 
  FaSignOutAlt,
  FaClipboardList,
  FaChartPie,
  FaCog
} from 'react-icons/fa';

const Sidebar = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const isAdmin = user?.role === 'Admin' || user?.role === 'admin';

  const menuItems = [
    { path: '/dashboard', icon: <FaChartPie />, label: 'Dashboard' },
    { path: '/tasks', icon: <FaTasks />, label: 'Tasks' },
    { path: '/profile', icon: <FaUser />, label: 'Profile' },
  ];

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    logout();
    window.location.href = '/login';
  };

  return (
    <div className="sidebar">
      <div className="sidebar-brand">
        <FaClipboardList className="brand-icon" />
        <span>TaskFlow</span>
      </div>
      <nav className="sidebar-nav">
        {menuItems.map((item) => (
          <Link
            key={item.path}
            to={item.path}
            className={`sidebar-link ${location.pathname === item.path ? 'active' : ''}`}
          >
            {item.icon}
            <span>{item.label}</span>
          </Link>
        ))}
        <button className="sidebar-link logout-btn" onClick={handleLogout}>
          <FaSignOutAlt />
          <span>Logout</span>
        </button>
      </nav>
      <div className="sidebar-footer">
        <small>{user?.fullName || user?.email}</small>
        {isAdmin && <span className="admin-badge">Admin</span>}
      </div>
    </div>
  );
};

export default Sidebar;
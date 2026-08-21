import React, { useState, useEffect } from 'react';
import { TaskAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';
import { 
  FaClipboardList, 
  FaCheckCircle, 
  FaSpinner, 
  FaClock, 
  FaExclamationTriangle,
  FaCrown
} from 'react-icons/fa';

const Dashboard = () => {
  const [stats, setStats] = useState(null);
  const [recentTasks, setRecentTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();

  const isAdmin = user?.role === 'Admin' || user?.role === 'admin';

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      const statsResponse = await TaskAPI.getDashboardStats();
      setStats(statsResponse.data);

      const tasksResponse = await TaskAPI.getAll();
      const sortedTasks = tasksResponse.data
        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
        .slice(0, 5);
      setRecentTasks(sortedTasks);
    } catch (error) {
      console.error('Error fetching dashboard data:', error);
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status) => {
    const variants = {
      0: 'status-pending',
      1: 'status-progress',
      2: 'status-completed',
      3: 'status-cancelled',
    };
    const labels = {
      0: 'Pending',
      1: 'In Progress',
      2: 'Completed',
      3: 'Cancelled',
    };
    return <span className={`status-badge ${variants[status]}`}>{labels[status]}</span>;
  };

  const getPriorityBadge = (priority) => {
    const variants = {
      0: 'priority-low',
      1: 'priority-medium',
      2: 'priority-high',
      3: 'priority-urgent',
    };
    const labels = {
      0: 'Low',
      1: 'Medium',
      2: 'High',
      3: 'Urgent',
    };
    return <span className={`priority-badge ${variants[priority]}`}>{labels[priority]}</span>;
  };

  if (loading) {
    return <LoadingSpinner />;
  }

  const statCards = [
    { 
      label: 'Total Tasks', 
      value: stats?.totalTasks || 0, 
      icon: <FaClipboardList />, 
      color: 'stat-primary' 
    },
    { 
      label: 'Completed', 
      value: stats?.completedTasks || 0, 
      icon: <FaCheckCircle />, 
      color: 'stat-success' 
    },
    { 
      label: 'In Progress', 
      value: stats?.inProgressTasks || 0, 
      icon: <FaSpinner />, 
      color: 'stat-warning' 
    },
    { 
      label: 'Pending', 
      value: stats?.pendingTasks || 0, 
      icon: <FaClock />, 
      color: 'stat-danger' 
    },
  ];

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Dashboard</h1>
          <p className="page-subtitle">
            Welcome back, {user?.fullName || 'User'}!
            {isAdmin && (
              <span className="admin-badge" style={{ marginLeft: '12px' }}>
                <FaCrown /> Admin
              </span>
            )}
          </p>
        </div>
      </div>

      {/* Stats Cards */}
      <div className="stats-grid">
        {statCards.map((stat, index) => (
          <div key={index} className={`stat-card ${stat.color}`}>
            <div className="stat-icon">{stat.icon}</div>
            <div className="stat-number">{stat.value}</div>
            <div className="stat-label">{stat.label}</div>
          </div>
        ))}
      </div>

      {/* Admin Overview */}
      {isAdmin && (
        <div className="glass-card admin-overview">
          <h3>👑 Admin Overview</h3>
          <p>You have full access to all tasks across all users.</p>
          <div className="admin-stats">
            <span>Total Tasks in System: <strong>{stats?.totalTasks || 0}</strong></span>
          </div>
        </div>
      )}

      {/* Overdue Tasks */}
      <div className="glass-card overdue-card">
        <div className="overdue-header">
          <FaExclamationTriangle className="overdue-icon" />
          <h3>Overdue Tasks</h3>
        </div>
        <div className="overdue-number">{stats?.overdueTasks || 0}</div>
        <p className="overdue-text">Tasks past their due date</p>
      </div>

      {/* Recent Tasks */}
      <div className="glass-card" style={{ marginTop: '24px' }}>
        <h3 style={{ marginBottom: '16px' }}>Recent Tasks</h3>
        {recentTasks.length === 0 ? (
          <p className="text-muted">No tasks found</p>
        ) : (
          <div className="table-responsive">
            <table className="tasks-table">
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Status</th>
                  <th>Priority</th>
                  <th>Due Date</th>
                  {isAdmin && <th>Assigned To</th>}
                </tr>
              </thead>
              <tbody>
                {recentTasks.map((task) => (
                  <tr key={task.id} className="task-row">
                    <td className="task-title">{task.title}</td>
                    <td>{getStatusBadge(task.status)}</td>
                    <td>{getPriorityBadge(task.priority)}</td>
                    <td>{new Date(task.dueDate).toLocaleDateString()}</td>
                    {isAdmin && <td>{task.userName || 'Unassigned'}</td>}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
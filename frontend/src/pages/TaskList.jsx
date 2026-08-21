import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { TaskAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';
import { FaPlus, FaEye, FaEdit, FaTrash } from 'react-icons/fa';

const TaskList = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin' || user?.role === 'admin';

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try {
      const response = await TaskAPI.getAll();
      setTasks(response.data);
    } catch (error) {
      console.error('Error fetching tasks:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this task?')) {
      try {
        await TaskAPI.delete(id);
        setTasks(tasks.filter(task => task.id !== id));
      } catch (error) {
        console.error('Error deleting task:', error);
      }
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

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">Tasks</h1>
          <p className="page-subtitle">Manage your tasks and track progress</p>
        </div>
        <Link to="/tasks/new" className="btn-primary-custom">
          <FaPlus /> New Task
        </Link>
      </div>

      <div className="glass-card" style={{ padding: '0', overflow: 'hidden' }}>
        <div className="table-responsive">
          <table className="tasks-table">
            <thead>
              <tr>
                <th>Title</th>
                <th>Priority</th>
                <th>Status</th>
                <th>Due Date</th>
                {isAdmin && <th>Assigned To</th>}
                <th className="text-center">Actions</th>
              </tr>
            </thead>
            <tbody>
              {tasks.length === 0 ? (
                <tr>
                  <td colSpan={isAdmin ? 6 : 5} className="empty-state">
                    <div className="empty-icon">📋</div>
                    <p>No tasks found</p>
                    <span>Create your first task to get started</span>
                  </td>
                </tr>
              ) : (
                tasks.map((task) => (
                  <tr key={task.id} className="task-row">
                    <td>
                      <div className="task-title">{task.title}</div>
                      {task.category && (
                        <span className="task-category">{task.category}</span>
                      )}
                    </td>
                    <td>{getPriorityBadge(task.priority)}</td>
                    <td>{getStatusBadge(task.status)}</td>
                    <td>{new Date(task.dueDate).toLocaleDateString()}</td>
                    {isAdmin && <td>{task.userName || 'Unassigned'}</td>}
                    <td>
                      <div className="action-buttons">
                        <Link to={`/tasks/${task.id}`} className="action-btn view-btn" title="View">
                          <FaEye />
                        </Link>
                        <Link to={`/tasks/${task.id}/edit`} className="action-btn edit-btn" title="Edit">
                          <FaEdit />
                        </Link>
                        <button 
                          className="action-btn delete-btn" 
                          onClick={() => handleDelete(task.id)}
                          title="Delete"
                        >
                          <FaTrash />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default TaskList;
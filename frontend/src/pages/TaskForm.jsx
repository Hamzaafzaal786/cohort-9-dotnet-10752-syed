import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { TaskAPI, UserAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';
import { FaSave, FaTimes, FaUserCheck, FaClipboardList } from 'react-icons/fa';

const TaskForm = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin' || user?.role === 'admin';

  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(true);
  const [error, setError] = useState('');
  const [users, setUsers] = useState([]);
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    priority: 1,
    status: 0,
    dueDate: '',
    category: '',
    userId: ''
  });

  const isEditMode = !!id;

  useEffect(() => {
    const loadData = async () => {
      setFetching(true);
      try {
        if (isEditMode) {
          await fetchTask();
        }
        if (isAdmin) {
          await fetchUsers();
        }
      } catch (error) {
        console.error('Error loading data:', error);
      } finally {
        setFetching(false);
      }
    };
    loadData();
  }, [id]);

  const fetchTask = async () => {
    try {
      const response = await TaskAPI.getById(id);
      const task = response.data;
      setFormData({
        title: task.title || '',
        description: task.description || '',
        priority: task.priority !== undefined ? task.priority : 1,
        status: task.status !== undefined ? task.status : 0,
        dueDate: task.dueDate ? task.dueDate.split('T')[0] : '',
        category: task.category || '',
        userId: task.userId || ''
      });
    } catch (error) {
      setError('Failed to load task');
      console.error(error);
    }
  };

  const fetchUsers = async () => {
    try {
      const response = await UserAPI.getAll();
      setUsers(response.data);
    } catch (error) {
      console.error('Error fetching users:', error);
      setUsers([{ id: user.id, fullName: user.fullName || user.email }]);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      // Build data with proper types
      const data = {
        title: formData.title.trim(),
        description: formData.description?.trim() || '',
        priority: parseInt(formData.priority),
        status: parseInt(formData.status),  // ✅ Convert to integer
        dueDate: formData.dueDate ? new Date(formData.dueDate).toISOString() : new Date().toISOString(),
        category: formData.category?.trim() || '',
        userId: formData.userId || null
      };

      if (isEditMode) {
        const updateData = { ...data, id: parseInt(id) };
        await TaskAPI.update(updateData);
      } else {
        await TaskAPI.create(data);
      }
      navigate('/tasks');
    } catch (err) {
      console.error('Error response:', err.response?.data);
      setError(err.response?.data?.message || 'Failed to save task');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) {
    return <LoadingSpinner />;
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">
            {isEditMode ? 'Edit Task' : 'Create New Task'}
          </h1>
          <p className="page-subtitle">
            {isEditMode ? 'Update task details' : 'Add a new task to your list'}
          </p>
        </div>
      </div>

      <div className="glass-card">
        {error && <div className="auth-error">{error}</div>}

        <form onSubmit={handleSubmit} className="task-form">
          <div className="form-row">
            <div className="form-group full-width">
              <label htmlFor="title">Title *</label>
              <div className="input-wrapper">
                <FaClipboardList className="input-icon" />
                <input
                  id="title"
                  name="title"
                  type="text"
                  placeholder="Enter task title"
                  value={formData.title}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group full-width">
              <label htmlFor="description">Description</label>
              <textarea
                id="description"
                name="description"
                rows="4"
                placeholder="Describe the task in detail..."
                value={formData.description}
                onChange={handleChange}
                className="task-textarea"
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="priority">Priority</label>
              <select
                id="priority"
                name="priority"
                value={formData.priority}
                onChange={handleChange}
                className="auth-select"
              >
                <option value={0}>Low</option>
                <option value={1}>Medium</option>
                <option value={2}>High</option>
                <option value={3}>Urgent</option>
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="status">Status</label>
              <select
                id="status"
                name="status"
                value={formData.status}
                onChange={handleChange}
                className="auth-select"
              >
                <option value={0}>Pending</option>
                <option value={1}>In Progress</option>
                <option value={2}>Completed</option>
                <option value={3}>Cancelled</option>
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="dueDate">Due Date *</label>
              <input
                id="dueDate"
                name="dueDate"
                type="date"
                value={formData.dueDate}
                onChange={handleChange}
                className="auth-select"
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="category">Category</label>
              <input
                id="category"
                name="category"
                type="text"
                placeholder="e.g., Work, Personal, Urgent"
                value={formData.category}
                onChange={handleChange}
                className="auth-select"
              />
            </div>
          </div>

          {/* Admin: Assign to User */}
          {isAdmin && (
            <div className="form-row">
              <div className="form-group full-width">
                <label htmlFor="userId">Assign To</label>
                <div className="input-wrapper">
                  <FaUserCheck className="input-icon" />
                  <select
                    id="userId"
                    name="userId"
                    value={formData.userId}
                    onChange={handleChange}
                    className="auth-select"
                  >
                    <option value="">Select User</option>
                    {users.map(u => (
                      <option key={u.id} value={u.id}>
                        {u.fullName || u.email}
                      </option>
                    ))}
                  </select>
                </div>
                <p className="role-hint">Leave blank to assign to yourself</p>
              </div>
            </div>
          )}

          <div className="form-actions">
            <button type="submit" className="auth-btn" disabled={loading}>
              <FaSave />
              {loading ? 'Saving...' : (isEditMode ? 'Update Task' : 'Create Task')}
            </button>
            <button type="button" className="auth-btn cancel-btn" onClick={() => navigate('/tasks')}>
              <FaTimes />
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default TaskForm;
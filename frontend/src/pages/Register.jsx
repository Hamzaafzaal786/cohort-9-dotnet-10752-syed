import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import { 
  FaUser, 
  FaEnvelope, 
  FaLock, 
  FaUserPlus, 
  FaSpinner, 
  FaUserCog, 
  FaInfoCircle,
  FaCheckCircle,
  FaTimesCircle
} from 'react-icons/fa';

const Register = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [role, setRole] = useState('User');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const [passwordErrors, setPasswordErrors] = useState([]);

  const requirements = [
    { id: 'length', label: 'At least 6 characters', test: (pwd) => pwd.length >= 6 },
    { id: 'uppercase', label: 'One uppercase letter', test: (pwd) => /[A-Z]/.test(pwd) },
    { id: 'lowercase', label: 'One lowercase letter', test: (pwd) => /[a-z]/.test(pwd) },
    { id: 'number', label: 'One number', test: (pwd) => /[0-9]/.test(pwd) },
    { id: 'special', label: 'One special character (!@#$%^&*)', test: (pwd) => /[!@#$%^&*(),.?":{}|<>]/.test(pwd) },
  ];

  const validatePassword = (pwd) => {
    const errors = [];
    if (!requirements[0].test(pwd)) errors.push(requirements[0].label);
    if (!requirements[1].test(pwd)) errors.push(requirements[1].label);
    if (!requirements[2].test(pwd)) errors.push(requirements[2].label);
    if (!requirements[3].test(pwd)) errors.push(requirements[3].label);
    if (!requirements[4].test(pwd)) errors.push(requirements[4].label);
    return errors;
  };

  const handlePasswordChange = (e) => {
    const newPassword = e.target.value;
    setPassword(newPassword);
    setPasswordErrors(validatePassword(newPassword));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    setLoading(true);

    try {
      const response = await AuthAPI.register({ 
        email, 
        password, 
        fullName, 
        role 
      });
      const { token, user } = response.data;
      login(token, user);
      navigate('/dashboard');
    } catch (err) {
      const errorMessage = err.response?.data?.message || 'Registration failed. Please try again.';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const passwordRequirements = requirements.map(req => ({
    ...req,
    met: passwordErrors.length === 0 || !passwordErrors.includes(req.label)
  }));

  return (
    <div className="auth-container register-container">
      <div className="auth-card register-card">
        <div className="auth-header">
          <h1>Create Account</h1>
          <p>Join TaskFlow and start managing your tasks</p>
        </div>

        {error && <div className="auth-error">{error}</div>}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="fullName">Full Name</label>
            <div className="input-wrapper">
              <FaUser className="input-icon" />
              <input
                id="fullName"
                type="text"
                placeholder="Enter your full name"
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="email">Email Address</label>
            <div className="input-wrapper">
              <FaEnvelope className="input-icon" />
              <input
                id="email"
                type="email"
                placeholder="Enter your email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <div className="input-wrapper">
              <FaLock className="input-icon" />
              <input
                id="password"
                type="password"
                placeholder="Min 6 characters"
                value={password}
                onChange={handlePasswordChange}
                required
                minLength={6}
              />
            </div>
            {password && (
              <div className="password-hint">
                <div className="hint-header">
                  <FaInfoCircle className="hint-icon" />
                  <span>Password requirements:</span>
                </div>
                <ul className="password-requirements">
                  {passwordRequirements.map((req, index) => (
                    <li 
                      key={index} 
                      className={`requirement-item ${req.met ? 'requirement-met' : 'requirement-unmet'}`}
                    >
                      {req.met ? (
                        <FaCheckCircle className="req-icon success" />
                      ) : (
                        <FaTimesCircle className="req-icon danger" />
                      )}
                      {req.label}
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirm Password</label>
            <div className="input-wrapper">
              <FaLock className="input-icon" />
              <input
                id="confirmPassword"
                type="password"
                placeholder="Confirm your password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
            </div>
            {confirmPassword && password !== confirmPassword && (
              <div className="password-mismatch">
                <FaTimesCircle className="status-icon" /> Passwords do not match
              </div>
            )}
            {confirmPassword && password === confirmPassword && (
              <div className="password-match">
                <FaCheckCircle className="status-icon" /> Passwords match
              </div>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="role">Role</label>
            <div className="input-wrapper">
              <FaUserCog className="input-icon" />
              <select
                id="role"
                value={role}
                onChange={(e) => setRole(e.target.value)}
                className="auth-select"
              >
                <option value="User">User</option>
                <option value="Admin">Admin</option>
              </select>
            </div>
            <p className="role-hint">Admin users can manage all tasks and users</p>
          </div>

          <button type="submit" className="auth-btn register-btn" disabled={loading}>
            {loading ? <FaSpinner className="spinner" /> : <FaUserPlus />}
            {loading ? 'Creating Account...' : 'Create Account'}
          </button>
        </form>

        <div className="auth-footer">
          <p>Already have an account? <Link to="/login">Sign In</Link></p>
        </div>
      </div>
    </div>
  );
};

export default Register;
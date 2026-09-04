import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';
import { 
  FaUser, 
  FaEnvelope, 
  FaUserTag, 
  FaCalendarAlt, 
  FaLock, 
  FaSave,
  FaTimes,
  FaShieldAlt,
  FaCheckCircle,
  FaExclamationTriangle
} from 'react-icons/fa';
import API from '../services/api';

const Profile = () => {
  const { user } = useAuth();
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPasswordForm, setShowPasswordForm] = useState(false);

  const isAdmin = user?.role === 'Admin' || user?.role === 'admin';

  const handleChangePassword = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');

    // ✅ Check if passwords match
    if (newPassword !== confirmPassword) {
      setError('❌ New passwords do not match');
      return;
    }

    // ✅ Check password length
    if (newPassword.length < 6) {
      setError('❌ Password must be at least 6 characters');
      return;
    }

    // ✅ Check current password is not empty
    if (!currentPassword) {
      setError('❌ Please enter your current password');
      return;
    }

    setLoading(true);

    try {
      await API.post('/auth/change-password', {
        currentPassword,
        newPassword
      });
      
      // ✅ Success message
      setMessage('✅ Password changed successfully!');
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
      
      // Auto-hide success message after 5 seconds
      setTimeout(() => setMessage(''), 5000);
    } catch (err) {
      // ✅ Show error from backend
      const errorMsg = err.response?.data?.message || 'Failed to change password';
      
      if (errorMsg.includes('Incorrect password') || errorMsg.includes('current password')) {
        setError('❌ Current password is incorrect');
      } else if (errorMsg.includes('PasswordTooShort')) {
        setError('❌ Password must be at least 6 characters');
      } else if (errorMsg.includes('PasswordRequiresUpper')) {
        setError('❌ Password must contain at least one uppercase letter');
      } else if (errorMsg.includes('PasswordRequiresLower')) {
        setError('❌ Password must contain at least one lowercase letter');
      } else if (errorMsg.includes('PasswordRequiresDigit')) {
        setError('❌ Password must contain at least one number');
      } else if (errorMsg.includes('PasswordRequiresNonAlphanumeric')) {
        setError('❌ Password must contain at least one special character (!@#$%^&*)');
      } else {
        setError('❌ ' + errorMsg);
      }
      
      // Auto-hide error after 5 seconds
      setTimeout(() => setError(''), 5000);
    } finally {
      setLoading(false);
    }
  };

  if (!user) {
    return <LoadingSpinner />;
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <div>
          <h1 className="page-title">My Profile</h1>
          <p className="page-subtitle">Manage your account details and security</p>
        </div>
        {isAdmin && (
          <span className="admin-badge" style={{ fontSize: '14px', padding: '6px 18px' }}>
            <FaShieldAlt /> Admin
          </span>
        )}
      </div>

      <div className="profile-grid">
        {/* Profile Info Card */}
        <div className="glass-card profile-info-card">
          <div className="profile-avatar">
            <div className="avatar-circle">
              {user?.fullName?.charAt(0) || user?.email?.charAt(0) || 'U'}
            </div>
            {isAdmin && <div className="avatar-badge">Admin</div>}
          </div>

          <div className="profile-details">
            <div className="profile-detail-item">
              <FaUser className="detail-icon" />
              <div>
                <span className="detail-label">Full Name</span>
                <span className="detail-value">{user?.fullName || 'N/A'}</span>
              </div>
            </div>

            <div className="profile-detail-item">
              <FaEnvelope className="detail-icon" />
              <div>
                <span className="detail-label">Email</span>
                <span className="detail-value">{user?.email || 'N/A'}</span>
              </div>
            </div>

            <div className="profile-detail-item">
              <FaUserTag className="detail-icon" />
              <div>
                <span className="detail-label">Role</span>
                <span className="detail-value">
                  {isAdmin ? (
                    <span className="role-admin">Administrator</span>
                  ) : (
                    <span className="role-user">User</span>
                  )}
                </span>
              </div>
            </div>

            <div className="profile-detail-item">
              <FaCalendarAlt className="detail-icon" />
              <div>
                <span className="detail-label">Joined</span>
                <span className="detail-value">
                  {user?.createdAt ? new Date(user.createdAt).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                  }) : 'N/A'}
                </span>
              </div>
            </div>
          </div>
        </div>

        {/* Security Card */}
        <div className="glass-card security-card">
          <div className="security-header">
            <FaLock className="security-icon" />
            <h3>Security</h3>
          </div>

          {!showPasswordForm ? (
            <div className="security-actions">
              <p>Keep your account secure by changing your password regularly.</p>
              <button 
                className="btn-primary-custom" 
                onClick={() => setShowPasswordForm(true)}
              >
                <FaLock /> Change Password
              </button>
            </div>
          ) : (
            <form onSubmit={handleChangePassword} className="password-form">
              {/* ✅ Success Message */}
              {message && (
                <div className="auth-success" style={{ 
                  background: '#E8F5E9', 
                  color: '#1B5E20', 
                  padding: '12px 16px', 
                  borderRadius: '12px', 
                  marginBottom: '16px',
                  borderLeft: '4px solid #00C9A7'
                }}>
                  {message}
                </div>
              )}
              
              {/* ✅ Error Message */}
              {error && (
                <div className="auth-error" style={{ 
                  background: '#FBE9E7', 
                  color: '#BF360C', 
                  padding: '12px 16px', 
                  borderRadius: '12px', 
                  marginBottom: '16px',
                  borderLeft: '4px solid #FF6B6B'
                }}>
                  {error}
                </div>
              )}

              <div className="form-group">
                <label htmlFor="currentPassword">Current Password</label>
                <div className="input-wrapper">
                  <FaLock className="input-icon" />
                  <input
                    id="currentPassword"
                    type="password"
                    placeholder="Enter current password"
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="newPassword">New Password</label>
                <div className="input-wrapper">
                  <FaLock className="input-icon" />
                  <input
                    id="newPassword"
                    type="password"
                    placeholder="Min 6 characters"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    required
                    minLength={6}
                  />
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="confirmPassword">Confirm New Password</label>
                <div className="input-wrapper">
                  <FaCheckCircle className="input-icon" />
                  <input
                    id="confirmPassword"
                    type="password"
                    placeholder="Confirm new password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="form-actions">
                <button type="submit" className="auth-btn" disabled={loading}>
                  <FaSave />
                  {loading ? 'Updating...' : 'Update Password'}
                </button>
                <button 
                  type="button" 
                  className="auth-btn cancel-btn" 
                  onClick={() => {
                    setShowPasswordForm(false);
                    setError('');
                    setMessage('');
                    setCurrentPassword('');
                    setNewPassword('');
                    setConfirmPassword('');
                  }}
                >
                  <FaTimes />
                  Cancel
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
};

export default Profile;
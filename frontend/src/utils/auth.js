// Save user and token to localStorage
export const setAuthData = (token, user) => {
  localStorage.setItem('token', token);
  localStorage.setItem('user', JSON.stringify(user));
};

// Get token from localStorage
export const getToken = () => {
  return localStorage.getItem('token');
};

// Get user from localStorage
export const getUser = () => {
  const user = localStorage.getItem('user');
  return user ? JSON.parse(user) : null;
};

// Check if user is logged in
export const isAuthenticated = () => {
  return !!getToken();
};

// Clear auth data on logout
export const clearAuthData = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
};

// Get auth headers for API calls
export const getAuthHeaders = () => {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
};
import axios from 'axios';

const API = axios.create({
  baseURL: 'https://localhost:52994/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

API.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

API.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const AuthAPI = {
  login: (data) => API.post('/auth/login', data),
  register: (data) => API.post('/auth/register', data),
  logout: () => API.post('/auth/logout'),
  refreshToken: (refreshToken) => API.post('/auth/refresh-token', refreshToken),
};

export const TaskAPI = {
  getAll: () => API.get('/tasks'),
  getById: (id) => API.get(`/tasks/${id}`),
  create: (data) => API.post('/tasks', data),
  update: (data) => API.put('/tasks', data),
  delete: (id) => API.delete(`/tasks/${id}`),
  getByStatus: (status) => API.get(`/tasks/status/${status}`),
  getDashboardStats: () => API.get('/tasks/dashboard/stats'),
};

// NEW: User API calls
export const UserAPI = {
  getAll: () => API.get('/user'),
};

export default API;
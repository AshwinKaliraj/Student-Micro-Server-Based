import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

// ✅ Create separate axios instance for AuthService
const authApi = axios.create({
  baseURL: 'http://localhost:5130',
  headers: {
    'Content-Type': 'application/json',
  },
});

export const authService = {
  login: async (email, password) => {
    try {
      const response = await authApi.post('/api/Auth/login', { email, password });
      
      // ✅ FIXED: The backend wraps data in response.data.data
      const responseData = response.data;
      
      // Check if response is successful
      if (!responseData.success || !responseData.data) {
        throw new Error(responseData.message || 'Login failed');
      }
      
      // ✅ Extract the actual data from the nested structure
      const data = responseData.data;
      
      // Check if token exists
      if (!data.token) {
        throw new Error('No token received');
      }
      
      localStorage.setItem('token', data.token);
      localStorage.setItem('user', JSON.stringify({
        email: data.email,
        role: data.role,
        name: data.name,
        userId: data.userId
      }));
      
      return data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  },

  register: async (userData) => {
    try {
      const response = await authApi.post('/api/Auth/register', userData);
      return response.data;
    } catch (error) {
      console.error('Register error:', error);
      throw error;
    }
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated: () => {
    const token = localStorage.getItem('token');
    if (!token) return false;

    try {
      const decoded = jwtDecode(token);
      return decoded.exp * 1000 > Date.now();
    } catch (error) {
      return false;
    }
  },

  isTeacher: () => {
    const user = authService.getCurrentUser();
    return user?.role === 'Teacher';
  },

  isStudent: () => {
    const user = authService.getCurrentUser();
    return user?.role === 'Student';
  }
};

export default authApi;

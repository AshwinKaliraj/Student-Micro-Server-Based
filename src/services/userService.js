import axios from 'axios';

const userApi = axios.create({
  baseURL: 'http://localhost:7080/api',  // Direct to UserService
});

userApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const userService = {
  getAllUsers: async () => {
    const response = await userApi.get('/users');
    return response.data;
  },

  getUserById: async (id) => {
    const response = await userApi.get(`/users/${id}`);
    return response.data;
  },

  createUser: async (userData) => {
    const response = await userApi.post('/users', userData);
    return response.data;
  },

  updateUser: async (id, userData) => {
    const response = await userApi.put(`/users/${id}`, userData);
    return response.data;
  },

  deleteUser: async (id) => {
    const response = await userApi.delete(`/users/${id}`);
    return response.data;
  }
};

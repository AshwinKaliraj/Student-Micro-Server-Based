import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import './UsersList.css';

const UsersList = () => {
  const navigate = useNavigate();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/api/users');
      setUsers(response.data);
      setLoading(false);
    } catch (err) {
      console.error('Fetch users error:', err);
      setError('Failed to fetch users. Make sure UserService is running on port 7080.');
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      try {
        await api.delete(`/api/users/${id}`);
        fetchUsers(); // Refresh list
      } catch (err) {
        alert('Failed to delete user');
      }
    }
  };

  if (loading) return <div className="loading">Loading...</div>;
  if (error) return <div className="error-container"><div className="error-message">{error}</div></div>;

  return (
    <div className="users-container">
      <div className="users-header">
        <h2>User Management</h2>
        <button 
          className="btn-primary" 
          onClick={() => navigate('/users/create')}
        >
          ➕ Create New User
        </button>
      </div>

      {users.length === 0 ? (
        <div className="no-users">No users found. Create your first user!</div>
      ) : (
        <table className="user-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>Designation</th>
              <th>Date of Birth</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.map(user => (
              <tr key={user.id}>
                <td>{user.id}</td>
                <td>{user.name}</td>
                <td>{user.email}</td>
                <td>{user.designation}</td>
                <td>{user.dateOfBirth ? new Date(user.dateOfBirth).toLocaleDateString() : 'N/A'}</td>
                <td>
                  <button 
                    className="btn-edit" 
                    onClick={() => navigate(`/users/update/${user.id}`)}
                  >
                    Edit
                  </button>
                  <button 
                    className="btn-delete" 
                    onClick={() => handleDelete(user.id)}
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default UsersList;

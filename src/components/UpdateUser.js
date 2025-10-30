import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import './UpdateUser.css';

function UpdateUser() {
  const navigate = useNavigate();
  const { id } = useParams();
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '', // Leave blank, don't send old hashed password
    role: 'Student',
    dateOfBirth: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchUser();
  }, [id]);

  const fetchUser = async () => {
    try {
      const response = await fetch(`http://localhost:7080/api/users/${id}`);
      if (response.ok) {
        const data = await response.json();
        setFormData({
          name: data.name || '',
          email: data.email || '',
          password: '', // DON'T populate with hashed password
          role: data.role || 'Student',
          dateOfBirth: data.dateOfBirth ? data.dateOfBirth.split('T')[0] : ''
        });
      } else {
        setError('Failed to load user data');
      }
    } catch (error) {
      console.error('Error fetching user:', error);
      setError('Failed to load user data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // Prepare update data - only send password if it's been changed
    const updateData = {
      name: formData.name,
      email: formData.email,
      role: formData.role,
      dateOfBirth: formData.dateOfBirth || null
    };

    // Only include password if user entered a new one
    if (formData.password && formData.password.trim() !== '') {
      updateData.password = formData.password;
    }

    console.log('📤 Sending update data:', updateData);

    try {
      const response = await fetch(`http://localhost:7080/api/users/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(updateData),
      });

      if (response.ok) {
        const result = await response.json();
        console.log('✅ Update successful:', result);
        navigate('/users');
      } else {
        const errorData = await response.text();
        console.error('❌ Update failed:', errorData);
        setError('Failed to update user');
      }
    } catch (error) {
      console.error('❌ Error updating user:', error);
      setError('Failed to update user. Please try again.');
    }
  };

  if (loading) {
    return (
      <div className="update-user-container">
        <div className="loading">Loading user data...</div>
      </div>
    );
  }

  return (
    <div className="update-user-container">
      <div className="update-user-card">
        <h2>Update User</h2>
        
        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Name</label>
            <input
              type="text"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
              placeholder="Enter full name"
            />
          </div>

          <div className="form-group">
            <label>Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              placeholder="Enter email address"
            />
          </div>

          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Leave blank to keep current password"
            />
          </div>

          <div className="form-group">
            <label>Role</label>
            <select
              name="role"
              value={formData.role}
              onChange={handleChange}
              required
            >
              <option value="Student">Student</option>
              <option value="Teacher">Teacher</option>
            </select>
          </div>

          <div className="form-group">
            <label>Date of Birth</label>
            <input
              type="date"
              name="dateOfBirth"
              value={formData.dateOfBirth}
              onChange={handleChange}
            />
          </div>

          <div className="button-group">
            <button type="submit" className="btn-submit">
              Update User
            </button>
            <button
              type="button"
              className="btn-cancel"
              onClick={() => navigate('/users')}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default UpdateUser;

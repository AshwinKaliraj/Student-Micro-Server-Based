import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';  // ✅ Import authService
import './Dashboard.css';

const Dashboard = () => {
  const navigate = useNavigate();
  const [userName, setUserName] = useState('');
  const [userRole, setUserRole] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // ✅ Check if user is authenticated
    if (!authService.isAuthenticated()) {
      navigate('/login');
      return;
    }

    // ✅ Get current user
    const user = authService.getCurrentUser();
    
    if (!user) {
      navigate('/login');
      return;
    }

    setUserName(user.name || user.email || 'User');
    setUserRole(user.role || 'Student');
    setLoading(false);
  }, [navigate]);

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h1>Student Management System</h1>
        <div className="user-info">
          <span>{userName}</span>
          <span className="role-badge">{userRole}</span>
          <button onClick={handleLogout} className="logout-btn">Logout</button>
        </div>
      </div>

      <div className="dashboard-content">
        <div className="welcome-card">
          <h2>Welcome, {userName}!</h2>
          <p>You are logged in as a {userRole}</p>
        </div>

        <div className="action-cards">
          <div className="card" onClick={() => navigate('/users')}>
            <h3>📋 View Users</h3>
            <p>See all registered users in the system</p>
          </div>

          {userRole === 'Teacher' && (
            <div className="card" onClick={() => navigate('/users/create')}>
              <h3>➕ Create User</h3>
              <p>Add a new user to the system</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;

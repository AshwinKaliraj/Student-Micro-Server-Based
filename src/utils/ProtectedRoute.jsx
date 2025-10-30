import { Navigate } from 'react-router-dom';
import { authService } from '../services/authService';

const ProtectedRoute = ({ children, requireTeacher = false }) => {
  if (!authService.isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }

  if (requireTeacher && !authService.isTeacher()) {
    return <Navigate to="/dashboard" replace />;
  }

  return children;
};

export default ProtectedRoute;

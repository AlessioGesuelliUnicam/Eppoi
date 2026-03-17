import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import VerifyEmail from './components/VerifyEmail';
import Navbar from './components/Navbar';

/**
 * Wrapper component for routes that require authentication.
 * Checks for a JWT token in localStorage — if missing, redirects to login.
 * Automatically renders the Navbar above the page content so it doesn't
 * need to be imported in every protected page.
 */
function ProtectedRoute({ children }) {
  const token = localStorage.getItem('authToken');
  if (!token) return <Navigate to="/login" replace />;

  return (
    <>
      <Navbar />
      {children}
    </>
  );
}

function App() {
  return (
    <Router>
      <Routes>
        {/* Public routes — accessible without authentication */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/verify-email" element={<VerifyEmail />} />

        {/* Protected routes — require JWT token */}
        <Route path="/home" element={
          <ProtectedRoute>
            <div>Home (TODO)</div>
          </ProtectedRoute>
        } />

        <Route path="/events" element={
          <ProtectedRoute>
            <div>Events (TODO)</div>
          </ProtectedRoute>
        } />

        <Route path="/news" element={
          <ProtectedRoute>
            <div>News (TODO)</div>
          </ProtectedRoute>
        } />

        <Route path="/organizations" element={
          <ProtectedRoute>
            <div>Organizations (TODO)</div>
          </ProtectedRoute>
        } />

        {/* Catch-all — redirect unknown routes to login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </Router>
  );
}

export default App;

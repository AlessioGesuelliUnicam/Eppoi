import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import VerifyEmail from './components/VerifyEmail';
import Navbar from './components/Navbar';
import Home from './pages/Home';
import Events from './pages/Events';
import News from './pages/News';
import Organizations from './pages/Organizations';
import Questionnaire from './pages/Questionnaire';
import Chat from './pages/Chat';

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

        {/* Questionnaire — protected but without Navbar (full-screen flow) */}
        <Route path="/questionnaire" element={
          localStorage.getItem('authToken')
            ? <Questionnaire />
            : <Navigate to="/login" replace />
        } />

        {/* Protected routes — require JWT token */}
        <Route path="/home" element={
          <ProtectedRoute>
            <Home />
          </ProtectedRoute>
        } />

        <Route path="/events" element={
          <ProtectedRoute>
            <Events />
          </ProtectedRoute>
        } />

        <Route path="/news" element={
          <ProtectedRoute>
            <News />
          </ProtectedRoute>
        } />

        <Route path="/organizations" element={
          <ProtectedRoute>
            <Organizations />
          </ProtectedRoute>
        } />

        <Route path="/chat" element={
          <ProtectedRoute>
            <Chat />
          </ProtectedRoute>
        } />

        {/* Catch-all — redirect unknown routes to login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </Router>
  );
}

export default App;

import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchWithAuth } from '../utils/api';
import './Auth.css';

export default function Login() {
  const navigate = useNavigate();
  const [credentials, setCredentials] = useState({ email: '', password: '' });
  const [status, setStatus] = useState({ type: '', message: '' });

  // Check for email verification or error query params
  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    if (params.get('verified') === 'true') {
      setStatus({ type: 'success', message: 'Email verificata con successo! Ora puoi accedere.' });
    } else if (params.get('error') === 'invalid-token') {
      setStatus({ type: 'error', message: 'Link di verifica non valido o scaduto.' });
    }
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setCredentials((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setStatus({ type: '', message: '' });

    try {
      const response = await fetch('http://localhost:5052/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || 'Invalid email or password. Please try again.');
      }

      const data = await response.json();

      if (data.token) {
        localStorage.setItem('authToken', data.token);
        setCredentials({ email: '', password: '' });
        setStatus({ type: 'success', message: 'Authentication successful! Redirecting...' });

        const profile = await fetchWithAuth('/api/profile/me');
        if (profile && !profile.hasCompletedQuestionnaire) {
          navigate('/questionnaire');
        } else {
          navigate('/home');
        }
      } else {
        throw new Error('Authentication failed: No valid token received.');
      }

    } catch (error) {
      setStatus({ type: 'error', message: error.message });
    }
  };

  return (
      <div className="auth-wrapper">
        <div className="auth-card">
          <h2 className="auth-title">Welcome Back</h2>

          {status.message && (
              <div className={`auth-message ${status.type}`}>
                {status.message}
              </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="input-group">
              <label htmlFor="email">Email Address</label>
              <input
                  type="email"
                  id="email"
                  name="email"
                  value={credentials.email}
                  onChange={handleInputChange}
                  placeholder="you@example.com"
                  required
              />
            </div>

            <div className="input-group">
              <label htmlFor="password">Password</label>
              <input
                  type="password"
                  id="password"
                  name="password"
                  value={credentials.password}
                  onChange={handleInputChange}
                  placeholder="••••••••"
                  required
              />
            </div>

            <button type="submit" className="auth-button">Sign In</button>
          </form>

          <div className="auth-switch">
            Don't have an account yet?
            <span onClick={() => navigate('/register')}>Sign up here</span>
          </div>
        </div>
      </div>
  );
}
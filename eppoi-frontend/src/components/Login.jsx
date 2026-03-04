import { useState } from 'react';
import './Auth.css';

export default function Login({ onSwitchView }) {
  const [credentials, setCredentials] = useState({ email: '', password: '' });
  const [status, setStatus] = useState({ type: '', message: '' });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setCredentials((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setStatus({ type: '', message: '' });

    try {
      // Pointing to the specific backend endpoint for login
      const response = await fetch('http://localhost:5052/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(credentials)
      });
      
      if (!response.ok) {
        throw new Error('Invalid email or password. Please try again.');
      }
      
      const data = await response.json();
      
      // Store the JWT token securely
      if (data.token) {
        localStorage.setItem('authToken', data.token);
        setStatus({ type: 'success', message: 'Authentication successful! Redirecting...' });
        
        // Clear credentials from state after successful login
        setCredentials({ email: '', password: '' });
        
        // TODO: Implement routing to redirect the user to the dashboard
      } else {
        throw new Error('Authentication failed: No valid token received.');
      }

    } catch (error) {
      // Handles network errors (e.g., backend not running yet) gracefully
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
          <span onClick={onSwitchView}>Sign up here</span>
        </div>
      </div>
    </div>
  );
}
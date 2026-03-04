import { useState } from 'react';
import './Auth.css';

export default function Register({ onSwitchView }) {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [status, setStatus] = useState({ type: '', message: '' });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setStatus({ type: '', message: '' });
    
    // Client-side security check: ensure passwords match
    if (formData.password !== formData.confirmPassword) {
      setStatus({ type: 'error', message: 'Passwords do not match. Please try again.' });
      return;
    }

    // Client-side security check: minimum password length
    if (formData.password.length < 6) {
      setStatus({ type: 'error', message: 'Password must be at least 6 characters long.' });
      return;
    }

    try {
      // Pointing to the specific backend endpoint for registration
      const response = await fetch('http://localhost:5052/api/Auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: formData.email,
          password: formData.password
        })
      });
      
      if (!response.ok) {
        throw new Error('Registration failed. The email might already be in use or the server is unreachable.');
      }
      
      setStatus({ type: 'success', message: 'Account created successfully! You can now log in.' });
      
      // Clear sensitive data from memory
      setFormData({ email: '', password: '', confirmPassword: '' });
      
    } catch (error) {
      // Handles network errors (e.g., backend not running yet) gracefully
      setStatus({ type: 'error', message: error.message });
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="auth-card">
        <h2 className="auth-title">Create an Account</h2>
        
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
              value={formData.email} 
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
              value={formData.password} 
              onChange={handleInputChange} 
              placeholder="••••••••"
              required 
            />
          </div>
          
          <div className="input-group">
            <label htmlFor="confirmPassword">Confirm Password</label>
            <input 
              type="password" 
              id="confirmPassword"
              name="confirmPassword" 
              value={formData.confirmPassword} 
              onChange={handleInputChange} 
              placeholder="••••••••"
              required 
            />
          </div>
          
          <button type="submit" className="auth-button">Sign Up</button>
        </form>
        
        <div className="auth-switch">
          Already have an account? 
          <span onClick={onSwitchView}>Log in here</span>
        </div>
      </div>
    </div>
  );
}
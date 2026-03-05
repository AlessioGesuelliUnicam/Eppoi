import { useEffect, useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import './Auth.css';

export default function VerifyEmail() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState({ type: '', message: 'Verifying your email...' });

  useEffect(() => {
    const token = searchParams.get('token');
    if (!token) {
      setStatus({ type: 'error', message: 'Invalid verification link. No token provided.' });
      return;
    }

    fetch(`http://localhost:5052/api/Auth/verify-email?token=${encodeURIComponent(token)}`)
      .then(async (response) => {
        const data = await response.json();
        if (!response.ok) {
          throw new Error(data.message || 'Email verification failed.');
        }
        setStatus({ type: 'success', message: data.message || 'Email verified successfully! You can now login.' });
      })
      .catch((error) => {
        setStatus({ type: 'error', message: error.message });
      });
  }, [searchParams]);

  return (
    <div className="auth-wrapper">
      <div className="auth-card">
        <h2 className="auth-title">Email Verification</h2>

        {status.message && (
          <div className={`auth-message ${status.type}`}>
            {status.message}
          </div>
        )}

        {status.type === 'success' && (
          <button className="auth-button" onClick={() => navigate('/login')}>
            Go to Login
          </button>
        )}
      </div>
    </div>
  );
}

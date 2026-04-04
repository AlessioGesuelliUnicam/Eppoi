const BASE_URL = 'http://localhost:5052';

/**
 * Wrapper around fetch that automatically attaches the JWT token
 * from localStorage and handles 401 responses by redirecting to login.
 *
 * Supports both GET and POST requests:
 *   GET:  fetchWithAuth('/api/content/poi')
 *   POST: fetchWithAuth('/api/profile/questionnaire', {
 *           method: 'POST',
 *           headers: { 'Content-Type': 'application/json' },
 *           body: JSON.stringify({ ... })
 *         })
 */
export const fetchWithAuth = async (endpoint, options = {}) => {
  const token = localStorage.getItem('authToken');

  // Merge the auth header with any additional headers provided by the caller
  const headers = {
    ...options.headers,
    'Authorization': `Bearer ${token}`
  };

  const res = await fetch(`${BASE_URL}${endpoint}`, { ...options, headers });

  // If the token is expired or missing, clear it and force re-login
  if (res.status === 401) {
    localStorage.removeItem('authToken');
    window.location.href = '/login';
    return null;
  }

  return res.json();
};

/**
 * Builds the full image URL from a relative path returned by the backend.
 * The backend stores relative paths (e.g. "/Media/POI/photo.webp") that
 * need the external API base URL prepended.
 * Returns a placeholder if the path is null/undefined.
 */
export const getImageUrl = (imagePath) => {
  return imagePath
    ? `https://eppoi.io${imagePath}`
    : '/placeholder.svg';
};

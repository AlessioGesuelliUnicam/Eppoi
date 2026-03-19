import { useState, useEffect } from 'react';
import { fetchWithAuth, getImageUrl } from '../utils/api';
import './Events.css';

export default function Events() {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadEvents = async () => {
      const data = await fetchWithAuth('/api/content/events');
      if (data) setEvents(data);
      setLoading(false);
    };
    loadEvents();
  }, []);

  /**
   * Formats an ISO date string into a readable format (e.g. "15 Mar 2026").
   * Returns null if the date is missing so the UI can skip rendering it.
   */
  const formatDate = (dateString) => {
    if (!dateString) return null;
    return new Date(dateString).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  };

  if (loading) return <div className="page-loading">Loading...</div>;

  return (
    <div className="page-container">
      <h1 className="page-title">Events</h1>

      {events.length === 0 ? (
        <p className="page-empty">No events available at the moment.</p>
      ) : (
        <div className="card-grid">
          {events.map((event) => (
            <div key={event.id} className="card">
              <img
                src={getImageUrl(event.imagePath)}
                alt={event.title}
                className="card-image"
                // Prevent infinite loop: set placeholder and stop retrying
                onError={(e) => { e.target.onerror = null; e.target.src = '/placeholder.svg'; }}
              />
              <div className="card-body">
                <h3 className="card-title">{event.title}</h3>
                {event.typology && <span className="card-badge">{event.typology}</span>}
                {/* Show date range if available (startDate — endDate) */}
                {event.startDate && (
                  <p className="card-detail">
                    {formatDate(event.startDate)}
                    {event.endDate && ` — ${formatDate(event.endDate)}`}
                  </p>
                )}
                {event.address && <p className="card-detail">{event.address}</p>}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

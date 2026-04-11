import { useState, useEffect } from 'react';
import { fetchWithAuth, getImageUrl } from '../utils/api';
import MunicipalityFilter from '../components/MunicipalityFilter';
import './Home.css';

export default function Home() {
  const [content, setContent] = useState([]);
  const [loading, setLoading] = useState(true);
  const [municipalityId, setMunicipalityId] = useState(null);

  /**
   * Fetch personalized content (mix of POI and Events sorted by relevance).
   * Re-fetches whenever the selected municipality changes.
   */
  useEffect(() => {
    const loadContent = async () => {
      setLoading(true);
      const endpoint = municipalityId
        ? `/api/content/personalized?municipalityId=${municipalityId}`
        : '/api/content/personalized';
      const data = await fetchWithAuth(endpoint);
      if (data) setContent(data);
      setLoading(false);
    };
    loadContent();
  }, [municipalityId]);

  const formatDate = (dateString) => {
    if (!dateString) return null;
    return new Date(dateString).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  };

  if (loading) return <div className="page-loading">Loading...</div>;

  return (
    <div className="page-container">
      <h1 className="page-title">For You</h1>
      <MunicipalityFilter onChange={setMunicipalityId} />

      {content.length === 0 ? (
        <p className="page-empty">No personalized content available yet.</p>
      ) : (
        <div className="card-grid">
          {content.map((item) => (
            <div key={`${item.contentType}-${item.id}`} className="card">
              <div className="card-image-wrapper">
                <img
                  src={getImageUrl(item.imagePath)}
                  alt={item.contentType === 'POI' ? item.name : item.title}
                  className="card-image"
                  onError={(e) => { e.target.onerror = null; e.target.src = '/placeholder.svg'; }}
                />
                {/* "For you" badge for highly relevant content (score > 0.7) */}
                {item.score > 0.7 && (
                  <span className="card-score-badge">For you</span>
                )}
                {/* Small label to distinguish POI from Event */}
                <span className={`card-type-label ${item.contentType.toLowerCase()}`}>
                  {item.contentType === 'POI' ? 'Place' : 'Event'}
                </span>
              </div>
              <div className="card-body">
                <h3 className="card-title">
                  {item.contentType === 'POI' ? item.name : item.title}
                </h3>
                {item.type && <span className="card-badge">{item.type}</span>}
                {/* Show date range for events */}
                {item.contentType === 'Event' && item.startDate && (
                  <p className="card-detail">
                    {formatDate(item.startDate)}
                    {item.endDate && ` — ${formatDate(item.endDate)}`}
                  </p>
                )}
                {item.address && <p className="card-detail">{item.address}</p>}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

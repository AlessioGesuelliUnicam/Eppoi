import { useState, useEffect } from 'react';
import { fetchWithAuth, getImageUrl } from '../utils/api';
import './Home.css';

export default function Home() {
  const [pois, setPois] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadPois = async () => {
      const data = await fetchWithAuth('/api/content/poi');
      if (data) setPois(data);
      setLoading(false);
    };
    loadPois();
  }, []);

  if (loading) return <div className="page-loading">Loading...</div>;

  return (
    <div className="page-container">
      <h1 className="page-title">Points of Interest</h1>

      {pois.length === 0 ? (
        <p className="page-empty">No points of interest found.</p>
      ) : (
        <div className="card-grid">
          {pois.map((poi) => (
            <div key={poi.id} className="card">
              <img
                src={getImageUrl(poi.imagePath)}
                alt={poi.name}
                className="card-image"
                // Prevent infinite loop: set placeholder and stop retrying
                onError={(e) => { e.target.onerror = null; e.target.src = '/placeholder.svg'; }}
              />
              <div className="card-body">
                <h3 className="card-title">{poi.name}</h3>
                {/* Show the POI type as a badge (e.g. "ArtCulture", "Nature") */}
                {poi.type && <span className="card-badge">{poi.type}</span>}
                {poi.address && <p className="card-detail">{poi.address}</p>}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

import { useState, useEffect } from 'react';
import { fetchWithAuth, getImageUrl } from '../utils/api';
import MunicipalityFilter from '../components/MunicipalityFilter';
import './Organizations.css';

export default function Organizations() {
  const [organizations, setOrganizations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [municipalityId, setMunicipalityId] = useState(null);

  useEffect(() => {
    const loadOrganizations = async () => {
      setLoading(true);
      const endpoint = municipalityId
        ? `/api/content/organizations?municipalityId=${municipalityId}`
        : '/api/content/organizations';
      const data = await fetchWithAuth(endpoint);
      if (data) setOrganizations(data);
      setLoading(false);
    };
    loadOrganizations();
  }, [municipalityId]);

  if (loading) return <div className="page-loading">Loading...</div>;

  return (
    <div className="page-container">
      <h1 className="page-title">Organizations</h1>
      <MunicipalityFilter onChange={setMunicipalityId} />

      {organizations.length === 0 ? (
        <p className="page-empty">No organizations found.</p>
      ) : (
        <div className="card-grid">
          {organizations.map((org) => (
            <div key={org.id} className="card">
              <img
                src={getImageUrl(org.imagePath)}
                alt={org.name}
                className="card-image"
                onError={(e) => { e.target.onerror = null; e.target.src = '/placeholder.svg'; }}
              />
              <div className="card-body">
                <h3 className="card-title">{org.name}</h3>
                {org.type && <span className="card-badge">{org.type}</span>}
                {org.address && <p className="card-detail">{org.address}</p>}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

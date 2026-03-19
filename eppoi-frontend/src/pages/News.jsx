import { useState, useEffect } from 'react';
import { fetchWithAuth, getImageUrl } from '../utils/api';
import MunicipalityFilter from '../components/MunicipalityFilter';
import './News.css';

export default function News() {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [municipalityId, setMunicipalityId] = useState(null);

  useEffect(() => {
    const loadArticles = async () => {
      setLoading(true);
      const endpoint = municipalityId
        ? `/api/content/articles?municipalityId=${municipalityId}`
        : '/api/content/articles';
      const data = await fetchWithAuth(endpoint);
      if (data) setArticles(data);
      setLoading(false);
    };
    loadArticles();
  }, [municipalityId]);

  // Format ISO date to readable string (e.g. "15 Mar 2026")
  const formatDate = (dateString) => {
    if (!dateString) return null;
    return new Date(dateString).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  };

  if (loading) return <div className="page-loading">Loading...</div>;

  return (
    <div className="page-container">
      <h1 className="page-title">News</h1>
      <MunicipalityFilter onChange={setMunicipalityId} />

      {articles.length === 0 ? (
        <p className="page-empty">No articles available at the moment.</p>
      ) : (
        <div className="card-grid">
          {articles.map((article) => (
            <div key={article.id} className="card">
              <img
                src={getImageUrl(article.imagePath)}
                alt={article.title}
                className="card-image"
                onError={(e) => { e.target.onerror = null; e.target.src = '/placeholder.svg'; }}
              />
              <div className="card-body">
                <h3 className="card-title">{article.title}</h3>
                {article.subtitle && <p className="card-subtitle">{article.subtitle}</p>}
                {article.updatedAt && (
                  <p className="card-detail">{formatDate(article.updatedAt)}</p>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

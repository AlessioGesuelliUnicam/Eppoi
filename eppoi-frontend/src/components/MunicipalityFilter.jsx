import { useState, useEffect } from 'react';
import { fetchWithAuth } from '../utils/api';
import './MunicipalityFilter.css';

/**
 * Dropdown that fetches the list of municipalities from the backend
 * and calls onChange with the selected municipalityId (or null for "All").
 * Used by all content pages to filter results by municipality.
 */
export default function MunicipalityFilter({ onChange }) {
  const [municipalities, setMunicipalities] = useState([]);

  useEffect(() => {
    const load = async () => {
      const data = await fetchWithAuth('/api/content/municipalities');
      if (data) setMunicipalities(data);
    };
    load();
  }, []);

  const handleChange = (e) => {
    // Convert to number or null ("all" option)
    const value = e.target.value === '' ? null : parseInt(e.target.value);
    onChange(value);
  };

  return (
    <div className="municipality-filter">
      <label htmlFor="municipality-select">Municipality</label>
      <select id="municipality-select" onChange={handleChange} defaultValue="">
        <option value="">All municipalities</option>
        {municipalities.map((m) => (
          <option key={m.id} value={m.id}>{m.name}</option>
        ))}
      </select>
    </div>
  );
}

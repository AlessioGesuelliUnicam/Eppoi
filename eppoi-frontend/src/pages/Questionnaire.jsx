import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchWithAuth } from '../utils/api';
import './Questionnaire.css';

const INTERESTS = [
  'History & Culture',
  'Nature & Adventure',
  'Food & Wine',
  'Art & Museums',
  'Nightlife',
  'Wellness & Relaxation',
  'Shopping',
  'Beach & Sea',
];

const TRAVEL_STYLES = [
  'Solo Traveler',
  'Couple',
  'Family',
  'Group of Friends',
];

const TIME_OPTIONS = [
  { value: 'HalfDay', label: 'Half Day' },
  { value: 'FullDay', label: 'Full Day' },
  { value: 'Weekend', label: 'Weekend' },
];

export default function Questionnaire() {
  const navigate = useNavigate();
  const [step, setStep] = useState(1);
  const [selectedInterests, setSelectedInterests] = useState([]);
  const [selectedStyle, setSelectedStyle] = useState('');
  const [selectedTime, setSelectedTime] = useState('');
  const [error, setError] = useState('');

  const totalSteps = 3;

  // Toggle an interest on/off in the selection array
  const toggleInterest = (interest) => {
    setSelectedInterests((prev) =>
      prev.includes(interest)
        ? prev.filter((i) => i !== interest)
        : [...prev, interest]
    );
  };

  const handleNext = () => {
    setError('');

    // Validate current step before advancing
    if (step === 1 && selectedInterests.length === 0) {
      setError('Please select at least one interest.');
      return;
    }
    if (step === 2 && !selectedStyle) {
      setError('Please select a travel style.');
      return;
    }

    setStep(step + 1);
  };

  const handleBack = () => {
    setError('');
    setStep(step - 1);
  };

  const handleSubmit = async () => {
    setError('');

    if (!selectedTime) {
      setError('Please select your time availability.');
      return;
    }

    const res = await fetchWithAuth('/api/profile/questionnaire', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        interests: selectedInterests,
        travelStyle: selectedStyle,
        timeAvailability: selectedTime,
      }),
    });

    if (res) {
      navigate('/home');
    }
  };

  return (
    <div className="questionnaire-wrapper">
      <div className="questionnaire-card">
        <h2 className="questionnaire-title">Tell us about yourself</h2>

        {/* Progress bar showing current step */}
        <div className="progress-bar">
          {[1, 2, 3].map((s) => (
            <div
              key={s}
              className={`progress-step ${s <= step ? 'active' : ''}`}
            />
          ))}
        </div>
        <p className="step-indicator">Step {step} of {totalSteps}</p>

        {error && <div className="questionnaire-error">{error}</div>}

        {/* Step 1: Interests (multi-select) */}
        {step === 1 && (
          <div className="step-content">
            <h3>What are you interested in?</h3>
            <p className="step-hint">Select one or more</p>
            <div className="options-grid">
              {INTERESTS.map((interest) => (
                <button
                  key={interest}
                  className={`option-chip ${selectedInterests.includes(interest) ? 'selected' : ''}`}
                  onClick={() => toggleInterest(interest)}
                >
                  {interest}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Step 2: Travel style (single-select) */}
        {step === 2 && (
          <div className="step-content">
            <h3>How do you usually travel?</h3>
            <div className="options-grid">
              {TRAVEL_STYLES.map((style) => (
                <button
                  key={style}
                  className={`option-chip ${selectedStyle === style ? 'selected' : ''}`}
                  onClick={() => setSelectedStyle(style)}
                >
                  {style}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Step 3: Time availability (single-select) */}
        {step === 3 && (
          <div className="step-content">
            <h3>How much time do you have?</h3>
            <div className="options-grid">
              {TIME_OPTIONS.map((opt) => (
                <button
                  key={opt.value}
                  className={`option-chip ${selectedTime === opt.value ? 'selected' : ''}`}
                  onClick={() => setSelectedTime(opt.value)}
                >
                  {opt.label}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Navigation buttons — disabled until the user makes a selection */}
        <div className="questionnaire-actions">
          {step > 1 && (
            <button className="btn-back" onClick={handleBack}>Back</button>
          )}
          {step < totalSteps ? (
            <button
              className="btn-next"
              onClick={handleNext}
              disabled={
                (step === 1 && selectedInterests.length === 0) ||
                (step === 2 && !selectedStyle)
              }
            >
              Next
            </button>
          ) : (
            <button
              className="btn-submit"
              onClick={handleSubmit}
              disabled={!selectedTime}
            >
              Finish
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

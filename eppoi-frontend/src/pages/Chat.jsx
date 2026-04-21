import { useState, useRef, useEffect } from 'react';
import './Chat.css';

const BASE_URL = 'http://localhost:5052';

export default function Chat() {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [serviceError, setServiceError] = useState(false);
  const [municipalityId, setMunicipalityId] = useState(null);
  const bottomRef = useRef(null);

  // Fetch the first available municipality on mount
  useEffect(() => {
    const fetchMunicipality = async () => {
      try {
        const token = localStorage.getItem('authToken');
        const res = await fetch(`${BASE_URL}/api/content/municipalities`, {
          headers: { 'Authorization': `Bearer ${token}` }
        });
        if (res.ok) {
          const data = await res.json();
          if (data.length > 0) setMunicipalityId(data[0].id);
        }
      } catch {
        // If it fails, municipalityId stays null and the backend uses all data
      }
    };
    fetchMunicipality();
  }, []);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages, loading]);

  const sendMessage = async (e) => {
    e.preventDefault();
    const text = input.trim();
    if (!text || loading) return;

    setMessages((prev) => [...prev, { role: 'user', text }]);
    setInput('');
    setLoading(true);
    setServiceError(false);

    try {
      const token = localStorage.getItem('authToken');
      const res = await fetch(`${BASE_URL}/api/chat`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ message: text, municipalityId })
      });

      if (res.status === 401) {
        localStorage.removeItem('authToken');
        window.location.href = '/login';
        return;
      }

      if (res.status === 503) {
        setServiceError(true);
        return;
      }

      const data = await res.json();
      setMessages((prev) => [
        ...prev,
        { role: 'bot', text: data.message, outOfDomain: data.isOutOfDomain }
      ]);
    } catch {
      setServiceError(true);
    } finally {
      setLoading(false);
    }
  };

  return (
      <div className="chat-container">
        <h1 className="page-title">Chat</h1>
        <p className="chat-subtitle">Ask anything about tourism in our municipalities.</p>

        <div className="chat-window">
          {messages.length === 0 && !loading && (
              <p className="chat-empty">Start the conversation by typing a message below.</p>
          )}

          {messages.map((m, i) => (
              <div
                  key={i}
                  className={`chat-bubble ${m.role} ${m.outOfDomain ? 'out-of-domain' : ''}`}
              >
                {m.outOfDomain && <span className="chat-warning-label">Off-topic</span>}
                {m.text}
              </div>
          ))}

          {loading && (
              <div className="chat-bubble bot loading">
                <span className="chat-dot" />
                <span className="chat-dot" />
                <span className="chat-dot" />
              </div>
          )}

          {serviceError && (
              <div className="chat-error">
                Chatbot service unavailable. Make sure Ollama is running.
              </div>
          )}

          <div ref={bottomRef} />
        </div>

        <form className="chat-input-row" onSubmit={sendMessage}>
          <input
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              placeholder="Type a message..."
              className="chat-input"
              disabled={loading}
          />
          <button
              type="submit"
              className="chat-send"
              disabled={loading || !input.trim()}
          >
            Send
          </button>
        </form>
      </div>
  );
}
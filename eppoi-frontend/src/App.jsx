// src/App.jsx
import { useState } from 'react';
import Login from './components/Login';
import Register from './components/Register';

function App() {
  const [currentView, setCurrentView] = useState('login');

  const toggleView = () => {
    setCurrentView(currentView === 'login' ? 'register' : 'login');
  };

  return (
    <div>
      {currentView === 'login' ? (
        <Login onSwitchView={toggleView} />
      ) : (
        <Register onSwitchView={toggleView} />
      )}
    </div>
  );
}
export default App;
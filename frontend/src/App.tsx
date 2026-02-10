import React, { useState, useEffect } from 'react';
import './App.css';

interface Prompt {
    id: string;
    content: string;
    status: number;
    result: string | null;
    createdAt: string;
}

const getStatusName = (status: number) => {
    switch (status) {
        case 0: return '⏳ Pending';
        case 1: return '⚙️ Processing';
        case 2: return '✅ Completed';
        case 3: return '❌ Failed';
        default: return '❓';
    }
};

function App() {
    const [prompts, setPrompts] = useState<Prompt[]>([]);
    const [input, setInput] = useState('');
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchPrompts = async () => {
            try {
                const response = await fetch('/api/prompts');
                

                const data = await response.json();
                setPrompts(data);
            } catch (error) {
                console.error('Błąd:', error);
            }
        };

        fetchPrompts();
        const interval = setInterval(fetchPrompts, 2000);
        return () => clearInterval(interval);
    }, []);

    const sendPrompt = async () => {
        if (!input.trim()) return;
        setLoading(true);

        try {
            await fetch('https://localhost:7154/api/prompts', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(input.trim()),
            });
            setInput('');
        } catch (error) {
            console.error('Błąd wysyłania:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="app">
            <div className="container">
                <h1>🤖 AI Prompt Processor</h1>

                <div className="input-section">
                    <input
                        value={input}
                        onChange={(e) => setInput(e.target.value)}
                        placeholder="Wpisz pytanie dla AI (np. co to gorączka)..."
                        className="input"
                        onKeyPress={(e) => e.key === 'Enter' && sendPrompt()}
                    />
                    <button onClick={sendPrompt} disabled={loading || !input.trim()}>
                        {loading ? '⏳ Wysyłanie...' : '🚀 Wyślij'}
                    </button>
                </div>

                <div className="stats">
                    Aktywnych promptów: <strong>{prompts.filter(p => p.status === 0).length}</strong>
                </div>

                <div className="prompts">
                    {prompts.map((prompt) => (
                        <div key={prompt.id} className={`prompt status-${prompt.status}`}>
                            <div className="prompt-header">
                                <div className="prompt-text">{prompt.content}</div>
                                <div className={`status-badge status-${prompt.status}`}>
                                    {getStatusName(prompt.status)}
                                </div>
                            </div>

                            {prompt.result && (
                                <div className="result">
                                    <div className="result-label">🤖 Odpowiedź AI:</div>
                                    <div className="result-text">{prompt.result}</div>
                                </div>
                            )}

                            <div className="prompt-time">
                                {new Date(prompt.createdAt).toLocaleString('pl-PL')}
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}

export default App;

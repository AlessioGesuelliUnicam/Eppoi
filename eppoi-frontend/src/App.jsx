import { useEffect, useState } from "react"

function App() {
    const [status, setStatus] = useState("Connecting...")

    useEffect(() => {
        fetch("http://localhost:5052/api/health")
            .then(res => {
                if (res.ok) setStatus("✅ Backend connected!")
                else setStatus("❌ Backend error")
            })
            .catch(() => setStatus("❌ Cannot reach backend"))
    }, [])

    return (
        <div style={{ padding: "2rem", fontFamily: "Arial" }}>
            <h1>Eppoi App</h1>
            <p>Backend status: <strong>{status}</strong></p>
        </div>
    )
}

export default App
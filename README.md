# Eppoi App

A tourism Progressive Web App (PWA) built with .NET 10 and React 19. Users can explore points of interest, events, news and organizations of a chosen municipality, with personalized content recommendations and an AI-powered chatbot assistant.

**Municipality:** Gradara (municipalityId = 4)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 10 Web API, Entity Framework Core, PostgreSQL |
| Frontend | React 19, Vite, React Router |
| Auth | JWT Bearer Token, BCrypt |
| Email | MailKit + Gmail SMTP |
| AI/Chatbot | Ollama (llama3.2:3b) |
| Testing | xUnit, Moq |

---

## Prerequisites

Before running the project, make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Ollama](https://ollama.com/) with the `llama3.2:3b` model

### Pull the Ollama model

```bash
ollama pull llama3.2:3b
```

---

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/AlessioGesuelliUnicam/Eppoi.git
cd Eppoi
```

### 2. Configure the backend

Create the file `Eppoi.API/appsettings.Development.json` with your credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=eppoi;Username=YOUR_PG_USER;Password=YOUR_PG_PASSWORD"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "eppoi-api",
    "Audience": "eppoi-frontend"
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "Eppoi App",
    "Password": "your-gmail-app-password"
  }
}
```

> **Note:** `appsettings.Development.json` is git-excluded. Never commit real credentials.

### 3. Create the database and apply migrations

```bash
cd Eppoi.API
dotnet ef database update
```

---

## Running the Project

### Start Ollama

Make sure Ollama is running before starting the backend:

```bash
ollama serve
```

> If you see `address already in use`, Ollama is already running — you can skip this step.

### Start the backend

```bash
cd Eppoi.API
dotnet run
```

The API will be available at `http://localhost:5052`.

### Start the frontend

Open a new terminal:

```bash
cd eppoi-frontend
npm install
npm run dev
```

The app will be available at `http://localhost:5173`.

---

## Data Ingestion

The database is empty by default. After starting the backend, you need to ingest the tourism data for Gradara.

**1. Register and login to get a JWT token:**

```bash
curl -X POST http://localhost:5052/api/Auth/register \
  -H "Content-Type: application/json" \
  -d '{"name": "Admin", "email": "admin@eppoi.com", "password": "123456"}'
```

> Check your email and click the verification link before logging in.

```bash
curl -X POST http://localhost:5052/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@eppoi.com", "password": "123456"}'
```

Copy the `token` from the response.

**2. Run the ingestion:**

```bash
curl -X POST "http://localhost:5052/api/Ingestion/ingest?municipality=Comune%20di%20Gradara" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

This will fetch and store all tourism data from the external API (`apispm.eppoi.io`).

Expected result: **15 POI, 14 organizations, 2 articles**.

> The ingestion is idempotent — safe to run multiple times without creating duplicates.

---

## Running the Tests

```bash
cd Eppoi.Tests
dotnet test
```

Expected output: **42 tests passed**.

---

## Project Structure

```
Eppoi/
├── Eppoi.API/                  # .NET 10 Web API
│   ├── Controllers/            # API endpoints
│   ├── Services/               # Business logic
│   ├── Repositories/           # Data access layer
│   ├── Models/                 # Database entities
│   ├── DTOs/                   # Request/response objects
│   ├── Data/                   # EF Core DbContext
│   └── Migrations/             # EF Core migrations
├── eppoi-frontend/             # React 19 PWA
│   ├── src/
│   │   ├── pages/              # App pages (Home, Chat, Login...)
│   │   ├── components/         # Reusable components
│   │   └── utils/              # API utilities
└── Eppoi.Tests/                # xUnit + Moq unit tests
```

---

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Auth/register` | ❌ | Register a new user |
| POST | `/api/Auth/login` | ❌ | Login and get JWT token |
| POST | `/api/Auth/logout` | ✅ | Logout |
| GET | `/api/Auth/me` | ✅ | Get current user info |
| GET | `/api/Auth/verify-email` | ❌ | Verify email address |
| POST | `/api/Auth/forgot-password` | ❌ | Request password reset |
| POST | `/api/Auth/reset-password` | ❌ | Reset password with token |
| POST | `/api/Profile/questionnaire` | ✅ | Submit user questionnaire |
| GET | `/api/Profile/me` | ✅ | Get user profile |
| GET | `/api/content/poi` | ✅ | Get points of interest |
| GET | `/api/content/events` | ✅ | Get events |
| GET | `/api/content/articles` | ✅ | Get news articles |
| GET | `/api/content/organizations` | ✅ | Get organizations |
| GET | `/api/content/municipalities` | ✅ | Get municipalities |
| GET | `/api/content/personalized` | ✅ | Get personalized content |
| POST | `/api/chat` | ✅ | Send message to chatbot |
| POST | `/api/Ingestion/ingest` | ✅ | Ingest data from external API |

---

## Team

| Name | Role |
|------|------|
| Alessio Gesuelli | Backend Development |
| Mattia Giaccaglia | Frontend Development |

Software Project Management Lab — University of Camerino — A.Y. 2025/2026
#  Community Car MVC

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/Mostafa-SAID7/Community-Car-MVC)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Framework](https://img.shields.io/badge/Framework-ASP.NET%20Core%209.0-blue)](https://dotnet.microsoft.com/download)

A premium, feature-rich community platform for car enthusiasts. Built with ASP.NET Core MVC, SignalR, and modern web technologies, this platform offers real-time interactions, localization, and an AI-powered assistant.

---

## ✨ Key Features

### 🏛️ Community Features
- **Q&A System**: Ask and answer questions with real-time updates.
- **Feeds & Stories**: Share your automotive journey with posts and temporary stories.
- **Groups & Events**: Join enthusiast groups and participate in local car meets.
- **Maps & Points of Interest**: Discover car-friendly locations and tracks.

### 💬 Real-time Communication
- **SignalR Powered**: Instant notifications and private localized chat functionality.
- **Interactions**: Like, react, comment, and share content seamlessly across the platform.

### 🤖 AI Assistant
- Integrated AI powered by **Google Gemini** and **Hugging Face**.
- Glassmorphism design with interactive chat widget and smart responsiveness.

### 🌐 Global Experience
- **Full Localization**: Support for English and Arabic (RTL support included).
- **Dashboard Managed**: Dynamic translation management through a dedicated admin dashboard.

---

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 9.0 (MVC), Entity Framework Core
- **Database**: SQL Server
- **Frontend**: Vanilla CSS (Custom Variables), JavaScript, Razor Pages
- **Real-time**: SignalR
- **AI Integration**: Gemini API, Hugging Face Hub

---

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/Mostafa-SAID7/Community-Car-MVC.git
   ```
2. Navigate to the project directory:
   ```bash
   cd "Community Car MVC"
   ```
3. Update `appsettings.json` with your connection string and API keys.
4. Run migrations/seed data:
   ```bash
   dotnet run --project src/CommunityCar.Web
   ```

---

## 📂 Project Structure

```text
src/
├── CommunityCar.Domain         # Core entities and enums
├── CommunityCar.Application    # Business logic, interfaces, and DTOs
├── CommunityCar.Infrastructure # Data access, repository implementations, and services
├── CommunityCar.AI             # AI service implementations
└── CommunityCar.Web            # UI layer, Controllers, and Views
```

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) to get started.

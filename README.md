# 🌍 Cross-Cloud AI Cost Advisor

**Cross-Cloud AI Cost Advisor** is a **.NET + Blazor dashboard** that aggregates **AWS and Azure cloud costs**, normalizes the data, and generates **AI-powered insights** for smarter cost management.  

This project bridges the gap between siloed billing consoles by providing a **unified multi-cloud view** with **narrative recommendations**.

---

## ✨ Current Status

| Component | Implemented | In-Progress / Planned |
|---|---|---|
| Cost data fetching (AWS , Azure & GCP) | ✅ | |
| Normalizing costs schema + storage | ✅ | Refinements of entities & tags |
| UI for costs & charts | ✅ | Dashboard cards, service breakdown & trends |
| OpenAI‐powered recommendations | ⚙️ Partial (fake / stub data) | Real integration + recommendations stored |
| Scheduler (periodic fetch) | ⚙️ (design proposed) | Setup (Hangfire or Quartz.NET) |

---

---

## ✨ Features (MVP)

- 🔄 Fetch AWS (Cost Explorer) and Azure (Cost Management) billing data  
- 🗂 Normalize costs into a single unified schema  
- 📊 Blazor dashboard with:  
  - Cost by provider (AWS vs Azure)  
  - Monthly cost trend (line graph)  
- 🤖 AI-powered recommendations (OpenAI)  
- ⚙️ Automated daily sync (Hangfire / Quartz.NET)  

---

## 🏗️ Architecture (MVP)

```mermaid
flowchart TD
    A[AWS Cost Explorer] --> C[API]
    B[Azure Cost Mgmt API] --> C[API]
    C --> D[BillingService]
    D --> E[(Database)]
    D --> F[InsightsService]
    F --> G[OpenAI API]
    E --> H[Blazor Dashboard]
    F --> H[Blazor Dashboard]
```
## 🏛 Architecture & Schema

- **Providers** → AWS, Azure (fake or real billing APIs)  
- **Accounts** → Each provider + account identifier (e.g. AWS account, Azure subscription)  
- **NormalizedCosts** table:  
  - `Id`, `AccountId`, `Region`, `Service`, `UsageAmount`, `Cost`, `Date`  
  - Navigation from `Account` → `Provider`  
- **Recommendations** table:  
  - `Id`, `CostId` (NormalizedCost), `Message`, `Confidence`, `EstimatedSavings`, `CreatedAt`

---
## 📂 Project Structure

```
cross-cloud-ai-cost-advisor/
 ├── src/
 │   ├── CostAdvisor.API/            # ASP.NET Core Web API
 │   ├── CostAdvisor.UI/             # Blazor Server frontend
 │   ├── CostAdvisor.Core/           # Business logic, connectors, services
 │   └── CostAdvisor.Infrastructure/ # Persistence, DB, config
 ├── tests/
 │   └── CostAdvisor.UnitTests/      # Unit tests
 ├── docs/                           # Architecture docs, diagrams
 ├── README.md
 └── LICENSE
```

## 🚀 Getting Started

The app supports both:

- **Fake mode**: Generate synthetic billing / recommendation data (useful for development)  
- **Real mode**: Connect to real cloud billing APIs + OpenAI (when credentials configured)
You will need:

- AWS / Azure credentials (if using real billing APIs)  
- OpenAI API key (for real recommendations)  
- Database connection string (Postgres SQL)

1. Clone the Repository
```
git clone https://github.com/your-username/cross-cloud-ai-cost-advisor.git
cd cross-cloud-ai-cost-advisor
```

2. Configure Credentials

Create appsettings.json (or use environment variables):
```
{
  "AWS": {
    "AccessKey": "your-access-key",
    "SecretKey": "your-secret-key",
    "Region": "us-east-1"
  },
  "Azure": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "SubscriptionId": "your-subscription-id"
  },
  "OpenAI": {
    "ApiKey": "your-openai-key"
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=CostAdvisor;User Id=sa;Password=yourpassword;"
  }
}
```
3. Run API
```
cd src/CostAdvisor.API
dotnet run
```

4. Run UI
```
cd src/CostAdvisor.UI
dotnet run
```
5. Open in Browser

👉 http://localhost:5000

---

## 🔄 Roadmap

 Forecasting with AI narrative reports

 Custom rules engine & alerts (Slack/Teams)

 Multi-tenant support for enterprises

 Docker + Kubernetes deployment


## 🛠 Tech Stack

---

.NET 9 (ASP.NET Core Web API, Blazor Server)

AWS Cost Explorer API

Azure Cost Management API

PostgreSQL / SQL Server

OpenAI GPT (recommendations)

Hangfire / Quartz.NET (scheduling)

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!
Feel free to open a PR or create an issue.

## 📜 License

This project is licensed under the Apache 2.0 License – see the LICENSE file for details

## Samples


![ResourceCosts](https://github.com/user-attachments/assets/7557f013-ff6d-430f-a80e-16f6d5c381dc)
![Recommendations](https://github.com/user-attachments/assets/511a2590-396b-4cfe-8fab-970cec6ee076)
![ProviderChart](https://github.com/user-attachments/assets/700927f6-f1cd-4e83-9982-79dbb3115d5c)
![Top services](https://github.com/user-attachments/assets/59878223-4a64-4678-8e0e-894f49a46808)

# 🛍️ Shopping E-Commerce Web API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?logo=nuget)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC292B?logo=microsoft-sql-server&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-Distributed%20Cache-DC382D?logo=redis&logoColor=white)
![JWT](https://img.shields.io/badge/Authentication-JWT%20Bearer-black?logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/API%20Docs-Swagger%20%2F%20Stoplight-85EA2D?logo=swagger&logoColor=black)

A high-performance, scalable, and modular **E-Commerce RESTful Web API** built with **ASP.NET Core (.NET 10)** following Clean Architecture principles, Repository & Unit of Work design patterns, Redis distributed caching, and SQL Server persistence.

---

## 🌟 Key Features

- **🔐 Authentication & User Identity:**
  - Secure registration, login, email activation, and password reset flows using ASP.NET Core Identity.
  - JWT (JSON Web Token) authentication with secure cookie fallback support.
  - User profile & shipping address management.

- **📦 Product & Category Catalog:**
  - Product management with pagination, filtering, and sorting.
  - Category classification and product relationships.
  - Multi-image uploading and management for product media.
  - Product reviews and nested comment system.

- **🛒 High-Performance Basket (Redis):**
  - Customer basket caching with **StackExchange.Redis** for ultra-low latency cart operations.
  - Real-time basket creation, item updates, quantity adjustment, and item removal.

- **💳 Checkout & Payment Integration:**
  - Delivery method selection and order checkout processing.
  - **Stripe** payment processing integration.

- **🛡️ Security & Performance:**
  - In-memory rate limiting middleware to prevent brute-force and DDoS attacks.
  - Security headers (CSP, X-Content-Type-Options, X-Frame-Options, Referrer-Policy).
  - Global centralized exception handling middleware.
  - Configured CORS policies for seamless frontend (React / Vite / Angular) pairing.

- **📖 Interactive API Documentation:**
  - Interactive **Swagger UI** for testing all endpoints.
  - Modern **Stoplight Elements** documentation interface.

---

## 🏗️ Architecture & Project Structure

The solution follows a multi-tier modular architecture:

```
shopping-asp-main-main/
├── api/                   # Presentation Layer (Controllers, Middlewares, DTO Mappings, Startup)
│   ├── Controllers/       # RESTful API Controllers (Account, Product, Basket, Checkout, etc.)
│   ├── middleware/        # Custom Rate Limiter, Security Headers & Global Exception Handler
│   ├── mapping/           # AutoMapper Profiles & DTO Transformations
│   ├── halper/            # Response Wrappers & Pagination Helpers
│   └── Program.cs         # Application Dependency Injection & Middleware Pipeline
│
├── core/                  # Domain Layer (Entities, DTOs, Business Interfaces)
│   ├── Entities/          # Core Domain Entities (Product, Category, Basket, Order, AppUser)
│   ├── interfaces/        # Repository Contracts & UnitOfWork Interfaces
│   ├── Services/          # Service Contracts (Auth, Email, Token, Payment, Basket)
│   └── Dto/               # Data Transfer Objects
│
└── inftastructer/         # Data & External Services Layer
    ├── Data/              # AppDbContext, Entity Configurations & Seeders
    ├── Migrations/        # Entity Framework Core Migrations
    └── Repository/        # Generic Repositories, Unit of Work, Redis Basket & Services
```

---

## 🛠️ Tech Stack

- **Backend Framework:** ASP.NET Core Web API (.NET 10)
- **Language:** C# 13
- **ORM:** Entity Framework Core 10 (Code-First)
- **Database:** Microsoft SQL Server
- **Caching:** Redis (StackExchange.Redis)
- **Identity & Security:** ASP.NET Core Identity, JWT Bearer Tokens
- **Mapping:** AutoMapper
- **Mailing:** MailKit & MimeKit
- **Payment Gateway:** Stripe API
- **API Documentation:** Swagger / OpenAPI & Stoplight Elements

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed on your machine:
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or SQL Express / LocalDB)
- [Redis Server](https://redis.io/download/) (running on `localhost:6379`)

### 1. Clone the Repository

```bash
git clone https://github.com/ahmed-Basal/shopping-asp-main-main.git
cd shopping-asp-main-main/shopping-asp-main-main
```

### 2. Configure Connection Strings

Update `api/appsettings.Development.json` with your SQL Server and Redis connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=new-ecomercev1;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "Token": {
    "Secret": "your_long_super_secret_key_here_for_hmac_sha512",
    "Issuer": "http://localhost:5239"
  }
}
```

### 3. Run Database Migrations

Apply EF Core migrations to create the database schema:

```bash
dotnet ef database update --project inftastructer --startup-project api
```

### 4. Run the API

```bash
dotnet run --project api
```

The API will start listening at:
- **HTTP:** `http://localhost:5239`
- **Swagger Documentation:** `http://localhost:5239/swagger`
- **Stoplight Elements Docs:** `http://localhost:5239/docs`

---

## 📡 API Endpoints Overview

| Area | HTTP Method | Endpoint | Description |
| :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/Account/register` | Register new user account |
| **Auth** | `POST` | `/api/Account/login` | Login and receive JWT token |
| **Auth** | `POST` | `/api/Account/ActiveEmail` | Confirm user email address |
| **Auth** | `POST` | `/api/Account/forget-password` | Request password reset token |
| **Products** | `GET` | `/api/Product/get-all` | Get paginated list of products |
| **Products** | `GET` | `/api/Product/get-by-id/{id}` | Get product details by ID |
| **Products** | `POST` | `/api/Product/Add-Product` | Create new product with photos |
| **Categories** | `GET` | `/api/Categories/get-all` | Get all product categories |
| **Categories** | `POST` | `/api/Categories/add-category` | Create new category |
| **Basket** | `GET` | `/api/Baskets/get-basket` | Retrieve customer shopping cart |
| **Basket** | `POST` | `/api/Baskets/create-cart` | Create customer basket in Redis |
| **Basket** | `PUT` | `/api/Baskets/update-item` | Add / Update item in basket |
| **Basket** | `DELETE`| `/api/Baskets/delete-cart` | Clear / Delete customer basket |
| **Comments** | `POST` | `/api/Comments/ADD-Commnent` | Add review or comment to product |
| **Checkout** | `POST` | `/api/Checkout/Create/{id}` | Process checkout with delivery |

---

## 👨‍💻 Author

Developed with ❤️ by **[Ahmed Bassal](https://github.com/ahmed-Basal)**

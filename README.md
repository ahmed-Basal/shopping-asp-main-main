# 🛍️ Shopping E-Commerce Web API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-10.0-512BD4?logo=nuget)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC292B?logo=microsoft-sql-server&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-Distributed%20Cache-DC382D?logo=redis&logoColor=white)
![JWT & Refresh Token](https://img.shields.io/badge/Auth-JWT%20%2B%20Refresh%20Token-black?logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/API%20Docs-Swagger%20%2F%20Stoplight-85EA2D?logo=swagger&logoColor=black)

A high-performance, scalable, and modular **E-Commerce RESTful Web API** built with **ASP.NET Core (.NET 10)** following Clean Architecture principles, Repository & Unit of Work design patterns, Redis distributed caching, SQL Server persistence, and secure JWT + Refresh Token authentication with Token Rotation.

---

## 🌟 Key Features

- **🔐 Advanced Authentication & User Identity:**
  - **ASP.NET Core Identity** integration for user registration, email verification, password reset, and password changing.
  - **JWT (JSON Web Token) Access Tokens** for fast, stateless API authorization.
  - **Secure Refresh Tokens with Token Rotation:**
    - Cryptographically generated 64-byte random tokens with a 7-day sliding expiration.
    - Stored in the database and tied to user sessions.
    - Automatic revocation upon reuse (Token Rotation) to prevent replay and theft attacks.
  - **Dual-Channel Delivery:** Returns tokens in JSON responses (for SPA state / mobile apps) and simultaneously sets them in secure **HttpOnly Cookies** (`authToken` & `refreshToken`).
  - **Revocation & Logout:** Dedicated endpoint to revoke refresh tokens and clear cookies immediately upon logout.

- **🛡️ Security & Middleware Protection:**
  - **In-Memory Rate Limiting:** Custom IP-based rate limiter (80 requests per 30-second window) protecting the API against brute-force, scraping, and DDoS attacks.
  - **Hardened Security Headers:** Content Security Policy (CSP), `X-Content-Type-Options`, `X-Frame-Options`, and `Referrer-Policy`.
  - **Global Exception Middleware:** Centralized error interception returning standardized JSON error schemas (`ApiExceptions`).
  - **Dynamic CORS:** Pre-configured for local frontend dev servers (Vite, React, Next.js) supporting cross-origin credentials and cookies.

- **📦 Product & Category Catalog:**
  - Product catalog with pagination, filtering, search, and category mapping.
  - Multi-image uploading and management for product media stored in `wwwroot`.
  - Reviews and nested comment system for products.

- **🛒 High-Performance Basket (Redis Cache):**
  - High-speed distributed customer cart powered by **StackExchange.Redis**.
  - Real-time cart creation, quantity updates, item removal, and auto-TTL expiration.

- **💳 Checkout & Payment Integration:**
  - Delivery method selection and order checkout processing.
  - **Stripe** payment processing integration.

- **📖 Interactive API Documentation:**
  - Interactive **Swagger UI** for testing endpoints in the browser.
  - Modern **Stoplight Elements** interface available at `/docs`.

---

## 🏗️ Architecture & Project Structure

The solution follows Clean Architecture and Separation of Concerns:

```
shopping-asp-main-main/
├── api/                   # Presentation Layer (Controllers, Middlewares, DTO Mappings, Program.cs)
│   ├── Controllers/       # RESTful API Controllers (Account, Product, Basket, Checkout, etc.)
│   ├── middleware/        # Rate Limiting, Security Headers & Global Exception Handling
│   ├── mapping/           # AutoMapper Profiles & DTO Transformations
│   ├── halper/            # Response Wrappers, Pagination & Error Models
│   └── wwwroot/           # Static Product Photos & Assets
│
├── core/                  # Domain Layer (Entities, DTOs, Business Interfaces)
│   ├── Entities/          # AppUser, RefreshToken, Product, Category, Basket, Address, Comment
│   ├── interfaces/        # Repository Contracts & Unit of Work Interface
│   ├── Services/          # Service Contracts (IAccountService, ITokenGenerate, IEmailServices, etc.)
│   └── Dto/               # Request & Response Data Transfer Objects (AuthResponseDto, etc.)
│
└── inftastructer/         # Data & External Services Layer
    ├── Data/              # AppDbContext & Fluent API Entity Configurations
    ├── Migrations/        # Entity Framework Core Code-First Migrations
    └── Repository/        # Repositories, UnitOfWork, Token Generation & Redis Cart Implementation
```

---

## 🛠️ Tech Stack

- **Backend Framework:** ASP.NET Core Web API (.NET 10)
- **Language:** C# 13
- **ORM:** Entity Framework Core 10 (Code-First)
- **Database:** Microsoft SQL Server
- **Caching:** Redis (StackExchange.Redis)
- **Identity & Security:** ASP.NET Core Identity, JWT Bearer Tokens, Refresh Tokens
- **Mapping:** AutoMapper
- **Mailing:** MailKit & MimeKit
- **Payment Gateway:** Stripe API
- **API Documentation:** Swagger / OpenAPI & Stoplight Elements

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed on your machine:
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
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

Apply EF Core migrations to create/update tables (including the `RefreshTokens` table):

```bash
dotnet ef database update --project inftastructer --startup-project api
```

### 4. Run the API

```bash
dotnet run --project api
```

The API will start listening at:
- **API Host:** `http://localhost:5239`
- **Swagger Documentation:** `http://localhost:5239/swagger`
- **Stoplight Elements Docs:** `http://localhost:5239/docs`

---

## 📡 API Endpoints Overview

| Area | HTTP Method | Endpoint | Description |
| :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/Account/register` | Register new user account |
| **Auth** | `POST` | `/api/Account/login` | Login, returns JWT + Refresh Token (JSON & Cookies) |
| **Auth** | `POST` | `/api/Account/refresh-token` | Renew expired access token using refresh token (Rotation) |
| **Auth** | `POST` | `/api/Account/revoke-token` | Revoke active refresh token on logout |
| **Auth** | `GET`  | `/api/Account/ActiveEmail` | Confirm user email address via token |
| **Auth** | `POST` | `/api/Account/forget-password` | Request password reset code via email |
| **Auth** | `POST` | `/api/Account/reset-password` | Reset password using email verification code |
| **Auth** | `POST` | `/api/Account/change-password` | Change password for authenticated user |
| **Auth** | `PUT`  | `/api/Account/update-address` | Update user shipping address |
| **Products** | `GET` | `/api/Product/get-all` | Get paginated list of products with filters |
| **Products** | `GET` | `/api/Product/get-by-id/{id}` | Get product details by ID |
| **Products** | `POST` | `/api/Product/Add-Product` | Create new product with photo uploads |
| **Categories** | `GET` | `/api/Categories/get-all` | Get all product categories |
| **Categories** | `POST` | `/api/Categories/add-category` | Create new category |
| **Basket** | `GET` | `/api/Baskets/get-basket` | Retrieve customer Redis shopping cart |
| **Basket** | `POST` | `/api/Baskets/create-cart` | Create customer basket in Redis |
| **Basket** | `PUT` | `/api/Baskets/update-item` | Add / Update item in customer basket |
| **Basket** | `DELETE`| `/api/Baskets/delete-cart` | Clear / Delete customer basket |
| **Comments** | `POST` | `/api/Comments/ADD-Commnent` | Add review or comment to product |
| **Checkout** | `POST` | `/api/Checkout/Create/{id}` | Process checkout with selected delivery |

---

## 👨‍💻 Author

Developed with ❤️ by **[Ahmed Bassal](https://github.com/ahmed-Basal)**

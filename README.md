# 🎓 Student Management API

> **Zest India IT Pvt Ltd — Full Stack Developer Technical Assignment**  
> ASP.NET Core 8 Web API with JWT Authentication, SQL Server, Layered Architecture & Swagger

---

## 📋 Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Setup & Run (Local)](#setup--run-local)
- [Setup & Run (Docker)](#setup--run-docker)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Testing the API](#testing-the-api-with-swagger)
- [Running Unit Tests](#running-unit-tests)
- [Architecture](#architecture)
- [Logging](#logging)
- [Environment Variables](#environment-variables)

---

## Overview

A fully functional **Student Management REST API** built using **ASP.NET Core 8 Web API**. It supports full CRUD operations on students, secured with JWT authentication, and follows a clean 3-layer architecture (Controller → Service → Repository).

### Features
- ✅ Get all students
- ✅ Get student by ID
- ✅ Add new student
- ✅ Update student
- ✅ Delete student
- ✅ JWT Authentication & Authorization
- ✅ Global Exception Handling (Middleware)
- ✅ Structured Logging with Serilog (Console + Rolling File)
- ✅ Swagger / OpenAPI Documentation (with JWT support)
- ✅ Layered Architecture (Controller → Service → Repository)
- ✅ Entity Framework Core with SQL Server
- ✅ Seed Data (2 sample students on startup)
- ✅ Unit Tests (xUnit + Moq + FluentAssertions)
- ✅ Docker & Docker Compose support

---

## Tech Stack

| Layer            | Technology                          |
|------------------|-------------------------------------|
| Framework        | ASP.NET Core 8 Web API              |
| ORM              | Entity Framework Core 8             |
| Database         | SQL Server 2022                     |
| Authentication   | JWT Bearer (Microsoft.AspNetCore)   |
| Logging          | Serilog (Console + File)            |
| Documentation    | Swagger / Swashbuckle               |
| Testing          | xUnit, Moq, FluentAssertions        |
| Containerization | Docker, Docker Compose              |

---

## Project Structure

```
StudentManagementAPI/
│
├── StudentManagementAPI.sln
│
├── StudentManagementAPI/                    # Main API project
│   ├── Controllers/
│   │   ├── AuthController.cs               # POST /api/auth/login
│   │   └── StudentsController.cs           # CRUD /api/students
│   │
│   ├── Services/
│   │   ├── IStudentService.cs
│   │   ├── StudentService.cs               # Business logic
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs                  # JWT generation
│   │
│   ├── Repositories/
│   │   ├── IStudentRepository.cs
│   │   └── StudentRepository.cs            # EF Core data access
│   │
│   ├── Models/
│   │   └── Student.cs                      # Entity model
│   │
│   ├── DTOs/
│   │   └── StudentDtos.cs                  # Request/Response DTOs
│   │
│   ├── Data/
│   │   └── AppDbContext.cs                 # EF Core DbContext + seed
│   │
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs  # Global error handler
│   │
│   ├── Migrations/                         # EF Core migrations
│   │
│   ├── Program.cs                          # App entry, DI, pipeline
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── StudentManagementAPI.Tests/              # Unit test project
│   ├── StudentServiceTests.cs
│   └── StudentsControllerTests.cs
│
├── Dockerfile
├── docker-compose.yml
└── .gitignore
```

---

## Prerequisites

Before running locally, make sure you have:

| Tool           | Version   | Download                                          |
|----------------|-----------|---------------------------------------------------|
| .NET SDK       | 8.0+      | https://dotnet.microsoft.com/download             |
| SQL Server     | 2019/2022 | https://www.microsoft.com/en-us/sql-server        |
| Git            | Any       | https://git-scm.com/                             |
| Docker Desktop | Any       | https://www.docker.com/products/docker-desktop    |

---

## Setup & Run (Local)

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/StudentManagementAPI.git
cd StudentManagementAPI
```

### 2. Configure the Database Connection

Open `StudentManagementAPI/appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

> **Note:** If using SQL Server with username/password instead of Windows Auth:
> ```
> Server=localhost;Database=StudentManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
> ```

### 3. Apply Database Migrations

```bash
cd StudentManagementAPI
dotnet ef database update
```

This will:
- Create the `StudentManagementDB` database
- Create the `Students` table
- Insert 2 sample students (Alice Johnson & Bob Smith)

### 4. Run the Application

```bash
dotnet run
```

The API will start at:
- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:5001`
- **Swagger UI:** `http://localhost:5000` (redirects to Swagger in Development)

---

## Setup & Run (Docker)

The easiest way — no SQL Server installation needed!

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/StudentManagementAPI.git
cd StudentManagementAPI
```

### 2. Start with Docker Compose

```bash
docker-compose up --build
```

This will:
- Start a SQL Server 2022 container
- Build and start the API container
- Apply migrations automatically on startup
- Expose the API at `http://localhost:8080`

### 3. Access the API

- **Swagger UI:** `http://localhost:8080`
- **API Base URL:** `http://localhost:8080/api`

### 4. Stop the Containers

```bash
docker-compose down
```

To also remove volumes (wipes DB data):
```bash
docker-compose down -v
```

---

## API Endpoints

### 🔐 Auth

| Method | Endpoint          | Description        | Auth Required |
|--------|-------------------|--------------------|---------------|
| POST   | `/api/auth/login` | Login & get JWT    | ❌ No          |

### 👨‍🎓 Students

| Method | Endpoint             | Description          | Auth Required |
|--------|----------------------|----------------------|---------------|
| GET    | `/api/students`      | Get all students     | ✅ Yes        |
| GET    | `/api/students/{id}` | Get student by ID    | ✅ Yes        |
| POST   | `/api/students`      | Create new student   | ✅ Yes        |
| PUT    | `/api/students/{id}` | Update student       | ✅ Yes        |
| DELETE | `/api/students/{id}` | Delete student       | ✅ Yes        |

---

## Authentication

The API uses **JWT Bearer Authentication**.

### Step 1: Login to get a token

**POST** `/api/auth/login`

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Demo Credentials:**

| Username | Password   | Role  |
|----------|------------|-------|
| admin    | Admin@123  | Admin |
| user     | User@123   | User  |

**Response:**
```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "expiresAt": "2024-01-01T12:00:00Z"
  }
}
```

### Step 2: Use the token in subsequent requests

Add to request headers:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Testing the API with Swagger

1. Open `http://localhost:5000` (or `http://localhost:8080` for Docker)
2. Click **POST /api/auth/login**, click **Try it out**
3. Enter credentials and click **Execute**
4. Copy the token from the response
5. Click the **Authorize 🔒** button at the top right
6. Enter: `Bearer YOUR_TOKEN_HERE` and click **Authorize**
7. Now all student endpoints are unlocked — try them out!

### Sample Request Bodies

**Create Student (POST /api/students):**
```json
{
  "name": "John Doe",
  "email": "john.doe@example.com",
  "age": 22,
  "course": "Computer Science"
}
```

**Update Student (PUT /api/students/1):**
```json
{
  "name": "John Doe Updated",
  "email": "john.doe@example.com",
  "age": 23,
  "course": "Data Science"
}
```

### Sample API Response

```json
{
  "success": true,
  "message": "Retrieved 2 student(s).",
  "data": [
    {
      "id": 1,
      "name": "Alice Johnson",
      "email": "alice@example.com",
      "age": 21,
      "course": "Computer Science",
      "createdDate": "2024-01-15T00:00:00Z"
    }
  ]
}
```

---

## Running Unit Tests

```bash
cd StudentManagementAPI.Tests
dotnet test
```

Or from the root solution folder:
```bash
dotnet test
```

To see test output verbosely:
```bash
dotnet test --logger "console;verbosity=detailed"
```

To generate a coverage report:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Coverage

| Class                  | Tests |
|------------------------|-------|
| `StudentService`       | 8     |
| `StudentsController`   | 7     |
| **Total**              | **15**|

---

## Architecture

The project follows a clean **3-Layer Architecture**:

```
HTTP Request
    │
    ▼
┌─────────────────────────────────────┐
│         Middleware Pipeline         │  ← Global Exception Handler
│         (ExceptionHandling,         │  ← Serilog Request Logging
│          JWT Auth, CORS)            │  ← JWT Authentication
└─────────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│         Controller Layer            │  ← Route handling, validation,
│  (StudentsController, AuthController│    HTTP responses
└─────────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│          Service Layer              │  ← Business logic,
│  (StudentService, AuthService)      │    duplicate email checks
└─────────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│        Repository Layer             │  ← EF Core data access,
│  (StudentRepository)                │    SQL Server queries
└─────────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────────┐
│           SQL Server DB             │  ← StudentManagementDB
│         (Students table)            │    Students table
└─────────────────────────────────────┘
```

---

## Logging

Serilog writes logs to two sinks:

| Sink     | Location                           | Level (Dev) |
|----------|------------------------------------|-------------|
| Console  | Terminal output                    | Debug+      |
| File     | `logs/student-api-YYYYMMDD.log`    | Information+|

**Log file rolls daily.** Example log output:
```
[2024-01-15 10:30:01 INF] POST /api/auth/login responded 200 in 45.3 ms
[2024-01-15 10:30:05 INF] Service: Creating student John Doe.
[2024-01-15 10:30:05 INF] POST /api/students responded 201 in 82.1 ms
```

---

## Environment Variables

| Variable                             | Description                     | Default                              |
|--------------------------------------|---------------------------------|--------------------------------------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string  | (See appsettings.json)               |
| `Jwt__SecretKey`                     | JWT signing secret              | `ZestIndiaJwtSuperSecretKey2024!...` |
| `Jwt__Issuer`                        | JWT issuer                      | `StudentManagementAPI`               |
| `Jwt__Audience`                      | JWT audience                    | `StudentManagementClient`            |
| `Jwt__ExpiryHours`                   | Token expiry in hours           | `2`                                  |
| `ASPNETCORE_ENVIRONMENT`             | `Development` or `Production`   | `Development`                        |

---

## Database Schema

**Table: `Students`**

| Column        | Type            | Constraints              |
|---------------|-----------------|--------------------------|
| `Id`          | INT             | Primary Key, Identity    |
| `Name`        | NVARCHAR(100)   | NOT NULL                 |
| `Email`       | NVARCHAR(150)   | NOT NULL, UNIQUE         |
| `Age`         | INT             | NOT NULL, Range(1-150)   |
| `Course`      | NVARCHAR(100)   | NOT NULL                 |
| `CreatedDate` | DATETIME2       | NOT NULL, Default UTC Now|

---

## Evaluation Criteria Checklist

| Criteria           | Status | Notes                                        |
|--------------------|--------|----------------------------------------------|
| Code Quality       | ✅      | Clean code, consistent naming, SOLID         |
| Architecture       | ✅      | Controller → Service → Repository layers     |
| Error Handling     | ✅      | Global middleware + per-operation handling   |
| Security           | ✅      | JWT Auth on all student endpoints            |
| API Functionality  | ✅      | Full CRUD with proper HTTP status codes      |
| Unit Testing       | ✅      | 15 tests with Moq + FluentAssertions         |
| Docker             | ✅      | Dockerfile + docker-compose.yml              |
| Swagger Docs       | ✅      | Full OpenAPI docs with JWT UI support        |
| Logging            | ✅      | Serilog — Console + rolling file             |
| Seed Data          | ✅      | 2 sample students auto-seeded                |

---

## Submission

- 📁 **Repository:** `https://github.com/YOUR_USERNAME/StudentManagementAPI`
- 🔗 Share this link with the Zest India IT team

---

*Built for the Zest India IT Pvt Ltd Full Stack Developer Technical Assignment.*

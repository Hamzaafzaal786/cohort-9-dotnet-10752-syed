# Task Management System

A full-stack task management application built with **ASP.NET Core 7** and **React.js**, featuring user authentication, role-based authorization, and complete task management.

---

## 📌 Table of Contents

1. [Features](#-features)
2. [Technology Stack](#-technology-stack)
3. [Prerequisites](#-prerequisites)
4. [Setup Instructions](#-setup-instructions)

   * [Backend Setup](#-backend-setup)
   * [Frontend Setup](#-frontend-setup)
   * [Database Setup](#-database-setup)
5. [Running the Application](#-running-the-application)
6. [Testing](#-testing)
7. [API Documentation](#-api-documentation)
8. [Test Credentials](#-test-credentials)
9. [Project Structure](#-project-structure)
10. [Troubleshooting](#-troubleshooting)

---

## 🚀 Features

### Backend Features

* ✅ **User Authentication** - JWT-based Login/Register
* ✅ **Role-Based Authorization** - Admin and User roles
* ✅ **Task Management** - Complete CRUD operations
* ✅ **Task Assignment** - Admin can assign tasks to users
* ✅ **Global Exception Handling** - Centralized error handling
* ✅ **Serilog Logging** - File and console logging
* ✅ **Swagger Documentation** - Interactive API docs
* ✅ **SQL Database** - Entity Framework Core with SQL Server

### Frontend Features

* ✅ **Login & Register Pages** - With role selection
* ✅ **Password Requirements** - Color-coded validation
* ✅ **Dashboard** - User/Admin views with statistics
* ✅ **Task Management** - Create, Edit, Delete, View tasks
* ✅ **Task Assignment** - Admin can assign tasks to any user
* ✅ **Profile Page** - View profile and change password
* ✅ **Professional UI** - Dark navy theme, responsive design

### Testing & Quality

* ✅ **Unit Tests** - 16 passing tests (xUnit + Moq)
* ✅ **Code Coverage** - Configured with coverlet
* ✅ **SonarQube** - Code quality analysis integrated

---

## 🛠️ Technology Stack

| Category            | Technology                              |
| ------------------- | --------------------------------------- |
| **Backend**         | ASP.NET Core 7, Entity Framework Core 7 |
| **Database**        | SQL Server                              |
| **Authentication**  | JWT (JSON Web Tokens)                   |
| **Logging**         | Serilog                                 |
| **Frontend**        | React 18, React Router v6               |
| **UI Framework**    | Bootstrap, React Icons                  |
| **Testing**         | xUnit, Moq, FluentAssertions            |
| **Quality**         | SonarQube, coverlet                     |
| **Version Control** | Git, GitHub                             |

---

## 📋 Prerequisites

| Requirement   | Version                    |
| ------------- | -------------------------- |
| .NET SDK      | 7.0 or later               |
| SQL Server    | 2019 or later (or LocalDB) |
| Node.js       | 16.x or later              |
| npm           | 8.x or later               |
| Visual Studio | 2022 (or VS Code)          |
| Docker        | (Optional - for SonarQube) |

---

## 🔧 Setup Instructions

### 1️⃣ Clone the Repository

```bash
git clone https://github.com/Hamzaafzaal786/cohort-9-dotnet-10752-syed.git
cd cohort-9-dotnet-10752-syed
git checkout develop
```

---

### 2️⃣ Backend Setup

#### Step 1: Open Solution in Visual Studio

```bash
# Or open TaskManagementSystem.sln in Visual Studio
```

#### Step 2: Set Environment Variables

**Option A: Using launchSettings.json (Recommended)**

Open `src/TaskManagementSystem.API/Properties/launchSettings.json` and update:

```json
{
  "profiles": {
    "TaskManagementSystem.API": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "JWT_SECRET": "your-secret-key-min-32-characters",
        "DB_CONNECTION": "Server=localhost;Database=TaskManagementDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
      },
      "applicationUrl": "https://localhost:52994;http://localhost:52995"
    }
  }
}
```

**Option B: Using Command Line**

```bash
# Windows (PowerShell)
$env:JWT_SECRET="your-secret-key-min-32-characters"
$env:DB_CONNECTION="Server=localhost;Database=TaskManagementDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"

# Windows (CMD)
set JWT_SECRET=your-secret-key-min-32-characters
set DB_CONNECTION=Server=localhost;Database=TaskManagementDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true

# Linux/Mac
export JWT_SECRET="your-secret-key-min-32-characters"
export DB_CONNECTION="Server=localhost;Database=TaskManagementDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
```

#### Step 3: Update Connection String

If your SQL Server name is different, update the `DB_CONNECTION` environment variable:

```bash
# For SQL Server Express
Server=YOUR_SERVER_NAME\\SQLEXPRESS;Database=TaskManagementDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true

# For LocalDB
Server=(localdb)\\MSSQLLocalDB;Database=TaskManagementDB_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

#### Step 4: Run Database Migrations

```bash
cd src/TaskManagementSystem.API
dotnet ef database update
```

#### Step 5: Create Roles in Database

Run this SQL in SSMS or SQL Server Object Explorer:

```sql
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
('1', 'Admin', 'ADMIN', NEWID()),
('2', 'User', 'USER', NEWID());
```

#### Step 6: Run the Backend

```bash
dotnet run
```

**Backend will run at:** `https://localhost:52994`

---

### 3️⃣ Frontend Setup

#### Step 1: Navigate to Frontend Folder

```bash
cd frontend
```

#### Step 2: Install Dependencies

```bash
npm install
```

#### Step 3: Update API URL (If Needed)

Open `frontend/src/services/api.js` and update the `baseURL`:

```javascript
const API = axios.create({
  baseURL: 'https://localhost:52994/api',  // Match your backend URL
  headers: {
    'Content-Type': 'application/json',
  },
});
```

#### Step 4: Run the Frontend

```bash
npm start
```

**Frontend will run at:** `http://localhost:3000`

---

### 4️⃣ Database Setup (Detailed)

#### Option A: Using Visual Studio Package Manager Console

1. Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console)
2. Set **Default project** to `TaskManagementSystem.Infrastructure`
3. Run:

```powershell
Add-Migration InitialCreate
Update-Database
```

#### Option B: Using Command Line

```bash
cd src/TaskManagementSystem.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 🏃 Running the Application

### Start Backend API

```bash
cd src/TaskManagementSystem.API
dotnet run
```

### Start React Frontend

```bash
cd frontend
npm start
```

### Access the Application

| Application      | URL                               |
| ---------------- | --------------------------------- |
| **Frontend**     | `http://localhost:3000`           |
| **Backend API**  | `https://localhost:52994`         |
| **Swagger Docs** | `https://localhost:52994/swagger` |

---

## 🧪 Testing

### Run All Tests

```bash
dotnet test
```

### Run Tests with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Test Results

| Test Category   | Tests  | Status            |
| --------------- | ------ | ----------------- |
| AuthService     | 3      | ✅ Passing         |
| TaskService     | 3      | ✅ Passing         |
| TaskRepository  | 3      | ✅ Passing         |
| TasksController | 4      | ✅ Passing         |
| Integration     | 1      | ✅ Passing         |
| **Total**       | **16** | ✅ **All Passing** |

---

## 📚 API Documentation

Once the backend is running, access Swagger at:

```text
https://localhost:52994/swagger
```

### Key Endpoints

| Method | Endpoint                     | Description                          |
| ------ | ---------------------------- | ------------------------------------ |
| POST   | `/api/auth/register`         | Register a new user                  |
| POST   | `/api/auth/login`            | Login and get JWT token              |
| POST   | `/api/auth/refresh-token`    | Refresh JWT token                    |
| POST   | `/api/auth/change-password`  | Change user password                 |
| GET    | `/api/tasks`                 | Get all tasks (admin) / user's tasks |
| POST   | `/api/tasks`                 | Create a new task                    |
| PUT    | `/api/tasks`                 | Update a task                        |
| DELETE | `/api/tasks/{id}`            | Delete a task                        |
| GET    | `/api/tasks/dashboard/stats` | Get dashboard statistics             |

---

## 👤 Test Credentials

### Admin User

| Field    | Value                   |
| -------- | ----------------------- |
| Email    | `admin@taskmanager.com` |
| Password | `Admin123!`             |

### Regular Users

| Full Name  | Email                  | Password   |
| ---------- | ---------------------- | ---------- |
| John Doe   | `john@taskmanager.com` | `John123!` |
| Test User | `testuser@example.com` | `Test123!` |


---

## 📁 Project Structure

```text
cohort-9-dotnet-10752-syed/
├── src/
│   ├── TaskManagementSystem.Domain/          # Domain Layer (Entities, Enums)
│   │   ├── Common/
│   │   ├── Entities/
│   │   └── Enums/
│   ├── TaskManagementSystem.Application/     # Application Layer (DTOs, Services)
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Mappings/
│   │   └── Services/
│   ├── TaskManagementSystem.Infrastructure/  # Infrastructure Layer (Data)
│   │   ├── Data/
│   │   └── Repositories/
│   └── TaskManagementSystem.API/             # API Layer (Controllers)
│       ├── Controllers/
│       ├── Middleware/
│       └── Properties/
├── tests/
│   ├── TaskManagementSystem.UnitTests/       # Unit Tests
│   │   ├── Controllers/
│   │   ├── Repositories/
│   │   └── Services/
│   └── TaskManagementSystem.IntegrationTests/ # Integration Tests
├── frontend/                                  # React Frontend
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   └── utils/
│   └── public/
├── docs/
├── TaskManagementSystem.sln
├── coverlet.runsettings
└── README.md
```

---

## 🔧 Troubleshooting

### Issue: "JWT_SECRET environment variable is not set"

**Solution:** Set the environment variable:

```bash
# Windows PowerShell
$env:JWT_SECRET="your-secret-key-min-32-characters"

# Windows CMD
set JWT_SECRET=your-secret-key-min-32-characters

# Linux/Mac
export JWT_SECRET="your-secret-key-min-32-characters"
```

### Issue: "Cannot connect to SQL Server"

**Solution:**

1. Update the `DB_CONNECTION` environment variable with your SQL Server name
2. Make sure SQL Server is running
3. Check connection string format

### Issue: "Role User does not exist"

**Solution:** Run the SQL script to create roles:

```sql
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
('1', 'Admin', 'ADMIN', NEWID()),
('2', 'User', 'USER', NEWID());
```

### Issue: Port 3000 already in use

**Solution:**

* Press `Y` when prompted to run on another port
* Or manually kill the process using port 3000

### Issue: Node modules not installed

**Solution:**

```bash
cd frontend
npm install
```

---

## 📊 SonarQube Analysis (Optional)

### Run SonarQube Locally

```bash
# Start SonarQube with Docker
docker run -d --name sonarqube -p 9000:9000 sonarqube:lts-community

# Access at: http://localhost:9000
# Login: admin / admin
```

### Run Analysis

```bash
dotnet sonarscanner begin /k:"TaskManagementSystem" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN"
dotnet build --no-incremental
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
dotnet sonarscanner end /d:sonar.login="YOUR-TOKEN"
```

---

## 👥 Contributors

* **Hamza Afzaal** - Developer

---

## 📄 License

This project is created for educational purposes as part of the .NET Fullstack Internship assignment.

---

## 🎯 Project Status

✅ **Complete** - All features implemented and tested

| Feature        | Status                     |
| -------------- | -------------------------- |
| Backend API    | ✅ Complete                 |
| React Frontend | ✅ Complete                 |
| Database       | ✅ Complete                 |
| Authentication | ✅ Complete                 |
| Authorization  | ✅ Complete                 |
| Unit Tests     | ✅ Complete (16/16 Passing) |
| SonarQube      | ✅ Complete                 |
| Documentation  | ✅ Complete                 |

---

**🚀 Thank you for reviewing this project!**

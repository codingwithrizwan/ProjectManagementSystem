#

## Important Note About Folder Structure

After cloning this repository with `git clone`, all main folders (PMS.API, PMS.Web, PMS.Application, PMS.Domain, PMS.Infrastructure, etc.) will be directly inside the `ProjectManagementSystem` directory.

You will see:

```
ProjectManagementSystem/
  PMS.API/
  PMS.Web/
  PMS.Application/
  PMS.Domain/
  PMS.Infrastructure/
  ...
```

So, All code will be inside `ProjectManagementSystem`.
# Running the Web and API Projects
#
## Run the Web Project
- Open a terminal in the `ProjectManagementSystem` directory.
- Run:
  ```
  dotnet run --project PMS.Web
  ```
- The web app will run at: **https://localhost:7200/**

## Run the API Project
- Open a terminal in the `ProjectManagementSystem` directory.
- Run:
  ```
  dotnet run --project PMS.API
  ```
- The API will run at: **https://localhost:7064/**

## Important: Set API Base URL in Web
- In `PMS.Web/appsettings.json`, verify that the API base URL is set to `https://localhost:7064/` so the web app can call the API correctly.

## Swagger (API Documentation)
- Open in browser: **https://localhost:7064/index.html**

#
# Project Setup: Step-by-Step Guide
#
1. **Clone or Download the Repository**
2. **Configure Database Connection**
  - Open `ProjectManagementSystem/PMS.API/appsettings.json`.
  - Set your SQL Server connection string under `ConnectionStrings:DefaultConnection` (e.g., `Server=.;Database=PMS_Rizwan;Trusted_Connection=True;TrustServerCertificate=True`).
3. **Run Database Migrations (from the `ProjectManagementSystem` folder)**
   - Open a terminal and navigate to the `ProjectManagementSystem` directory:
     ```
     cd ProjectManagementSystem
     ```
   - For a fresh start (after deleting migrations):
     ```
     dotnet ef migrations add InitialCreate --project PMS.Infrastructure --startup-project PMS.API
     ```
   - Otherwise, just update the database:
     ```
     dotnet ef database update --project PMS.Infrastructure --startup-project PMS.API
     ```
4. **Run the API**
  - Start the API project (`PMS.API`).
  - The Admin user will be seeded automatically (see `AdminSeed` in `appsettings.json`).
5. **Run the Web Project**
  - Start the Razor Pages web project (`PMS.Web`).
6. **Register Employees**
  - Only Employee registration is available via the UI/API. Admin credentials are seeded.
7. **Login and Use the System**
  - Admin: Use seeded credentials to login and manage projects/tasks.
  - Employee: Register and login to view/update assigned tasks.

#
# API Endpoints Summary Notes
#
## Authentication
- `POST /api/Auth/register` — Register a new Employee
- `POST /api/Auth/login` — Login and receive JWT

## Projects
- `POST /api/Projects/create-project` — Create a new project (Admin)
- `GET /api/Projects` — Get all projects with progress and task count (Admin, Dashboard)
- `GET /api/Projects/{id}` — Get project details with all tasks (Admin)

## Tasks
- `POST /api/Projects/create-task` — Create and assign a task to Employee (Admin)
- `PUT /api/Projects/update-task-status` — Update a task’s status (Employee)
- `GET /api/Projects/my-tasks` — Get all tasks assigned to the logged-in employee (Employee)
- `GET /api/Projects/my-tasks/{taskId}` — Get details of a specific assigned task (Employee)

#
# Assessment Requirements & Implementation Notes
#
## Project Overview
Admins manage projects and tasks; Employees execute and update tasks. The system tracks task lifecycle timestamps and provides project progress metrics.

## Technical Stack
- ASP.NET Core (.NET 10 LTS)
- Razor Pages (UI)
- RESTful API
- EF Core with LINQ
- DDD & CQRS

## Functional Requirements
- **Admin:** Create projects, create/assign tasks
- **Employee:** View assigned tasks, update status
- **Task Assignment:** Must assign employee at creation
- **Timestamps:**
  - CreatedAt: On creation
  - StartedAt: When status is "In Progress"
  - CompletedAt: When status is "Done"
- **Progress Calculation:**
  - Project: % completed tasks
  - Overall: Average across all projects
  - Logic in Domain layer

## Screen Requirements
1. Authentication (Login/Register)
2. Dashboard: Total projects, average progress, project list with stats
3. Project Detail: All tasks, status, employee, timestamps

## API Requirements
- Create Project
- Create Task
- Update Task/Status

## Architectural Standards
- DDD principles
- CQRS (Commands/Queries separated)

## Bonus Points
- **Caching:** In-memory caching for heavy operations (implemented for GetAllProjects And AllTask api only)
- **OAuth2 (Keycloak):** Not implemented

## Deliverables
- Source code (clean repo)
- Database migrations
- Focus: DDD, CQRS, clean architecture
---
#
# Environment Information
#
- **Database:** Microsoft SQL Server 2022 | 16.0.1000.6
- **API:** .NET 10 (LTS)
- **Web:** Razor Pages

# Project Management Mini-System API Documentation

## Overview
RESTful API endpoints for managing projects, tasks, and task statuses. Designed for Clean Architecture, DDD, and CQRS.

---

## Authentication

- **POST** `/api/Auth/register`
  - Register a new Employee user. Admin is seeded automatically and cannot be registered via this endpoint.
- **POST** `/api/Auth/login`
  - Authenticate and receive a JWT token.

---

## Projects

- **POST** `/api/Projects/create-project`
  - Create a new project.
  - **Body:**
    ```json
    {
      "name": "string",
      "description": "string"
    }
    ```
  - **Role:** Admin

- **GET** `/api/Projects`
  - Get all projects with progress and task count.
  - **Role:** Admin

- **GET** `/api/Projects/{id}`
  - Get project details, including all tasks.
  - **Role:** Admin

---

## Tasks

- **POST** `/api/Projects/create-task`
  - Create and assign a task to an employee.
  - **Body:**
    ```json
    {
      "projectId": long,
      "title": "string",
      "description": "string",
      "assignedEmployeeId": long
    }
    ```
  - **Role:** Admin

- **PUT** `/api/Projects/update-task-status`
  - Update a task’s status (e.g., To Do → In Progress → Done).
  - **Body:**
    ```json
    {
      "taskId": long,
      "status": int // 0: To Do, 1: In Progress, 2: Done
    }
    ```
  - **Role:** Employee

- **GET** `/api/Projects/my-tasks`
  - Get all tasks assigned to the logged-in employee.
  - **Role:** Employee

- **GET** `/api/Projects/my-tasks/{taskId}`
  - Get details of a specific assigned task.
  - **Role:** Employee

---


## Dashboard

- **GET** `/api/Projects`
  - Get all projects with progress and task count. Use this endpoint for dashboard statistics (total projects, average progress rate, etc.).
  - **Role:** Admin

---



## Admin Seeding & Registration Note

- The Admin user is seeded automatically when you run database migrations and start the API. Admin credentials are set in the application seeding logic and are not exposed via the registration UI/API.
- Only Employee registration is available through the registration endpoint and UI. Admins must log in with the seeded credentials.


---

## appsettings.json & Database Setup

### appsettings.json Keys

- **ConnectionStrings**: Set your SQL Server connection string here. Example:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=PMS_Rizwan;Trusted_Connection=True;TrustServerCertificate=True"
  }
  ```
- **JwtSettings**: JWT authentication settings (secret key, issuer, audience, token expiry).
- **AdminSeed**: Credentials for the default Admin user seeded on first migration and API run.

### Database Migration & Seeding

1. **If you deleted all migrations and want a fresh start:**
   - Run:
     ```
     dotnet ef migrations add InitialCreate --project PMS.Infrastructure --startup-project PMS.API
     ```
2. **Otherwise, just update the database:**
   - Run:
     ```
     dotnet ef database update --project PMS.Infrastructure --startup-project PMS.API
     ```
3. **Before running migrations, make sure your connection string is set in appsettings.json as shown above.**
4. **When the database is created and the API runs, the Admin user will be seeded automatically.**
5. **Only Employee registration is available via the UI/API.**

---

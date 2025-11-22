# IcompCare API

Backend service for the Academic Support System (Sistema de Apoio Acadêmico) of ICOMP/UFAM.

## Technologies

- **.NET 8**
- **PostgreSQL**
- **Swagger / OpenAPI** for documentation

## Project Structure

The solution follows a Clean Architecture approach:

- **`src/IcompCare.Api`**: The entry point, API controllers, and configuration.
- **`src/IcompCare.Application`**: Business logic, services, and use cases.
- **`src/IcompCare.Domain`**: Core entities, interfaces, and business rules.
- **`src/IcompCare.Infrastructure`**: Data access, database context, and external services implementation.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/)

### Running with Dev Container (Recommended)

This project is configured to run inside a VS Code Dev Container which sets up .NET 8 and PostgreSQL automatically.

1. Open the project in VS Code.
2. Click "Reopen in Container" when prompted (or use the Command Palette: `Dev Containers: Reopen in Container`).
3. The environment will be set up with all dependencies.

### Running Manually

1. Navigate to the API project folder:

   ```bash
   cd src/IcompCare.Api
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

The API will start, typically on `http://localhost:5000` or `https://localhost:5001`.

### Running with Docker

You can also run the API as a Docker container.

1. **Build the Docker image:**

   Run this command from the workspace root (`/workspaces/ufam-tcc`):

   ```bash
   docker build -t icompcare-api -f code/api/Dockerfile code/api
   ```

2. **Run the container:**

   Use the `--name` flag to give your container a specific name (e.g., `icompcare-api-container`).

   ```bash
   docker run --name icompcare-api-container -p 8080:8080 icompcare-api
   ```

   The API will be available at `http://localhost:8080`.

   **Note on Database Connection:**
   By default, the application tries to connect to a host named `db`. If you are running the container standalone, you need to override the connection string to point to your actual database host (e.g., `host.docker.internal` if running locally).

   ```bash
   docker run --name icompcare-api-container -p 8080:8080 -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Database=appdb;Username=postgres;Password=password" icompcare-api
   ```

## API Documentation

- **Swagger UI**: Interactive API documentation is available at `/swagger` (e.g., `http://localhost:5000/swagger`) when running in Development mode.
- **Health Check**: A health check endpoint is available at `/health` to verify the service status.
